// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Extensions;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Internal
{
    public class MySqlAnnotationProvider : RelationalAnnotationProvider
    {
        public MySqlAnnotationProvider([NotNull] RelationalAnnotationProviderDependencies dependencies)
            : base(dependencies)
        {
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

            if (column.PropertyMappings.Select(m => m.Property.GetCharSet())
                .FirstOrDefault(c => c != null) is string charset)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charset);
            }

            if (column.PropertyMappings.Select(m => m.Property.GetCollation())
                .FirstOrDefault(c => c != null) is string collation)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.Collation,
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
