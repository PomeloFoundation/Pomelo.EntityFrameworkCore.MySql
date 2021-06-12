// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <inheritdoc />
    public class MySqlSqlNullabilityProcessor : SqlNullabilityProcessor
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="MySqlSqlNullabilityProcessor" /> class.
        /// </summary>
        /// <param name="dependencies">Parameter object containing dependencies for this class.</param>
        /// <param name="useRelationalNulls">A bool value indicating whether relational null semantics are in use.</param>
        public MySqlSqlNullabilityProcessor(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls)
            : base(dependencies, useRelationalNulls)
            => _sqlExpressionFactory = dependencies.SqlExpressionFactory;

        /// <inheritdoc />
        protected override SqlExpression VisitCustomSqlExpression(
            SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
            => sqlExpression switch
            {
                MySqlBinaryExpression binaryExpression => VisitBinary(binaryExpression, allowOptimizedExpansion, out nullable),
                MySqlCollateExpression collateExpression => VisitCollate(collateExpression, allowOptimizedExpansion, out nullable),
                MySqlComplexFunctionArgumentExpression complexFunctionArgumentExpression => VisitComplexFunctionArgument(complexFunctionArgumentExpression, allowOptimizedExpansion, out nullable),
                MySqlMatchExpression matchExpression => VisitMatch(matchExpression, allowOptimizedExpansion, out nullable),
                MySqlJsonArrayIndexExpression arrayIndexExpression => VisitJsonArrayIndex(arrayIndexExpression, allowOptimizedExpansion, out nullable),
                MySqlJsonTraversalExpression jsonTraversalExpression => VisitJsonTraversal(jsonTraversalExpression, allowOptimizedExpansion, out nullable),
                MySqlRegexpExpression regexpExpression => VisitRegexp(regexpExpression, allowOptimizedExpansion, out nullable),
                MySqlColumnAliasReferenceExpression columnAliasReferenceExpression => VisitColumnAliasReference(columnAliasReferenceExpression, allowOptimizedExpansion, out nullable),
                _ => base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable)
            };

        private SqlExpression VisitColumnAliasReference(MySqlColumnAliasReferenceExpression columnAliasReferenceExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(columnAliasReferenceExpression, nameof(columnAliasReferenceExpression));

            var expression = Visit(columnAliasReferenceExpression.Expression, allowOptimizedExpansion, out nullable);

            return columnAliasReferenceExpression.Update(columnAliasReferenceExpression.Alias, expression);
        }

        /// <summary>
        /// Visits a <see cref="MySqlBinaryExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="binaryExpression">A <see cref="MySqlBinaryExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitBinary(
            [NotNull] MySqlBinaryExpression binaryExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(binaryExpression, nameof(binaryExpression));

            var left = Visit(binaryExpression.Left, allowOptimizedExpansion, out var leftNullable);
            var right = Visit(binaryExpression.Right, allowOptimizedExpansion, out var rightNullable);

            nullable = leftNullable || rightNullable;

            return binaryExpression.Update(left, right);
        }

        /// <summary>
        /// Visits a <see cref="MySqlCollateExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="collateExpression">A <see cref="MySqlCollateExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitCollate(
            [NotNull] MySqlCollateExpression collateExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(collateExpression, nameof(collateExpression));

            var valueExpression = Visit(collateExpression.ValueExpression, allowOptimizedExpansion, out nullable);

            return collateExpression.Update(valueExpression);
        }

        /// <summary>
        /// Visits a <see cref="MySqlComplexFunctionArgumentExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="complexFunctionArgumentExpression">A <see cref="MySqlComplexFunctionArgumentExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitComplexFunctionArgument(
            [NotNull] MySqlComplexFunctionArgumentExpression complexFunctionArgumentExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(complexFunctionArgumentExpression, nameof(complexFunctionArgumentExpression));

            nullable = false;

            var argumentParts = new SqlExpression[complexFunctionArgumentExpression.ArgumentParts.Count];

            for (var i = 0; i < argumentParts.Length; i++)
            {
                argumentParts[i] = Visit(complexFunctionArgumentExpression.ArgumentParts[i], allowOptimizedExpansion, out var argumentPartNullable);
                nullable |= argumentPartNullable;
            }

            return complexFunctionArgumentExpression.Update(argumentParts, complexFunctionArgumentExpression.Delimiter);
        }

        /// <summary>
        /// Visits a <see cref="MySqlMatchExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="matchExpression">A <see cref="MySqlMatchExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitMatch(
            [NotNull] MySqlMatchExpression matchExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(matchExpression, nameof(matchExpression));

            var match = Visit(matchExpression.Match, allowOptimizedExpansion, out var matchNullable);
            var pattern = Visit(matchExpression.Against, allowOptimizedExpansion, out var patternNullable);

            nullable = matchNullable || patternNullable;

            return matchExpression.Update(match, pattern);
        }

        /// <summary>
        /// Visits an <see cref="MySqlJsonArrayIndexExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="jsonArrayIndexExpression">A <see cref="MySqlJsonArrayIndexExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitJsonArrayIndex(
            [NotNull] MySqlJsonArrayIndexExpression jsonArrayIndexExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(jsonArrayIndexExpression, nameof(jsonArrayIndexExpression));

            var index = Visit(jsonArrayIndexExpression.Expression, allowOptimizedExpansion, out nullable);

            return jsonArrayIndexExpression.Update(index);
        }

        /// <summary>
        /// Visits a <see cref="MySqlJsonTraversalExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="jsonTraversalExpression">A <see cref="MySqlJsonTraversalExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitJsonTraversal(
            [NotNull] MySqlJsonTraversalExpression jsonTraversalExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(jsonTraversalExpression, nameof(jsonTraversalExpression));

            var expression = Visit(jsonTraversalExpression.Expression, out nullable);

            List<SqlExpression> newPath = null;
            for (var i = 0; i < jsonTraversalExpression.Path.Count; i++)
            {
                var pathComponent = jsonTraversalExpression.Path[i];
                var newPathComponent = Visit(pathComponent, allowOptimizedExpansion, out var nullablePathComponent);
                nullable |= nullablePathComponent;
                if (newPathComponent != pathComponent && newPath is null)
                {
                    newPath = new List<SqlExpression>();
                    for (var j = 0; j < i; j++)
                    {
                        newPath.Add(newPathComponent);
                    }
                }

                newPath?.Add(newPathComponent);
            }

            nullable = false;

            return jsonTraversalExpression.Update(
                expression,
                newPath is null
                    ? jsonTraversalExpression.Path
                    : newPath.ToArray());
        }

        /// <summary>
        /// Visits a <see cref="MySqlRegexpExpression" /> and computes its nullability.
        /// </summary>
        /// <param name="regexpExpression">A <see cref="MySqlRegexpExpression" /> expression to visit.</param>
        /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
        /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
        /// <returns>An optimized sql expression.</returns>
        protected virtual SqlExpression VisitRegexp(
            [NotNull] MySqlRegexpExpression regexpExpression, bool allowOptimizedExpansion, out bool nullable)
        {
            Check.NotNull(regexpExpression, nameof(regexpExpression));

            var match = Visit(regexpExpression.Match, out var matchNullable);
            var pattern = Visit(regexpExpression.Pattern, out var patternNullable);

            nullable = matchNullable || patternNullable;

            return regexpExpression.Update(match, pattern);
        }
    }
}
