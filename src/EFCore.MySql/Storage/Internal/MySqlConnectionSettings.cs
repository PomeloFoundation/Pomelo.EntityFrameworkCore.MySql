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

            if (csb.GuidFormat == MySqlGuidFormat.Default)
            {
                GuidFormat = csb.OldGuids
                    ? MySqlGuidFormat.LittleEndianBinary16
                    : MySqlGuidFormat.Char36;
            }
            else
            {
                GuidFormat = csb.GuidFormat;
            }

            TreatTinyAsBoolean = csb.TreatTinyAsBoolean;
        }

        public virtual MySqlGuidFormat GuidFormat { get; }
        public virtual bool TreatTinyAsBoolean { get; }

        protected bool Equals(MySqlConnectionSettings other)
        {
            return GuidFormat == other.GuidFormat &&
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

            return Equals((MySqlConnectionSettings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)GuidFormat * 397) ^ TreatTinyAsBoolean.GetHashCode();
            }
        }
    }
}
