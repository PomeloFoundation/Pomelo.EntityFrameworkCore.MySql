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

        private static readonly Dictionary<Type, string> _spatialClrTypeMappings = new Dictionary<Type, string>
        {
            { typeof(Geometry), "geometry" },                     // geometry
            { typeof(Point), "point" },                           // geometry -> point
            { typeof(LineString), "linestring" },                 // geometry -> curve -> linestring
            { typeof(LinearRing), "linearring" },                 // geometry -> curve -> linestring -> linearring
            { typeof(Polygon), "polygon" },                       // geometry -> surface -> polygon
            { typeof(GeometryCollection), "geometrycollection" }, // geometry -> geometrycollection
            { typeof(MultiPoint), "multipoint" },                 // geometry -> geometrycollection -> multipoint
            { typeof(MultiLineString), "multilinestring" },       // geometry -> geometrycollection -> multicurve -> multilinestring
            { typeof(MultiPolygon), "multipolygon" },             // geometry -> geometrycollection -> multisurface -> multipolygon
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
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                // First look for the fully qualified store type name.
                if (_spatialStoreTypeMappings.TryGetValue(storeTypeName, out var mappedClrType))
                {
                    // We found the user-specified store type.
                    // If no CLR type was provided, we're probably scaffolding from an existing database. Take the first
                    // mapping as the default.
                    // If a CLR type was provided, look for a mapping between the store and CLR types. If none is found,
                    // fail immediately.
                    clrType = clrType == null
                        ? mappedClrType
                        : clrType.IsAssignableFrom(mappedClrType)
                            ? clrType
                            : null;

                    return clrType == null
                        ? null
                        : (RelationalTypeMapping)Activator.CreateInstance(
                            typeof(MySqlGeometryTypeMapping<>).MakeGenericType(clrType),
                            _geometryServices,
                            storeTypeName,
                            _options);
                }

                // Then look for the base store type name.
                if (_spatialStoreTypeMappings.TryGetValue(storeTypeNameBase, out mappedClrType))
                {
                    clrType = clrType == null
                        ? mappedClrType
                        : clrType.IsAssignableFrom(mappedClrType)
                            ? clrType
                            : null;

                    if (clrType == null)
                    {
                        return null;
                    }

                    var typeMapping = (RelationalTypeMapping)Activator.CreateInstance(
                        typeof(MySqlGeometryTypeMapping<>).MakeGenericType(clrType),
                        _geometryServices,
                        storeTypeName,
                        _options);

                    return typeMapping.Clone(mappingInfo);
                }

                // A store type name was provided, but is unknown. This could be a domain (alias) type, in which case
                // we proceed with a CLR type lookup (if the type doesn't exist at all the failure will come later).
            }

            if (clrType != null &&
                _spatialClrTypeMappings.TryGetValue(clrType, out var mappedStoreTypeName))
            {
                return (RelationalTypeMapping)Activator.CreateInstance(
                    typeof(MySqlGeometryTypeMapping<>).MakeGenericType(clrType),
                    _geometryServices,
                    mappedStoreTypeName,
                    _options);
            }

            return null;
        }
    }
}
