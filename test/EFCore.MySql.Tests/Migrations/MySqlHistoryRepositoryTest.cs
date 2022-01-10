using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations;

public class MySqlHistoryRepositoryTest
{
    [ConditionalFact]
    public void ExistsSql_respects_SchemaBehavior_explicit()
    {
        var sql = CreateHistoryRepository("IgnoreThisDefaultSchema")
            .ExistsSql;

        Assert.Equal(
            @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='DummyDatabase' AND TABLE_NAME='__EFMigrationsHistory';",
            sql);
    }

    [ConditionalFact]
    public void ExistsSql_respects_SchemaBehavior()
    {
        var sql = CreateHistoryRepository("IgnoreThisExplicitSchema")
            .ExistsSql;

        Assert.Equal(
            @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='DummyDatabase' AND TABLE_NAME='__EFMigrationsHistory';",
            sql);
    }

    private static TestMysqlHistoryRepository CreateHistoryRepository(string schema = null)
        => (TestMysqlHistoryRepository)new TestDbContext(
                new DbContextOptionsBuilder(
                        MySqlTestHelpers.Instance.CreateOptions(builder => builder
                            .SchemaBehavior(MySqlSchemaBehavior.Ignore)
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema)))
                    .UseInternalServiceProvider(
                        MySqlTestHelpers.Instance.CreateServiceProvider(
                            new ServiceCollection()
                                .AddScoped<IHistoryRepository, TestMysqlHistoryRepository>()))
                    .Options)
            .GetService<IHistoryRepository>();

    private class TestMysqlHistoryRepository : MySqlHistoryRepository
    {
        public TestMysqlHistoryRepository([NotNull] HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        public new string ExistsSql => base.ExistsSql;
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("IgnoreThisDefaultSchema");
        }
    }

    private class Blog
    {
        public int Id { get; set; }
    }
}
