// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlConnectionTest
    {
        [ConditionalFact]
        public void Creates_SQL_Server_connection_string()
        {
            using (var connection = new MySqlConnection(CreateDependencies()))
            {
                Assert.IsType<SqlConnection>(connection.DbConnection);
            }
        }

        [ConditionalFact]
        public void Can_create_master_connection()
        {
            using (var connection = new MySqlConnection(CreateDependencies()))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master", master.ConnectionString);
                    Assert.Equal(60, master.CommandTimeout);
                }
            }
        }

        [ConditionalFact]
        public void Master_connection_string_contains_filename()
        {
            var options = new DbContextOptionsBuilder()
                .UseMySql(
                    @"Server=(localdb)\MSSQLLocalDB;Database=MySqlConnectionTest;AttachDBFilename=C:\Narf.mdf",
                    b => b.CommandTimeout(55))
                .Options;

            using (var connection = new MySqlConnection(CreateDependencies(options)))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master", master.ConnectionString);
                }
            }
        }

        [ConditionalFact]
        public void Master_connection_string_none_default_command_timeout()
        {
            var options = new DbContextOptionsBuilder()
                .UseMySql(
                    @"Server=(localdb)\MSSQLLocalDB;Database=MySqlConnectionTest",
                    b => b.CommandTimeout(55))
                .Options;

            using (var connection = new MySqlConnection(CreateDependencies(options)))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(55, master.CommandTimeout);
                }
            }
        }

        public static RelationalConnectionDependencies CreateDependencies(DbContextOptions options = null)
        {
            options ??= new DbContextOptionsBuilder()
                .UseMySql(@"Server=(localdb)\MSSQLLocalDB;Database=MySqlConnectionTest")
                .Options;

            return new RelationalConnectionDependencies(
                options,
                new DiagnosticsLogger<DbLoggerCategory.Database.Transaction>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new MySqlLoggingDefinitions()),
                new DiagnosticsLogger<DbLoggerCategory.Database.Connection>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new MySqlLoggingDefinitions()),
                new NamedConnectionStringResolver(options),
                new RelationalTransactionFactory(new RelationalTransactionFactoryDependencies()),
                new CurrentDbContext(new FakeDbContext()));
        }

        private class FakeDbContext : DbContext
        {
        }
    }
}
