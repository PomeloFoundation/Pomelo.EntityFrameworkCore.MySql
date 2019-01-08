// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class MySqlNorthwindTestStoreFactory : MySqlTestStoreFactory
    {
        public const string Name = "Northwind";
        public static readonly string NorthwindConnectionString = MySqlTestStore.CreateConnectionString(Name);
        public static new MySqlNorthwindTestStoreFactory Instance { get; } = new MySqlNorthwindTestStoreFactory();

        protected MySqlNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => MySqlTestStore.GetOrCreate(Name, "Northwind.sql");
    }
}
