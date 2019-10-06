// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
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
    public class MySqlConvertTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo[] _supportedMethods
            = new []
                {
                    nameof(Convert.ToByte),
                    nameof(Convert.ToDecimal),
                    nameof(Convert.ToDouble),
                    nameof(Convert.ToInt16),
                    nameof(Convert.ToInt32),
                    nameof(Convert.ToInt64),
                    nameof(Convert.ToString),
                }
                .SelectMany(t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
                    .Where(m => m.GetParameters().Length == 1))
                .ToArray();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
           => methodCallExpression.Method.Name == nameof(ToString)
               ? new MySqlObjectToStringTranslator().Translate(methodCallExpression)
               : _supportedMethods.Contains(methodCallExpression.Method)
                   ? new ExplicitCastExpression(
                       methodCallExpression.Arguments[0],
                       methodCallExpression.Method.ReturnType)
                   : null;
    }
}
