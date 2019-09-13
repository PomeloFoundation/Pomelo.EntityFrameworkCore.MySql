// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlConvertTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

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

        public MySqlConvertTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments)
           => method.Name == nameof(ToString)
               ? new MySqlObjectToStringTranslator(_sqlExpressionFactory).Translate(instance, method, arguments)
               : _supportedMethods.Contains(method)
                   ? _sqlExpressionFactory.MakeUnary(ExpressionType.Convert,
                       arguments[0],
                       method.ReturnType)
                   : null;
    }
}
