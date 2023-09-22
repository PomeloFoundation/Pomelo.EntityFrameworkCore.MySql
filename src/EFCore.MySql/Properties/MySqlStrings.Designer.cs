// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class MySqlStrings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Pomelo.EntityFrameworkCore.MySql.Properties.MySqlStrings", typeof(MySqlStrings).GetTypeInfo().Assembly);

        /// <summary>
        ///     Identity value generation cannot be used for the property '{property}' on entity type '{entityType}' because the property type is '{propertyType}'. Identity value generation can only be used with integer, DateTime, and DateTimeOffset properties.
        /// </summary>
        public static string IdentityBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
            => string.Format(
                GetString("IdentityBadType", nameof(property), nameof(entityType), nameof(propertyType)),
                property, entityType, propertyType);

        /// <summary>
        ///     Data type '{dataType}' is not supported in this form. Either specify the length explicitly in the type name, for example as '{dataType}(16)', or remove the data type and use APIs such as HasMaxLength to allow EF choose the data type.
        /// </summary>
        public static string UnqualifiedDataType([CanBeNull] object dataType)
            => string.Format(
                GetString("UnqualifiedDataType", nameof(dataType)),
                dataType);

        /// <summary>
        ///     Data type '{dataType}' for property '{property}' is not supported in this form. Either specify the length explicitly in the type name, for example as '{dataType}(16)', or remove the data type and use APIs such as HasMaxLength to allow EF choose the data type.
        /// </summary>
        public static string UnqualifiedDataTypeOnProperty([CanBeNull] object dataType, [CanBeNull] object property)
            => string.Format(
                GetString("UnqualifiedDataTypeOnProperty", nameof(dataType), nameof(property)),
                dataType, property);

        /// <summary>
        ///     MySQL sequences cannot be used to generate values for the property '{property}' on entity type '{entityType}' because the property type is '{propertyType}'. Sequences can only be used with integer properties.
        /// </summary>
        public static string SequenceBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
            => string.Format(
                GetString("SequenceBadType", nameof(property), nameof(entityType), nameof(propertyType)),
                property, entityType, propertyType);

        /// <summary>
        ///     MySQL requires the table name to be specified for rename index operations. Specify table name in the call to MigrationBuilder.RenameIndex.
        /// </summary>
        public static string IndexTableRequired
            => GetString("IndexTableRequired");

        /// <summary>
        ///     To set memory-optimized on a table on or off the table needs to be dropped and recreated.
        /// </summary>
        public static string AlterMemoryOptimizedTable
            => GetString("AlterMemoryOptimizedTable");

        /// <summary>
        ///     To change the IDENTITY property of a column, the column needs to be dropped and recreated.
        /// </summary>
        public static string AlterIdentityColumn
            => GetString("AlterIdentityColumn");

        /// <summary>
        ///     An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure()' to the 'UseMySql' call.
        /// </summary>
        public static string TransientExceptionDetected
            => GetString("TransientExceptionDetected");

        /// <summary>
        ///     The property '{property}' on entity type '{entityType}' is configured to use 'SequenceHiLo' value generator, which is only intended for keys. If this was intentional configure an alternate key on the property, otherwise call 'ValueGeneratedNever' or configure store generation for this property.
        /// </summary>
        public static string NonKeyValueGeneration([CanBeNull] object property, [CanBeNull] object entityType)
            => string.Format(
                GetString("NonKeyValueGeneration", nameof(property), nameof(entityType)),
                property, entityType);

        /// <summary>
        ///     The properties {properties} are configured to use 'Identity' value generator and are mapped to the same table '{table}'. Only one column per table can be configured as 'Identity'. Call 'ValueGeneratedNever' for properties that should not use 'Identity'.
        /// </summary>
        public static string MultipleIdentityColumns([CanBeNull] object properties, [CanBeNull] object table)
            => string.Format(
                GetString("MultipleIdentityColumns", nameof(properties), nameof(table)),
                properties, table);

        /// <summary>
        ///     Cannot use table '{table}' for entity type '{entityType}' since it is being used for entity type '{otherEntityType}' and entity type '{memoryOptimizedEntityType}' is marked as memory-optimized, but entity type '{nonMemoryOptimizedEntityType}' is not.
        /// </summary>
        public static string IncompatibleTableMemoryOptimizedMismatch([CanBeNull] object table, [CanBeNull] object entityType, [CanBeNull] object otherEntityType, [CanBeNull] object memoryOptimizedEntityType, [CanBeNull] object nonMemoryOptimizedEntityType)
            => string.Format(
                GetString("IncompatibleTableMemoryOptimizedMismatch", nameof(table), nameof(entityType), nameof(otherEntityType), nameof(memoryOptimizedEntityType), nameof(nonMemoryOptimizedEntityType)),
                table, entityType, otherEntityType, memoryOptimizedEntityType, nonMemoryOptimizedEntityType);

        /// <summary>
        ///     The database name could not be determined. To use EnsureDeleted, the connection string must specify Initial Catalog.
        /// </summary>
        public static string NoInitialCatalog
            => GetString("NoInitialCatalog");

        /// <summary>
        ///     '{entityType1}.{property1}' and '{entityType2}.{property2}' are both mapped to column '{columnName}' in '{table}' but are configured with different value generation strategies.
        /// </summary>
        public static string DuplicateColumnNameValueGenerationStrategyMismatch([CanBeNull] object entityType1, [CanBeNull] object property1, [CanBeNull] object entityType2, [CanBeNull] object property2, [CanBeNull] object columnName, [CanBeNull] object table)
            => string.Format(
                GetString("DuplicateColumnNameValueGenerationStrategyMismatch", nameof(entityType1), nameof(property1), nameof(entityType2), nameof(property2), nameof(columnName), nameof(table)),
                entityType1, property1, entityType2, property2, columnName, table);

        /// <summary>
        ///     The specified table '{table}' is not valid. Specify tables using the format '[schema].[table]'.
        /// </summary>
        public static string InvalidTableToIncludeInScaffolding([CanBeNull] object table)
            => string.Format(
                GetString("InvalidTableToIncludeInScaffolding", nameof(table)),
                table);

        /// <summary>
        ///     The '{methodName}' method is not supported because the query has switched to client-evaluation. Inspect the log to determine which query expressions are triggering client-evaluation.
        /// </summary>
        public static string FunctionOnClient([CanBeNull] object methodName)
            => string.Format(
                GetString("FunctionOnClient", nameof(methodName)),
                methodName);

        /// <summary>
        ///     Computed value generation cannot be used for the property '{property}' on entity type '{entityType}' because the property type is '{propertyType}'. Computed value generation can only be used with DateTime and DateTimeOffset properties.
        /// </summary>
        public static string ComputedBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
            => string.Format(
                GetString("ComputedBadType", nameof(property), nameof(entityType), nameof(propertyType)),
                property, entityType, propertyType);

        /// <summary>
        ///     The specified expression does not have the correct Type.
        /// </summary>
        public static string ExpressionTypeMismatch
            => GetString("ExpressionTypeMismatch");

        /// <summary>
        ///     Translation of the '{declaringTypeName}.{methodName}' overload with a 'StringComparison' parameter is not supported by default. To opt-in to translations of methods with a 'StringComparison' parameter, call `{optionName}` on your MySQL specific 'DbContext' options. For general EF Core information about this error, see https://go.microsoft.com/fwlink/?linkid=2129535 for more information.
        /// </summary>
        public static string QueryUnableToTranslateMethodWithStringComparison([CanBeNull] object declaringTypeName, [CanBeNull] object methodName, [CanBeNull] object optionName)
            => string.Format(
                GetString("QueryUnableToTranslateMethodWithStringComparison", nameof(declaringTypeName), nameof(methodName), nameof(optionName)),
                declaringTypeName, methodName, optionName);

        /// <summary>
        ///     The entity type '{entityType}' is mapped to the stored procedure '{sproc}', which is configured with result columns. MySQL stored procedures do not support result columns; use output parameters instead.
        /// </summary>
        public static string StoredProcedureResultColumnsNotSupported(object entityType, object sproc)
            => string.Format(
                GetString("StoredProcedureResultColumnsNotSupported", nameof(entityType), nameof(sproc)),
                entityType, sproc);

        /// <summary>
        ///     The entity type '{entityType}' is mapped to the stored procedure '{sproc}', which is configured with a return value. MySQL stored procedures do not support return values; use an output parameter instead.
        /// </summary>
        public static string StoredProcedureReturnValueNotSupported(object entityType, object sproc)
            => string.Format(
                GetString("StoredProcedureReturnValueNotSupported", nameof(entityType), nameof(sproc)),
                entityType, sproc);

        /// <summary>
        ///     The EF Core 7.0 JSON support isn't currently implemented. Instead, there is support for a more extensive implementation. See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql for more information on how to map JSON.
        /// </summary>
        public static string Ef7CoreJsonMappingNotSupported
            => GetString("Ef7CoreJsonMappingNotSupported");

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }
    }
}

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class MySqlResources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Pomelo.EntityFrameworkCore.MySql.Properties.MySqlStrings", typeof(MySqlResources).GetTypeInfo().Assembly);

        /// <summary>
        ///     No type was specified for the decimal column '{property}' on entity type '{entityType}'. This will cause values to be silently truncated if they do not fit in the default precision and scale. Explicitly specify the SQL server column type that can accommodate all the values using 'ForHasColumnType()'.
        /// </summary>
        public static EventDefinition<string, string> LogDefaultDecimalTypeColumn([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogDefaultDecimalTypeColumn;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogDefaultDecimalTypeColumn,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        MySqlEventId.DecimalTypeDefaultWarning,
                        LogLevel.Warning,
                        "MySqlEventId.DecimalTypeDefaultWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            MySqlEventId.DecimalTypeDefaultWarning,
                            _resourceManager.GetString("LogDefaultDecimalTypeColumn"))));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     The property '{property}' on entity type '{entityType}' is of type 'byte', but is set up to use a MySQL identity column. This requires that values starting at 255 and counting down will be used for temporary key values. A temporary key value is needed for every entity inserted in a single call to 'SaveChanges'. Care must be taken that these values do not collide with real key values.
        /// </summary>
        public static EventDefinition<string, string> LogByteIdentityColumn([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogByteIdentityColumn;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogByteIdentityColumn,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        MySqlEventId.ByteIdentityColumnWarning,
                        LogLevel.Warning,
                        "MySqlEventId.ByteIdentityColumnWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            MySqlEventId.ByteIdentityColumnWarning,
                            _resourceManager.GetString("LogByteIdentityColumn"))));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Found default schema {defaultSchema}.
        /// </summary>
        public static EventDefinition<string> LogFoundDefaultSchema([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundDefaultSchema;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundDefaultSchema,
                    () => new EventDefinition<string>(
                        logger.Options,
                        MySqlEventId.DefaultSchemaFound,
                        LogLevel.Debug,
                        "MySqlEventId.DefaultSchemaFound",
                        level => LoggerMessage.Define<string>(
                            level,
                            MySqlEventId.DefaultSchemaFound,
                            _resourceManager.GetString("LogFoundDefaultSchema"))));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     Found type alias with name: {alias} which maps to underlying data type {dataType}.
        /// </summary>
        public static EventDefinition<string, string> LogFoundTypeAlias([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundTypeAlias;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundTypeAlias,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        MySqlEventId.TypeAliasFound,
                        LogLevel.Debug,
                        "MySqlEventId.TypeAliasFound",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            MySqlEventId.TypeAliasFound,
                            _resourceManager.GetString("LogFoundTypeAlias"))));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Found column with table: {tableName}, column name: {columnName}, ordinal: {ordinal}, data type: {dataType}, maximum length: {maxLength}, precision: {precision}, scale: {scale}, nullable: {isNullable}, identity: {isIdentity}, default value: {defaultValue}, computed value: {computedValue}
        /// </summary>
        public static FallbackEventDefinition LogFoundColumn([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundColumn;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundColumn,
                    () => new FallbackEventDefinition(
                        logger.Options,
                        MySqlEventId.ColumnFound,
                        LogLevel.Debug,
                        "MySqlEventId.ColumnFound",
                        _resourceManager.GetString("LogFoundColumn")));
            }

            return (FallbackEventDefinition)definition;
        }

        /// <summary>
        ///     Found foreign key on table: {tableName}, name: {foreignKeyName}, principal table: {principalTableName}, delete action: {deleteAction}.
        /// </summary>
        public static EventDefinition<string, string, string, string> LogFoundForeignKey([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundForeignKey;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundForeignKey,
                    () => new EventDefinition<string, string, string, string>(
                        logger.Options,
                        MySqlEventId.ForeignKeyFound,
                        LogLevel.Debug,
                        "MySqlEventId.ForeignKeyFound",
                        level => LoggerMessage.Define<string, string, string, string>(
                            level,
                            MySqlEventId.ForeignKeyFound,
                            _resourceManager.GetString("LogFoundForeignKey"))));
            }

            return (EventDefinition<string, string, string, string>)definition;
        }

        /// <summary>
        ///     For foreign key {fkName} on table {tableName}, unable to model the end of the foreign key on principal table {principaltableName}. This is usually because the principal table was not included in the selection set.
        /// </summary>
        public static EventDefinition<string, string, string> LogPrincipalTableNotInSelectionSet([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogPrincipalTableNotInSelectionSet;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogPrincipalTableNotInSelectionSet,
                    () => new EventDefinition<string, string, string>(
                        logger.Options,
                        MySqlEventId.ForeignKeyReferencesMissingPrincipalTableWarning,
                        LogLevel.Warning,
                        "MySqlEventId.ForeignKeyReferencesMissingPrincipalTableWarning",
                        level => LoggerMessage.Define<string, string, string>(
                            level,
                            MySqlEventId.ForeignKeyReferencesMissingPrincipalTableWarning,
                            _resourceManager.GetString("LogPrincipalTableNotInSelectionSet"))));
            }

            return (EventDefinition<string, string, string>)definition;
        }

        /// <summary>
        ///     Unable to find a schema in the database matching the selected schema {schema}.
        /// </summary>
        public static EventDefinition<string> LogMissingSchema([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogMissingSchema;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogMissingSchema,
                    () => new EventDefinition<string>(
                        logger.Options,
                        MySqlEventId.MissingSchemaWarning,
                        LogLevel.Warning,
                        "MySqlEventId.MissingSchemaWarning",
                        level => LoggerMessage.Define<string>(
                            level,
                            MySqlEventId.MissingSchemaWarning,
                            _resourceManager.GetString("LogMissingSchema"))));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     Unable to find a table in the database matching the selected table {table}.
        /// </summary>
        public static EventDefinition<string> LogMissingTable([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogMissingTable;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogMissingTable,
                    () => new EventDefinition<string>(
                        logger.Options,
                        MySqlEventId.MissingTableWarning,
                        LogLevel.Warning,
                        "MySqlEventId.MissingTableWarning",
                        level => LoggerMessage.Define<string>(
                            level,
                            MySqlEventId.MissingTableWarning,
                            _resourceManager.GetString("LogMissingTable"))));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     Found sequence name: {name}, data type: {dataType}, cyclic: {isCyclic}, increment: {increment}, start: {start}, minimum: {min}, maximum: {max}.
        /// </summary>
        public static FallbackEventDefinition LogFoundSequence([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundSequence;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundSequence,
                    () => new FallbackEventDefinition(
                        logger.Options,
                        MySqlEventId.SequenceFound,
                        LogLevel.Debug,
                        "MySqlEventId.SequenceFound",
                        _resourceManager.GetString("LogFoundSequence")));
            }

            return (FallbackEventDefinition)definition;
        }

        /// <summary>
        ///     Found table with name: {name}.
        /// </summary>
        public static EventDefinition<string> LogFoundTable([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundTable;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundTable,
                    () => new EventDefinition<string>(
                        logger.Options,
                        MySqlEventId.TableFound,
                        LogLevel.Debug,
                        "MySqlEventId.TableFound",
                        level => LoggerMessage.Define<string>(
                            level,
                            MySqlEventId.TableFound,
                            _resourceManager.GetString("LogFoundTable"))));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     Found index with name: {indexName}, table: {tableName}, is unique: {isUnique}.
        /// </summary>
        public static EventDefinition<string, string, bool> LogFoundIndex([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundIndex;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundIndex,
                    () => new EventDefinition<string, string, bool>(
                        logger.Options,
                        MySqlEventId.IndexFound,
                        LogLevel.Debug,
                        "MySqlEventId.IndexFound",
                        level => LoggerMessage.Define<string, string, bool>(
                            level,
                            MySqlEventId.IndexFound,
                            _resourceManager.GetString("LogFoundIndex"))));
            }

            return (EventDefinition<string, string, bool>)definition;
        }

        /// <summary>
        ///     Found primary key with name: {primaryKeyName}, table: {tableName}.
        /// </summary>
        public static EventDefinition<string, string> LogFoundPrimaryKey([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundPrimaryKey;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundPrimaryKey,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        MySqlEventId.PrimaryKeyFound,
                        LogLevel.Debug,
                        "MySqlEventId.PrimaryKeyFound",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            MySqlEventId.PrimaryKeyFound,
                            _resourceManager.GetString("LogFoundPrimaryKey"))));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Found unique constraint with name: {uniqueConstraintName}, table: {tableName}.
        /// </summary>
        public static EventDefinition<string, string> LogFoundUniqueConstraint([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundUniqueConstraint;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogFoundUniqueConstraint,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        MySqlEventId.UniqueConstraintFound,
                        LogLevel.Debug,
                        "MySqlEventId.UniqueConstraintFound",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            MySqlEventId.UniqueConstraintFound,
                            _resourceManager.GetString("LogFoundUniqueConstraint"))));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     For foreign key {foreignKeyName} on table {tableName}, unable to find the column called {principalColumnName} on the foreign key's principal table, {principaltableName}. Skipping foreign key.
        /// </summary>
        public static EventDefinition<string, string, string, string> LogPrincipalColumnNotFound([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogPrincipalColumnNotFound;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogPrincipalColumnNotFound,
                    () => new EventDefinition<string, string, string, string>(
                        logger.Options,
                        MySqlEventId.ForeignKeyPrincipalColumnMissingWarning,
                        LogLevel.Warning,
                        "MySqlEventId.ForeignKeyPrincipalColumnMissingWarning",
                        level => LoggerMessage.Define<string, string, string, string>(
                            level,
                            MySqlEventId.ForeignKeyPrincipalColumnMissingWarning,
                            _resourceManager.GetString("LogPrincipalColumnNotFound"))));
            }

            return (EventDefinition<string, string, string, string>)definition;
        }

        /// <summary>
        ///     The default value '{defaultValue}' is being ignored, because the database server version {version} does not support constant
        ///     default values for type '{type}' and does not support default value expressions in general.
        /// </summary>
        public static EventDefinition<string, string, string> LogDefaultValueNotSupported([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogDefaultValueNotSupported;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.MySqlLoggingDefinitions)logger.Definitions).LogDefaultValueNotSupported,
                    () => new EventDefinition<string, string, string>(
                        logger.Options,
                        MySqlEventId.DefaultValueNotSupportedWarning,
                        LogLevel.Warning,
                        "MySqlEventId.DefaultValueNotSupportedWarning",
                        level => LoggerMessage.Define<string, string, string>(
                            level,
                            MySqlEventId.DefaultValueNotSupportedWarning,
                            _resourceManager.GetString("LogDefaultValueNotSupported"))));
            }

            return (EventDefinition<string, string, string>)definition;
        }
    }
}
