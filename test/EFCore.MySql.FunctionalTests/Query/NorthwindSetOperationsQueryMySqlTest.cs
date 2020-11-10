using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindSetOperationsQueryMySqlTest : NorthwindSetOperationsQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindSetOperationsQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Intersect(bool async)
        {
            await base.Intersect(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
INTERSECT
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM `Customers` AS `c0`
WHERE `c0`.`ContactName` LIKE '%Thomas%'");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Intersect_nested(bool async)
        {
            await base.Intersect_nested(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'México D.F.'
INTERSECT
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM `Customers` AS `c0`
WHERE `c0`.`ContactTitle` = 'Owner'
INTERSECT
SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
FROM `Customers` AS `c1`
WHERE `c1`.`Fax` IS NOT NULL");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Intersect_non_entity(bool async)
        {
            await base.Intersect_non_entity(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'México D.F.'
INTERSECT
SELECT `c0`.`CustomerID`
FROM `Customers` AS `c0`
WHERE `c0`.`ContactTitle` = 'Owner'");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptInterceptPrecedence))]
        public override async Task Union_Intersect(bool async)
        {
            await base.Union_Intersect(async);

            AssertSql(
                @"(
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE `c`.`City` = 'Berlin'
    UNION
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` = 'London'
)
INTERSECT
SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
FROM `Customers` AS `c1`
WHERE `c1`.`ContactName` LIKE '%Thomas%'");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Except(bool async)
        {
            await base.Except(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
EXCEPT
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM `Customers` AS `c0`
WHERE `c0`.`ContactName` LIKE '%Thomas%'");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Except_simple_followed_by_projecting_constant(bool async)
        {
            await base.Except_simple_followed_by_projecting_constant(async);

            AssertSql(
                @"SELECT 1
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    EXCEPT
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
) AS `t`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Except_nested(bool async)
        {
            await base.Except_nested(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactTitle` = 'Owner'
EXCEPT
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM `Customers` AS `c0`
WHERE `c0`.`City` = 'México D.F.'
EXCEPT
SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
FROM `Customers` AS `c1`
WHERE `c1`.`City` = 'Seattle'");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Except_non_entity(bool async)
        {
            await base.Except_non_entity(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`ContactTitle` = 'Owner'
EXCEPT
SELECT `c0`.`CustomerID`
FROM `Customers` AS `c0`
WHERE `c0`.`City` = 'México D.F.'");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Select_Except_reference_projection(bool async)
        {
            await base.Select_Except_reference_projection(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
EXCEPT
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM `Orders` AS `o0`
LEFT JOIN `Customers` AS `c0` ON `o0`.`CustomerID` = `c0`.`CustomerID`
WHERE `o0`.`CustomerID` = 'ALFKI'");
        }

        [ConditionalTheory(Skip = "TODO: MySQL does not seem to allow an ORDER BY or LIMIT clause directly in a SELECT statement that is part of a UNION.")]
        public override Task Union_Take_Union_Take(bool async)
        {
            // TODO: MySQL does not seem to allow an ORDER BY or LIMIT clause directly in a SELECT statement that is part of a UNION.
            //       To make this work, the SELECT statement containing the ORDER BY and/or LIMIT clause needs to be wrapped by another
            //       SELECT statement.
            return base.Union_Take_Union_Take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override async Task Union_Select_scalar(bool async)
        {
            await base.Union_Select_scalar(async);

            AssertSql(
                @"SELECT 1
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    EXCEPT
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
) AS `t`");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
