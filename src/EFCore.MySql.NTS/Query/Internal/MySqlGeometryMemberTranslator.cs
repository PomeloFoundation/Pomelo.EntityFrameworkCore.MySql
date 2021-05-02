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
    public class MySqlGeometryMemberTranslator : IMemberTranslator
    {
        private static readonly IDictionary<MemberInfo, (string Name, bool OnlyNullByArgs)> _memberToFunctionName = new Dictionary<MemberInfo, (string Name, bool OnlyNullByArgs)>
        {
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.Area)), ("ST_Area", false) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.Dimension)), ("ST_Dimension", true) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.GeometryType)), ("ST_GeometryType", true) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.IsEmpty)), ("ST_IsEmpty", true) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.IsValid)), ("ST_IsValid", true) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.Length)), ("ST_Length", false) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.NumGeometries)), ("ST_NumGeometries", false) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.NumPoints)), ("ST_NumPoints", false) }
        };

        private static readonly IDictionary<MemberInfo, (string Name, bool OnlyNullByArgs)> _geometryMemberToFunctionName = new Dictionary<MemberInfo, (string Name, bool OnlyNullByArgs)>
        {
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.Boundary)), ("ST_Boundary", false) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.Centroid)), ("ST_Centroid", false) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.Envelope)), ("ST_Envelope", true) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.InteriorPoint)), ("ST_PointOnSurface", false) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.IsSimple)), ("ST_IsSimple", true) },
            { typeof(Geometry).GetRuntimeProperty(nameof(Geometry.PointOnSurface)), ("ST_PointOnSurface", false) }
        };

        private static readonly MemberInfo _ogcGeometryType = typeof(Geometry).GetRuntimeProperty(nameof(Geometry.OgcGeometryType));
        private static readonly MemberInfo _srid = typeof(Geometry).GetRuntimeProperty(nameof(Geometry.SRID));

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlGeometryMemberTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (typeof(Geometry).IsAssignableFrom(member.DeclaringType))
            {
                Debug.Assert(instance.TypeMapping != null, $"Instance must have {nameof(SqlExpression.TypeMapping)} assigned.");
                var storeType = instance.TypeMapping.StoreType;

                if (_memberToFunctionName.TryGetValue(member, out var mapping) ||
                    _geometryMemberToFunctionName.TryGetValue(member, out mapping))
                {
                    var resultTypeMapping = typeof(Geometry).IsAssignableFrom(returnType)
                        ? _typeMappingSource.FindMapping(returnType, storeType)
                        : _typeMappingSource.FindMapping(returnType);

                    SqlExpression sqlExpression = _sqlExpressionFactory.NullableFunction(
                        mapping.Name,
                        new [] {instance},
                        returnType,
                        resultTypeMapping,
                        mapping.OnlyNullByArgs);

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

                if (Equals(member, _ogcGeometryType))
                {
                    var whenClauses = new List<CaseWhenClause>
                    {
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("Point"), _sqlExpressionFactory.Constant(OgcGeometryType.Point)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("LineString"), _sqlExpressionFactory.Constant(OgcGeometryType.LineString)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("Polygon"), _sqlExpressionFactory.Constant(OgcGeometryType.Polygon)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("MultiPoint"), _sqlExpressionFactory.Constant(OgcGeometryType.MultiPoint)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("MultiLineString"),
                            _sqlExpressionFactory.Constant(OgcGeometryType.MultiLineString)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("MultiPolygon"),
                            _sqlExpressionFactory.Constant(OgcGeometryType.MultiPolygon)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("GeometryCollection"),
                            _sqlExpressionFactory.Constant(OgcGeometryType.GeometryCollection)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("CircularString"),
                            _sqlExpressionFactory.Constant(OgcGeometryType.CircularString)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("CompoundCurve"),
                            _sqlExpressionFactory.Constant(OgcGeometryType.CompoundCurve)),
                        new CaseWhenClause(
                            _sqlExpressionFactory.Constant("CurvePolygon"),
                            _sqlExpressionFactory.Constant(OgcGeometryType.CurvePolygon))
                    };

                    return _sqlExpressionFactory.Case(
                        _sqlExpressionFactory.NullableFunction(
                            "ST_GeometryType",
                            new [] {instance},
                            typeof(string)),
                        whenClauses.ToArray(),
                        null);
                }

                if (Equals(member, _srid))
                {
                    return _sqlExpressionFactory.NullableFunction(
                        "ST_SRID",
                        new [] {instance},
                        returnType);
                }
            }

            return null;
        }
    }
}
