// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlStringSubstringTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
            => _methodInfo.Equals(methodCallExpression.Method)
                ? new SqlFunctionExpression(
                    "SUBSTRING",
                    methodCallExpression.Type,
                    new[]
                    {
                        methodCallExpression.Object,
                        // Accommodate for SQL Server assumption of 1-based string indexes
                        methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant
                            ? (Expression)Expression.Constant(
                                (int)((ConstantExpression)methodCallExpression.Arguments[0]).Value + 1)
                            : Expression.Add(
                                methodCallExpression.Arguments[0],
                                Expression.Constant(1)),
                        methodCallExpression.Arguments[1]
                    })
                : null;
    }
}
