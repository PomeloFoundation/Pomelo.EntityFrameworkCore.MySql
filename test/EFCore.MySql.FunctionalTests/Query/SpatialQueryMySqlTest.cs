using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
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

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
