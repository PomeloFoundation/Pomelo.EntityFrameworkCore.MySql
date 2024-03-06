using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
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

        [Fact]
        public void Can_create_admin_connection_with_data_source()
        {
            using var _ = ((MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionTest"))
                .Initialize(null, (Func<DbContext>)null);

            using var dataSource = new MySqlDataSourceBuilder(MySqlTestStore.CreateConnectionString("ConnectionTest")).Build();

            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseMySql(dataSource, AppConfig.ServerVersion, b => b.ApplyConfiguration());
            using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalConnection = context.GetService<IMySqlRelationalConnection>();
            using var masterConnection = relationalConnection.CreateMasterConnection();

            Assert.Equal(string.Empty, new MySqlConnectionStringBuilder(masterConnection.ConnectionString).Database);

            masterConnection.Open();
        }

        [Fact]
        public void Can_create_admin_connection_with_connection_string()
        {
            using var _ = ((MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionTest"))
                .Initialize(null, (Func<DbContext>)null);

            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseMySql(MySqlTestStore.CreateConnectionString("ConnectionTest"), AppConfig.ServerVersion, b => b.ApplyConfiguration());
            using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalConnection = context.GetService<IMySqlRelationalConnection>();
            using var masterConnection = relationalConnection.CreateMasterConnection();

            Assert.Equal(string.Empty, new MySqlConnectionStringBuilder(masterConnection.ConnectionString).Database);

            masterConnection.Open();
        }

        [Fact]
        public void Can_create_admin_connection_with_connection()
        {
            using var _ = ((MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionTest"))
                .Initialize(null, (Func<DbContext>)null);

            using var connection = new MySqlConnection(MySqlTestStore.CreateConnectionString("ConnectionTest"));
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseMySql(connection, AppConfig.ServerVersion, b => b.ApplyConfiguration());
            using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalConnection = context.GetService<IMySqlRelationalConnection>();
            using var masterConnection = relationalConnection.CreateMasterConnection();

            Assert.Equal(string.Empty, new MySqlConnectionStringBuilder(masterConnection.ConnectionString).Database);

            masterConnection.Open();
        }

        [Fact]
        public void Can_create_database_with_disablebackslashescaping()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseMySql(MySqlTestStore.CreateConnectionString("ConnectionTest_" + Guid.NewGuid()), AppConfig.ServerVersion, b => b.ApplyConfiguration().DisableBackslashEscaping());
            using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalDatabaseCreator = context.GetService<IRelationalDatabaseCreator>();

            try
            {
                relationalDatabaseCreator.EnsureCreated();
            }
            finally
            {
                try
                {
                    relationalDatabaseCreator.EnsureDeleted();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMySql()
            .BuildServiceProvider();

        protected ConnectionMysqlContext CreateContext(string connectionString)
            => new ConnectionMysqlContext(_serviceProvider, connectionString);

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

        public class GeneralOptionsContext : DbContext
        {
            public GeneralOptionsContext(DbContextOptions<GeneralOptionsContext> options)
                : base(options)
            {
            }
        }
    }
}
