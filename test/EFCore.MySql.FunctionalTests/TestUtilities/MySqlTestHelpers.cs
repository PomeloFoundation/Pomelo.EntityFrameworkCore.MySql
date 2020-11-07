using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests;

//ReSharper disable once CheckNamespace
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestHelpers : TestHelpers
    {
        protected MySqlTestHelpers()
        {
        }

        public static MySqlTestHelpers Instance { get; } = new MySqlTestHelpers();

        public override IServiceCollection AddProviderServices(IServiceCollection services)
            => services.AddEntityFrameworkMySql();

        public override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql("Database=DummyDatabase", AppConfig.ServerVersion);

        public IServiceProvider CreateContextServices(ServerVersion serverVersion)
            => ((IInfrastructure<IServiceProvider>)new DbContext(CreateOptions(serverVersion))).Instance;

        public IServiceProvider CreateContextServices(CharSetBehavior charSetBehavior, CharSet charSet)
            => ((IInfrastructure<IServiceProvider>)new DbContext(CreateOptions(charSetBehavior, charSet))).Instance;

        public IServiceProvider CreateContextServices(Action<MySqlDbContextOptionsBuilder> builder)
            => ((IInfrastructure<IServiceProvider>)new DbContext(CreateOptions(builder))).Instance;

        public ModelBuilder CreateConventionBuilder(IServiceProvider contextServices)
        {
            var conventionSet = contextServices.GetRequiredService<IConventionSetBuilder>()
                .CreateConventionSet();

            return new ModelBuilder(conventionSet);
        }

        public DbContextOptions CreateOptions(ServerVersion serverVersion)
            => CreateOptions(b => {});

        public DbContextOptions CreateOptions(CharSetBehavior charSetBehavior, CharSet charSet)
            => CreateOptions(
                b => b.CharSetBehavior(charSetBehavior)
                    .CharSet(charSet));

        public DbContextOptions CreateOptions(Action<MySqlDbContextOptionsBuilder> builder)
            => new DbContextOptionsBuilder()
                .UseMySql("Database=DummyDatabase", AppConfig.ServerVersion, builder)
                .Options;
    }
}
