// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using static Pomelo.EntityFrameworkCore.MySql.Utilities.Statics;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlLineStringMemberTranslator : IMemberTranslator
    {
        private static readonly IDictionary<MemberInfo, string> _memberToFunctionName = new Dictionary<MemberInfo, string>
        {
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.Count)), "ST_NumPoints" },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.EndPoint)), "ST_EndPoint" },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.IsClosed)), "ST_IsClosed" },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.StartPoint)), "ST_StartPoint" },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.IsRing)), "ST_IsRing" }
        };

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;

        public MySqlLineStringMemberTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
        }

        public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_memberToFunctionName.TryGetValue(member, out var functionName))
            {
                Debug.Assert(instance.TypeMapping != null, "Instance must have typeMapping assigned.");
                var storeType = instance.TypeMapping.StoreType;

                var resultTypeMapping = typeof(Geometry).IsAssignableFrom(returnType)
                    ? _typeMappingSource.FindMapping(returnType, storeType)
                    : _typeMappingSource.FindMapping(returnType);

                // Emulate ST_IsRing if not supported.
                var sqlExpression = functionName != "ST_IsRing" ||
                                  _options.ServerVersion.SupportsSpatialIsRingFunction
                    ? (SqlExpression)_sqlExpressionFactory.Function(
                        functionName,
                        new[] {instance},
                        nullable: true,
                        argumentsPropagateNullability: TrueArrays[1],
                        returnType,
                        resultTypeMapping)
                    : _sqlExpressionFactory.AndAlso(
                        _sqlExpressionFactory.Function(
                            "ST_IsClosed",
                            new[] {instance},
                            nullable: true,
                            argumentsPropagateNullability: TrueArrays[1],
                            returnType,
                            resultTypeMapping),
                        _sqlExpressionFactory.Function(
                            "ST_IsSimple",
                            new[] {instance},
                            nullable: true,
                            argumentsPropagateNullability: TrueArrays[1],
                            returnType,
                            resultTypeMapping)
                    );

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
