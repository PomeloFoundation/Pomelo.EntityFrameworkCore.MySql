using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SpatialGeographyQueryMySqlTest : QueryTestBase<SpatialGeographyQueryMySqlTest.SpatialGeographyQueryMySqlFixture>
    {
        public SpatialGeographyQueryMySqlTest(SpatialGeographyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact]
        public virtual void Distance()
        {
            const double expectedGreatCircleDistance = 8117916.968066435; // result of ST_Distance_Sphere() for SRID 4326

            using var context = CreateContext();

            var seattle = context.Cities.First(c => c.Name == "Seattle");
            var cities = context.Cities
                .Where(c => c.Name == "Berlin")
                .Select(c => new {Distance = c.Location.Distance(seattle.Location)})
                .ToList();

            var deviation = Math.Abs(cities.Single().Distance - expectedGreatCircleDistance) / expectedGreatCircleDistance;
            Assert.True(deviation < 0.01);
        }

        [ConditionalFact]
        public virtual void IsWithinDistance()
        {
            const double expectedGreatCircleDistance = 8117916.968066435 + 100_000; // +100 km

            using var context = CreateContext();

            var seattle = context.Cities.First(c => c.Name == "Seattle");
            var cities = context.Cities
                .Where(c => c.CityId != seattle.CityId &&
                            c.Location.IsWithinDistance(seattle.Location, expectedGreatCircleDistance))
                .ToList();

            Assert.Single(cities);
            Assert.Equal("Berlin", cities[0].Name);
        }

        [ConditionalFact]
        public virtual void SpatialDistancePlanar()
        {
            const double expectedPlanarDistance = 135.82593753560775; // result of ST_Distance() for SRID 0

            using var context = CreateContext();

            var seattle = context.Cities.First(c => c.Name == "Seattle");
            var cities = context.Cities
                .Where(c => c.Name == "Berlin")
                .Select(c => new
                {
                    Distance = EF.Functions.SpatialDistancePlanar(c.Location, seattle.Location),
                })
                .ToList();

            var deviation = Math.Abs(cities.Single().Distance - expectedPlanarDistance) / expectedPlanarDistance;
            Assert.True(deviation < 0.01);
        }

        [ConditionalFact]
        public virtual void SpatialDistanceSphere_Native()
        {
            const double expectedGreatCircleDistance = 8117916.968066435; // result of ST_Distance_Sphere() for SRID 4326

            using var context = CreateContext();

            var seattle = context.Cities.First(c => c.Name == "Seattle");
            var cities = context.Cities
                .Where(c => c.Name == "Berlin")
                .Select(c => new
                {
                    Distance = c.Location.Distance(seattle.Location),
                    DistanceNative = EF.Functions.SpatialDistanceSphere(c.Location, seattle.Location, SpatialDistanceAlgorithm.Native)
                })
                .ToList();

            var deviation = Math.Abs(cities.Single().DistanceNative - expectedGreatCircleDistance) / expectedGreatCircleDistance;
            Assert.Equal(cities.Single().Distance, cities.Single().DistanceNative);
            Assert.True(deviation < 0.01);
        }

        [ConditionalFact]
        public virtual void SpatialDistanceSphere_Andoyer()
        {
            const double expectedGreatCircleDistance = 8117916.968066435; // result of ST_Distance_Sphere() for SRID 4326

            using var context = CreateContext();

            var seattle = context.Cities.First(c => c.Name == "Seattle");
            var cities = context.Cities
                .Where(c => c.Name == "Berlin")
                .Select(c => new
                {
                    Distance = EF.Functions.SpatialDistanceSphere(c.Location, seattle.Location, SpatialDistanceAlgorithm.Andoyer),
                })
                .ToList();

            var deviation = Math.Abs(cities.Single().Distance - expectedGreatCircleDistance) / expectedGreatCircleDistance;
            Assert.True(deviation < 0.01);
        }

        [ConditionalFact]
        public virtual void SpatialDistanceSphere_Haversine()
        {
            const double expectedGreatCircleDistance = 8117916.968066435; // result of ST_Distance_Sphere() for SRID 4326

            using var context = CreateContext();

            var seattle = context.Cities.First(c => c.Name == "Seattle");
            var cities = context.Cities
                .Where(c => c.Name == "Berlin")
                .Select(c => new
                {
                    Distance = EF.Functions.SpatialDistanceSphere(c.Location, seattle.Location, SpatialDistanceAlgorithm.Haversine),
                })
                .ToList();

            var deviation = Math.Abs(cities.Single().Distance - expectedGreatCircleDistance) / expectedGreatCircleDistance;
            Assert.True(deviation < 0.01);
        }

        private SpatialGeographyContext CreateContext() => Fixture.CreateContext();

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class SpatialGeographyContext : PoolableDbContext
        {
            public DbSet<City> Cities { get; set; }

            public SpatialGeographyContext(DbContextOptions options)
                : base(options)
            {
            }

            public static void Seed(SpatialGeographyContext context, GeometryFactory factory)
            {
                context.AddRange(SpatialGeographyData.CreateCities(factory));
                context.SaveChanges();
            }

            public class City
            {
                public int CityId { get; set; }
                public string Name { get; set; }
                public Point Location { get; set; }
            }
        }

        public class SpatialGeographyData : ISetSource
        {
            private readonly IReadOnlyList<SpatialGeographyContext.City> _cities;

            public SpatialGeographyData(GeometryFactory factory)
            {
                _cities = CreateCities(factory);
            }

            public static IReadOnlyList<SpatialGeographyContext.City> CreateCities(GeometryFactory factory)
            {
                return new []
                {
                    new SpatialGeographyContext.City {CityId = 1, Name = "Berlin", Location = factory.CreatePoint(new Coordinate(13.404954, 52.520007))},
                    new SpatialGeographyContext.City {CityId = 2, Name = "Seattle", Location = factory.CreatePoint(new Coordinate(-122.3320708, 47.6062095))},
                    new SpatialGeographyContext.City {CityId = 3, Name = "Warsaw", Location = factory.CreatePoint(new Coordinate(21.012229, 52.229676))}
                };
            }

            public virtual IQueryable<TEntity> Set<TEntity>()
                where TEntity : class
            {
                if (typeof(TEntity) == typeof(SpatialGeographyContext.City))
                {
                    return (IQueryable<TEntity>)_cities.AsQueryable();
                }

                throw new InvalidOperationException("Unknown entity type: " + typeof(TEntity));
            }
        }

        public class SpatialGeographyQueryMySqlFixture : SharedStoreFixtureBase<SpatialGeographyContext>, IQueryFixtureBase
        {
            private GeometryFactory _geometryFactory;

            public new RelationalTestStore TestStore
                => (RelationalTestStore)base.TestStore;

            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            public virtual GeometryFactory GeometryFactory
                => LazyInitializer.EnsureInitialized(
                    ref _geometryFactory,
                    () => NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

            protected override string StoreName
                => "SpatialGeographyQueryTest";

            protected override bool ShouldLogCategory(string logCategory)
                => logCategory == DbLoggerCategory.Query.Name;

            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                => base.AddServices(serviceCollection)
                    .AddEntityFrameworkMySqlNetTopologySuite();

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var optionsBuilder = base.AddOptions(builder);
                new MySqlDbContextOptionsBuilder(optionsBuilder)
                    .UseNetTopologySuite();

                return optionsBuilder;
            }

            public override SpatialGeographyContext CreateContext()
            {
                var context = base.CreateContext();
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                return context;
            }

            protected override void Seed(SpatialGeographyContext context)
                => SpatialGeographyContext.Seed(context, GeometryFactory);

            public Func<DbContext> GetContextCreator()
                => CreateContext;

            // CHECK: Unused?
            public ISetSource GetExpectedData()
                => new SpatialGeographyData(_geometryFactory);

            // CHECK: Unused?
            public IReadOnlyDictionary<Type, object> GetEntitySorters()
                => new Dictionary<Type, Func<object, object>>
                {
                    { typeof(SpatialGeographyContext.City), e => ((SpatialGeographyContext.City)e)?.CityId },
                }.ToDictionary(e => e.Key, e => (object)e.Value);

            // CHECK: Unused?
            public IReadOnlyDictionary<Type, object> GetEntityAsserters()
                => new Dictionary<Type, Action<object, object>>
                {
                    {
                        typeof(SpatialGeographyContext.City), (e, a) =>
                        {
                            Assert.Equal(e == null, a == null);

                            if (a != null)
                            {
                                var ee = (SpatialGeographyContext.City)e;
                                var aa = (SpatialGeographyContext.City)a;

                                Assert.Equal(ee.CityId, aa.CityId);
                                Assert.Equal(ee.Name, aa.Name);
                                Assert.Equal(ee.Location, aa.Location);
                            }
                        }
                    },
                }.ToDictionary(e => e.Key, e => (object)e.Value);
        }
    }
}
