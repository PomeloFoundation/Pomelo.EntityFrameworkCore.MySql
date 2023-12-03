// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    public class MySqlOptions : IMySqlOptions
    {
        private static readonly MySqlSchemaNameTranslator _ignoreSchemaNameTranslator = (_, objectName) => objectName;

        public MySqlOptions()
        {
            ConnectionSettings = new MySqlConnectionSettings();
            DataSource = null;
            ServerVersion = null;

            // We explicitly use `utf8mb4` in all instances, where charset based calculations need to be done, but accessing annotations
            // isn't possible (e.g. in `MySqlTypeMappingSource`).
            // This is also being used as the universal fallback character set, if no character set was explicitly defined for the model,
            // which will result in similar behavior as in previous versions and ensure that databases use a decent/the recommended charset
            // by default, if none was explicitly set.
            DefaultCharSet = CharSet.Utf8Mb4;

            // NCHAR and NVARCHAR are prefdefined by MySQL.
            NationalCharSet = CharSet.Utf8Mb3;

            // Optimize space and performance for GUID columns.
            DefaultGuidCollation = "ascii_general_ci";

            ReplaceLineBreaksWithCharFunction = true;
            DefaultDataTypeMappings = new MySqlDefaultDataTypeMappings();

            // Throw by default if a schema is being used with any type.
            SchemaNameTranslator = null;

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;

            LimitKeyedOrIndexedStringColumnLength = true;
            StringComparisonTranslations = false;
            PrimitiveCollectionsSupport = false;
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            var mySqlJsonOptions = (MySqlJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is MySqlJsonOptionsExtension);

            ConnectionSettings = GetConnectionSettings(mySqlOptions);
            DataSource = mySqlOptions.DataSource;
            ServerVersion = mySqlOptions.ServerVersion ?? throw new InvalidOperationException($"The {nameof(ServerVersion)} has not been set.");
            NoBackslashEscapes = mySqlOptions.NoBackslashEscapes;
            ReplaceLineBreaksWithCharFunction = mySqlOptions.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = ApplyDefaultDataTypeMappings(mySqlOptions.DefaultDataTypeMappings, ConnectionSettings);
            SchemaNameTranslator = mySqlOptions.SchemaNameTranslator ?? (mySqlOptions.SchemaBehavior == MySqlSchemaBehavior.Ignore
                ? _ignoreSchemaNameTranslator
                : null);
            IndexOptimizedBooleanColumns = mySqlOptions.IndexOptimizedBooleanColumns;
            JsonChangeTrackingOptions = mySqlJsonOptions?.JsonChangeTrackingOptions ?? default;
            LimitKeyedOrIndexedStringColumnLength = mySqlOptions.LimitKeyedOrIndexedStringColumnLength;
            StringComparisonTranslations = mySqlOptions.StringComparisonTranslations;
            PrimitiveCollectionsSupport = mySqlOptions.PrimitiveCollectionsSupport;
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<MySqlOptionsExtension>() ?? new MySqlOptionsExtension();
            var mySqlJsonOptions = (MySqlJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is MySqlJsonOptionsExtension);
            var connectionSettings = GetConnectionSettings(mySqlOptions);

            //
            // CHECK: To we have to ensure that the ApplicationServiceProvider itself is not replaced, because we rely on it in our
            //        DbDataSource check, or is that not possible?
            //

            // Even though we only save a DbDataSource that has been explicitly set using the MySqlOptionsExtensions here in MySqlOptions,
            // we will later also fall back to a DbDataSource that has been added as a service to the ApplicationServiceProvider, if no
            // DbDataSource has been explicitly set here. We call that DbDataSource the "effective" DbDataSource and handle it in the same
            // way we would handle a singleton option.
            var effectiveDataSource = mySqlOptions.DataSource ??
                                      options.FindExtension<CoreOptionsExtension>()?.ApplicationServiceProvider?.GetService<MySqlDataSource>();
            if (effectiveDataSource is not null &&
                !ReferenceEquals(DataSource, mySqlOptions.DataSource))
            {
                throw new InvalidOperationException(
                    MySqlStrings.TwoDataSourcesInSameServiceProvider(nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ServerVersion, mySqlOptions.ServerVersion))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlOptionsExtension.ServerVersion),
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

            if (!Equals(LimitKeyedOrIndexedStringColumnLength, mySqlOptions.LimitKeyedOrIndexedStringColumnLength))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.LimitKeyedOrIndexedStringColumnLength),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(StringComparisonTranslations, mySqlOptions.StringComparisonTranslations))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.EnableStringComparisonTranslations),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(PrimitiveCollectionsSupport, mySqlOptions.PrimitiveCollectionsSupport))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(MySqlDbContextOptionsBuilder.EnablePrimitiveCollectionsSupport),
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
                    ServerVersion.Supports.DateTime6
                        ? MySqlDateTimeType.DateTime6
                        : MySqlDateTimeType.DateTime);
            }

            if (defaultDataTypeMappings.ClrDateTimeOffset == MySqlDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTimeOffset(
                    ServerVersion.Supports.DateTime6
                        ? MySqlDateTimeType.DateTime6
                        : MySqlDateTimeType.DateTime);
            }

            if (defaultDataTypeMappings.ClrTimeSpan == MySqlTimeSpanType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeSpan(
                    ServerVersion.Supports.DateTime6
                        ? MySqlTimeSpanType.Time6
                        : MySqlTimeSpanType.Time);
            }

            if (defaultDataTypeMappings.ClrTimeOnlyPrecision < 0)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeOnly(
                    ServerVersion.Supports.DateTime6
                        ? 6
                        : 0);
            }

            return defaultDataTypeMappings;
        }

        private static MySqlConnectionSettings GetConnectionSettings(MySqlOptionsExtension relationalOptions)
            => relationalOptions.Connection != null
                ? new MySqlConnectionSettings(relationalOptions.Connection)
                : new MySqlConnectionSettings(relationalOptions.ConnectionString);

        protected virtual bool Equals(MySqlOptions other)
        {
            return Equals(ConnectionSettings, other.ConnectionSettings) &&
                   ReferenceEquals(DataSource, other.DataSource) &&
                   Equals(ServerVersion, other.ServerVersion) &&
                   Equals(DefaultCharSet, other.DefaultCharSet) &&
                   Equals(NationalCharSet, other.NationalCharSet) &&
                   Equals(DefaultGuidCollation, other.DefaultGuidCollation) &&
                   NoBackslashEscapes == other.NoBackslashEscapes &&
                   ReplaceLineBreaksWithCharFunction == other.ReplaceLineBreaksWithCharFunction &&
                   Equals(DefaultDataTypeMappings, other.DefaultDataTypeMappings) &&
                   Equals(SchemaNameTranslator, other.SchemaNameTranslator) &&
                   IndexOptimizedBooleanColumns == other.IndexOptimizedBooleanColumns &&
                   JsonChangeTrackingOptions == other.JsonChangeTrackingOptions &&
                   LimitKeyedOrIndexedStringColumnLength == other.LimitKeyedOrIndexedStringColumnLength &&
                   StringComparisonTranslations == other.StringComparisonTranslations &&
                   PrimitiveCollectionsSupport == other.PrimitiveCollectionsSupport;
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
            hashCode.Add(DataSource?.ConnectionString);
            hashCode.Add(ServerVersion);
            hashCode.Add(DefaultCharSet);
            hashCode.Add(NationalCharSet);
            hashCode.Add(DefaultGuidCollation);
            hashCode.Add(NoBackslashEscapes);
            hashCode.Add(ReplaceLineBreaksWithCharFunction);
            hashCode.Add(DefaultDataTypeMappings);
            hashCode.Add(SchemaNameTranslator);
            hashCode.Add(IndexOptimizedBooleanColumns);
            hashCode.Add(JsonChangeTrackingOptions);
            hashCode.Add(LimitKeyedOrIndexedStringColumnLength);
            hashCode.Add(StringComparisonTranslations);
            hashCode.Add(PrimitiveCollectionsSupport);

            return hashCode.ToHashCode();
        }

        public virtual MySqlConnectionSettings ConnectionSettings { get; private set; }

        /// <summary>
        /// If null, there might still be a `DbDataSource` in the ApplicationServiceProvider.
        /// </summary>
        public virtual DbDataSource DataSource { get; private set; }

        public virtual ServerVersion ServerVersion { get; private set; }
        public virtual CharSet DefaultCharSet { get; private set; }
        public virtual CharSet NationalCharSet { get; }
        public virtual string DefaultGuidCollation { get; private set; }
        public virtual bool NoBackslashEscapes { get; private set; }
        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }
        public virtual MySqlDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }
        public virtual MySqlSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public virtual bool IndexOptimizedBooleanColumns { get; private set; }
        public virtual MySqlJsonChangeTrackingOptions JsonChangeTrackingOptions { get; private set; }
        public virtual bool LimitKeyedOrIndexedStringColumnLength { get; private set; }
        public virtual bool StringComparisonTranslations { get; private set; }
        public virtual bool PrimitiveCollectionsSupport { get; private set; }
    }
}
