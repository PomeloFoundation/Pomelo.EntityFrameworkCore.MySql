// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace EFCore.MySql.Storage.Internal
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
                ServerVersion version;
                using (var schemalessConnection = new MySqlConnection(csb.ConnectionString))
                {
                    schemalessConnection.Open();
                    version = new ServerVersion(schemalessConnection.ServerVersion);
                }
                return new MySqlConnectionSettings(settingsCsb, version);
            });
        }

        public static MySqlConnectionSettings GetSettings(DbConnection connection)
        {
            var csb = new MySqlConnectionStringBuilder(connection.ConnectionString);
            var settingsCsb = _settingsCsb(csb);
            return Settings.GetOrAdd(settingsCsb.ConnectionString, key =>
            {
                ServerVersion version;
                if (connection.State == ConnectionState.Closed)
                {
                    csb.Database = "";
                    csb.Pooling = false;
                    using (var schemalessConnection = new MySqlConnection(csb.ConnectionString))
                    {
                        schemalessConnection.Open();
                        version = new ServerVersion(schemalessConnection.ServerVersion);
                    }
                }
                else
                {
                    version = new ServerVersion(connection.ServerVersion);
                }

                return new MySqlConnectionSettings(settingsCsb, version);
            });
        }

        internal MySqlConnectionSettings(MySqlConnectionStringBuilder settingsCsb, ServerVersion serverVersion)
        {
            // Settings from the connection string
            OldGuids = settingsCsb.OldGuids;
            TreatTinyAsBoolean = settingsCsb.TreatTinyAsBoolean;
            ServerVersion = serverVersion;
        }

        public readonly bool OldGuids;
        public readonly bool TreatTinyAsBoolean;
        public readonly ServerVersion ServerVersion;
    }
}
