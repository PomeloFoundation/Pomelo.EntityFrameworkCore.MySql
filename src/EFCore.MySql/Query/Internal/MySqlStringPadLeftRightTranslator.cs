using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlStringPadLeftRightTranslator : IMethodCallTranslator
    {
        private static readonly Dictionary<MethodInfo, string> withOneArgumentMethods = new Dictionary<MethodInfo, string>
        {
            { typeof(string).GetRuntimeMethod(nameof(string.PadLeft), new[] { typeof(int) }), "LPAD" },
            { typeof(string).GetRuntimeMethod(nameof(string.PadRight), new[] { typeof(int) }), "RPAD" }
        };

        private static readonly Dictionary<MethodInfo, string> withTwoArgumentsMethods = new Dictionary<MethodInfo, string>
        {
            { typeof(string).GetRuntimeMethod(nameof(string.PadLeft), new[] { typeof(int), typeof(char) }), "LPAD" },
            { typeof(string).GetRuntimeMethod(nameof(string.PadRight), new[] { typeof(int), typeof(char) }), "RPAD" }
        };
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlStringPadLeftRightTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            SqlExpression padstr = null;
            SqlExpression len = null;

            if (withOneArgumentMethods.TryGetValue(method, out var sqlFunction)
                && arguments[0].NodeType == ExpressionType.Constant)
            {
                padstr = _sqlExpressionFactory.Fragment(" ");
                len = arguments[0];
            }
            else if (withTwoArgumentsMethods.TryGetValue(method, out sqlFunction)
                && arguments[0].NodeType == ExpressionType.Constant
                && arguments[1].NodeType == ExpressionType.Constant)
            {
                len = arguments[0];
                padstr = arguments[1];
            }
            else
            {
                return null;
            }

            return _sqlExpressionFactory.Function(
                    sqlFunction,
                    new[]
                    {
                        arguments[0],
                        len,
                        padstr
                    },
                    method.ReturnType);
        }
    }
}
