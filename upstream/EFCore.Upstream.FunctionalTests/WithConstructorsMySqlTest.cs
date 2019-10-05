// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class WithConstructorsMySqlTest : WithConstructorsTestBase<WithConstructorsMySqlTest.WithConstructorsMySqlFixture>
    {
        public WithConstructorsMySqlTest(WithConstructorsMySqlFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact(Skip = "Issue #16323")]
        public override void Query_with_keyless_type()
            => base.Query_with_keyless_type();

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class WithConstructorsMySqlFixture : WithConstructorsFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<BlogQuery>().HasNoKey().ToQuery(
                    () => context.Set<BlogQuery>().FromSqlRaw("SELECT * FROM Blog"));
            }
        }
    }
}
