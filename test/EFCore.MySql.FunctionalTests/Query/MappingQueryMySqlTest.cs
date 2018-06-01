using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class MappingQueryMySqlTest : MappingQueryTestBase<MappingQueryMySqlTest.MappingQueryMySqlFixture>
    {
        public MappingQueryMySqlTest(MappingQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void All_customers()
        {
            base.All_customers();

            Assert.Contains(
                @"SELECT `c`.`CustomerID`, `c`.`CompanyName`" + _eol +
                @"FROM `Customers` AS `c`",
                Sql);
        }

        public override void All_employees()
        {
            base.All_employees();

            Assert.Contains(
                @"SELECT `e`.`EmployeeID`, `e`.`City`" + _eol +
                @"FROM `Employees` AS `e`",
                Sql);
        }

        [Fact(Skip = "issue #573")]
        public override void All_orders()
        {
            base.All_orders();

            Assert.Contains(
                @"SELECT `o`.`OrderID`, `o`.`ShipVia`" + _eol +
                @"FROM `Orders` AS `o`",
                Sql);
        }

        [Fact(Skip = "issue #573")]
        public override void Project_nullable_enum()
        {
            base.Project_nullable_enum();

            Assert.Contains(
                @"SELECT `o`.`ShipVia`" + _eol +
                @"FROM `Orders` AS `o`",
                Sql);
        }

        private static readonly string _eol = Environment.NewLine;

        private string Sql => Fixture.TestSqlLoggerFactory.Sql;

        public class MappingQueryMySqlFixture : MappingQueryFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override string DatabaseSchema { get; } = null;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<MappedCustomer>(
                    e =>
                        {
                            e.Property(c => c.CompanyName2).Metadata.Relational().ColumnName = "CompanyName";
                            e.Metadata.Relational().TableName = "Customers";
                        });
            }
        }
    }
}
