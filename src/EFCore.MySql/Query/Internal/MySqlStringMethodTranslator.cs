using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

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

        // Method defined in netcoreapp2.0 only
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

        // Method defined in netstandard2.0
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
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        private const char LikeEscapeChar = '\\';

        public MySqlStringMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (_indexOfMethodInfo.Equals(method))
            {
                var argument = arguments[0];
                var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, argument);

                return _sqlExpressionFactory.Subtract(
                    _sqlExpressionFactory.Function(
                        "LOCATE",
                        new[]
                        {
                            _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping),
                            _sqlExpressionFactory.ApplyTypeMapping(argument, stringTypeMapping)
                        },
                        method.ReturnType),
                    _sqlExpressionFactory.Constant(1));
            }

            if (_replaceMethodInfo.Equals(method))
            {
                var firstArgument = arguments[0];
                var secondArgument = arguments[1];
                var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, firstArgument, secondArgument);

                return _sqlExpressionFactory.Function(
                    "REPLACE",
                    new[]
                    {
                        _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(firstArgument, stringTypeMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(secondArgument, stringTypeMapping)
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
                var argument = arguments[0];

                return ProcessTrimMethod(instance, arguments, null);
            }

            if (_trimStartMethodInfoWithoutArgs?.Equals(method) == true
                || _trimStartMethodInfoWithCharArg?.Equals(method) == true
                || _trimStartMethodInfoWithCharArrayArg.Equals(method))
            {
                return ProcessTrimMethod(instance, arguments, "LEADING");
            }

            if (_trimEndMethodInfoWithoutArgs?.Equals(method) == true
                || _trimEndMethodInfoWithCharArg?.Equals(method) == true
                || _trimEndMethodInfoWithCharArrayArg.Equals(method))
            {
                return ProcessTrimMethod(instance, arguments, "TRAILING");
            }

            if (_trimMethodInfoWithoutArgs?.Equals(method) == true
                || _trimMethodInfoWithCharArg?.Equals(method) == true
                || _trimMethodInfoWithCharArrayArg.Equals(method))
            {
                return ProcessTrimMethod(instance, arguments, null);
            }

            if (_containsMethodInfo.Equals(method))
            {
                var pattern = arguments[0];
                var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, pattern);

                instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);
                pattern = _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping);

                var locateCondition = _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.Function(
                        "LOCATE",
                        new[]
                        {
                            new MySqlComplexFunctionArgumentExpression(
                                new[]
                                {
                                    _sqlExpressionFactory.Fragment("BINARY"), // for case sensitivity
                                    pattern,
                                },
                                typeof(string)),
                            instance,
                        },
                        typeof(int)),
                    _sqlExpressionFactory.Constant(0));

                if (pattern is SqlConstantExpression constantPattern)
                {
                    if ((string)constantPattern.Value == string.Empty)
                    {
                        return _sqlExpressionFactory.Constant(true);
                    }

                    return locateCondition;
                }

                return _sqlExpressionFactory.OrElse(
                    _sqlExpressionFactory.Equal(
                        pattern,
                        _sqlExpressionFactory.Constant(string.Empty, stringTypeMapping)),
                    locateCondition);
            }

            if (_startsWithMethodInfo.Equals(method))
            {
                return TranslateStartsEndsWith(instance, arguments[0], true);
            }

            if (_endsWithMethodInfo.Equals(method))
            {
                return TranslateStartsEndsWith(instance, arguments[0], false);
            }

            return null;
        }

        private SqlExpression TranslateStartsEndsWith(SqlExpression instance, SqlExpression pattern, bool startsWith)
        {
            var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, pattern);

            instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);
            pattern = _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping);

            if (pattern is SqlConstantExpression constantExpression)
            {
                // The pattern is constant. Aside from null or empty, we escape all special characters (%, _, \)
                // in C# and send a simple LIKE
                if (!(constantExpression.Value is string constantString))
                {
                    return _sqlExpressionFactory.Like(instance, _sqlExpressionFactory.Constant(null, stringTypeMapping));
                }
                if (constantString.Length == 0)
                {
                    return _sqlExpressionFactory.Constant(true);
                }

                return constantString.Any(c => IsLikeWildChar(c))
                    ? _sqlExpressionFactory.Like(
                        instance,
                        _sqlExpressionFactory.Constant(
                            startsWith
                                ? EscapeLikePattern(constantString) + '%'
                                : '%' + EscapeLikePattern(constantString)),
                        _sqlExpressionFactory.Constant(LikeEscapeChar.ToString()))
                    : _sqlExpressionFactory.Like(
                        instance,
                        _sqlExpressionFactory.Constant(startsWith ? constantString + '%' : '%' + constantString),
                        null);
            }

            // The pattern is non-constant, we use LEFT or RIGHT to extract substring and compare.
            // For StartsWith we also first run a LIKE to quickly filter out most non-matching results (sargable, but imprecise
            // because of wildchars).
            if (startsWith)
            {
                return _sqlExpressionFactory.AndAlso(
                    _sqlExpressionFactory.Like(
                        instance,
                        _sqlExpressionFactory.Add(
                            instance,
                            _sqlExpressionFactory.Constant("%"))),
                    _sqlExpressionFactory.Equal(
                        _sqlExpressionFactory.Function(
                            "LEFT",
                            new[] {
                                instance,
                                _sqlExpressionFactory.Function("CHAR_LENGTH", new[] { pattern }, typeof(int))
                            },
                            typeof(string),
                            stringTypeMapping),
                        pattern));
            }

            return _sqlExpressionFactory.Equal(
                _sqlExpressionFactory.Function(
                    "RIGHT",
                    new[] {
                        instance,
                        _sqlExpressionFactory.Function("CHAR_LENGTH", new[] { pattern }, typeof(int))
                    },
                    typeof(string),
                    stringTypeMapping),
                pattern);
        }

        // See https://dev.mysql.com/doc/refman/8.0/en/string-comparison-functions.html#operator_like
        private bool IsLikeWildChar(char c) => c == '%' || c == '_';

        private string EscapeLikePattern(string pattern)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < pattern.Length; i++)
            {
                var c = pattern[i];
                if (IsLikeWildChar(c) || c == LikeEscapeChar)
                {
                    builder.Append(LikeEscapeChar);
                }
                builder.Append(c);
            }
            return builder.ToString();
        }

        private SqlExpression ProcessTrimMethod(SqlExpression instance, IReadOnlyList<SqlExpression> arguments, string locationSpecifier)
        {
            // Builds a TRIM({BOTH | LEADING | TRAILING} remstr FROM str) expression.

            var typeMapping = instance.TypeMapping;
            if (typeMapping == null)
            {
                return null;
            }

            var sqlArguments = new List<SqlExpression>();

            if (locationSpecifier != null)
            {
                sqlArguments.Add(_sqlExpressionFactory.Fragment(locationSpecifier));
            }

            if (arguments.Count == 1)
            {
                var constantValue = (arguments[0] as SqlConstantExpression)?.Value;
                var charactersToTrim = new List<char>();

                if (constantValue is char singleChar)
                {
                    charactersToTrim.Add(singleChar);
                }
                else if (constantValue is char[] charArray)
                {
                    charactersToTrim.AddRange(charArray);
                }
                else
                {
                    return null;
                }

                if (charactersToTrim.Count > 0)
                {
                    sqlArguments.Add(_sqlExpressionFactory.Constant(new string(charactersToTrim.ToArray()), typeMapping));
                }
            }

            if (sqlArguments.Count > 0)
            {
                sqlArguments.Add(_sqlExpressionFactory.Fragment("FROM"));
            }

            return _sqlExpressionFactory.Function(
                "TRIM",
                new[] { new MySqlComplexFunctionArgumentExpression(
                    sqlArguments.ToArray(),
                    typeof(string)),
                },
                typeof(string),
                typeMapping);
        }
    }
}
