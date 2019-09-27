using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlDatabaseCleaner : RelationalDatabaseCleaner
    {
        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new MySqlDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    loggerFactory,
                    new LoggingOptions(),
                    new DiagnosticListener("Fake"),
                    new MySqlLoggingDefinitions()));

        protected override bool AcceptIndex(DatabaseIndex index) => false;
    }
}
