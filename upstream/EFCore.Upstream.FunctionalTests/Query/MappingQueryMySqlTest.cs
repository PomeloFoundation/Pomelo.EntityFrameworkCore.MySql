// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class MappingQueryMySqlTest : MappingQueryTestBase<MappingQueryMySqlTest.MappingQueryMySqlFixture>
    {
        public override void All_customers()
        {
            base.All_customers();

            Assert.Equal(
                @"SELECT [c].[CustomerID], [c].[CompanyName]
FROM [dbo].[Customers] AS [c]",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void All_employees()
        {
            base.All_employees();

            Assert.Equal(
                @"SELECT [e].[EmployeeID], [e].[City]
FROM [dbo].[Employees] AS [e]",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void All_orders()
        {
            base.All_orders();

            Assert.Equal(
                @"SELECT [o].[OrderID], [o].[ShipVia]
FROM [dbo].[Orders] AS [o]",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Project_nullable_enum()
        {
            base.Project_nullable_enum();

            Assert.Equal(
                @"SELECT [o].[ShipVia]
FROM [dbo].[Orders] AS [o]",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public MappingQueryMySqlTest(MappingQueryMySqlFixture fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        private string Sql => Fixture.TestSqlLoggerFactory.Sql;

        public class MappingQueryMySqlFixture : MappingQueryFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlNorthwindTestStoreFactory.Instance;

            protected override string DatabaseSchema { get; } = "dbo";

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<MappedCustomer>(
                    e =>
                        {
                            e.Property(c => c.CompanyName2).Metadata.MySql().ColumnName = "CompanyName";
                            e.Metadata.MySql().TableName = "Customers";
                            e.Metadata.MySql().Schema = "dbo";
                        });

                modelBuilder.Entity<MappedEmployee>()
                    .Property(c => c.EmployeeID)
                    .HasColumnType("int");
            }
        }
    }
}
