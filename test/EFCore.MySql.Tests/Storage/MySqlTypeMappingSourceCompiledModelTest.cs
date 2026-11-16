// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.Storage
{
    /// <summary>
    ///     Guards the invariant that EF Core 11 compiled models rely on.
    /// </summary>
    /// <remarks>
    ///     EF Core 11 generates compiled models as "TypeMappingClass.Default.Clone(...)" and
    ///     RelationalTypeMapping.Clone() no longer accepts a CLR type, so it always preserves the CLR type of the instance it
    ///     is cloned from. A mapping class that serves more than one CLR type therefore silently produces properties whose
    ///     comparers belong to the wrong CLR type, which aborts the process while the compiled model is being initialized.
    /// </remarks>
    public class MySqlTypeMappingSourceCompiledModelTest
    {
        public static TheoryData<Type, string> MappedStoreTypes { get; } = new()
        {
            { typeof(TimeOnly), "time" },
            { typeof(TimeOnly), "time(6)" },
            { typeof(TimeSpan), "time" },
            { typeof(TimeSpan), "time(6)" },
            { typeof(DateOnly), "date" },
            { typeof(DateTime), "date" },
            { typeof(DateTime), "datetime(6)" },
            { typeof(DateTimeOffset), "timestamp(6)" },
        };

        [Theory]
        [MemberData(nameof(MappedStoreTypes))]
        public void Type_mapping_is_compatible_with_compiled_models(Type clrType, string storeType)
        {
            var mapping = GetTypeMappingSource().FindMapping(clrType, storeType);

            Assert.NotNull(mapping);
            AssertCompiledModelCompatible(mapping);
        }

        public static TheoryData<Type> MappedClrTypes { get; } = new()
        {
            typeof(TimeOnly), typeof(TimeSpan), typeof(DateOnly), typeof(DateTime), typeof(DateTimeOffset)
        };

        [Theory]
        [MemberData(nameof(MappedClrTypes))]
        public void Default_type_mapping_is_compatible_with_compiled_models(Type clrType)
        {
            var mapping = GetTypeMappingSource().FindMapping(clrType);

            Assert.NotNull(mapping);
            AssertCompiledModelCompatible(mapping);
        }

        private static IRelationalTypeMappingSource GetTypeMappingSource()
        {
            var options = new DbContextOptionsBuilder()
                .UseMySql("server=localhost;database=none;user=none", ServerVersion.Parse("8.0.40-mysql"))
                .Options;

            // No connection is ever opened; only the service provider is needed.
            using var context = new DbContext(options);
            return context.GetService<IRelationalTypeMappingSource>();
        }

        // Mirrors CSharpRuntimeAnnotationCodeGenerator.CreateDefaultTypeMapping.
        private static void AssertCompiledModelCompatible(RelationalTypeMapping mapping)
        {
            var mappingType = mapping.GetType();

            Assert.True(mappingType.IsPublic, $"'{mappingType.Name}' must be public to be usable from a compiled model.");

            var defaultProperty = mappingType.GetProperty("Default");

            Assert.True(
                defaultProperty?.GetMethod is { IsStatic: true, IsPublic: true }
                && mappingType.IsAssignableFrom(defaultProperty.PropertyType),
                $"'{mappingType.Name}' must declare a public static 'Default' property of its own type.");

            var defaultMapping = (RelationalTypeMapping)defaultProperty.GetValue(null);

            Assert.Equal(mapping.ClrType, defaultMapping.ClrType);
        }
    }
}
