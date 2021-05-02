// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDbFunctionsExtensionsMethodTranslator : IMethodCallTranslator
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        private static readonly Type[] _supportedLikeTypes = {
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

        private static readonly MethodInfo[] _likeMethodInfos
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethods()
                .Where(method => method.Name == nameof(MySqlDbFunctionsExtensions.Like)
                                 && method.IsGenericMethod
                                 && method.GetParameters().Length >= 3 && method.GetParameters().Length <= 4)
                .SelectMany(method => _supportedLikeTypes.Select(type => method.MakeGenericMethod(type))).ToArray();

        private static readonly MethodInfo _matchMethodInfo
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                nameof(MySqlDbFunctionsExtensions.Match),
                new[] {typeof(DbFunctions), typeof(string), typeof(string), typeof(MySqlMatchSearchMode)});

        private static readonly MethodInfo _matchWithMultiplePropertiesMethodInfo
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                nameof(MySqlDbFunctionsExtensions.Match),
                new[] {typeof(DbFunctions), typeof(string[]), typeof(string), typeof(MySqlMatchSearchMode)});

        private static readonly Type[] _supportedHexTypes = {
            typeof(string),
            typeof(byte[]),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(int?),
            typeof(long?),
            typeof(short?),
            typeof(sbyte?),
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(byte),
            typeof(uint?),
            typeof(ulong?),
            typeof(ushort?),
            typeof(byte?),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(decimal?),
            typeof(double?),
            typeof(float?),
        };

        private static readonly MethodInfo[] _hexMethodInfos
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethods()
                .Where(method => method.Name == nameof(MySqlDbFunctionsExtensions.Hex) &&
                                 method.IsGenericMethod)
                .SelectMany(method => _supportedHexTypes.Select(type => method.MakeGenericMethod(type)))
                .ToArray();

        private static readonly MethodInfo _unhexMethodInfo = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.Unhex), new[] {typeof(DbFunctions), typeof(string)});

        public MySqlDbFunctionsExtensionsMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_likeMethodInfos.Any(m => Equals(method, m)))
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

            if (Equals(method, _matchMethodInfo) ||
                Equals(method, _matchWithMultiplePropertiesMethodInfo))
            {
                if (arguments[3] is SqlConstantExpression constant)
                {
                    return _sqlExpressionFactory.MakeMatch(
                        arguments[1],
                        arguments[2],
                        (MySqlMatchSearchMode)constant.Value);
                }

                if (arguments[3] is SqlParameterExpression parameter)
                {
                    // Use nested OR clauses here, because MariaDB does not support MATCH...AGAINST from inside of
                    // CASE statements and the nested OR clauses use the fulltext index, while using CASE does not:
                    // <search_mode_1> = @p AND MATCH ... AGAINST ... OR
                    // <search_mode_2> = @p AND MATCH ... AGAINST ... OR [...]
                    var andClauses = Enum.GetValues(typeof(MySqlMatchSearchMode))
                        .Cast<MySqlMatchSearchMode>()
                        .OrderByDescending(m => m)
                        .Select(m => _sqlExpressionFactory.AndAlso(
                            _sqlExpressionFactory.Equal(parameter, _sqlExpressionFactory.Constant(m)),
                            _sqlExpressionFactory.MakeMatch(arguments[1], arguments[2], m)))
                        .ToArray();

                    return andClauses
                        .Skip(1)
                        .Aggregate(
                            andClauses.First(),
                            (currentAnd, previousExpression) => _sqlExpressionFactory.OrElse(previousExpression, currentAnd));
                }
            }

            if (_hexMethodInfos.Any(m => Equals(method, m)))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "HEX",
                    new[] {arguments[1]},
                    typeof(string));
            }

            if (Equals(method, _unhexMethodInfo))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "UNHEX",
                    new[] {arguments[1]},
                    typeof(string),
                    false);
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
