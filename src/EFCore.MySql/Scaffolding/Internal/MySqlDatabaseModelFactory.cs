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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    public class MySqlDatabaseModelFactory : DatabaseModelFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly IMySqlOptions _options;

        protected virtual MySqlScaffoldingConnectionSettings Settings { get; set; }

        public MySqlDatabaseModelFactory(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] IMySqlOptions options)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(typeMappingSource, nameof(typeMappingSource));
            Check.NotNull(options, nameof(options));

            _logger = logger;
            _typeMappingSource = typeMappingSource;
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

            var connectionStartedOpen = connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                connection.Open();
            }

            try
            {
                return GetDatabase(connection, options);
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

        private const string GetDatabaseSettings = @"SELECT
	`DEFAULT_CHARACTER_SET_NAME`,
    `DEFAULT_COLLATION_NAME`
FROM
	`INFORMATION_SCHEMA`.`SCHEMATA`
WHERE
	`SCHEMA_NAME` = SCHEMA()";

        protected virtual DatabaseModel GetDatabase(DbConnection connection, DatabaseModelFactoryOptions options)
        {
            var databaseModel = new DatabaseModel
            {
                DatabaseName = connection.Database,
                DefaultSchema = GetDefaultSchema(connection)
            };

            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetDatabaseSettings;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var defaultCharSet = reader.GetValueOrDefault<string>("DEFAULT_CHARACTER_SET_NAME");
                        var defaultCollation = reader.GetValueOrDefault<string>("DEFAULT_COLLATION_NAME");

                        databaseModel[MySqlAnnotationNames.CharSet] = Settings.CharSet
                            ? defaultCharSet
                            : null;
                        databaseModel.Collation = Settings.Collation
                            ? defaultCollation
                            : null;
                    }
                }
            }

            var schemaList = Enumerable.Empty<string>().ToList();
            var tableList = options.Tables.ToList();
            var tableFilter = GenerateTableFilter(tableList, schemaList);

            var tables = GetTables(connection, tableFilter, (string)databaseModel[MySqlAnnotationNames.CharSet], databaseModel.Collation);
            foreach (var table in tables)
            {
                table.Database = databaseModel;
                databaseModel.Tables.Add(table);
            }

            return databaseModel;
        }

        protected virtual string GetDefaultSchema(DbConnection connection)
            => null;

        protected virtual Func<string, string, bool> GenerateTableFilter(
            IReadOnlyList<string> tables,
            IReadOnlyList<string> schemas)
            => tables.Count > 0 ? (s, t) => tables.Contains(t) : (Func<string, string, bool>)null;

        private const string GetTablesQuery = @"SELECT
    `t`.`TABLE_NAME`,
    `t`.`TABLE_TYPE`,
    IF(`t`.`TABLE_COMMENT` = 'VIEW' AND `t`.`TABLE_TYPE` = 'VIEW', '', `t`.`TABLE_COMMENT`) AS `TABLE_COMMENT`,
    `ccsa`.`CHARACTER_SET_NAME` as `TABLE_CHARACTER_SET`,
    `t`.`TABLE_COLLATION`
FROM
    `INFORMATION_SCHEMA`.`TABLES` as `t`
LEFT JOIN
	`INFORMATION_SCHEMA`.`COLLATION_CHARACTER_SET_APPLICABILITY` as `ccsa` ON `ccsa`.`COLLATION_NAME` = `t`.`TABLE_COLLATION`
WHERE
    `TABLE_SCHEMA` = SCHEMA()
AND
    `TABLE_TYPE` IN ('BASE TABLE', 'VIEW');";

        protected virtual IEnumerable<DatabaseTable> GetTables(
            DbConnection connection,
            Func<string, string, bool> filter,
            string defaultCharSet,
            string defaultCollation)
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
                        var charset = reader.GetValueOrDefault<string>("TABLE_CHARACTER_SET");
                        var collation = reader.GetValueOrDefault<string>("TABLE_COLLATION");

                        var table = string.Equals(type, "base table", StringComparison.OrdinalIgnoreCase)
                            ? new DatabaseTable()
                            : new DatabaseView();

                        table.Schema = null;
                        table.Name = name;
                        table.Comment = string.IsNullOrEmpty(comment) ? null : comment;
                        table[MySqlAnnotationNames.CharSet] = Settings.CharSet &&
                                                              charset != defaultCharSet
                            ? charset
                            : null;
                        table[RelationalAnnotationNames.Collation] = Settings.Collation &&
                                                                     collation != defaultCollation
                            ? collation
                            : null;

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
                GetColumns(connection, tables, filter, defaultCharSet, defaultCollation);
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
    `EXTRA`/*!50706 ,
    `GENERATION_EXPRESSION` */ /*M!100200 ,
    `GENERATION_EXPRESSION` */ /*!80003 ,
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
            Func<string, string, bool> tableFilter,
            string defaultCharSet,
            string defaultCollation)
        {
            foreach (var table in tables)
            {
                var columnTypeOverrides = GetColumnTypeOverrides(connection, table);

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
                            var columnType = reader.GetValueOrDefault<string>("COLUMN_TYPE");
                            var extra = reader.GetValueOrDefault<string>("EXTRA");
                            var comment = reader.GetValueOrDefault<string>("COLUMN_COMMENT");

                            // Generated colums are not supported on every MySQL/MariaDB version.
                            var generation = reader.HasName("GENERATION_EXPRESSION")
                                ? reader.GetValueOrDefault<string>("GENERATION_EXPRESSION").NullIfEmpty()
                                : null;

                            // MariaDB does not support SRID column restrictions.
                            var srid = reader.HasName("SRS_ID")
                                ? reader.GetValueOrDefault<uint?>("SRS_ID")
                                : null;

                            var isStored = generation != null
                                ? (bool?)extra.Contains("stored generated", StringComparison.OrdinalIgnoreCase)
                                : null;

                            // Cleanup the column type, because it might contain trailing C style comments on MariaDB, like the following,
                            // if an explicit cast is being done in the SELECT of a VIEW:
                            //     datetime /* mariadb-5.3 */
                            columnType = Regex.Replace(columnType, @"\s*/\*(?:.*?)\*/\s*$", string.Empty, RegexOptions.Singleline);

                            // Override this column's type, if we detected earlier that this column should actually by added to the model
                            // with a different type than the one returned by INFORMATION_SCHEMA.COLUMNS.
                            // This ensures, that e.g. the `json` alias for the `longtext` type for MariaDB databases will be added to the
                            // model as `json` instead of as `longtext`.
                            columnType = columnTypeOverrides.TryGetValue(name, out var columnTypeOverride)
                                ? columnTypeOverride((dataType: dataType, charset: charset, collation: collation))
                                : columnType;

                            // MySQL enforces the `utf8mb4` charset and `utf8mb4_bin` collation for `json` columns and MariaDB will use them
                            // automatically for `json` columns as well.
                            // Both will refuse explicit specifications of other charsets/collations, even though `json` is just an alias
                            // for `longtext` for MariaDB and setting `longtext` to other charsets/collations works fine.
                            // We therefore do not scaffold thouse charsets/collations in the first place, so that users don't get confused.
                            if (columnType == "json")
                            {
                                charset = null;
                                collation = null;
                            }

                            if (generation is not null)
                            {
                                // MySQL saves the generation expression with enclosing parenthesis, while MariaDB doesn't.
                                generation = _options.ServerVersion.Supports.ParenthesisEnclosedGeneratedColumnExpressions
                                    ? Regex.Replace(generation, @"^\((.*)\)$", "$1", RegexOptions.Singleline)
                                    : generation;

                                // MySQL 8 contains a regression bug, that escapes the outer quotes of a string in a generated expression.
                                generation = _options.ServerVersion.Supports.MySqlBug104294Workaround
                                    ? generation.Replace(@"\'", @"'")
                                    : generation;
                            }

                            var isDefaultValueSqlFunction = IsDefaultValueSqlFunction(defaultValue, dataType);
                            var isDefaultValueExpression = false;

                            if (defaultValue != null)
                            {
                                // MySQL 8.0.13+ fully supports complex default value expressions.
                                isDefaultValueExpression = extra.Contains("DEFAULT_GENERATED", StringComparison.OrdinalIgnoreCase) &&
                                                           !IsSimpleNumericDefaultValue(defaultValue);

                                // MariaDB uses a slightly different syntax.
                                defaultValue = _options.ServerVersion.Supports.AlternativeDefaultExpression
                                    ? ConvertDefaultValueFromMariaDbToMySql(defaultValue, out isDefaultValueExpression)
                                    : defaultValue;

                                defaultValue = generation == null
                                    ? FilterClrDefaults(
                                        dataType,
                                        nullable,
                                        defaultValue)
                                    : null;
                            }

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
                                    extra.IndexOf("CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    valueGenerated = ValueGenerated.OnAddOrUpdate;
                                }
                                else
                                {
                                    valueGenerated = ValueGenerated.OnUpdate;
                                }
                            }
                            else
                            {
                                // Using `null` results in `ValueGeneratedNever()` being output for primary keys without
                                // auto increment as desired, while explicitly using `ValueGenerated.Never` results in
                                // no value generated output at all.
                                valueGenerated = null;
                            }

                            var column = new DatabaseColumn
                            {
                                Table = table,
                                Name = name,
                                StoreType = columnType,
                                IsNullable = nullable,
                                DefaultValueSql = CreateDefaultValueString(defaultValue, dataType, isDefaultValueSqlFunction, isDefaultValueExpression),
                                ComputedColumnSql = generation,
                                IsStored = isStored,
                                ValueGenerated = valueGenerated,
                                Comment = string.IsNullOrEmpty(comment)
                                    ? null
                                    : comment,
                                [MySqlAnnotationNames.CharSet] = Settings.CharSet &&
                                                                 charset != (table[MySqlAnnotationNames.CharSet] as string ?? defaultCharSet)
                                    ? charset
                                    : null,
                                Collation = Settings.Collation &&
                                            collation != (table[RelationalAnnotationNames.Collation] as string ?? defaultCollation)
                                    ? collation
                                    : null,
                                [MySqlAnnotationNames.SpatialReferenceSystemId] = srid.HasValue
                                    ? (int?)(int)srid.Value
                                    : null,
                            };

                            table.Columns.Add(column);
                        }
                    }
                }
            }
        }

        private bool IsDefaultValueSqlFunction(string defaultValue, string dataType)
        {
            if (defaultValue == null)
            {
                return false;
            }

            // MySQL uses `CURRENT_TIMESTAMP` (or `CURRENT_TIMESTAMP(6)`),
            // while MariaDB uses `current_timestamp()` (or `current_timestamp(6)`).
            // MariaDB also allows the usage of `curdate()` as a default for datetime or timestamp columns, but this is handled by the next
            // section.
            if ((string.Equals(dataType, "timestamp", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(dataType, "datetime", StringComparison.OrdinalIgnoreCase)) &&
                Regex.IsMatch(defaultValue, @"^CURRENT_TIMESTAMP(?:\(\d*\))?$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // If SQL functions are used as a default value in MariaDB, they will always end in a parenthesis pair.
            if (_options.ServerVersion.Supports.AlternativeDefaultExpression &&
                defaultValue.EndsWith("()", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// MariaDB 10.2.7+ implements default values differently from MySQL, to support their own default expression
        /// syntax. We convert their column values to MySQL compatible syntax here.
        /// See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/994#issuecomment-568271740
        /// for tables with differences.
        /// </summary>
        protected virtual string ConvertDefaultValueFromMariaDbToMySql([NotNull] string defaultValue, out bool isDefaultValueExpression)
        {
            isDefaultValueExpression = false;

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

            isDefaultValueExpression = !IsSimpleNumericDefaultValue(defaultValue);

            return defaultValue;
        }

        private static bool IsSimpleNumericDefaultValue(string defaultValue)
            => Regex.IsMatch(defaultValue, @"^\d+(?:\.\d+)?$");

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

        protected virtual string CreateDefaultValueString(
            string defaultValue, string dataType, bool isSqlFunction, bool isDefaultValueExpression)
        {
            if (defaultValue == null)
            {
                return null;
            }

            if (isSqlFunction ||
                isDefaultValueExpression)
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

                                var firstKeyColumn = key.Columns[0];

                                if (key.Columns.Count == 1 &&
                                    firstKeyColumn.ValueGenerated == null &&
                                    (firstKeyColumn.DefaultValueSql == null ||
                                     string.Equals(firstKeyColumn.DefaultValueSql, "uuid()", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(firstKeyColumn.DefaultValueSql, "uuid_to_bin(uuid())", StringComparison.OrdinalIgnoreCase)) &&
                                    _typeMappingSource.FindMapping(firstKeyColumn.StoreType) is MySqlGuidTypeMapping)
                                {
                                    firstKeyColumn.ValueGenerated = ValueGenerated.OnAdd;
                                    firstKeyColumn.DefaultValueSql = null;
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

        private const string GetCreateTableStatementQuery = @"SHOW CREATE TABLE `{0}`.`{1}`;";

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

                //
                // Post-process the full-text indices, because we cannot open to data readers over the same connection at the same time.
                //

                var fullTextIndexes = table.Indexes
                    .Where(i => ((bool?) i[MySqlAnnotationNames.FullTextIndex]).GetValueOrDefault())
                    .ToList();

                if (fullTextIndexes.Any())
                {
                    var createTableQuery = GetCreateTableQuery(connection, table);
                    var fullTextParsers = GetFullTextParsers(createTableQuery);

                    foreach (var fullTextIndex in fullTextIndexes)
                    {
                        if (fullTextParsers.TryGetValue(fullTextIndex.Name, out var fullTextParser))
                        {
                            fullTextIndex[MySqlAnnotationNames.FullTextParser] = fullTextParser;
                        }
                    }
                }
            }
        }

        private static Dictionary<string, string> GetFullTextParsers(string createTableQuery)
            => Regex.Matches(
                    createTableQuery,
                    @"\s*FULLTEXT\s+(?:INDEX|KEY)\s+(?:`(?<IndexName>(?:[^`]|``)+)`|(?<IndexName>\S+)).*WITH\s+PARSER\s+(?:`(?<FullTextParser>(?:[^`]|``)+)`|(?<FullTextParser>\S+))",
                    RegexOptions.IgnoreCase)
                .Where(m => m.Success)
                .ToDictionary(
                    m => m.Groups["IndexName"].Value.Replace("``", "`"),
                    m => m.Groups["FullTextParser"].Value.Replace("``", "`"));

        private static string GetCreateTableQuery(DbConnection connection, DatabaseTable table)
        {
            using var command = connection.CreateCommand();
            command.CommandText = string.Format(GetCreateTableStatementQuery, connection.Database, table.Name);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetValueOrDefault<string>("Create Table");
            }

            throw new InvalidOperationException("The statement 'SHOW CREATE TABLE' did not return any results.");
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
                                referencedTable = tables.SingleOrDefault(t => string.Equals(t.Name, referencedTableName, StringComparison.OrdinalIgnoreCase));
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

        private const string GetCheckConstraintsQuery = @"SELECT `c`.`CONSTRAINT_NAME`, `c`.`CHECK_CLAUSE`
FROM `INFORMATION_SCHEMA`.`CHECK_CONSTRAINTS` as `c`
INNER JOIN `INFORMATION_SCHEMA`.`TABLE_CONSTRAINTS` as `t` on `t`.`CONSTRAINT_CATALOG` = `c`.`CONSTRAINT_CATALOG` and `t`.`CONSTRAINT_SCHEMA` = `c`.`CONSTRAINT_SCHEMA` and `t`.`CONSTRAINT_NAME` = `c`.`CONSTRAINT_NAME`
WHERE `t`.`TABLE_SCHEMA` = '{0}' AND `t`.`CONSTRAINT_SCHEMA` = `t`.`TABLE_SCHEMA` AND `t`.`TABLE_NAME` = '{1}';";

        protected virtual Dictionary<string, Func<(string dataType, string charset, string collation), string>> GetColumnTypeOverrides(
            DbConnection connection,
            DatabaseTable table)
        {
            var columnTypeOverrides = new Dictionary<string, Func<(string dataType, string charset, string collation), string>>();

            // For MariaDB. the `json` type is just an alias for `longtext`.
            // In newer versions however, it adds a json_valid(`columnName`) check constraint when a column was created with the type
            // `json`, which we can use as a very strong heuristic that a `longtext` column is being used as a `json` column.
            if (_options.ServerVersion.Supports.IdentifyJsonColumsByCheckConstraints &&
                _options.ServerVersion.Supports.InformationSchemaCheckConstraintsTable)
            {
                using var command = connection.CreateCommand();
                command.CommandText = string.Format(GetCheckConstraintsQuery, connection.Database, table.Name);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var constraintName = reader.GetValueOrDefault<string>("CONSTRAINT_NAME");
                    var checkClause = reader.GetValueOrDefault<string>("CHECK_CLAUSE");

                    var match = Regex.Match(
                        checkClause,
                        @"json_valid\s*\(\s*`(?<columnName>(?:[^`]|``)+)`\s*\)",
                        RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (match.Success)
                    {
                        columnTypeOverrides.TryAdd(
                            match.Groups["columnName"].Value,
                            t => t.charset == "utf8mb4" &&
                                 t.collation == "utf8mb4_bin"
                                ? "json"
                                : t.dataType);
                    }
                }
            }

            return columnTypeOverrides;
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
