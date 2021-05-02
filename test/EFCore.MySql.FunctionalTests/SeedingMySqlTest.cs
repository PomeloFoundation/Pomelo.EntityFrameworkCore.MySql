using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SeedingMySqlTest : SeedingTestBase
    {
        protected override TestStore TestStore => MySqlTestStore.Create("SeedingTest");

        protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
            => new SeedingMySqlContext(testId);

        protected class SeedingMySqlContext : SeedingContext
        {
            public SeedingMySqlContext(string testId)
                : base(testId)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseMySql(MySqlTestStore.CreateConnectionString($"Seeds{TestId}"), AppConfig.ServerVersion);
        }
    }
}
