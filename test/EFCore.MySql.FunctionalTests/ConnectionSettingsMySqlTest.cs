using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ConnectionSettingsMySqlTest
    {
        [ConditionalTheory]
        [InlineData(MySqlGuidFormat.Char36, "'850368D8-93EA-4023-ACC7-6FA6E4C3B27F'")]
        [InlineData(MySqlGuidFormat.Char32, "'850368D893EA4023ACC76FA6E4C3B27F'")]
        [InlineData(MySqlGuidFormat.Binary16, "UUID_TO_BIN('850368D8-93EA-4023-ACC7-6FA6E4C3B27F', 0)")]
        [InlineData(MySqlGuidFormat.TimeSwapBinary16, "UUID_TO_BIN('850368D8-93EA-4023-ACC7-6FA6E4C3B27F', 1)")]
        [InlineData(MySqlGuidFormat.LittleEndianBinary16, "X'D8680385EA932340ACC76FA6E4C3B27F'")]
        [InlineData(MySqlGuidFormat.None, "X'D8680385EA932340ACC76FA6E4C3B27F'")]
        public virtual void Insert_and_read_Guid_value (MySqlGuidFormat guidFormat, string sqlEquivalent)
        {
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

            Assert.Equal(1, result.Count);
            Assert.Equal(new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F"), result[0].GuidValue);
            Assert.Equal(1, sqlResult.Count);
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
                .UseMySql(MySqlTestStore.CreateConnectionString(_databaseName, false, _guidFormat))
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
