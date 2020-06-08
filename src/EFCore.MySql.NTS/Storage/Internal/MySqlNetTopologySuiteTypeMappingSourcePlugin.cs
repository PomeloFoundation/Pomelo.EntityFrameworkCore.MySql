// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;

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
        private readonly NtsGeometryServices _geometryServices;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlNetTopologySuiteTypeMappingSourcePlugin([NotNull] NtsGeometryServices geometryServices)
        {
            Check.NotNull(geometryServices, nameof(geometryServices));

            _geometryServices = geometryServices;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType ?? typeof(Geometry);
            var storeTypeName = mappingInfo.StoreTypeName;

            return typeof(Geometry).IsAssignableFrom(clrType)
                   || storeTypeName != null
                   && storeTypeName.Equals("geometry", StringComparison.OrdinalIgnoreCase)
                ? (RelationalTypeMapping)Activator.CreateInstance(
                    typeof(MySqlGeometryTypeMapping<>).MakeGenericType(clrType),
                    _geometryServices,
                    storeTypeName ?? "geometry")
                : null;
        }
    }
}
