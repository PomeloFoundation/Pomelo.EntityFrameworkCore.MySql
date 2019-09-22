// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDbFunctionsExtensionsMethodTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        private static readonly Type[] _supportedTypes = {
            typeof(int),
            typeof(long),
            typeof(DateTime),
            typeof(Guid),
            typeof(bool),
            typeof(byte),
            typeof(byte[]),
            typeof(double),
            typeof(DateTimeOffset),
            typeof(char),
            typeof(short),
            typeof(float),
            typeof(decimal),
            typeof(TimeSpan),
            typeof(uint),
            typeof(ushort),
            typeof(ulong),
            typeof(sbyte),
            typeof(int?),
            typeof(long?),
            typeof(DateTime?),
            typeof(Guid?),
            typeof(bool?),
            typeof(byte?),
            typeof(double?),
            typeof(DateTimeOffset?),
            typeof(char?),
            typeof(short?),
            typeof(float?),
            typeof(decimal?),
            typeof(TimeSpan?),
            typeof(uint?),
            typeof(ushort?),
            typeof(ulong?),
            typeof(sbyte?),
        };

        private static readonly MethodInfo[] _methodInfos
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethods()
                .Where(method => method.Name == nameof(MySqlDbFunctionsExtensions.Like)
                                 && method.IsGenericMethod)
                .SelectMany(method => _supportedTypes.Select(type => method.MakeGenericMethod(type))).ToArray();

        public MySqlDbFunctionsExtensionsMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            return _methodInfos.Any(m => Equals(method, m))
                       ? method.GetParameters().Length == 3
                           ? _sqlExpressionFactory.Like(arguments[1], arguments[2])
                           : _sqlExpressionFactory.Like(arguments[1], arguments[2], arguments[3])
                       : null;
        }
    }
}
