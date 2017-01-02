// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlConnectionSettings
    {
        private static readonly ConcurrentDictionary<string, MySqlConnectionSettings> Settings
            = new ConcurrentDictionary<string, MySqlConnectionSettings>();

        public static MySqlConnectionSettings GetSettings(string connectionString)
        {
            return Settings.GetOrAdd(connectionString, key => new MySqlConnectionSettings(connectionString));
        }

        private MySqlConnectionSettings(string connectionString)
        {
            // Settings from the connection string
            var csb = new MySqlConnectionStringBuilder(connectionString);
            OldGuids = csb.OldGuids;
            TreatTinyAsBoolean = csb.TreatTinyAsBoolean;

            // Connect to the database to get the version
            csb.Database = "";
            csb.Pooling = false;
            Version version;
            using (var schemalessConnection = new MySqlConnection(csb.ConnectionString))
            {
                schemalessConnection.Open();
                version = ServerVersion.ParseVersion(schemalessConnection.ServerVersion);
            }

            // Settings from databse version
            SupportsDateTime6 = version >= new Version(5,6);
        }

        public readonly bool OldGuids;
        public readonly bool TreatTinyAsBoolean;
        public readonly bool SupportsDateTime6;
    }
}