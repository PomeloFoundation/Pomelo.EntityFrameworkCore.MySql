using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Used to specify one of multiple spherical distance algorithms.
    /// </summary>
    public enum SpatialDistanceAlgorithm
    {
        /// <summary>
        /// Uses the native server implementation of the `ST_Distance_Sphere()` function.
        /// If the function is not implemented, the `Andoyer` algorithm is used as a fallback.
        /// </summary>
        Native,
        /// <summary>
        /// For MySQL, uses the native server implementation of the `ST_Distance()` function, which already
        /// implements the Andoyer algorithm when used with SRID 4326.
        /// If the function is not implemented, a custom implementation of the `Andoyer` algorithm is used.
        /// </summary>
        Andoyer,
        /// <summary>
        /// Uses a custom implementation of the `Haversine` algorithm.
        /// </summary>
        Haversine,
    }

    /// <summary>
    ///     Provides CLR methods that get translated to database functions when used in LINQ to Entities queries.
    ///     The methods on this class are accessed via <see cref="EF.Functions" />.
    /// </summary>
    public static class MySqlSpatialDbFunctionsExtensions
    {
        /// <summary>
        ///     Returns the distance between g1 and g2, measured in the length unit of the spatial reference system
        ///     (SRS) of the geometry arguments.
        ///     For MySQL 8, this is equivalent of calling ST_Distance() when using an SRID of `0`.
        ///     For MySQL &lt; 8 and MariaDB, this is equivalent of calling ST_Distance() with any SRID.
        /// </summary>
        /// <param name="_">The DbFunctions instance.</param>
        /// <param name="g1">First geometry argument.</param>
        /// <param name="g2">Second geometry argument.</param>
        /// <returns>Distance betweeen g1 and g2.</returns>
        public static double SpatialDistancePlanar(
            [CanBeNull] this DbFunctions _,
            Geometry g1,
            Geometry g2)
            => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(SpatialDistancePlanar)));

        /// <summary>
        ///     Returns the mimimum spherical distance between Point or MultiPoint arguments on a sphere in meters,
        ///     by using the specified algorithm.
        ///     It is assumed that `g1` and `g2` are associated with an SRID of `4326`.
        /// </summary>
        /// <param name="_">The DbFunctions instance.</param>
        /// <param name="g1">First geometry argument.</param>
        /// <param name="g2">Second geometry argument.</param>
        /// <param name="algorithm">The algorithm to use. Must be directly supplied as a constant.</param>
        /// <returns>Distance betweeen g1 and g2.</returns>
        public static double SpatialDistanceSphere(
            [CanBeNull] this DbFunctions _,
            Geometry g1,
            Geometry g2,
            SpatialDistanceAlgorithm algorithm)
            => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(SpatialDistanceSphere)));
    }
}
