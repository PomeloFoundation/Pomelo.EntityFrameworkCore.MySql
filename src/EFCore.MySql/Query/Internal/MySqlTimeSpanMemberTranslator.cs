// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlTimeSpanMemberTranslator : IMemberTranslator
    {
        private static readonly Dictionary<string, (string Part, int Divisor)> _datePartMapping
            = new Dictionary<string, (string, int)>
            {
                { nameof(TimeSpan.Hours), ("hour", 1) },
                { nameof(TimeSpan.Minutes), ("minute", 1) },
                { nameof(TimeSpan.Seconds), ("second", 1) },
                { nameof(TimeSpan.Milliseconds), ("microsecond", 1000) },
            };

        private static readonly Dictionary<string, double> _totalTimePartMapping
            = new Dictionary<string, double>
                        {
                { nameof(TimeSpan.TotalDays), 24 * 60 * 60 },
                { nameof(TimeSpan.TotalHours), 60 * 60 },
                { nameof(TimeSpan.TotalMinutes), 60 },
                { nameof(TimeSpan.TotalSeconds), 1 },
                { nameof(TimeSpan.TotalMilliseconds), 0.001 },
                { nameof(TimeSpan.TotalNanoseconds), 0.001 * 0.001 * 0.001 },
            };

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public MySqlTimeSpanMemberTranslator(
            MySqlSqlExpressionFactory sqlExpressionFactory,
            IRelationalTypeMappingSource typeMappingSource)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _typeMappingSource = typeMappingSource;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            var declaringType = member.DeclaringType;
            var memberName = member.Name;

            if (declaringType == typeof(TimeSpan))
            {
                if (_datePartMapping.TryGetValue(memberName, out var datePart))
                {
                    var extract = _sqlExpressionFactory.NullableFunction(
                        "EXTRACT",
                        new[]
                        {
                        _sqlExpressionFactory.ComplexFunctionArgument(
                            new [] {
                                _sqlExpressionFactory.Fragment($"{datePart.Part} FROM"),
                                instance
                            },
                            " ",
                            typeof(string))
                        },
                        returnType,
                        false);

                    if (datePart.Divisor != 1)
                    {
                        return _sqlExpressionFactory.MySqlIntegerDivide(
                            extract,
                            _sqlExpressionFactory.Constant(datePart.Divisor));
                    }

                    return extract;
                }
                else if (_totalTimePartMapping.TryGetValue(memberName, out var multiplicator) == true)
                {
                    var convertToSecondsExpression = _sqlExpressionFactory.NullableFunction(
                        name: "TIME_TO_SEC",
                        arguments: new[] { instance },
                        returnType: typeof(int),
                        typeMapping: _typeMappingSource.FindMapping(typeof(int))
                    );

                    var divideExpression = _sqlExpressionFactory.Divide(
                        left: convertToSecondsExpression,
                        right: _sqlExpressionFactory.Constant(multiplicator),
                        typeMapping: _typeMappingSource.FindMapping(typeof(double))
                    );

                    return divideExpression;
                }
            }

            return null;
        }
    }
}
