// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Metadata.Internal;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.Metadata
{
    public class MySqlAnnotationProviderTest
    {
        [ConditionalTheory]
        [InlineData(null, true)]
        [InlineData("varchar(255)", true)]
        [InlineData("longtext", true)]
        // Non-character based store types (e.g. `vector`, which has been introduced in MariaDB 11.7 and MySQL 9.0) do not
        // support the `CHARACTER SET` and `COLLATE` column attributes.
        [InlineData("vector(312)", false)]
        [InlineData("uuid", false)]
        [InlineData("inet6", false)]
        public void Column_charset_is_only_inherited_by_character_based_store_types(string storeType, bool expectCharSet)
        {
            var column = GetColumn(
                modelBuilder => modelBuilder.Entity<Blog>(
                    entity =>
                    {
                        var propertyBuilder = entity.Property(e => e.Embedding);

                        if (storeType is not null)
                        {
                            propertyBuilder.HasColumnType(storeType);
                        }
                    }),
                nameof(Blog.Embedding));

            if (expectCharSet)
            {
                Assert.NotNull(column[MySqlAnnotationNames.CharSet]);
            }
            else
            {
                Assert.Null(column[MySqlAnnotationNames.CharSet]);
                Assert.Null(column[RelationalAnnotationNames.Collation]);
                Assert.Null(column.Collation);
            }
        }

        [ConditionalFact]
        public void Column_collation_is_not_inherited_by_non_character_based_store_types()
        {
            var column = GetColumn(
                modelBuilder =>
                {
                    modelBuilder.UseCollation("utf8mb4_bin");
                    modelBuilder.Entity<Blog>(
                        entity => entity.Property(e => e.Embedding)
                            .HasColumnType("vector(312)"));
                },
                nameof(Blog.Embedding));

            Assert.Null(column[MySqlAnnotationNames.CharSet]);
            Assert.Null(column[RelationalAnnotationNames.Collation]);
            Assert.Null(column.Collation);
        }

        private static IColumn GetColumn(Action<ModelBuilder> buildAction, string columnName)
        {
            var services = MySqlTestHelpers.Instance.CreateContextServices(b => { });
            var modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder(services);

            buildAction(modelBuilder);

            var model = services.GetRequiredService<IModelRuntimeInitializer>()
                .Initialize(modelBuilder.FinalizeModel(), designTime: true, validationLogger: null);

            return model.GetRelationalModel()
                .Tables
                .Single()
                .Columns
                .Single(c => c.Name == columnName);
        }

        private class Blog
        {
            public int BlogId { get; set; }
            public string Embedding { get; set; }
        }
    }
}
