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
using MySqlConnector;
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
        private readonly IMySqlOptions _options;

        protected MySqlScaffoldingConnectionSettings Settings { get; set; }

        public MySqlDatabaseModelFactory(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
            IMySqlOptions options)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
            _options = options;
            Settings = new MySqlScaffoldingConnectionSettings(string.Empty);
        }

        public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(options, nameof(options));

            Settings = new MySqlScaffoldingConnectionSettings(connectionString);

            using var connection = new MySqlConnection(Settings.GetProviderCompatibleConnectionString());
            return Create(connection, options);
        }

        public override DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(options, nameof(options));

            SetupMySqlOptions(connection);
            _logger.Logger.LogInformation($"Using {nameof(ServerVersion)} '{_options.ServerVersion}'.");

            var databaseModel = new DatabaseModel();

            var connectionStartedOpen = connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                connection.Open();
            }

            try
            {
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

        protected virtual void SetupMySqlOptions(DbConnection connection)
        {
            // Set the actual server version from the open connection here, so we can
            // access it from IMySqlOptions later when generating the code for the
            // `UseMySql()` call.

            if (Equals(_options, new MySqlOptions()))
            {
                ServerVersion serverVersion;

                _logger.Logger.LogDebug($"No explicit {nameof(ServerVersion)} was set.");

                try
                {
                    serverVersion = ServerVersion.AutoDetect((MySqlConnection)connection);
                    _logger.Logger.LogDebug($"{nameof(ServerVersion)} '{serverVersion}' was automatically detected.");
                }
                catch (InvalidOperationException)
                {
                    // If we cannot determine the server version for some reason, just fall
                    // back on the latest MySQL version.
                    serverVersion = MySqlServerVersion.LatestSupportedServerVersion;

                    _logger.Logger.LogWarning($"No {nameof(ServerVersion)} could be automatically detected. The latest supported {nameof(ServerVersion)} will be used.");
                }

                _options.Initialize(
                    new DbContextOptionsBuilder()
                        .UseMySql(connection, serverVersion)
                        .Options);
            }
        }

        protected virtual string GetDefaultSchema(DbConnection connection)
            => null;

        protected virtual Func<string, string, bool> GenerateTableFilter(
            IReadOnlyList<string> tables,
            IReadOnlyList<string> schemas)
            => tables.Count > 0 ? (s, t) => tables.Contains(t) : (Func<string, string, bool>)null;

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

        protected virtual IEnumerable<DatabaseTable> GetTables(
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

                        var isValidByFilter = filter?.Invoke(table.Schema, table.Name) ?? true;
                        var isValidBySettings = !(table is DatabaseView) || Settings.Views;

                        if (isValidByFilter &&
                            isValidBySettings)
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
    `EXTRA` /*!80003 ,
    `SRS_ID` */
FROM
	`INFORMATION_SCHEMA`.`COLUMNS`
WHERE
	`TABLE_SCHEMA` = SCHEMA()
AND
	`TABLE_NAME` = '{0}'
ORDER BY
    `ORDINAL_POSITION`;";

        protected virtual void GetColumns(
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

                            // MariaDB does not support SRID column restrictions.
                            var srid = reader.GetColumnSchema().Any(c => string.Equals(c.ColumnName, "SRS_ID", StringComparison.OrdinalIgnoreCase))
                                ? reader.GetValueOrDefault<uint?>("SRS_ID")
                                : null;

                            defaultValue = _options.ServerVersion.Supports.AlternativeDefaultExpression &&
                                           defaultValue != null
                                ? ConvertDefaultValueFromMariaDbToMySql(defaultValue)
                                : defaultValue;

                            ValueGenerated? valueGenerated;

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
                                // Using `null` results in `ValueGeneratedNever()` being output for primary keys without
                                // auto increment as desired, while explicitly using `ValueGenerated.Never` results in
                                // no value generated output at all.
                                valueGenerated = null;
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
                                [MySqlAnnotationNames.CharSet] = Settings.CharSet ? charset : null,
                                [MySqlAnnotationNames.Collation] = Settings.Collation ? collation : null,
                                [MySqlAnnotationNames.SpatialReferenceSystemId] = srid.HasValue ? (int?)(int)srid.Value : null,
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
        protected virtual string ConvertDefaultValueFromMariaDbToMySql([NotNull] string defaultValue)
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

        protected virtual string FilterClrDefaults(string dataTypeName, bool nullable, string defaultValue)
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

        protected virtual string CreateDefaultValueString(string defaultValue, string dataType)
        {
            if (defaultValue == null)
            {
                return null;
            }

            // MySQL uses `CURRENT_TIMESTAMP` (or `CURRENT_TIMESTAMP(6)`),
            // while MariaDB uses `current_timestamp()` (or `current_timestamp(6)`).
            if ((string.Equals(dataType, "timestamp", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(dataType, "datetime", StringComparison.OrdinalIgnoreCase)) &&
                Regex.IsMatch(defaultValue, @"^CURRENT_TIMESTAMP(?:\(\d*\))?$", RegexOptions.IgnoreCase))
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
     GROUP_CONCAT(`COLUMN_NAME` ORDER BY `SEQ_IN_INDEX` SEPARATOR ',') AS `COLUMNS`,
     GROUP_CONCAT(IFNULL(`SUB_PART`, 0) ORDER BY `SEQ_IN_INDEX` SEPARATOR ',') AS `SUB_PARTS`
     FROM `INFORMATION_SCHEMA`.`STATISTICS`
     WHERE `TABLE_SCHEMA` = '{0}'
     AND `TABLE_NAME` = '{1}'
     AND `INDEX_NAME` = 'PRIMARY'
     GROUP BY `INDEX_NAME`;";

        protected virtual void GetPrimaryKeys(
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
                                var key = new DatabasePrimaryKey
                                {
                                    Table = table,
                                    Name = reader.GetValueOrDefault<string>("INDEX_NAME"),
                                };

                                foreach (var column in reader.GetValueOrDefault<string>("COLUMNS").Split(','))
                                {
                                    key.Columns.Add(table.Columns.Single(y => y.Name == column));
                                }

                                var prefixLengths = reader.GetValueOrDefault<string>("SUB_PARTS")
                                    .Split(',')
                                    .Select(int.Parse)
                                    .ToArray();

                                if (prefixLengths.Length > 1 ||
                                    prefixLengths.Length == 1 && prefixLengths[0] > 0)
                                {
                                    key[MySqlAnnotationNames.IndexPrefixLength] = prefixLengths;
                                }

                                table.PrimaryKey = key;
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
     GROUP_CONCAT(`COLUMN_NAME` ORDER BY `SEQ_IN_INDEX` SEPARATOR ',') AS `COLUMNS`,
     GROUP_CONCAT(IFNULL(`SUB_PART`, 0) ORDER BY `SEQ_IN_INDEX` SEPARATOR ',') AS `SUB_PARTS`,
     `INDEX_TYPE`
     FROM `INFORMATION_SCHEMA`.`STATISTICS`
     WHERE `TABLE_SCHEMA` = '{0}'
     AND `TABLE_NAME` = '{1}'
     AND `INDEX_NAME` <> 'PRIMARY'
     GROUP BY `INDEX_NAME`, `NON_UNIQUE`, `INDEX_TYPE`;";

        protected virtual void GetIndexes(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables,
            Func<string, string, bool> tableFilter)
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
                                var columns = reader.GetValueOrDefault<string>("COLUMNS").Split(',').Select(s => GetColumn(table, s)).ToList();

                                // Reuse an existing index over the same columns, to workaround an EF Core
                                // bug (EF#11846 and #1189).
                                // The columns could be in a different order.
                                var index = table.Indexes.FirstOrDefault(
                                                i => i.Columns
                                                    .OrderBy(c => c.Name)
                                                    .SequenceEqual(columns.OrderBy(c => c.Name))) ??
                                            new DatabaseIndex
                                            {
                                                Table = table,
                                                Name = reader.GetValueOrDefault<string>("INDEX_NAME"),
                                            };

                                index.IsUnique |= !reader.GetValueOrDefault<bool>("NON_UNIQUE");

                                var prefixLengths = reader.GetValueOrDefault<string>("SUB_PARTS")
                                    .Split(',')
                                    .Select(int.Parse)
                                    .ToArray();

                                var hasPrefixLengths = prefixLengths.Any(n => n > 0);
                                if (hasPrefixLengths)
                                {
                                    if (index.Columns.Count <= 0)
                                    {
                                        // If this is the first time an index with this set of columns is being defined,
                                        // then use whatever prefices have been declared.
                                        index[MySqlAnnotationNames.IndexPrefixLength] = prefixLengths;
                                    }
                                    else
                                    {
                                        // Use no prefix length at all or the highest prefix length for a given column
                                        // from all indexes with the same set of columns.
                                        var existingPrefixLengths = (int[])index[MySqlAnnotationNames.IndexPrefixLength];

                                        // Bring the prefix length in the same column order used for the already
                                        // existing prefix lengths from a previous index with the same set of columns.
                                        var newPrefixLengths = index.Columns
                                            .Select(indexColumn => columns.IndexOf(indexColumn))
                                            .Select(
                                                i => i < prefixLengths.Length
                                                    ? prefixLengths[i]
                                                    : 0)
                                            .Zip(
                                                existingPrefixLengths, (l, r) => l == 0 || r == 0
                                                    ? 0
                                                    : Math.Max(l, r))
                                            .ToArray();

                                        index[MySqlAnnotationNames.IndexPrefixLength] = newPrefixLengths.Any(p => p > 0)
                                            ? newPrefixLengths
                                            : null;
                                    }
                                }
                                else
                                {
                                    // If any index (with the same columns) is defined without index prefices at all,
                                    // then don't use any prefices.
                                    index[MySqlAnnotationNames.IndexPrefixLength] = null;
                                }

                                var indexType = reader.GetValueOrDefault<string>("INDEX_TYPE");

                                if (string.Equals(indexType, "spatial", StringComparison.OrdinalIgnoreCase))
                                {
                                    index[MySqlAnnotationNames.SpatialIndex] = true;
                                }

                                if (string.Equals(indexType, "fulltext", StringComparison.OrdinalIgnoreCase))
                                {
                                    index[MySqlAnnotationNames.FullTextIndex] = true;
                                }

                                if (index.Columns.Count <= 0)
                                {
                                    foreach (var column in columns)
                                    {
                                        index.Columns.Add(column);
                                    }

                                    table.Indexes.Add(index);
                                }
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

        protected virtual void GetConstraints(
            DbConnection connection,
            IReadOnlyList<DatabaseTable> tables)
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
                            if (referencedTable == null)
                            {
                                // On operation systems with insensitive file name handling, the saved reference table name might have a
                                // different casing than the actual table name. (#1017)
                                // In the unlikely event that there are multiple tables with the same spelling, differing only in casing,
                                // we can't be certain which is the right match, so rather fail to be safe.
                                referencedTable = tables.Single(t => string.Equals(t.Name, referencedTableName, StringComparison.OrdinalIgnoreCase));
                            }
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

        protected virtual ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
            => onDeleteAction.ToUpperInvariant() switch
            {
                "RESTRICT" => ReferentialAction.Restrict,
                "CASCADE" => ReferentialAction.Cascade,
                "SET NULL" => ReferentialAction.SetNull,
                "NO ACTION" => ReferentialAction.NoAction,
                _ => null
            };

        private DatabaseColumn GetColumn(DatabaseTable table, string columnName)
            => FindColumn(table, columnName) ??
               throw new InvalidOperationException($"Could not find column '{columnName}' in table '{table.Name}'.");

        private DatabaseColumn FindColumn(DatabaseTable table, string columnName)
            => table.Columns.SingleOrDefault(c => string.Equals(c.Name, columnName, StringComparison.Ordinal)) ??
               table.Columns.SingleOrDefault(c => string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase));
    }
}
