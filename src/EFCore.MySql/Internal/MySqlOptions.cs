// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    public class MySqlOptions : IMySqlOptions
    {
        public MySqlOptions()
        {
            ConnectionSettings = new MySqlConnectionSettings();
            ServerVersion = new ServerVersion(null);
            CharSetBehavior = CharSetBehavior.AppendToAllColumns;
            CharSet = ServerVersion.DefaultCharSet;
            NationalCharSet = CharSet.Utf8Mb3; // Prefdefined by MySQL
            ReplaceLineBreaksWithCharFunction = true;
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

            ConnectionSettings = GetConnectionSettings(mySqlOptions);
            ServerVersion = mySqlOptions.ServerVersion ?? ServerVersion;
            CharSetBehavior = mySqlOptions.NullableCharSetBehavior ?? CharSetBehavior;
            CharSet = mySqlOptions.CharSet ?? CharSet;
            NoBackslashEscapes = mySqlOptions.NoBackslashEscapes;
            ReplaceLineBreaksWithCharFunction = mySqlOptions.ReplaceLineBreaksWithCharFunction;
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            var connectionSettings = GetConnectionSettings(mySqlOptions);

            if (!Equals(ServerVersion, mySqlOptions.ServerVersion ?? new ServerVersion(null)))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.ServerVersion),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ConnectionSettings.TreatTinyAsBoolean, connectionSettings.TreatTinyAsBoolean))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlConnectionStringBuilder.TreatTinyAsBoolean),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ConnectionSettings.OldGuids, connectionSettings.OldGuids))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlConnectionStringBuilder.OldGuids),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(CharSetBehavior, mySqlOptions.NullableCharSetBehavior ?? CharSetBehavior.AppendToAllColumns))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.CharSetBehavior),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(CharSet, mySqlOptions.CharSet ?? ServerVersion.DefaultCharSet))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.CharSet),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(NoBackslashEscapes, mySqlOptions.NoBackslashEscapes))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.DisableBackslashEscaping),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ReplaceLineBreaksWithCharFunction, mySqlOptions.ReplaceLineBreaksWithCharFunction))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.DisableLineBreakToCharSubstition),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        private static MySqlConnectionSettings GetConnectionSettings(MySqlOptionsExtension relationalOptions)
        {
            if (relationalOptions.Connection != null)
            {
                return new MySqlConnectionSettings(relationalOptions.Connection);
            }

            if (relationalOptions.ConnectionString != null)
            {
                return new MySqlConnectionSettings(relationalOptions.ConnectionString);
            }

            throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
        }

        protected bool Equals(MySqlOptions other)
        {
            return Equals(ConnectionSettings, other.ConnectionSettings) &&
                   Equals(ServerVersion, other.ServerVersion) &&
                   CharSetBehavior == other.CharSetBehavior &&
                   Equals(CharSet, other.CharSet) &&
                   Equals(NationalCharSet, other.NationalCharSet) &&
                   NoBackslashEscapes == other.NoBackslashEscapes &&
                   ReplaceLineBreaksWithCharFunction == other.ReplaceLineBreaksWithCharFunction;
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

            return Equals((MySqlOptions)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ConnectionSettings != null ? ConnectionSettings.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (ServerVersion != null ? ServerVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)CharSetBehavior;
                hashCode = (hashCode * 397) ^ (CharSet != null ? CharSet.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NationalCharSet != null ? NationalCharSet.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ NoBackslashEscapes.GetHashCode();
                hashCode = (hashCode * 397) ^ ReplaceLineBreaksWithCharFunction.GetHashCode();
                return hashCode;
            }
        }

        public virtual MySqlConnectionSettings ConnectionSettings { get; private set; }
        public virtual ServerVersion ServerVersion { get; private set; }
        public virtual CharSetBehavior CharSetBehavior { get; private set; }
        public virtual CharSet CharSet { get; private set; }
        public CharSet NationalCharSet { get; }
        public virtual bool NoBackslashEscapes { get; private set; }
        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }
    }
}
