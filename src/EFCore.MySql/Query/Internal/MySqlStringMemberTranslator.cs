// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlStringMemberTranslator : IMemberTranslator
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlStringMemberTranslator(MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (member.Name == nameof(string.Length)
                && instance?.Type == typeof(string))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "CHAR_LENGTH",
                    new[] { instance },
                    returnType);
            }

            return null;
        }
    }
}
