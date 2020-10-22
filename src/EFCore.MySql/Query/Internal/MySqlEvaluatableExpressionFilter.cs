// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlEvaluatableExpressionFilter : RelationalEvaluatableExpressionFilter
    {
        private readonly IEnumerable<IMySqlEvaluatableExpressionFilter> _mySqlEvaluatableExpressionFilters;

        public MySqlEvaluatableExpressionFilter(
            [NotNull] EvaluatableExpressionFilterDependencies dependencies,
            [NotNull] RelationalEvaluatableExpressionFilterDependencies relationalDependencies,
            [NotNull] IEnumerable<IMySqlEvaluatableExpressionFilter> mySqlEvaluatableExpressionFilters)
            : base(dependencies, relationalDependencies)
        {
            _mySqlEvaluatableExpressionFilters = mySqlEvaluatableExpressionFilters;
        }

        public override bool IsEvaluatableExpression(Expression expression, IModel model)
        {
            foreach (var evaluatableExpressionFilter in _mySqlEvaluatableExpressionFilters)
            {
                var evaluatable = evaluatableExpressionFilter.IsEvaluatableExpression(expression, model);
                if (evaluatable.HasValue)
                {
                    return evaluatable.Value;
                }
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                var declaringType = methodCallExpression.Method.DeclaringType;

                if (declaringType == typeof(MySqlDbFunctionsExtensions) ||
                    declaringType == typeof(MySqlJsonDbFunctionsExtensions))
                {
                    return false;
                }
            }

            return base.IsEvaluatableExpression(expression, model);
        }
    }
}
