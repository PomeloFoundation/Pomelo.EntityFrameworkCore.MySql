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

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlLineStringMemberTranslator : IMemberTranslator
    {
        private static readonly IDictionary<MemberInfo, (string Name, bool OnlyNullByArgs)> _memberToFunctionName = new Dictionary<MemberInfo, (string Name, bool OnlyNullByArgs)>
        {
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.Count)), ("ST_NumPoints", false) },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.EndPoint)), ("ST_EndPoint", false) },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.IsClosed)), ("ST_IsClosed", false) },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.StartPoint)), ("ST_StartPoint", false) },
            { typeof(LineString).GetRuntimeProperty(nameof(LineString.IsRing)), ("ST_IsRing", false) }
        };

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;

        public MySqlLineStringMemberTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            MySqlSqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_memberToFunctionName.TryGetValue(member, out var mapping))
            {
                Debug.Assert(instance.TypeMapping != null, "Instance must have typeMapping assigned.");
                var storeType = instance.TypeMapping.StoreType;

                var resultTypeMapping = typeof(Geometry).IsAssignableFrom(returnType)
                    ? _typeMappingSource.FindMapping(returnType, storeType)
                    : _typeMappingSource.FindMapping(returnType);

                // Emulate ST_IsRing if not supported.
                var sqlExpression = mapping.Name != "ST_IsRing" ||
                                  _options.ServerVersion.Supports.SpatialFunctionAdditions
                    ? (SqlExpression)_sqlExpressionFactory.NullableFunction(
                        mapping.Name,
                        new[] {instance},
                        returnType,
                        resultTypeMapping,
                        mapping.OnlyNullByArgs)
                    : _sqlExpressionFactory.AndAlso(
                        _sqlExpressionFactory.NullableFunction(
                            "ST_IsClosed",
                            new[] {instance},
                            returnType,
                            resultTypeMapping,
                            false),
                        _sqlExpressionFactory.NullableFunction(
                            "ST_IsSimple",
                            new[] {instance},
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
