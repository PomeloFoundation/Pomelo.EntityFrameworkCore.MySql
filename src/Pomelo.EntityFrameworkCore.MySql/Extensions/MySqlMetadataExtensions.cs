// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlMetadataExtensions
    {
        public static IRelationalEntityTypeAnnotations MySql([NotNull] this IEntityType entityType)
               => new RelationalEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)), MySqlFullAnnotationNames.Instance);

        public static RelationalEntityTypeAnnotations MySql([NotNull] this IMutableEntityType entityType)
            => (RelationalEntityTypeAnnotations)MySql((IEntityType)entityType);

        public static IRelationalForeignKeyAnnotations MySql([NotNull] this IForeignKey foreignKey)
            => new RelationalForeignKeyAnnotations(Check.NotNull(foreignKey, nameof(foreignKey)), MySqlFullAnnotationNames.Instance);

        public static RelationalForeignKeyAnnotations MySql([NotNull] this IMutableForeignKey foreignKey)
            => (RelationalForeignKeyAnnotations)MySql((IForeignKey)foreignKey);

        public static IMySqlIndexAnnotations MySql([NotNull] this IIndex index)
            => new MySqlIndexAnnotations(Check.NotNull(index, nameof(index)));

        public static RelationalIndexAnnotations MySql([NotNull] this IMutableIndex index)
            => (MySqlIndexAnnotations)MySql((IIndex)index);

        public static IRelationalKeyAnnotations MySql([NotNull] this IKey key)
            => new RelationalKeyAnnotations(Check.NotNull(key, nameof(key)), MySqlFullAnnotationNames.Instance);

        public static RelationalKeyAnnotations MySql([NotNull] this IMutableKey key)
            => (RelationalKeyAnnotations)MySql((IKey)key);

        public static IMySqlModelAnnotations MySql([NotNull] this IModel model)
            => new MySqlModelAnnotations(Check.NotNull(model, nameof(model)));

        public static MySqlModelAnnotations MySql([NotNull] this IMutableModel model)
            => (MySqlModelAnnotations)MySql((IModel)model);

        public static IRelationalPropertyAnnotations MySql([NotNull] this IProperty property)
            => new RelationalPropertyAnnotations(Check.NotNull(property, nameof(property)), MySqlFullAnnotationNames.Instance);

        public static RelationalPropertyAnnotations MySql([NotNull] this IMutableProperty property)
            => (RelationalPropertyAnnotations)MySql((IProperty)property);

    }
}
