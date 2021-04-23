using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public static class MySqlDatabaseFacadeTestExtensions
    {
        public static void EnsureClean(this DatabaseFacade databaseFacade)
            => new MySqlDatabaseCleaner(
                    databaseFacade.GetService<IMySqlOptions>(),
                    databaseFacade.GetService<IRelationalTypeMappingSource>())
                .Clean(databaseFacade);
    }
}
