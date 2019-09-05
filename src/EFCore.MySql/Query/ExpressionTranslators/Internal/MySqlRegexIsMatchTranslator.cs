// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections.Generic;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{

    public class MySqlRegexIsMatchTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(Regex).GetRuntimeMethod(nameof(Regex.IsMatch), new[] { typeof(string), typeof(string) });

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
            => _methodInfo.Equals(method)
                ? new RegexpExpression(
                    arguments[0],
                    arguments[1])
                : null;
    }
}
