// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlEntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder ForMySqlToTable(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string name)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            var relationalEntityTypeBuilder = ((IInfrastructure<InternalEntityTypeBuilder>)entityTypeBuilder).GetInfrastructure()
                .MySql(ConfigurationSource.Explicit);
            relationalEntityTypeBuilder.TableName = name;

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<TEntity> ForMySqlToTable<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string name)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)ForMySqlToTable((EntityTypeBuilder)entityTypeBuilder, name);

        public static EntityTypeBuilder ForMySqlToTable(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var relationalEntityTypeBuilder = ((IInfrastructure<InternalEntityTypeBuilder>)entityTypeBuilder).GetInfrastructure()
                .MySql(ConfigurationSource.Explicit);
            relationalEntityTypeBuilder.TableName = name;
            relationalEntityTypeBuilder.Schema = schema;

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<TEntity> ForMySqlToTable<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)ForMySqlToTable((EntityTypeBuilder)entityTypeBuilder, name, schema);
    }
}
