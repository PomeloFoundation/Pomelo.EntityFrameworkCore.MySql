using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlSqlTranslatingExpressionVisitor : RelationalSqlTranslatingExpressionVisitor
    {
        private static readonly HashSet<string> _dateTimeDataTypes
            = new HashSet<string>
            {
                "time",
                "date",
                "datetime",
                "datetime2",
                "datetimeoffset"
            };

        private static readonly HashSet<ExpressionType> _arithmeticOperatorTypes
            = new HashSet<ExpressionType>
            {
                ExpressionType.Add,
                ExpressionType.Subtract,
                ExpressionType.Multiply,
                ExpressionType.Divide,
                ExpressionType.Modulo,
            };

        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlSqlTranslatingExpressionVisitor(
            RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
            IModel model,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
            : base(dependencies, model, queryableMethodTranslatingExpressionVisitor)
        {
            _sqlExpressionFactory = dependencies.SqlExpressionFactory;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            var visitedExpression = (SqlExpression)base.VisitBinary(binaryExpression);

            if (visitedExpression == null)
            {
                return null;
            }

            if (visitedExpression is SqlBinaryExpression visitedBinaryExpression)
            {
                // Returning null forces client projection.
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

        public override SqlExpression TranslateLongCount(Expression expression = null)
        {
            // TODO: Translate Count with predicate for GroupBy
            return _sqlExpressionFactory.ApplyDefaultTypeMapping(
                _sqlExpressionFactory.Function("COUNT_BIG", new[] { _sqlExpressionFactory.Fragment("*") }, typeof(long)));
        }

        private static string GetProviderType(SqlExpression expression)
        {
            return expression.TypeMapping?.StoreType;
        }
    }
}
