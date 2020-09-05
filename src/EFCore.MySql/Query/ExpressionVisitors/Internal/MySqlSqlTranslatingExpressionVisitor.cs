using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlSqlTranslatingExpressionVisitor : RelationalSqlTranslatingExpressionVisitor
    {
        private readonly MySqlJsonPocoTranslator _jsonPocoTranslator;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        //private readonly RelationalTypeMapping _jsonTypeMapping;

        public MySqlSqlTranslatingExpressionVisitor(
            RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
            IModel model,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
            : base(dependencies, model, queryableMethodTranslatingExpressionVisitor)
        {
            _jsonPocoTranslator = ((MySqlMemberTranslatorProvider)Dependencies.MemberTranslatorProvider).JsonPocoTranslator;
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)Dependencies.SqlExpressionFactory;
            //_jsonTypeMapping = _sqlExpressionFactory.FindMapping("json");
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

                return _jsonPocoTranslator.TranslateArrayLength(sqlOperand);
            }

            return base.VisitUnary(unaryExpression);
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
                return _jsonPocoTranslator.TranslateMemberAccess(
                    sqlLeft,
                    _sqlExpressionFactory.JsonArrayIndex(sqlRight),
                    binaryExpression.Type);
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

                // The following section might not be needed. Lets keep it for a bit, until we are sure.
                /*
                // When comparing a JSON value to some string value or when assigning a string value
                // to a JSON column, convert the string value to JSON first.
                // Also explicitly convert parameter instances to JSON.
                if (visitedBinaryExpression.Left.TypeMapping?.StoreType == "json" &&
                    visitedBinaryExpression.Right.TypeMapping?.StoreType != "json" &&
                    visitedBinaryExpression.Right.TypeMapping?.ClrType == typeof(string))
                {
                    visitedExpression = _sqlExpressionFactory.MakeBinary(
                        visitedBinaryExpression.OperatorType,
                        visitedBinaryExpression.Left,
                        _sqlExpressionFactory.Convert(
                            visitedBinaryExpression.Right,
                            visitedBinaryExpression.Right.Type,
                            _jsonTypeMapping),
                        _jsonTypeMapping);
                }
                else if (visitedBinaryExpression.Right.TypeMapping?.StoreType == "json" &&
                         visitedBinaryExpression.Left.TypeMapping?.StoreType != "json" &&
                         visitedBinaryExpression.Left.TypeMapping?.ClrType == typeof(string) &&
                         visitedBinaryExpression.OperatorType != ExpressionType.Assign)
                {
                    visitedExpression = _sqlExpressionFactory.MakeBinary(
                        visitedBinaryExpression.OperatorType,
                        _sqlExpressionFactory.Convert(
                            visitedBinaryExpression.Left,
                            visitedBinaryExpression.Left.Type,
                            _jsonTypeMapping),
                        visitedBinaryExpression.Right,
                        _jsonTypeMapping);
                }
                */
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
