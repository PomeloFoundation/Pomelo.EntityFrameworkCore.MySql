using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql
{
    /// <summary>
    ///     Verifies that schema generation (CREATE TABLE script) stays compatible after the EF Core 10
    ///     upgrade. The generated script is printed so that it can be compared against the EF Core 9
    ///     baseline (preview.3): an upgrade must not introduce unexpected schema changes.
    /// </summary>
    /// <remarks>
    ///     The generated DDL is MySQL specific, so the class is skipped on MariaDB and on MySQL
    ///     versions below 8.0.19.
    /// </remarks>
    [SupportedServerVersionCondition("8.0.19")]
    public class MySqlMigrationsCompatibilityTest : TestBase<MySqlMigrationsCompatibilityTest.MigrationsContext>
    {
        private readonly ITestOutputHelper _output;

        public MySqlMigrationsCompatibilityTest(ITestOutputHelper output)
            => _output = output;

        [ConditionalFact]
        public async Task Create_script_is_generated_for_model()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<MigrationEntity>().ToTable("migration_compat_test"));

            var script = context.Database.GenerateCreateScript();

            _output.WriteLine(script);
            Console.WriteLine("[MIGRATION_SQL]");
            Console.WriteLine(script);

            Assert.Contains("CREATE TABLE", script);
            Assert.Contains("`Id`", script);
            Assert.Contains("`Name`", script);
        }

        public class MigrationsContext : ContextBase
        {
        }

        public class MigrationEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
