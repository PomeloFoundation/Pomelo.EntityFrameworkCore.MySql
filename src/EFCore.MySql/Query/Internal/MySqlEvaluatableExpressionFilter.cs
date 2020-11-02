// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlEvaluatableExpressionFilter : RelationalEvaluatableExpressionFilter
    {
        public MySqlEvaluatableExpressionFilter([NotNull] EvaluatableExpressionFilterDependencies dependencies, [NotNull] RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        public override bool IsEvaluatableExpression(Expression expression, IModel model)
        {
            if (expression is MethodCallExpression methodCallExpression
                && methodCallExpression.Method.DeclaringType == typeof(MySqlDbFunctionsExtensions))
            {
                return false;
            }

            return base.IsEvaluatableExpression(expression, model);
        }
    }
}
