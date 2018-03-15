// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace EFCore.MySql.Scaffolding.Internal
{
    public class MySqlDatabaseModelFactory : IDatabaseModelFactory
    {
        private MySqlConnection _connection;
        private DatabaseModel _databaseModel;
        private Dictionary<string, DatabaseTable> _tables;

        public MySqlDatabaseModelFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            Logger = loggerFactory.CreateCommandsLogger();
        }

        protected virtual ILogger Logger { get; }

        private void ResetState()
        {
            _connection = null;
            _databaseModel = new DatabaseModel();
            _tables = new Dictionary<string, DatabaseTable>();
            new Dictionary<string, DatabaseColumn>(StringComparer.OrdinalIgnoreCase);
        }

        public DatabaseModel Create(string connectionString, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                return Create(connection, tables, schemas);
            }
        }

        public DatabaseModel Create(DbConnection connection, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            ResetState();

            _connection = (MySqlConnection)connection;

            var connectionStartedOpen = _connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                _connection.Open();
            }

            try
            {
                _databaseModel.DatabaseName = _connection.Database;
                _databaseModel.DefaultSchema = null;

                GetTables(tables.ToList());
                GetColumns();
                GetPrimaryKeys();
                GetIndexes();
                GetConstraints();

                return _databaseModel;
            }
            finally
            {
                if (!connectionStartedOpen)
                {
                    _connection.Close();
                }
            }
        }

        private const string GetTablesQuery = @"SHOW FULL TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

        private void GetTables(List<string> allowedTables)
        {
            using (var command = new MySqlCommand(GetTablesQuery, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var table = new DatabaseTable
                    {
                        Schema = null,
                        Name = reader.GetString(0)
                    };

                    if (allowedTables.Contains(table.Name))
                    {
                        _databaseModel.Tables.Add(table);
                        _tables[table.Name] = table;
                    }
                }
            }
        }

        private const string GetColumnsQuery = @"SHOW COLUMNS FROM {0}";

        private void GetColumns()
        {
            foreach (var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetColumnsQuery, x.Key), _connection))
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
                            Table = x.Value,
                            Name = reader.GetString(0),
                            StoreType = Regex.Replace(reader.GetString(1), @"(?<=int)\(\d+\)(?=\sunsigned)",
                                string.Empty),
                            IsNullable = reader.GetString(2) == "YES",
                            DefaultValueSql = reader[4] == DBNull.Value
                                ? null
                                : '\'' + ParseToMySqlString(reader[4].ToString()) + '\'',
                            ValueGenerated = valueGenerated
                        };
                        x.Value.Columns.Add(column);
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
        private void GetPrimaryKeys()
        {
            foreach (var x in _tables)
            {
                using (var command =
                    new MySqlCommand(string.Format(GetPrimaryQuery, _connection.Database, x.Key.Replace("`", "")),
                        _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            var index = new DatabasePrimaryKey
                            {
                                Table = x.Value,
                                Name = reader.GetString(0)
                            };

                            foreach (var column in reader.GetString(2).Split(','))
                            {
                                index.Columns.Add(x.Value.Columns.Single(y => y.Name == column));
                            }

                            x.Value.PrimaryKey = index;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Error assigning primary key for {table}.", x.Key);
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
        private void GetIndexes()
        {
            foreach (var x in _tables)
            {
                using (var command =
                    new MySqlCommand(string.Format(GetIndexesQuery, _connection.Database, x.Key.Replace("`", "")),
                        _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            var index = new DatabaseIndex
                            {
                                Table = x.Value,
                                Name = reader.GetString(0),
                                IsUnique = reader.GetFieldType(1) == typeof(string)
                                    ? reader.GetString(1) == "1"
                                    : !reader.GetBoolean(1)
                            };

                            foreach (var column in reader.GetString(2).Split(','))
                            {
                                index.Columns.Add(x.Value.Columns.Single(y => y.Name == column));
                            }

                            x.Value.Indexes.Add(index);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Error assigning index for {table}.", x.Key);
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

        private void GetConstraints()
        {
            foreach (var x in _tables)
            {
                using (var command =
                    new MySqlCommand(string.Format(GetConstraintsQuery, _connection.Database, x.Key.Replace("`", "")),
                        _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (_tables.ContainsKey($"`{reader.GetString(2)}`"))
                        {
                            var fkInfo = new DatabaseForeignKey
                            {
                                Name = reader.GetString(0),
                                OnDelete = ConvertToReferentialAction(reader.GetString(4)),
                                Table = x.Value,
                                PrincipalTable = _tables[$"`{reader.GetString(2)}`"]
                            };
                            foreach (var pair in reader.GetString(3).Split(','))
                            {
                                fkInfo.Columns.Add(x.Value.Columns.Single(y =>
                                    string.Equals(y.Name, pair.Split('|')[0], StringComparison.OrdinalIgnoreCase)));
                                fkInfo.PrincipalColumns.Add(fkInfo.PrincipalTable.Columns.Single(y =>
                                    string.Equals(y.Name, pair.Split('|')[1], StringComparison.OrdinalIgnoreCase)));
                            }

                            x.Value.ForeignKeys.Add(fkInfo);
                        }
                        else
                        {
                            Logger.LogWarning($"Referenced table `{reader.GetString(2)}` is not in dictionary.");
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
