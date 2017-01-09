// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlConnectionSettings
    {
        private static readonly ConcurrentDictionary<string, MySqlConnectionSettings> Settings
            = new ConcurrentDictionary<string, MySqlConnectionSettings>();

        private static MySqlConnectionStringBuilder _settingsCsb(MySqlConnectionStringBuilder csb)
        {
            return new MySqlConnectionStringBuilder
            {
                Server = csb.Server,
                Port = csb.Port,
                OldGuids = csb.OldGuids,
                TreatTinyAsBoolean = csb.TreatTinyAsBoolean,
            };
        }

        public static MySqlConnectionSettings GetSettings(string connectionString)
        {
            var csb = new MySqlConnectionStringBuilder(connectionString);
            var settingsCsb = _settingsCsb(csb);
            return Settings.GetOrAdd(settingsCsb.ConnectionString, key =>
            {
                csb.Database = "";
                csb.Pooling = false;
                string serverVersion;
                using (var schemalessConnection = new MySqlConnection(csb.ConnectionString))
                {
                    schemalessConnection.Open();
                    serverVersion = schemalessConnection.ServerVersion;
                }
                var version = ServerVersion.ParseVersion(serverVersion);
                return new MySqlConnectionSettings(settingsCsb, version);
            });
        }

        public static MySqlConnectionSettings GetSettings(DbConnection connection)
        {
            var csb = new MySqlConnectionStringBuilder(connection.ConnectionString);
            var settingsCsb = _settingsCsb(csb);
            return Settings.GetOrAdd(settingsCsb.ConnectionString, key =>
            {
                var opened = false;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    opened = true;
                }
                var version = ServerVersion.ParseVersion(connection.ServerVersion);
                var connectionSettings = new MySqlConnectionSettings(settingsCsb, version);
                if (opened)
                    connection.Close();
                return connectionSettings;
            });
        }

        private MySqlConnectionSettings(MySqlConnectionStringBuilder settingsCsb, Version version)
        {
            // Settings from the connection string
            OldGuids = settingsCsb.OldGuids;
            TreatTinyAsBoolean = settingsCsb.TreatTinyAsBoolean;

            // Settings from databse version
            SupportsDateTime6 = version >= new Version(5,6);
        }

        public readonly bool OldGuids;
        public readonly bool TreatTinyAsBoolean;
        public readonly bool SupportsDateTime6;
    }
}