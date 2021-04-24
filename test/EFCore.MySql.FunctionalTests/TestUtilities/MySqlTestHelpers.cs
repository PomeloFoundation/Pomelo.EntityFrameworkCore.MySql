using System;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
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

        public DbContextOptions CreateOptions(Action<MySqlDbContextOptionsBuilder> builder)
            => new DbContextOptionsBuilder()
                .UseMySql("Database=DummyDatabase", AppConfig.ServerVersion, builder)
                .Options;

        public void EnsureSufficientKeySpace(IMutableModel model, TestStore testStore = null, bool limitColumnSize = false)
        {
            if (!AppConfig.ServerVersion.Supports.LargerKeyLength)
            {
                var databaseCharSet = CharSet.GetCharSetFromName((testStore as MySqlTestStore)?.DatabaseCharSet) ?? CharSet.Utf8Mb4;

                foreach (var entityType in model.GetEntityTypes())
                {
                    var indexes = entityType.GetIndexes();
                    var indexedStringProperties = entityType.GetProperties()
                        .Where(p => p.ClrType == typeof(string) &&
                                    indexes.Any(i => i.Properties.Contains(p)))
                        .ToList();

                    if (indexedStringProperties.Count > 0)
                    {
                        var safePropertyLength = AppConfig.ServerVersion.MaxKeyLength /
                                                 databaseCharSet.MaxBytesPerChar /
                                                 indexedStringProperties.Count;

                        if (limitColumnSize)
                        {
                            foreach (var property in indexedStringProperties)
                            {
                                property.SetMaxLength(safePropertyLength);
                            }
                        }
                        else
                        {
                            foreach (var index in indexes)
                            {
                                index.SetPrefixLength(
                                    index.Properties.Select(
                                            p => indexedStringProperties.Contains(p) && p.GetMaxLength() > safePropertyLength
                                                ? safePropertyLength
                                                : 0)
                                        .ToArray());
                            }
                        }
                    }
                }
            }
        }

        public int GetIndexedStringPropertyDefaultLength
            => Math.Min(AppConfig.ServerVersion.MaxKeyLength / (CharSet.Utf8Mb4.MaxBytesPerChar * 2), 255);
    }
}
