// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MySql.Data.MySqlClient;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Internal
{
    public class MySqlOptions : IMySqlOptions
    {

        private MySqlOptionsExtension _relationalOptions;

        private readonly Lazy<MySqlConnectionSettings> _lazyConnectionSettings;

        public MySqlOptions()
        {
            _lazyConnectionSettings = new Lazy<MySqlConnectionSettings>(() =>
            {
                if (_relationalOptions.Connection != null)
                    return MySqlConnectionSettings.GetSettings(_relationalOptions.Connection);
                return MySqlConnectionSettings.GetSettings(_relationalOptions.ConnectionString);
            }, LazyThreadSafetyMode.PublicationOnly);
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            _relationalOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

        }

        public virtual void Validate(IDbContextOptions options)
        {
            if (_relationalOptions.ConnectionString == null && _relationalOptions.Connection == null)
                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
        }

        public virtual MySqlConnectionSettings ConnectionSettings => _lazyConnectionSettings.Value;

        public virtual string GetCreateTable(ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            if (_relationalOptions.Connection != null)
                return GetCreateTable(_relationalOptions.Connection, sqlGenerationHelper, table, schema);
            return GetCreateTable(_relationalOptions.ConnectionString, sqlGenerationHelper, table, schema);
        }

        private static string GetCreateTable(string connectionString, ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteCreateTable(connection, sqlGenerationHelper, table, schema);
            }
        }

        private static string GetCreateTable(DbConnection connection, ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            var opened = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                opened = true;
            }
            try
            {
                return ExecuteCreateTable(connection, sqlGenerationHelper, table, schema);
            }
            finally
            {
                if (opened)
                    connection.Close();
            }
        }

        private static string ExecuteCreateTable(DbConnection connection, ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SHOW CREATE TABLE {sqlGenerationHelper.DelimitIdentifier(table, schema)}";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetFieldValue<string>(1);
                }
            }
            return null;
        }

    }
}
