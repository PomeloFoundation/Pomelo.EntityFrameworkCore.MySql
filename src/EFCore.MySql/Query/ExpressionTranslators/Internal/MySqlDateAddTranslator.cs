// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDateAddTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDatePartMapping = new Dictionary<MethodInfo, string>
        {
            {  typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddYears), new[] { typeof(int) }), "year" },
            {  typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMonths), new[] { typeof(int) }), "month" },
            {  typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddDays), new[] { typeof(double) }), "day" },
            {  typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddHours), new[] { typeof(double) }), "hour" },
            {  typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMinutes), new[] { typeof(double) }), "minute" },
            {  typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddSeconds), new[] { typeof(double) }), "second" },
            {  typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddYears), new[] { typeof(int) }), "year" },
            {  typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMonths), new[] { typeof(int) }), "month" },
            {  typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddDays), new[] { typeof(double) }), "day" },
            {  typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddHours), new[] { typeof(double) }), "hour" },
            {  typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMinutes), new[] { typeof(double) }), "minute" },
            {  typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddSeconds), new[] { typeof(double) }), "second" },
        };

        /// <summary>
        ///     Translates the given method call expression.
        /// </summary>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns>
        ///     A SQL expression representing the translated MethodCallExpression.
        /// </returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_methodInfoDatePartMapping.TryGetValue(methodCallExpression.Method, out var datePart))
            {
                var amountToAdd = methodCallExpression.Arguments.First();

                if (!datePart.Equals("year")
                    && !datePart.Equals("month")
                    && amountToAdd is ConstantExpression constantExpression
                    && ((double)constantExpression.Value >= int.MaxValue
                        || (double)constantExpression.Value <= int.MinValue))
                {
                    return null;
                }

                return new SqlFunctionExpression(
                    functionName: "DATE_ADD",
                    returnType: methodCallExpression.Type,
                    arguments: new[]
                    {
                        methodCallExpression.Object,
                        new SqlFragmentExpression("INTERVAL " + amountToAdd + " " + datePart),
                    });
            }

            return null;
        }
    }
}
