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
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `t2`.`OrderID`, `t2`.`CustomerID`, `t2`.`EmployeeID`, `t2`.`OrderDate`, `t2`.`CustomerID0`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `t0`.`OrderID`, `t0`.`CustomerID`, `t0`.`EmployeeID`, `t0`.`OrderDate`, `t`.`CustomerID` AS `CustomerID0`
    FROM (
        SELECT `o`.`CustomerID`
        FROM `Orders` AS `o`
        WHERE `o`.`CustomerID` = `c`.`CustomerID`
        GROUP BY `o`.`CustomerID`
    ) AS `t`
    LEFT JOIN (
        SELECT `t1`.`OrderID`, `t1`.`CustomerID`, `t1`.`EmployeeID`, `t1`.`OrderDate`
        FROM (
            SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`, ROW_NUMBER() OVER(PARTITION BY `o0`.`CustomerID` ORDER BY `o0`.`OrderDate` DESC) AS `row`
            FROM `Orders` AS `o0`
            WHERE `o0`.`CustomerID` = `c`.`CustomerID`
        ) AS `t1`
        WHERE `t1`.`row` <= 1
    ) AS `t0` ON `t`.`CustomerID` = `t0`.`CustomerID`
) AS `t2` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`, `t2`.`CustomerID0`");
        }

        public override async Task Complex_query_with_groupBy_in_subquery1(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery1(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `t`.`Sum`, `t`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `t` ON TRUE
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Complex_query_with_groupBy_in_subquery2(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery2(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `t`.`Max`, `t`.`Sum`, `t`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `t` ON TRUE
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Complex_query_with_groupBy_in_subquery3(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery3(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `t`.`Max`, `t`.`Sum`, `t`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
) AS `t` ON TRUE
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Select_nested_collection_with_groupby(bool async)
        {
            await base.Select_nested_collection_with_groupby(async);

            AssertSql(
                @"SELECT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`), `c`.`CustomerID`, `t`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o0`.`OrderID`
    FROM `Orders` AS `o0`
    WHERE `c`.`CustomerID` = `o0`.`CustomerID`
    GROUP BY `o0`.`OrderID`
) AS `t` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override async Task GroupBy_group_Distinct_Select_Distinct_aggregate(bool async)
        {
            await base.GroupBy_group_Distinct_Select_Distinct_aggregate(async);

            AssertSql(
                @"SELECT `o`.`CustomerID` AS `Key`, (
    SELECT DISTINCT MAX(DISTINCT (`t`.`OrderDate`))
    FROM (
        SELECT DISTINCT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
        FROM `Orders` AS `o0`
        WHERE (`o`.`CustomerID` = `o0`.`CustomerID`) OR (`o`.`CustomerID` IS NULL AND (`o0`.`CustomerID` IS NULL))
    ) AS `t`) AS `Max`
FROM `Orders` AS `o`
GROUP BY `o`.`CustomerID`");
        }

        [ConditionalTheory(Skip = "Does not work when using ONLY_FULL_GROUP_BY. See https://github.com/dotnet/efcore/issues/19027")]
        public override Task GroupBy_scalar_subquery(bool async)
        {
            return base.GroupBy_scalar_subquery(async);
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
