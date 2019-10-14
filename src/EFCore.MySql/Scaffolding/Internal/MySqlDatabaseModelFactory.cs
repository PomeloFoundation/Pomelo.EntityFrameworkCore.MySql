// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    public class MySqlDatabaseModelFactory : DatabaseModelFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMySqlOptions _options;

        public MySqlDatabaseModelFactory(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
            IServiceProvider serviceProvider,
            IMySqlOptions options)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(options, nameof(options));

            using (var connection = new MySqlConnection(connectionString))
            {
                return Create(connection, options);
            }
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

                var schemaList = options.Schemas.ToList();
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
            optionsBuilder.UseMySql(connection);

            _options.Initialize(optionsBuilder.Options);
            MySqlConnectionInfo.SetServerVersion((MySqlConnection)connection, _serviceProvider);
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

        private const string GetTablesQuery = @"SHOW FULL TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

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
                        var table = new DatabaseTable
                        {
                            Schema = null,
                            Name = reader.GetString(0).Replace("`", "")
                        };

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

        private const string GetColumnsQuery = @"SHOW COLUMNS FROM `{0}`";

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
                            var extra = reader.GetString(5);
                            ValueGenerated valueGenerated;
                            if (extra.IndexOf("auto_increment", StringComparison.Ordinal) >= 0)
                            {
                                valueGenerated = ValueGenerated.OnAdd;
                            }
                            else if (extra.IndexOf("on update", StringComparison.Ordinal) >= 0)
                            {
                                if (reader[4] != DBNull.Value && extra.IndexOf(reader[4].ToString(), StringComparison.Ordinal) > 0)
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
                                valueGenerated = ValueGenerated.Never;
                            }


                            var column = new DatabaseColumn
                            {
                                Table = table,
                                Name = reader.GetString(0),
                                StoreType = Regex.Replace(reader.GetString(1), @"(?<=int)\(\d+\)(?=\sunsigned)",
                                    string.Empty),
                                IsNullable = reader.GetString(2) == "YES",
                                DefaultValueSql = reader[4] == DBNull.Value
                                    ? null
                                    : '\'' + ParseToMySqlString(reader[4].ToString()) + '\'',
                                ValueGenerated = valueGenerated
                            };
                            table.Columns.Add(column);
                        }
                    }
                }
            }
        }

        private string ParseToMySqlString(string str)
        {
            // Pending the MySqlConnector implement MySqlCommandBuilder class
            return str
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"");
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
                                var index = new DatabasePrimaryKey
                                {
                                    Table = table,
                                    Name = reader.GetString(0)
                                };

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
                                var index = new DatabaseIndex
                                {
                                    Table = table,
                                    Name = reader.GetString(0),
                                    IsUnique = reader.GetFieldType(1) == typeof(string)
                                        ? reader.GetString(1) == "1"
                                        : !reader.GetBoolean(1)
                                };

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
                            var referencedTable = tables.Where(t => t.Name == referencedTableName).FirstOrDefault();
                            if (referencedTable != null)
                            {
                                var fkInfo = new DatabaseForeignKey
                                {
                                    Name = reader.GetString(0),
                                    OnDelete = ConvertToReferentialAction(reader.GetString(4)),
                                    Table = table,
                                    PrincipalTable = referencedTable
                                };
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
