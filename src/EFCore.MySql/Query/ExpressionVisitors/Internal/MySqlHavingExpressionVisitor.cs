// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
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
                if (!_containsAggregateFunctionExpressionVisitor.Process(havingExpression))
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
                        subQuery.Projection,
                        subQuery.Tables,
                        subQuery.Predicate,
                        groupBy,
                        columnAliasReferenceExpression,
                        subQuery.Orderings,
                        subQuery.Limit,
                        subQuery.Offset);

                    selectExpression = selectExpression.Update(
                        selectExpression.Projection,
                        new[] {subQuery},
                        selectExpression.Predicate,
                        selectExpression.GroupBy,
                        selectExpression.Having,
                        selectExpression.Orderings,
                        selectExpression.Limit,
                        selectExpression.Offset);
                }
            }

            return base.VisitExtension(selectExpression);
        }

        /// <summary>
        /// Looks for aggregate functions (like SUM(), AVG() etc.) in an expression tree, but not in subqueries.
        /// </summary>
        private sealed class MySqlContainsAggregateFunctionExpressionVisitor : ExpressionVisitor
        {
            // See https://dev.mysql.com/doc/refman/8.0/en/aggregate-functions.html
            private static readonly SortedSet<string> _aggregateFunctions = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AVG",
                "BIT_AND",
                "BIT_OR",
                "BIT_XOR",
                "COUNT",
                "GROUP_CONCAT",
                "JSON_ARRAYAGG",
                "JSON_OBJECTAGG",
                "MAX",
                "MIN",
                "STD",
                "STDDEV",
                "STDDEV_POP",
                "STDDEV_SAMP",
                "SUM",
                "VAR_POP",
                "VAR_SAMP",
                "VARIANCE",
            };

            public bool AggregateFunctionFound { get; private set; }

            public bool Process(Expression node)
            {
                // Can be reused within the same thread.
                AggregateFunctionFound = false;

                Visit(node);

                return AggregateFunctionFound;
            }

            public override Expression Visit(Expression node)
                => AggregateFunctionFound
                    ? node
                    : base.Visit(node);

            protected override Expression VisitExtension(Expression extensionExpression)
                => extensionExpression switch
                {
                    SqlFunctionExpression sqlFunctionExpression => VisitSqlFunction(sqlFunctionExpression),
                    SelectExpression selectExpression => selectExpression,
                    ShapedQueryExpression shapedQueryExpression => shapedQueryExpression,
                    _ => base.VisitExtension(extensionExpression)
                };

            private Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
            {
                if (_aggregateFunctions.Contains(sqlFunctionExpression.Name))
                {
                    AggregateFunctionFound = true;
                    return sqlFunctionExpression;
                }

                return base.VisitExtension(sqlFunctionExpression);
            }
        }
    }
}
