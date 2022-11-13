using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlDatabaseCleaner : RelationalDatabaseCleaner
    {
        private readonly IMySqlOptions _options;
        private readonly IRelationalTypeMappingSource _relationalTypeMappingSource;

        public MySqlDatabaseCleaner(IMySqlOptions options, IRelationalTypeMappingSource relationalTypeMappingSource)
        {
            _options = options;
            _relationalTypeMappingSource = relationalTypeMappingSource;
        }

        public override void Clean(DatabaseFacade facade)
        {
            var creator = facade.GetService<IRelationalDatabaseCreator>();
            var connection = facade.GetService<IRelationalConnection>();

            if (creator.Exists())
            {
                OpenConnection(connection);

                try
                {
                    var commands = new StringBuilder();

                    var getRoutinesSql = $@"
SELECT `ROUTINE_SCHEMA`, `ROUTINE_NAME`, `ROUTINE_TYPE`
FROM `INFORMATION_SCHEMA`.`ROUTINES`
WHERE `ROUTINE_SCHEMA` = SCHEMA();";

                    using var command = connection.DbConnection.CreateCommand();
                    command.CommandText = getRoutinesSql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (string.Equals(reader["ROUTINE_TYPE"] as string, "PROCEDURE", StringComparison.OrdinalIgnoreCase))
                            {
                                commands.AppendLine($"DROP PROCEDURE IF EXISTS `{reader["ROUTINE_SCHEMA"]}`.`{reader["ROUTINE_NAME"]}`;");
                            }
                            else if (string.Equals(reader["ROUTINE_TYPE"] as string, "FUNCTION", StringComparison.OrdinalIgnoreCase))
                            {
                                commands.AppendLine($"DROP FUNCTION IF EXISTS `{reader["ROUTINE_SCHEMA"]}`.`{reader["ROUTINE_NAME"]}`;");
                            }
                        }
                    }

                    if (commands.Length > 0)
                    {
                        command.CommandText = commands.ToString();
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            base.Clean(facade);
        }

        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new MySqlDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    loggerFactory,
                    new LoggingOptions(),
                    new DiagnosticListener("Fake"),
                    new MySqlLoggingDefinitions(),
                    new NullDbContextLogger()),
                _relationalTypeMappingSource,
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
