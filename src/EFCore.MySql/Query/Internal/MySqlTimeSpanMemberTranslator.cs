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
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlTimeSpanMemberTranslator(MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            var declaringType = member.DeclaringType;
            var memberName = member.Name;

            if (declaringType == typeof(TimeSpan) &&
                _datePartMapping.TryGetValue(memberName, out var datePart))
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

            return null;
        }
    }
}
