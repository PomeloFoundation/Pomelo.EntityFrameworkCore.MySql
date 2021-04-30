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

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlPolygonMemberTranslator : IMemberTranslator
    {
        private static readonly IDictionary<MemberInfo, (string Name, bool OnlyNullByArgs)> _memberToFunctionName = new Dictionary<MemberInfo, (string Name, bool OnlyNullByArgs)>
        {
            { typeof(Polygon).GetRuntimeProperty(nameof(Polygon.ExteriorRing)), ("ST_ExteriorRing", false) },
            { typeof(Polygon).GetRuntimeProperty(nameof(Polygon.NumInteriorRings)), ("ST_NumInteriorRings", false) }, // MariaDB bug: Only knows `ST_NumInteriorRings` instead of `ST_NumInteriorRing` (without `s`).
        };

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlPolygonMemberTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (typeof(Polygon).IsAssignableFrom(member.DeclaringType))
            {
                Debug.Assert(instance.TypeMapping != null, "Instance must have typeMapping assigned.");
                var storeType = instance.TypeMapping.StoreType;

                if (_memberToFunctionName.TryGetValue(member, out var mapping))
                {
                    var resultTypeMapping = typeof(Geometry).IsAssignableFrom(returnType)
                        ? _typeMappingSource.FindMapping(returnType, storeType)
                        : _typeMappingSource.FindMapping(returnType);

                    return _sqlExpressionFactory.NullableFunction(
                        mapping.Name,
                        new [] {instance},
                        returnType,
                        resultTypeMapping,
                        mapping.OnlyNullByArgs);
                }
            }

            return null;
        }
    }
}
