using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlStringComparisonTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _equalsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _staticEqualsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Equals), new[] { typeof(string), typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _startsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _endsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _containsWithoutParametersMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });
        private static readonly MethodInfo _containsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo _indexOfMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) });

        private static readonly IReadOnlyList<Expression> _caseSensitiveComparisons =
            new ReadOnlyCollection<Expression>(
                new Expression[] {
                    Expression.Constant(StringComparison.Ordinal),
                    Expression.Constant(StringComparison.CurrentCulture),
                    Expression.Constant(StringComparison.InvariantCulture)
                }
            );

        public Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (Equals(methodCallExpression.Method, _equalsMethodInfo) &&
                methodCallExpression.Object != null)
            {
                return MakeStringEqualsExpression(
                    methodCallExpression.Object,
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1]
                );
            }
            else if (Equals(methodCallExpression.Method, _staticEqualsMethodInfo))
            {
                return MakeStringEqualsExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1],
                    methodCallExpression.Arguments[2]
                );
            }
            else if (Equals(methodCallExpression.Method, _startsWithMethodInfo) &&
                methodCallExpression.Object != null)
            {
                return MakeStartsWithExpression(
                    methodCallExpression.Object,
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1]
                );
            }
            else if (Equals(methodCallExpression.Method, _endsWithMethodInfo) &&
                methodCallExpression.Object != null)
            {
                return MakeEndsWithExpression(
                    methodCallExpression.Object,
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1]
                );
            }
            else if (Equals(methodCallExpression.Method, _containsWithoutParametersMethodInfo) &&
                     methodCallExpression.Object != null)
            {
                return MakeContainsExpression(
                    methodCallExpression.Object,
                    methodCallExpression.Arguments[0],
                    Expression.Constant(StringComparison.Ordinal)
                );
            }
            else if (Equals(methodCallExpression.Method, _containsMethodInfo) &&
                     methodCallExpression.Object != null)
            {
                return MakeContainsExpression(
                    methodCallExpression.Object,
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1]
                );
            }
            else if (Equals(methodCallExpression.Method, _indexOfMethodInfo) &&
                methodCallExpression.Object != null)
            {
                return MakeIndexOfExpression(
                    methodCallExpression.Object,
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1]
                );
            }

            return null;
        }

        private static Expression MakeStringEqualsExpression(
            [NotNull] Expression leftValue,
            [NotNull] Expression rightValue,
            [NotNull] Expression stringComparison)
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
                            return Expression.Equal(
                                leftValue,
                                Utf8Bin(rightValue)
                            );
                        }
                        else
                        {
                            return Expression.Equal(
                                Utf8Bin(leftValue),
                                rightValue
                            );
                        }
                    },
                    () =>
                        Expression.Equal(
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
                            new InExpression(stringComparison, _caseSensitiveComparisons),
                            // Case sensitive, accent sensitive
                            Expression.Equal(
                                leftValue,
                                Utf8Bin(rightValue)
                            )
                        )
                    },
                    // Case insensitive, accent sensitive
                    Expression.Equal(
                        LCase(leftValue),
                        Utf8Bin(LCase(rightValue))
                    )
                );
            }
        }

        private static Expression MakeStartsWithExpression(
            [NotNull] Expression target,
            [NotNull] Expression prefix,
            [NotNull] Expression stringComparison)
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
                            new InExpression(stringComparison, _caseSensitiveComparisons),
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

        private static BinaryExpression MakeStartsWithExpressionImpl(
            Expression target,
            Expression prefix,
            Expression originalPrefix = null) {
            return Expression.AndAlso(
                new LikeExpression(
                    target,
                    new SqlFunctionExpression(
                        "CONCAT",
                        typeof(string),
                        // when performing the like it is preferable to use the untransformed prefix
                        // value to ensure the index can be used
                        new[] { originalPrefix ?? prefix, Expression.Constant("%") })
                ),
                new NullCompensatedExpression(Expression.Equal(
                    new SqlFunctionExpression(
                        "LEFT",
                        typeof(string),
                        new[]
                        {
                            target,
                            CharLength(prefix)
                        }),
                    prefix
                ))
            );
        }

        private static Expression MakeEndsWithExpression(
            [NotNull] Expression target,
            [NotNull] Expression suffix,
            [NotNull] Expression stringComparison)
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
                            new InExpression(stringComparison, _caseSensitiveComparisons),
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

        private static Expression MakeEndsWithExpressionImpl(
            [NotNull] Expression target,
            [NotNull] Expression suffix,
            [NotNull] Expression originalSuffix)
        {
            var endsWithExpression =
                new NullCompensatedExpression(Expression.Equal(
                    new SqlFunctionExpression(
                        "RIGHT",
                        target.Type,
                        new[]
                        {
                            target,
                            CharLength(suffix)
                        }),
                    suffix));

            if (originalSuffix is ConstantExpression constantSuffix)
            {
                return (string)constantSuffix.Value == string.Empty
                    ? (Expression)Expression.Constant(true)
                    : endsWithExpression;
            }
            else
            {
                return Expression.OrElse(
                    endsWithExpression,
                    Expression.Equal(originalSuffix, Expression.Constant(string.Empty)));
            }
        }

        internal static Expression MakeContainsExpression(
            [NotNull] Expression target,
            [NotNull] Expression search,
            [NotNull] Expression stringComparison)
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
                            new InExpression(stringComparison, _caseSensitiveComparisons),
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

        private static Expression MakeContainsExpressionImpl(
            [NotNull] Expression target,
            [NotNull] Expression search,
            [NotNull] Expression originalSearch)
        {

            var containsExpression = Expression.GreaterThan(
                new SqlFunctionExpression("LOCATE", typeof(int), new[] { search, target }),
                Expression.Constant(0)
            );

            if (originalSearch is ConstantExpression constantSuffix)
            {
                return (string)constantSuffix.Value == string.Empty
                    ? (Expression)Expression.Constant(true)
                    : containsExpression;
            }
            else
            {
                return Expression.OrElse(
                    containsExpression,
                    Expression.Equal(originalSearch, Expression.Constant(string.Empty)));
            }
        }

        private static Expression MakeIndexOfExpression(
            [NotNull] Expression target,
            [NotNull] Expression search,
            [NotNull] Expression stringComparison)
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
                return new CaseExpression(
                    new[]
                    {
                        new CaseWhenClause(
                            new InExpression(stringComparison, _caseSensitiveComparisons),
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

        private static Expression MakeIndexOfExpressionImpl(
            [NotNull] Expression target,
            [NotNull] Expression search)
        {
            return Expression.Subtract(
                new SqlFunctionExpression(
                    "LOCATE",
                    typeof(int),
                    new[] { search, target }),
                Expression.Constant(1)
            );
        }

        private static bool TryGetExpressionValue<T>(Expression expression, out T value)
        {
            if (expression.Type != typeof(T))
            {
                throw new ArgumentException(
                    MySqlStrings.ExpressionTypeMismatch,
                    nameof(expression)
                );
            }

            if (expression is ConstantExpression constant)
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

        private static Expression CreateExpressionForCaseSensitivity(
            StringComparison cmp,
            Func<Expression> ifCaseSensitive,
            Func<Expression> ifCaseInsensitive)
        {
            switch (cmp) {
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

        private static Expression LCase(Expression value)
        {
            return new SqlFunctionExpression("LCASE", value.Type, new[] { value });
        }

        private static Expression Utf8Bin(Expression value)
        {
            return new MySqlCollateExpression(
                value,
                "utf8mb4",
                "utf8mb4_bin"
            );
        }

        private static Expression CharLength(Expression value)
        {
            return new SqlFunctionExpression("CHAR_LENGTH", typeof(int), new[] { value });
        }

    }
}
