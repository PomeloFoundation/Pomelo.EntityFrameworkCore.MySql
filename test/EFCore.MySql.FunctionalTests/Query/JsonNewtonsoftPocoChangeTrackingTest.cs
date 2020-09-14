﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Extensions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class JsonNewtonsoftPocoChangeTrackingTest : JsonPocoChangeTrackingTestBase<JsonNewtonsoftPocoChangeTrackingTest.JsonNewtonsoftPocoChangeTrackingFixture>
    {
        public JsonNewtonsoftPocoChangeTrackingTest(JsonNewtonsoftPocoChangeTrackingFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public class JsonNewtonsoftPocoChangeTrackingFixture : JsonPocoChangeTrackingFixtureBase
        {
            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkMySqlJsonNewtonsoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder, MySqlJsonChangeTrackingOptions? changeTrackingOptions)
            {
                builder = base.AddOptions(builder, changeTrackingOptions);

                if (changeTrackingOptions != null)
                {
                    new MySqlDbContextOptionsBuilder(builder)
                        .UseNewtonsoftJson(changeTrackingOptions.Value);
                }

                return builder;
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                var currentContext = (JsonPocoChangeTrackingContext)context;
                if (currentContext.ChangeTrackingOptions != null &&
                    !currentContext.ChangeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    modelBuilder.Entity<JsonEntity>(
                        entity =>
                        {
                            entity.Property(e => e.Customer)
                                .UseJsonChangeTrackingOptions(currentContext.ChangeTrackingOptions.Value.ChangeTrackingOptions);
                            entity.Property(e => e.ToplevelArray)
                                .UseJsonChangeTrackingOptions(currentContext.ChangeTrackingOptions.Value.ChangeTrackingOptions);
                        });
                }
            }
        }
    }
}
