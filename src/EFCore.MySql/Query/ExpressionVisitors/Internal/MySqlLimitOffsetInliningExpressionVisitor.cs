// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    /// <summary>
    /// MySQL and MariaDB do not allow arbitrary expressions (e.g. function calls like <c>LEAST</c>/<c>GREATEST</c>) in the
    /// <c>LIMIT</c> and <c>OFFSET</c> clauses; only integer literals or parameter placeholders are accepted.
    /// EF Core 10 can produce a <c>LEAST</c>/<c>GREATEST</c> expression for these clauses when combining nested
    /// <c>Skip</c>/<c>Take</c> operators. Because this visitor runs in the parameter-based SQL processor (after the parameter
    /// values are known), it evaluates such expressions to a single constant, which the database accepts.
    /// </summary>
    public class MySqlLimitOffsetInliningExpressionVisitor : ExpressionVisitor
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private ParametersCacheDecorator _parametersDecorator;

        public MySqlLimitOffsetInliningExpressionVisitor(ISqlExpressionFactory sqlExpressionFactory)
        {
            Check.NotNull(sqlExpressionFactory, nameof(sqlExpressionFactory));

            _sqlExpressionFactory = sqlExpressionFactory;
            _parametersDecorator = null!;
        }

        public virtual Expression Process(Expression expression, ParametersCacheDecorator parametersDecorator)
        {
            Check.NotNull(expression, nameof(expression));
            Check.NotNull(parametersDecorator, nameof(parametersDecorator));

            _parametersDecorator = parametersDecorator;

            return Visit(expression);
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            var visited = base.VisitExtension(extensionExpression);

            if (visited is SelectExpression selectExpression)
            {
                var newLimit = TryEvaluate(selectExpression.Limit);
                var newOffset = TryEvaluate(selectExpression.Offset);

                if (!ReferenceEquals(newLimit, selectExpression.Limit) ||
                    !ReferenceEquals(newOffset, selectExpression.Offset))
                {
                    return selectExpression.Update(
                        selectExpression.Tables,
                        selectExpression.Predicate,
                        selectExpression.GroupBy,
                        selectExpression.Having,
                        selectExpression.Projection,
                        selectExpression.Orderings,
                        newOffset,
                        newLimit);
                }
            }

            return visited;
        }

        private SqlExpression? TryEvaluate(SqlExpression? expression)
        {
            // Only LEAST/GREATEST (and nested combinations thereof) are problematic in LIMIT/OFFSET. Simple constants and
            // parameter placeholders are valid as-is and must be left untouched, so that the SQL can still be cached.
            if (expression is SqlFunctionExpression { IsBuiltIn: true } function &&
                (string.Equals(function.Name, "LEAST", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(function.Name, "GREATEST", StringComparison.OrdinalIgnoreCase)) &&
                function.Arguments is { Count: > 0 })
            {
                var isLeast = string.Equals(function.Name, "LEAST", StringComparison.OrdinalIgnoreCase);
                long? result = null;

                foreach (var argument in function.Arguments)
                {
                    if (!TryGetValue(argument, out var value))
                    {
                        return expression;
                    }

                    result = result is null
                        ? value
                        : isLeast
                            ? Math.Min(result.Value, value)
                            : Math.Max(result.Value, value);
                }

                if (result is not null)
                {
                    // The value depends on the actual parameter values, so the resulting SQL must not be cached.
                    _parametersDecorator.GetAndDisableCaching();

                    return (SqlExpression)_sqlExpressionFactory.Constant(
                        Convert.ChangeType(result.Value, expression.Type, CultureInfo.InvariantCulture),
                        expression.TypeMapping);
                }
            }

            return expression;
        }

        private bool TryGetValue(SqlExpression expression, out long value)
        {
            switch (expression)
            {
                case SqlConstantExpression { Value: { } constantValue }:
                    value = Convert.ToInt64(constantValue, CultureInfo.InvariantCulture);
                    return true;

                case SqlParameterExpression parameter
                    when _parametersDecorator.GetAndDisableCaching().TryGetValue(parameter.InvariantName, out var parameterValue)
                         && parameterValue is not null:
                    value = Convert.ToInt64(parameterValue, CultureInfo.InvariantCulture);
                    return true;

                case SqlFunctionExpression:
                    // Support nested LEAST/GREATEST by evaluating the inner function first.
                    var evaluated = TryEvaluate(expression);
                    if (evaluated is SqlConstantExpression { Value: { } innerValue })
                    {
                        value = Convert.ToInt64(innerValue, CultureInfo.InvariantCulture);
                        return true;
                    }

                    break;
            }

            value = default;
            return false;
        }
    }
}
