// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

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
                                 && method.IsGenericMethod
                                 && method.GetParameters().Length >= 3 && method.GetParameters().Length <= 4)
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
            if (_methodInfos.Any(m => Equals(method, m)))
            {
                var match = _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]);

                var pattern = InferStringTypeMappingOrApplyDefault(
                    arguments[2],
                    match.TypeMapping);

                var excapeChar = arguments.Count == 4
                    ? InferStringTypeMappingOrApplyDefault(
                        arguments[3],
                        match.TypeMapping)
                    : null;

                return _sqlExpressionFactory.Like(
                    match,
                    pattern,
                    excapeChar);
            }

            return null;
        }

        private SqlExpression InferStringTypeMappingOrApplyDefault(SqlExpression expression, RelationalTypeMapping inferenceSourceTypeMapping)
        {
            if (expression == null)
            {
                return null;
            }

            if (expression.TypeMapping != null)
            {
                return expression;
            }

            if (expression.Type == typeof(string) && inferenceSourceTypeMapping?.ClrType == typeof(string))
            {
                return _sqlExpressionFactory.ApplyTypeMapping(expression, inferenceSourceTypeMapping);
            }
            
            return _sqlExpressionFactory.ApplyDefaultTypeMapping(expression);
        }
    }
}
