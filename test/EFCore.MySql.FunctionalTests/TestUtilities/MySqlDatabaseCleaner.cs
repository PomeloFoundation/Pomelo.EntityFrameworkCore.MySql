using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlDatabaseCleaner : RelationalDatabaseCleaner
    {
        private readonly IMySqlOptions _options;

        public MySqlDatabaseCleaner(IMySqlOptions options)
        {
            _options = options;
        }

        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new MySqlDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    loggerFactory,
                    new LoggingOptions(),
                    new DiagnosticListener("Fake"),
                    new MySqlLoggingDefinitions(),
                    new NullDbContextLogger()),
                _options);

        protected override bool AcceptIndex(DatabaseIndex index) => false;
        protected override bool AcceptTable(DatabaseTable table) => !(table is DatabaseView);

        protected override string BuildCustomSql(DatabaseModel databaseModel)
            => @"SET @views = NULL;

SELECT GROUP_CONCAT(CONCAT('`', `TABLE_SCHEMA`, '.', `TABLE_NAME`, '`')) INTO @views
FROM `INFORMATION_SCHEMA`.`VIEWS`
WHERE `TABLE_SCHEMA` = SCHEMA();

SET @views = IFNULL(CONCAT('DROP VIEW IF EXISTS ', @views), 'SELECT 0');

PREPARE stmt FROM @views;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;";
    }
}
