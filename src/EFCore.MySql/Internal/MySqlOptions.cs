// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    public class MySqlOptions : IMySqlOptions
    {
        public MySqlOptions()
        {
            ConnectionSettings = new MySqlConnectionSettings();
            ServerVersion = new ServerVersion(null);
            CharSetBehavior = CharSetBehavior.AppendToAllAnsiColumns;
            AnsiCharSetInfo = new CharSetInfo(CharSet.Latin1);
            UnicodeCharSetInfo = new CharSetInfo(CharSet.Utf8mb4);
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

            ConnectionSettings = GetConnectionSettings(mySqlOptions);
            ServerVersion = mySqlOptions.ServerVersion ?? ServerVersion;
            CharSetBehavior = mySqlOptions.NullableCharSetBehavior ?? CharSetBehavior;
            AnsiCharSetInfo = mySqlOptions.AnsiCharSetInfo ?? AnsiCharSetInfo;
            UnicodeCharSetInfo = mySqlOptions.UnicodeCharSetInfo ?? UnicodeCharSetInfo;
            NoBackslashEscapes = mySqlOptions.NoBackslashEscapes;
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();

            if (!Equals(ServerVersion, mySqlOptions.ServerVersion ?? new ServerVersion(null)))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.ServerVersion),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            var connectionSettings = GetConnectionSettings(mySqlOptions);

            if (!Equals(ConnectionSettings.OldGuids, connectionSettings.OldGuids)
                || !Equals(ConnectionSettings.TreatTinyAsBoolean, connectionSettings.TreatTinyAsBoolean))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsExtensions.UseMySql),
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

        public virtual MySqlConnectionSettings ConnectionSettings { get; private set; }
        public virtual ServerVersion ServerVersion { get; private set; }
        public virtual CharSetBehavior CharSetBehavior { get; private set; }
        public virtual CharSetInfo AnsiCharSetInfo { get; private set; }
        public virtual CharSetInfo UnicodeCharSetInfo { get; private set; }
        public virtual bool NoBackslashEscapes { get; private set; }
    }
}
