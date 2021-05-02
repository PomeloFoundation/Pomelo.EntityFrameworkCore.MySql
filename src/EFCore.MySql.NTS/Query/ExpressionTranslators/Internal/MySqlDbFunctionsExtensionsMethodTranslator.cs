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
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlSpatialDbFunctionsExtensionsMethodTranslator : IMethodCallTranslator
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly IMySqlOptions _options;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        private static readonly MethodInfo _spatialDistancePlanarMethodInfo = typeof(MySqlSpatialDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlSpatialDbFunctionsExtensions.SpatialDistancePlanar), new[] {typeof(DbFunctions), typeof(Geometry), typeof(Geometry)});
        private static readonly MethodInfo _spatialDistanceSphere = typeof(MySqlSpatialDbFunctionsExtensions).GetRuntimeMethod(nameof(MySqlSpatialDbFunctionsExtensions.SpatialDistanceSphere), new[] {typeof(DbFunctions), typeof(Geometry), typeof(Geometry), typeof(SpatialDistanceAlgorithm)});

        public MySqlSpatialDbFunctionsExtensionsMethodTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            MySqlSqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _typeMappingSource = typeMappingSource;
            _options = options;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (Equals(method, _spatialDistancePlanarMethodInfo))
            {
                // MySQL 8 uses the Andoyer algorithm by default for `ST_Distance()`, if an SRID of 4326 has been
                // associated with the geometry.
                // Since this call explicitly asked for a planar distance calculation, we need to ensure that
                // MySQL actually does that.
                // MariaDB ignores SRIDs and always calculates the planar distance.
                // CHECK: It could be faster to just manually apply the Pythagoras Theorem instead of changing the
                //        SRID in the case where ST_SRID() does not support a second parameter yet (see
                //        SetSrid()).
                if (_options.ServerVersion.Supports.SpatialSupportFunctionAdditions &&
                    _options.ServerVersion.Supports.SpatialDistanceFunctionImplementsAndoyer)
                {
                    return _sqlExpressionFactory.Case(
                        new[]
                        {
                            new CaseWhenClause(
                                _sqlExpressionFactory.Equal(
                                    _sqlExpressionFactory.NullableFunction(
                                        "ST_SRID",
                                        new[] {arguments[1]},
                                        typeof(int)),
                                    _sqlExpressionFactory.Constant(0)),
                                GetStDistanceFunctionCall(
                                    arguments[1],
                                    arguments[2],
                                    method.ReturnType,
                                    _typeMappingSource.FindMapping(method.ReturnType),
                                    _sqlExpressionFactory))
                        },
                        GetStDistanceFunctionCall(
                            SetSrid(arguments[1], 0, _sqlExpressionFactory, _options),
                            SetSrid(arguments[2], 0, _sqlExpressionFactory, _options),
                            method.ReturnType,
                            _typeMappingSource.FindMapping(method.ReturnType),
                            _sqlExpressionFactory));
                }

                return GetStDistanceFunctionCall(
                    arguments[1],
                    arguments[2],
                    method.ReturnType,
                    _typeMappingSource.FindMapping(method.ReturnType),
                    _sqlExpressionFactory);
            }

            if (Equals(method, _spatialDistanceSphere))
            {
                if (!(arguments[3] is SqlConstantExpression algorithm))
                {
                    throw new InvalidOperationException("The 'algorithm' parameter must be supplied as a constant.");
                }

                return GetStDistanceSphereFunctionCall(
                    arguments[1],
                    arguments[2],
                    (SpatialDistanceAlgorithm)algorithm.Value,
                    method.ReturnType,
                    _typeMappingSource.FindMapping(method.ReturnType),
                    _sqlExpressionFactory,
                    _options);
            }

            return null;
        }

        private static SqlFunctionExpression SetSrid(
            SqlExpression geometry,
            int srid,
            MySqlSqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
            => options.ServerVersion.Supports.SpatialSetSridFunction
                ? sqlExpressionFactory.NullableFunction(
                    "ST_SRID",
                    new[]
                    {
                        geometry,
                        sqlExpressionFactory.Constant(srid)
                    },
                    typeof(int))
                : sqlExpressionFactory.NullableFunction(
                    "ST_GeomFromWKB",
                    new SqlExpression[]
                    {
                        sqlExpressionFactory.NullableFunction(
                            "ST_AsBinary",
                            new[] {geometry},
                            typeof(byte[])),
                        sqlExpressionFactory.Constant(srid)
                    },
                    geometry.Type);

        public static SqlExpression GetStDistanceFunctionCall(
            SqlExpression left,
            SqlExpression right,
            Type resultType,
            RelationalTypeMapping resultTypeMapping,
            MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            // Returns null for empty geometry arguments.
            return sqlExpressionFactory.NullableFunction(
                "ST_Distance",
                new[] {left, right},
                resultType,
                resultTypeMapping,
                false);
        }

        public static SqlExpression GetStDistanceSphereFunctionCall(
            SqlExpression left,
            SqlExpression right,
            SpatialDistanceAlgorithm algorithm,
            Type resultType,
            RelationalTypeMapping resultTypeMapping,
            MySqlSqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            if (options.ServerVersion.Supports.SpatialDistanceSphereFunction)
            {
                if (algorithm == SpatialDistanceAlgorithm.Native)
                {
                    // Returns null for empty geometry arguments.
                    return sqlExpressionFactory.NullableFunction(
                        "ST_Distance_Sphere",
                        new[] {left, right},
                        resultType,
                        resultTypeMapping,
                        false);
                }

                if (algorithm == SpatialDistanceAlgorithm.Andoyer &&
                    options.ServerVersion.Supports.SpatialDistanceFunctionImplementsAndoyer)
                {
                    // The `ST_Distance()` in MySQL already uses the Andoyer algorithm, when SRID 4326 is associated
                    // with the geometry.
                    // CHECK: It might be faster to just run the custom implementation, if `ST_SRID()` does not support
                    //        a second parameter yet (see SetSrid()).
                    return sqlExpressionFactory.Case(
                        new[]
                        {
                            new CaseWhenClause(
                                sqlExpressionFactory.Equal(
                                    sqlExpressionFactory.NullableFunction(
                                        "ST_SRID",
                                        new[] {left},
                                        typeof(int)),
                                    sqlExpressionFactory.Constant(4326)),
                                GetStDistanceFunctionCall(
                                    left,
                                    right,
                                    resultType,
                                    resultTypeMapping,
                                    sqlExpressionFactory))
                        },
                        GetStDistanceFunctionCall(
                            SetSrid(left, 4326, sqlExpressionFactory, options),
                            SetSrid(right, 4326, sqlExpressionFactory, options),
                            resultType,
                            resultTypeMapping,
                            sqlExpressionFactory));
                }

                if (algorithm == SpatialDistanceAlgorithm.Haversine)
                {
                    // The Haversine algorithm assumes planar coordinates.
                    return sqlExpressionFactory.Case(
                        new[]
                        {
                            new CaseWhenClause(
                                sqlExpressionFactory.Equal(
                                    sqlExpressionFactory.NullableFunction(
                                        "ST_SRID",
                                        new[] {left},
                                        typeof(int)),
                                    sqlExpressionFactory.Constant(0)),
                                GetHaversineDistance(
                                    left,
                                    right,
                                    resultType,
                                    sqlExpressionFactory))
                        },
                        GetHaversineDistance(
                            SetSrid(left, 0, sqlExpressionFactory, options),
                            SetSrid(right, 0, sqlExpressionFactory, options),
                            resultType,
                            sqlExpressionFactory));
                }
            }

            if (algorithm == SpatialDistanceAlgorithm.Haversine)
            {
                return GetHaversineDistance(left, right, resultType, sqlExpressionFactory);
            }

            return GetAndoyerDistance(left, right, resultType, sqlExpressionFactory);
        }

        private static SqlExpression GetHaversineDistance(
            SqlExpression left,
            SqlExpression right,
            Type resultType,
            MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            // HAVERSINE = 6371000 * 2 * ASIN(
            //     SQRT(
            //         POWER(SIN((ST_Y(pt2) - ST_Y(pt1)) * pi()/180 / 2), 2) +
            //         COS(ST_Y(pt1) * pi()/180) *
            //         COS(ST_Y(pt2) * pi()/180) *
            //         POWER(SIN((ST_X(pt2) - ST_X(pt1)) * pi()/180 / 2), 2)
            //     )
            // )

            return sqlExpressionFactory.Multiply(
                sqlExpressionFactory.Constant(6370986.0), // see https://postgis.net/docs/manual-1.4/ST_Distance_Sphere.html
                sqlExpressionFactory.Multiply(
                    sqlExpressionFactory.Constant(2.0),
                    sqlExpressionFactory.NullableFunction(
                        "ASIN",
                        new[]
                        {
                            sqlExpressionFactory.NullableFunction(
                                "SQRT",
                                new[]
                                {
                                    sqlExpressionFactory.Add(
                                        sqlExpressionFactory.NullableFunction(
                                            "POWER",
                                            new SqlExpression[]
                                            {
                                                sqlExpressionFactory.NullableFunction(
                                                    "SIN",
                                                    new[]
                                                    {
                                                        sqlExpressionFactory.Divide(
                                                            sqlExpressionFactory.Divide(
                                                                sqlExpressionFactory.Multiply(
                                                                    sqlExpressionFactory.Subtract(
                                                                        sqlExpressionFactory.NullableFunction(
                                                                            "ST_Y",
                                                                            new[] {right},
                                                                            resultType),
                                                                        sqlExpressionFactory.NullableFunction(
                                                                            "ST_Y",
                                                                            new[] {left},
                                                                            resultType)),
                                                                    sqlExpressionFactory.NonNullableFunction(
                                                                        "PI",
                                                                        Array.Empty<SqlExpression>(),
                                                                        resultType)),
                                                                sqlExpressionFactory.Constant(180.0)),
                                                            sqlExpressionFactory.Constant(2.0))
                                                    },
                                                    resultType),
                                                sqlExpressionFactory.Constant(2),
                                            },
                                            resultType),
                                        sqlExpressionFactory.Multiply(
                                            sqlExpressionFactory.NullableFunction(
                                                "COS",
                                                new[]
                                                {
                                                    sqlExpressionFactory.Divide(
                                                        sqlExpressionFactory.Multiply(
                                                            sqlExpressionFactory.NullableFunction(
                                                                "ST_Y",
                                                                new[] {left},
                                                                resultType),
                                                            sqlExpressionFactory.NonNullableFunction(
                                                                "PI",
                                                                Array.Empty<SqlExpression>(),
                                                                resultType)),
                                                        sqlExpressionFactory.Constant(180.0)),
                                                },
                                                resultType),
                                            sqlExpressionFactory.Multiply(
                                                sqlExpressionFactory.NullableFunction(
                                                    "COS",
                                                    new[]
                                                    {
                                                        sqlExpressionFactory.Divide(
                                                            sqlExpressionFactory.Multiply(
                                                                sqlExpressionFactory.NullableFunction(
                                                                    "ST_Y",
                                                                    new[] {right},
                                                                    resultType),
                                                                sqlExpressionFactory.NonNullableFunction(
                                                                    "PI",
                                                                    Array.Empty<SqlExpression>(),
                                                                    resultType)),
                                                            sqlExpressionFactory.Constant(180.0)),
                                                    },
                                                    resultType),
                                                sqlExpressionFactory.NullableFunction(
                                                    "POWER",
                                                    new SqlExpression[]
                                                    {
                                                        sqlExpressionFactory.NullableFunction(
                                                            "SIN",
                                                            new[]
                                                            {
                                                                sqlExpressionFactory.Divide(
                                                                    sqlExpressionFactory.Divide(
                                                                        sqlExpressionFactory.Multiply(
                                                                            sqlExpressionFactory.Subtract(
                                                                                sqlExpressionFactory.NullableFunction(
                                                                                    "ST_X",
                                                                                    new[] {right},
                                                                                    resultType),
                                                                                sqlExpressionFactory.NullableFunction(
                                                                                    "ST_X",
                                                                                    new[] {left},
                                                                                    resultType)),
                                                                            sqlExpressionFactory.NonNullableFunction(
                                                                                "PI",
                                                                                Array.Empty<SqlExpression>(),
                                                                                resultType)),
                                                                        sqlExpressionFactory.Constant(180.0)),
                                                                    sqlExpressionFactory.Constant(2.0))
                                                            },
                                                            resultType),
                                                        sqlExpressionFactory.Constant(2),
                                                    },
                                                    resultType))))
                                },
                                resultType,
                                false)
                        },
                        resultType,
                        false)));
        }

        private static SqlExpression GetAndoyerDistance(
            SqlExpression left,
            SqlExpression right,
            Type resultType,
            MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            SqlExpression toDegrees(SqlExpression coord)
                => sqlExpressionFactory.Divide(
                    sqlExpressionFactory.Multiply(
                        coord,
                        sqlExpressionFactory.NonNullableFunction(
                            "PI",
                            Array.Empty<SqlExpression>(),
                            resultType)),
                    sqlExpressionFactory.Constant(180.0));

            SqlExpression xCoord(SqlExpression point)
                => sqlExpressionFactory.NullableFunction(
                    "ST_X",
                    new[] {point},
                    resultType);

            SqlExpression yCoord(SqlExpression point)
                => sqlExpressionFactory.NullableFunction(
                    "ST_Y",
                    new[] {point},
                    resultType);

            var c0 = sqlExpressionFactory.Constant(0.0);
            var c1 = sqlExpressionFactory.Constant(1.0);
            var c2 = sqlExpressionFactory.Constant(2.0);
            var c3 = sqlExpressionFactory.Constant(3.0);
            var c2Int = sqlExpressionFactory.Constant(2);

            var lon1 = toDegrees(xCoord(left));
            var lat1 = toDegrees(yCoord(left));
            var lon2 = toDegrees(xCoord(right));
            var lat2 = toDegrees(yCoord(right));

            var g = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Subtract(
                    lat1,
                    lat2),
                c2);
            var lambda = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Subtract(
                    lon1,
                    lon2),
                c2);

            var f = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Add(
                    lat1,
                    lat2),
                c2);

            var sinG2 = sqlExpressionFactory.NullableFunction(
                "POWER",
                new SqlExpression[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "SIN",
                        new[] {g},
                        resultType),
                    c2Int
                },
                resultType);
            var cosG2 = sqlExpressionFactory.NullableFunction(
                "POWER",
                new SqlExpression[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "COS",
                        new[] {g},
                        resultType),
                    c2Int
                },
                resultType);
            var sinF2 = sqlExpressionFactory.NullableFunction(
                "POWER",
                new SqlExpression[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "SIN",
                        new[] {f},
                        resultType),
                    c2Int
                },
                resultType);
            var cosF2 = sqlExpressionFactory.NullableFunction(
                "POWER",
                new SqlExpression[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "COS",
                        new[] {f},
                        resultType),
                    c2Int
                },
                resultType);
            var sinL2 = sqlExpressionFactory.NullableFunction(
                "POWER",
                new SqlExpression[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "SIN",
                        new[] {lambda},
                        resultType),
                    c2Int
                },
                resultType);
            var cosL2 = sqlExpressionFactory.NullableFunction(
                "POWER",
                new SqlExpression[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "COS",
                        new[] {lambda},
                        resultType),
                    c2Int
                },
                resultType);

            var s = sqlExpressionFactory.Add(
                sqlExpressionFactory.Multiply(sinG2, cosL2),
                sqlExpressionFactory.Multiply(cosF2, sinL2));
            var c = sqlExpressionFactory.Add(
                sqlExpressionFactory.Multiply(cosG2, cosL2),
                sqlExpressionFactory.Multiply(sinF2, sinL2));

            var radiusA = sqlExpressionFactory.Constant(6378137.0);
            var radiusB = sqlExpressionFactory.Constant(6356752.3142451793);
            var flattening = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Subtract(radiusA, radiusB),
                radiusA);

            var omega = sqlExpressionFactory.NullableFunction(
                "ATAN",
                new[]
                {
                    sqlExpressionFactory.NullableFunction(
                        "SQRT",
                        new[] {sqlExpressionFactory.Divide(s, c)},
                        resultType,
                        false)
                },
                resultType);
            var r3 = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Multiply(
                    c3,
                    sqlExpressionFactory.NullableFunction(
                        "SQRT",
                        new[] {sqlExpressionFactory.Multiply(s, c)},
                        resultType,
                        false)),
                omega);
            var d = sqlExpressionFactory.Multiply(
                sqlExpressionFactory.Multiply(c2, omega),
                radiusA);
            var h1 = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Subtract(r3, c1),
                sqlExpressionFactory.Multiply(c2, c));
            var h2 = sqlExpressionFactory.Divide(
                sqlExpressionFactory.Add(r3, c1),
                sqlExpressionFactory.Multiply(c2, s));

            var andoyer = sqlExpressionFactory.Multiply(
                d,
                sqlExpressionFactory.Add(
                    c1,
                    sqlExpressionFactory.Multiply(
                        flattening,
                        sqlExpressionFactory.Subtract(
                            sqlExpressionFactory.Multiply(
                                sqlExpressionFactory.Multiply(
                                    h1,
                                    sinF2),
                                cosG2),
                            sqlExpressionFactory.Multiply(
                                sqlExpressionFactory.Multiply(
                                    h2,
                                    cosF2),
                                sinG2)))));

            return sqlExpressionFactory.Case(
                new[]
                {
                    new CaseWhenClause(
                        sqlExpressionFactory.OrElse(
                            sqlExpressionFactory.OrElse(
                                sqlExpressionFactory.AndAlso(
                                    sqlExpressionFactory.Equal(lambda, c0),
                                    sqlExpressionFactory.Equal(g, c0)),
                                sqlExpressionFactory.Equal(s, c0)),
                            sqlExpressionFactory.Equal(c, c0)),
                        c0),
                },
                andoyer);
        }
    }
}
