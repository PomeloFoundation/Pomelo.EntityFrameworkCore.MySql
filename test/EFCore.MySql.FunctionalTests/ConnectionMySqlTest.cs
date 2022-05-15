using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ConnectionMySqlTest
    {
        [ConditionalFact]
        public virtual void SetConnectionString()
        {
            var correctConnectionString = MySqlTestStore.CreateConnectionString("ConnectionTest");

            var csb = new MySqlConnectionStringBuilder(correctConnectionString);
            var correctPort = csb.Port;

            // Set an incorrect port, where no database server is listening.
            csb.Port = 65123;

            var incorrectConnectionString = csb.ConnectionString;
            using var context = CreateContext(incorrectConnectionString);

            context.Database.SetConnectionString(correctConnectionString);

            var connection = (MySqlConnection)context.Database.GetDbConnection();
            csb = new MySqlConnectionStringBuilder(connection.ConnectionString);

            Assert.Equal(csb.Port, correctPort);
        }

        [ConditionalFact]
        public virtual void SetConnectionString_affects_master_connection()
        {
            var correctConnectionString = MySqlTestStore.CreateConnectionString("ConnectionTest");

            // Set an incorrect port, where no database server is listening.
            var csb = new MySqlConnectionStringBuilder(correctConnectionString) { Port = 65123 };

            var incorrectConnectionString = csb.ConnectionString;
            using var context = CreateContext(incorrectConnectionString);

            context.Database.SetConnectionString(correctConnectionString);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMySql()
            .BuildServiceProvider();

        protected ConnectionMysqlContext CreateContext(string connectionString)
            => new ConnectionMysqlContext(_serviceProvider, connectionString);
    }

    public class ConnectionMysqlContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _connectionString;

        public ConnectionMysqlContext(IServiceProvider serviceProvider, string connectionString)
        {
            _serviceProvider = serviceProvider;
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseMySql(_connectionString, AppConfig.ServerVersion)
                .UseInternalServiceProvider(_serviceProvider);
    }
}
