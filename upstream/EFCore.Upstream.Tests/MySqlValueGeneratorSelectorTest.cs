// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable UnusedMember.Local
namespace Microsoft.EntityFrameworkCore
{
    public class MySqlValueGeneratorSelectorTest
    {
        [ConditionalFact]
        public void Returns_built_in_generators_for_types_setup_for_value_generation()
        {
            AssertGenerator<TemporaryIntValueGenerator>("Id");
            AssertGenerator<CustomValueGenerator>("Custom");
            AssertGenerator<TemporaryLongValueGenerator>("Long");
            AssertGenerator<TemporaryShortValueGenerator>("Short");
            AssertGenerator<TemporaryByteValueGenerator>("Byte");
            AssertGenerator<TemporaryIntValueGenerator>("NullableInt");
            AssertGenerator<TemporaryLongValueGenerator>("NullableLong");
            AssertGenerator<TemporaryShortValueGenerator>("NullableShort");
            AssertGenerator<TemporaryByteValueGenerator>("NullableByte");
            AssertGenerator<TemporaryDecimalValueGenerator>("Decimal");
            AssertGenerator<StringValueGenerator>("String");
            AssertGenerator<SequentialGuidValueGenerator>("Guid");
            AssertGenerator<BinaryValueGenerator>("Binary");
        }

        private void AssertGenerator<TExpected>(string propertyName, bool setSequences = false)
        {
            var builder = MySqlTestHelpers.Instance.CreateConventionBuilder();
            builder.Entity<AnEntity>(
                b =>
                {
                    b.Property(e => e.Custom).HasValueGenerator<CustomValueGenerator>();
                    b.Property(propertyName).ValueGeneratedOnAdd();
                    b.HasKey(propertyName);
                });

            if (setSequences)
            {
                builder.UseHiLo();
                Assert.NotNull(builder.Model.FindSequence(MySqlModelExtensions.DefaultHiLoSequenceName));
            }

            var model = builder.FinalizeModel();
            var entityType = model.FindEntityType(typeof(AnEntity));

            var selector = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

            Assert.IsType<TExpected>(selector.Select(entityType.FindProperty(propertyName), entityType));
        }

        [ConditionalFact]
        public void Returns_temp_guid_generator_when_default_sql_set()
        {
            var builder = MySqlTestHelpers.Instance.CreateConventionBuilder();
            builder.Entity<AnEntity>(
                b =>
                {
                    b.Property(e => e.Guid).HasDefaultValueSql("newid()");
                    b.HasKey(e => e.Guid);
                });
            var model = builder.FinalizeModel();
            var entityType = model.FindEntityType(typeof(AnEntity));

            var selector = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

            Assert.IsType<TemporaryGuidValueGenerator>(selector.Select(entityType.FindProperty("Guid"), entityType));
        }

        [ConditionalFact]
        public void Returns_temp_string_generator_when_default_sql_set()
        {
            var builder = MySqlTestHelpers.Instance.CreateConventionBuilder();
            builder.Entity<AnEntity>(
                b =>
                {
                    b.Property(e => e.String).ValueGeneratedOnAdd().HasDefaultValueSql("Foo");
                    b.HasKey(e => e.String);
                });
            var model = builder.FinalizeModel();
            var entityType = model.FindEntityType(typeof(AnEntity));

            var selector = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

            var generator = selector.Select(entityType.FindProperty("String"), entityType);
            Assert.IsType<StringValueGenerator>(generator);
            Assert.True(generator.GeneratesTemporaryValues);
        }

        [ConditionalFact]
        public void Returns_temp_binary_generator_when_default_sql_set()
        {
            var builder = MySqlTestHelpers.Instance.CreateConventionBuilder();
            builder.Entity<AnEntity>(
                b =>
                {
                    b.HasKey(e => e.Binary);
                    b.Property(e => e.Binary).HasDefaultValueSql("Foo").ValueGeneratedOnAdd();
                });
            var model = builder.FinalizeModel();
            var entityType = model.FindEntityType(typeof(AnEntity));

            var selector = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

            var generator = selector.Select(entityType.FindProperty("Binary"), entityType);
            Assert.IsType<BinaryValueGenerator>(generator);
            Assert.True(generator.GeneratesTemporaryValues);
        }

        [ConditionalFact]
        public void Returns_sequence_value_generators_when_configured_for_model()
        {
            AssertGenerator<MySqlSequenceHiLoValueGenerator<int>>("Id", setSequences: true);
            AssertGenerator<CustomValueGenerator>("Custom", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<long>>("Long", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<short>>("Short", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<byte>>("Byte", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<int>>("NullableInt", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<long>>("NullableLong", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<short>>("NullableShort", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<byte>>("NullableByte", setSequences: true);
            AssertGenerator<MySqlSequenceHiLoValueGenerator<decimal>>("Decimal", setSequences: true);
            AssertGenerator<StringValueGenerator>("String", setSequences: true);
            AssertGenerator<SequentialGuidValueGenerator>("Guid", setSequences: true);
            AssertGenerator<BinaryValueGenerator>("Binary", setSequences: true);
        }

        [ConditionalFact]
        public void Throws_for_unsupported_combinations()
        {
            var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();
            builder.Entity<AnEntity>(
                b =>
                {
                    b.Property(e => e.Random).ValueGeneratedOnAdd();
                    b.HasKey(e => e.Random);
                });
            var model = builder.FinalizeModel();
            var entityType = model.FindEntityType(typeof(AnEntity));

            var selector = InMemoryTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

            Assert.Equal(
                CoreStrings.NoValueGenerator("Random", "AnEntity", "Something"),
                Assert.Throws<NotSupportedException>(() => selector.Select(entityType.FindProperty("Random"), entityType)).Message);
        }

        [ConditionalFact]
        public void Returns_generator_configured_on_model_when_property_is_identity()
        {
            var builder = MySqlTestHelpers.Instance.CreateConventionBuilder();

            builder.Entity<AnEntity>();

            builder
                .UseHiLo()
                .HasSequence<int>(MySqlModelExtensions.DefaultHiLoSequenceName);

            var model = builder.UseHiLo().FinalizeModel();
            var entityType = model.FindEntityType(typeof(AnEntity));

            var selector = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

            Assert.IsType<MySqlSequenceHiLoValueGenerator<int>>(selector.Select(entityType.FindProperty("Id"), entityType));
        }

        private class AnEntity
        {
            public int Id { get; set; }
            public int Custom { get; set; }
            public long Long { get; set; }
            public short Short { get; set; }
            public byte Byte { get; set; }
            public int? NullableInt { get; set; }
            public long? NullableLong { get; set; }
            public short? NullableShort { get; set; }
            public byte? NullableByte { get; set; }
            public string String { get; set; }
            public Guid Guid { get; set; }
            public byte[] Binary { get; set; }
            public float Float { get; set; }
            public decimal Decimal { get; set; }

            [NotMapped]
            public Something Random { get; set; }
        }

        private struct Something
        {
            public int Id { get; set; }
        }

        private class CustomValueGenerator : ValueGenerator<int>
        {
            public override int Next(EntityEntry entry) => throw new NotImplementedException();
            public override bool GeneratesTemporaryValues => false;
        }
    }
}
