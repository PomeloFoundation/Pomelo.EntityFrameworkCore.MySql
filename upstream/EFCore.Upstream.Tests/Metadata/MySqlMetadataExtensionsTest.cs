// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlMetadataExtensionsTest
    {
        [Fact]
        public void Can_get_and_set_column_name()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Name)
                .Metadata;

            Assert.Equal("Name", property.MySql().ColumnName);
            Assert.Equal("Name", ((IProperty)property).MySql().ColumnName);

            property.Relational().ColumnName = "Eman";

            Assert.Equal("Name", property.Name);
            Assert.Equal("Eman", property.Relational().ColumnName);
            Assert.Equal("Eman", property.MySql().ColumnName);
            Assert.Equal("Eman", ((IProperty)property).MySql().ColumnName);

            property.MySql().ColumnName = "MyNameIs";

            Assert.Equal("Name", property.Name);
            Assert.Equal("MyNameIs", property.Relational().ColumnName);
            Assert.Equal("MyNameIs", property.MySql().ColumnName);
            Assert.Equal("MyNameIs", ((IProperty)property).MySql().ColumnName);

            property.MySql().ColumnName = null;

            Assert.Equal("Name", property.Name);
            Assert.Equal("Name", property.Relational().ColumnName);
            Assert.Equal("Name", property.MySql().ColumnName);
            Assert.Equal("Name", ((IProperty)property).MySql().ColumnName);
        }

        [Fact]
        public void Can_get_and_set_table_name()
        {
            var modelBuilder = GetModelBuilder();

            var entityType = modelBuilder
                .Entity<Customer>()
                .Metadata;

            Assert.Equal("Customer", entityType.MySql().TableName);
            Assert.Equal("Customer", ((IEntityType)entityType).MySql().TableName);

            entityType.Relational().TableName = "Customizer";

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Customizer", entityType.Relational().TableName);
            Assert.Equal("Customizer", entityType.MySql().TableName);
            Assert.Equal("Customizer", ((IEntityType)entityType).MySql().TableName);

            entityType.MySql().TableName = "Custardizer";

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Custardizer", entityType.Relational().TableName);
            Assert.Equal("Custardizer", entityType.MySql().TableName);
            Assert.Equal("Custardizer", ((IEntityType)entityType).MySql().TableName);

            entityType.MySql().TableName = null;

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Customer", entityType.Relational().TableName);
            Assert.Equal("Customer", entityType.MySql().TableName);
            Assert.Equal("Customer", ((IEntityType)entityType).MySql().TableName);
        }

        [Fact]
        public void Can_get_and_set_schema_name()
        {
            var modelBuilder = GetModelBuilder();

            var entityType = modelBuilder
                .Entity<Customer>()
                .Metadata;

            Assert.Null(entityType.Relational().Schema);
            Assert.Null(entityType.MySql().Schema);
            Assert.Null(((IEntityType)entityType).MySql().Schema);

            entityType.Relational().Schema = "db0";

            Assert.Equal("db0", entityType.Relational().Schema);
            Assert.Equal("db0", entityType.MySql().Schema);
            Assert.Equal("db0", ((IEntityType)entityType).MySql().Schema);

            entityType.MySql().Schema = "dbOh";

            Assert.Equal("dbOh", entityType.Relational().Schema);
            Assert.Equal("dbOh", entityType.MySql().Schema);
            Assert.Equal("dbOh", ((IEntityType)entityType).MySql().Schema);

            entityType.MySql().Schema = null;

            Assert.Null(entityType.Relational().Schema);
            Assert.Null(entityType.MySql().Schema);
            Assert.Null(((IEntityType)entityType).MySql().Schema);
        }

        [Fact]
        public void Can_get_and_set_column_type()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Name)
                .Metadata;

            Assert.Null(property.Relational().ColumnType);
            Assert.Null(property.MySql().ColumnType);
            Assert.Null(((IProperty)property).MySql().ColumnType);

            property.Relational().ColumnType = "nvarchar(max)";

            Assert.Equal("nvarchar(max)", property.Relational().ColumnType);
            Assert.Equal("nvarchar(max)", property.MySql().ColumnType);
            Assert.Equal("nvarchar(max)", ((IProperty)property).MySql().ColumnType);

            property.MySql().ColumnType = "nvarchar(verstappen)";

            Assert.Equal("nvarchar(verstappen)", property.Relational().ColumnType);
            Assert.Equal("nvarchar(verstappen)", property.MySql().ColumnType);
            Assert.Equal("nvarchar(verstappen)", ((IProperty)property).MySql().ColumnType);

            property.MySql().ColumnType = null;

            Assert.Null(property.Relational().ColumnType);
            Assert.Null(property.MySql().ColumnType);
            Assert.Null(((IProperty)property).MySql().ColumnType);
        }

        [Fact]
        public void Can_get_and_set_column_default_expression()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Name)
                .Metadata;

            Assert.Null(property.Relational().DefaultValueSql);
            Assert.Null(property.MySql().DefaultValueSql);
            Assert.Null(((IProperty)property).MySql().DefaultValueSql);

            property.Relational().DefaultValueSql = "newsequentialid()";

            Assert.Equal("newsequentialid()", property.Relational().DefaultValueSql);
            Assert.Equal("newsequentialid()", property.MySql().DefaultValueSql);
            Assert.Equal("newsequentialid()", ((IProperty)property).MySql().DefaultValueSql);

            property.MySql().DefaultValueSql = "expressyourself()";

            Assert.Equal("expressyourself()", property.Relational().DefaultValueSql);
            Assert.Equal("expressyourself()", property.MySql().DefaultValueSql);
            Assert.Equal("expressyourself()", ((IProperty)property).MySql().DefaultValueSql);

            property.MySql().DefaultValueSql = null;

            Assert.Null(property.Relational().DefaultValueSql);
            Assert.Null(property.MySql().DefaultValueSql);
            Assert.Null(((IProperty)property).MySql().DefaultValueSql);
        }

        [Fact]
        public void Can_get_and_set_column_computed_expression()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Name)
                .Metadata;

            Assert.Null(property.Relational().ComputedColumnSql);
            Assert.Null(property.MySql().ComputedColumnSql);
            Assert.Null(((IProperty)property).MySql().ComputedColumnSql);

            property.Relational().ComputedColumnSql = "newsequentialid()";

            Assert.Equal("newsequentialid()", property.Relational().ComputedColumnSql);
            Assert.Equal("newsequentialid()", property.MySql().ComputedColumnSql);
            Assert.Equal("newsequentialid()", ((IProperty)property).MySql().ComputedColumnSql);

            property.MySql().ComputedColumnSql = "expressyourself()";

            Assert.Equal("expressyourself()", property.Relational().ComputedColumnSql);
            Assert.Equal("expressyourself()", property.MySql().ComputedColumnSql);
            Assert.Equal("expressyourself()", ((IProperty)property).MySql().ComputedColumnSql);

            property.MySql().ComputedColumnSql = null;

            Assert.Null(property.Relational().ComputedColumnSql);
            Assert.Null(property.MySql().ComputedColumnSql);
            Assert.Null(((IProperty)property).MySql().ComputedColumnSql);
        }

        [Fact]
        public void Can_get_and_set_column_default_value()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.ByteArray)
                .Metadata;

            Assert.Null(property.Relational().DefaultValue);
            Assert.Null(property.MySql().DefaultValue);
            Assert.Null(((IProperty)property).MySql().DefaultValue);

            property.Relational().DefaultValue = new byte[] { 69, 70, 32, 82, 79, 67, 75, 83 };

            Assert.Equal(new byte[] { 69, 70, 32, 82, 79, 67, 75, 83 }, property.Relational().DefaultValue);
            Assert.Equal(new byte[] { 69, 70, 32, 82, 79, 67, 75, 83 }, property.MySql().DefaultValue);
            Assert.Equal(new byte[] { 69, 70, 32, 82, 79, 67, 75, 83 }, ((IProperty)property).MySql().DefaultValue);

            property.MySql().DefaultValue = new byte[] { 69, 70, 32, 83, 79, 67, 75, 83 };

            Assert.Equal(new byte[] { 69, 70, 32, 83, 79, 67, 75, 83 }, property.Relational().DefaultValue);
            Assert.Equal(new byte[] { 69, 70, 32, 83, 79, 67, 75, 83 }, property.MySql().DefaultValue);
            Assert.Equal(new byte[] { 69, 70, 32, 83, 79, 67, 75, 83 }, ((IProperty)property).MySql().DefaultValue);

            property.MySql().DefaultValue = null;

            Assert.Null(property.Relational().DefaultValue);
            Assert.Null(property.MySql().DefaultValue);
            Assert.Null(((IProperty)property).MySql().DefaultValue);
        }

        [Theory]
        [InlineData(nameof(RelationalPropertyAnnotations.DefaultValue), nameof(RelationalPropertyAnnotations.DefaultValueSql))]
        [InlineData(nameof(RelationalPropertyAnnotations.DefaultValue), nameof(RelationalPropertyAnnotations.ComputedColumnSql))]
        [InlineData(nameof(RelationalPropertyAnnotations.DefaultValue), nameof(MySqlPropertyAnnotations.ValueGenerationStrategy))]
        [InlineData(nameof(RelationalPropertyAnnotations.DefaultValueSql), nameof(RelationalPropertyAnnotations.DefaultValue))]
        [InlineData(nameof(RelationalPropertyAnnotations.DefaultValueSql), nameof(RelationalPropertyAnnotations.ComputedColumnSql))]
        [InlineData(nameof(RelationalPropertyAnnotations.DefaultValueSql), nameof(MySqlPropertyAnnotations.ValueGenerationStrategy))]
        [InlineData(nameof(RelationalPropertyAnnotations.ComputedColumnSql), nameof(RelationalPropertyAnnotations.DefaultValue))]
        [InlineData(nameof(RelationalPropertyAnnotations.ComputedColumnSql), nameof(RelationalPropertyAnnotations.DefaultValueSql))]
        [InlineData(nameof(RelationalPropertyAnnotations.ComputedColumnSql), nameof(MySqlPropertyAnnotations.ValueGenerationStrategy))]
        [InlineData(nameof(MySqlPropertyAnnotations.ValueGenerationStrategy), nameof(RelationalPropertyAnnotations.DefaultValue))]
        [InlineData(nameof(MySqlPropertyAnnotations.ValueGenerationStrategy), nameof(RelationalPropertyAnnotations.DefaultValueSql))]
        [InlineData(nameof(MySqlPropertyAnnotations.ValueGenerationStrategy), nameof(RelationalPropertyAnnotations.ComputedColumnSql))]
        public void Metadata_throws_when_setting_conflicting_serverGenerated_values(string firstConfiguration, string secondConfiguration)
        {
            var modelBuilder = GetModelBuilder();

            var propertyBuilder = modelBuilder
                .Entity<Customer>()
                .Property(e => e.NullableInt);

            ConfigureProperty(propertyBuilder.Metadata, firstConfiguration, "1");

            Assert.Equal(
                RelationalStrings.ConflictingColumnServerGeneration(secondConfiguration, nameof(Customer.NullableInt), firstConfiguration),
                Assert.Throws<InvalidOperationException>(
                    () =>
                        ConfigureProperty(propertyBuilder.Metadata, secondConfiguration, "2")).Message);
        }

        protected virtual void ConfigureProperty(IMutableProperty property, string configuration, string value)
        {
            var propertyAnnotations = property.MySql();
            switch (configuration)
            {
                case nameof(RelationalPropertyAnnotations.DefaultValue):
                    propertyAnnotations.DefaultValue = int.Parse(value);
                    break;
                case nameof(RelationalPropertyAnnotations.DefaultValueSql):
                    propertyAnnotations.DefaultValueSql = value;
                    break;
                case nameof(RelationalPropertyAnnotations.ComputedColumnSql):
                    propertyAnnotations.ComputedColumnSql = value;
                    break;
                case nameof(MySqlPropertyAnnotations.ValueGenerationStrategy):
                    propertyAnnotations.ValueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [Fact]
        public void Can_get_and_set_column_key_name()
        {
            var modelBuilder = GetModelBuilder();

            var key = modelBuilder
                .Entity<Customer>()
                .HasKey(e => e.Id)
                .Metadata;

            Assert.Equal("PK_Customer", key.Relational().Name);
            Assert.Equal("PK_Customer", key.MySql().Name);
            Assert.Equal("PK_Customer", ((IKey)key).MySql().Name);

            key.Relational().Name = "PrimaryKey";

            Assert.Equal("PrimaryKey", key.Relational().Name);
            Assert.Equal("PrimaryKey", key.MySql().Name);
            Assert.Equal("PrimaryKey", ((IKey)key).MySql().Name);

            key.MySql().Name = "PrimarySchool";

            Assert.Equal("PrimarySchool", key.Relational().Name);
            Assert.Equal("PrimarySchool", key.MySql().Name);
            Assert.Equal("PrimarySchool", ((IKey)key).MySql().Name);

            key.MySql().Name = null;

            Assert.Equal("PK_Customer", key.Relational().Name);
            Assert.Equal("PK_Customer", key.MySql().Name);
            Assert.Equal("PK_Customer", ((IKey)key).MySql().Name);
        }

        [Fact]
        public void Can_get_and_set_column_foreign_key_name()
        {
            var modelBuilder = GetModelBuilder();

            modelBuilder
                .Entity<Customer>()
                .HasKey(e => e.Id);

            var foreignKey = modelBuilder
                .Entity<Order>()
                .HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Order>(e => e.CustomerId)
                .Metadata;

            Assert.Equal("FK_Order_Customer_CustomerId", foreignKey.Relational().Name);
            Assert.Equal("FK_Order_Customer_CustomerId", ((IForeignKey)foreignKey).Relational().Name);

            foreignKey.Relational().Name = "FK";

            Assert.Equal("FK", foreignKey.Relational().Name);
            Assert.Equal("FK", ((IForeignKey)foreignKey).Relational().Name);

            foreignKey.Relational().Name = "KFC";

            Assert.Equal("KFC", foreignKey.Relational().Name);
            Assert.Equal("KFC", ((IForeignKey)foreignKey).Relational().Name);

            foreignKey.Relational().Name = null;

            Assert.Equal("FK_Order_Customer_CustomerId", foreignKey.Relational().Name);
            Assert.Equal("FK_Order_Customer_CustomerId", ((IForeignKey)foreignKey).Relational().Name);
        }

        [Fact]
        public void Can_get_and_set_index_name()
        {
            var modelBuilder = GetModelBuilder();

            var index = modelBuilder
                .Entity<Customer>()
                .HasIndex(e => e.Id)
                .Metadata;

            Assert.Equal("IX_Customer_Id", index.Relational().Name);
            Assert.Equal("IX_Customer_Id", ((IIndex)index).Relational().Name);

            index.Relational().Name = "MyIndex";

            Assert.Equal("MyIndex", index.Relational().Name);
            Assert.Equal("MyIndex", ((IIndex)index).Relational().Name);

            index.MySql().Name = "DexKnows";

            Assert.Equal("DexKnows", index.Relational().Name);
            Assert.Equal("DexKnows", ((IIndex)index).Relational().Name);

            index.MySql().Name = null;

            Assert.Equal("IX_Customer_Id", index.Relational().Name);
            Assert.Equal("IX_Customer_Id", ((IIndex)index).Relational().Name);
        }

        [Fact]
        public void Can_get_and_set_index_filter()
        {
            var modelBuilder = new ModelBuilder(new ConventionSet());

            var index = modelBuilder
                .Entity<Customer>()
                .HasIndex(e => e.Id)
                .Metadata;

            Assert.Null(index.Relational().Filter);
            Assert.Null(index.MySql().Filter);
            Assert.Null(((IIndex)index).MySql().Filter);

            index.Relational().Name = "Generic expression";

            Assert.Equal("Generic expression", index.Relational().Name);
            Assert.Equal("Generic expression", index.MySql().Name);
            Assert.Equal("Generic expression", ((IIndex)index).MySql().Name);

            index.MySql().Name = "MySql-specific expression";

            Assert.Equal("MySql-specific expression", index.Relational().Name);
            Assert.Equal("MySql-specific expression", index.MySql().Name);
            Assert.Equal("MySql-specific expression", ((IIndex)index).MySql().Name);

            index.MySql().Name = null;

            Assert.Null(index.Relational().Filter);
            Assert.Null(index.MySql().Filter);
            Assert.Null(((IIndex)index).MySql().Filter);
        }

        [Fact]
        public void Can_get_and_set_index_clustering()
        {
            var modelBuilder = GetModelBuilder();

            var index = modelBuilder
                .Entity<Customer>()
                .HasIndex(e => e.Id)
                .Metadata;

            Assert.Null(index.MySql().IsClustered);
            Assert.Null(((IIndex)index).MySql().IsClustered);

            index.MySql().IsClustered = true;

            Assert.True(index.MySql().IsClustered.Value);
            Assert.True(((IIndex)index).MySql().IsClustered.Value);

            index.MySql().IsClustered = null;

            Assert.Null(index.MySql().IsClustered);
            Assert.Null(((IIndex)index).MySql().IsClustered);
        }

        [Fact]
        public void Can_get_and_set_key_clustering()
        {
            var modelBuilder = GetModelBuilder();

            var key = modelBuilder
                .Entity<Customer>()
                .HasKey(e => e.Id)
                .Metadata;

            Assert.Null(key.MySql().IsClustered);
            Assert.Null(((IKey)key).MySql().IsClustered);

            key.MySql().IsClustered = true;

            Assert.True(key.MySql().IsClustered.Value);
            Assert.True(((IKey)key).MySql().IsClustered.Value);

            key.MySql().IsClustered = null;

            Assert.Null(key.MySql().IsClustered);
            Assert.Null(((IKey)key).MySql().IsClustered);
        }

        [Fact]
        public void Can_get_and_set_sequence()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            Assert.Null(model.Relational().FindSequence("Foo"));
            Assert.Null(model.MySql().FindSequence("Foo"));
            Assert.Null(((IModel)model).MySql().FindSequence("Foo"));

            var sequence = model.MySql().GetOrAddSequence("Foo");

            Assert.Equal("Foo", model.Relational().FindSequence("Foo").Name);
            Assert.Equal("Foo", ((IModel)model).Relational().FindSequence("Foo").Name);
            Assert.Equal("Foo", model.MySql().FindSequence("Foo").Name);
            Assert.Equal("Foo", ((IModel)model).MySql().FindSequence("Foo").Name);

            Assert.Equal("Foo", sequence.Name);
            Assert.Null(sequence.Schema);
            Assert.Equal(1, sequence.IncrementBy);
            Assert.Equal(1, sequence.StartValue);
            Assert.Null(sequence.MinValue);
            Assert.Null(sequence.MaxValue);
            Assert.Same(typeof(long), sequence.ClrType);

            Assert.NotNull(model.Relational().FindSequence("Foo"));

            var sequence2 = model.MySql().FindSequence("Foo");

            sequence.StartValue = 1729;
            sequence.IncrementBy = 11;
            sequence.MinValue = 2001;
            sequence.MaxValue = 2010;
            sequence.ClrType = typeof(int);

            Assert.Equal("Foo", sequence.Name);
            Assert.Null(sequence.Schema);
            Assert.Equal(11, sequence.IncrementBy);
            Assert.Equal(1729, sequence.StartValue);
            Assert.Equal(2001, sequence.MinValue);
            Assert.Equal(2010, sequence.MaxValue);
            Assert.Same(typeof(int), sequence.ClrType);

            Assert.Equal(sequence2.Name, sequence.Name);
            Assert.Equal(sequence2.Schema, sequence.Schema);
            Assert.Equal(sequence2.IncrementBy, sequence.IncrementBy);
            Assert.Equal(sequence2.StartValue, sequence.StartValue);
            Assert.Equal(sequence2.MinValue, sequence.MinValue);
            Assert.Equal(sequence2.MaxValue, sequence.MaxValue);
            Assert.Same(sequence2.ClrType, sequence.ClrType);
        }

        [Fact]
        public void Can_get_and_set_sequence_with_schema_name()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            Assert.Null(model.Relational().FindSequence("Foo", "Smoo"));
            Assert.Null(model.MySql().FindSequence("Foo", "Smoo"));
            Assert.Null(((IModel)model).MySql().FindSequence("Foo", "Smoo"));

            var sequence = model.MySql().GetOrAddSequence("Foo", "Smoo");

            Assert.Equal("Foo", model.Relational().FindSequence("Foo", "Smoo").Name);
            Assert.Equal("Foo", ((IModel)model).Relational().FindSequence("Foo", "Smoo").Name);
            Assert.Equal("Foo", model.MySql().FindSequence("Foo", "Smoo").Name);
            Assert.Equal("Foo", ((IModel)model).MySql().FindSequence("Foo", "Smoo").Name);

            Assert.Equal("Foo", sequence.Name);
            Assert.Equal("Smoo", sequence.Schema);
            Assert.Equal(1, sequence.IncrementBy);
            Assert.Equal(1, sequence.StartValue);
            Assert.Null(sequence.MinValue);
            Assert.Null(sequence.MaxValue);
            Assert.Same(typeof(long), sequence.ClrType);

            Assert.NotNull(model.Relational().FindSequence("Foo", "Smoo"));

            var sequence2 = model.MySql().FindSequence("Foo", "Smoo");

            sequence.StartValue = 1729;
            sequence.IncrementBy = 11;
            sequence.MinValue = 2001;
            sequence.MaxValue = 2010;
            sequence.ClrType = typeof(int);

            Assert.Equal("Foo", sequence.Name);
            Assert.Equal("Smoo", sequence.Schema);
            Assert.Equal(11, sequence.IncrementBy);
            Assert.Equal(1729, sequence.StartValue);
            Assert.Equal(2001, sequence.MinValue);
            Assert.Equal(2010, sequence.MaxValue);
            Assert.Same(typeof(int), sequence.ClrType);

            Assert.Equal(sequence2.Name, sequence.Name);
            Assert.Equal(sequence2.Schema, sequence.Schema);
            Assert.Equal(sequence2.IncrementBy, sequence.IncrementBy);
            Assert.Equal(sequence2.StartValue, sequence.StartValue);
            Assert.Equal(sequence2.MinValue, sequence.MinValue);
            Assert.Equal(sequence2.MaxValue, sequence.MaxValue);
            Assert.Same(sequence2.ClrType, sequence.ClrType);
        }

        [Fact]
        public void Can_get_multiple_sequences()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            model.Relational().GetOrAddSequence("Fibonacci");
            model.MySql().GetOrAddSequence("Golomb");

            var sequences = model.MySql().Sequences;

            Assert.Equal(2, sequences.Count);
            Assert.Contains(sequences, s => s.Name == "Fibonacci");
            Assert.Contains(sequences, s => s.Name == "Golomb");
        }

        [Fact]
        public void Can_get_multiple_sequences_when_overridden()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            model.Relational().GetOrAddSequence("Fibonacci").StartValue = 1;
            model.MySql().GetOrAddSequence("Fibonacci").StartValue = 3;
            model.MySql().GetOrAddSequence("Golomb");

            var sequences = model.MySql().Sequences;

            Assert.Equal(2, sequences.Count);
            Assert.Contains(sequences, s => s.Name == "Golomb");

            var sequence = sequences.FirstOrDefault(s => s.Name == "Fibonacci");
            Assert.NotNull(sequence);
            Assert.Equal(3, sequence.StartValue);
        }

        [Fact]
        public void Can_get_and_set_value_generation_on_model()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            Assert.Equal(MySqlValueGenerationStrategy.IdentityColumn, model.MySql().ValueGenerationStrategy);

            model.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, model.MySql().ValueGenerationStrategy);

            model.MySql().ValueGenerationStrategy = null;

            Assert.Null(model.MySql().ValueGenerationStrategy);
        }

        [Fact]
        public void Can_get_and_set_default_sequence_name_on_model()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            Assert.Null(model.MySql().HiLoSequenceName);
            Assert.Null(((IModel)model).MySql().HiLoSequenceName);

            model.MySql().HiLoSequenceName = "Tasty.Snook";

            Assert.Equal("Tasty.Snook", model.MySql().HiLoSequenceName);
            Assert.Equal("Tasty.Snook", ((IModel)model).MySql().HiLoSequenceName);

            model.MySql().HiLoSequenceName = null;

            Assert.Null(model.MySql().HiLoSequenceName);
            Assert.Null(((IModel)model).MySql().HiLoSequenceName);
        }

        [Fact]
        public void Can_get_and_set_default_sequence_schema_on_model()
        {
            var modelBuilder = GetModelBuilder();
            var model = modelBuilder.Model;

            Assert.Null(model.MySql().HiLoSequenceSchema);
            Assert.Null(((IModel)model).MySql().HiLoSequenceSchema);

            model.MySql().HiLoSequenceSchema = "Tasty.Snook";

            Assert.Equal("Tasty.Snook", model.MySql().HiLoSequenceSchema);
            Assert.Equal("Tasty.Snook", ((IModel)model).MySql().HiLoSequenceSchema);

            model.MySql().HiLoSequenceSchema = null;

            Assert.Null(model.MySql().HiLoSequenceSchema);
            Assert.Null(((IModel)model).MySql().HiLoSequenceSchema);
        }

        [Fact]
        public void Can_get_and_set_value_generation_on_property()
        {
            var modelBuilder = GetModelBuilder();
            modelBuilder.Model.MySql().ValueGenerationStrategy = null;

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .Metadata;

            Assert.Null(property.MySql().ValueGenerationStrategy);
            Assert.Equal(ValueGenerated.OnAdd, property.ValueGenerated);

            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, property.MySql().ValueGenerationStrategy);
            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, ((IProperty)property).MySql().ValueGenerationStrategy);
            Assert.Equal(ValueGenerated.OnAdd, property.ValueGenerated);

            property.MySql().ValueGenerationStrategy = null;

            Assert.Null(property.MySql().ValueGenerationStrategy);
            Assert.Equal(ValueGenerated.OnAdd, property.ValueGenerated);
        }

        [Fact]
        public void Can_get_and_set_value_generation_on_nullable_property()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.NullableInt)
                .Metadata;

            Assert.Null(property.MySql().ValueGenerationStrategy);

            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, property.MySql().ValueGenerationStrategy);
            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, ((IProperty)property).MySql().ValueGenerationStrategy);

            property.MySql().ValueGenerationStrategy = null;

            Assert.Null(property.MySql().ValueGenerationStrategy);
        }

        [Fact]
        public void Throws_setting_sequence_generation_for_invalid_type()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Name)
                .Metadata;

            Assert.Equal(
                MySqlStrings.SequenceBadType("Name", nameof(Customer), "string"),
                Assert.Throws<ArgumentException>(
                    () => property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo).Message);
        }

        [Fact]
        public void Throws_setting_identity_generation_for_invalid_type()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Name)
                .Metadata;

            Assert.Equal(
                MySqlStrings.IdentityBadType("Name", nameof(Customer), "string"),
                Assert.Throws<ArgumentException>(
                    () => property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn).Message);
        }

        [Fact]
        public void Can_get_and_set_sequence_name_on_property()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .Metadata;

            Assert.Null(property.MySql().HiLoSequenceName);
            Assert.Null(((IProperty)property).MySql().HiLoSequenceName);

            property.MySql().HiLoSequenceName = "Snook";

            Assert.Equal("Snook", property.MySql().HiLoSequenceName);
            Assert.Equal("Snook", ((IProperty)property).MySql().HiLoSequenceName);

            property.MySql().HiLoSequenceName = null;

            Assert.Null(property.MySql().HiLoSequenceName);
            Assert.Null(((IProperty)property).MySql().HiLoSequenceName);
        }

        [Fact]
        public void Can_get_and_set_sequence_schema_on_property()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .Metadata;

            Assert.Null(property.MySql().HiLoSequenceSchema);
            Assert.Null(((IProperty)property).MySql().HiLoSequenceSchema);

            property.MySql().HiLoSequenceSchema = "Tasty";

            Assert.Equal("Tasty", property.MySql().HiLoSequenceSchema);
            Assert.Equal("Tasty", ((IProperty)property).MySql().HiLoSequenceSchema);

            property.MySql().HiLoSequenceSchema = null;

            Assert.Null(property.MySql().HiLoSequenceSchema);
            Assert.Null(((IProperty)property).MySql().HiLoSequenceSchema);
        }

        [Fact]
        public void TryGetSequence_returns_null_if_property_is_not_configured_for_sequence_value_generation()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw");

            Assert.Null(property.MySql().FindHiLoSequence());
            Assert.Null(((IProperty)property).MySql().FindHiLoSequence());

            property.MySql().HiLoSequenceName = "DaneelOlivaw";

            Assert.Null(property.MySql().FindHiLoSequence());
            Assert.Null(((IProperty)property).MySql().FindHiLoSequence());

            modelBuilder.Model.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;

            Assert.Null(property.MySql().FindHiLoSequence());
            Assert.Null(((IProperty)property).MySql().FindHiLoSequence());

            modelBuilder.Model.MySql().ValueGenerationStrategy = null;
            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;

            Assert.Null(property.MySql().FindHiLoSequence());
            Assert.Null(((IProperty)property).MySql().FindHiLoSequence());
        }

        [Fact]
        public void TryGetSequence_returns_sequence_property_is_marked_for_sequence_generation()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw");
            property.MySql().HiLoSequenceName = "DaneelOlivaw";
            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
        }

        [Fact]
        public void TryGetSequence_returns_sequence_property_is_marked_for_default_generation_and_model_is_marked_for_sequence_generation()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw");
            modelBuilder.Model.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;
            property.MySql().HiLoSequenceName = "DaneelOlivaw";

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
        }

        [Fact]
        public void TryGetSequence_returns_sequence_property_is_marked_for_sequence_generation_and_model_has_name()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw");
            modelBuilder.Model.MySql().HiLoSequenceName = "DaneelOlivaw";
            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
        }

        [Fact]
        public void TryGetSequence_returns_sequence_property_is_marked_for_default_generation_and_model_is_marked_for_sequence_generation_and_model_has_name()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw");
            modelBuilder.Model.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;
            modelBuilder.Model.MySql().HiLoSequenceName = "DaneelOlivaw";

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
        }

        [Fact]
        public void TryGetSequence_with_schema_returns_sequence_property_is_marked_for_sequence_generation()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw", "R");
            property.MySql().HiLoSequenceName = "DaneelOlivaw";
            property.MySql().HiLoSequenceSchema = "R";
            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
            Assert.Equal("R", property.MySql().FindHiLoSequence().Schema);
            Assert.Equal("R", ((IProperty)property).MySql().FindHiLoSequence().Schema);
        }

        [Fact]
        public void TryGetSequence_with_schema_returns_sequence_model_is_marked_for_sequence_generation()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw", "R");
            modelBuilder.Model.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;
            property.MySql().HiLoSequenceName = "DaneelOlivaw";
            property.MySql().HiLoSequenceSchema = "R";

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
            Assert.Equal("R", property.MySql().FindHiLoSequence().Schema);
            Assert.Equal("R", ((IProperty)property).MySql().FindHiLoSequence().Schema);
        }

        [Fact]
        public void TryGetSequence_with_schema_returns_sequence_property_is_marked_for_sequence_generation_and_model_has_name()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw", "R");
            modelBuilder.Model.MySql().HiLoSequenceName = "DaneelOlivaw";
            modelBuilder.Model.MySql().HiLoSequenceSchema = "R";
            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
            Assert.Equal("R", property.MySql().FindHiLoSequence().Schema);
            Assert.Equal("R", ((IProperty)property).MySql().FindHiLoSequence().Schema);
        }

        [Fact]
        public void TryGetSequence_with_schema_returns_sequence_model_is_marked_for_sequence_generation_and_model_has_name()
        {
            var modelBuilder = GetModelBuilder();

            var property = modelBuilder
                .Entity<Customer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata;

            modelBuilder.Model.MySql().GetOrAddSequence("DaneelOlivaw", "R");
            modelBuilder.Model.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.SequenceHiLo;
            modelBuilder.Model.MySql().HiLoSequenceName = "DaneelOlivaw";
            modelBuilder.Model.MySql().HiLoSequenceSchema = "R";

            Assert.Equal("DaneelOlivaw", property.MySql().FindHiLoSequence().Name);
            Assert.Equal("DaneelOlivaw", ((IProperty)property).MySql().FindHiLoSequence().Name);
            Assert.Equal("R", property.MySql().FindHiLoSequence().Schema);
            Assert.Equal("R", ((IProperty)property).MySql().FindHiLoSequence().Schema);
        }

        private static ModelBuilder GetModelBuilder() => MySqlTestHelpers.Instance.CreateConventionBuilder();

        private class Customer
        {
            public int Id { get; set; }
            public int? NullableInt { get; set; }
            public string Name { get; set; }
            public byte Byte { get; set; }
            public byte? NullableByte { get; set; }
            public byte[] ByteArray { get; set; }
        }

        private class Order
        {
            public int OrderId { get; set; }
            public int CustomerId { get; set; }
        }
    }
}
