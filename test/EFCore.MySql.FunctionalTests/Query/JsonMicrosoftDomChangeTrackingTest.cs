using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Extensions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonMicrosoftDomChangeTrackingTest : IClassFixture<JsonMicrosoftDomChangeTrackingTest.JsonMicrosoftDomChangeTrackingFixture>
    {
        public JsonMicrosoftDomChangeTrackingTest(JsonMicrosoftDomChangeTrackingFixture fixture)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected JsonMicrosoftDomChangeTrackingFixture Fixture { get; }

        [Fact]
        public void Root_property_changed_JsonDocument()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerDocument = JsonMicrosoftDomChangeTrackingContext.CreateCustomer2();
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.CustomerDocument).IsModified);
        }

        [Fact]
        public void Root_property_changed_JsonElement()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerElement = JsonMicrosoftDomChangeTrackingContext.CreateCustomer2().RootElement;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.CustomerElement).IsModified);
        }

        [Fact]
        public void Root_property_unchanged_JsonDocument()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerDocument).IsModified);
        }

        [Fact]
        public void Root_property_unchanged_JsonElement()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerElement).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareRootPropertyOnly_JsonDocument()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomerDocument = x.CustomerDocument;
            var newCustomerDocument = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1();
            x.CustomerDocument = newCustomerDocument;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomerDocument, newCustomerDocument));
            Assert.True(ctx.Entry(x).Property(e => e.CustomerDocument).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareRootPropertyOnly_JsonElement()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerElement = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1().RootElement;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.CustomerElement).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareDomSemantically_JsonDocument()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareDomSemantically);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomerDocument = x.CustomerDocument;
            var newCustomerDocument = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1();
            x.CustomerDocument = newCustomerDocument;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomerDocument, newCustomerDocument));
            Assert.False(ctx.Entry(x).Property(e => e.CustomerDocument).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareDomSemantically_JsonElement()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.CompareDomSemantically);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerElement = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1().RootElement;
            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerElement).IsModified);
        }

        [Fact]
        public void HashDomSemantially_JsonDocument()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.HashDomSemantially);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerDocument).IsModified);
        }

        [Fact]
        public void HashDomSemantially_JsonElement()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.HashDomSemantially);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerElement).IsModified);
        }

        [Fact]
        public void HashDomSemantiallyOptimized_JsonDocument()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.HashDomSemantiallyOptimized);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerDocument).IsModified);
        }

        [Fact]
        public void HashDomSemantiallyOptimized_JsonElement()
        {
            using var ctx = CreateContext(MySqlJsonChangeTrackingOptions.HashDomSemantiallyOptimized);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerElement).IsModified);
        }

        #region Support

        protected JsonMicrosoftDomChangeTrackingContext CreateContext(
            MySqlJsonChangeTrackingOptions? changeTrackingOptions = null,
            bool isGlobal = false)
            => Fixture.CreateContext(
                serviceCollection => serviceCollection.Configure<JsonMicrosoftDomChangeTrackingContext.JsonPocoChangeTrackingContextOptions>(
                    options =>
                    {
                        options.ChangeTrackingOptions = changeTrackingOptions;
                        options.AreChangeTrackingOptionsGlobal = isGlobal;
                    })
            );

        public class JsonMicrosoftDomChangeTrackingContext : PoolableDbContext
        {
            private readonly IOptions<JsonPocoChangeTrackingContextOptions> _changeTrackingOptions;
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonMicrosoftDomChangeTrackingContext(
                DbContextOptions<JsonMicrosoftDomChangeTrackingContext> options,
                IOptions<JsonPocoChangeTrackingContextOptions> changeTrackingOptions = null)
                : base(options)
            {
                _changeTrackingOptions = changeTrackingOptions;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (_changeTrackingOptions?.Value.ChangeTrackingOptions != null &&
                    _changeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension((MySqlJsonMicrosoftOptionsExtension)
                        (optionsBuilder.Options.FindExtension<MySqlJsonMicrosoftOptionsExtension>() ??
                         new MySqlJsonMicrosoftOptionsExtension())
                            .WithJsonChangeTrackingOptions(_changeTrackingOptions.Value.ChangeTrackingOptions.Value));
                }
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                if (_changeTrackingOptions != null &&
                    !_changeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    modelBuilder.Entity<JsonEntity>(
                        entity =>
                        {
                            entity.Property(e => e.CustomerDocument)
                                .UseJsonChangeTrackingOptions(_changeTrackingOptions.Value.ChangeTrackingOptions);
                            entity.Property(e => e.CustomerElement)
                                .UseJsonChangeTrackingOptions(_changeTrackingOptions.Value.ChangeTrackingOptions);
                        });
                }
            }

            public void Seed()
            {
                var (customer1, customer2, customer3) = (CreateCustomer1(), CreateCustomer2(), CreateCustomer3());

                JsonEntities.AddRange(
                    new JsonEntity { Id = 1, CustomerDocument = customer1, CustomerElement = customer1.RootElement },
                    new JsonEntity { Id = 2, CustomerDocument = customer2, CustomerElement = customer2.RootElement },
                    new JsonEntity { Id = 3, CustomerDocument = customer3, CustomerElement = customer3.RootElement });
                SaveChanges();
            }

            public static JsonDocument CreateCustomer1()
                => JsonDocument.Parse(
                    @"
                {
                    ""Name"": ""Joe"",
                    ""Age"": 25,
                    ""ID"": ""00000000-0000-0000-0000-000000000000"",
                    ""IsVip"": false,
                    ""Statistics"":
                    {
                        ""Visits"": 4,
                        ""Purchases"": 3,
                        ""Nested"":
                        {
                            ""SomeProperty"": 10,
                            ""SomeNullableInt"": 20,
                            ""SomeNullableGuid"": ""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",
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
                            ""Price"": 23.1,
                            ""ShippingAddress"": ""Some address 2"",
                            ""ShippingDate"": ""2019-10-10""
                        }
                    ]
                }");

            public static JsonDocument CreateCustomer2()
                => JsonDocument.Parse(
                    @"
                {
                    ""Name"": ""Moe"",
                    ""Age"": 35,
                    ""ID"": ""3272b593-bfe2-4ecf-81ae-4242b0632465"",
                    ""IsVip"": true,
                    ""Statistics"":
                    {
                        ""Visits"": 20,
                        ""Purchases"": 25,
                        ""Nested"":
                        {
                            ""SomeProperty"": 20,
                            ""SomeNullableInt"": null,
                            ""SomeNullableGuid"": null,
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
                }");

            public static JsonDocument CreateCustomer3()
                => JsonDocument.Parse(@"""foo""");

            public class JsonPocoChangeTrackingContextOptions
            {
                public MySqlJsonChangeTrackingOptions? ChangeTrackingOptions { get; set; }
                public bool AreChangeTrackingOptionsGlobal { get; set; }
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            public JsonDocument CustomerDocument { get; set; }
            public JsonElement CustomerElement { get; set; }
        }

        public class JsonMicrosoftDomChangeTrackingFixture : ServiceProviderPerContextFixtureBase<JsonMicrosoftDomChangeTrackingContext>
        {
            protected override string StoreName => "JsonMicrosoftDomChangeTrackingTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            protected override void Seed(JsonMicrosoftDomChangeTrackingContext context) => context.Seed();

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkMySqlJsonMicrosoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var options = base.AddOptions(builder);
                new MySqlDbContextOptionsBuilder(options)
                    .UseMicrosoftJson();

                return options;
            }
        }

        #endregion
    }
}
