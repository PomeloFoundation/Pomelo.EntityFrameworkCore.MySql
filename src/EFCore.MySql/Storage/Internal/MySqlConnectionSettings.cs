// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Linq;
using MySqlConnector;

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

            // It would be nice to have access to a public and currently non-existing
            // MySqlConnectionStringOption.TreatTinyAsBoolean.HasValue() method, so we can safely find out, whether
            // TreatTinyAsBoolean has been explicitly set or not.
            var treatTinyAsBooleanKeys = new[] {"Treat Tiny As Boolean", "TreatTinyAsBoolean"};
            TreatTinyAsBoolean = treatTinyAsBooleanKeys.Any(k => csb.ContainsKey(k))
                ? (bool?)csb.TreatTinyAsBoolean
                : null;
        }

        public virtual MySqlGuidFormat GuidFormat { get; }
        public virtual bool? TreatTinyAsBoolean { get; }

        protected virtual bool Equals(MySqlConnectionSettings other)
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
            => HashCode.Combine(
                GuidFormat,
                TreatTinyAsBoolean);
    }
}
