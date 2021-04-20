// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
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

        public override IEnumerable<IAnnotation> For(IRelationalModel model)
        {
            // If neither character set nor collation has been explicitly defined for the model, we use Pomelo's universal fallback default character set,
            // which is `utf8mb4`.
            if (model.Model.GetAnnotations().All(a => (a.Name != MySqlAnnotationNames.CharSet || a.Value is not string) &&
                                                      (a.Name != RelationalAnnotationNames.Collation || a.Value is not string)) &&
                _options.DefaultCharSet?.Name is not null)
            {
                yield return new Annotation(MySqlAnnotationNames.CharSet, _options.DefaultCharSet.Name);
            }

            foreach (var annotation in model.Model.GetAnnotations()
                .Where(
                    a => a.Name == MySqlAnnotationNames.CharSet ||
                         a.Name == MySqlAnnotationNames.CharSetDelegation ||
                         a.Name == MySqlAnnotationNames.CollationDelegation))
            {
                yield return annotation;
            }
        }

        public override IEnumerable<IAnnotation> For(ITable table)
        {
            // Model validation ensures that these facets are the same on all mapped entity types
            var entityType = table.EntityTypeMappings.First().EntityType;

            // Use an explicitly defined character set, if set.
            // Otherwise, explicitly use the model/database character set, if delegation is enabled.
            var charSet = entityType.GetCharSet() ??
                          (entityType.Model.GetCharSetDelegation().GetValueOrDefault(true)
                              ? entityType.Model.GetCharSet()
                              : null);

            if (charSet is not null)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charSet);
            }

            // Use an explicitly defined collation, if set.
            // Otherwise, explicitly use the model/database collation, if delegation is enabled.
            var collation = entityType.GetCollation() ??
                            (entityType.Model.GetCollationDelegation().GetValueOrDefault(true)
                                ? entityType.Model.GetCollation()
                                : null);

            if (collation is not null)
            {
                yield return new Annotation(
                    RelationalAnnotationNames.Collation,
                    collation);
            }

            // Handle other annotations (including the delegation annotations).
            foreach (var annotation in entityType.GetAnnotations()
                .Where(
                    a => (a.Name == MySqlAnnotationNames.CharSetDelegation && a.Value != null) ||
                         (a.Name == MySqlAnnotationNames.CollationDelegation && a.Value != null)))
            {
                yield return annotation;
            }
        }

        public override IEnumerable<IAnnotation> For(IUniqueConstraint constraint)
        {
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

        public override IEnumerable<IAnnotation> For(ITableIndex index)
        {
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

        public override IEnumerable<IAnnotation> For(IColumn column)
        {
            if (column.PropertyMappings.Select(m => m.Property)
                .FirstOrDefault(p => p.GetValueGenerationStrategy() != null &&
                                     p.GetValueGenerationStrategy() != MySqlValueGenerationStrategy.None) is IProperty property)
            {
                var valueGenerationStrategy = property.GetValueGenerationStrategy();
                yield return new Annotation(
                    MySqlAnnotationNames.ValueGenerationStrategy,
                    valueGenerationStrategy);
            }

            // Use an explicitly defined character set, if set.
            // Otherwise, explicitly use the entity/table or model/database character set, if delegation is enabled.
            var charSet = column.PropertyMappings.Select(m => m.Property.GetCharSet()).FirstOrDefault(c => c != null) ??
                          column.PropertyMappings.Select(
                                  m => m.Property.FindTypeMapping() is MySqlStringTypeMapping &&
                                       m.Property.DeclaringEntityType.GetCharSetDelegation().GetValueOrDefault(true)
                                      ? m.Property.DeclaringEntityType.GetCharSet()
                                      : null)
                              .FirstOrDefault(c => c != null) ??
                          column.PropertyMappings.Select(
                                  m => m.Property.FindTypeMapping() is MySqlStringTypeMapping &&
                                       m.Property.DeclaringEntityType.Model.GetCharSetDelegation().GetValueOrDefault(true)
                                      ? m.Property.DeclaringEntityType.Model.GetCharSet()
                                      : null)
                              .FirstOrDefault(c => c != null);

            if (charSet is not null)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charSet);
            }

            // We have been using the `MySql:Collation` annotation before EF Core added collation support.
            // Our `MySqlPropertyExtensions.GetMySqlLegacyCollation()` method handles the legacy case, so we explicitly
            // call it here and setup the relational annotation, even though EF Core sets it up as well.
            // This ensures, that from this point onwards, only the `Relational:Collation` annotation is being used.
            // If no collation has been set, explicitly use the the model/database collation, if delegation is enabled.
            var collation = column.PropertyMappings.All(m => m.Property.GetCollation() is null)
                ? column.PropertyMappings.Select(
                          m => m.Property.GetMySqlLegacyCollation())
                      .FirstOrDefault(c => c != null) ??
                  column.PropertyMappings.Select(
                          m => m.Property.FindTypeMapping() is MySqlStringTypeMapping &&
                               m.Property.DeclaringEntityType.GetCollationDelegation().GetValueOrDefault(true)
                              ? m.Property.DeclaringEntityType.GetCollation()
                              : null)
                      .FirstOrDefault(c => c != null) ??
                  column.PropertyMappings.Select(
                          m => m.Property.FindTypeMapping() is MySqlStringTypeMapping &&
                               m.Property.DeclaringEntityType.Model.GetCollationDelegation().GetValueOrDefault(true)
                              ? m.Property.DeclaringEntityType.Model.GetCollation()
                              : null)
                      .FirstOrDefault(c => c != null)
                : null;

            if (collation is not null)
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
    }
}
