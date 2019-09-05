// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlContainsOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlContainsOptimizedTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (Equals(method, _methodInfo))
            {
                var patternExpression = arguments[0];

                var charIndexExpression = _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.Function("LOCATE", new[] { patternExpression, instance }, typeof(int)),
                    _sqlExpressionFactory.Constant(0));

                return
                    patternExpression is SqlConstantExpression patternConstantExpression
                        ? (string)patternConstantExpression.Value == string.Empty
                            ? (SqlExpression)_sqlExpressionFactory.Constant(true)
                            : charIndexExpression
                        : _sqlExpressionFactory.OrElse(
                            charIndexExpression,
                            _sqlExpressionFactory.Equal(patternExpression, _sqlExpressionFactory.Constant(string.Empty)));
            }

            return null;
        }
    }
}
