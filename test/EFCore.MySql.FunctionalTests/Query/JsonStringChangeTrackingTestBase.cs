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
    public abstract class JsonStringChangeTrackingTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : JsonStringChangeTrackingTestBase<TFixture>.JsonStringChangeTrackingFixtureBase
    {
        protected JsonStringChangeTrackingTestBase(JsonStringChangeTrackingFixtureBase fixture)
        {
            Fixture = fixture;
        }

        protected JsonStringChangeTrackingFixtureBase Fixture { get; }

        [Fact]
        public void Root_property_changed_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = JsonStringChangeTrackingContext.Customer2;
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

            x.Customer = x.Customer.Replace("25", "42");
            ctx.ChangeTracker.DetectChanges();

            Assert.Matches(@"""Age""\s*:\s*42", x.Customer);
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_whitespace_changed_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = x.Customer.Replace("25,", "25 ,");
            ctx.ChangeTracker.DetectChanges();

            Assert.Matches(@"""Age""\s*:\s*25 ,", x.Customer);
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_whitespace_changed_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var original = x.Customer;
            x.Customer = x.Customer.Replace("25,", "25 ,");
            ctx.ChangeTracker.DetectChanges();

            Assert.Matches(@"""Age""\s*:\s*25 ,", x.Customer);
            Assert.False(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Inner_property_changed_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = x.Customer.Replace("25", "42");
            ctx.ChangeTracker.DetectChanges();

            Assert.Matches(@"""Age""\s*:\s*42", x.Customer);
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

            x.Customer = x.Customer.Replace("25", "42");
            ctx.ChangeTracker.DetectChanges();

            Assert.Matches(@"""Age""\s*:\s*42", x.Customer);
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareRootPropertyOnly()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomer = x.Customer;
            var newCustomer = new string(JsonStringChangeTrackingContext.Customer1);
            x.Customer = newCustomer;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomer, newCustomer));
            Assert.True(Equals(x.Customer, newCustomer));
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareStringRootPropertyByEquals()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareStringRootPropertyByEquals);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomer = x.Customer;
            var newCustomer = new string(JsonStringChangeTrackingContext.Customer1);
            x.Customer = newCustomer;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomer, newCustomer));
            Assert.True(Equals(x.Customer, newCustomer));
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_None()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareStringRootPropertyByEquals);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomer = x.Customer;
            var newCustomer = new string(JsonStringChangeTrackingContext.Customer1);
            x.Customer = newCustomer;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomer, newCustomer));
            Assert.True(Equals(x.Customer, newCustomer));
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        [Fact]
        public void SnapshotCallsClone()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.SnapshotCallsClone);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.Customer = x.Customer.Replace("25", "42");
            ctx.ChangeTracker.DetectChanges();

            Assert.Matches(@"""Age""\s*:\s*42", x.Customer);
            Assert.True(ctx.Entry(x).Property(e => e.Customer).IsModified);
        }

        #region Support

        protected JsonStringChangeTrackingContext CreateContext(
            MySqlJsonChangeTrackingOptions? changeTrackingOptions = null,
            bool isGlobal = false)
            => Fixture.CreateContext(
                serviceCollection => serviceCollection.Configure<JsonStringChangeTrackingContext.JsonPocoChangeTrackingContextOptions>(
                    options =>
                    {
                        options.ChangeTrackingOptions = changeTrackingOptions;
                        options.AreChangeTrackingOptionsGlobal = isGlobal;
                    }),
                options => Fixture.AddOptions(options, changeTrackingOptions));

        public class JsonStringChangeTrackingContext : PoolableDbContext
        {
            public const string Customer1 = @"
{
    ""Name"": ""Joe"",
    ""Age"": 25,
    ""IsVip"": false,
    ""Statistics"":
    {
        ""Visits"": 4,
        ""Purchases"": 3,
        ""Nested"":
        {
            ""SomeProperty"": 10,
            ""IntArray"": [3, 4]
        }
    },
    ""Orders"":
    [
        {
            ""Price"": 99.5,
            ""ShippingAddress"": ""Some address 1"",
            ""ShippingDate"": ""2019-10-01""
        },
        {
            ""Price"": 23,
            ""ShippingAddress"": ""Some address 2"",
            ""ShippingDate"": ""2019-10-10""
        }
    ]
}";

            public const string Customer2 = @"
{
    ""Name"": ""Moe"",
    ""Age"": 35,
    ""IsVip"": true,
    ""Statistics"":
    {
        ""Visits"": 20,
        ""Purchases"": 25,
        ""Nested"":
        {
            ""SomeProperty"": 20,
            ""IntArray"": [5, 6]
        }
    },
    ""Orders"":
    [
        {
            ""Price"": 5,
            ""ShippingAddress"": ""Moe's address"",
            ""ShippingDate"": ""2019-11-03""
        }
    ]
}";

            public IOptions<JsonPocoChangeTrackingContextOptions> ChangeTrackingOptions { get; }
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonStringChangeTrackingContext(
                DbContextOptions<JsonStringChangeTrackingContext> options,
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
                             m.GetParameters().Length == 1 &&
                             m.GetParameters()[0].ParameterType.IsGenericParameter)
                    .MakeGenericMethod(mySqlJsonOptions.GetType());

                addOrUpdateExtensionMethod.Invoke(optionsBuilder, new object[] {mySqlJsonOptions});
            }

            public void Seed()
            {
                JsonEntities.AddRange(
                    new JsonEntity {Id = 1, Customer = Customer1},
                    new JsonEntity {Id = 2, Customer = Customer2});
                SaveChanges();
            }

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
            public string Customer { get; set; }
        }

        public class JsonStringChangeTrackingFixtureBase : ServiceProviderPerContextFixtureBase<JsonStringChangeTrackingContext>
        {
            protected override string StoreName => "JsonStringChangeTrackingTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            protected override void Seed(JsonStringChangeTrackingContext context) => context.Seed();

            public virtual DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder, MySqlJsonChangeTrackingOptions? changeTrackingOptions)
                => builder;
        }

        #endregion
    }
}
