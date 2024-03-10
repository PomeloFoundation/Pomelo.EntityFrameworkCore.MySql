// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDateDiffFunctionsTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDateDiffMapping
            = new Dictionary<MethodInfo, string>
            {
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "YEAR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "YEAR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "YEAR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "YEAR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "YEAR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "YEAR" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "QUARTER" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "QUARTER" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "QUARTER" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "QUARTER" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "QUARTER" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "QUARTER" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MONTH" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MONTH" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MONTH" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MONTH" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MONTH" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MONTH" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "WEEK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "WEEK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "WEEK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "WEEK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "WEEK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "WEEK" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "DAY" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "DAY" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "DAY" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "DAY" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "DAY" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "DAY" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "HOUR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "HOUR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "HOUR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "HOUR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "HOUR" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "HOUR" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MINUTE" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MINUTE" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MINUTE" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MINUTE" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MINUTE" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MINUTE" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "SECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "SECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "SECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "SECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "SECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "SECOND" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MILLISECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MILLISECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MILLISECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MILLISECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MILLISECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MILLISECOND" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MICROSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MICROSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MICROSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MICROSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MICROSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MICROSECOND" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "TICK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "TICK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "TICK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "TICK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "TICK" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "TICK" },

                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "NANOSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "NANOSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "NANOSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "NANOSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "NANOSECOND" },
                { typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "NANOSECOND" },
            };

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlDateDiffFunctionsTranslator(MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_methodInfoDateDiffMapping.TryGetValue(method, out var datePart))
            {
                var startDate = arguments[1];
                var endDate = arguments[2];
                var typeMapping = ExpressionExtensions.InferTypeMapping(startDate, endDate);

                startDate = _sqlExpressionFactory.ApplyTypeMapping(startDate, typeMapping);
                endDate = _sqlExpressionFactory.ApplyTypeMapping(endDate, typeMapping);

                var actualDatePart = datePart is "MILLISECOND"
                                              or "TICK"
                                              or "NANOSECOND"
                    ? "MICROSECOND"
                    : datePart;

                var timeStampDiffExpression = _sqlExpressionFactory.NullableFunction(
                    "TIMESTAMPDIFF",
                    new[]
                    {
                        _sqlExpressionFactory.Fragment(actualDatePart),
                        startDate,
                        endDate
                    },
                    typeof(int),
                    typeMapping: null,
                    onlyNullWhenAnyNullPropagatingArgumentIsNull: true,
                    argumentsPropagateNullability: new[] { false, true, true });

                return datePart switch
                {
                    "MILLISECOND" => _sqlExpressionFactory.MySqlIntegerDivide(timeStampDiffExpression, _sqlExpressionFactory.Constant(1_000)),
                    "TICK" => _sqlExpressionFactory.Multiply(timeStampDiffExpression, _sqlExpressionFactory.Constant(10)),
                    "NANOSECOND" => _sqlExpressionFactory.Multiply(timeStampDiffExpression, _sqlExpressionFactory.Constant(1_000)),
                    _ => timeStampDiffExpression
                };
            }

            return null;
        }
    }
}
