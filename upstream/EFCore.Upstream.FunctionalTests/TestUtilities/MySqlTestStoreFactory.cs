// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class MySqlTestStoreFactory : RelationalTestStoreFactory
    {
        public static MySqlTestStoreFactory Instance { get; } = new MySqlTestStoreFactory();

        protected MySqlTestStoreFactory()
        {
        }

        public override TestStore Create(string storeName)
            => MySqlTestStore.Create(storeName);

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(storeName);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql();
    }
}
