// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class SkipTakeCollapsingExpressionVisitor : ExpressionVisitor
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        private ParametersCacheDecorator _parametersDecorator;

        public SkipTakeCollapsingExpressionVisitor(ISqlExpressionFactory sqlExpressionFactory)
        {
            Check.NotNull(sqlExpressionFactory, nameof(sqlExpressionFactory));

            _sqlExpressionFactory = sqlExpressionFactory;
            _parametersDecorator = null!;
        }

        public virtual Expression Process(
            Expression selectExpression,
            ParametersCacheDecorator parametersDecorator)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersDecorator, nameof(parametersDecorator));

            _parametersDecorator = parametersDecorator;

            return Visit(selectExpression);
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            if (extensionExpression is SelectExpression selectExpression)
            {
                if (IsZero(selectExpression.Limit)
                    && IsZero(selectExpression.Offset))
                {
                    return selectExpression.Update(
                        selectExpression.Tables,
                        selectExpression.GroupBy.Count > 0
                            ? selectExpression.Predicate
                            : _sqlExpressionFactory.ApplyDefaultTypeMapping(_sqlExpressionFactory.Constant(false)),
                        selectExpression.GroupBy,
                        selectExpression.GroupBy.Count > 0
                            ? _sqlExpressionFactory.ApplyDefaultTypeMapping(_sqlExpressionFactory.Constant(false))
                            : null,
                        selectExpression.Projection,
                        new List<OrderingExpression>(0),
                        offset: null,
                        limit: null);
                }

                // MySQL/MariaDB do not support function expressions (e.g. LEAST/GREATEST) in LIMIT/OFFSET clauses.
                // Evaluate them client-side and replace with a constant.
                var newLimit = TryEvaluateForLimitOffset(selectExpression.Limit);
                var newOffset = TryEvaluateForLimitOffset(selectExpression.Offset);

                if (newLimit != selectExpression.Limit || newOffset != selectExpression.Offset)
                {
                    return base.VisitExtension(
                        selectExpression.Update(
                            selectExpression.Tables,
                            selectExpression.Predicate,
                            selectExpression.GroupBy,
                            selectExpression.Having,
                            selectExpression.Projection,
                            selectExpression.Orderings,
                            newOffset,
                            newLimit));
                }

                bool IsZero(SqlExpression? sqlExpression)
                {
                    switch (sqlExpression)
                    {
                        case SqlConstantExpression constant
                        when constant.Value is int intValue:
                            return intValue == 0;
                        case SqlParameterExpression parameter:
                            var parameterValues = _parametersDecorator.GetAndDisableCaching();
                            return parameterValues[parameter.Name] is int value && value == 0;

                        default:
                            return false;
                    }
                }
            }

            return base.VisitExtension(extensionExpression);
        }

        /// <summary>
        /// MySQL/MariaDB do not support arbitrary expressions (functions like LEAST/GREATEST) in LIMIT/OFFSET.
        /// If the expression is a function call (not a simple constant or parameter), evaluate it client-side
        /// using the current parameter values and return a constant.
        /// </summary>
        private SqlExpression? TryEvaluateForLimitOffset(SqlExpression? sqlExpression)
        {
            if (sqlExpression is null)
            {
                return null;
            }

            // Constants and simple parameters are fine as-is in LIMIT/OFFSET.
            if (sqlExpression is SqlConstantExpression or SqlParameterExpression)
            {
                return sqlExpression;
            }

            // For complex expressions (e.g. LEAST(@p, 1)), try to evaluate client-side.
            var value = EvaluateExpression(sqlExpression);
            if (value is not null)
            {
                return _sqlExpressionFactory.Constant(value, sqlExpression.TypeMapping);
            }

            return sqlExpression;
        }

        private object? EvaluateExpression(SqlExpression sqlExpression)
        {
            try
            {
                return sqlExpression switch
                {
                    SqlConstantExpression c => c.Value,
                    SqlParameterExpression p => _parametersDecorator.GetAndDisableCaching()[p.Name],
                    SqlFunctionExpression f => EvaluateFunction(f),
                    _ => null,
                };
            }
            catch
            {
                return null;
            }
        }

        private object? EvaluateFunction(SqlFunctionExpression function)
        {
            if (function.Arguments is null)
            {
                return null;
            }

            var args = function.Arguments.Select(EvaluateExpression).ToArray();

            return function.Name switch
            {
                "LEAST" => args.Where(a => a is not null).Min(),
                "GREATEST" => args.Where(a => a is not null).Max(),
                _ => null,
            };
        }
    }
}
