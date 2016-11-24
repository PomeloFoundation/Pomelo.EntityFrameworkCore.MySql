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
    public class MySqlDatabaseModelFactory : IInternalDatabaseModelFactory
    {
        MySqlConnection _connection;
        TableSelectionSet _tableSelectionSet;
        DatabaseModel _databaseModel;
        Dictionary<string, TableModel> _tables;
        Dictionary<string, ColumnModel> _tableColumns;

        static string TableKey(TableModel table) => TableKey(table.Name, table.SchemaName);
        static string TableKey(string name, string schema) => $"`{name}`";
        static string ColumnKey(TableModel table, string columnName) => $"{TableKey(table)}.`{columnName}`";
        

        public MySqlDatabaseModelFactory(/* [NotNull] */ ILoggerFactory loggerFactory)
        {
            // Check.NotNull(loggerFactory, nameof(loggerFactory));

            Logger = loggerFactory.CreateCommandsLogger();
        }

        public virtual ILogger Logger { get; }

        void ResetState()
        {
            _connection = null;
            _tableSelectionSet = null;
            _databaseModel = new DatabaseModel();
            _tables = new Dictionary<string, TableModel>();
            _tableColumns = new Dictionary<string, ColumnModel>(StringComparer.OrdinalIgnoreCase);
        }

        public DatabaseModel Create(string connectionString, TableSelectionSet tableSelectionSet)
        {
            // Check.NotEmpty(connectionString, nameof(connectionString));
            // Check.NotNull(tableSelectionSet, nameof(tableSelectionSet));

            using (var connection = new MySqlConnection(connectionString))
            {
                return Create(connection, tableSelectionSet);
            }
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
                _databaseModel.DefaultSchemaName = null;
                
                GetTables();
                GetColumns();
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

        const string GetTablesQuery = @"SHOW TABLES";

        void GetTables()
        {
            using (var command = new MySqlCommand(GetTablesQuery, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var table = new TableModel
                    {
                        SchemaName = null,
                        Name = reader.GetString(0)
                    };
                    
                    if (_tableSelectionSet.Allows(table.SchemaName, table.Name))
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
            foreach(var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetColumnsQuery, x.Key), _connection))
                using (var reader = command.ExecuteReader())
                while (reader.Read())
                {
                    var column = new ColumnModel
                    {
                        Table = x.Value,
                        PrimaryKeyOrdinal = reader[3].ToString() == "PRI" ? (int?)1 : null,
                        Name = reader.GetString(0),
                        DataType = reader.GetString(1),
                        IsNullable = reader.GetString(2) == "YES",
                        DefaultValue = reader[4].ToString() == "" ? null : reader[4].ToString(),
                    };
                    x.Value.Columns.Add(column);
                }
            }
        }

        const string GetIndexesQuery = @"SHOW INDEX FROM {0} WHERE `KEY_NAME` <> 'PRIMARY'";

        /// <remarks>
        /// Primary keys are handled as in <see cref="GetConstraints"/>, not here
        /// </remarks>
        void GetIndexes()
        {
            foreach(var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetIndexesQuery, x.Key), _connection))
                using (var reader = command.ExecuteReader())
                while (reader.Read())
                {
                    var index = new IndexModel
                    {
                        Table = x.Value,
                        Name = reader.GetString(2),
                        IsUnique = !reader.GetBoolean(1),
                    };
                    index.IndexColumns.Add(new IndexColumnModel { Column = x.Value.Columns.Single(y => y.Name == reader.GetString(4)), Index = index });

                    x.Value.Indexes.Add(index);
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
		AND `CONSTRAINT_NAME` <> 'PRIMARY'";

        void GetConstraints()
        {
            foreach(var x in _tables)
            {
                using (var command = new MySqlCommand(string.Format(GetConstraintsQuery, _connection.Database, x.Key.Replace("`","")), _connection))
                using (var reader = command.ExecuteReader())
                while (reader.Read())
                    {
                        var fkInfo = new ForeignKeyModel
                        {
                            Name = reader.GetString(1),
                            OnDelete = ConvertToReferentialAction(reader.GetString(6)),
                            Table = x.Value,
                            PrincipalTable = _tables[$"`{ reader.GetString(4) }`"]
                        };
                        fkInfo.Columns.Add(new ForeignKeyColumnModel
                        {
                            Column = x.Value.Columns.Single(y => y.Name == reader.GetString(3)),
                            PrincipalColumn = _tables[$"`{ reader.GetString(4) }`"].Columns.Single(y => y.Name == reader.GetString(5))
                        });
                        x.Value.ForeignKeys.Add(fkInfo);
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
