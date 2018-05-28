// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class MySqlAnnotationCodeGeneratorTest
    {
        [Fact]
        public void GenerateFluentApi_IKey_works_when_clustered()
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.HasKey("Id").ForMySqlIsClustered();
                });
            var key = modelBuilder.Model.FindEntityType("Post").GetKeys().Single();
            var annotation = key.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(key, annotation);

            Assert.Equal("ForMySqlIsClustered", result.Method);

            Assert.Equal(0, result.Arguments.Count);
        }

        [Fact]
        public void GenerateFluentApi_IKey_works_when_nonclustered()
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.HasKey("Id").ForMySqlIsClustered(false);
                });
            var key = modelBuilder.Model.FindEntityType("Post").GetKeys().Single();
            var annotation = key.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(key, annotation);

            Assert.Equal("ForMySqlIsClustered", result.Method);

            Assert.Equal(1, result.Arguments.Count);
            Assert.Equal(false, result.Arguments[0]);
        }

        [Fact]
        public void GenerateFluentApi_IIndex_works_when_clustered()
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.Property<string>("Name");
                    x.HasIndex("Name").ForMySqlIsClustered();
                });
            var index = modelBuilder.Model.FindEntityType("Post").GetIndexes().Single();
            var annotation = index.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(index, annotation);

            Assert.Equal("ForMySqlIsClustered", result.Method);

            Assert.Equal(0, result.Arguments.Count);
        }

        [Fact]
        public void GenerateFluentApi_IIndex_works_when_nonclustered()
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.Property<string>("Name");
                    x.HasIndex("Name").ForMySqlIsClustered(false);
                });
            var index = modelBuilder.Model.FindEntityType("Post").GetIndexes().Single();
            var annotation = index.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(index, annotation);

            Assert.Equal("ForMySqlIsClustered", result.Method);

            Assert.Equal(1, result.Arguments.Count);
            Assert.Equal(false, result.Arguments[0]);
        }
    }
}
