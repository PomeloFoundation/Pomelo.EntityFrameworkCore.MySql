using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.ManyToManyModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class ManyToManyTrackingMySqlTestBase<TFixture> : ManyToManyTrackingTestBase<TFixture>
        where TFixture : ManyToManyTrackingTestBase<TFixture>.ManyToManyTrackingFixtureBase
    {
        protected ManyToManyTrackingMySqlTestBase(TFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class ManyToManyTrackingMySqlFixtureBase : ManyToManyTrackingFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                var propertyBuilder = modelBuilder
                    .Entity<JoinOneSelfPayload>()
                    .Property(e => e.Payload);

                SetupUtcDefaultValue(propertyBuilder);

                modelBuilder
                    .SharedTypeEntity<Dictionary<string, object>>("JoinOneToThreePayloadFullShared")
                    .IndexerProperty<string>("Payload")
                    .HasMaxLength(255) // longtext does not support default values
                    .HasDefaultValue("Generated");

                modelBuilder
                    .Entity<JoinOneToThreePayloadFull>()
                    .Property(e => e.Payload)
                    .HasMaxLength(255) // longtext does not support default values
                    .HasDefaultValue("Generated");

                propertyBuilder = modelBuilder
                    .Entity<UnidirectionalJoinOneSelfPayload>()
                    .Property(e => e.Payload);

                SetupUtcDefaultValue(propertyBuilder);

                modelBuilder
                    .SharedTypeEntity<Dictionary<string, object>>("UnidirectionalJoinOneToThreePayloadFullShared")
                    .IndexerProperty<string>("Payload")
                    .HasMaxLength(255) // longtext does not support default values
                    .HasDefaultValue("Generated");

                modelBuilder
                    .Entity<UnidirectionalJoinOneToThreePayloadFull>()
                    .Property(e => e.Payload)
                    .HasMaxLength(255) // longtext does not support default values
                    .HasDefaultValue("Generated");
            }

            private void SetupUtcDefaultValue(PropertyBuilder<DateTime> propertyBuilder)
            {
                // The original SQL Server implementation uses GETUTCDATE().
                if (SupportsDefaultExpressions)
                {
                    propertyBuilder
                        .HasDefaultValueSql("(UTC_TIMESTAMP())");
                }
                else
                {
                    // This is the same as using .HasDefaultValueSql("CURRENT_TIMESTAMP()");
                    propertyBuilder
                        .ValueGeneratedOnAdd();
                }
            }

            protected virtual bool SupportsDefaultExpressions
                => AppConfig.ServerVersion.Supports.DefaultExpression ||
                   AppConfig.ServerVersion.Supports.AlternativeDefaultExpression;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var optionsBuilder = base.AddOptions(builder);

                // To support the Can_insert_many_to_many_self_with_payload_unidirectional test, a UTC_TIMESTAMP would need to be set as the
                // column default value. Since this depends on default value expression support, we explicitly check for that.
                // As a workaround for server versions that do not support default value expressions, we will set the default session
                // timezone to UTC and just use a regular CURRENT_TIMESTAMP for the column default value in question.
                if (!SupportsDefaultExpressions)
                {
                    optionsBuilder
                        .AddInterceptors(new SessionInitializingConnectionInterceptor());
                }

                return optionsBuilder;
            }

            private class SessionInitializingConnectionInterceptor : DbConnectionInterceptor
            {
                public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
                    => InitializeSession(connection);

                public override Task ConnectionOpenedAsync(DbConnection connection,
                    ConnectionEndEventData eventData,
                    CancellationToken cancellationToken = new CancellationToken())
                {
                    InitializeSession(connection);
                    return Task.CompletedTask;
                }

                private static void InitializeSession(DbConnection connection)
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = "SET @@session.time_zone = '+00:00'";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
