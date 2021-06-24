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
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlStringMethodTranslator : IMethodCallTranslator
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly IMySqlOptions _options;

        private static readonly MethodInfo _indexOfMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] { typeof(string) });
        private static readonly MethodInfo _replaceMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Replace), new[] { typeof(string), typeof(string) });
        private static readonly MethodInfo _toLowerMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.ToLower), Array.Empty<Type>());
        private static readonly MethodInfo _toUpperMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.ToUpper), Array.Empty<Type>());
        private static readonly MethodInfo _substringMethodInfoWithOneArg
            = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int) });
        private static readonly MethodInfo _substringMethodInfoWithTwoArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });
        private static readonly MethodInfo _isNullOrWhiteSpaceMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrWhiteSpace), new[] { typeof(string) });

        // Methods defined in netcoreapp2.0 only
        private static readonly MethodInfo _trimStartMethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), Array.Empty<Type>());
        private static readonly MethodInfo _trimStartMethodInfoWithCharArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new[] { typeof(char) });
        private static readonly MethodInfo _trimEndMethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), Array.Empty<Type>());
        private static readonly MethodInfo _trimEndMethodInfoWithCharArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new[] { typeof(char) });
        private static readonly MethodInfo _trimMethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), Array.Empty<Type>());
        private static readonly MethodInfo _trimMethodInfoWithCharArg
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), new[] { typeof(char) });

        // Methods defined in netstandard2.0
        private static readonly MethodInfo _trimStartMethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new[] { typeof(char[]) });
        private static readonly MethodInfo _trimEndMethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new[] { typeof(char[]) });
        private static readonly MethodInfo _trimMethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), new[] { typeof(char[]) });

        private static readonly MethodInfo _startsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });
        private static readonly MethodInfo _containsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });
        private static readonly MethodInfo _endsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) });

        private static readonly MethodInfo _padLeftWithOneArg
            = typeof(string).GetRuntimeMethod(nameof(string.PadLeft), new[] { typeof(int) });
        private static readonly MethodInfo _padRightWithOneArg
            = typeof(string).GetRuntimeMethod(nameof(string.PadRight), new[] { typeof(int) });

        private static readonly MethodInfo _padLeftWithTwoArgs
            = typeof(string).GetRuntimeMethod(nameof(string.PadLeft), new[] { typeof(int), typeof(char) });
        private static readonly MethodInfo _padRightWithTwoArgs
            = typeof(string).GetRuntimeMethod(nameof(string.PadRight), new[] { typeof(int), typeof(char) });

        private static readonly MethodInfo _firstOrDefaultMethodInfoWithoutArgs
            = typeof(Enumerable).GetRuntimeMethods().Single(
                m => m.Name == nameof(Enumerable.FirstOrDefault)
                     && m.GetParameters().Length == 1).MakeGenericMethod(typeof(char));

        private static readonly MethodInfo _lastOrDefaultMethodInfoWithoutArgs
            = typeof(Enumerable).GetRuntimeMethods().Single(
                m => m.Name == nameof(Enumerable.LastOrDefault)
                     && m.GetParameters().Length == 1).MakeGenericMethod(typeof(char));

        private static readonly MethodInfo _removeMethodInfoWithOneArg
            = typeof(string).GetRuntimeMethod(nameof(string.Remove), new[] { typeof(int) });
        private static readonly MethodInfo _removeMethodInfoWithTwoArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Remove), new[] { typeof(int), typeof(int) });

        private static readonly MethodInfo[] _concatMethodInfos = typeof(string).GetRuntimeMethods()
            .Where(
                m => m.Name == nameof(string.Concat) &&
                     (m.GetParameters()
                          .All(p => p.ParameterType == typeof(string) ||
                                    p.ParameterType == typeof(object)) ||
                      m.GetParameters().Length == 1 &&
                      (m.GetParameters()
                          .Any(p => p.ParameterType == typeof(string[]) ||
                                    p.ParameterType == typeof(object[]) ||
                                    p.ParameterType == typeof(IEnumerable<string>))) ||
                      m.IsGenericMethodDefinition &&
                      m.GetGenericArguments().Length == 1 &&
                      m.GetParameters()
                          .Any(p => p.ParameterType.IsGenericType &&
                                    p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))))
            .ToArray();

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlStringMethodTranslator(
            MySqlSqlExpressionFactory sqlExpressionFactory,
            MySqlTypeMappingSource typeMappingSource,
            IMySqlOptions options)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _typeMappingSource = typeMappingSource;
            _options = options;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_indexOfMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory, _options)
                    .MakeIndexOfExpression(instance, arguments[0]);
            }

            if (_replaceMethodInfo.Equals(method))
            {
                var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, arguments[0], arguments[1]);
                var replacementArgument = _sqlExpressionFactory.ApplyTypeMapping(arguments[1], stringTypeMapping);
                var replaceCall = _sqlExpressionFactory.NullableFunction(
                    "REPLACE",
                    new[]
                    {
                        _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(arguments[0], stringTypeMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(arguments[1], stringTypeMapping)
                    },
                    method.ReturnType,
                    stringTypeMapping);

                // Due to a bug in all versions of MariaDB and all MySQL versions below 8.0.x (exact version that fixed the issue is
                // currently unclear), using `null` as the replacement argument in a REPLACE() call leads to unexpected results, in which
                // the call returns the original string, instead of `null`.
                // See https://jira.mariadb.org/browse/MDEV-24263
                return _sqlExpressionFactory.Case(
                    new[]
                    {
                        new CaseWhenClause(
                            _sqlExpressionFactory.IsNotNull(replacementArgument),
                            replaceCall)
                    },
                    _sqlExpressionFactory.Constant(null, RelationalTypeMapping.NullMapping));
            }

            if (_toLowerMethodInfo.Equals(method)
                || _toUpperMethodInfo.Equals(method))
            {
                return _sqlExpressionFactory.NullableFunction(
                    _toLowerMethodInfo.Equals(method) ? "LOWER" : "UPPER",
                    new[] { instance },
                    method.ReturnType,
                    instance.TypeMapping);
            }

            if (_substringMethodInfoWithOneArg.Equals(method))
            {
                return _sqlExpressionFactory.Function(
                    "SUBSTRING",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Add(
                            arguments[0],
                            _sqlExpressionFactory.Constant(1)),
                        _sqlExpressionFactory.NullableFunction(
                            "CHAR_LENGTH",
                            new[] { instance },
                            typeof(int))
                    },
                    nullable: true,
                    argumentsPropagateNullability: new[] { true, true, true },
                    method.ReturnType,
                    instance.TypeMapping);
            }

            if (_substringMethodInfoWithTwoArgs.Equals(method))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "SUBSTRING",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Add(
                            arguments[0],
                            _sqlExpressionFactory.Constant(1)),
                        arguments[1]
                    },
                    method.ReturnType,
                    instance.TypeMapping);
            }

            if (_isNullOrWhiteSpaceMethodInfo.Equals(method))
            {
                return _sqlExpressionFactory.OrElse(
                    _sqlExpressionFactory.IsNull(arguments[0]),
                    _sqlExpressionFactory.Equal(
                        ProcessTrimMethod(arguments[0], null, null),
                        _sqlExpressionFactory.Constant(string.Empty)));
            }

            if (_trimStartMethodInfoWithoutArgs?.Equals(method) == true
                || _trimStartMethodInfoWithCharArg?.Equals(method) == true
                || _trimStartMethodInfoWithCharArrayArg.Equals(method))
            {
                return ProcessTrimMethod(instance, arguments.Count > 0 ? arguments[0] : null, "LEADING");
            }

            if (_trimEndMethodInfoWithoutArgs?.Equals(method) == true
                || _trimEndMethodInfoWithCharArg?.Equals(method) == true
                || _trimEndMethodInfoWithCharArrayArg.Equals(method))
            {
                return ProcessTrimMethod(instance, arguments.Count > 0 ? arguments[0] : null, "TRAILING");
            }

            if (_trimMethodInfoWithoutArgs?.Equals(method) == true
                || _trimMethodInfoWithCharArg?.Equals(method) == true
                || _trimMethodInfoWithCharArrayArg.Equals(method))
            {
                return ProcessTrimMethod(instance, arguments.Count > 0 ? arguments[0] : null, null);
            }

            if (_containsMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory, _options)
                    .MakeContainsExpression(instance, arguments[0]);
            }

            if (_startsWithMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory, _options)
                    .MakeStartsWithExpression(instance, arguments[0]);
            }

            if (_endsWithMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory, _options)
                    .MakeEndsWithExpression(instance, arguments[0]);
            }

            if (_padLeftWithOneArg.Equals(method))
            {
                return TranslatePadLeftRight(
                    true,
                    instance,
                    arguments[0],
                    _sqlExpressionFactory.Constant(" "),
                    method.ReturnType);
            }

            if (_padRightWithOneArg.Equals(method))
            {
                return TranslatePadLeftRight(
                    false,
                    instance,
                    arguments[0],
                    _sqlExpressionFactory.Constant(" "),
                    method.ReturnType);
            }

            if (_padLeftWithTwoArgs.Equals(method))
            {
                return TranslatePadLeftRight(
                    true,
                    instance,
                    arguments[0],
                    arguments[1],
                    method.ReturnType);
            }

            if (_padRightWithTwoArgs.Equals(method))
            {
                return TranslatePadLeftRight(
                    false,
                    instance,
                    arguments[0],
                    arguments[1],
                    method.ReturnType);
            }

            if (_firstOrDefaultMethodInfoWithoutArgs.Equals(method))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "SUBSTRING",
                    new[] { arguments[0], _sqlExpressionFactory.Constant(1), _sqlExpressionFactory.Constant(1) },
                    method.ReturnType);
            }

            if (_lastOrDefaultMethodInfoWithoutArgs.Equals(method))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "SUBSTRING",
                    new[]
                    {
                        arguments[0],
                        _sqlExpressionFactory.NullableFunction(
                            "CHAR_LENGTH",
                            new[] {arguments[0]},
                            typeof(int)),
                        _sqlExpressionFactory.Constant(1)
                    },
                    method.ReturnType);
            }

            if (_removeMethodInfoWithOneArg.Equals(method))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "SUBSTRING",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Constant(1),
                        arguments[0],
                    },
                    method.ReturnType,
                    instance.TypeMapping);
            }

            if (_removeMethodInfoWithTwoArgs.Equals(method))
            {
                var firstSubString = _sqlExpressionFactory.NullableFunction(
                    "SUBSTRING",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Constant(1),
                        arguments[0]
                    },
                    method.ReturnType,
                    instance.TypeMapping);

                var secondSubString = _sqlExpressionFactory.NullableFunction(
                    "SUBSTRING",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Add(
                            _sqlExpressionFactory.Add(
                                arguments[0],
                                arguments[1]),
                            _sqlExpressionFactory.Constant(1)),
                        _sqlExpressionFactory.Subtract(
                            _sqlExpressionFactory.NullableFunction(
                                "CHAR_LENGTH",
                                new[] {instance},
                                typeof(int)),
                            _sqlExpressionFactory.Add(
                                arguments[0],
                                arguments[1])),
                    },
                    method.ReturnType,
                    instance.TypeMapping);

                var concat = _sqlExpressionFactory.NullableFunction(
                    "CONCAT",
                    new[]
                    {
                        firstSubString,
                        secondSubString
                    },
                    method.ReturnType,
                    instance.TypeMapping);

                return concat;
            }

            if (_concatMethodInfos.Contains(
                (method.IsGenericMethod
                    ? method.GetGenericMethodDefinition()
                    : null) ?? method))
            {
                // Handle
                //     string[]
                //     IEnumerable<string>
                //     object[]
                //     IEnumerable<T>
                // and
                //     string, ...
                //     object, ...
                //
                // Some call signature variants can never reach this code, because they will be directly called and thus only their result
                // is translated.
                var concatArguments = arguments[0] is MySqlComplexFunctionArgumentExpression mySqlComplexFunctionArgumentExpression
                    ? new SqlExpression[] {mySqlComplexFunctionArgumentExpression}
                    : arguments.Select(
                            e => e switch
                            {
                                SqlConstantExpression c => _sqlExpressionFactory.Constant(c.Value.ToString()),
                                SqlParameterExpression p => p.ApplyTypeMapping(
                                    ((MySqlStringTypeMapping)_typeMappingSource.GetMapping(typeof(string))).Clone(forceToString: true)),
                                _ => e,
                            })
                        .ToArray();

                // We haven't implemented expansion of MySqlComplexFunctionArgumentExpression yet, so the default nullability check would
                // result in an invalid SQL generation.
                // TODO: Fix at some point.
                return _sqlExpressionFactory.NullableFunction(
                    "CONCAT",
                    concatArguments,
                    method.ReturnType,
                    onlyNullWhenAnyNullPropagatingArgumentIsNull: arguments[0] is not MySqlComplexFunctionArgumentExpression);
            }

            return null;
        }

        private SqlExpression TranslatePadLeftRight(bool leftPad, SqlExpression instance, SqlExpression length, SqlExpression padString, Type returnType)
            => length is SqlConstantExpression && padString is SqlConstantExpression
                ? _sqlExpressionFactory.NullableFunction(
                    leftPad ? "LPAD" : "RPAD",
                    new[] {
                        instance,
                        length,
                        padString
                    },
                    returnType,
                    false)
                : null;

        private SqlExpression ProcessTrimMethod(SqlExpression instance, SqlExpression trimChar, string locationSpecifier)
        {
            // Builds a TRIM({BOTH | LEADING | TRAILING} remstr FROM str) expression.

            var sqlArguments = new List<SqlExpression>();

            if (locationSpecifier != null)
            {
                sqlArguments.Add(_sqlExpressionFactory.Fragment(locationSpecifier));
            }

            if (trimChar != null)
            {
                var constantValue = (trimChar as SqlConstantExpression)?.Value;

                if (constantValue is char singleChar)
                {
                    sqlArguments.Add(_sqlExpressionFactory.Constant(singleChar));
                }
                else if (constantValue is char[] charArray && charArray.Length <= 1)
                {
                    if (charArray.Length == 1)
                    {
                        sqlArguments.Add(_sqlExpressionFactory.Constant(charArray[0]));
                    }
                }
                else
                {
                    return null;
                }
            }

            if (sqlArguments.Count > 0)
            {
                sqlArguments.Add(_sqlExpressionFactory.Fragment("FROM"));
            }

            sqlArguments.Add(instance);

            return _sqlExpressionFactory.NullableFunction(
                "TRIM",
                new[]
                {
                    _sqlExpressionFactory.ComplexFunctionArgument(
                        sqlArguments.ToArray(),
                        " ",
                        typeof(string)),
                },
                typeof(string));
        }
    }
}
