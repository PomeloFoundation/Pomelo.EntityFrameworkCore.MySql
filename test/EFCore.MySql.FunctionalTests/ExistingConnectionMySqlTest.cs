using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class ExistingConnectionMySqlTest
{
    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    private static async Task Can_use_an_existing_closed_connection(bool openConnection)
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMySql()
            .BuildServiceProvider();

        using (var store = (MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                   .GetOrCreate(null)
                   .Initialize(null, (Func<DbContext>)null))
        {
            store.CloseConnection();

            var openCount = 0;
            var closeCount = 0;

            using (var connection = new MySqlConnection(store.ConnectionString))
            {
                if (openConnection)
                {
                    await connection.OpenAsync();
                }

                connection.StateChange += (_, a) =>
                {
                    if (a.CurrentState == ConnectionState.Open)
                    {
                        openCount++;
                    }
                    else if (a.CurrentState == ConnectionState.Closed)
                    {
                        closeCount++;
                    }
                };

                var options = new DbContextOptionsBuilder<NorthwindContext>()
                    .UseMySql(connection, AppConfig.ServerVersion)
                    .UseInternalServiceProvider(serviceProvider)
                    .Options;

                using (var context = new NorthwindContext(options))
                {
                    Assert.Equal(91, await context.Customers.CountAsync());
                }

                if (openConnection)
                {
                    Assert.Equal(ConnectionState.Open, connection.State);
                    Assert.Equal(0, openCount);
                    Assert.Equal(0, closeCount);
                }
                else
                {
                    Assert.Equal(ConnectionState.Closed, connection.State);
                    Assert.Equal(1, openCount);
                    Assert.Equal(1, closeCount);
                }
            }
        }
    }

    [Fact]
    private static async Task Closed_connection_missing_AllowUserVariables_true_in_original_connection_string()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMySql()
            .BuildServiceProvider();

        await using (var store = (MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                         .GetOrCreate(null)
                         .Initialize(null, (Func<DbContext>)null))
        {
            store.CloseConnection();

            var csb = new MySqlConnectionStringBuilder(store.ConnectionString) { AllowUserVariables = false };

            await using (var connection = new MySqlConnection(csb.ConnectionString))
            {
                var options = new DbContextOptionsBuilder<NorthwindContext>()
                    .UseMySql(connection, AppConfig.ServerVersion)
                    .UseInternalServiceProvider(serviceProvider)
                    .Options;

                using var context = new NorthwindContext(options);

                Assert.Equal(91, await context.Customers.CountAsync());
            }
        }
    }

    [Fact]
    private static async Task Opened_connection_missing_AllowUserVariables_true_in_original_connection_string_throws()
    {
        await using (var store = (MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                         .GetOrCreate(null)
                         .Initialize(null, (Func<DbContext>)null))
        {
            store.CloseConnection();

            var csb = new MySqlConnectionStringBuilder(store.ConnectionString) { AllowUserVariables = false };

            await using (var connection = new MySqlConnection(csb.ConnectionString))
            {
                await connection.OpenAsync();

                Assert.Equal(
                    Assert.Throws<InvalidOperationException>(
                        () =>
                        {
                            new DbContextOptionsBuilder<NorthwindContext>()
                                .UseMySql(connection, AppConfig.ServerVersion);
                        }).Message,
                    @"The connection string of a connection used by Pomelo.EntityFrameworkCore.MySql must contain ""AllowUserVariables=true;UseAffectedRows=false"".");
            }
        }
    }

    [Fact]
    private static async Task Closed_connection_missing_UseAffectedRows_false_in_original_connection_string()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMySql()
            .BuildServiceProvider();

        await using (var store = (MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                         .GetOrCreate(null)
                         .Initialize(null, (Func<DbContext>)null))
        {
            store.CloseConnection();

            var csb = new MySqlConnectionStringBuilder(store.ConnectionString) { UseAffectedRows = true };

            await using (var connection = new MySqlConnection(csb.ConnectionString))
            {
                var options = new DbContextOptionsBuilder<NorthwindContext>()
                    .UseMySql(connection, AppConfig.ServerVersion)
                    .UseInternalServiceProvider(serviceProvider)
                    .Options;

                using var context = new NorthwindContext(options);

                Assert.Equal(91, await context.Customers.CountAsync());
            }
        }
    }

    [Fact]
    private static async Task Opened_connection_missing_UseAffectedRows_false_in_original_connection_string_throws()
    {
        await using (var store = (MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                         .GetOrCreate(null)
                         .Initialize(null, (Func<DbContext>)null))
        {
            store.CloseConnection();

            var csb = new MySqlConnectionStringBuilder(store.ConnectionString) { UseAffectedRows = true };

            await using (var connection = new MySqlConnection(csb.ConnectionString))
            {
                await connection.OpenAsync();

                Assert.Equal(
                    Assert.Throws<InvalidOperationException>(
                        () =>
                        {
                            new DbContextOptionsBuilder<NorthwindContext>()
                                .UseMySql(connection, AppConfig.ServerVersion);
                        }).Message,
                    @"The connection string of a connection used by Pomelo.EntityFrameworkCore.MySql must contain ""AllowUserVariables=true;UseAffectedRows=false"".");
            }
        }
    }

    private class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(c => c.CustomerId);
                b.ToTable("Customers");
            });
    }

    private class Customer
    {
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string Fax { get; set; }
    }
}
