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
    [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.SpatialSetSridFunction))]
    public class SpatialBehavior : RawSqlTestWithFixture<SpatialBehavior.SpatialBehaviorFixture>
    {
        public SpatialBehavior(SpatialBehaviorFixture fixture, ITestOutputHelper testOutputHelper)
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
        public void MySql7_and_MariaDb_use_x_longitude_and_y_latitude_order()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = @"
SELECT
ST_X(`Location`) as `X`,
ST_Y(`Location`) as `Y`
FROM `IceCreamShops`;";

            using var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var x = (double)dataReader["X"];
                var y = (double)dataReader["Y"];

                // X and Y coordinates should be in the same order in the database that they are used in NTS, because MySQL < 8 and MariaDB
                // do not really support any SRIDs and treat everything as SRID 0.
                // Longitude is stored as X and Latitude is stored as Y.
                Assert.Equal(13.3777041, x); // lon
                Assert.Equal(52.5162746, y); // lat
            }
        }

        public static class Model
        {
            public class IceCreamShop
            {
                public int IceCreamShopId { get; set; }
                public string Name { get; set; }
                public Point Location { get; set; }
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
                            // This has no effect on MySQL < 8 and MariaDB, because MySQL < 8 and MariaDB do not support SRID constraints.
                            //
                            // entity.Property(e => e.Location)
                            //     .HasSpatialReferenceSystem(srid);

                            entity.HasData(
                                new IceCreamShop
                                {
                                    IceCreamShopId = 1,
                                    Name = "Cold & Sweet",

                                    // Brandenburg Gate, Berlin, Germany:
                                    // lon=13.3777041, lat=52.5162746
                                    // or @52.516377,13.3776013 as Google Maps outputs them in @lat,lon order)
                                    Location = factory.CreatePoint(new Coordinate( /* lon */ x: 13.3777041, /* lat */ y: 52.5162746))
                                });
                        });
                }
            }
        }

        public class SpatialBehaviorFixture : MySqlTestFixtureBase<Model.SpatialBehaviorContext>
        {
            // Add a shop directly through SQL.
            protected override string SetupDatabaseScript
                => @"
insert into `IceCreamShops` (`IceCreamShopId`, `Name`, `Location`) values (2, 'Cold & Sweet', ST_GeomFromText('POINT(13.3777041 52.5162746)', 4326)); -- lon lat
insert into `IceCreamShops` (`IceCreamShopId`, `Name`, `Location`) values (3, 'Cold & Sweet', ST_GeomFromText('POINT(13.3777041 52.5162746)', 0)); -- lon lat
insert into `IceCreamShops` (`IceCreamShopId`, `Name`, `Location`) values (4, 'Cold & Sweet', ST_GeomFromText('POINT(13.3777041 52.5162746)')); -- lon lat
insert into `IceCreamShops` (`IceCreamShopId`, `Name`, `Location`) values (5, 'Cold & Sweet', Point(13.3777041, 52.5162746)); -- lon lat";

            public override DbContext CreateDefaultDbContext()
                => CreateContext(
                    mySqlOptions: builder => builder.UseNetTopologySuite(),
                    serviceCollection: collection => collection.AddEntityFrameworkMySqlNetTopologySuite());
        }
    }
}
