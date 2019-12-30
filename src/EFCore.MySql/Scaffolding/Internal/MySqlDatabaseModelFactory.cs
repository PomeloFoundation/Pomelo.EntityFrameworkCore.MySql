// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    public class MySqlDatabaseModelFactory : DatabaseModelFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;
        private MySqlScaffoldingConnectionSettings _settings;
        private readonly IMySqlOptions _options;

        public MySqlDatabaseModelFactory(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
            IMySqlOptions options)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
            _options = options;
            _settings = new MySqlScaffoldingConnectionSettings(string.Empty);
        }

        public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(options, nameof(options));

            _settings = new MySqlScaffoldingConnectionSettings(connectionString);

            using var connection = new MySqlConnection(_settings.GetProviderCompatibleConnectionString());
            return Create(connection, options);
        }

        public override DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(options, nameof(options));

            var databaseModel = new DatabaseModel();

            var connectionStartedOpen = connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                connection.Open();
            }

            try
            {
                SetupMySqlOptions(connection);

                databaseModel.DatabaseName = connection.Database;
                databaseModel.DefaultSchema = GetDefaultSchema(connection);

                var schemaList = Enumerable.Empty<string>().ToList();
                var tableList = options.Tables.ToList();
                var tableFilter = GenerateTableFilter(tableList, schemaList);

                var tables = GetTables(connection, tableFilter);
                foreach (var table in tables)
                {
                    table.Database = databaseModel;
                    databaseModel.Tables.Add(table);
                }

                return databaseModel;
            }
            finally
            {
                if (!connectionStartedOpen)
                {
                    connection.Close();
                }
            }
        }

        private void SetupMySqlOptions(DbConnection connection)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql(connection, builder =>
            {
                // Set the actual server version from the open connection here, so we can
                // access it from IMySqlOptions later when generating the code for the
                // `UseMySql()` call.
                if (_options.ServerVersion.IsDefault)
                {
                    try
                    {
                        var mySqlConnection = (MySqlConnection)connection;
                        builder.ServerVersion(new ServerVersion(mySqlConnection.ServerVersion));
                    }
                    catch (InvalidOperationException)
                    {
                        // If we cannot determine the server version for some reason, just fall
                        // back on the latest one (the default).

                        // TODO: Output warning.
                    }
                }
            });

            if (Equals(_options, new MySqlOptions()))
            {
                _options.Initialize(optionsBuilder.Options);
            }
        }

        private string GetDefaultSchema(DbConnection connection)
        {
            return null;
        }

        private static Func<string, string, bool> GenerateTableFilter(
            IReadOnlyList<string> tables,
            IReadOnlyList<string> schemas)
        {
            return tables.Count > 0 ? (s, t) => tables.Contains(t) : (Func<string, string, bool>)null;
        }

        private const string GetTablesQuery = @"SELECT
    `TABLE_NAME`,
    `TABLE_TYPE`,
    IF(`TABLE_COMMENT` = 'VIEW' AND `TABLE_TYPE` = 'VIEW', '', `TABLE_COMMENT`) AS `TABLE_COMMENT`
FROM
    `INFORMATION_SCHEMA`.`TABLES`
WHERE
    `TABLE_SCHEMA` = SCHEMA()
AND
    `TABLE_TYPE` IN ('BASE TABLE', 'VIEW');";

        private IEnumerable<DatabaseTable> GetTables(
            DbConnection connection,
            Func<string, string, bool> filter)
        {
            using (var command = connection.CreateCommand())
            {
                var tables = new List<DatabaseTable>();
                command.CommandText = GetTablesQuery;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetValueOrDefault<string>("TABLE_NAME");
                        var type = reader.GetValueOrDefault<string>("TABLE_TYPE");
                        var comment = reader.GetValueOrDefault<string>("TABLE_COMMENT");

                        var table = string.Equals(type, "base table", StringComparison.OrdinalIgnoreCase)
                            ? new DatabaseTable()
                            : new DatabaseView();

                        table.Schema = null;
                        table.Name = name;
                        table.Comment = string.IsNullOrEmpty(comment) ? null : comment;

                        if (filter?.Invoke(table.Schema, table.Name) ?? true)
                        {
                            tables.Add(table);
                        }
                    }
                }

                // This is done separately due to MARS property may be turned off
                GetColumns(connection, tables, filter);
                GetPrimaryKeys(connection, tables);
                GetIndexes(connection, tables, filter);
                GetConstraints(connection, tables);

                return tables;
            }
        }

        private const string GetColumnsQuery = @"SELECT
	`COLUMN_NAME`,
    `ORDINAL_POSITION`,
    `COLUMN_DEFAULT`,
    IF(`IS_NULLABLE` = 'YES', 1, 0) AS `IS_NULLABLE`,
    `DATA_TYPE`,
    `CHARACTER_SET_NAME`,
    `COLLATION_NAME`,
    `COLUMN_TYPE`,
    `COLUMN_COMMENT`,
    `EXTRA`
FROM
	`INFORMATION_SCHEMA`.`COLUMNS`
WHERE
	`TABLE_SCHEMA` = SCHEMA()
AND
	`TABLE_NAME` = '{0}'
ORDER BY
    `ORDINAL_POSITION`;";

        private void GetColumns(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables,
            Func<string, string, bool> tableFilter)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetColumnsQuery, table.Name);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetValueOrDefault<string>("COLUMN_NAME");
                            var defaultValue = reader.GetValueOrDefault<string>("COLUMN_DEFAULT");
                            var nullable = reader.GetValueOrDefault<bool>("IS_NULLABLE");
                            var dataType = reader.GetValueOrDefault<string>("DATA_TYPE");
                            var charset = reader.GetValueOrDefault<string>("CHARACTER_SET_NAME");
                            var collation = reader.GetValueOrDefault<string>("COLLATION_NAME");
                            var columType = reader.GetValueOrDefault<string>("COLUMN_TYPE");
                            var extra = reader.GetValueOrDefault<string>("EXTRA");
                            var comment = reader.GetValueOrDefault<string>("COLUMN_COMMENT");

                            defaultValue = _options.ServerVersion.SupportsAlternativeDefaultExpression &&
                                           defaultValue != null
                                ? ConvertDefaultValueFromMariaDbToMySql(defaultValue)
                                : defaultValue;

                            ValueGenerated valueGenerated;

                            if (extra.IndexOf("auto_increment", StringComparison.Ordinal) >= 0)
                            {
                                valueGenerated = ValueGenerated.OnAdd;
                            }
                            else if (extra.IndexOf("on update", StringComparison.Ordinal) >= 0)
                            {
                                if (defaultValue != null && extra.IndexOf(defaultValue, StringComparison.Ordinal) > 0 ||
                                    (string.Equals(dataType, "timestamp", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(dataType, "datetime", StringComparison.OrdinalIgnoreCase)) &&
                                    extra.IndexOf("CURRENT_TIMESTAMP", StringComparison.Ordinal) > 0)
                                {
                                    valueGenerated = ValueGenerated.OnAddOrUpdate;
                                }
                                else
                                {
                                    // BUG: EF Core does not handle code generation for `OnUpdate`.
                                    //      Instead, it just generates an empty method call ".()".
                                    //      Tracked by: https://github.com/aspnet/EntityFrameworkCore/issues/18579
                                    //
                                    //      As a partial workaround, use `OnAddOrUpdate`, if a default value
                                    //      has been specified.

                                    if (defaultValue != null)
                                    {
                                        valueGenerated = ValueGenerated.OnAddOrUpdate;
                                    }
                                    else
                                    {
                                        valueGenerated = ValueGenerated.OnUpdate;
                                    }
                                }
                            }
                            else
                            {
                                valueGenerated = ValueGenerated.Never;
                            }

                            defaultValue = FilterClrDefaults(dataType, nullable, defaultValue);

                            var column = new DatabaseColumn
                            {
                                Table = table,
                                Name = name,
                                StoreType = columType,
                                IsNullable = nullable,
                                DefaultValueSql = CreateDefaultValueString(defaultValue, dataType),
                                ValueGenerated = valueGenerated,
                                Comment = string.IsNullOrEmpty(comment) ? null : comment,
                                [MySqlAnnotationNames.CharSet] = _settings.CharSet ? charset : null,
                                [MySqlAnnotationNames.Collation] = _settings.Collation ? collation : null,
                            };

                            table.Columns.Add(column);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// MariaDB 10.2.7+ implements default values differently from MySQL, to support their own default expression
        /// syntax. We convert their column values to MySQL compatible syntax here.
        /// See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/994#issuecomment-568271740
        /// for tables with differences.
        /// </summary>
        private string ConvertDefaultValueFromMariaDbToMySql([NotNull] string defaultValue)
        {
            if (string.Equals(defaultValue, "NULL", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (defaultValue.StartsWith("'", StringComparison.Ordinal) &&
                defaultValue.EndsWith("'", StringComparison.Ordinal) &&
                defaultValue.Length >= 2)
            {
                // MariaDb escapes all single quotes with two single quotes in default value strings, even if they are
                // escaped with backslashes in the original `CREATE TABLE` statement.
                return defaultValue.Substring(1, defaultValue.Length - 2)
                    .Replace("''", "'");
            }

            return defaultValue;
        }

        private static string FilterClrDefaults(string dataTypeName, bool nullable, string defaultValue)
        {
            if (defaultValue == null)
            {
                return null;
            }

            if (nullable)
            {
                return defaultValue;
            }

            if (defaultValue == "0")
            {
                if (dataTypeName == "bit"
                    || dataTypeName == "tinyint"
                    || dataTypeName == "smallint"
                    || dataTypeName == "int"
                    || dataTypeName == "bigint"
                    || dataTypeName == "decimal"
                    || dataTypeName == "double"
                    || dataTypeName == "float")
                {
                    return null;
                }
            }
            else if (Regex.IsMatch(defaultValue, @"^0\.0+$"))
            {
                if (dataTypeName == "decimal"
                    || dataTypeName == "double"
                    || dataTypeName == "float")
                {
                    return null;
                }
            }

            return defaultValue;
        }

        private string CreateDefaultValueString(string defaultValue, string dataType)
        {
            if (defaultValue == null)
            {
                return null;
            }

            // Pending the MySqlConnector implement MySqlCommandBuilder class
            if ((string.Equals(dataType, "timestamp", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(dataType, "datetime", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(defaultValue, "CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase))
            {
                return defaultValue;
            }

            // Handle bit values.
            if (string.Equals(dataType, "bit", StringComparison.OrdinalIgnoreCase)
                && defaultValue.StartsWith("b'"))
            {
                return defaultValue;
            }

            return "'" + defaultValue.Replace(@"\", @"\\").Replace("'", "''") + "'";
        }

        private const string GetPrimaryQuery = @"SELECT `INDEX_NAME`,
     `NON_UNIQUE`,
     GROUP_CONCAT(`COLUMN_NAME` ORDER BY `SEQ_IN_INDEX` SEPARATOR ',') AS COLUMNS
     FROM `INFORMATION_SCHEMA`.`STATISTICS`
     WHERE `TABLE_SCHEMA` = '{0}'
     AND `TABLE_NAME` = '{1}'
     AND `INDEX_NAME` = 'PRIMARY'
     GROUP BY `INDEX_NAME`, `NON_UNIQUE`;";

        /// <remarks>
        /// Primary keys are handled as in <see cref="GetConstraints"/>, not here
        /// </remarks>
        private void GetPrimaryKeys(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetPrimaryQuery, connection.Database, table.Name);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                var index = new DatabasePrimaryKey {Table = table, Name = reader.GetString(0)};

                                foreach (var column in reader.GetString(2).Split(','))
                                {
                                    index.Columns.Add(table.Columns.Single(y => y.Name == column));
                                }

                                table.PrimaryKey = index;
                            }
                            catch (Exception ex)
                            {
                                _logger.Logger.LogError(ex, "Error assigning primary key for {table}.", table.Name);
                            }
                        }
                    }
                }
            }
        }

        private const string GetIndexesQuery = @"SELECT `INDEX_NAME`,
     `NON_UNIQUE`,
     GROUP_CONCAT(`COLUMN_NAME` ORDER BY `SEQ_IN_INDEX` SEPARATOR ',') AS COLUMNS
     FROM `INFORMATION_SCHEMA`.`STATISTICS`
     WHERE `TABLE_SCHEMA` = '{0}'
     AND `TABLE_NAME` = '{1}'
     AND `INDEX_NAME` <> 'PRIMARY'
     GROUP BY `INDEX_NAME`, `NON_UNIQUE`;";

        /// <remarks>
        /// Primary keys are handled as in <see cref="GetConstraints"/>, not here
        /// </remarks>
        private void GetIndexes(DbConnection connection, IReadOnlyList<DatabaseTable> tables, Func<string, string, bool> tableFilter)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetIndexesQuery, connection.Database, table.Name);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                var index = new DatabaseIndex {Table = table, Name = reader.GetString(0), IsUnique = !reader.GetBoolean(1)};

                                foreach (var column in reader.GetString(2).Split(','))
                                {
                                    index.Columns.Add(table.Columns.Single(y => y.Name == column));
                                }

                                table.Indexes.Add(index);
                            }
                            catch (Exception ex)
                            {
                                _logger.Logger.LogError(ex, "Error assigning index for {table}.", table.Name);
                            }
                        }
                    }
                }
            }
        }

        private const string GetConstraintsQuery = @"SELECT
 	`CONSTRAINT_NAME`,
 	`TABLE_NAME`,
 	`REFERENCED_TABLE_NAME`,
 	GROUP_CONCAT(CONCAT_WS('|', `COLUMN_NAME`, `REFERENCED_COLUMN_NAME`) ORDER BY `ORDINAL_POSITION` SEPARATOR ',') AS PAIRED_COLUMNS,
 	(SELECT `DELETE_RULE` FROM `INFORMATION_SCHEMA`.`REFERENTIAL_CONSTRAINTS` WHERE `REFERENTIAL_CONSTRAINTS`.`CONSTRAINT_NAME` = `KEY_COLUMN_USAGE`.`CONSTRAINT_NAME` AND `REFERENTIAL_CONSTRAINTS`.`CONSTRAINT_SCHEMA` = `KEY_COLUMN_USAGE`.`CONSTRAINT_SCHEMA`) AS `DELETE_RULE`
 FROM `INFORMATION_SCHEMA`.`KEY_COLUMN_USAGE`
 WHERE `TABLE_SCHEMA` = '{0}'
 		AND `TABLE_NAME` = '{1}'
 		AND `CONSTRAINT_NAME` <> 'PRIMARY'
        AND `REFERENCED_TABLE_NAME` IS NOT NULL
        GROUP BY `CONSTRAINT_SCHEMA`,
        `CONSTRAINT_NAME`,
        `TABLE_NAME`,
        `REFERENCED_TABLE_NAME`;";

        private void GetConstraints(DbConnection connection, IReadOnlyList<DatabaseTable> tables)
        {
            foreach (var table in tables)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(GetConstraintsQuery, connection.Database, table.Name);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var referencedTableName = reader.GetString(2);
                            var referencedTable = tables.FirstOrDefault(t => t.Name == referencedTableName);
                            if (referencedTable != null)
                            {
                                var fkInfo = new DatabaseForeignKey {Name = reader.GetString(0), OnDelete = ConvertToReferentialAction(reader.GetString(4)), Table = table, PrincipalTable = referencedTable};
                                foreach (var pair in reader.GetString(3).Split(','))
                                {
                                    fkInfo.Columns.Add(table.Columns.Single(y =>
                                        string.Equals(y.Name, pair.Split('|')[0], StringComparison.OrdinalIgnoreCase)));
                                    fkInfo.PrincipalColumns.Add(fkInfo.PrincipalTable.Columns.Single(y =>
                                        string.Equals(y.Name, pair.Split('|')[1], StringComparison.OrdinalIgnoreCase)));
                                }

                                table.ForeignKeys.Add(fkInfo);
                            }
                            else
                            {
                                _logger.Logger.LogWarning($"Referenced table `{referencedTableName}` is not in dictionary.");
                            }
                        }
                    }
                }
            }
        }

        private static ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
        {
            switch (onDeleteAction.ToUpperInvariant())
            {
                case "RESTRICT":
                    return ReferentialAction.Restrict;

                case "CASCADE":
                    return ReferentialAction.Cascade;

                case "SET NULL":
                    return ReferentialAction.SetNull;

                case "NO ACTION":
                    return ReferentialAction.NoAction;

                default:
                    return null;
            }
        }
    }
}
