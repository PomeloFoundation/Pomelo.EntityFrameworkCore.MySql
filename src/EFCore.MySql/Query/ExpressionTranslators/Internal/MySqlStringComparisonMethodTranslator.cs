using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlStringComparisonMethodTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _equalsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _staticEqualsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Equals), new[] { typeof(string), typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _startsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _endsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _containsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _indexOfMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) });

        private readonly SqlExpression _caseSensitiveComparisons;

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlStringComparisonMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            _caseSensitiveComparisons = _sqlExpressionFactory.Constant(
                new []
                {
                    StringComparison.Ordinal,
                    StringComparison.CurrentCulture,
                    StringComparison.InvariantCulture
                });
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (Equals(method, _equalsMethodInfo) && instance != null)
            {
                return MakeStringEqualsExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }
            else if (Equals(method, _staticEqualsMethodInfo))
            {
                return MakeStringEqualsExpression(
                    arguments[0],
                    arguments[1],
                    arguments[2]
                );
            }
            else if (Equals(method, _startsWithMethodInfo) && instance != null)
            {
                return MakeStartsWithExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }
            else if (Equals(method, _endsWithMethodInfo) && instance != null)
            {
                return MakeEndsWithExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }
            else if (Equals(method, _containsMethodInfo) && instance != null)
            {
                return MakeContainsExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }
            else if (Equals(method, _indexOfMethodInfo) && instance != null)
            {
                return MakeIndexOfExpression(
                    instance,
                    arguments[0],
                    arguments[1]
                );
            }

            return null;
        }

        public SqlExpression MakeStringEqualsExpression(
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
                                Utf8Bin(rightValue)
                            );
                        }
                        else
                        {
                            return _sqlExpressionFactory.Equal(
                                Utf8Bin(leftValue),
                                rightValue
                            );
                        }
                    },
                    () =>
                        _sqlExpressionFactory.Equal(
                            LCase(leftValue),
                            Utf8Bin(LCase(rightValue))
                        )
                );
            }
            else
            {
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
                        Utf8Bin(LCase(rightValue))
                    )
                );
            }
        }

        public SqlExpression MakeStartsWithExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression prefix,
            [NotNull] SqlExpression stringComparison)
        {
            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () =>
                        MakeStartsWithExpressionImpl(
                            target,
                            Utf8Bin(prefix),
                            originalPrefix: prefix
                        ),
                    () =>
                        MakeStartsWithExpressionImpl(
                            LCase(target),
                            Utf8Bin(LCase(prefix))
                        )
                );
            }
            else
            {
                return new CaseExpression(
                    new[]
                    {
                        new CaseWhenClause(
                            _sqlExpressionFactory.In(stringComparison, _caseSensitiveComparisons, false),
                            // Case sensitive, accent sensitive
                            MakeStartsWithExpressionImpl(
                                target,
                                Utf8Bin(prefix),
                                originalPrefix: prefix
                            )
                        )
                    },
                    // Case insensitive, accent sensitive
                    MakeStartsWithExpressionImpl(
                        LCase(target),
                        Utf8Bin(LCase(prefix))
                    )
                );
            }
        }

        private SqlBinaryExpression MakeStartsWithExpressionImpl(
            SqlExpression target,
            SqlExpression prefix,
            SqlExpression originalPrefix = null)
        {
            // BUG: EF Core #17389 will lead to a System.NullReferenceException, if SqlExpressionFactory.Like()
            //      is being called with match and pattern as two expressions, that have not been applied a
            //      TypeMapping yet and no escapeChar (null).
            //      As a workaround, apply/infer the type mapping for the match expression manually for now.
            return _sqlExpressionFactory.AndAlso(
                _sqlExpressionFactory.Like(
                    target,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(_sqlExpressionFactory.Function(
                        "CONCAT",
                        // when performing the like it is preferable to use the untransformed prefix
                        // value to ensure the index can be used
                        new[] { originalPrefix ?? prefix, _sqlExpressionFactory.Constant("%") },
                        typeof(string)))),
                _sqlExpressionFactory.Equal(
                    _sqlExpressionFactory.Function(
                        "LEFT",
                        new[]
                        {
                            target,
                            CharLength(prefix)
                        },
                        typeof(string)),
                    prefix
                ));
        }

        public SqlExpression MakeEndsWithExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression suffix,
            [NotNull] SqlExpression stringComparison)
        {
            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () =>
                        MakeEndsWithExpressionImpl(
                            target,
                            Utf8Bin(suffix),
                            suffix
                        ),
                    () =>
                        MakeEndsWithExpressionImpl(
                            LCase(target),
                            Utf8Bin(LCase(suffix)),
                            suffix
                        )
                );
            }
            else
            {
                return new CaseExpression(
                    new[]
                    {
                        new CaseWhenClause(
                            _sqlExpressionFactory.In(stringComparison, _caseSensitiveComparisons, false),
                            // Case sensitive, accent sensitive
                            MakeEndsWithExpressionImpl(
                                target,
                                Utf8Bin(suffix),
                                suffix
                            )
                        )
                    },
                    // Case insensitive, accent sensitive
                    MakeEndsWithExpressionImpl(
                        LCase(target),
                        Utf8Bin(LCase(suffix)),
                        suffix
                    )
                );
            }
        }

        private SqlExpression MakeEndsWithExpressionImpl(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression suffix,
            [NotNull] SqlExpression originalSuffix)
        {
            var endsWithExpression =
                _sqlExpressionFactory.Equal(
                    _sqlExpressionFactory.Function(
                        "RIGHT",
                        new[]
                        {
                            target,
                            CharLength(suffix)
                        },
                        target.Type,
                        null),
                    suffix);

            if (originalSuffix is SqlConstantExpression constantSuffix)
            {
                return (string)constantSuffix.Value == string.Empty
                    ? _sqlExpressionFactory.Constant(true)
                    : (SqlExpression)endsWithExpression;
            }
            else
            {
                return _sqlExpressionFactory.OrElse(
                    endsWithExpression,
                    _sqlExpressionFactory.Equal(originalSuffix, _sqlExpressionFactory.Constant(string.Empty)));
            }
        }

        public SqlExpression MakeContainsExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression search,
            [NotNull] SqlExpression stringComparison)
        {
            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () =>
                        MakeContainsExpressionImpl(
                            target,
                            Utf8Bin(search),
                            search
                        ),
                    () =>
                        MakeContainsExpressionImpl(
                            LCase(target),
                            Utf8Bin(LCase(search)),
                            search
                        )
                );
            }
            else
            {
                return new CaseExpression(
                    new[]
                    {
                        new CaseWhenClause(
                            _sqlExpressionFactory.In(stringComparison, _caseSensitiveComparisons, false),
                            // Case sensitive, accent sensitive
                            MakeContainsExpressionImpl(
                                target,
                                Utf8Bin(search),
                                search
                            )
                        )
                    },
                    // Case insensitive, accent sensitive
                    MakeContainsExpressionImpl(
                        LCase(target),
                        Utf8Bin(LCase(search)),
                        search
                    )
                );
            }
        }

        private SqlExpression MakeContainsExpressionImpl(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression search,
            [NotNull] SqlExpression originalSearch)
        {

            var containsExpression = _sqlExpressionFactory.GreaterThan(
                _sqlExpressionFactory.Function("LOCATE", new[] { search, target }, typeof(int), null),
                _sqlExpressionFactory.Constant(0)
            );

            if (originalSearch is SqlConstantExpression constantSuffix)
            {
                return (string)constantSuffix.Value == string.Empty
                    ? _sqlExpressionFactory.Constant(true)
                    : (SqlExpression)containsExpression;
            }
            else
            {
                return _sqlExpressionFactory.OrElse(
                    containsExpression,
                    _sqlExpressionFactory.Equal(originalSearch, _sqlExpressionFactory.Constant(string.Empty)));
            }
        }

        public SqlExpression MakeIndexOfExpression(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression search,
            [NotNull] SqlExpression stringComparison)
        {
            if (TryGetExpressionValue<StringComparison>(stringComparison, out var cmp))
            {
                return CreateExpressionForCaseSensitivity(
                    cmp,
                    () =>
                        MakeIndexOfExpressionImpl(
                            target,
                            Utf8Bin(search)
                        ),
                    () =>
                        MakeIndexOfExpressionImpl(
                            LCase(target),
                            Utf8Bin(LCase(search))
                        )
                );
            }
            else
            {
                return _sqlExpressionFactory.Case(
                    new[]
                    {
                        new CaseWhenClause(
                           _sqlExpressionFactory.In(stringComparison, _caseSensitiveComparisons, false),
                            // Case sensitive, accent sensitive
                            MakeIndexOfExpressionImpl(
                                target,
                                Utf8Bin(search)
                            )
                        )
                    },
                    // Case insensitive, accent sensitive
                    MakeIndexOfExpressionImpl(
                        LCase(target),
                        Utf8Bin(LCase(search))
                    )
                );
            }
        }

        private SqlExpression MakeIndexOfExpressionImpl(
            [NotNull] SqlExpression target,
            [NotNull] SqlExpression search)
        {
            return _sqlExpressionFactory.Subtract(
                _sqlExpressionFactory.Function(
                    "LOCATE",
                    new[] { search, target },
                    typeof(int)),
                _sqlExpressionFactory.Constant(1)
            );
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
            else
            {
                value = default;
                return false;
            }
        }

        private static SqlExpression CreateExpressionForCaseSensitivity(
            StringComparison cmp,
            Func<SqlExpression> ifCaseSensitive,
            Func<SqlExpression> ifCaseInsensitive)
        {
            switch (cmp)
            {
                case StringComparison.Ordinal:
                case StringComparison.CurrentCulture:
                case StringComparison.InvariantCulture:
                    return ifCaseSensitive();
                case StringComparison.OrdinalIgnoreCase:
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                    return ifCaseInsensitive();
                default:
                    return default;
            }
        }

        private SqlExpression LCase(SqlExpression value)
        {
            return _sqlExpressionFactory.Function("LCASE", new[] { value }, value.Type, null);
        }

        private SqlExpression Utf8Bin(SqlExpression value)
        {
            return _sqlExpressionFactory.Collate(
                value,
                "utf8mb4",
                "utf8mb4_bin"
            );
        }

        private SqlExpression CharLength(SqlExpression value)
        {
            return _sqlExpressionFactory.Function("CHAR_LENGTH", new[] { value }, typeof(int), null);
        }
    }
}
