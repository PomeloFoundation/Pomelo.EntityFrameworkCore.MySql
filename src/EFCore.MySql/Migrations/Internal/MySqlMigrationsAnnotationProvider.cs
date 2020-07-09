using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations.Internal
{
    public class MySqlMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        public MySqlMigrationsAnnotationProvider([NotNull] MigrationsAnnotationProviderDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IEnumerable<IAnnotation> For(IKey key)
        {
            var prefixLength = key.PrefixLength();
            if (prefixLength != null &&
                prefixLength.Length > 0)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.IndexPrefixLength,
                    prefixLength);
            }
        }

        public override IEnumerable<IAnnotation> For(IIndex index)
        {
            var prefixLength = index.PrefixLength();
            if (prefixLength != null &&
                prefixLength.Length > 0)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.IndexPrefixLength,
                    prefixLength);
            }

            var isFullText = index.IsFullText();
            if (isFullText.HasValue)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.FullTextIndex,
                    isFullText.Value);
            }

            var isSpatial = index.IsSpatial();
            if (isSpatial.HasValue)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.SpatialIndex,
                    isSpatial.Value);
            }
        }

        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            if (property.GetValueGenerationStrategy() == MySqlValueGenerationStrategy.IdentityColumn)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.ValueGenerationStrategy,
                    MySqlValueGenerationStrategy.IdentityColumn);
            }

            if (property.GetValueGenerationStrategy() == MySqlValueGenerationStrategy.ComputedColumn)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.ValueGenerationStrategy,
                    MySqlValueGenerationStrategy.ComputedColumn);
            }

            var charset = property.GetCharSet();
            if (charset != null)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.CharSet,
                    charset);
            }

            var collation = property.GetCollation();
            if (collation != null)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.Collation,
                    collation);
            }

            var srid = property.GetSpatialReferenceSystem();
            if (srid != null)
            {
                yield return new Annotation(
                    MySqlAnnotationNames.SpatialReferenceSystemId,
                    srid.Value);
            }
        }
    }
}
