﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlContainsOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (Equals(methodCallExpression.Method, _methodInfo))
            {
                var patternExpression = methodCallExpression.Arguments[0];
                var patternConstantExpression = patternExpression as ConstantExpression;
                var binaryExpression = new MySqlBinaryExpression(patternExpression);

                var charIndexExpression = Expression.GreaterThan(
                    new SqlFunctionExpression("LOCATE", typeof(int), new[] { binaryExpression, methodCallExpression.Object }),
                    Expression.Constant(0));

                return
                    patternConstantExpression != null
                        ? (string)patternConstantExpression.Value == string.Empty
                            ? (Expression)Expression.Constant(true)
                            : charIndexExpression
                        : Expression.OrElse(
                            charIndexExpression,
                            Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
            }

            return null;
        }
    }
}
