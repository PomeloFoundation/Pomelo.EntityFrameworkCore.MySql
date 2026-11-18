// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Microting.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     Converts <c>OUTER APPLY</c> / <c>CROSS APPLY</c> to <c>LEFT JOIN</c> / <c>INNER JOIN</c> for servers without <c>LATERAL</c>
    ///     support (MariaDB, MySQL &lt; 8.0.14), if the applied subquery is correlated only through predicates that compare one of its
    ///     grouping keys (or, for <c>DISTINCT</c>, one of its projected expressions) with an expression of the outer query.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         EF Core normally performs this conversion itself while translating (<c>SelectExpression.AddJoin()</c>), but gives up when
    ///         the correlation key column of a grouped/distinct subquery is not contained, with identical nullability, in the subquery's
    ///         projection. That happens for <c>subquery.Where(...).DefaultIfEmpty()</c>, because <c>DefaultIfEmpty()</c> makes the
    ///         projection nullable (see https://github.com/dotnet/efcore/issues/39220). The unconverted apply is then rejected by
    ///         <see cref="MySqlCompatibilityExpressionVisitor"/> on servers that cannot execute it.
    ///     </para>
    ///     <para>
    ///         Moving such a predicate out of the subquery and into the join condition is semantically neutral: every group (or distinct
    ///         row) is evaluated independently of the outer row, so filtering the groups inside the lateral subquery is equivalent to
    ///         joining all groups on the same condition. Anything else (correlation in the projection, in <c>WHERE</c> of a grouped
    ///         query, <c>LIMIT</c>/<c>OFFSET</c>, aggregates or window functions above the filter, etc.) is left untouched.
    ///     </para>
    ///     <para>
    ///         This visitor must run on the immutable query tree (after <see cref="RelationalQueryTranslationPostprocessor.Process"/>)
    ///         and before <see cref="MySqlHavingExpressionVisitor"/> rewrites the remaining <c>HAVING</c> clauses.
    ///     </para>
    /// </remarks>
    public class MySqlApplyToJoinExpressionVisitor : ExpressionVisitor
    {
        private readonly bool _convertOuterApply;
        private readonly bool _convertCrossApply;

        public MySqlApplyToJoinExpressionVisitor(IMySqlOptions options)
        {
            _convertOuterApply = !options.ServerVersion.Supports.OuterApply;
            _convertCrossApply = !options.ServerVersion.Supports.CrossApply;
        }

        public virtual Expression Process(Expression expression)
            => _convertOuterApply || _convertCrossApply
                ? Visit(expression)
                : expression;

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                OuterApplyExpression outerApplyExpression when _convertOuterApply
                    => VisitApply(outerApplyExpression, outerApplyExpression.Table, isOuter: true),
                CrossApplyExpression crossApplyExpression when _convertCrossApply
                    => VisitApply(crossApplyExpression, crossApplyExpression.Table, isOuter: false),
                ShapedQueryExpression shapedQueryExpression
                    => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        private Expression VisitApply(JoinExpressionBase apply, TableExpressionBase table, bool isOuter)
        {
            // Convert nested applies first.
            var visitedTable = (TableExpressionBase)Visit(table);

            if (visitedTable is SelectExpression select &&
                TryDecorrelate(select, out var decorrelated, out var correlations))
            {
                var joinPredicate = correlations
                    .Select(c => c.CreateJoinPredicate())
                    .Aggregate((left, right) => new SqlBinaryExpression(ExpressionType.AndAlso, left, right, typeof(bool), left.TypeMapping));

                return isOuter
                    ? new LeftJoinExpression(decorrelated, joinPredicate)
                    : new InnerJoinExpression(decorrelated, joinPredicate);
            }

            return apply.Update(visitedTable);
        }

        /// <summary>
        ///     Tries to remove all outer-correlated predicates from <paramref name="select"/>. On success, the returned select exposes the
        ///     inner side of every removed predicate as a projected column (see <see cref="Correlation.Key"/>) and no longer references
        ///     the outer query.
        /// </summary>
        private static bool TryDecorrelate(SelectExpression select, out SelectExpression result, out List<Correlation> correlations)
        {
            result = null;
            correlations = null;

            if (select.Limit is not null ||
                select.Offset is not null)
            {
                return false;
            }

            var success = IsGroupedFilter(select)
                ? TryExtractFromFilter(select, isHaving: true, out result, out correlations)
                : IsDistinctFilter(select)
                    ? TryExtractFromFilter(select, isHaving: false, out result, out correlations)
                    : IsTransparentWrapper(select) &&
                      TryDecorrelateWrapper(select, out result, out correlations);

            if (success &&
                !ContainsOuterReference(result))
            {
                return true;
            }

            result = null;
            correlations = null;
            return false;
        }

        private static bool IsGroupedFilter(SelectExpression select)
            => select.GroupBy.Count > 0 && !select.IsDistinct && select.Having is not null && !ContainsWindowFunction(select);

        private static bool IsDistinctFilter(SelectExpression select)
            => select.GroupBy.Count == 0 && select.IsDistinct && select.Predicate is not null && !ContainsWindowFunction(select);

        /// <summary>
        ///     A select that only forwards the columns of a single subquery (e.g. the result of EF Core pushing a query down into a
        ///     subquery). Filtering the subquery is equivalent to filtering the wrapper.
        /// </summary>
        private static bool IsTransparentWrapper(SelectExpression select)
            => select.Tables.Count == 1 &&
               select.Tables[0] is SelectExpression &&
               select.Predicate is null &&
               select.GroupBy.Count == 0 &&
               select.Having is null &&
               !select.IsDistinct &&
               select.Projection.All(p => p.Expression is ColumnExpression);

        private static bool TryDecorrelateWrapper(SelectExpression wrapper, out SelectExpression result, out List<Correlation> correlations)
        {
            result = null;
            correlations = null;

            var innerSelect = (SelectExpression)wrapper.Tables[0];

            if (!TryDecorrelate(innerSelect, out var decorrelatedInner, out var innerCorrelations))
            {
                return false;
            }

            var projection = wrapper.Projection.ToList();
            correlations = new List<Correlation>();

            foreach (var innerCorrelation in innerCorrelations)
            {
                var key = innerCorrelation.Key;
                var existing = projection.FirstOrDefault(p => p.Expression is ColumnExpression c &&
                                                              c.TableAlias == key.TableAlias &&
                                                              c.Name == key.Name);
                if (existing is null)
                {
                    existing = new ProjectionExpression(key, GetUniqueAlias(projection, key.Name));
                    projection.Add(existing);
                }

                correlations.Add(
                    innerCorrelation.WithKey(
                        new ColumnExpression(existing.Alias, wrapper.Alias, key.Type, key.TypeMapping, key.IsNullable)));
            }

            result = wrapper.Update(
                new[] { decorrelatedInner },
                wrapper.Predicate,
                wrapper.GroupBy,
                wrapper.Having,
                projection,
                wrapper.Orderings,
                wrapper.Offset,
                wrapper.Limit);

            return true;
        }

        private static bool TryExtractFromFilter(
            SelectExpression select,
            bool isHaving,
            out SelectExpression result,
            out List<Correlation> correlations)
        {
            result = null;
            correlations = null;

            var innerAliases = select.Tables.Select(t => t.GetRequiredAlias()).ToHashSet();
            var keptConjuncts = new List<SqlExpression>();
            var extracted = new List<(SqlBinaryExpression Binary, bool InnerOnLeft)>();

            foreach (var conjunct in SplitConjuncts(isHaving ? select.Having : select.Predicate))
            {
                if (conjunct is SqlBinaryExpression binary &&
                    IsComparison(binary.OperatorType) &&
                    TryGetSides(binary, innerAliases, select, isHaving, out var innerOnLeft))
                {
                    extracted.Add((binary, innerOnLeft));
                }
                else
                {
                    keptConjuncts.Add(conjunct);
                }
            }

            if (extracted.Count == 0)
            {
                return false;
            }

            var projection = select.Projection.ToList();
            correlations = new List<Correlation>();

            foreach (var (binary, innerOnLeft) in extracted)
            {
                var innerSide = innerOnLeft ? binary.Left : binary.Right;

                var projectionExpression = projection.FirstOrDefault(
                    p => !string.IsNullOrEmpty(p.Alias) && IsSameExpression(p.Expression, innerSide));
                if (projectionExpression is null)
                {
                    var baseName = innerSide is ColumnExpression column ? column.Name : "key";
                    projectionExpression = new ProjectionExpression(innerSide, GetUniqueAlias(projection, baseName));
                    projection.Add(projectionExpression);
                }

                var key = new ColumnExpression(
                    projectionExpression.Alias,
                    select.Alias,
                    innerSide.Type,
                    innerSide.TypeMapping,
                    innerSide is not ColumnExpression innerColumn || innerColumn.IsNullable);

                correlations.Add(new Correlation(binary, innerOnLeft, key));
            }

            var remaining = keptConjuncts.Count == 0
                ? null
                : keptConjuncts.Aggregate(
                    (left, right) => new SqlBinaryExpression(ExpressionType.AndAlso, left, right, typeof(bool), left.TypeMapping));

            result = select.Update(
                select.Tables,
                isHaving ? select.Predicate : remaining,
                select.GroupBy,
                isHaving ? remaining : select.Having,
                projection,
                select.Orderings,
                select.Offset,
                select.Limit);

            return true;
        }

        /// <summary>
        ///     Checks that one side of the comparison belongs to the subquery and is one of its keys (grouping key, or projected
        ///     expression for <c>DISTINCT</c>), and that the other side only references the outer query.
        /// </summary>
        private static bool TryGetSides(
            SqlBinaryExpression binary,
            HashSet<string> innerAliases,
            SelectExpression select,
            bool isHaving,
            out bool innerOnLeft)
        {
            innerOnLeft = false;

            if (!TryGetReferencedAliases(binary.Left, out var leftAliases) ||
                !TryGetReferencedAliases(binary.Right, out var rightAliases) ||
                leftAliases.Count == 0 ||
                rightAliases.Count == 0)
            {
                return false;
            }

            bool leftIsInner = leftAliases.All(innerAliases.Contains);
            bool rightIsInner = rightAliases.All(innerAliases.Contains);
            bool leftIsOuter = !leftAliases.Any(innerAliases.Contains);
            bool rightIsOuter = !rightAliases.Any(innerAliases.Contains);

            SqlExpression innerSide;
            if (leftIsInner && rightIsOuter)
            {
                innerOnLeft = true;
                innerSide = binary.Left;
            }
            else if (rightIsInner && leftIsOuter)
            {
                innerSide = binary.Right;
            }
            else
            {
                return false;
            }

            return isHaving
                ? select.GroupBy.Any(g => IsSameExpression(g, innerSide))
                : select.Projection.Any(p => IsSameExpression(p.Expression, innerSide));
        }

        private static bool IsComparison(ExpressionType operatorType)
            => operatorType is ExpressionType.Equal
                or ExpressionType.NotEqual
                or ExpressionType.LessThan
                or ExpressionType.LessThanOrEqual
                or ExpressionType.GreaterThan
                or ExpressionType.GreaterThanOrEqual;

        private static IEnumerable<SqlExpression> SplitConjuncts(SqlExpression expression)
        {
            if (expression is SqlBinaryExpression { OperatorType: ExpressionType.AndAlso } and)
            {
                foreach (var conjunct in SplitConjuncts(and.Left).Concat(SplitConjuncts(and.Right)))
                {
                    yield return conjunct;
                }
            }
            else
            {
                yield return expression;
            }
        }

        // Nullability can differ between the same column in the projection and in predicates, so it is deliberately ignored.
        private static bool IsSameExpression(SqlExpression left, SqlExpression right)
            => left.Equals(right) ||
               left is ColumnExpression leftColumn &&
               right is ColumnExpression rightColumn &&
               leftColumn.TableAlias == rightColumn.TableAlias &&
               leftColumn.Name == rightColumn.Name;

        private static string GetUniqueAlias(IReadOnlyList<ProjectionExpression> projection, string baseName)
        {
            var alias = baseName;
            for (var i = 0; projection.Any(p => p.Alias == alias); i++)
            {
                alias = baseName + i;
            }

            return alias;
        }

        private static bool ContainsWindowFunction(SelectExpression select)
        {
            var finder = new WindowFunctionFinder();
            foreach (var projection in select.Projection)
            {
                finder.Visit(projection.Expression);
            }

            return finder.Found;
        }

        private static bool TryGetReferencedAliases(SqlExpression expression, out HashSet<string> aliases)
        {
            var collector = new AliasCollector();
            collector.Visit(expression);
            aliases = collector.UsedAliases;
            return !collector.ContainsSubquery;
        }

        private static bool ContainsOuterReference(SelectExpression select)
        {
            var collector = new AliasCollector();
            collector.Visit(select);
            return collector.UsedAliases.Any(a => !collector.DefinedAliases.Contains(a));
        }

        private readonly struct Correlation
        {
            private readonly SqlBinaryExpression _binary;
            private readonly bool _innerOnLeft;

            public Correlation(SqlBinaryExpression binary, bool innerOnLeft, ColumnExpression key)
            {
                _binary = binary;
                _innerOnLeft = innerOnLeft;
                Key = key;
            }

            /// <summary>The column of the decorrelated select that exposes the inner side of the removed predicate.</summary>
            public ColumnExpression Key { get; }

            public Correlation WithKey(ColumnExpression key)
                => new(_binary, _innerOnLeft, key);

            public SqlExpression CreateJoinPredicate()
                => _innerOnLeft
                    ? _binary.Update(Key, _binary.Right)
                    : _binary.Update(_binary.Left, Key);
        }

        private sealed class AliasCollector : ExpressionVisitor
        {
            public HashSet<string> UsedAliases { get; } = new();
            public HashSet<string> DefinedAliases { get; } = new();
            public bool ContainsSubquery { get; private set; }

            protected override Expression VisitExtension(Expression node)
            {
                switch (node)
                {
                    case ColumnExpression column:
                        UsedAliases.Add(column.TableAlias);
                        return node;

                    case SelectExpression select:
                        ContainsSubquery = true;
                        foreach (var table in select.Tables)
                        {
                            var alias = table.GetRequiredAlias();
                            if (alias is not null)
                            {
                                DefinedAliases.Add(alias);
                            }
                        }

                        return base.VisitExtension(node);

                    default:
                        return base.VisitExtension(node);
                }
            }
        }

        private sealed class WindowFunctionFinder : ExpressionVisitor
        {
            public bool Found { get; private set; }

            protected override Expression VisitExtension(Expression node)
            {
                if (node is RowNumberExpression)
                {
                    Found = true;
                    return node;
                }

                return base.VisitExtension(node);
            }
        }
    }
}
