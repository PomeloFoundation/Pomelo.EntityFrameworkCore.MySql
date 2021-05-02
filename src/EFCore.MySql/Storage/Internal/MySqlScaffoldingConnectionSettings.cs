// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    // ReSharper disable VirtualMemberCallInConstructor
    // ReSharper disable NonReadonlyMemberInGetHashCode
    public class MySqlScaffoldingConnectionSettings
    {
        public const string ScaffoldPrefix = "Scaffold:";
        public const string CharSetKey = ScaffoldPrefix + "CharSet";
        public const string CollationKey = ScaffoldPrefix + "Collation";
        public const string ViewsKey = ScaffoldPrefix + "Views";

        private readonly DbConnectionStringBuilder _csb;

        public MySqlScaffoldingConnectionSettings(string connectionString)
        {
            _csb = new DbConnectionStringBuilder { ConnectionString = connectionString };

            CharSet = GetBoolean(CharSetKey, true);
            Collation = GetBoolean(CollationKey, true);
            Views = GetBoolean(ViewsKey, true);
        }

        public virtual bool CharSet { get; set; }
        public virtual bool Collation { get; set; }
        public virtual bool Views { get; set; }

        public virtual string GetProviderCompatibleConnectionString()
        {
            var csb = new DbConnectionStringBuilder { ConnectionString = _csb.ConnectionString };

            csb.Remove(CharSetKey);
            csb.Remove(CollationKey);
            csb.Remove(ViewsKey);

            return csb.ConnectionString;
        }

        protected virtual bool GetBoolean(string key, bool defaultValue = default)
        {
            if (_csb.TryGetValue(key, out var value))
            {
                if (value is string stringValue)
                {
                    if (int.TryParse(stringValue, out var intValue))
                    {
                        return intValue != 0;
                    }

                    if (bool.TryParse(stringValue, out var boolValue))
                    {
                        return boolValue;
                    }

                    if (stringValue.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("enable", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("enabled", StringComparison.OrdinalIgnoreCase))
                        return true;

                    if (stringValue.Equals("off", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("disable", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("disabled", StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                return Convert.ToBoolean(value);
            }

            return defaultValue;
        }

        protected virtual bool Equals(MySqlScaffoldingConnectionSettings other)
        {
            return CharSet == other.CharSet &&
                   Collation == other.Collation &&
                   Views == other.Views;
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

            return Equals((MySqlScaffoldingConnectionSettings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CharSet.GetHashCode() * 397) ^ Collation.GetHashCode();
            }
        }
    }
}
