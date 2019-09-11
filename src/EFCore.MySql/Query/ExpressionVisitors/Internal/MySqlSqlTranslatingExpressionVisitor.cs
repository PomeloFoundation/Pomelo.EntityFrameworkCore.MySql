using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlSqlTranslatingExpressionVisitor : SqlTranslatingExpressionVisitor
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlSqlTranslatingExpressionVisitor(
            [NotNull] SqlTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] RelationalQueryModelVisitor queryModelVisitor,
            [NotNull] SelectExpression targetSelectExpression = null,
            [NotNull] Expression topLevelPredicate = null,
            bool inProjection = false)
            : base(dependencies, queryModelVisitor, targetSelectExpression, topLevelPredicate, inProjection)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            var visitedExpression = base.VisitBinary(binaryExpression);

            if (visitedExpression == null)
            {
                return null;
            }

            // Returning null forces client projection.
            switch (visitedExpression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                    return IsDateTimeBasedOperation(visitedExpression)
                        ? null
                        : visitedExpression;
            }

            return visitedExpression;
        }

        private static bool IsDateTimeBasedOperation(Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                var typeMapping = InferTypeMappingFromColumn(binaryExpression.Left)
                                  ?? InferTypeMappingFromColumn(binaryExpression.Right);

                if (typeMapping != null
                    && (typeMapping.StoreType.StartsWith("date") || typeMapping.StoreType.StartsWith("time")))
                {
                    return true;
                }
            }

            return false;
        }

        private static RelationalTypeMapping InferTypeMappingFromColumn(Expression expression)
            => expression.FindProperty(expression.Type)?.FindRelationalMapping();
    }
}
