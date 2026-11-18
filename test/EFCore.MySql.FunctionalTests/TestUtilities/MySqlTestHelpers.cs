using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microting.EntityFrameworkCore.MySql.Tests;
using Xunit;

//ReSharper disable once CheckNamespace
namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestHelpers : RelationalTestHelpers
    {
        protected MySqlTestHelpers()
        {
        }

        public static MySqlTestHelpers Instance { get; } = new MySqlTestHelpers();

        public override IServiceCollection AddProviderServices(IServiceCollection services)
            => services.AddEntityFrameworkMySql();

        public override DbContextOptionsBuilder UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql("Database=DummyDatabase", AppConfig.ServerVersion);

        public override LoggingDefinitions LoggingDefinitions { get; } = new MySqlLoggingDefinitions();

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
                                            p => p is IMutableProperty property
                                                 && indexedStringProperties.Contains(property)
                                                 && property.GetMaxLength() > safePropertyLength
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

        public static DateTimeOffset GetExpectedValue(DateTimeOffset value)
        {
            const int mySqlMaxMillisecondDecimalPlaces = 6;
            var decimalPlacesFactor = (decimal)Math.Pow(10, 7 - mySqlMaxMillisecondDecimalPlaces);

            // Change DateTimeOffset values, because MySQL does not preserve offsets and has a maximum of 6 decimal places, in contrast to
            // .NET which has 7.
            return new DateTimeOffset(
                (long)(Math.Truncate(value.UtcTicks / decimalPlacesFactor) * decimalPlacesFactor),
                TimeSpan.Zero);
        }

        public static string CastAsDouble(string innerSql)
            => AppConfig.ServerVersion.Supports.DoubleCast
                ? $@"CAST({innerSql} AS double)"
                : $@"(CAST({innerSql} AS decimal(65,30)) + 0e0)";

        public static string MySqlBug96947Workaround(string innerSql, string type = "char")
            => AppConfig.ServerVersion.Supports.MySqlBug96947Workaround
                ? $@"CAST({innerSql} AS {type})"
                : innerSql;

        public static bool HasPrimitiveCollectionsSupport<TContext>(SharedStoreFixtureBase<TContext> fixture)
            where TContext : DbContext
            => HasPrimitiveCollectionsSupport(fixture.CreateOptions());

        public static bool HasPrimitiveCollectionsSupport(DbContextOptions options)
            => AppConfig.ServerVersion.Supports.JsonTable &&
               options.GetExtension<MySqlOptionsExtension>().PrimitiveCollectionsSupport;

        /// <summary>
        /// Same implementation as EF Core base class, except that it generates code that reproduces the full parameter list of the test
        /// method being overridden (the EF Core implementation only handles a single `bool async` parameter).
        /// </summary>
        public static void AssertAllMethodsOverridden(Type testClass, bool withAssertSqlCall = true)
        {
            var methods = testClass
                .GetRuntimeMethods()
                .Where(
                    m => m.DeclaringType != testClass
                         && (Attribute.IsDefined(m, typeof(FactAttribute))
                             || Attribute.IsDefined(m, typeof(TheoryAttribute))))
                .ToList();

            var methodCalls = new StringBuilder();

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var parameterDeclarations = string.Join(", ", parameters.Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));
                var parameterNames = string.Join(", ", parameters.Select(p => p.Name));

                if (method.ReturnType == typeof(Task))
                {
                    methodCalls.Append(
                        @$"public override async Task {method.Name}({parameterDeclarations})
{{
    await base.{method.Name}({parameterNames});{(withAssertSqlCall ?
"""


    AssertSql();
""" : null)}
}}

");
                }
                else
                {
                    methodCalls.Append(
                        @$"public override void {method.Name}({parameterDeclarations})
{{
    base.{method.Name}({parameterNames});{(withAssertSqlCall ?
"""


    AssertSql();
""" : null)}
}}

");
                }
            }

            Assert.False(
                methods.Count > 0,
                "\r\n-- Missing test overrides --\r\n\r\n" + methodCalls);
        }

        private static string GetTypeName(Type type)
            => type switch
            {
                _ when type == typeof(bool) => "bool",
                _ when type == typeof(int) => "int",
                _ when type == typeof(string) => "string",
                _ when type == typeof(object) => "object",
                { IsGenericType: true } => type.Name[..type.Name.IndexOf('`')] +
                                           "<" +
                                           string.Join(", ", type.GetGenericArguments().Select(GetTypeName)) +
                                           ">",
                _ => type.Name
            };
    }
}
