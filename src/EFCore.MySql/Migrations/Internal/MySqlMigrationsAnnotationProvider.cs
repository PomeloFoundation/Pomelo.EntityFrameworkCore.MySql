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

        public override IEnumerable<IAnnotation> For(IModel model) => base.For(model);

        public override IEnumerable<IAnnotation> For(IEntityType entityType) => base.For(entityType);

        public override IEnumerable<IAnnotation> For(IIndex index)
        {
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
        }
    }
}
