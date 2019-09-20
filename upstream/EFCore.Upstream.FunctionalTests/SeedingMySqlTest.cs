// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class SeedingMySqlTest : SeedingTestBase
    {
        protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
        {
            var context = new SeedingMySqlContext(testId);

            context.Database.EnsureClean();

            return context;
        }

        protected class SeedingMySqlContext : SeedingContext
        {
            public SeedingMySqlContext(string testId)
                : base(testId)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseMySql(MySqlTestStore.CreateConnectionString($"Seeds{TestId}"));
        }
    }
}
