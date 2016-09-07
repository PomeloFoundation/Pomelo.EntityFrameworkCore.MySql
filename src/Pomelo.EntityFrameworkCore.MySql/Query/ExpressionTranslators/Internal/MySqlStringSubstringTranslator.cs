// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlStringSubstringTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo = typeof(string).GetTypeInfo()
            .GetDeclaredMethods(nameof(string.Substring))
            .Single(m => m.GetParameters().Length == 2);

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method == _methodInfo)
            {
                var sqlArguments = new[] { methodCallExpression.Object }.Concat(methodCallExpression.Arguments);
                return new SqlFunctionExpression("SUBSTRING", methodCallExpression.Type, sqlArguments);
            }

            return null;
        }
    }
}
