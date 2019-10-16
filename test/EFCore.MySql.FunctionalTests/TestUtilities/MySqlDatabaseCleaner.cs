using System;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlDatabaseCleaner : RelationalDatabaseCleaner
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMySqlOptions _options;

        public MySqlDatabaseCleaner(IServiceProvider serviceProvider, IMySqlOptions options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new MySqlDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    loggerFactory,
                    new LoggingOptions(),
                    new DiagnosticListener("Fake"),
                    new MySqlLoggingDefinitions()),
                _serviceProvider,
                _options);

        protected override bool AcceptIndex(DatabaseIndex index) => false;
    }
}
