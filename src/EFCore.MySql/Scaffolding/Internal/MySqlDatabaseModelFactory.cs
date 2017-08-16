using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class MySqlDatabaseModelFactory : IDatabaseModelFactory
    {
        MySqlConnection _connection;
        TableSelectionSet _tableSelectionSet;
        DatabaseModel _databaseModel;
        Dictionary<string, DatabaseTable> _tables;
        Dictionary<string, DatabaseColumn> _tableColumns;

        static string TableKey(DatabaseTable table) => TableKey(table.Name, table.Schema);
        static string TableKey(string name, string schema) => $"`{name}`";
        static string ColumnKey(DatabaseTable table, string columnName) => $"{TableKey(table)}.`{columnName}`";


        public MySqlDatabaseModelFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            Logger = loggerFactory.CreateCommandsLogger();
        }

        public virtual ILogger Logger { get; }

        void ResetState()
        {
            _connection = null;
            _tableSelectionSet = null;
            _databaseModel = new DatabaseModel();
            _tables = new Dictionary<string, DatabaseTable>();
            _tableColumns = new Dictionary<string, DatabaseColumn>(StringComparer.OrdinalIgnoreCase);
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
            return Create(connection, new TableSelectionSet(tables, schemas));
        }

        public DatabaseModel Create(DbConnection connection, TableSelectionSet tableSelectionSet)
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
                _tableSelectionSet = tableSelectionSet;

                _databaseModel.DatabaseName = _connection.Database;
                _databaseModel.DefaultSchema = null;

                GetTables();
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

        const string GetTablesQuery = @"SHOW FULL TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

        void GetTables()
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

                    if (_tableSelectionSet.Allows(table.Schema, table.Name))
                    {
                        _databaseModel.Tables.Add(table);
                        _tables[TableKey(table)] = table;
                    }
                }
            }
        }

        const string GetColumnsQuery = @"SHOW COLUMNS FROM {0}";

        void GetColumns()
        {
            foreach (var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetColumnsQuery, x.Key), _connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var column = new DatabaseColumn
                        {
                            Table = x.Value,
                            Name = reader.GetString(0),
                            StoreType = reader.GetString(1),
                            IsNullable = reader.GetString(2) == "YES",
                            DefaultValueSql = reader[4].ToString() == "" ? null : reader[4].ToString(),
                        };
                        x.Value.Columns.Add(column);
                    }
            }
        }

        const string GetPrimaryQuery = @"SELECT `INDEX_NAME`, 
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
        void GetPrimaryKeys()
        {
            foreach (var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetPrimaryQuery, _connection.Database, x.Key.Replace("`", "")), _connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        try
                        {
                            var index = new DatabasePrimaryKey
                            {
                                Table = x.Value,
                                Name = reader.GetString(0),
                            };

                            foreach (var column in reader.GetString(2).Split(','))
                            {
                                index.Columns.Add(x.Value.Columns.Single(y => y.Name == column));
                            }

                            x.Value.PrimaryKey = index;
                        }
                        catch { }
                    }
            }
        }

        const string GetIndexesQuery = @"SELECT `INDEX_NAME`, 
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
        void GetIndexes()
        {
            foreach (var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetIndexesQuery, _connection.Database, x.Key.Replace("`", "")), _connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        try
                        {
                            var index = new DatabaseIndex
                            {
                                Table = x.Value,
                                Name = reader.GetString(0),
                                IsUnique = !reader.GetBoolean(1),
                            };

                            foreach (var column in reader.GetString(2).Split(','))
                            {
                                index.Columns.Add(x.Value.Columns.Single(y => y.Name == column));
                            }

                            x.Value.Indexes.Add(index);
                        }
                        catch { }
                    }
            }
        }

        const string GetConstraintsQuery = @"SELECT `CONSTRAINT_SCHEMA`, 
 	`CONSTRAINT_NAME`, 
 	`TABLE_NAME`, 
 	`COLUMN_NAME`, 
 	`REFERENCED_TABLE_NAME`, 
 	`REFERENCED_COLUMN_NAME`, 
 	(SELECT `DELETE_RULE` FROM `INFORMATION_SCHEMA`.`REFERENTIAL_CONSTRAINTS` WHERE `REFERENTIAL_CONSTRAINTS`.`CONSTRAINT_NAME` = `KEY_COLUMN_USAGE`.`CONSTRAINT_NAME`) AS `DELETE_RULE`
 FROM `INFORMATION_SCHEMA`.`KEY_COLUMN_USAGE` 
 WHERE `CONSTRAINT_SCHEMA` = '{0}' 
 		AND `TABLE_SCHEMA` = '{0}' 
 		AND `TABLE_NAME` = '{1}' 
 		AND `CONSTRAINT_NAME` <> 'PRIMARY'
         AND `REFERENCED_TABLE_NAME` IS NOT NULL";

        void GetConstraints()
        {
            foreach (var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetConstraintsQuery, _connection.Database, x.Key.Replace("`", "")), _connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        if (_tables.ContainsKey($"`{ reader.GetString(4) }`"))
                        {
                            var fkInfo = new DatabaseForeignKey
                            {
                                Name = reader.GetString(1),
                                OnDelete = ConvertToReferentialAction(reader.GetString(6)),
                                Table = x.Value,
                                PrincipalTable = _tables[$"`{ reader.GetString(4) }`"]
                            };
                            fkInfo.Columns.Add(x.Value.Columns.Single(y => y.Name == reader.GetString(3)));
                            x.Value.ForeignKeys.Add(fkInfo);
                        }
                        else
                        {
                            Logger.LogWarning($"Referenced table `{ reader.GetString(4) }` is not in dictionary.");
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
