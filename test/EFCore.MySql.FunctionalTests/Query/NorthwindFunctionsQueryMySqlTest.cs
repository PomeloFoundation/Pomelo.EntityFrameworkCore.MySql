using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindFunctionsQueryMySqlTest : NorthwindFunctionsQueryRelationalTestBase<
        CaseSensitiveNorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindFunctionsQueryMySqlTest(
            CaseSensitiveNorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Where_functions_nested(bool async)
        {
            await base.Where_functions_nested(async);

            AssertSql(
                $@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE POWER({MySqlTestHelpers.CastAsDouble("CHAR_LENGTH(`c`.`CustomerID`)")}, 2.0) = 25.0");
        }

        public override async Task Order_by_length_twice(bool async)
        {
            await base.Order_by_length_twice(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY CHAR_LENGTH(`c`.`CustomerID`), `c`.`CustomerID`");
        }

        public override async Task Order_by_length_twice_followed_by_projection_of_naked_collection_navigation(bool async)
        {
            await base.Order_by_length_twice_followed_by_projection_of_naked_collection_navigation(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
ORDER BY CHAR_LENGTH(`c`.`CustomerID`), `c`.`CustomerID`");
        }

        public override async Task Static_equals_nullable_datetime_compared_to_non_nullable(bool async)
        {
            await base.Static_equals_nullable_datetime_compared_to_non_nullable(async);

            AssertSql(
"""
@arg='1996-07-04T00:00:00.0000000' (DbType = DateTime)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` = @arg
""");
        }

        public override async Task Static_equals_int_compared_to_long(bool async)
        {
            await base.Static_equals_int_compared_to_long(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE FALSE");
        }

        public override async Task Sum_over_round_works_correctly_in_projection(bool async)
        {
            await base.Sum_over_round_works_correctly_in_projection(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, (
    SELECT COALESCE(SUM(ROUND(`o0`.`UnitPrice`, 2)), 0.0)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) AS `Sum`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10300
""");
        }

        public override async Task Sum_over_round_works_correctly_in_projection_2(bool async)
        {
            await base.Sum_over_round_works_correctly_in_projection_2(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, (
    SELECT COALESCE(SUM(ROUND(`o0`.`UnitPrice` * `o0`.`UnitPrice`, 2)), 0.0)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) AS `Sum`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10300
""");
        }

        public override async Task Sum_over_truncate_works_correctly_in_projection(bool async)
        {
            await base.Sum_over_truncate_works_correctly_in_projection(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, (
    SELECT COALESCE(SUM(TRUNCATE(`o0`.`UnitPrice`, 0)), 0.0)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) AS `Sum`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10300
""");
        }

        public override async Task Sum_over_truncate_works_correctly_in_projection_2(bool async)
        {
            await base.Sum_over_truncate_works_correctly_in_projection_2(async);

            AssertSql(
"""
SELECT `o`.`OrderID`, (
    SELECT COALESCE(SUM(TRUNCATE(`o0`.`UnitPrice` * `o0`.`UnitPrice`, 0)), 0.0)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) AS `Sum`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` < 10300
""");
        }

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
