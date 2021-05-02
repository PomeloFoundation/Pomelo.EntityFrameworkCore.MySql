// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Internal;
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

        protected static readonly MethodInfo[] NewArrayExpressionSupportMethodInfos = Array.Empty<MethodInfo>()
            .Concat(typeof(MySqlDbFunctionsExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(MySqlDbFunctionsExtensions.Match)))
            .Concat(typeof(string).GetRuntimeMethods().Where(m => m.Name == nameof(string.Concat)))
            .Where(m => m.GetParameters().Any(p => p.ParameterType.IsArray))
            .ToArray();

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
                    return QueryCompilationContext.NotTranslatedExpression;
                }

                if (sqlOperand.Type == typeof(byte[]) &&
                    (sqlOperand.TypeMapping is null or MySqlByteArrayTypeMapping))
                {
                    return _sqlExpressionFactory.NullableFunction(
                        "LENGTH",
                        new[] {sqlOperand},
                        typeof(int));
                }

                return _jsonPocoTranslator?.TranslateArrayLength(sqlOperand) ??
                       QueryCompilationContext.NotTranslatedExpression;
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

            if (visitedExpression is SqlUnaryExpression sqlUnaryExpression &&
                sqlUnaryExpression.OperatorType == ExpressionType.Not &&
                sqlUnaryExpression.Type != typeof(bool))
            {
                // MySQL implicitly casts numbers used in BITWISE NOT operations (~ operator) to BIGINT UNSIGNED.
                // We need to cast them back, to get the expected result.
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
                    return QueryCompilationContext.NotTranslatedExpression;
                }

                if (binaryExpression.Left.Type == typeof(byte[]))
                {
                    var left = Visit(binaryExpression.Left);
                    var right = Visit(binaryExpression.Right);

                    if (left is SqlExpression leftSql &&
                        right is SqlExpression rightSql)
                    {
                        return _sqlExpressionFactory.NullableFunction(
                            "ASCII",
                            new[]
                            {
                                _sqlExpressionFactory.NullableFunction(
                                    "SUBSTRING",
                                    new[]
                                    {
                                        leftSql, Dependencies.SqlExpressionFactory.Add(
                                            Dependencies.SqlExpressionFactory.ApplyDefaultTypeMapping(rightSql),
                                            Dependencies.SqlExpressionFactory.Constant(1)),
                                        Dependencies.SqlExpressionFactory.Constant(1)
                                    },
                                    typeof(byte[]))
                            },
                            typeof(byte));
                    }
                }

                // Try translating ArrayIndex inside json column
                var expression = _jsonPocoTranslator?.TranslateMemberAccess(
                    sqlLeft,
                    _sqlExpressionFactory.JsonArrayIndex(sqlRight),
                    binaryExpression.Type);

                if (expression is not null)
                {
                    return expression;
                }
            }

            var visitedExpression = base.VisitBinary(binaryExpression);

            if (visitedExpression is SqlBinaryExpression visitedBinaryExpression)
            {
                // TODO: Is this still true in .NET Core 3.0?
                switch (visitedBinaryExpression.OperatorType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.Subtract:
                    case ExpressionType.Multiply:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                        return IsDateTimeBasedOperation(visitedBinaryExpression)
                            ? QueryCompilationContext.NotTranslatedExpression
                            : visitedBinaryExpression;
                }
            }

            return visitedExpression;
        }

        protected virtual Expression VisitMethodCallNewArray(NewArrayExpression newArrayExpression)
        {
            // Needed for MySqlDbFunctionsExtensions.Match() and String.Concat() translation.
            if (newArrayExpression.Type == typeof(string[]))
            {
                return _sqlExpressionFactory.ComplexFunctionArgument(
                    newArrayExpression.Expressions.Select(e => (SqlExpression)Visit(e))
                        .ToArray(),
                    ", ",
                    typeof(string[]));
            }

            // Needed for String.Concat() translation.
            if (newArrayExpression.Type == typeof(object[]))
            {
                var typeMapping = ((MySqlStringTypeMapping)Dependencies.TypeMappingSource.GetMapping(typeof(string))).Clone(forceToString: true);
                return _sqlExpressionFactory.ComplexFunctionArgument(
                    newArrayExpression.Expressions.Select(e => Dependencies.SqlExpressionFactory.ApplyTypeMapping((SqlExpression)Visit(e), typeMapping))
                        .ToArray(),
                    ", ",
                    typeof(object[]),
                    typeMapping);
            }

            return base.VisitNewArray(newArrayExpression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (NewArrayExpressionSupportMethodInfos.Contains(methodCallExpression.Method))
            {
                var arguments = new Expression[methodCallExpression.Arguments.Count];
                for (var i = 0; i < arguments.Length; i++)
                {
                    var argument = methodCallExpression.Arguments[i];

                    if (argument is NewArrayExpression newArrayExpression)
                    {
                        if (TranslationFailed(argument, VisitMethodCallNewArray(newArrayExpression), out var sqlExpression))
                        {
                            return QueryCompilationContext.NotTranslatedExpression;
                        }

                        arguments[i] = sqlExpression;
                    }
                    else
                    {
                        arguments[i] = argument;
                    }
                }

                methodCallExpression = methodCallExpression.Update(methodCallExpression.Object, arguments);
            }

            var result = base.VisitMethodCall(methodCallExpression);
            if (result == QueryCompilationContext.NotTranslatedExpression &&
                MySqlStringComparisonMethodTranslator.StringComparisonMethodInfos.Any(m => m == methodCallExpression.Method))
            {
                var message = MySqlStrings.QueryUnableToTranslateMethodWithStringComparison(
                    methodCallExpression.Method.DeclaringType.Name,
                    methodCallExpression.Method.Name,
                    nameof(MySqlDbContextOptionsBuilder.EnableStringComparisonTranslations));

                // EF Core returns an error message on its own, when the string.Equals() methods (static and non-static) are being used with
                // a `StringComparison` parameter.
                // Since we also support other translations, but all of them only when opted in, we will replace the EF Core error message
                // with our own, that is more appropriate for our case.
                if (TranslationErrorDetails.Contains(CoreStrings.QueryUnableToTranslateStringEqualsWithStringComparison))
                {
                    var translationErrorDetails = TranslationErrorDetails;
                    ResetTranslationErrorDetails();
                    message = translationErrorDetails.Replace(CoreStrings.QueryUnableToTranslateStringEqualsWithStringComparison, message);
                }

                AddTranslationErrorDetails(message);
            }

            return result;
        }

        private static bool IsDateTimeBasedOperation(SqlBinaryExpression binaryExpression)
        {
            if (binaryExpression.TypeMapping is RelationalTypeMapping typeMapping &&
                (typeMapping.StoreType.StartsWith("date") || typeMapping.StoreType.StartsWith("time")))
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
            if (original != null && translation is not SqlExpression)
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
