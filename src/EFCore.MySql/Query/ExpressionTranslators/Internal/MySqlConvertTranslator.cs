// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MySqlConvertTranslator : IMethodCallTranslator
    {
        private static readonly Dictionary<string, string> _typeMapping = new Dictionary<string, string>
        {
            [nameof(Convert.ToByte)] = "signed",
            [nameof(Convert.ToDecimal)] = "decimal",
            [nameof(Convert.ToDouble)] = "decimal",
            [nameof(Convert.ToInt16)] = "signed",
            [nameof(Convert.ToInt32)] = "signed",
            [nameof(Convert.ToInt64)] = "signed",
            [nameof(Convert.ToString)] = "char"
        };

        private static readonly List<Type> _supportedTypes = new List<Type>
        {
            typeof(bool),
            typeof(byte),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(string)
        };

        private static readonly IEnumerable<MethodInfo> _supportedMethods
            = _typeMapping.Keys
                .SelectMany(t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
                    .Where(m => m.GetParameters().Length == 1
                        && _supportedTypes.Contains(m.GetParameters().First().ParameterType)));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
           => (methodCallExpression.Method.Name == nameof(ToString)) ?
           new MySqlObjectToStringTranslator().Translate(methodCallExpression) :
           (_supportedMethods.Contains(methodCallExpression.Method)
               ? new SqlFunctionExpression(
                   "CONVERT",
                   methodCallExpression.Type,
                   new[]
                   {
                        new SqlFragmentExpression(
                            _typeMapping[methodCallExpression.Method.Name]),
                        methodCallExpression.Arguments[0]
                   })
               : null);
    }
}
