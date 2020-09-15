using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public abstract class JsonPocoChangeTrackingTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : JsonPocoChangeTrackingTestBase<TFixture>.JsonPocoChangeTrackingFixtureBase
    {
        protected JsonPocoChangeTrackingTestBase(JsonPocoChangeTrackingFixtureBase fixture)
        {
            Fixture = fixture;
        }

        protected JsonPocoChangeTrackingFixtureBase Fixture { get; }

        [Fact]
        public void Root_property_changed_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = JsonPocoChangeTrackingContext.CreateCustomer2();
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Root_property_unchanged_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_changed_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer.Age = 42;
            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_changed_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer.Age = 42;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_unchanged_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_changed_with_None_global()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None, true);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer.Age = 42;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = JsonPocoChangeTrackingContext.CreateCustomer1();
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = JsonPocoChangeTrackingContext.CreateCustomer1();
            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void SnapshotCallsClone()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.SnapshotCallsClone);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer.Age = 42;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(x.Customer.CloneCallCount >= 1);
            Assert.Equal(0, x.Customer.DeepCloneCallCount);
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void SnapshotCallsDeepClone()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.SnapshotCallsDeepClone);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer.Age = 42;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(x.Customer.DeepCloneCallCount >= 1);
            Assert.Equal(0, x.Customer.CloneCallCount);
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Toplevel_array_element_changed_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.ToplevelArray[2] = "fortytwo";
            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Toplevel_array_element_unchanged_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.ToplevelArray).IsModified);
        }

        [Fact]
        public void Toplevel_array_element_changed_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.ToplevelArray[2] = "fortytwo";
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.ToplevelArray).IsModified);
        }

        [Fact]
        public void Toplevel_array_element_unchanged_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.ToplevelArray).IsModified);
        }

        #region Support

        protected JsonPocoChangeTrackingContext CreateContext(
            MySqlJsonChangeTrackingOptions? changeTrackingOptions = null,
            bool isGlobal = false)
            => Fixture.CreateContext(
                serviceCollection => serviceCollection.Configure<JsonPocoChangeTrackingContext.JsonPocoChangeTrackingContextOptions>(
                    options =>
                    {
                        options.ChangeTrackingOptions = changeTrackingOptions;
                        options.AreChangeTrackingOptionsGlobal = isGlobal;
                    }),
                options => Fixture.AddOptions(options, changeTrackingOptions));

        public class JsonPocoChangeTrackingContext : PoolableDbContext
        {
            public IOptions<JsonPocoChangeTrackingContextOptions> ChangeTrackingOptions { get; }
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonPocoChangeTrackingContext(
                DbContextOptions<JsonPocoChangeTrackingContext> options,
                IOptions<JsonPocoChangeTrackingContextOptions> changeTrackingOptions = null)
                : base(options)
            {
                ChangeTrackingOptions = changeTrackingOptions;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (ChangeTrackingOptions?.Value.ChangeTrackingOptions != null &&
                    ChangeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    SetGlobalJsonChangeTrackingOptions(optionsBuilder, ChangeTrackingOptions.Value.ChangeTrackingOptions.Value);
                }
            }

            private void SetGlobalJsonChangeTrackingOptions(
                DbContextOptionsBuilder optionsBuilder,
                MySqlJsonChangeTrackingOptions jsonChangeTrackingOptions)
            {
                var mySqlJsonOptions = (MySqlJsonOptionsExtension) optionsBuilder.Options.Extensions.Last(e => e is MySqlJsonOptionsExtension);
                mySqlJsonOptions = mySqlJsonOptions.WithJsonChangeTrackingOptions(jsonChangeTrackingOptions);

                var addOrUpdateExtensionMethod = optionsBuilder.GetType()
                    .GetInterfaceMap(typeof(IDbContextOptionsBuilderInfrastructure))
                    .TargetMethods.First(
                        m => m.IsGenericMethod &&
                             m.ReturnType == typeof(void) &&
                             m.GetParameters()
                                 .Length == 1 &&
                             m.GetParameters()[0]
                                 .ParameterType.IsGenericParameter)
                    .MakeGenericMethod(mySqlJsonOptions.GetType());

                addOrUpdateExtensionMethod.Invoke(optionsBuilder, new object[] {mySqlJsonOptions});
            }

            public void Seed()
            {
                JsonEntities.AddRange(
                    new JsonEntity { Id = 1, Customer = CreateCustomer1(), ToplevelArray = new[] { "one", "two", "three" } },
                    new JsonEntity { Id = 2, Customer = CreateCustomer2() });
                SaveChanges();
            }

            public static Customer CreateCustomer1()
                => new Customer
                {
                    Name = "Joe",
                    Age = 25,
                    ID = Guid.Empty,
                    IsVip = false,
                    Statistics = new Statistics {Visits = 4, Purchases = 3, Nested = new NestedStatistics {SomeProperty = 10, SomeNullableInt = 20, SomeNullableGuid = Guid.Parse("d5f2685d-e5c4-47e5-97aa-d0266154eb2d"), IntArray = new[] {3, 4}}},
                    Orders = new[] {new Order {Price = 99.5m, ShippingAddress = "Some address 1", ShippingDate = new DateTime(2019, 10, 1)}, new Order {Price = 23.1m, ShippingAddress = "Some address 2", ShippingDate = new DateTime(2019, 10, 10)}}
                };

            public static Customer CreateCustomer2()
                => new Customer
                {
                    Name = "Moe",
                    Age = 35,
                    ID = Guid.Parse("3272b593-bfe2-4ecf-81ae-4242b0632465"),
                    IsVip = true,
                    Statistics = new Statistics {Visits = 20, Purchases = 25, Nested = new NestedStatistics {SomeProperty = 20, SomeNullableInt = null, SomeNullableGuid = null, IntArray = new[] {5, 6}}},
                    Orders = new[] {new Order {Price = 5, ShippingAddress = "Moe's address", ShippingDate = new DateTime(2019, 11, 3)}}
                };

            public class JsonPocoChangeTrackingContextOptions
            {
                public MySqlJsonChangeTrackingOptions? ChangeTrackingOptions { get; set; }
                public bool AreChangeTrackingOptionsGlobal { get; set; }
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            [Column(TypeName = "json")]
            public Customer Customer { get; set; }

            [Column(TypeName = "json")]
            public string[] ToplevelArray { get; set; }
        }

        public class JsonPocoChangeTrackingFixtureBase : ServiceProviderPerContextFixtureBase<JsonPocoChangeTrackingContext>
        {
            protected override string StoreName => "JsonPocoChangeTrackingTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            protected override void Seed(JsonPocoChangeTrackingContext context) => context.Seed();

            public virtual DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder, MySqlJsonChangeTrackingOptions? changeTrackingOptions)
                => builder;
        }

        public class Customer
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Guid ID { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("is_vip")]
            [Newtonsoft.Json.JsonProperty("is_vip")]
            public bool IsVip { get; set; }
            public Statistics Statistics { get; set; }
            public Order[] Orders { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [Newtonsoft.Json.JsonIgnore]
            public int DeepCloneCallCount { get; private set; }

            public Customer DeepClone()
            {
                DeepCloneCallCount++;

                // Good enough clone for tests (but not for real apps, because it does not clone the whole hierarchy.
                return new Customer
                {
                    Name = Name,
                    Age = Age,
                    ID = ID,
                    IsVip = IsVip,
                    Statistics = Statistics,
                    Orders = Orders,
                };
            }

            [System.Text.Json.Serialization.JsonIgnore]
            [Newtonsoft.Json.JsonIgnore]
            public int CloneCallCount { get; private set; }

            public Customer Clone()
            {
                CloneCallCount++;

                // Good enough clone for tests (but not for real apps, because it does not clone the whole hierarchy.
                return new Customer
                {
                    Name = Name,
                    Age = Age,
                    ID = ID,
                    IsVip = IsVip,
                    Statistics = Statistics,
                    Orders = Orders,
                };
            }
        }

        public class Statistics
        {
            public long Visits { get; set; }
            public int Purchases { get; set; }
            public NestedStatistics Nested { get; set; }
        }

        public class NestedStatistics
        {
            public int SomeProperty { get; set; }
            public int? SomeNullableInt { get; set; }
            public int[] IntArray { get; set; }
            public Guid? SomeNullableGuid { get; set; }
        }

        public class Order
        {
            public decimal Price { get; set; }
            public string ShippingAddress { get; set; }
            public DateTime ShippingDate { get; set; }
        }

        #endregion
    }
}
