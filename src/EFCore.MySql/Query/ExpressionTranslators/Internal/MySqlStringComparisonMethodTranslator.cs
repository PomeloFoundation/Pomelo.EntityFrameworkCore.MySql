// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlStringComparisonMethodTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _equalsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Equals), new[] {typeof(string), typeof(StringComparison)});

        private static readonly MethodInfo _staticEqualsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Equals), new[] {typeof(string), typeof(string), typeof(StringComparison)});

        private static readonly MethodInfo _startsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] {typeof(string), typeof(StringComparison)});

        private static readonly MethodInfo _endsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] {typeof(string), typeof(StringComparison)});

        private static readonly MethodInfo _containsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] {typeof(string), typeof(StringComparison)});

        private static readonly MethodInfo _indexOfMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] {typeof(string), typeof(StringComparison)});

        internal static readonly MethodInfo[] StringComparisonMethodInfos =
        {
            _equalsMethodInfo,
            _staticEqualsMethodInfo,
            _startsWithMethodInfo,
            _endsWithMethodInfo,
            _containsMethodInfo,
            _indexOfMethodInfo,
        };

        internal static readonly MethodInfo[] RelationalErrorHandledStringComparisonMethodInfos =
        {
            _equalsMethodInfo,
            _staticEqualsMethodInfo,
        };

        private readonly SqlExpression _caseSensitiveComparisons;

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;

        public MySqlStringComparisonMethodTranslator(ISqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            _options = options;
            _caseSensitiveComparisons = _sqlExpressionFactory.Constant(
                new[] {StringComparison.Ordinal, StringComparison.CurrentCulture, StringComparison.InvariantCulture});
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (Equals(method, _equalsMethodInfo) && instance != null && _options.StringComparisonTranslations)
            {
                return MakeStringEqualsExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }

            if (Equals(method, _staticEqualsMethodInfo) && _options.StringComparisonTranslations)
            {
                return MakeStringEqualsExpression(
                    arguments[0],
                    arguments[1],
                    arguments[2]
                );
            }

            if (Equals(method, _startsWithMethodInfo) && instance != null && _options.StringComparisonTranslations)
            {
                return MakeStartsWithExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }

            if (Equals(method, _endsWithMethodInfo) && instance != null && _options.StringComparisonTranslations)
            {
                return MakeEndsWithExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }

            if (Equals(method, _containsMethodInfo) && instance != null && _options.StringComparisonTranslations)
            {
                return MakeContainsExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }

            if (Equals(method, _indexOfMethodInfo) && instance != null && _options.StringComparisonTranslations)
            {
                return MakeIndexOfExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }

            return null;
        }

        public virtual SqlExpression MakeStringEqualsExpression(
            [NotNull] SqlExpression leftValue,
            [NotNull] SqlExpression rightValue,
            [NotNull] SqlExpression stringComparison)
        {
            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () =>
                    {
                        if (leftValue is ColumnExpression)
                        {
                            // Applying the binary operator to the non-column value enables SQL to
                            // utilize an index if one exists.
                            return _sqlExpressionFactory.Equal(
                                leftValue,
                                Utf8Bin(rightValue));
                        }

                        return _sqlExpressionFactory.Equal(
                            Utf8Bin(leftValue),
                            rightValue);
                    },
                    () => _sqlExpressionFactory.Equal(
                        LCase(leftValue),
                        Utf8Bin(LCase(rightValue))));
            }

            return new CaseExpression(
                new[]
                {
                    new CaseWhenClause(
                        _sqlExpressionFactory.In(stringComparison, _caseSensitiveComparisons, false),
                        // Case sensitive, accent sensitive
                        _sqlExpressionFactory.Equal(
                            leftValue,
                            Utf8Bin(rightValue)
                        )
                    )
                },
                // Case insensitive, accent sensitive
                _sqlExpressionFactory.Equal(
                    LCase(leftValue),
                    Utf8Bin(LCase(rightValue)))
            );
        }

        public virtual SqlExpression MakeStartsWithExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression prefix,
            [CanBeNull] SqlExpression stringComparison = null)
        {
            if (stringComparison == null)
            {
                return MakeStartsWithEndsWithExpressionImpl(
                    target,
                    e => e,
                    prefix,
                    e => e,
                    true);
            }

            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () => MakeStartsWithEndsWithExpressionImpl(
                        target,
                        e => e,
                        prefix,
                        e => Utf8Bin(e),
                        true),
                    () => MakeStartsWithEndsWithExpressionImpl(
                        LCase(target),
                        e => LCase(e),
                        prefix,
                        e => Utf8Bin(LCase(e)),
                        true));
            }

            return new CaseExpression(
                new[]
                {
                    new CaseWhenClause(
                        _sqlExpressionFactory.In(
                            stringComparison,
                            _caseSensitiveComparisons,
                            false),
                        // Case sensitive, accent sensitive
                        MakeStartsWithEndsWithExpressionImpl(
                            target,
                            e => e,
                            prefix,
                            e => Utf8Bin(prefix),
                            true))
                },
                // Case insensitive, accent sensitive
                MakeStartsWithEndsWithExpressionImpl(
                    target,
                    e => LCase(e),
                    prefix,
                    e => Utf8Bin(LCase(e)),
                    true));
        }

        public virtual SqlExpression MakeEndsWithExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression suffix,
            [CanBeNull] SqlExpression stringComparison = null)
        {
            if (stringComparison == null)
            {
                return MakeStartsWithEndsWithExpressionImpl(
                    target,
                    e => e,
                    suffix,
                    e => e,
                    false);
            }

            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () => MakeStartsWithEndsWithExpressionImpl(
                        target,
                        e => e,
                        suffix,
                        e => Utf8Bin(e),
                        false),
                    () => MakeStartsWithEndsWithExpressionImpl(
                        target,
                        e => LCase(e),
                        suffix,
                        e => Utf8Bin(LCase(e)),
                        false));
            }

            return new CaseExpression(
                new[]
                {
                    new CaseWhenClause(
                        _sqlExpressionFactory.In(
                            stringComparison,
                            _caseSensitiveComparisons,
                            false),
                        // Case sensitive, accent sensitive
                        MakeStartsWithEndsWithExpressionImpl(
                            target,
                            e => e,
                            suffix,
                            e => Utf8Bin(e),
                            false))
                },
                // Case insensitive, accent sensitive
                MakeStartsWithEndsWithExpressionImpl(
                    target,
                    e => LCase(e),
                    suffix,
                    e => Utf8Bin(LCase(e)),
                    false));
        }

        public virtual SqlExpression MakeContainsExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression search,
            [CanBeNull] SqlExpression stringComparison = null)
        {
            // Check, whether we should generate an optimized expression, that uses the current database
            // settings instead of an explicit string comparison value.
            if (stringComparison == null)
            {
                return MakeContainsExpressionImpl(
                    target,
                    e => e,
                    search,
                    e => e);
            }

            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () =>
                        MakeContainsExpressionImpl(
                            target,
                            e => e,
                            search,
                            e => Utf8Bin(e)
                        ),
                    () =>
                        MakeContainsExpressionImpl(
                            target,
                            e => LCase(e),
                            search,
                            e => Utf8Bin(LCase(e))
                        )
                );
            }

            return new CaseExpression(
                new[]
                {
                    new CaseWhenClause(
                        _sqlExpressionFactory.In(stringComparison, _caseSensitiveComparisons, false),
                        // Case sensitive, accent sensitive
                        MakeContainsExpressionImpl(
                            target,
                            e => e,
                            search,
                            e => Utf8Bin(e)
                        )
                    )
                },
                // Case insensitive, accent sensitive
                MakeContainsExpressionImpl(
                    target,
                    e => LCase(e),
                    search,
                    e => Utf8Bin(LCase(e))
                )
            );
        }

        private SqlExpression MakeStartsWithEndsWithExpressionImpl(
            SqlExpression target,
            [NotNull] Func<SqlExpression, SqlExpression> targetTransform,
            SqlExpression prefixSuffix,
            [NotNull] Func<SqlExpression, SqlExpression> prefixSuffixTransform,
            bool startsWith)
        {
            var stringTypeMapping = ExpressionExtensions.InferTypeMapping(target, prefixSuffix);
            target = _sqlExpressionFactory.ApplyTypeMapping(target, stringTypeMapping);
            prefixSuffix = _sqlExpressionFactory.ApplyTypeMapping(prefixSuffix, stringTypeMapping);

            if (prefixSuffix is SqlConstantExpression constantPrefixSuffixExpression)
            {
                // The prefix is constant. Aside from null or empty, we escape all special characters (%, _, \)
                // in C# and send a simple LIKE.
                if (constantPrefixSuffixExpression.Value is string constantPrefixSuffixString)
                {
                    // TRUE (pattern == "")
                    // something LIKE 'foo%' (pattern != "", StartsWith())
                    // something LIKE '%foo' (pattern != "", EndsWith())
                    return constantPrefixSuffixString == string.Empty
                        ? (SqlExpression)_sqlExpressionFactory.Constant(true)
                        : _sqlExpressionFactory.Like(
                            targetTransform(target),
                            prefixSuffixTransform(
                                _sqlExpressionFactory.Constant(
                                    (startsWith
                                        ? string.Empty
                                        : "%") +
                                    EscapeLikePattern(constantPrefixSuffixString) +
                                    (startsWith
                                        ? "%"
                                        : string.Empty))));
                }

                // https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-607876040
                // Can return NULL in .NET 5 after https://github.com/dotnet/efcore/issues/20498 has been fixed.
                // `something LIKE NULL` always returns `NULL`. We will return `false`, to indicate, that no match
                // could be found, because returning a constant of `NULL` will throw later in EF Core when used as
                // a predicate.
                // return _sqlExpressionFactory.Constant(null, RelationalTypeMapping.NullMapping);
                // This results in NULL anyway, but works around EF Core's inability to handle predicates that are
                // constant null values.
                return _sqlExpressionFactory.Like(target, _sqlExpressionFactory.Constant(null, stringTypeMapping));
            }

            // TODO: Generally, LEFT & compare is faster than escaping potential pattern characters with REPLACE().
            // However, this might not be the case, if the pattern is constant after all (e.g. `LCASE('fo%o')`), in
            // which case, `something LIKE CONCAT(REPLACE(REPLACE(LCASE('fo%o'), '%', '\\%'), '_', '\\_'), '%')` should
            // be faster than `LEFT(something, CHAR_LENGTH('fo%o')) = LCASE('fo%o')`.
            // See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-607733553

            // The prefix is non-constant, we use LEFT to extract the substring and compare.
            return _sqlExpressionFactory.Equal(
                _sqlExpressionFactory.NullableFunction(
                    startsWith
                        ? "LEFT"
                        : "RIGHT",
                    new[] {targetTransform(target), CharLength(prefixSuffix)},
                    typeof(string),
                    stringTypeMapping),
                prefixSuffixTransform(prefixSuffix));
        }

        private SqlExpression MakeContainsExpressionImpl(
            SqlExpression target,
            [NotNull] Func<SqlExpression, SqlExpression> targetTransform,
            SqlExpression pattern,
            [NotNull] Func<SqlExpression, SqlExpression> patternTransform)
        {
            var stringTypeMapping = ExpressionExtensions.InferTypeMapping(target, pattern);
            target = _sqlExpressionFactory.ApplyTypeMapping(target, stringTypeMapping);
            pattern = _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping);

            if (pattern is SqlConstantExpression constantPatternExpression)
            {
                // The prefix is constant. Aside from null or empty, we escape all special characters (%, _, \)
                // in C# and send a simple LIKE.
                if (constantPatternExpression.Value is string constantPatternString)
                {
                    // TRUE (pattern == "")
                    // something LIKE '%foo%' (pattern != "")
                    return constantPatternString == string.Empty
                        ? (SqlExpression)_sqlExpressionFactory.Constant(true)
                        : _sqlExpressionFactory.Like(
                            targetTransform(target),
                            patternTransform(_sqlExpressionFactory.Constant('%' + EscapeLikePattern(constantPatternString) + '%')));
                }

                // TODO: EF Core 5
                // https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-607876040
                // Can return NULL in .NET 5 after https://github.com/dotnet/efcore/issues/20498 has been fixed.
                // `something LIKE NULL` always returns `NULL`. We will return `false`, to indicate, that no match
                // could be found, because returning a constant of `NULL` will throw later in EF Core when used as
                // a predicate.
                // return _sqlExpressionFactory.Constant(null, RelationalTypeMapping.NullMapping);
                // This results in NULL anyway, but works around EF Core's inability to handle predicates that are
                // constant null values.
                return _sqlExpressionFactory.Like(target, _sqlExpressionFactory.Constant(null, stringTypeMapping));
            }

            // 'foo' LIKE '' OR LOCATE('foo', 'barfoobar') > 0
            // This cannot be "'   ' = '' OR ..", because '   ' would be trimmed to '' when using equals, but not when using LIKE.
            // Using an empty pattern `LOCATE('', 'barfoobar')` returns 1.
            return _sqlExpressionFactory.OrElse(
                _sqlExpressionFactory.Like(
                    pattern,
                    _sqlExpressionFactory.Constant(string.Empty, stringTypeMapping)),
                _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.NullableFunction(
                        "LOCATE",
                        new[] {patternTransform(pattern), targetTransform(target)},
                        typeof(int)),
                        _sqlExpressionFactory.Constant(0)));
        }

        public virtual SqlExpression MakeIndexOfExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression search,
            [CanBeNull] SqlExpression stringComparison = null)
        {
            if (stringComparison == null)
            {
                return MakeIndexOfExpressionImpl(
                    target,
                    e => e,
                    search,
                    e => e);
            }

            // Users have to opt-in, to use string method translations with an explicit StringComparison parameter.
            if (!_options.StringComparisonTranslations)
            {
                return null;
            }

            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () => MakeIndexOfExpressionImpl(
                        target,
                        e => e,
                        search,
                        e => Utf8Bin(e)),
                    () => MakeIndexOfExpressionImpl(
                        target,
                        e => LCase(e),
                        search,
                        e => Utf8Bin(LCase(e))));
            }

            return _sqlExpressionFactory.Case(
                new[]
                {
                    new CaseWhenClause(
                        _sqlExpressionFactory.In(
                            stringComparison,
                            _caseSensitiveComparisons,
                            false),
                        // Case sensitive, accent sensitive
                        MakeIndexOfExpressionImpl(
                            target,
                            e => e,
                            search,
                            e => Utf8Bin(e)))
                },
                // Case insensitive, accent sensitive
                MakeIndexOfExpressionImpl(
                    target,
                    e => LCase(e),
                    search,
                    e => Utf8Bin(LCase(e))));
        }

        private SqlExpression MakeIndexOfExpressionImpl(
            SqlExpression target,
            [NotNull] Func<SqlExpression, SqlExpression> targetTransform,
            SqlExpression pattern,
            [NotNull] Func<SqlExpression, SqlExpression> patternTransform)
        {
            // LOCATE('foo', 'barfoobar') - 1
            // Using an empty pattern `LOCATE('', 'barfoobar') - 1` returns 0.
            return _sqlExpressionFactory.Subtract(
                _sqlExpressionFactory.NullableFunction(
                    "LOCATE",
                    new[] {patternTransform(pattern), targetTransform(target)},
                    typeof(int)),
                _sqlExpressionFactory.Constant(1));
        }

        private static bool TryGetExpressionValue<T>(SqlExpression expression, out T value)
        {
            if (expression.Type != typeof(T))
            {
                throw new ArgumentException(
                    MySqlStrings.ExpressionTypeMismatch,
                    nameof(expression)
                );
            }

            if (expression is SqlConstantExpression constant)
            {
                value = (T)constant.Value;
                return true;
            }

            value = default;
            return false;
        }

        private static SqlExpression CreateExpressionForCaseSensitivity(
            StringComparison cmp,
            Func<SqlExpression> ifCaseSensitive,
            Func<SqlExpression> ifCaseInsensitive)
            => cmp switch
            {
                StringComparison.Ordinal => ifCaseSensitive(),
                StringComparison.CurrentCulture => ifCaseSensitive(),
                StringComparison.InvariantCulture => ifCaseSensitive(),
                StringComparison.OrdinalIgnoreCase => ifCaseInsensitive(),
                StringComparison.CurrentCultureIgnoreCase => ifCaseInsensitive(),
                StringComparison.InvariantCultureIgnoreCase => ifCaseInsensitive(),
                _ => default
            };

        private SqlExpression LCase(SqlExpression value)
            => _sqlExpressionFactory.NullableFunction(
                "LCASE",
                new[] {value},
                value.Type);

        private SqlExpression Utf8Bin(SqlExpression value)
            => _sqlExpressionFactory.Collate(
                value,
                "utf8mb4",
                "utf8mb4_bin"
            );

        private SqlExpression CharLength(SqlExpression value)
            => _sqlExpressionFactory.NullableFunction(
                "CHAR_LENGTH",
                new[] {value},
                typeof(int));

        private const char LikeEscapeChar = '\\';

        private static bool IsLikeWildChar(char c) => c == '%' || c == '_';

        private static string EscapeLikePattern(string pattern)
        {
            var builder = new StringBuilder();
            foreach (var c in pattern)
            {
                if (IsLikeWildChar(c) ||
                    c == LikeEscapeChar)
                {
                    builder.Append(LikeEscapeChar);
                }

                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
