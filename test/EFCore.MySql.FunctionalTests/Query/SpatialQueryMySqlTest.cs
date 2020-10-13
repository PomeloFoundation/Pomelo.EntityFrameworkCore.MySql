using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SpatialQueryMySqlTest : SpatialQueryTestBase<SpatialQueryMySqlFixture>
    {
        public SpatialQueryMySqlTest(SpatialQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionTheory(ServerVersion.SpatialBoundaryFunctionSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Boundary(bool async)
            => base.Boundary(async);

        [SupportedServerVersionTheory(ServerVersion.SpatialFunctionAdditionsSupportKey, Skip = "MySQL is unable to work with different SRIDs.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Distance_constant_srid_4326(bool async)
            => base.Distance_constant_srid_4326(async);

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task GeometryType(bool async)
            => AssertQuery(
                async,
                ss => ss.Set<PointEntity>().Select(
                    e => new { e.Id, GeometryType = e.Point == null ? null : e.Point.GeometryType.ToLower() }),
                elementSorter: x => x.Id);

        [SupportedServerVersionTheory(ServerVersion.SpatialPointOnSurfaceFunctionSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task InteriorPoint(bool async)
            => base.InteriorPoint(async);

        [SupportedServerVersionTheory(ServerVersion.SpatialIsValidFunctionSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task IsValid(bool async)
            => base.IsValid(async);

        [SupportedServerVersionTheory(ServerVersion.SpatialPointOnSurfaceFunctionSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task PointOnSurface(bool async)
            => base.PointOnSurface(async);

        [SupportedServerVersionTheory(ServerVersion.SpatialRelateFunctionSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Relate(bool async)
            => base.Relate(async);

        [ConditionalTheory(Skip = "The M coordinate is currently not supported by MySQL and MariaDB.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task M(bool async)
            => base.M(async);

        [ConditionalTheory(Skip = "The Z coordinate is currently not supported by MySQL and MariaDB.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Z(bool async)
            => base.Z(async);

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
