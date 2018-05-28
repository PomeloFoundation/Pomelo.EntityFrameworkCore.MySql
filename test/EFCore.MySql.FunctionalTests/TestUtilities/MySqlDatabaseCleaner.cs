using EFCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;

namespace EFCore.MySql.UpstreamFunctionalTests.TestUtilities
{
    public class MySqlDatabaseCleaner : RelationalDatabaseCleaner
    {
        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new MySqlDatabaseModelFactory(loggerFactory);

        protected override bool AcceptIndex(DatabaseIndex index) => false;
    }
}
