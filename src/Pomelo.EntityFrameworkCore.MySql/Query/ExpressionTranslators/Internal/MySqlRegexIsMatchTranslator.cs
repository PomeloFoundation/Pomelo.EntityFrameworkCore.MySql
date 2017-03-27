// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlRegexIsMatchTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo IsMatch;

        static MySqlRegexIsMatchTranslator()
        {
            IsMatch = typeof (Regex).GetTypeInfo().GetDeclaredMethods("IsMatch").Single(m =>
                m.GetParameters().Count() == 2 &&
                m.GetParameters().All(p => p.ParameterType == typeof(string))
            );
        }

        public Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method == IsMatch)
            {
                return new RegexMatchExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1]
                );
            }

            return null;
        }
    }
}
