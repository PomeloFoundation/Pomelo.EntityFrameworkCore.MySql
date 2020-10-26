// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlSqlTranslatingExpressionVisitor : RelationalSqlTranslatingExpressionVisitor
    {
        private readonly IMySqlJsonPocoTranslator _jsonPocoTranslator;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlSqlTranslatingExpressionVisitor(
            RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor,
            [CanBeNull] IMySqlJsonPocoTranslator jsonPocoTranslator)
            : base(dependencies, queryCompilationContext, queryableMethodTranslatingExpressionVisitor)
        {
            _jsonPocoTranslator = jsonPocoTranslator;
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)Dependencies.SqlExpressionFactory;
        }

        /// <inheritdoc />
        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType == ExpressionType.ArrayLength)
            {
                if (TranslationFailed(unaryExpression.Operand, Visit(unaryExpression.Operand), out var sqlOperand))
                {
                    return null;
                }

                if (sqlOperand.Type == typeof(byte[]) &&
                    (sqlOperand.TypeMapping == null || sqlOperand.TypeMapping is MySqlByteArrayTypeMapping))
                {
                    return _sqlExpressionFactory.NullableFunction(
                        "LENGTH",
                        new[] {sqlOperand},
                        typeof(int));
                }

                return _jsonPocoTranslator?.TranslateArrayLength(sqlOperand);
            }


            // Make explicit casts implicit if they are applied to a JSON traversal object.
            // It is pretty common for Newtonsoft.Json objects to be cast to other types (e.g. casting from
            // JToken to JArray to check an arrays length via the JContainer.Count property).
            if (unaryExpression.NodeType == ExpressionType.Convert ||
                unaryExpression.NodeType == ExpressionType.ConvertChecked)
            {
                var visitedOperand = Visit(unaryExpression.Operand);
                if (visitedOperand is MySqlJsonTraversalExpression traversal)
                {
                    return unaryExpression.Type == typeof(object)
                        ? traversal
                        : traversal.Clone(
                            traversal.ReturnsText,
                            unaryExpression.Type,
                            Dependencies.TypeMappingSource.FindMapping(unaryExpression.Type));
                }

                ResetTranslationErrorDetails();
            }

            var visitedExpression = base.VisitUnary(unaryExpression);
            if (visitedExpression == null)
            {
                return null;
            }

            // MySQL implicitly casts numbers used in BITWISE NOT operations (~ operator) to BIGINT UNSIGNED.
            // We need to cast them back, to get the expected result.
            if (visitedExpression is SqlUnaryExpression sqlUnaryExpression &&
                sqlUnaryExpression.OperatorType == ExpressionType.Not &&
                sqlUnaryExpression.Type != typeof(bool))
            {
                return _sqlExpressionFactory.Convert(
                    sqlUnaryExpression,
                    sqlUnaryExpression.Type,
                    sqlUnaryExpression.TypeMapping);
            }

            return visitedExpression;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType == ExpressionType.ArrayIndex)
            {
                if (TranslationFailed(binaryExpression.Left, Visit(TryRemoveImplicitConvert(binaryExpression.Left)), out var sqlLeft)
                    || TranslationFailed(binaryExpression.Right, Visit(TryRemoveImplicitConvert(binaryExpression.Right)), out var sqlRight))
                {
                    return null;
                }

                // Try translating ArrayIndex inside json column
                var expression = _jsonPocoTranslator?.TranslateMemberAccess(
                    sqlLeft,
                    _sqlExpressionFactory.JsonArrayIndex(sqlRight),
                    binaryExpression.Type);

                if (expression != null)
                {
                    return expression;
                }
            }

            var visitedExpression = (SqlExpression)base.VisitBinary(binaryExpression);
            if (visitedExpression == null)
            {
                return null;
            }

            if (visitedExpression is SqlBinaryExpression visitedBinaryExpression)
            {
                // Returning null forces client projection.
                // CHECK: Is this still true in .NET Core 3.0?
                switch (visitedBinaryExpression.OperatorType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.Subtract:
                    case ExpressionType.Multiply:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                        return IsDateTimeBasedOperation(visitedBinaryExpression)
                            ? null
                            : visitedBinaryExpression;
                }
            }

            return visitedExpression;
        }

        private static bool IsDateTimeBasedOperation(SqlBinaryExpression binaryExpression)
        {
            if (binaryExpression.TypeMapping != null
                && (binaryExpression.TypeMapping.StoreType.StartsWith("date") || binaryExpression.TypeMapping.StoreType.StartsWith("time")))
            {
                return true;
            }

            return false;
        }

        protected virtual void ResetTranslationErrorDetails()
        {
            // When we try translating an expression and later decide that we want to discard the result, we need to remove any translation
            // error details, or those details might end up more than once in generated exceptions down the stack.
            //
            // We use a workaround here, that will result in the TranslationErrorDetails being set to `null` again.
            // Otherwise, we would need to override  TranslationErrorDetails, AddTranslationErrorDetails and Translate, reimplement the
            // TranslationErrorDetails functionality and maintain everything just to support resetting the TranslationErrorDetails property.
            base.Translate(Expression.Constant(0));
        }

        #region Copied from RelationalSqlTranslatingExpressionVisitor

        private static Expression TryRemoveImplicitConvert(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
            {
                if (unaryExpression.NodeType == ExpressionType.Convert
                    || unaryExpression.NodeType == ExpressionType.ConvertChecked)
                {
                    var innerType = unaryExpression.Operand.Type.UnwrapNullableType();
                    if (innerType.IsEnum)
                    {
                        innerType = Enum.GetUnderlyingType(innerType);
                    }
                    var convertedType = unaryExpression.Type.UnwrapNullableType();

                    if (innerType == convertedType
                        || (convertedType == typeof(int)
                            && (innerType == typeof(byte)
                                || innerType == typeof(sbyte)
                                || innerType == typeof(char)
                                || innerType == typeof(short)
                                || innerType == typeof(ushort))))
                    {
                        return TryRemoveImplicitConvert(unaryExpression.Operand);
                    }
                }
            }

            return expression;
        }

        [DebuggerStepThrough]
        private bool TranslationFailed(Expression original, Expression translation, out SqlExpression castTranslation)
        {
            if (original != null && !(translation is SqlExpression))
            {
                castTranslation = null;
                return true;
            }

            castTranslation = translation as SqlExpression;
            return false;
        }

        #endregion Copied from RelationalSqlTranslatingExpressionVisitor
    }
}
