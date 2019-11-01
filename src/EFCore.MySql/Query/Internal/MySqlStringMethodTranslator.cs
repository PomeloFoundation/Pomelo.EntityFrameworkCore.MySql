using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlStringMethodTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _indexOfMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] { typeof(string) });
        private static readonly MethodInfo _replaceMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Replace), new[] { typeof(string), typeof(string) });
        private static readonly MethodInfo _toLowerMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.ToLower), Array.Empty<Type>());
        private static readonly MethodInfo _toUpperMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.ToUpper), Array.Empty<Type>());
        private static readonly MethodInfo _substringMethodInfo
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

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlStringMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (_indexOfMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory)
                    .MakeIndexOfExpression(instance, arguments[0], _sqlExpressionFactory.Constant(StringComparison.CurrentCulture));
            }

            if (_replaceMethodInfo.Equals(method))
            {
                var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, arguments[0], arguments[1]);

                return _sqlExpressionFactory.Function(
                    "REPLACE",
                    new[]
                    {
                        _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(arguments[0], stringTypeMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(arguments[1], stringTypeMapping)
                    },
                    method.ReturnType,
                    stringTypeMapping);
            }

            if (_toLowerMethodInfo.Equals(method)
                || _toUpperMethodInfo.Equals(method))
            {
                return _sqlExpressionFactory.Function(
                    _toLowerMethodInfo.Equals(method) ? "LOWER" : "UPPER",
                    new[] { instance },
                    method.ReturnType,
                    instance.TypeMapping);
            }

            if (_substringMethodInfo.Equals(method))
            {
                return _sqlExpressionFactory.Function(
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
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory)
                    .MakeContainsExpression(instance, arguments[0], _sqlExpressionFactory.Constant(StringComparison.Ordinal));
            }

            if (_startsWithMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory)
                    .MakeStartsWithExpression(instance, arguments[0], _sqlExpressionFactory.Constant(StringComparison.CurrentCulture));
            }

            if (_endsWithMethodInfo.Equals(method))
            {
                return new MySqlStringComparisonMethodTranslator(_sqlExpressionFactory)
                    .MakeEndsWithExpression(instance, arguments[0], _sqlExpressionFactory.Constant(StringComparison.CurrentCulture));
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

            return null;
        }

        private SqlExpression TranslatePadLeftRight(bool leftPad, SqlExpression instance, SqlExpression length, SqlExpression padString, Type returnType)
            => length is SqlConstantExpression && padString is SqlConstantExpression
                ? _sqlExpressionFactory.Function(
                    leftPad ? "LPAD" : "RPAD",
                    new[] {
                        instance,
                        length,
                        padString
                    },
                    returnType)
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

            return _sqlExpressionFactory.Function(
                "TRIM",
                new[] { _sqlExpressionFactory.ComplexFunctionArgument(
                    sqlArguments.ToArray(),
                    typeof(string)),
                },
                typeof(string));
        }
    }
}
