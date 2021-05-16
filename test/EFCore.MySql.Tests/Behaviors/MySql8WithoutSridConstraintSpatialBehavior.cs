using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.Behaviors
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialSetSridFunction))]
    public class MySql8WithoutSridConstraintSpatialBehavior : RawSqlTestWithFixture<MySql8WithoutSridConstraintSpatialBehavior.MySql8WithoutSridConstraintSpatialBehaviorFixture>
    {
        public MySql8WithoutSridConstraintSpatialBehavior(MySql8WithoutSridConstraintSpatialBehaviorFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
        }

        [Fact]
        public void NetTopologySuite_uses_x_longitude_and_y_latitude_order()
        {
            var iceCreamShops = Context.Set<Model.IceCreamShop>().ToList();

            Assert.All(iceCreamShops, s => Assert.Equal(13.3777041, s.Location.X));
            Assert.All(iceCreamShops, s => Assert.Equal(52.5162746, s.Location.Y));
        }

        [Fact]
        public void MySql8_longitude_latitude_order_depends_on_srid()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = @"
SELECT
`Srid`,
ST_X(`Location`) as `X`,
ST_Y(`Location`) as `Y`,
case `Srid` when 4326 then ST_Longitude(`Location`) else null end as `Lon`,
case `Srid` when 4326 then ST_Latitude(`Location`) else null end as `Lat`
FROM `IceCreamShops`;";

            using var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var srid = dataReader["Srid"] as int?;
                var x = (double)dataReader["X"];
                var y = (double)dataReader["Y"];
                var lon = dataReader["Lon"] as double?;
                var lat = dataReader["Lat"] as double?;

                if (srid == 4326)
                {
                    // X and Y coordinates should be swapped in the database in comparison to NTS, because MySQL expects X to be latitude
                    // and Y to be longitude for SRID 4326.
                    Assert.Equal(52.5162746, x);
                    Assert.Equal(52.5162746, lat);

                    Assert.Equal(13.3777041, y);
                    Assert.Equal(13.3777041, lon);
                }
                else
                {
                    // X and Y coordinates should be in the same order in the database that they are used in NTS.
                    // Longitude is stored as X and latitude is stored as Y.
                    Assert.Equal(13.3777041, x); // lon
                    Assert.Equal(52.5162746, y); // lat
                }
            }
        }

        public static class Model
        {
            public class IceCreamShop
            {
                public int IceCreamShopId { get; set; }
                public string Name { get; set; }
                public Point Location { get; set; }
                public int? Srid { get; set; }
            }

            public class SpatialBehaviorContext : ContextBase
            {
                public DbSet<IceCreamShop> IceCreamShops { get; set; }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    var srid = 4326;
                    var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid);

                    // Add a shop through NTS and Pomelo.
                    modelBuilder.Entity<IceCreamShop>(
                        entity =>
                        {
                            // entity.Property(e => e.Location)
                            //     .HasSpatialReferenceSystem(srid);

                            entity.HasData(
                                new IceCreamShop
                                {
                                    IceCreamShopId = 1,
                                    Name = "Cold & Sweet",
                                    Srid = 4326,

                                    // Brandenburg Gate, Berlin, Germany:
                                    // lon=13.3777041, lat=52.5162746
                                    // or @52.516377,13.3776013 as Google Maps outputs them in @lat,lon order)
                                    Location = factory.CreatePoint(new Coordinate( /* lon */ x: 13.3777041, /* lat */ y: 52.5162746))
                                });
                        });
                }
            }
        }

        public class MySql8WithoutSridConstraintSpatialBehaviorFixture : MySqlTestFixtureBase<Model.SpatialBehaviorContext>
        {
            // Add a shop directly through SQL.
            protected override string SetupDatabaseScript
                => @"
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', 4326, ST_GeomFromText('POINT(52.5162746 13.3777041)', 4326)); -- LAT LON
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', 0, ST_GeomFromText('POINT(13.3777041 52.5162746)', 0)); -- lon lat
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', null, ST_GeomFromText('POINT(13.3777041 52.5162746)')); -- lon lat
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', null, Point(13.3777041, 52.5162746)); -- lon lat
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', 4326, ST_SRID(ST_GeomFromText('POINT(13.3777041 52.5162746)'), 4326)); -- lon lat
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', 0, ST_SRID(ST_GeomFromText('POINT(13.3777041 52.5162746)'), 0)); -- lon lat
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', 4326, ST_SRID(Point(13.3777041, 52.5162746), 4326)); -- lon lat
insert into `IceCreamShops` (`Name`, `Srid`, `Location`) values ('Cold & Sweet', 0, ST_SRID(Point(13.3777041, 52.5162746), 0)); -- lon lat
";

            public override DbContext CreateDefaultDbContext()
                => CreateContext(
                    mySqlOptions: builder => builder.UseNetTopologySuite(),
                    serviceCollection: collection => collection.AddEntityFrameworkMySqlNetTopologySuite());
        }
    }
}
