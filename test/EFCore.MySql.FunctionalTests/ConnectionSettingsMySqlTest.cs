using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ConnectionSettingsMySqlTest
    {
        [ConditionalTheory]
        [InlineData(MySqlGuidFormat.Char36, "'850368D8-93EA-4023-ACC7-6FA6E4C3B27F'", null)]
        [InlineData(MySqlGuidFormat.Char32, "'850368D893EA4023ACC76FA6E4C3B27F'", null)]
        [InlineData(MySqlGuidFormat.Binary16, "UUID_TO_BIN('850368D8-93EA-4023-ACC7-6FA6E4C3B27F', 0)", "8.0.0-mysql")]
        [InlineData(MySqlGuidFormat.Binary16, "X'850368D893EA4023ACC76FA6E4C3B27F'", null)]
        [InlineData(MySqlGuidFormat.TimeSwapBinary16, "UUID_TO_BIN('850368D8-93EA-4023-ACC7-6FA6E4C3B27F', 1)", "8.0.0-mysql")]
        [InlineData(MySqlGuidFormat.TimeSwapBinary16, "X'402393EA850368D8ACC76FA6E4C3B27F'", null)]
        [InlineData(MySqlGuidFormat.LittleEndianBinary16, "X'D8680385EA932340ACC76FA6E4C3B27F'", null)]
        [InlineData(MySqlGuidFormat.None, "X'D8680385EA932340ACC76FA6E4C3B27F'", null)]
        public virtual void Insert_and_read_Guid_value(MySqlGuidFormat guidFormat, string sqlEquivalent, string supportedServerVersion)
        {
            if (supportedServerVersion != null &&
                !AppConfig.ServerVersion.Supports.Version(ServerVersion.Parse(supportedServerVersion)))
            {
                return;
            }

            using var context = CreateContext(guidFormat);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.SimpleGuidEntities.Add(new SimpleGuidEntity { GuidValue = new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F") });
            context.SaveChanges();

            var result = context.SimpleGuidEntities
                .Where(e => e.GuidValue == new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F"))
                .ToList();

            var sqlResult = context.SimpleGuidEntities
                .FromSqlRaw(@"select * from `SimpleGuidEntities` where `GuidValue` = " + string.Format(sqlEquivalent, new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F")))
                .ToList();

            Assert.Single(result);
            Assert.Equal(new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F"), result[0].GuidValue);
            Assert.Single(sqlResult);
        }

        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMySql()
            .BuildServiceProvider();

        protected ConnectionSettingsContext CreateContext(MySqlGuidFormat guidFormat)
            => new ConnectionSettingsContext(_serviceProvider, "ConnectionSettings", guidFormat);
    }

    public class ConnectionSettingsContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _databaseName;
        private readonly MySqlGuidFormat _guidFormat;

        public ConnectionSettingsContext(IServiceProvider serviceProvider, string databaseName, MySqlGuidFormat guidFormat)
        {
            _serviceProvider = serviceProvider;
            _databaseName = databaseName;
            _guidFormat = guidFormat;
        }

        public DbSet<SimpleGuidEntity> SimpleGuidEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseMySql(MySqlTestStore.CreateConnectionString(_databaseName, false, _guidFormat), AppConfig.ServerVersion)
                .UseInternalServiceProvider(_serviceProvider);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<SimpleGuidEntity>();
    }

    public class SimpleGuidEntity
    {
        public int SimpleGuidEntityId { get; set; }
        public Guid GuidValue { get; set; }
    }
}
