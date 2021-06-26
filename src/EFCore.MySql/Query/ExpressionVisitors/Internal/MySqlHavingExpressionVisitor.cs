// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
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
                havingExpression is not SqlConstantExpression)
            {
                _containsAggregateFunctionExpressionVisitor ??= new MySqlContainsAggregateFunctionExpressionVisitor();
                if (!_containsAggregateFunctionExpressionVisitor.Process(havingExpression))
                {
                    var alias = GetUniqueAlias(selectExpression.Projection);

                    // Hack around the fact, that EF Core does not allow us to add our own ProjectionExpression with an alias.
                    // We can however indirectly control the alias, by first setting a ColumnExpression, whose name is then used as the base
                    // for the alias, and then update it afterwards with the actual expression we are interested in, when it gets visited.
                    selectExpression.AddToProjection(
                        new HatSwappingColumnExpression(
                            havingExpression,
                            alias,
                            havingExpression.Type,
                            havingExpression.TypeMapping));

                    var columnAliasReferenceExpression = _sqlExpressionFactory.ColumnAliasReference(
                        alias,
                        havingExpression,
                        havingExpression.Type,
                        havingExpression.TypeMapping);

                    // Having expressions, not containing an aggregate function, need to be part of the GROUP BY clause, because they now also
                    // appear as part of the SELECT clause.
                    var groupBy = selectExpression.GroupBy.ToList();
                    groupBy.Add(columnAliasReferenceExpression);

                    selectExpression = selectExpression.Update(
                        selectExpression.Projection,
                        selectExpression.Tables,
                        selectExpression.Predicate,
                        groupBy,
                        columnAliasReferenceExpression,
                        selectExpression.Orderings,
                        selectExpression.Limit,
                        selectExpression.Offset);
                }
            }

            return base.VisitExtension(selectExpression);
        }

        private static string GetUniqueAlias(IReadOnlyCollection<ProjectionExpression> projection)
        {
            const string baseAlias = "having";

            var counter = 0;
            var currentAlias = baseAlias;

            while (projection.Any(pe => string.Equals(pe.Alias, currentAlias, StringComparison.OrdinalIgnoreCase)))
            {
                currentAlias = $"{baseAlias}{counter++}";
            }

            return currentAlias;
        }

        /// <summary>
        /// This is just a magic wrapper for an arbitrary SqlExpression, posing as a ColumnExpression.
        /// Returns the inner SqlExpression, when visited.
        /// </summary>
        private sealed class HatSwappingColumnExpression : ColumnExpression
        {
            private readonly SqlExpression _expression;

            public HatSwappingColumnExpression(
                SqlExpression expression,
                [CanBeNull] string name,
                [NotNull] Type type,
                [CanBeNull] RelationalTypeMapping typeMapping)
                : base(type, typeMapping)
            {
                _expression = expression;
                Name = name;
            }

            public override ColumnExpression MakeNullable()
                => this;

            public override string Name { get; }
            public override TableExpressionBase Table { get; }
            public override string TableAlias { get; }
            public override bool IsNullable { get; }

            protected override Expression VisitChildren(ExpressionVisitor visitor)
                => _expression; // does not return itself or a copy of itself, but the inner expression instead

            public override bool Equals(object obj)
                => Equals(obj as HatSwappingColumnExpression);

            public bool Equals(HatSwappingColumnExpression other)
                => ReferenceEquals(this, other) ||
                   other != null &&
                   base.Equals(other) &&
                   Equals(_expression, other._expression);

            public override int GetHashCode()
                => HashCode.Combine(base.GetHashCode(), _expression);

            protected override void Print(ExpressionPrinter expressionPrinter)
                => expressionPrinter.Visit(_expression);

            public override string ToString()
                => _expression.ToString();
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
