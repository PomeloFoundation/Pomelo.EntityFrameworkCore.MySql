using System;
using System.Collections.Generic;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

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

        public IServiceProvider CreateContextServices(CharSetBehavior charSetBehavior, CharSet ansiCharSet, CharSet unicodeCharSet)
            => ((IInfrastructure<IServiceProvider>)new DbContext(CreateOptions(charSetBehavior, ansiCharSet, unicodeCharSet))).Instance;

        public ModelBuilder CreateConventionBuilder(IServiceProvider contextServices)
        {
            var conventionSetBuilder = new CompositeConventionSetBuilder(
                contextServices.GetRequiredService<IEnumerable<IConventionSetBuilder>>().ToList());
            var conventionSet = contextServices.GetRequiredService<ICoreConventionSetBuilder>().CreateConventionSet();
            conventionSet = conventionSetBuilder.AddConventions(conventionSet);
            return new ModelBuilder(conventionSet);
        }

        public DbContextOptions CreateOptions(Version version, ServerType type)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=DummyDatabase", b => b.ServerVersion(version, type));

            return optionsBuilder.Options;
        }

        public DbContextOptions CreateOptions(CharSetBehavior charSetBehavior, CharSet ansiCharSet, CharSet unicodeCharSet)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=DummyDatabase", b => b.CharSetBehavior(charSetBehavior).AnsiCharSet(ansiCharSet).UnicodeCharSet(unicodeCharSet));

            return optionsBuilder.Options;
        }
    }
}
