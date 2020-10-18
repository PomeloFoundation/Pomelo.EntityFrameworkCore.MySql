using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SpatialMySqlTest : SpatialTestBase<SpatialMySqlFixture>
    {
        public SpatialMySqlTest(SpatialMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        [ConditionalFact]
        public override void Can_roundtrip_Z_and_M()
        {
            using var db = Fixture.CreateContext();
            var entity = db.Set<PointEntity>()
                .FirstOrDefault(e => e.Id == PointEntity.WellKnownId);

            Assert.NotNull(entity);
            Assert.NotNull(entity.Point);
            Assert.True(double.IsNaN(entity.Point.Z));
            Assert.True(double.IsNaN(entity.Point.M));
            Assert.True(double.IsNaN(entity.PointZ.Z));
            Assert.True(double.IsNaN(entity.PointZ.M));
            Assert.True(double.IsNaN(entity.PointM.Z));
            Assert.True(double.IsNaN(entity.PointM.M));
            Assert.True(double.IsNaN(entity.PointZM.Z));
            Assert.True(double.IsNaN(entity.PointZM.M));
        }

        [ConditionalFact(Skip = "Point.Empty is currently not supported by MySQL and MariaDB.")]
        public override void Translators_handle_static_members()
            => base.Translators_handle_static_members();
    }
}
