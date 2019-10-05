// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class ProxyGraphUpdatesMySqlTest
    {
        public abstract class ProxyGraphUpdatesMySqlTestBase<TFixture> : ProxyGraphUpdatesTestBase<TFixture>
            where TFixture : ProxyGraphUpdatesMySqlTestBase<TFixture>.ProxyGraphUpdatesMySqlFixtureBase, new()
        {
            protected ProxyGraphUpdatesMySqlTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class ProxyGraphUpdatesMySqlFixtureBase : ProxyGraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            }
        }

        public class LazyLoading : ProxyGraphUpdatesMySqlTestBase<LazyLoading.ProxyGraphUpdatesWithLazyLoadingMySqlFixture>
        {
            public LazyLoading(ProxyGraphUpdatesWithLazyLoadingMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class ProxyGraphUpdatesWithLazyLoadingMySqlFixture : ProxyGraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "ProxyGraphLazyLoadingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseLazyLoadingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UseIdentityColumns();

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }
    }
}
