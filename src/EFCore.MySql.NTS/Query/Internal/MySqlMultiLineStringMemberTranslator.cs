// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlMultiLineStringMemberTranslator : IMemberTranslator
    {
        private static readonly MemberInfo _isClosed = typeof(MultiLineString).GetRuntimeProperty(nameof(MultiLineString.IsClosed));
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlMultiLineStringMemberTranslator(MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (Equals(member, _isClosed))
            {
                SqlExpression sqlExpression = _sqlExpressionFactory.NullableFunction(
                    "ST_IsClosed",
                    new [] {instance},
                    returnType,
                    false);

                // ST_IsRing and others returns TRUE for a NULL value in MariaDB, which is inconsistent with NTS' implementation.
                // We return the following instead:
                // CASE
                //     WHEN instance IS NULL THEN NULL
                //     ELSE expression
                // END
                if (returnType == typeof(bool))
                {
                    sqlExpression = _sqlExpressionFactory.Case(
                        new[]
                        {
                            new CaseWhenClause(
                                _sqlExpressionFactory.IsNull(instance),
                                _sqlExpressionFactory.Constant(null, RelationalTypeMapping.NullMapping))
                        },
                        sqlExpression);
                }

                return sqlExpression;
            }

            return null;
        }
    }
}
