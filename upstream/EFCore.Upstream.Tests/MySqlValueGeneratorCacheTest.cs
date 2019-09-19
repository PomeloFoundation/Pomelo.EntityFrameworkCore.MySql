// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore
{
    public class MySqlValueGeneratorCacheTest
    {
        [ConditionalFact]
        public void Uses_single_generator_per_property()
        {
            var model = CreateModel();
            var entityType = model.FindEntityType(typeof(Led));
            var property1 = GetProperty1(model);
            var property2 = GetProperty2(model);
            var cache = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IMySqlValueGeneratorCache>();

            var generator1 = cache.GetOrAdd(property1, entityType, (p, et) => new TemporaryIntValueGenerator());
            Assert.NotNull(generator1);
            Assert.Same(generator1, cache.GetOrAdd(property1, entityType, (p, et) => new TemporaryIntValueGenerator()));

            var generator2 = cache.GetOrAdd(property2, entityType, (p, et) => new TemporaryIntValueGenerator());
            Assert.NotNull(generator2);
            Assert.Same(generator2, cache.GetOrAdd(property2, entityType, (p, et) => new TemporaryIntValueGenerator()));
            Assert.NotSame(generator1, generator2);
        }

        [ConditionalFact]
        public void Uses_single_sequence_generator_per_sequence()
        {
            var model = CreateModel();
            var property1 = GetProperty1(model);
            var property2 = GetProperty2(model);
            var property3 = GetProperty3(model);
            var cache = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IMySqlValueGeneratorCache>();
            var connection = CreateConnection();

            var generator1 = cache.GetOrAddSequenceState(property1, connection);
            Assert.NotNull(generator1);
            Assert.Same(generator1, cache.GetOrAddSequenceState(property1, connection));

            var generator2 = cache.GetOrAddSequenceState(property2, connection);
            Assert.NotNull(generator2);
            Assert.Same(generator2, cache.GetOrAddSequenceState(property2, connection));
            Assert.Same(generator1, generator2);

            var generator3 = cache.GetOrAddSequenceState(property3, connection);
            Assert.NotNull(generator3);
            Assert.Same(generator3, cache.GetOrAddSequenceState(property3, connection));
            Assert.NotSame(generator1, generator3);
        }

        [ConditionalFact]
        public void Uses_single_sequence_generator_per_database()
        {
            var model = CreateModel();
            var property1 = GetProperty1(model);
            var cache = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IMySqlValueGeneratorCache>();
            var connection1 = CreateConnection("DbOne");
            var connection2 = CreateConnection("DbTwo");

            var generator1 = cache.GetOrAddSequenceState(property1, connection1);
            Assert.NotNull(generator1);
            Assert.Same(generator1, cache.GetOrAddSequenceState(property1, connection1));

            var generator2 = cache.GetOrAddSequenceState(property1, connection2);
            Assert.NotNull(generator2);
            Assert.Same(generator2, cache.GetOrAddSequenceState(property1, connection2));
            Assert.NotSame(generator1, generator2);
        }

        [ConditionalFact]
        public void Uses_single_sequence_generator_per_server()
        {
            var model = CreateModel();
            var property1 = GetProperty1(model);
            var cache = MySqlTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IMySqlValueGeneratorCache>();
            var connection1 = CreateConnection(serverName: "ServerOne");
            var connection2 = CreateConnection(serverName: "ServerTwo");

            var generator1 = cache.GetOrAddSequenceState(property1, connection1);
            Assert.NotNull(generator1);
            Assert.Same(generator1, cache.GetOrAddSequenceState(property1, connection1));

            var generator2 = cache.GetOrAddSequenceState(property1, connection2);
            Assert.NotNull(generator2);
            Assert.Same(generator2, cache.GetOrAddSequenceState(property1, connection2));
            Assert.NotSame(generator1, generator2);
        }

        private static FakeRelationalConnection CreateConnection(
            string databaseName = null,
            string serverName = null)
        {
            var connection = new FakeRelationalConnection();
            connection.UseConnection(
                new SqlConnection(
                    $"Database={databaseName ?? "DbOne"};Data Source={serverName ?? "ServerOne"}"));

            return connection;
        }

        [ConditionalFact]
        public void Block_size_is_obtained_from_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo()
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal(10, cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy);
        }

        [ConditionalFact]
        public void Block_size_is_obtained_from_named_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw")
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal(10, cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy);
        }

        [ConditionalFact]
        public void Block_size_is_obtained_from_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo()
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal(10, cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy);
        }

        [ConditionalFact]
        public void Block_size_is_obtained_from_named_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo("DaneelOlivaw")
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal(10, cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy);
        }

        [ConditionalFact]
        public void Block_size_is_obtained_from_specified_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .HasSequence("DaneelOlivaw", b => b.IncrementsBy(11))
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw")
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal(11, cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy);
        }

        [ConditionalFact]
        public void Non_positive_block_sizes_are_not_allowed()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .HasSequence("DaneelOlivaw", b => b.IncrementsBy(-1))
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw")
                .Metadata;

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            modelBuilder.FinalizeModel();

            Assert.StartsWith(
                CoreStrings.HiLoBadBlockSize,
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy).Message);
        }

        [ConditionalFact]
        public void Block_size_is_obtained_from_specified_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo("DaneelOlivaw")
                .HasSequence("DaneelOlivaw", b => b.IncrementsBy(11))
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal(11, cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.IncrementBy);
        }

        [ConditionalFact]
        public void Sequence_name_is_obtained_from_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo()
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("EntityFrameworkHiLoSequence", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
        }

        [ConditionalFact]
        public void Sequence_name_is_obtained_from_named_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw")
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
        }

        [ConditionalFact]
        public void Sequence_name_is_obtained_from_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo()
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("EntityFrameworkHiLoSequence", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
        }

        [ConditionalFact]
        public void Sequence_name_is_obtained_from_named_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo("DaneelOlivaw")
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
        }

        [ConditionalFact]
        public void Sequence_name_is_obtained_from_specified_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .HasSequence("DaneelOlivaw", b => b.IncrementsBy(11))
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw")
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
        }

        [ConditionalFact]
        public void Sequence_name_is_obtained_from_specified_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo("DaneelOlivaw")
                .HasSequence("DaneelOlivaw", b => b.IncrementsBy(11))
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
        }

        [ConditionalFact]
        public void Schema_qualified_sequence_name_is_obtained_from_named_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw", "R")
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
            Assert.Equal("R", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Schema);
        }

        [ConditionalFact]
        public void Schema_qualified_sequence_name_is_obtained_from_named_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo("DaneelOlivaw", "R")
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
            Assert.Equal("R", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Schema);
        }

        [ConditionalFact]
        public void Schema_qualified_sequence_name_is_obtained_from_specified_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .HasSequence("DaneelOlivaw", "R", b => b.IncrementsBy(11))
                .Entity<Robot>()
                .Property(e => e.Id)
                .UseHiLo("DaneelOlivaw", "R")
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
            Assert.Equal("R", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Schema);
        }

        [ConditionalFact]
        public void Schema_qualified_sequence_name_is_obtained_from_specified_model_default_sequence()
        {
            var modelBuilder = CreateConventionModelBuilder();

            var property = modelBuilder
                .UseHiLo("DaneelOlivaw", "R")
                .HasSequence("DaneelOlivaw", "R", b => b.IncrementsBy(11))
                .Entity<Robot>()
                .Property(e => e.Id)
                .Metadata;

            modelBuilder.FinalizeModel();

            var cache = new MySqlValueGeneratorCache(new ValueGeneratorCacheDependencies());

            Assert.Equal("DaneelOlivaw", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Name);
            Assert.Equal("R", cache.GetOrAddSequenceState(property, CreateConnection()).Sequence.Schema);
        }

        protected virtual ModelBuilder CreateConventionModelBuilder() => MySqlTestHelpers.Instance.CreateConventionBuilder();

        private class Robot
        {
            public int Id { get; set; }
        }

        private static IProperty GetProperty1(IModel model) => model.FindEntityType(typeof(Led)).FindProperty("Zeppelin");

        private static IProperty GetProperty2(IModel model) => model.FindEntityType(typeof(Led)).FindProperty("Stairway");

        private static IProperty GetProperty3(IModel model) => model.FindEntityType(typeof(Led)).FindProperty("WholeLotta");

        private static IModel CreateModel()
        {
            var modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder();

            modelBuilder.HasSequence("Heaven");
            modelBuilder.HasSequence("Rosie");

            modelBuilder.Entity<Led>(
                b =>
                {
                    b.Property(e => e.Zeppelin).UseHiLo("Heaven");
                    b.Property(e => e.Stairway).UseHiLo("Heaven");
                    b.Property(e => e.WholeLotta).UseHiLo("Rosie");
                });

            return modelBuilder.Model;
        }

        private class Led
        {
            public int Zeppelin { get; set; }
            public int Stairway { get; set; }
            public int WholeLotta { get; set; }
        }
    }
}
