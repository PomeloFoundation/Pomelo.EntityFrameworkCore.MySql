// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Design
{
    /// <summary>
    /// Tests for <see cref="MySqlAnnotationCodeGenerator"/> ensuring that generated
    /// MethodCallCodeFragment instances produce fluent chaining output instead of static method calls.
    /// </summary>
    /// <remarks>
    /// Fixes: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1990
    /// </remarks>
    public class MySqlAnnotationCodeGeneratorTest
    {
        [Fact]
        public void GenerateFluentApiCalls_for_property_identity_column_has_null_MethodInfo()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            modelBuilder.Entity("TestEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, MySqlValueGenerationStrategy.IdentityColumn);
            });

            var model = modelBuilder.FinalizeModel();
            var property = model.FindEntityType("TestEntity")!.FindProperty("Id")!;

            var annotations = property.GetAnnotations()
                .ToDictionary(a => a.Name, a => a);

            var generator = CreateGenerator();

            // Act
            var fragments = generator.GenerateFluentApiCalls(property, annotations);

            // Assert
            var identityFragment = fragments.FirstOrDefault(f => f.Method == "UseMySqlIdentityColumn");
            Assert.NotNull(identityFragment);

            // Key assertion: MethodInfo must be null for fluent chaining
            // When MethodInfo is null, EF Core's CSharpSnapshotGenerator outputs fluent chaining
            // instead of static method calls like MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(...)
            Assert.Null(identityFragment.MethodInfo);
        }

        [Fact]
        public void GenerateFluentApiCalls_for_property_computed_column_has_null_MethodInfo()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            modelBuilder.Entity("TestEntity", b =>
            {
                b.Property<int>("ComputedValue")
                    .HasAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, MySqlValueGenerationStrategy.ComputedColumn);
            });

            var model = modelBuilder.FinalizeModel();
            var property = model.FindEntityType("TestEntity")!.FindProperty("ComputedValue")!;

            var annotations = property.GetAnnotations()
                .ToDictionary(a => a.Name, a => a);

            var generator = CreateGenerator();

            // Act
            var fragments = generator.GenerateFluentApiCalls(property, annotations);

            // Assert
            var computedFragment = fragments.FirstOrDefault(f => f.Method == "UseMySqlComputedColumn");
            Assert.NotNull(computedFragment);
            Assert.Null(computedFragment.MethodInfo);
        }

        [Fact]
        public void GenerateFluentApiCalls_for_model_auto_increment_has_null_MethodInfo()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            modelBuilder.HasAnnotation(
                MySqlAnnotationNames.ValueGenerationStrategy,
                MySqlValueGenerationStrategy.IdentityColumn);

            var model = modelBuilder.FinalizeModel();

            var annotations = model.GetAnnotations()
                .ToDictionary(a => a.Name, a => a);

            var generator = CreateGenerator();

            // Act
            var fragments = generator.GenerateFluentApiCalls(model, annotations);

            // Assert
            var autoIncrementFragment = fragments.FirstOrDefault(f => f.Method == "AutoIncrementColumns");
            Assert.NotNull(autoIncrementFragment);
            Assert.Null(autoIncrementFragment.MethodInfo);
        }

        private static MySqlAnnotationCodeGenerator CreateGenerator()
        {
            var typeMappingSourceMock = new Mock<IRelationalTypeMappingSource>();
            var dependencies = new AnnotationCodeGeneratorDependencies(typeMappingSourceMock.Object);

            return new MySqlAnnotationCodeGenerator(dependencies);
        }
    }
}
