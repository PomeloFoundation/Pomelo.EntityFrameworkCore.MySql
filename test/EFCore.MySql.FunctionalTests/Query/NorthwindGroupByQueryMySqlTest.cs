using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindGroupByQueryMySqlTest : NorthwindGroupByQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindGroupByQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        public override async Task AsEnumerable_in_subquery_for_GroupBy(bool async)
        {
            await base.AsEnumerable_in_subquery_for_GroupBy(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `s`.`OrderID`, `s`.`CustomerID`, `s`.`EmployeeID`, `s`.`OrderDate`, `s`.`CustomerID0`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o3`.`OrderID`, `o3`.`CustomerID`, `o3`.`EmployeeID`, `o3`.`OrderDate`, `o1`.`CustomerID` AS `CustomerID0`
    FROM (
        SELECT `o`.`CustomerID`
        FROM `Orders` AS `o`
        WHERE `o`.`CustomerID` = `c`.`CustomerID`
        GROUP BY `o`.`CustomerID`
    ) AS `o1`
    LEFT JOIN (
        SELECT `o2`.`OrderID`, `o2`.`CustomerID`, `o2`.`EmployeeID`, `o2`.`OrderDate`
        FROM (
            SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`, ROW_NUMBER() OVER(PARTITION BY `o0`.`CustomerID` ORDER BY `o0`.`OrderDate` DESC) AS `row`
            FROM `Orders` AS `o0`
            WHERE `o0`.`CustomerID` = `c`.`CustomerID`
        ) AS `o2`
        WHERE `o2`.`row` <= 1
    ) AS `o3` ON `o1`.`CustomerID` = `o3`.`CustomerID`
) AS `s` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`, `s`.`CustomerID0`
""");
        }

        public override async Task Complex_query_with_groupBy_in_subquery1(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery1(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `o0`.`Sum`, `o0`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `o0` ON TRUE
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Complex_query_with_groupBy_in_subquery2(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery2(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `o0`.`Max`, `o0`.`Sum`, `o0`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `o0` ON TRUE
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Complex_query_with_groupBy_in_subquery3(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery3(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `o0`.`Max`, `o0`.`Sum`, `o0`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
) AS `o0` ON TRUE
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Select_nested_collection_with_groupby(bool async)
        {
            await base.Select_nested_collection_with_groupby(async);

        AssertSql(
"""
SELECT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`), `c`.`CustomerID`, `o1`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o0`.`OrderID`
    FROM `Orders` AS `o0`
    WHERE `c`.`CustomerID` = `o0`.`CustomerID`
    GROUP BY `o0`.`OrderID`
) AS `o1` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override async Task GroupBy_group_Distinct_Select_Distinct_aggregate(bool async)
        {
            await base.GroupBy_group_Distinct_Select_Distinct_aggregate(async);

            AssertSql(
                @"SELECT `o`.`CustomerID` AS `Key`, MAX(DISTINCT (`o`.`OrderDate`)) AS `Max`
FROM `Orders` AS `o`
GROUP BY `o`.`CustomerID`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task GroupBy_Count_in_projection(bool async)
        {
            return base.GroupBy_Count_in_projection(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql", "0.0.0-mariadb")]
        public override Task GroupBy_group_Where_Select_Distinct_aggregate(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/898.
            return base.GroupBy_group_Where_Select_Distinct_aggregate(async);
        }

        [SupportedServerVersionCondition("8.0.0-mysql", "0.0.0-mariadb")] // Is an issue issue in MySQL 5.7.34, but not in 8.0.25.
        public override Task GroupBy_constant_with_where_on_grouping_with_aggregate_operators(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/980.
            return base.GroupBy_constant_with_where_on_grouping_with_aggregate_operators(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
