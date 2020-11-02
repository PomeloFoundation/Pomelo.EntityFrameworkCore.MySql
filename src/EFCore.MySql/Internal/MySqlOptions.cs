// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    public class MySqlOptions : IMySqlOptions
    {
        private static readonly MySqlSchemaNameTranslator _ignoreSchemaNameTranslator = (_, objectName) => objectName;

        public MySqlOptions()
        {
            ConnectionSettings = new MySqlConnectionSettings();
            ServerVersion = new ServerVersion(null);
            CharSetBehavior = CharSetBehavior.AppendToAllColumns; // TODO: Change to `NeverAppend` for EF Core 5.

            // We do not use the MySQL versions's default, but explicitly use `utf8mb4`
            // if not changed by the user.
            CharSet = CharSet.Utf8Mb4;

            // NCHAR and NVARCHAR are prefdefined by MySQL.
            NationalCharSet = CharSet.Utf8Mb3;

            ReplaceLineBreaksWithCharFunction = true;
            DefaultDataTypeMappings = new MySqlDefaultDataTypeMappings();

            // Throw by default if a schema is being used with any type.
            SchemaNameTranslator = null;

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            var mySqlJsonOptions = (MySqlJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is MySqlJsonOptionsExtension);

            ConnectionSettings = GetConnectionSettings(mySqlOptions);
            ServerVersion = mySqlOptions.ServerVersion ?? ServerVersion;
            CharSetBehavior = mySqlOptions.NullableCharSetBehavior ?? CharSetBehavior;
            CharSet = mySqlOptions.CharSet ?? CharSet;
            NoBackslashEscapes = mySqlOptions.NoBackslashEscapes;
            ReplaceLineBreaksWithCharFunction = mySqlOptions.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = ApplyDefaultDataTypeMappings(mySqlOptions.DefaultDataTypeMappings, ConnectionSettings);
            SchemaNameTranslator = mySqlOptions.SchemaNameTranslator ?? (mySqlOptions.SchemaBehavior == MySqlSchemaBehavior.Ignore
                ? _ignoreSchemaNameTranslator
                : null);
            IndexOptimizedBooleanColumns = mySqlOptions.IndexOptimizedBooleanColumns;
            JsonChangeTrackingOptions = mySqlJsonOptions?.JsonChangeTrackingOptions ?? default;
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            var mySqlJsonOptions = (MySqlJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is MySqlJsonOptionsExtension);
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

            if (!Equals(ConnectionSettings.GuidFormat, connectionSettings.GuidFormat))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlConnectionStringBuilder.GuidFormat),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(CharSetBehavior, mySqlOptions.NullableCharSetBehavior ?? CharSetBehavior.AppendToAllColumns))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.CharSetBehavior),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(CharSet, mySqlOptions.CharSet ?? CharSet.Utf8Mb4))
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

            if (!Equals(DefaultDataTypeMappings, ApplyDefaultDataTypeMappings(mySqlOptions.DefaultDataTypeMappings ?? new MySqlDefaultDataTypeMappings(), connectionSettings)))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.DefaultDataTypeMappings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(
                SchemaNameTranslator,
                mySqlOptions.SchemaBehavior == MySqlSchemaBehavior.Ignore
                    ? _ignoreSchemaNameTranslator
                    : SchemaNameTranslator))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.SchemaBehavior),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(IndexOptimizedBooleanColumns, mySqlOptions.IndexOptimizedBooleanColumns))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.EnableIndexOptimizedBooleanColumns),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(JsonChangeTrackingOptions, mySqlJsonOptions?.JsonChangeTrackingOptions ?? default))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlJsonOptionsExtension.JsonChangeTrackingOptions),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        protected virtual MySqlDefaultDataTypeMappings ApplyDefaultDataTypeMappings(MySqlDefaultDataTypeMappings defaultDataTypeMappings, MySqlConnectionSettings connectionSettings)
        {
            defaultDataTypeMappings ??= DefaultDataTypeMappings;

            // Explicitly set MySqlDefaultDataTypeMappings values take precedence over connection string options.
            if (connectionSettings.TreatTinyAsBoolean.HasValue &&
                defaultDataTypeMappings.ClrBoolean == MySqlBooleanType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrBoolean(
                    connectionSettings.TreatTinyAsBoolean.Value
                        ? MySqlBooleanType.TinyInt1
                        : MySqlBooleanType.Bit1);
            }

            if (defaultDataTypeMappings.ClrDateTime == MySqlDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTime(
                    ServerVersion.SupportsDateTime6
                        ? MySqlDateTimeType.DateTime6
                        : MySqlDateTimeType.DateTime);
            }

            if (defaultDataTypeMappings.ClrDateTimeOffset == MySqlDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTimeOffset(
                    ServerVersion.SupportsDateTime6
                        ? MySqlDateTimeType.DateTime6
                        : MySqlDateTimeType.DateTime);
            }

            if (defaultDataTypeMappings.ClrTimeSpan == MySqlTimeSpanType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeSpan(
                    ServerVersion.SupportsDateTime6
                        ? MySqlTimeSpanType.Time6
                        : MySqlTimeSpanType.Time);
            }

            return defaultDataTypeMappings;
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
                   ReplaceLineBreaksWithCharFunction == other.ReplaceLineBreaksWithCharFunction &&
                   Equals(DefaultDataTypeMappings, other.DefaultDataTypeMappings) &&
                   Equals(SchemaNameTranslator, other.SchemaNameTranslator) &&
                   IndexOptimizedBooleanColumns == other.IndexOptimizedBooleanColumns &&
                   JsonChangeTrackingOptions == other.JsonChangeTrackingOptions;
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
            var hashCode = new HashCode();
            hashCode.Add(ConnectionSettings);
            hashCode.Add(ServerVersion);
            hashCode.Add((int) CharSetBehavior);
            hashCode.Add(CharSet);
            hashCode.Add(NationalCharSet);
            hashCode.Add(NoBackslashEscapes);
            hashCode.Add(ReplaceLineBreaksWithCharFunction);
            hashCode.Add(DefaultDataTypeMappings);
            hashCode.Add(SchemaNameTranslator);
            hashCode.Add(IndexOptimizedBooleanColumns);
            hashCode.Add(JsonChangeTrackingOptions);
            return hashCode.ToHashCode();
        }

        public virtual MySqlConnectionSettings ConnectionSettings { get; private set; }
        public virtual ServerVersion ServerVersion { get; private set; }
        public virtual CharSetBehavior CharSetBehavior { get; private set; }
        public virtual CharSet CharSet { get; private set; }
        public virtual CharSet NationalCharSet { get; }
        public virtual bool NoBackslashEscapes { get; private set; }
        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }
        public virtual MySqlDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }
        public virtual MySqlSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public virtual bool IndexOptimizedBooleanColumns { get; private set; }
        public virtual MySqlJsonChangeTrackingOptions JsonChangeTrackingOptions { get; private set; }
    }
}
