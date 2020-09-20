// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlGeometryMethodTranslator : IMethodCallTranslator
    {
        private static readonly IDictionary<MethodInfo, string> _methodToFunctionName = new Dictionary<MethodInfo, string>
        {
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.AsBinary), Type.EmptyTypes), "ST_AsBinary" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.AsText), Type.EmptyTypes), "ST_AsText" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Buffer), new[] { typeof(double) }), "ST_Buffer" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Contains), new[] { typeof(Geometry) }), "ST_Contains" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.ConvexHull), Type.EmptyTypes), "ST_ConvexHull" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Difference), new[] { typeof(Geometry) }), "ST_Difference" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Disjoint), new[] { typeof(Geometry) }), "ST_Disjoint" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.EqualsTopologically), new[] { typeof(Geometry) }), "ST_Equals" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Intersection), new[] { typeof(Geometry) }), "ST_Intersection" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Intersects), new[] { typeof(Geometry) }), "ST_Intersects" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Overlaps), new[] { typeof(Geometry) }), "ST_Overlaps" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.SymmetricDifference), new[] { typeof(Geometry) }), "ST_SymDifference" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.ToBinary), Type.EmptyTypes), "ST_AsBinary" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.ToText), Type.EmptyTypes), "ST_AsText" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Union), new[] { typeof(Geometry) }), "ST_Union" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Within), new[] { typeof(Geometry) }), "ST_Within" }
        };

        private static readonly IDictionary<MethodInfo, string> _geometryMethodToFunctionName = new Dictionary<MethodInfo, string>
        {
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Crosses), new[] { typeof(Geometry) }), "ST_Crosses" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Relate), new[] { typeof(Geometry), typeof(string) }), "ST_Relate" },
            { typeof(Geometry).GetRuntimeMethod(nameof(Geometry.Touches), new[] { typeof(Geometry) }), "ST_Touches" }
        };

        private static readonly MethodInfo _getGeometryN = typeof(Geometry).GetRuntimeMethod(
            nameof(Geometry.GetGeometryN), new[] { typeof(int) });

        private static readonly MethodInfo _isWithinDistance = typeof(Geometry).GetRuntimeMethod(
            nameof(Geometry.IsWithinDistance), new[] { typeof(Geometry), typeof(double) });

        private static readonly MethodInfo _distance = typeof(Geometry).GetRuntimeMethod(
            nameof(Geometry.Distance),
            new[] { typeof(Geometry) });

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;

        public MySqlGeometryMethodTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (typeof(Geometry).IsAssignableFrom(method.DeclaringType))
            {
                var geometryExpressions = new[] { instance }.Concat(
                    arguments.Where(e => typeof(Geometry).IsAssignableFrom(e.Type)));
                var typeMapping = ExpressionExtensions.InferTypeMapping(geometryExpressions.ToArray());

                Debug.Assert(typeMapping != null, "At least one argument must have typeMapping.");
                var storeType = typeMapping.StoreType;

                if (_methodToFunctionName.TryGetValue(method, out var functionName) ||
                    _geometryMethodToFunctionName.TryGetValue(method, out functionName))
                {
                    instance = _sqlExpressionFactory.ApplyTypeMapping(
                        instance,
                        _typeMappingSource.FindMapping(instance.Type, storeType));

                    var typeMappedArguments = new List<SqlExpression>
                    {
                        instance
                    };

                    foreach (var argument in arguments)
                    {
                        typeMappedArguments.Add(
                            _sqlExpressionFactory.ApplyTypeMapping(
                                argument,
                                typeof(Geometry).IsAssignableFrom(argument.Type)
                                    ? _typeMappingSource.FindMapping(argument.Type, storeType)
                                    : _typeMappingSource.FindMapping(argument.Type)));
                    }

                    var resultTypeMapping = typeof(Geometry).IsAssignableFrom(method.ReturnType)
                        ? _typeMappingSource.FindMapping(method.ReturnType, storeType)
                        : _typeMappingSource.FindMapping(method.ReturnType);

                    return _sqlExpressionFactory.Function(
                        functionName,
                        typeMappedArguments,
                        method.ReturnType,
                        resultTypeMapping);
                }

                if (Equals(method, _getGeometryN))
                {
                    return _sqlExpressionFactory.Function(
                        "ST_GeometryN",
                        new[]
                        {
                            instance,
                            _sqlExpressionFactory.Add(
                                arguments[0],
                                _sqlExpressionFactory.Constant(1))
                        },
                        method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType, storeType));
                }

                if (Equals(method, _distance))
                {
                    return GetDistanceCallBySrid(
                        instance,
                        arguments[0],
                        method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType, storeType));
                }

                if (Equals(method, _isWithinDistance))
                {
                    return _sqlExpressionFactory.LessThanOrEqual(
                        GetDistanceCallBySrid(
                            instance,
                            arguments[0],
                            _distance.ReturnType,
                            _typeMappingSource.FindMapping(_distance.ReturnType)),
                        arguments[1]);
                }
            }

            return null;
        }

        // If the distance should be calculated, use `ST_Distance_Sphere()` for SRID 4326 and
        // `ST_Distance()` for all other cases.
        private SqlExpression GetDistanceCallBySrid(
            SqlExpression left,
            SqlExpression right,
            Type resultType,
            RelationalTypeMapping resultTypeMapping)
        {
            return _sqlExpressionFactory.Case(
                new[]
                {
                    new CaseWhenClause(
                        _sqlExpressionFactory.Equal(
                            _sqlExpressionFactory.Function(
                                "ST_SRID",
                                new[] {left},
                                typeof(int),
                                _typeMappingSource.FindMapping(typeof(int))),
                            _sqlExpressionFactory.Constant(4326)),
                        MySqlSpatialDbFunctionsExtensionsMethodTranslator.GetStDistanceSphereFunctionCall(
                            left,
                            right,
                            SpatialDistanceAlgorithm.Native,
                            resultType,
                            resultTypeMapping,
                            _sqlExpressionFactory,
                            _options)),
                },
                MySqlSpatialDbFunctionsExtensionsMethodTranslator.GetStDistanceFunctionCall(
                    left,
                    right,
                    resultType,
                    resultTypeMapping,
                    _sqlExpressionFactory));
        }
    }
}
