using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public static class MySqlDatabaseFacadeTestExtensions
    {
        public static void EnsureClean(this DatabaseFacade databaseFacade)
            => new MySqlDatabaseCleaner().Clean(databaseFacade);
    }
}
