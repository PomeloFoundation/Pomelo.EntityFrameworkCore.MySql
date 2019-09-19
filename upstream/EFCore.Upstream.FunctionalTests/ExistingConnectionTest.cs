// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore
{
    public class ExistingConnectionTest
    {
        // See aspnet/Data#135
        [ConditionalFact]
        public Task Can_use_an_existing_closed_connection()
        {
            return Can_use_an_existing_closed_connection_test(openConnection: false);
        }

        [ConditionalFact]
        public Task Can_use_an_existing_open_connection()
        {
            return Can_use_an_existing_closed_connection_test(openConnection: true);
        }

        private static async Task Can_use_an_existing_closed_connection_test(bool openConnection)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkMySql()
                .BuildServiceProvider();

            using (var store = MySqlTestStore.GetNorthwindStore())
            {
                store.CloseConnection();

                var openCount = 0;
                var closeCount = 0;
                var disposeCount = 0;

                using (var connection = new SqlConnection(store.ConnectionString))
                {
                    if (openConnection)
                    {
                        await connection.OpenAsync();
                    }

                    connection.StateChange += (_, a) =>
                    {
                        switch (a.CurrentState)
                        {
                            case ConnectionState.Open:
                                openCount++;
                                break;
                            case ConnectionState.Closed:
                                closeCount++;
                                break;
                        }
                    };
                    connection.Disposed += (_, __) => disposeCount++;

                    using (var context = new NorthwindContext(serviceProvider, connection))
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

                    Assert.Equal(0, disposeCount);
                }
            }
        }

        private class NorthwindContext : DbContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly SqlConnection _connection;

            public NorthwindContext(IServiceProvider serviceProvider, SqlConnection connection)
            {
                _serviceProvider = serviceProvider;
                _connection = connection;
            }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public DbSet<Customer> Customers { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseMySql(_connection, b => b.ApplyConfiguration())
                    .UseInternalServiceProvider(_serviceProvider);

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.Entity<Customer>(
                    b =>
                    {
                        b.HasKey(c => c.CustomerID);
                        b.ToTable("Customers");
                    });
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Customer
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string CustomerID { get; set; }

            // ReSharper disable UnusedMember.Local
            public string CompanyName { get; set; }

            public string Fax { get; set; }
        }
    }
}
