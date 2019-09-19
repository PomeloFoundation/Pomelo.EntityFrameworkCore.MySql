// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore
{
    public class DbSetAsTableNameMySqlTest : DbSetAsTableNameTest
    {
        protected override string GetTableName<TEntity>(DbContext context)
            => context.Model.FindEntityType(typeof(TEntity)).GetTableName();

        protected override SetsContext CreateContext() => new MySqlSetsContext();

        protected override SetsContext CreateNamedTablesContext() => new MySqlNamedTablesContextContext();

        protected class MySqlSetsContext : SetsContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database = Dummy");
        }

        protected class MySqlNamedTablesContextContext : NamedTablesContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseInternalServiceProvider(MySqlFixture.DefaultServiceProvider)
                    .UseMySql("Database = Dummy");
        }
    }
}
