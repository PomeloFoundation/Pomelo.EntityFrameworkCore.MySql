// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlMathRoundTranslator : IMethodCallTranslator
    {
        private static IEnumerable<MethodInfo> _methodInfos = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Round))
            .Where(m => m.GetParameters().Count() == 1
                || (m.GetParameters().Count() == 2 && m.GetParameters()[1].ParameterType == typeof(int)));

        public virtual Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            if (_methodInfos.Contains(methodCallExpression.Method))
            {
                var arguments = methodCallExpression.Arguments.Count == 1
                    ? new[] { methodCallExpression.Arguments[0], Expression.Constant(0) }
                    : new[] { methodCallExpression.Arguments[1], methodCallExpression.Arguments[1] };

                return new SqlFunctionExpression("ROUND", methodCallExpression.Type, arguments);
            }

            return null;
        }
    }
}
