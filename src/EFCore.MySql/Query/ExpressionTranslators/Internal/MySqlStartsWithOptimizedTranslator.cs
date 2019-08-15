// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlStartsWithOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

        private static readonly MethodInfo _concat
            = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (Equals(methodCallExpression.Method, _methodInfo))
            {
                var patternExpression = methodCallExpression.Arguments[0];

                var startsWithExpression = Expression.AndAlso(
                    new LikeExpression(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        methodCallExpression.Object,
                        Expression.Add(methodCallExpression.Arguments[0], Expression.Constant("%", typeof(string)), _concat)),
                    new NullCompensatedExpression(
                        Expression.Equal(
                            new SqlFunctionExpression(
                                "LEFT",
                                // ReSharper disable once PossibleNullReferenceException
                                methodCallExpression.Object.Type,
                                new[]
                                {
                                    methodCallExpression.Object,
                                    new SqlFunctionExpression("CHAR_LENGTH", typeof(int), new[] { patternExpression })
                                }),
                            patternExpression)));

                return patternExpression is ConstantExpression patternConstantExpression
                    ? (string)patternConstantExpression.Value == string.Empty
                        ? (Expression)Expression.Constant(true)
                        : startsWithExpression
                    : Expression.OrElse(
                        startsWithExpression,
                        Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
            }

            return null;
        }
    }
}
