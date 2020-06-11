// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySql.Data.Types;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.ValueConversion.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class GeometryValueConverter<TGeometry> : ValueConverter<TGeometry, MySqlGeometry>
        where TGeometry : Geometry
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public GeometryValueConverter(NtsGeometryServices geometryServices)
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly ConcurrentDictionary<uint, NtsGeometryServices> _geometryServiceses = new ConcurrentDictionary<uint, NtsGeometryServices>();

        private static MySqlGeometry ConvertToProviderCore(TGeometry v)
        {
            var geometry = MySqlGeometry.FromWkb(v.SRID, v.ToBinary());
#if DEBUG
            var hexString = BitConverter.ToString(geometry.Value).Replace("-", string.Empty);
            Debug.WriteLine("Spatial to provider: " + hexString);
#endif
            return geometry;
        }

        private static TGeometry ConvertFromProviderCore(MySqlGeometry v)
        {
#if DEBUG
            var hexString = BitConverter.ToString(v.Value).Replace("-", string.Empty);
            Debug.WriteLine("Spatial from provider: " + hexString);
#endif
            using var memoryStream = new MemoryStream(v.Value);

            // MySQL starts it's spatial data with a 4 byte SRID, that is unexpected by WKBReader.
            var biEndianBinaryReader = new BiEndianBinaryReader(memoryStream);
            var srid = biEndianBinaryReader.ReadUInt32();

            var geometryServices = _geometryServiceses.GetOrAdd(
                srid,
                b => new NtsGeometryServices(
                    NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory,
                    NtsGeometryServices.Instance.DefaultPrecisionModel,
                    (int)b));

            var reader = new WKBReader(geometryServices);
            var geometry = reader.Read(memoryStream);

            return (TGeometry)geometry;
        }
    }
}
