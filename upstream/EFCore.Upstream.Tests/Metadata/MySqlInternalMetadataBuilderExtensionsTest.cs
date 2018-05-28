// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlInternalMetadataBuilderExtensionsTest
    {
        private InternalModelBuilder CreateBuilder()
            => new InternalModelBuilder(new Model());

        [Fact]
        public void Can_access_model()
        {
            var builder = CreateBuilder();

            Assert.True(builder.MySql(ConfigurationSource.Convention).ValueGenerationStrategy(MySqlValueGenerationStrategy.SequenceHiLo));
            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, builder.Metadata.MySql().ValueGenerationStrategy);

            Assert.True(builder.MySql(ConfigurationSource.DataAnnotation).ValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn));
            Assert.Equal(MySqlValueGenerationStrategy.IdentityColumn, builder.Metadata.MySql().ValueGenerationStrategy);

            Assert.False(builder.MySql(ConfigurationSource.Convention).ValueGenerationStrategy(MySqlValueGenerationStrategy.SequenceHiLo));
            Assert.Equal(MySqlValueGenerationStrategy.IdentityColumn, builder.Metadata.MySql().ValueGenerationStrategy);

            Assert.Equal(
                1, builder.Metadata.GetAnnotations().Count(
                    a => a.Name.StartsWith(MySqlAnnotationNames.Prefix, StringComparison.Ordinal)));
        }

        [Fact]
        public void Can_access_entity_type()
        {
            var typeBuilder = CreateBuilder().Entity(typeof(Splot), ConfigurationSource.Convention);

            Assert.True(typeBuilder.MySql(ConfigurationSource.Convention).ToTable("Splew"));
            Assert.Equal("Splew", typeBuilder.Metadata.MySql().TableName);

            Assert.True(typeBuilder.MySql(ConfigurationSource.DataAnnotation).ToTable("Splow"));
            Assert.Equal("Splow", typeBuilder.Metadata.MySql().TableName);

            Assert.False(typeBuilder.MySql(ConfigurationSource.Convention).ToTable("Splod"));
            Assert.Equal("Splow", typeBuilder.Metadata.MySql().TableName);

            Assert.Equal(
                1, typeBuilder.Metadata.GetAnnotations().Count(
                    a => a.Name.StartsWith(RelationalAnnotationNames.Prefix, StringComparison.Ordinal)));
        }

        [Fact]
        public void Can_access_property()
        {
            var propertyBuilder = CreateBuilder()
                .Entity(typeof(Splot), ConfigurationSource.Convention)
                .Property("Id", typeof(int), ConfigurationSource.Convention);

            Assert.True(propertyBuilder.MySql(ConfigurationSource.Convention).HiLoSequenceName("Splew"));
            Assert.Equal("Splew", propertyBuilder.Metadata.MySql().HiLoSequenceName);

            Assert.True(propertyBuilder.MySql(ConfigurationSource.DataAnnotation).HiLoSequenceName("Splow"));
            Assert.Equal("Splow", propertyBuilder.Metadata.MySql().HiLoSequenceName);

            Assert.False(propertyBuilder.MySql(ConfigurationSource.Convention).HiLoSequenceName("Splod"));
            Assert.Equal("Splow", propertyBuilder.Metadata.MySql().HiLoSequenceName);

            Assert.Equal(
                1, propertyBuilder.Metadata.GetAnnotations().Count(
                    a => a.Name.StartsWith(MySqlAnnotationNames.Prefix, StringComparison.Ordinal)));
        }

        [Fact]
        public void Throws_setting_sequence_generation_for_invalid_type_only_with_explicit()
        {
            var propertyBuilder = CreateBuilder()
                .Entity(typeof(Splot), ConfigurationSource.Convention)
                .Property("Name", typeof(string), ConfigurationSource.Convention);

            Assert.False(
                propertyBuilder.MySql(ConfigurationSource.Convention)
                    .ValueGenerationStrategy(MySqlValueGenerationStrategy.SequenceHiLo));

            Assert.Equal(
                MySqlStrings.SequenceBadType("Name", nameof(Splot), "string"),
                Assert.Throws<ArgumentException>(
                    () => propertyBuilder.MySql(ConfigurationSource.Explicit).ValueGenerationStrategy(MySqlValueGenerationStrategy.SequenceHiLo)).Message);
        }

        [Fact]
        public void Throws_setting_identity_generation_for_invalid_type_only_with_explicit()
        {
            var propertyBuilder = CreateBuilder()
                .Entity(typeof(Splot), ConfigurationSource.Convention)
                .Property("Name", typeof(string), ConfigurationSource.Convention);

            Assert.False(
                propertyBuilder.MySql(ConfigurationSource.Convention)
                    .ValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn));

            Assert.Equal(
                MySqlStrings.IdentityBadType("Name", nameof(Splot), "string"),
                Assert.Throws<ArgumentException>(
                    () => propertyBuilder.MySql(ConfigurationSource.Explicit).ValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn)).Message);
        }

        [Fact]
        public void Can_access_key()
        {
            var modelBuilder = CreateBuilder();
            var entityTypeBuilder = modelBuilder.Entity(typeof(Splot), ConfigurationSource.Convention);
            var idProperty = entityTypeBuilder.Property("Id", typeof(string), ConfigurationSource.Convention).Metadata;
            var keyBuilder = entityTypeBuilder.HasKey(new[] { idProperty.Name }, ConfigurationSource.Convention);

            Assert.True(keyBuilder.MySql(ConfigurationSource.Convention).IsClustered(true));
            Assert.True(keyBuilder.Metadata.MySql().IsClustered);

            Assert.True(keyBuilder.MySql(ConfigurationSource.DataAnnotation).IsClustered(false));
            Assert.False(keyBuilder.Metadata.MySql().IsClustered);

            Assert.False(keyBuilder.MySql(ConfigurationSource.Convention).IsClustered(true));
            Assert.False(keyBuilder.Metadata.MySql().IsClustered);

            Assert.Equal(
                1, keyBuilder.Metadata.GetAnnotations().Count(
                    a => a.Name.StartsWith(MySqlAnnotationNames.Prefix, StringComparison.Ordinal)));
        }

        [Fact]
        public void Can_access_index()
        {
            var modelBuilder = CreateBuilder();
            var entityTypeBuilder = modelBuilder.Entity(typeof(Splot), ConfigurationSource.Convention);
            entityTypeBuilder.Property("Id", typeof(int), ConfigurationSource.Convention);
            var indexBuilder = entityTypeBuilder.HasIndex(new[] { "Id" }, ConfigurationSource.Convention);

            Assert.True(indexBuilder.MySql(ConfigurationSource.Convention).IsClustered(true));
            Assert.True(indexBuilder.Metadata.MySql().IsClustered);

            Assert.True(indexBuilder.MySql(ConfigurationSource.DataAnnotation).IsClustered(false));
            Assert.False(indexBuilder.Metadata.MySql().IsClustered);

            Assert.False(indexBuilder.MySql(ConfigurationSource.Convention).IsClustered(true));
            Assert.False(indexBuilder.Metadata.MySql().IsClustered);

            Assert.Equal(
                1, indexBuilder.Metadata.GetAnnotations().Count(
                    a => a.Name.StartsWith(MySqlAnnotationNames.Prefix, StringComparison.Ordinal)));
        }

        [Fact]
        public void Can_access_relationship()
        {
            var modelBuilder = CreateBuilder();
            var entityTypeBuilder = modelBuilder.Entity(typeof(Splot), ConfigurationSource.Convention);
            entityTypeBuilder.Property("Id", typeof(int), ConfigurationSource.Convention);
            var relationshipBuilder = entityTypeBuilder.HasForeignKey("Splot", new[] { "Id" }, ConfigurationSource.Convention);

            Assert.True(relationshipBuilder.MySql(ConfigurationSource.Convention).HasConstraintName("Splew"));
            Assert.Equal("Splew", relationshipBuilder.Metadata.Relational().Name);

            Assert.True(relationshipBuilder.MySql(ConfigurationSource.DataAnnotation).HasConstraintName("Splow"));
            Assert.Equal("Splow", relationshipBuilder.Metadata.Relational().Name);

            Assert.False(relationshipBuilder.MySql(ConfigurationSource.Convention).HasConstraintName("Splod"));
            Assert.Equal("Splow", relationshipBuilder.Metadata.Relational().Name);

            Assert.Equal(
                1, relationshipBuilder.Metadata.GetAnnotations().Count(
                    a => a.Name.StartsWith(RelationalAnnotationNames.Prefix, StringComparison.Ordinal)));
        }

        private class Splot
        {
        }
    }
}
