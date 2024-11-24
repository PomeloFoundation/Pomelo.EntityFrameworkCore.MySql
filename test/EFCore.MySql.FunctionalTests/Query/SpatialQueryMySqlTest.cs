using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SpatialQueryMySqlTest : SpatialQueryRelationalTestBase<SpatialQueryMySqlFixture>
    {
        public SpatialQueryMySqlTest(SpatialQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialFunctionAdditions))]
        public override Task Boundary(bool async)
            => base.Boundary(async);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialFunctionAdditions), Skip = "MySQL is unable to work with different SRIDs.")]
        public override Task Distance_constant_srid_4326(bool async)
            => base.Distance_constant_srid_4326(async);

        [ConditionalTheory]
        public override Task GeometryType(bool async)
            => AssertQuery(
                async,
                ss => ss.Set<PointEntity>().Select(
                    e => new { e.Id, GeometryType = e.Point == null ? null : e.Point.GeometryType.ToLower() }),
                elementSorter: x => x.Id);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialFunctionAdditions))]
        public override Task InteriorPoint(bool async)
            => base.InteriorPoint(async);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialSupportFunctionAdditions))]
        public override Task IsValid(bool async)
            => base.IsValid(async);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialFunctionAdditions))]
        public override Task PointOnSurface(bool async)
            => base.PointOnSurface(async);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialFunctionAdditions))]
        public override Task Relate(bool async)
            => base.Relate(async);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialSupportFunctionAdditions))] // Actually supported since MySQL 5.7.5 (not 5.7.6)
        public override Task ConvexHull(bool async)
            => base.ConvexHull(async);

        public override Task Combine_aggregate(bool async)
            => AssertQuery(
                async,
                ss => ss.Set<PointEntity>()
                    .Where(e => e.Point != null)
                    .GroupBy(e => e.Group)
                    .Select(g => new { Id = g.Key, Combined = GeometryCombiner.Combine(g.Select(e => e.Point)
                        .OrderBy(p => p.X).ThenBy(p => p.Y)) }), // <-- needs to be explicitly ordered to be deterministic
                elementSorter: x => x.Id,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.Id, a.Id);

                    // Note that NTS returns a MultiPoint (which is a subclass of GeometryCollection), whereas SQL Server returns a
                    // GeometryCollection.
                    var eCollection = (GeometryCollection)e.Combined;
                    var aCollection = (GeometryCollection)a.Combined;

                    Assert.Equal(eCollection.Geometries, aCollection.Geometries);
                });

        public override async Task SimpleSelect(bool async)
        {
            await base.SimpleSelect(async);

            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Geometry`, `p`.`Group`, `p`.`Point`, `p`.`PointM`, `p`.`PointZ`, `p`.`PointZM`
FROM `PointEntity` AS `p`
""",
                //
                """
SELECT `l`.`Id`, `l`.`LineString`
FROM `LineStringEntity` AS `l`
""",
                //
                """
SELECT `p`.`Id`, `p`.`Polygon`
FROM `PolygonEntity` AS `p`
""",
                //
                """
SELECT `m`.`Id`, `m`.`MultiLineString`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task WithConversion(bool async)
        {
            await base.WithConversion(async);

            AssertSql(
"""
SELECT `g`.`Id`, `g`.`Location`
FROM `GeoPointEntity` AS `g`
""");
        }

        public override async Task Area(bool async)
        {
            await base.Area(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_Area(`p`.`Polygon`) AS `Area`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task AsBinary(bool async)
        {
            await base.AsBinary(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_AsBinary(`p`.`Point`) AS `Binary`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task AsBinary_with_null_check(bool async)
        {
            await base.AsBinary_with_null_check(async);

            AssertSql(
"""
SELECT `p`.`Id`, CASE
    WHEN `p`.`Point` IS NULL THEN NULL
    ELSE ST_AsBinary(`p`.`Point`)
END AS `Binary`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task AsText(bool async)
        {
            await base.AsText(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_AsText(`p`.`Point`) AS `Text`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Buffer(bool async)
        {
            await base.Buffer(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_Buffer(`p`.`Polygon`, 1.0) AS `Buffer`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Centroid(bool async)
        {
            await base.Centroid(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_Centroid(`p`.`Polygon`) AS `Centroid`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task EnvelopeCombine_aggregate(bool async)
        {
            await base.EnvelopeCombine_aggregate(async);

            AssertSql(
"""
SELECT `p1`.`Group`, `p2`.`Point`, `p2`.`Id`
FROM (
    SELECT `p`.`Group`
    FROM `PointEntity` AS `p`
    WHERE `p`.`Point` IS NOT NULL
    GROUP BY `p`.`Group`
) AS `p1`
LEFT JOIN (
    SELECT `p0`.`Point`, `p0`.`Id`, `p0`.`Group`
    FROM `PointEntity` AS `p0`
    WHERE `p0`.`Point` IS NOT NULL
) AS `p2` ON `p1`.`Group` = `p2`.`Group`
ORDER BY `p1`.`Group`
""");
        }

        public override async Task Contains(bool async)
        {
            await base.Contains(async);

            AssertSql(
"""
@__point_0='0x000000000101000000000000000000D03F000000000000D03F' (DbType = Binary)

SELECT `p`.`Id`, ST_Contains(`p`.`Polygon`, @__point_0) AS `Contains`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task ConvexHull_aggregate(bool async)
        {
            await base.ConvexHull_aggregate(async);

            AssertSql(
"""
SELECT `p1`.`Group`, `p2`.`Point`, `p2`.`Id`
FROM (
    SELECT `p`.`Group`
    FROM `PointEntity` AS `p`
    WHERE `p`.`Point` IS NOT NULL
    GROUP BY `p`.`Group`
) AS `p1`
LEFT JOIN (
    SELECT `p0`.`Point`, `p0`.`Id`, `p0`.`Group`
    FROM `PointEntity` AS `p0`
    WHERE `p0`.`Point` IS NOT NULL
) AS `p2` ON `p1`.`Group` = `p2`.`Group`
ORDER BY `p1`.`Group`
""");
        }

        public override async Task IGeometryCollection_Count(bool async)
        {
            await base.IGeometryCollection_Count(async);

            AssertSql(
"""
SELECT `m`.`Id`, ST_NumGeometries(`m`.`MultiLineString`) AS `Count`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task LineString_Count(bool async)
        {
            await base.LineString_Count(async);

            AssertSql(
"""
SELECT `l`.`Id`, ST_NumPoints(`l`.`LineString`) AS `Count`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task Crosses(bool async)
        {
            await base.Crosses(async);

            AssertSql(
"""
@__lineString_0='0x00000000010200000002000000000000000000E03F000000000000E0BF000000...' (DbType = Binary)

SELECT `l`.`Id`, ST_Crosses(`l`.`LineString`, @__lineString_0) AS `Crosses`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task Difference(bool async)
        {
            await base.Difference(async);

            AssertSql(
"""
@__polygon_0='0x0000000001030000000100000004000000000000000000000000000000000000...' (DbType = Binary)

SELECT `p`.`Id`, ST_Difference(`p`.`Polygon`, @__polygon_0) AS `Difference`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Dimension(bool async)
        {
            await base.Dimension(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_Dimension(`p`.`Point`) AS `Dimension`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Disjoint_with_cast_to_nullable(bool async)
        {
            await base.Disjoint_with_cast_to_nullable(async);

            AssertSql(
"""
@__point_0='0x000000000101000000000000000000F03F000000000000F03F' (DbType = Binary)

SELECT `p`.`Id`, ST_Disjoint(`p`.`Polygon`, @__point_0) AS `Disjoint`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Disjoint_with_null_check(bool async)
        {
            await base.Disjoint_with_null_check(async);

            AssertSql(
"""
@__point_0='0x000000000101000000000000000000F03F000000000000F03F' (DbType = Binary)

SELECT `p`.`Id`, CASE
    WHEN `p`.`Polygon` IS NULL THEN NULL
    ELSE ST_Disjoint(`p`.`Polygon`, @__point_0)
END AS `Disjoint`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Distance_with_null_check(bool async)
        {
            await base.Distance_with_null_check(async);

            AssertSql(
"""
@__point_0='0x0000000001010000000000000000000000000000000000F03F' (DbType = Binary)

SELECT `p`.`Id`, CASE
    WHEN ST_SRID(`p`.`Point`) = 4326 THEN ST_Distance_Sphere(`p`.`Point`, @__point_0)
    ELSE ST_Distance(`p`.`Point`, @__point_0)
END AS `Distance`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Distance_with_cast_to_nullable(bool async)
        {
            await base.Distance_with_cast_to_nullable(async);

            AssertSql(
"""
@__point_0='0x0000000001010000000000000000000000000000000000F03F' (DbType = Binary)

SELECT `p`.`Id`, CASE
    WHEN ST_SRID(`p`.`Point`) = 4326 THEN ST_Distance_Sphere(`p`.`Point`, @__point_0)
    ELSE ST_Distance(`p`.`Point`, @__point_0)
END AS `Distance`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Distance_geometry(bool async)
        {
            await base.Distance_geometry(async);

            AssertSql(
"""
@__point_0='0x0000000001010000000000000000000000000000000000F03F' (DbType = Binary)

SELECT `p`.`Id`, CASE
    WHEN ST_SRID(`p`.`Geometry`) = 4326 THEN ST_Distance_Sphere(`p`.`Geometry`, @__point_0)
    ELSE ST_Distance(`p`.`Geometry`, @__point_0)
END AS `Distance`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Distance_constant(bool async)
        {
            await base.Distance_constant(async);

            AssertSql(
"""
SELECT `p`.`Id`, CASE
    WHEN ST_SRID(`p`.`Point`) = 4326 THEN ST_Distance_Sphere(`p`.`Point`, X'0000000001010000000000000000000000000000000000F03F')
    ELSE ST_Distance(`p`.`Point`, X'0000000001010000000000000000000000000000000000F03F')
END AS `Distance`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Distance_constant_lhs(bool async)
        {
            await base.Distance_constant_lhs(async);

            AssertSql(
"""
SELECT `p`.`Id`, CASE
    WHEN ST_SRID(X'0000000001010000000000000000000000000000000000F03F') = 4326 THEN ST_Distance_Sphere(X'0000000001010000000000000000000000000000000000F03F', `p`.`Point`)
    ELSE ST_Distance(X'0000000001010000000000000000000000000000000000F03F', `p`.`Point`)
END AS `Distance`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Distance_on_converted_geometry_type(bool async)
        {
            await base.Distance_on_converted_geometry_type(async);

            AssertSql(
"""
SELECT `g`.`Id`, `g`.`Location`
FROM `GeoPointEntity` AS `g`
""");
        }

        public override async Task Distance_on_converted_geometry_type_lhs(bool async)
        {
            await base.Distance_on_converted_geometry_type_lhs(async);

            AssertSql(
"""
SELECT `g`.`Id`, `g`.`Location`
FROM `GeoPointEntity` AS `g`
""");
        }

        public override async Task Distance_on_converted_geometry_type_constant(bool async)
        {
            await base.Distance_on_converted_geometry_type_constant(async);

            AssertSql(
"""
SELECT `g`.`Id`, `g`.`Location`
FROM `GeoPointEntity` AS `g`
""");
        }

        public override async Task Distance_on_converted_geometry_type_constant_lhs(bool async)
        {
            await base.Distance_on_converted_geometry_type_constant_lhs(async);

            AssertSql(
"""
SELECT `g`.`Id`, `g`.`Location`
FROM `GeoPointEntity` AS `g`
""");
        }

        public override async Task EndPoint(bool async)
        {
            await base.EndPoint(async);

            AssertSql(
"""
SELECT `l`.`Id`, ST_EndPoint(`l`.`LineString`) AS `EndPoint`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task Envelope(bool async)
        {
            await base.Envelope(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_Envelope(`p`.`Polygon`) AS `Envelope`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task EqualsTopologically(bool async)
        {
            await base.EqualsTopologically(async);

            AssertSql(
"""
@__point_0='0x00000000010100000000000000000000000000000000000000' (DbType = Binary)

SELECT `p`.`Id`, ST_Equals(`p`.`Point`, @__point_0) AS `EqualsTopologically`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task ExteriorRing(bool async)
        {
            await base.ExteriorRing(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_ExteriorRing(`p`.`Polygon`) AS `ExteriorRing`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task GetGeometryN(bool async)
        {
            await base.GetGeometryN(async);

            AssertSql(
"""
SELECT `m`.`Id`, ST_GeometryN(`m`.`MultiLineString`, 0 + 1) AS `Geometry0`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task GetGeometryN_with_null_argument(bool async)
        {
            await base.GetGeometryN_with_null_argument(async);

            AssertSql(
"""
SELECT `m`.`Id`, ST_GeometryN(`m`.`MultiLineString`, (
    SELECT MAX(`m0`.`Id`)
    FROM `MultiLineStringEntity` AS `m0`
    WHERE FALSE) + 1) AS `Geometry0`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task GetInteriorRingN(bool async)
        {
            await base.GetInteriorRingN(async);

            AssertSql(
"""
SELECT `p`.`Id`, CASE
    WHEN ST_NumInteriorRings(`p`.`Polygon`) = 0 THEN NULL
    ELSE ST_InteriorRingN(`p`.`Polygon`, 0 + 1)
END AS `InteriorRing0`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task GetPointN(bool async)
        {
            await base.GetPointN(async);

            AssertSql(
"""
SELECT `l`.`Id`, ST_PointN(`l`.`LineString`, 0 + 1) AS `Point0`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task Intersection(bool async)
        {
            await base.Intersection(async);

            AssertSql(
"""
@__polygon_0='0x0000000001030000000100000004000000000000000000000000000000000000...' (DbType = Binary)

SELECT `p`.`Id`, ST_Intersection(`p`.`Polygon`, @__polygon_0) AS `Intersection`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Intersects(bool async)
        {
            await base.Intersects(async);

            AssertSql(
"""
@__lineString_0='0x00000000010200000002000000000000000000E03F000000000000E0BF000000...' (DbType = Binary)

SELECT `l`.`Id`, ST_Intersects(`l`.`LineString`, @__lineString_0) AS `Intersects`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task ICurve_IsClosed(bool async)
        {
            await base.ICurve_IsClosed(async);

            AssertSql(
"""
SELECT `l`.`Id`, CASE
    WHEN `l`.`LineString` IS NULL THEN NULL
    ELSE ST_IsClosed(`l`.`LineString`)
END AS `IsClosed`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task IMultiCurve_IsClosed(bool async)
        {
            await base.IMultiCurve_IsClosed(async);

            AssertSql(
"""
SELECT `m`.`Id`, CASE
    WHEN `m`.`MultiLineString` IS NULL THEN NULL
    ELSE ST_IsClosed(`m`.`MultiLineString`)
END AS `IsClosed`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task IsEmpty(bool async)
        {
            await base.IsEmpty(async);

            AssertSql(
"""
SELECT `m`.`Id`, CASE
    WHEN `m`.`MultiLineString` IS NULL THEN NULL
    ELSE ST_IsEmpty(`m`.`MultiLineString`)
END AS `IsEmpty`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task IsRing(bool async)
        {
            await base.IsRing(async);

            AssertSql(
"""
SELECT `l`.`Id`, CASE
    WHEN `l`.`LineString` IS NULL THEN NULL
    ELSE ST_IsClosed(`l`.`LineString`) AND ST_IsSimple(`l`.`LineString`)
END AS `IsRing`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task IsSimple(bool async)
        {
            await base.IsSimple(async);

            AssertSql(
"""
SELECT `l`.`Id`, CASE
    WHEN `l`.`LineString` IS NULL THEN NULL
    ELSE ST_IsSimple(`l`.`LineString`)
END AS `IsSimple`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task IsWithinDistance(bool async)
        {
            await base.IsWithinDistance(async);

            AssertSql(
"""
@__point_0='0x0000000001010000000000000000000000000000000000F03F' (DbType = Binary)

SELECT `p`.`Id`, CASE
    WHEN CASE
        WHEN ST_SRID(`p`.`Point`) = 4326 THEN ST_Distance_Sphere(`p`.`Point`, @__point_0)
        ELSE ST_Distance(`p`.`Point`, @__point_0)
    END <= 1.0 THEN TRUE
    ELSE FALSE
END AS `IsWithinDistance`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Item(bool async)
        {
            await base.Item(async);

            AssertSql(
"""
SELECT `m`.`Id`, ST_GeometryN(`m`.`MultiLineString`, 0 + 1) AS `Item0`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task Length(bool async)
        {
            await base.Length(async);

            AssertSql(
"""
SELECT `l`.`Id`, ST_Length(`l`.`LineString`) AS `Length`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task NumGeometries(bool async)
        {
            await base.NumGeometries(async);

            AssertSql(
"""
SELECT `m`.`Id`, ST_NumGeometries(`m`.`MultiLineString`) AS `NumGeometries`
FROM `MultiLineStringEntity` AS `m`
""");
        }

        public override async Task NumInteriorRings(bool async)
        {
            await base.NumInteriorRings(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_NumInteriorRings(`p`.`Polygon`) AS `NumInteriorRings`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task NumPoints(bool async)
        {
            await base.NumPoints(async);

            AssertSql(
"""
SELECT `l`.`Id`, ST_NumPoints(`l`.`LineString`) AS `NumPoints`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task OgcGeometryType(bool async)
        {
            await base.OgcGeometryType(async);

            AssertSql(
"""
SELECT `p`.`Id`, CASE ST_GeometryType(`p`.`Point`)
    WHEN 'Point' THEN 1
    WHEN 'LineString' THEN 2
    WHEN 'Polygon' THEN 3
    WHEN 'MultiPoint' THEN 4
    WHEN 'MultiLineString' THEN 5
    WHEN 'MultiPolygon' THEN 6
    WHEN 'GeometryCollection' THEN 7
    WHEN 'CircularString' THEN 8
    WHEN 'CompoundCurve' THEN 9
    WHEN 'CurvePolygon' THEN 10
END AS `OgcGeometryType`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Overlaps(bool async)
        {
            await base.Overlaps(async);

            AssertSql(
"""
@__polygon_0='0x0000000001030000000100000004000000000000000000000000000000000000...' (DbType = Binary)

SELECT `p`.`Id`, ST_Overlaps(`p`.`Polygon`, @__polygon_0) AS `Overlaps`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task SRID(bool async)
        {
            await base.SRID(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_SRID(`p`.`Point`) AS `SRID`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task SRID_geometry(bool async)
        {
            await base.SRID_geometry(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_SRID(`p`.`Geometry`) AS `SRID`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task StartPoint(bool async)
        {
            await base.StartPoint(async);

            AssertSql(
"""
SELECT `l`.`Id`, ST_StartPoint(`l`.`LineString`) AS `StartPoint`
FROM `LineStringEntity` AS `l`
""");
        }

        public override async Task SymmetricDifference(bool async)
        {
            await base.SymmetricDifference(async);

            AssertSql(
"""
@__polygon_0='0x0000000001030000000100000004000000000000000000000000000000000000...' (DbType = Binary)

SELECT `p`.`Id`, ST_SymDifference(`p`.`Polygon`, @__polygon_0) AS `SymmetricDifference`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task ToBinary(bool async)
        {
            await base.ToBinary(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_AsBinary(`p`.`Point`) AS `Binary`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task ToText(bool async)
        {
            await base.ToText(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_AsText(`p`.`Point`) AS `Text`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Touches(bool async)
        {
            await base.Touches(async);

            AssertSql(
"""
@__polygon_0='0x00000000010300000001000000040000000000000000000000000000000000F0...' (DbType = Binary)

SELECT `p`.`Id`, ST_Touches(`p`.`Polygon`, @__polygon_0) AS `Touches`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Union(bool async)
        {
            await base.Union(async);

            AssertSql(
"""
@__polygon_0='0x0000000001030000000100000004000000000000000000000000000000000000...' (DbType = Binary)

SELECT `p`.`Id`, ST_Union(`p`.`Polygon`, @__polygon_0) AS `Union`
FROM `PolygonEntity` AS `p`
""");
        }

        public override async Task Union_aggregate(bool async)
        {
            await base.Union_aggregate(async);

            AssertSql(
"""
SELECT `p1`.`Group`, `p2`.`Point`, `p2`.`Id`
FROM (
    SELECT `p`.`Group`
    FROM `PointEntity` AS `p`
    WHERE `p`.`Point` IS NOT NULL
    GROUP BY `p`.`Group`
) AS `p1`
LEFT JOIN (
    SELECT `p0`.`Point`, `p0`.`Id`, `p0`.`Group`
    FROM `PointEntity` AS `p0`
    WHERE `p0`.`Point` IS NOT NULL
) AS `p2` ON `p1`.`Group` = `p2`.`Group`
ORDER BY `p1`.`Group`
""");
        }

        public override async Task Within(bool async)
        {
            await base.Within(async);

            AssertSql(
"""
@__polygon_0='0x0000000001030000000100000005000000000000000000F0BF000000000000F0...' (DbType = Binary)

SELECT `p`.`Id`, ST_Within(`p`.`Point`, @__polygon_0) AS `Within`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task X(bool async)
        {
            await base.X(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_X(`p`.`Point`) AS `X`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task Y(bool async)
        {
            await base.Y(async);

            AssertSql(
"""
SELECT `p`.`Id`, ST_Y(`p`.`Point`) AS `Y`
FROM `PointEntity` AS `p`
""");
        }

        public override async Task XY_with_collection_join(bool async)
        {
            await base.XY_with_collection_join(async);

            AssertSql(
"""
SELECT `p1`.`Id`, `p1`.`c`, `p1`.`c0`, `p0`.`Id`, `p0`.`Geometry`, `p0`.`Group`, `p0`.`Point`, `p0`.`PointM`, `p0`.`PointZ`, `p0`.`PointZM`
FROM (
    SELECT `p`.`Id`, ST_X(`p`.`Point`) AS `c`, ST_Y(`p`.`Point`) AS `c0`
    FROM `PointEntity` AS `p`
    ORDER BY `p`.`Id`
    LIMIT 1
) AS `p1`
LEFT JOIN `PointEntity` AS `p0` ON `p1`.`Id` = `p0`.`Id`
ORDER BY `p1`.`Id`
""");
        }

        public override async Task IsEmpty_equal_to_null(bool async)
        {
            await base.IsEmpty_equal_to_null(async);

            AssertSql(
"""
SELECT `p`.`Id`
FROM `PointEntity` AS `p`
WHERE CASE
    WHEN `p`.`Point` IS NULL THEN NULL
    ELSE ST_IsEmpty(`p`.`Point`)
END IS NULL
""");
        }

        public override async Task IsEmpty_not_equal_to_null(bool async)
        {
            await base.IsEmpty_not_equal_to_null(async);

            AssertSql(
"""
SELECT `p`.`Id`
FROM `PointEntity` AS `p`
WHERE CASE
    WHEN `p`.`Point` IS NULL THEN NULL
    ELSE ST_IsEmpty(`p`.`Point`)
END IS NOT NULL
""");
        }

        public override async Task Intersects_equal_to_null(bool async)
        {
            await base.Intersects_equal_to_null(async);

            AssertSql(
"""
@__lineString_0='0x00000000010200000002000000000000000000E03F000000000000E0BF000000...' (DbType = Binary)

SELECT `l`.`Id`
FROM `LineStringEntity` AS `l`
WHERE ST_Intersects(`l`.`LineString`, @__lineString_0) IS NULL
""",
                //
                """
@__lineString_0='0x00000000010200000002000000000000000000E03F000000000000E0BF000000...' (DbType = Binary)

SELECT `l`.`Id`
FROM `LineStringEntity` AS `l`
WHERE ST_Intersects(@__lineString_0, `l`.`LineString`) IS NULL
""");
        }

        public override async Task Intersects_not_equal_to_null(bool async)
        {
            await base.Intersects_not_equal_to_null(async);

            AssertSql(
"""
@__lineString_0='0x00000000010200000002000000000000000000E03F000000000000E0BF000000...' (DbType = Binary)

SELECT `l`.`Id`
FROM `LineStringEntity` AS `l`
WHERE ST_Intersects(`l`.`LineString`, @__lineString_0) IS NOT NULL
""",
                //
                """
@__lineString_0='0x00000000010200000002000000000000000000E03F000000000000E0BF000000...' (DbType = Binary)

SELECT `l`.`Id`
FROM `LineStringEntity` AS `l`
WHERE ST_Intersects(@__lineString_0, `l`.`LineString`) IS NOT NULL
""");
        }

        #region Not supported by MySQL and MariaDB

        public override Task Buffer_quadrantSegments(bool async) => Task.CompletedTask;
        public override Task CoveredBy(bool async) => Task.CompletedTask; // Could be implemented using `MBRCoveredBy`
        public override Task Covers(bool async) => Task.CompletedTask;
        public override Task M(bool async) => Task.CompletedTask;
        public override Task Normalized(bool async) => Task.CompletedTask;
        public override Task Reverse(bool async) => Task.CompletedTask;
        public override Task Union_void(bool async) => Task.CompletedTask;
        public override Task Z(bool async) => Task.CompletedTask;

        #endregion

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
