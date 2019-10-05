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
        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void GenerateFluentApi_IKey_works_when_clustered(bool obsolete)
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");

                    if (obsolete)
                    {
#pragma warning disable 618
                        x.HasKey("Id").ForMySqlIsClustered();
#pragma warning restore 618
                    }
                    else
                    {
                        x.HasKey("Id").IsClustered();
                    }
                });
            var key = modelBuilder.Model.FindEntityType("Post").GetKeys().Single();
            var annotation = key.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(key, annotation);

            Assert.Equal("IsClustered", result.Method);

            Assert.Equal(0, result.Arguments.Count);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void GenerateFluentApi_IKey_works_when_nonclustered(bool obsolete)
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");

                    if (obsolete)
                    {
#pragma warning disable 618
                        x.HasKey("Id").ForMySqlIsClustered(false);
#pragma warning restore 618
                    }
                    else
                    {
                        x.HasKey("Id").IsClustered(false);
                    }
                });
            var key = modelBuilder.Model.FindEntityType("Post").GetKeys().Single();
            var annotation = key.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(key, annotation);

            Assert.Equal("IsClustered", result.Method);

            Assert.Equal(1, result.Arguments.Count);
            Assert.Equal(false, result.Arguments[0]);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void GenerateFluentApi_IIndex_works_when_clustered(bool obsolete)
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.Property<string>("Name");
                    if (obsolete)
                    {
#pragma warning disable 618
                        x.HasIndex("Name").ForMySqlIsClustered();
#pragma warning restore 618
                    }
                    else
                    {
                        x.HasIndex("Name").IsClustered();
                    }
                });
            var index = modelBuilder.Model.FindEntityType("Post").GetIndexes().Single();
            var annotation = index.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(index, annotation);

            Assert.Equal("IsClustered", result.Method);

            Assert.Equal(0, result.Arguments.Count);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void GenerateFluentApi_IIndex_works_when_nonclustered(bool obsolete)
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.Property<string>("Name");
                    if (obsolete)
                    {
#pragma warning disable 618
                        x.HasIndex("Name").ForMySqlIsClustered(false);
#pragma warning restore 618
                    }
                    else
                    {
                        x.HasIndex("Name").IsClustered(false);
                    }
                });
            var index = modelBuilder.Model.FindEntityType("Post").GetIndexes().Single();
            var annotation = index.FindAnnotation(MySqlAnnotationNames.Clustered);

            var result = generator.GenerateFluentApi(index, annotation);

            Assert.Equal("IsClustered", result.Method);

            Assert.Equal(1, result.Arguments.Count);
            Assert.Equal(false, result.Arguments[0]);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void GenerateFluentApi_IIndex_works_with_includes(bool obsolete)
        {
            var generator = new MySqlAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies());
            var modelBuilder = new ModelBuilder(MySqlConventionSetBuilder.Build());
            modelBuilder.Entity(
                "Post",
                x =>
                {
                    x.Property<int>("Id");
                    x.Property<string>("FirstName");
                    x.Property<string>("LastName");
                    if (obsolete)
                    {
#pragma warning disable 618
                        x.HasIndex("LastName").ForMySqlInclude("FirstName");
#pragma warning restore 618
                    }
                    else
                    {
                        x.HasIndex("LastName").IncludeProperties("FirstName");
                    }
                });
            var index = modelBuilder.Model.FindEntityType("Post").GetIndexes().Single();
            var annotation = index.FindAnnotation(MySqlAnnotationNames.Include);

            var result = generator.GenerateFluentApi(index, annotation);

            Assert.Equal("IncludeProperties", result.Method);

            Assert.Equal(1, result.Arguments.Count);
            var properties = Assert.IsType<string[]>(result.Arguments[0]);
            Assert.Equal(new[] { "FirstName" }, properties.AsEnumerable());
        }
    }
}
