// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class MySqlInternalMetadataBuilderExtensions
    {
        public static RelationalModelBuilderAnnotations MySql(
            [NotNull] this InternalModelBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalModelBuilderAnnotations(builder, configurationSource, MySqlFullAnnotationNames.Instance);

        public static RelationalPropertyBuilderAnnotations MySql(
            [NotNull] this InternalPropertyBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalPropertyBuilderAnnotations(builder, configurationSource, MySqlFullAnnotationNames.Instance);

        public static RelationalEntityTypeBuilderAnnotations MySql(
            [NotNull] this InternalEntityTypeBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalEntityTypeBuilderAnnotations(builder, configurationSource, MySqlFullAnnotationNames.Instance);

        public static RelationalKeyBuilderAnnotations MySql(
            [NotNull] this InternalKeyBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalKeyBuilderAnnotations(builder, configurationSource, MySqlFullAnnotationNames.Instance);

        public static RelationalIndexBuilderAnnotations MySql(
            [NotNull] this InternalIndexBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalIndexBuilderAnnotations(builder, configurationSource, MySqlFullAnnotationNames.Instance);

        public static RelationalForeignKeyBuilderAnnotations MySql(
            [NotNull] this InternalRelationshipBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalForeignKeyBuilderAnnotations(builder, configurationSource, MySqlFullAnnotationNames.Instance);
    }
}