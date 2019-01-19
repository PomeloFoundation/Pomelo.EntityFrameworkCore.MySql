// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public class MySqlDateDiffTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDateDiffMapping
            = new Dictionary<MethodInfo, string>
            {
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "YEAR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "YEAR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "YEAR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffYear),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "YEAR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MONTH"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MONTH"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MONTH"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMonth),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MONTH"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "DAY"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "DAY"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "DAY"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffDay),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "DAY"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "HOUR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "HOUR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "HOUR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffHour),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "HOUR"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MINUTE"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MINUTE"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MINUTE"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMinute),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MINUTE"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "SECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "SECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "SECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffSecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "SECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MILLISECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MILLISECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MILLISECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MILLISECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "MICROSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "MICROSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "MICROSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "MICROSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }),
                    "NANOSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }),
                    "NANOSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }),
                    "NANOSECOND"
                },
                {
                    typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                        nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond),
                        new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }),
                    "NANOSECOND"
                }
            };

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            return _methodInfoDateDiffMapping.TryGetValue(methodCallExpression.Method, out var datePart)
                ? new SqlFunctionExpression(
                    functionName: "DATEDIFF",
                    returnType: methodCallExpression.Type,
                    arguments: new[]
                    {
                        new SqlFragmentExpression(datePart),
                        methodCallExpression.Arguments[1],
                        methodCallExpression.Arguments[2]
                    })
                : null;
        }
    }
}
