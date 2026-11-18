// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microting.EntityFrameworkCore.MySql.Query.Internal
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

            var result = Visit(selectExpression);

            return result;
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
    }
}
