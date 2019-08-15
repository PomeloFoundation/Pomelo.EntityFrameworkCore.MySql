using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
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

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            ConstantExpression padstr = null;
            ConstantExpression len = null;

            if(withOneArgumentMethods.TryGetValue(methodCallExpression.Method, out var sqlFunction)
                && methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant)
            {
                padstr = Expression.Constant(' ');
                len = methodCallExpression.Arguments[0] as ConstantExpression;
            }
            else if (withTwoArgumentsMethods.TryGetValue(methodCallExpression.Method, out sqlFunction)
                && methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant
                && methodCallExpression.Arguments[1].NodeType == ExpressionType.Constant)
            {
                len = methodCallExpression.Arguments[0] as ConstantExpression;
                padstr = methodCallExpression.Arguments[1] as ConstantExpression;
            }
            else
            {
                return null;
            }

            return new SqlFunctionExpression(
                    sqlFunction,
                    methodCallExpression.Type,
                    new[] { methodCallExpression.Object, len, padstr });
        }
    }
}
