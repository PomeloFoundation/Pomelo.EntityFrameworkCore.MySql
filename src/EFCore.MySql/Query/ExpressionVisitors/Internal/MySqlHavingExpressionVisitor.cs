// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlHavingExpressionVisitor : ExpressionVisitor
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private MySqlContainsAggregateFunctionExpressionVisitor _containsAggregateFunctionExpressionVisitor;

        public MySqlHavingExpressionVisitor(MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SelectExpression selectExpression => VisitSelect(selectExpression),
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(
                    Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            // MySQL & MariaDB currently do not support complex expressions in HAVING clauses (e.g. function calls).
            // Instead, they want you to reference SELECT aliases for those expressions in the HAVING clause.
            // See https://bugs.mysql.com/bug.php?id=103961
            // This is only an issue for HAVING expressions that do not contain any aggregate functions.
            var havingExpression = selectExpression.Having;
            if (havingExpression is not null &&
                havingExpression is not SqlConstantExpression &&
                havingExpression is not MySqlColumnAliasReferenceExpression)
            {
                _containsAggregateFunctionExpressionVisitor ??= new MySqlContainsAggregateFunctionExpressionVisitor();
                if (!_containsAggregateFunctionExpressionVisitor.ProcessUntilSelect(havingExpression))
                {
                    selectExpression.PushdownIntoSubquery();
                    var subQuery = (SelectExpression) selectExpression.Tables.Single();

                    var projectionIndex = subQuery.AddToProjection(havingExpression);
                    var alias = subQuery.Projection[projectionIndex].Alias;

                    var columnAliasReferenceExpression = _sqlExpressionFactory.ColumnAliasReference(
                        alias,
                        havingExpression,
                        havingExpression.Type,
                        havingExpression.TypeMapping);

                    // Having expressions, not containing an aggregate function, need to be part of the GROUP BY clause, because they now also
                    // appear as part of the SELECT clause.
                    var groupBy = subQuery.GroupBy.ToList();
                    groupBy.Add(columnAliasReferenceExpression);

                    subQuery = subQuery.Update(
                        subQuery.Tables,
                        subQuery.Predicate,
                        groupBy,
                        columnAliasReferenceExpression,
                        subQuery.Projection,
                        subQuery.Orderings,
                        subQuery.Limit,
                        subQuery.Offset);

                    selectExpression = selectExpression.Update(
                        new[] {subQuery},
                        selectExpression.Predicate,
                        selectExpression.GroupBy,
                        selectExpression.Having,
                        selectExpression.Projection,
                        selectExpression.Orderings,
                        selectExpression.Limit,
                        selectExpression.Offset);
                }
            }

            return base.VisitExtension(selectExpression);
        }
    }
}
