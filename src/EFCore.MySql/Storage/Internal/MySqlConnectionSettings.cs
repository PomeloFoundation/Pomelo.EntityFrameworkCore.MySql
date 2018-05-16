// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using MySql.Data.MySqlClient;

namespace EFCore.MySql.Storage.Internal
{
    public class MySqlConnectionSettings
    {
        public MySqlConnectionSettings()
        {
        }

        public MySqlConnectionSettings(DbConnection connection)
            : this(connection.ConnectionString)
        {
        }

        public MySqlConnectionSettings(string connectionString)
        {
            var csb = new MySqlConnectionStringBuilder(connectionString);
            OldGuids = csb.OldGuids;
            TreatTinyAsBoolean = csb.TreatTinyAsBoolean;
        }

        public readonly bool OldGuids;
        public readonly bool TreatTinyAsBoolean;
    }
}
