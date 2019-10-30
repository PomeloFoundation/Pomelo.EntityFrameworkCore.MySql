using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

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

        protected override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql("Database=DummyDatabase");

        public IServiceProvider CreateContextServices(Version version, ServerType type)
            => ((IInfrastructure<IServiceProvider>)new DbContext(CreateOptions(version, type))).Instance;

        public IServiceProvider CreateContextServices(CharSetBehavior charSetBehavior, CharSet charSet)
            => ((IInfrastructure<IServiceProvider>)new DbContext(CreateOptions(charSetBehavior, charSet))).Instance;

        public ModelBuilder CreateConventionBuilder(IServiceProvider contextServices)
        {
            var conventionSet = contextServices.GetRequiredService<IConventionSetBuilder>()
                .CreateConventionSet();

            return new ModelBuilder(conventionSet);
        }

        public DbContextOptions CreateOptions(Version version, ServerType type)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=DummyDatabase", b => b.ServerVersion(version, type));

            return optionsBuilder.Options;
        }

        public DbContextOptions CreateOptions(CharSetBehavior charSetBehavior, CharSet charSet)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=DummyDatabase", b => b
                .CharSetBehavior(charSetBehavior)
                .CharSet(charSet));

            return optionsBuilder.Options;
        }
    }
}
