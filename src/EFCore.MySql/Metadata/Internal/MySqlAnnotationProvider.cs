// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Internal
{
    public class MySqlAnnotationProvider : RelationalAnnotationProvider
    {
        public MySqlAnnotationProvider([NotNull] RelationalAnnotationProviderDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IEnumerable<IAnnotation> For(IRelationalModel model)
            => model.GetAnnotations()
                .Where(a => a.Name == MySqlAnnotationNames.CharSet ||
                            a.Name == MySqlAnnotationNames.CharSetDelegation ||
                            a.Name == MySqlAnnotationNames.CollationDelegation);

        public override IEnumerable<IAnnotation> For(ITable table)
            => table.EntityTypeMappings.First()
                .EntityType.GetAnnotations()
                .Where(
                    a => a.Name == MySqlAnnotationNames.CharSet && a.Value != null ||
                         a.Name == RelationalAnnotationNames.Collation && a.Value != null);

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

            // Use the an explicitly defined character set, if set.
            // Otherwise, explicitly use the the model/database character set, if delegation is enabled.
            var charSet = column.PropertyMappings.Select(m => m.Property.GetCharSet()).FirstOrDefault(c => c != null) ??
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
