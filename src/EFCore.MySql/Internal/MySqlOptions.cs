// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using EFCore.MySql.Infrastructure.Internal;
using EFCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;

namespace EFCore.MySql.Internal
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
                {
                    return MySqlConnectionSettings.GetSettings(_relationalOptions.Connection);
                }

                return MySqlConnectionSettings.GetSettings(_relationalOptions.ConnectionString);
            }, LazyThreadSafetyMode.PublicationOnly);
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            _relationalOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

            ServerVersion = mySqlOptions.ServerVersion ?? new ServerVersion(null);

        }

        public virtual void Validate(IDbContextOptions options)
        {
            if (_relationalOptions.ConnectionString == null && _relationalOptions.Connection == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
            }

            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

            if (!Equals(ServerVersion, mySqlOptions.ServerVersion ?? new ServerVersion(null)))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.ServerVersion),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        public virtual MySqlConnectionSettings ConnectionSettings => _lazyConnectionSettings.Value;
        public virtual ServerVersion ServerVersion { get; private set; }

        public virtual string GetCreateTable(ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            if (_relationalOptions.Connection != null)
            {
                return GetCreateTable(_relationalOptions.Connection, sqlGenerationHelper, table, schema);
            }

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
                {
                    connection.Close();
                }
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
                    {
                        return reader.GetFieldValue<string>(1);
                    }
                }
            }
            return null;
        }
    }
}
