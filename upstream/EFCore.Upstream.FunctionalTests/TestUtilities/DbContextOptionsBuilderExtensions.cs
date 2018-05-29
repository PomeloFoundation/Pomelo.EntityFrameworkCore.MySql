// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static MySqlDbContextOptionsBuilder ApplyConfiguration(this MySqlDbContextOptionsBuilder optionsBuilder)
        {
            var maxBatch = TestEnvironment.GetInt(nameof(MySqlDbContextOptionsBuilder.MaxBatchSize));
            if (maxBatch.HasValue)
            {
                optionsBuilder.MaxBatchSize(maxBatch.Value);
            }

            var offsetSupport = TestEnvironment.GetFlag(nameof(MySqlCondition.SupportsOffset)) ?? true;
            if (!offsetSupport)
            {
                optionsBuilder.UseRowNumberForPaging();
            }

            if (TestEnvironment.IsSqlAzure)
            {
                optionsBuilder.ExecutionStrategy(c => new TestMySqlRetryingExecutionStrategy(c));
            }

            return optionsBuilder;
        }
    }
}
