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

            AssertSql();
        }

        public override async Task WithConversion(bool async)
        {
            await base.WithConversion(async);

            AssertSql();
        }

        public override async Task Area(bool async)
        {
            await base.Area(async);

            AssertSql();
        }

        public override async Task AsBinary(bool async)
        {
            await base.AsBinary(async);

            AssertSql();
        }

        public override async Task AsBinary_with_null_check(bool async)
        {
            await base.AsBinary_with_null_check(async);

            AssertSql();
        }

        public override async Task AsText(bool async)
        {
            await base.AsText(async);

            AssertSql();
        }

        public override async Task Buffer(bool async)
        {
            await base.Buffer(async);

            AssertSql();
        }

        public override async Task Centroid(bool async)
        {
            await base.Centroid(async);

            AssertSql();
        }

        public override async Task EnvelopeCombine_aggregate(bool async)
        {
            await base.EnvelopeCombine_aggregate(async);

            AssertSql();
        }

        public override async Task Contains(bool async)
        {
            await base.Contains(async);

            AssertSql();
        }

        public override async Task ConvexHull_aggregate(bool async)
        {
            await base.ConvexHull_aggregate(async);

            AssertSql();
        }

        public override async Task IGeometryCollection_Count(bool async)
        {
            await base.IGeometryCollection_Count(async);

            AssertSql();
        }

        public override async Task LineString_Count(bool async)
        {
            await base.LineString_Count(async);

            AssertSql();
        }

        public override async Task Crosses(bool async)
        {
            await base.Crosses(async);

            AssertSql();
        }

        public override async Task Difference(bool async)
        {
            await base.Difference(async);

            AssertSql();
        }

        public override async Task Dimension(bool async)
        {
            await base.Dimension(async);

            AssertSql();
        }

        public override async Task Disjoint_with_cast_to_nullable(bool async)
        {
            await base.Disjoint_with_cast_to_nullable(async);

            AssertSql();
        }

        public override async Task Disjoint_with_null_check(bool async)
        {
            await base.Disjoint_with_null_check(async);

            AssertSql();
        }

        public override async Task Distance_with_null_check(bool async)
        {
            await base.Distance_with_null_check(async);

            AssertSql();
        }

        public override async Task Distance_with_cast_to_nullable(bool async)
        {
            await base.Distance_with_cast_to_nullable(async);

            AssertSql();
        }

        public override async Task Distance_geometry(bool async)
        {
            await base.Distance_geometry(async);

            AssertSql();
        }

        public override async Task Distance_constant(bool async)
        {
            await base.Distance_constant(async);

            AssertSql();
        }

        public override async Task Distance_constant_lhs(bool async)
        {
            await base.Distance_constant_lhs(async);

            AssertSql();
        }

        public override async Task Distance_on_converted_geometry_type(bool async)
        {
            await base.Distance_on_converted_geometry_type(async);

            AssertSql();
        }

        public override async Task Distance_on_converted_geometry_type_lhs(bool async)
        {
            await base.Distance_on_converted_geometry_type_lhs(async);

            AssertSql();
        }

        public override async Task Distance_on_converted_geometry_type_constant(bool async)
        {
            await base.Distance_on_converted_geometry_type_constant(async);

            AssertSql();
        }

        public override async Task Distance_on_converted_geometry_type_constant_lhs(bool async)
        {
            await base.Distance_on_converted_geometry_type_constant_lhs(async);

            AssertSql();
        }

        public override async Task EndPoint(bool async)
        {
            await base.EndPoint(async);

            AssertSql();
        }

        public override async Task Envelope(bool async)
        {
            await base.Envelope(async);

            AssertSql();
        }

        public override async Task EqualsTopologically(bool async)
        {
            await base.EqualsTopologically(async);

            AssertSql();
        }

        public override async Task ExteriorRing(bool async)
        {
            await base.ExteriorRing(async);

            AssertSql();
        }

        public override async Task GetGeometryN(bool async)
        {
            await base.GetGeometryN(async);

            AssertSql();
        }

        public override async Task GetGeometryN_with_null_argument(bool async)
        {
            await base.GetGeometryN_with_null_argument(async);

            AssertSql();
        }

        public override async Task GetInteriorRingN(bool async)
        {
            await base.GetInteriorRingN(async);

            AssertSql();
        }

        public override async Task GetPointN(bool async)
        {
            await base.GetPointN(async);

            AssertSql();
        }

        public override async Task Intersection(bool async)
        {
            await base.Intersection(async);

            AssertSql();
        }

        public override async Task Intersects(bool async)
        {
            await base.Intersects(async);

            AssertSql();
        }

        public override async Task ICurve_IsClosed(bool async)
        {
            await base.ICurve_IsClosed(async);

            AssertSql();
        }

        public override async Task IMultiCurve_IsClosed(bool async)
        {
            await base.IMultiCurve_IsClosed(async);

            AssertSql();
        }

        public override async Task IsEmpty(bool async)
        {
            await base.IsEmpty(async);

            AssertSql();
        }

        public override async Task IsRing(bool async)
        {
            await base.IsRing(async);

            AssertSql();
        }

        public override async Task IsSimple(bool async)
        {
            await base.IsSimple(async);

            AssertSql();
        }

        public override async Task IsWithinDistance(bool async)
        {
            await base.IsWithinDistance(async);

            AssertSql();
        }

        public override async Task Item(bool async)
        {
            await base.Item(async);

            AssertSql();
        }

        public override async Task Length(bool async)
        {
            await base.Length(async);

            AssertSql();
        }

        public override async Task NumGeometries(bool async)
        {
            await base.NumGeometries(async);

            AssertSql();
        }

        public override async Task NumInteriorRings(bool async)
        {
            await base.NumInteriorRings(async);

            AssertSql();
        }

        public override async Task NumPoints(bool async)
        {
            await base.NumPoints(async);

            AssertSql();
        }

        public override async Task OgcGeometryType(bool async)
        {
            await base.OgcGeometryType(async);

            AssertSql();
        }

        public override async Task Overlaps(bool async)
        {
            await base.Overlaps(async);

            AssertSql();
        }

        public override async Task SRID(bool async)
        {
            await base.SRID(async);

            AssertSql();
        }

        public override async Task SRID_geometry(bool async)
        {
            await base.SRID_geometry(async);

            AssertSql();
        }

        public override async Task StartPoint(bool async)
        {
            await base.StartPoint(async);

            AssertSql();
        }

        public override async Task SymmetricDifference(bool async)
        {
            await base.SymmetricDifference(async);

            AssertSql();
        }

        public override async Task ToBinary(bool async)
        {
            await base.ToBinary(async);

            AssertSql();
        }

        public override async Task ToText(bool async)
        {
            await base.ToText(async);

            AssertSql();
        }

        public override async Task Touches(bool async)
        {
            await base.Touches(async);

            AssertSql();
        }

        public override async Task Union(bool async)
        {
            await base.Union(async);

            AssertSql();
        }

        public override async Task Union_aggregate(bool async)
        {
            await base.Union_aggregate(async);

            AssertSql();
        }

        public override async Task Within(bool async)
        {
            await base.Within(async);

            AssertSql();
        }

        public override async Task X(bool async)
        {
            await base.X(async);

            AssertSql();
        }

        public override async Task Y(bool async)
        {
            await base.Y(async);

            AssertSql();
        }

        public override async Task XY_with_collection_join(bool async)
        {
            await base.XY_with_collection_join(async);

            AssertSql();
        }

        public override async Task IsEmpty_equal_to_null(bool async)
        {
            await base.IsEmpty_equal_to_null(async);

            AssertSql();
        }

        public override async Task IsEmpty_not_equal_to_null(bool async)
        {
            await base.IsEmpty_not_equal_to_null(async);

            AssertSql();
        }

        public override async Task Intersects_equal_to_null(bool async)
        {
            await base.Intersects_equal_to_null(async);

            AssertSql();
        }

        public override async Task Intersects_not_equal_to_null(bool async)
        {
            await base.Intersects_not_equal_to_null(async);

            AssertSql();
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
