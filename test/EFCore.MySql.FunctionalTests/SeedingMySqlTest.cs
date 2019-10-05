using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SeedingMySqlTest : SeedingTestBase
    {
        protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
        {
            var context = new SeedingMySqlContext(testId);

            context.Database.EnsureClean();

            return context;
        }

        protected class SeedingMySqlContext : SeedingContext
        {
            public SeedingMySqlContext(string testId)
                : base(testId)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseMySql(MySqlTestStore.CreateConnectionString($"Seeds{TestId}", false));
        }
    }
}
