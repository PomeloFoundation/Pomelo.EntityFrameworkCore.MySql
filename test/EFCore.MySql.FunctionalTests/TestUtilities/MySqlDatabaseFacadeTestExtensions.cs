using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.MySql.UpstreamFunctionalTests.TestUtilities
{
    public static class MySqlDatabaseFacadeTestExtensions
    {
        public static void EnsureClean(this DatabaseFacade databaseFacade)
            => new MySqlDatabaseCleaner().Clean(databaseFacade);
    }
}
