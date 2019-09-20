// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class MySqlAdventureWorksTestStoreFactory : MySqlTestStoreFactory
    {
        public static new MySqlAdventureWorksTestStoreFactory Instance { get; } = new MySqlAdventureWorksTestStoreFactory();

        protected MySqlAdventureWorksTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(
                "adventureworks",
                Path.Combine("SqlAzure", "adventureworks.sql"));
    }
}
