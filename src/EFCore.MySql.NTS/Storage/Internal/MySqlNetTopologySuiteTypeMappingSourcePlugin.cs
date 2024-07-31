// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" /> and multiple registrations
    ///         are allowed. This means a single instance of each service is used by many <see cref="DbContext" />
    ///         instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class MySqlNetTopologySuiteTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
    {
        private static readonly Dictionary<string, Type> _spatialStoreTypeMappings = new Dictionary<string, Type>
        {
            { "geometry", typeof(Geometry) },                     // geometry
            { "point", typeof(Point) },                           // geometry -> point
            { "curve", typeof(Geometry) },                        // geometry -> curve
            { "linestring", typeof(LineString) },                 // geometry -> curve -> linestring
            { "line", typeof(LineString) },                       // geometry -> curve -> linestring -> line
            { "linearring", typeof(LinearRing) },                 // geometry -> curve -> linestring -> linearring
            { "surface", typeof(Geometry) },                      // geometry -> surface
            { "polygon", typeof(Polygon) },                       // geometry -> surface -> polygon
            { "geometrycollection", typeof(GeometryCollection) }, // geometry -> geometrycollection
            { "multipoint", typeof(MultiPoint) },                 // geometry -> geometrycollection -> multipoint
            { "multicurve", typeof(GeometryCollection) },         // geometry -> geometrycollection -> multicurve
            { "multilinestring", typeof(MultiLineString) },       // geometry -> geometrycollection -> multicurve -> multilinestring
            { "multisurface", typeof(GeometryCollection) },       // geometry -> geometrycollection -> multisurface
            { "multipolygon", typeof(MultiPolygon) },             // geometry -> geometrycollection -> multisurface -> multipolygon
        };

        private readonly NtsGeometryServices _geometryServices;
        private readonly IMySqlOptions _options;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlNetTopologySuiteTypeMappingSourcePlugin(
            [NotNull] NtsGeometryServices geometryServices,
            [NotNull] IMySqlOptions options)
        {
            Check.NotNull(geometryServices, nameof(geometryServices));

            _geometryServices = geometryServices;
            _options = options;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName?.ToLowerInvariant();
            string defaultStoreType = null;
            Type defaultClrType = null;

            var hasDefaultStoreType = clrType != null
                    && TryGetDefaultStoreType(clrType, out defaultStoreType);
            var hasDefaultClrType = storeTypeName != null
                       && _spatialStoreTypeMappings.TryGetValue(storeTypeName, out defaultClrType);

            if (!(hasDefaultStoreType || hasDefaultClrType))
            {
                return null;
            }

            // NOTE: If the incoming user-specified 'clrType' is of the known calculated 'defaultClrType', ONLY then proceeed
            // with the creation of 'MySqlGeometryTypeMapping'.
            clrType = clrType == null
                ? defaultClrType
                : clrType.IsAssignableFrom(defaultClrType)
                    ? clrType
                    : null;
            return clrType == null
                ? null
                : (RelationalTypeMapping)Activator.CreateInstance(
                    typeof(MySqlGeometryTypeMapping<>).MakeGenericType(clrType ?? defaultClrType ?? typeof(Geometry)),
                    _geometryServices,
                    storeTypeName ?? defaultStoreType ?? "geometry",
                    _options);
        }

        private static bool TryGetDefaultStoreType(Type type, out string defaultStoreType)
        {
            // geometry -> geometrycollection -> multisurface -> multipolygon
            if (typeof(MultiPolygon).IsAssignableFrom(type))
            {
                defaultStoreType = "multipolygon";
            }
            // geometry -> geometrycollection -> multicurve -> multilinestring
            else if (typeof(MultiLineString).IsAssignableFrom(type))
            {
                defaultStoreType = "multilinestring";
            }
            // geometry -> geometrycollection -> multipoint
            else if (typeof(MultiPoint).IsAssignableFrom(type))
            {
                defaultStoreType = "multipoint";
            }
            // geometry -> geometrycollection
            else if (typeof(GeometryCollection).IsAssignableFrom(type))
            {
                defaultStoreType = "geometrycollection";
            }
            // geometry -> surface -> polygon
            else if (typeof(Polygon).IsAssignableFrom(type))
            {
                defaultStoreType = "polygon";
            }
            // geometry -> curve -> linestring -> linearring
            else if (typeof(LinearRing).IsAssignableFrom(type))
            {
                defaultStoreType = "linearring";
            }
            // geometry -> curve -> linestring
            else if (typeof(LineString).IsAssignableFrom(type))
            {
                defaultStoreType = "linestring";
            }
            // geometry -> point
            else if (typeof(Point).IsAssignableFrom(type))
            {
                defaultStoreType = "point";
            }
            // geometry
            else if (typeof(Geometry).IsAssignableFrom(type))
            {
                defaultStoreType = "geometry";
            }
            else
            {
                defaultStoreType = null;
            }

            return defaultStoreType != null;
        }
    }
}
