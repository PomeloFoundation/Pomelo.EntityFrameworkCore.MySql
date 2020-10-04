// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Concurrent;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.ValueConversion.Internal
{
    internal static class GeometryValueConverter
    {
        internal static readonly ConcurrentDictionary<int, NtsGeometryServices> GeometryServices = new ConcurrentDictionary<int, NtsGeometryServices>();
    }

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

        private static MySqlGeometry ConvertToProviderCore(TGeometry v)
            => MySqlGeometry.FromWkb(v.SRID, v.ToBinary());

        private static TGeometry ConvertFromProviderCore(MySqlGeometry v)
        {
            using var memoryStream = new MemoryStream(v.Value);

            // MySQL starts it's spatial data with a 4 byte SRID, that is unexpected by WKBReader.
            var biEndianBinaryReader = new BiEndianBinaryReader(memoryStream, ByteOrder.LittleEndian);
            var srid = biEndianBinaryReader.ReadInt32();

            var geometryServices = GeometryValueConverter.GeometryServices.GetOrAdd(
                srid,
                b => new NtsGeometryServices(
                    NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory,
                    NtsGeometryServices.Instance.DefaultPrecisionModel,
                    b));

            var reader = new WKBReader(geometryServices);
            var geometry = reader.Read(memoryStream);

            if (geometry.SRID == -1 &&
                geometry.SRID != srid)
            {
                geometry.SRID = srid;
            }

            return (TGeometry)geometry;
        }
    }
}
