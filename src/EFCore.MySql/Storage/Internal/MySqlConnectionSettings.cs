// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
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

        protected bool Equals(MySqlConnectionSettings other)
        {
            return OldGuids == other.OldGuids &&
                   TreatTinyAsBoolean == other.TreatTinyAsBoolean;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MySqlConnectionSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (OldGuids.GetHashCode() * 397) ^ TreatTinyAsBoolean.GetHashCode();
            }
        }

        public readonly bool OldGuids;
        public readonly bool TreatTinyAsBoolean;
    }
}
