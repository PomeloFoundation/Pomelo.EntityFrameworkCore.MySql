// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public static class MySqlDbContextOptionsBuilderExtensions
    {
        public static MySqlDbContextOptionsBuilder ApplyConfiguration(this MySqlDbContextOptionsBuilder optionsBuilder)
        {
            var maxBatch = TestEnvironment.GetInt(nameof(MySqlDbContextOptionsBuilder.MaxBatchSize));
            if (maxBatch.HasValue)
            {
                optionsBuilder.MaxBatchSize(maxBatch.Value);
            }

            optionsBuilder.ExecutionStrategy(d => new TestMySqlRetryingExecutionStrategy(d));

            optionsBuilder.CommandTimeout(MySqlTestStore.DefaultCommandTimeout);

            return optionsBuilder;
        }
    }
}
