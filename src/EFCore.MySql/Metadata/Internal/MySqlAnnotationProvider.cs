// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Internal
{
    public class MySqlAnnotationProvider : RelationalAnnotationProvider
    {
        [NotNull] private readonly IMySqlOptions _options;

        public MySqlAnnotationProvider(
            [NotNull] RelationalAnnotationProviderDependencies dependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        public override IEnumerable<IAnnotation> For(IRelationalModel model, bool designTime)
        {
            if (!designTime)
            {
                yield break;
            }

            if (GetActualModelCharSet(model.Model, DelegationModes.ApplyToDatabases) is string charSet)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charSet);
            }

            // If a collation delegation modes has been set, but does not contain DelegationMode.ApplyToDatabase, we reset the EF Core
            // handled Collation property in MySqlMigrationsModelDiffer.

            // Handle other annotations (including the delegation annotations).
            foreach (var annotation in model.Model.GetAnnotations()
                .Where(a => a.Name is MySqlAnnotationNames.CharSetDelegation or
                                      MySqlAnnotationNames.CollationDelegation))
            {
                yield return annotation;
            }
        }

        public override IEnumerable<IAnnotation> For(ITable table, bool designTime)
        {
            if (!designTime)
            {
                yield break;
            }

            // Model validation ensures that these facets are the same on all mapped entity types
            var entityType = table.EntityTypeMappings.First().EntityType;

            // Use an explicitly defined character set, if set.
            // Otherwise, explicitly use the model/database character set, if delegation is enabled.
            if (GetActualEntityTypeCharSet(entityType, DelegationModes.ApplyToTables) is string charSet)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charSet);
            }

            // Use an explicitly defined collation, if set.
            // Otherwise, explicitly use the model/database collation, if delegation is enabled.
            if (GetActualEntityTypeCollation(entityType, DelegationModes.ApplyToTables) is string collation)
            {
                yield return new Annotation(
                    RelationalAnnotationNames.Collation,
                    collation);
            }

            // Handle other annotations (including the delegation annotations).
            foreach (var annotation in entityType.GetAnnotations()
                .Where(a => a.Name is MySqlAnnotationNames.CharSetDelegation or
                                      MySqlAnnotationNames.CollationDelegation or
                                      MySqlAnnotationNames.StoreOptions))
            {
                yield return annotation;
            }
        }

        public override IEnumerable<IAnnotation> For(IUniqueConstraint constraint, bool designTime)
        {
            if (!designTime)
            {
                yield break;
            }

            // Model validation ensures that these facets are the same on all mapped indexes
            var key = constraint.MappedKeys.First();

            var prefixLength = key.PrefixLength();
            if (prefixLength != null &&
                prefixLength.Length > 0)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.IndexPrefixLength,
                    prefixLength);
            }
        }

        public override IEnumerable<IAnnotation> For(ITableIndex index, bool designTime)
        {
            if (!designTime)
            {
                yield break;
            }

            // Model validation ensures that these facets are the same on all mapped indexes
            var modelIndex = index.MappedIndexes.First();

            var prefixLength = modelIndex.PrefixLength();
            if (prefixLength != null &&
                prefixLength.Length > 0)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.IndexPrefixLength,
                    prefixLength);
            }

            var isFullText = modelIndex.IsFullText();
            if (isFullText.HasValue)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.FullTextIndex,
                    isFullText.Value);
            }

            var fullTextParser = modelIndex.FullTextParser();
            if (!string.IsNullOrEmpty(fullTextParser))
            {
                yield return new Annotation(
                    MySqlAnnotationNames.FullTextParser,
                    fullTextParser);
            }

            var isSpatial = modelIndex.IsSpatial();
            if (isSpatial.HasValue)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.SpatialIndex,
                    isSpatial.Value);
            }
        }

        public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
        {
            if (!designTime)
            {
                yield break;
            }

            var table = StoreObjectIdentifier.Table(column.Table.Name, column.Table.Schema);
            var properties = column.PropertyMappings.Select(m => m.Property).ToArray();

            if (column.PropertyMappings.Where(
                    m => m.TableMapping.IsSharedTablePrincipal &&
                         m.TableMapping.EntityType == m.Property.DeclaringEntityType)
                .Select(m => m.Property)
                .FirstOrDefault(p => p.GetValueGenerationStrategy(table) == MySqlValueGenerationStrategy.IdentityColumn) is IProperty identityProperty)
            {
                var valueGenerationStrategy = identityProperty.GetValueGenerationStrategy(table);
                yield return new Annotation(
                    MySqlAnnotationNames.ValueGenerationStrategy,
                    valueGenerationStrategy);
            }
            else if (properties.FirstOrDefault(
                p => p.GetValueGenerationStrategy(table) == MySqlValueGenerationStrategy.ComputedColumn) is IProperty computedProperty)
            {
                var valueGenerationStrategy = computedProperty.GetValueGenerationStrategy(table);
                yield return new Annotation(
                    MySqlAnnotationNames.ValueGenerationStrategy,
                    valueGenerationStrategy);
            }

            // Use an explicitly defined character set, if set.
            // Otherwise, explicitly use the entity/table or model/database character set, if delegation is enabled.
            if (GetActualPropertyCharSet(properties, DelegationModes.ApplyToColumns) is string charSet)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charSet);
            }

            // Use an explicitly defined collation, if set.
            // Otherwise, explicitly use the entity/table or model/database collation, if delegation is enabled.
            if (GetActualPropertyCollation(properties, DelegationModes.ApplyToColumns) is string collation)
            {
                yield return new Annotation(
                    RelationalAnnotationNames.Collation,
                    collation);
            }

            if (column.PropertyMappings.Select(m => m.Property.GetSpatialReferenceSystem())
                .FirstOrDefault(c => c != null) is int srid)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.SpatialReferenceSystemId,
                    srid);
            }
        }

        protected virtual string GetActualModelCharSet(IModel model, DelegationModes currentLevel)
        {
            // If neither character set nor collation has been explicitly defined for the model, and no delegation has been setup, we use
            // Pomelo's universal fallback default character set (which is `utf8mb4`) and apply it to all database objects.
            return model.GetCharSet() is null &&
                   model.GetCharSetDelegation() is null &&
                   model.GetCollation() is null &&
                   model.GetCollationDelegation() is null
                ? _options.DefaultCharSet.Name
                : model.GetActualCharSetDelegation().HasFlag(currentLevel)
                    ? model.GetCharSet()
                    : null;
        }

        protected virtual string GetActualModelCollation(IModel model, DelegationModes currentLevel)
        {
            return model.GetActualCollationDelegation().HasFlag(currentLevel)
                ? model.GetCollation()
                : null;
        }

        protected virtual string GetActualEntityTypeCharSet(IEntityType entityType, DelegationModes currentLevel)
        {
            // There are the following variations at the entity level:
            //    1. entityTypeBuilder.HasCharSet(null, null) [or no call at all]
            //            -> Check the charset and delegation at the database level.
            //    2. a. entityTypeBuilder.HasCharSet(null, DelegationMode.ApplyToAll)
            //       b. entityTypeBuilder.HasCharSet(null, DelegationMode.ApplyToColumns)
            //            -> Do not explicitly use any charset.
            //    3. a. entityTypeBuilder.HasCharSet("latin1")
            //       b. entityTypeBuilder.HasCharSet("latin1", DelegationMode.ApplyToAll)
            //       c. entityTypeBuilder.HasCharSet("latin1", DelegationMode.ApplyToColumns)
            //            -> Explicitly use the specified charset.
            return (entityType.GetCharSet() is not null || // 3abc
                    entityType.GetCharSet() is null && entityType.GetCharSetDelegation() is not null) && // 2ab
                   entityType.GetActualCharSetDelegation().HasFlag(currentLevel) // 3abc, 2ab
                ? entityType.GetCharSet()
                // An explicitly defined collation on the current entity level takes precedence over an inherited charset.
                : GetActualModelCharSet(entityType.Model, currentLevel) is string charSet && // 1
                  (currentLevel != DelegationModes.ApplyToTables ||
                   entityType.GetCollation() is not string collation ||
                   !entityType.GetActualCollationDelegation().HasFlag(DelegationModes.ApplyToTables) ||
                   collation.StartsWith(charSet, StringComparison.OrdinalIgnoreCase))
                    ? charSet
                    : null;
        }

        protected virtual string GetActualEntityTypeCollation(IEntityType entityType, DelegationModes currentLevel)
        {
            // There are the following variations at the entity level:
            //    1. entityTypeBuilder.HasCollation(null, null) [or no call at all]
            //            -> Check the collation and delegation at the database level.
            //    2. a. entityTypeBuilder.HasCollation(null, DelegationMode.ApplyToAll)
            //       b. entityTypeBuilder.HasCollation(null, DelegationMode.ApplyToColumns)
            //            -> Do not explicitly use any collation.
            //    3. a. entityTypeBuilder.HasCollation("latin1_general_ci")
            //       b. entityTypeBuilder.HasCollation("latin1_general_ci", DelegationMode.ApplyToAll)
            //       c. entityTypeBuilder.HasCollation("latin1_general_ci", DelegationMode.ApplyToColumns)
            //            -> Explicitly use the specified collation.
            return (entityType.GetCollation() is not null || // 3abc
                    entityType.GetCollation() is null && entityType.GetCollationDelegation() is not null) && // 2ab
                   entityType.GetActualCollationDelegation().HasFlag(currentLevel)
                ? entityType.GetCollation()
                // An explicitly defined charset on the current entity level takes precedence over an inherited collation.
                : GetActualModelCollation(entityType.Model, currentLevel) is string collation && // 1
                  (currentLevel != DelegationModes.ApplyToTables ||
                   entityType.GetCharSet() is not string charSet ||
                   !entityType.GetActualCharSetDelegation().HasFlag(DelegationModes.ApplyToTables) ||
                   collation.StartsWith(charSet, StringComparison.OrdinalIgnoreCase))
                    ? collation
                    : null;
        }

        protected virtual string GetActualPropertyCharSet(IProperty[] properties, DelegationModes currentLevel)
        {
            return properties.Select(p => p.GetCharSet()).FirstOrDefault(s => s is not null) ??
                   properties.Select(
                           p => p.FindTypeMapping() is MySqlStringTypeMapping {IsNationalChar: false}
                               // An explicitly defined collation on the current property level takes precedence over an inherited charset.
                               ? GetActualEntityTypeCharSet(p.DeclaringEntityType, currentLevel) is string charSet &&
                                 (p.GetCollation() is not string collation ||
                                  collation.StartsWith(charSet, StringComparison.OrdinalIgnoreCase))
                                   ? charSet
                                   : null
                               : null)
                       .FirstOrDefault(s => s is not null);
        }

        protected virtual string GetActualPropertyCollation(IProperty[] properties, DelegationModes currentLevel)
        {
            Debug.Assert(currentLevel == DelegationModes.ApplyToColumns);

            // We have been using the `MySql:Collation` annotation before EF Core added collation support.
            // Our `MySqlPropertyExtensions.GetMySqlLegacyCollation()` method handles the legacy case, so we explicitly
            // call it here and setup the relational annotation, even though EF Core sets it up as well.
            // This ensures, that from this point onwards, only the `Relational:Collation` annotation is being used.
            //
            // If no collation has been set, explicitly use the the model/database collation, if delegation is enabled.
            //
            // The exception are Guid properties when the GuidFormat has been set to a char-based format, in which case we will use the
            // default guid collation setup for the model, or the fallback default from our options if none has been set, to optimize space
            // and performance for those columns. (We ignore the binary format, because its charset and collation is always `binary`.)
            return properties.All(p => p.GetCollation() is null)
                ? properties.Select(p => p.GetMySqlLegacyCollation()).FirstOrDefault(c => c is not null) ??
                  properties.Select(
                          // An explicitly defined charset on the current property level takes precedence over an inherited collation.
                          p => (p.FindTypeMapping() is MySqlStringTypeMapping {IsNationalChar: false}
                                   ? GetActualEntityTypeCollation(p.DeclaringEntityType, currentLevel)
                                   : p.FindTypeMapping() is MySqlGuidTypeMapping {IsCharBasedStoreType: true}
                                       ? p.DeclaringEntityType.Model.GetActualGuidCollation(_options.DefaultGuidCollation)
                                       : null) is string collation &&
                               (p.GetCharSet() is not string charSet ||
                                collation.StartsWith(charSet, StringComparison.OrdinalIgnoreCase))
                              ? collation
                              : null)
                      .FirstOrDefault(s => s is not null)
                : null;
        }
    }
}
