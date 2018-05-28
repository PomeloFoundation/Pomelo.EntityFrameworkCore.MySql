// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class MySqlTestStoreFactory : ITestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();

        protected MySqlTestStoreFactory()
        {
        }

        public virtual TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName);

        public virtual TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName);

        public virtual IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql()
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory());
    }
}
