// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Update
{
    public class MySqlModificationCommandBatchTest
    {
        [Fact]
        public void AddCommand_returns_false_when_max_batch_size_is_reached()
        {
            var typeMapper = new MySqlTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var batch = new MySqlModificationCommandBatch(
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
                        typeMapper)),
                1);

            Assert.True(
                batch.AddCommand(
                    new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
            Assert.False(
                batch.AddCommand(
                    new ModificationCommand("T1", null, new ParameterNameGenerator().GenerateNext, false, null)));
        }
    }
}
