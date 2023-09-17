using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class MaterializationInterceptionMySqlTest : MaterializationInterceptionTestBase<MaterializationInterceptionMySqlTest.MySqlLibraryContext>,
    IClassFixture<MaterializationInterceptionMySqlTest.MaterializationInterceptionMySqlFixture>
{
    public MaterializationInterceptionMySqlTest(MaterializationInterceptionMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class MySqlLibraryContext : LibraryContext
    {
        public MySqlLibraryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings);

            // TODO: https://github.com/npgsql/efcore.pg/issues/2548
            // modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings, b => b.ToJson());
        }
    }

    public override LibraryContext CreateContext(IEnumerable<ISingletonInterceptor> interceptors, bool inject)
        => new MySqlLibraryContext(Fixture.CreateOptions(interceptors, inject));

    public class MaterializationInterceptionMySqlFixture : SingletonInterceptorsFixtureBase
    {
        protected override string StoreName
            => "MaterializationInterception";

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<ISingletonInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkMySql(), injectedInterceptors);

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            new MySqlDbContextOptionsBuilder(base.AddOptions(builder))
                .ExecutionStrategy(d => new MySqlExecutionStrategy(d));
            return builder;
        }
    }
}
