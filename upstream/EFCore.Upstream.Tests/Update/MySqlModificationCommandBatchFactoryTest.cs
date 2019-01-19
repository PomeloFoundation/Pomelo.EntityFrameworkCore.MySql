// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Update
{
    public class MySqlModificationCommandBatchFactoryTest
    {
        [Fact]
        public void Uses_MaxBatchSize_specified_in_MySqlOptionsExtension()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=Crunchie", b => b.MaxBatchSize(1));

            var typeMapper = new MySqlTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var factory = new MySqlModificationCommandBatchFactory(
                new RelationalCommandBuilderFactory(
                    new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>(),
                    typeMapper),
                new MySqlSqlGenerationHelper(
                    new RelationalSqlGenerationHelperDependencies()),
                new MySqlUpdateSqlGenerator(
                    new UpdateSqlGeneratorDependencies(
                        new MySqlSqlGenerationHelper(
                            new RelationalSqlGenerationHelperDependencies()),
                        typeMapper)),
                new TypedRelationalValueBufferFactoryFactory(
                    new RelationalValueBufferFactoryDependencies(
                        typeMapper, new CoreSingletonOptions())),
                optionsBuilder.Options);

            var batch = factory.Create();

            Assert.True(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
            Assert.False(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
        }

        [Fact]
        public void MaxBatchSize_is_optional()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=Crunchie");

            var typeMapper = new MySqlTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var factory = new MySqlModificationCommandBatchFactory(
                new RelationalCommandBuilderFactory(
                    new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>(),
                    typeMapper),
                new MySqlSqlGenerationHelper(
                    new RelationalSqlGenerationHelperDependencies()),
                new MySqlUpdateSqlGenerator(
                    new UpdateSqlGeneratorDependencies(
                        new MySqlSqlGenerationHelper(
                            new RelationalSqlGenerationHelperDependencies()),
                        typeMapper)),
                new TypedRelationalValueBufferFactoryFactory(
                    new RelationalValueBufferFactoryDependencies(
                        typeMapper, new CoreSingletonOptions())),
                optionsBuilder.Options);

            var batch = factory.Create();

            Assert.True(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
            Assert.True(batch.AddCommand(new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
        }
    }
}
