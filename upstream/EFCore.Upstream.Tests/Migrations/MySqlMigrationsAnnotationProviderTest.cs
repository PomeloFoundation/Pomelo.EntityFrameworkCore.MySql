// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations.Internal
{
    public class MySqlMigrationsAnnotationProviderTest
    {
        private readonly ModelBuilder _modelBuilder;
        private readonly MySqlMigrationsAnnotationProvider _annotations;

        public MySqlMigrationsAnnotationProviderTest()
        {
            _modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder(/*skipValidation: true*/);
            _annotations = new MySqlMigrationsAnnotationProvider(new MigrationsAnnotationProviderDependencies());
        }

        [Fact]
        public void For_property_handles_identity_annotations()
        {
            var property = _modelBuilder.Entity("Entity")
                .Property<int>("Id").UseIdentityColumn(2, 3)
                .Metadata;

            var migrationAnnotations = _annotations.For(property).ToList();

            var identity = Assert.Single(migrationAnnotations, a => a.Name == MySqlAnnotationNames.Identity);
            Assert.Equal("2, 3", identity.Value);
        }

        [ConditionalFact]
        public void Resolves_column_names_for_Index_with_included_properties()
        {
            _modelBuilder.Entity<Entity>().Property(e => e.IncludedProp).HasColumnName("IncludedColumn");
            var index = _modelBuilder.Entity<Entity>().HasIndex(e => e.IndexedProp).IncludeProperties(e => e.IncludedProp).Metadata;
            _modelBuilder.FinalizeModel();

            Assert.Contains(_annotations.For(index), a => a.Name == MySqlAnnotationNames.Include && ((string[])a.Value).Contains("IncludedColumn"));
        }

        private class Entity
        {
            public int Id { get; set; }
            public string IndexedProp { get; set; }
            public string IncludedProp { get; set; }
        }
    }
}
