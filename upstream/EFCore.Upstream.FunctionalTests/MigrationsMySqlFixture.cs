// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class MigrationsMySqlFixture : MigrationsFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public MigrationsMySqlFixture()
        {
            ((MySqlTestStore)TestStore).ExecuteNonQuery(@"USE master
IF EXISTS(select * from sys.databases where name='TransactionSuppressed')
DROP DATABASE TransactionSuppressed");
        }

        public override MigrationsContext CreateContext()
        {
            var options = AddOptions(
                    new DbContextOptionsBuilder()
                        .UseMySql(TestStore.ConnectionString, b => b.ApplyConfiguration().CommandTimeout(MySqlTestStore.CommandTimeout)))
                .UseInternalServiceProvider(ServiceProvider)
                .Options;
            return new MigrationsContext(options);
        }
    }
}
