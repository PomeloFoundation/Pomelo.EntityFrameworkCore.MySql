// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using EFCore.MySql.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EFCore.MySql.Query.ExpressionTranslators.Internal
{

    public class MySqlRegexIsMatchTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(Regex).GetRuntimeMethod(nameof(Regex.IsMatch), new[] { typeof(string), typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
            => _methodInfo.Equals(methodCallExpression.Method)
                ? new RegexpExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1])
                : null;
    }
}
