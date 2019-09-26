using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class EscapesMySqlNoBackslashesTest : EscapesMySqlTestBase<NorthwindEscapesNoBackslashesMySqlFixture<NoopModelCustomizer>>
    {
        public EscapesMySqlNoBackslashesTest(NorthwindEscapesNoBackslashesMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper) : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override string Mode => "NO_BACKSLASH_ESCAPES";

        protected void AssertSql(params string[] expected)
        {
            Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
        }

        [ConditionalFact]
        public virtual void Input_query_escapes_parameter()
        {
            base.Input_query_escapes_parameter(@"Back\\slash's Insert Operation");
 
            AssertSql(
                @"@p0='NO_BACKSLASH_ESCAPES' (Nullable = false)

SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)",
                //
                @"@p0='ESCBCKSLINS' (Nullable = false) (Size = 255)
@p1='' (Size = 4000)
@p2='' (Size = 4000)
@p3='Back\slash's Insert Operation' (Size = 4000)
@p4='' (Size = 4000)
@p5='' (Size = 4000)
@p6='' (Size = 4000)
@p7='' (Size = 4000)
@p8='' (Size = 4000)
@p9='' (Size = 4000)
@p10='' (Size = 4000)

INSERT INTO `Customers` (`CustomerID`, `Address`, `City`, `CompanyName`, `ContactName`, `ContactTitle`, `Country`, `Fax`, `Phone`, `PostalCode`, `Region`)
VALUES ('ESCBCKSLINS', NULL, NULL, 'Back\slash''s Insert Operation', NULL, NULL, NULL, NULL, NULL, NULL, NULL);",
                //
                @"@p0='NO_BACKSLASH_ESCAPES' (Nullable = false)

SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)",
                //
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CompanyName` = 'Back\slash''s Insert Operation') AND `c`.`CompanyName` IS NOT NULL",
                //
                @"@p0='NO_BACKSLASH_ESCAPES' (Nullable = false)

SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)");
        }
        
        public virtual async Task Where_query_escapes_literal(bool isAsync)
        {
            await base.Where_query_escapes_literal(isAsync, @"Back\slash's Operation");
 
            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` = 'B''s Beverages'");
        }
        /*
        public override async Task Where_query_escapes_parameter(bool isAsync)
        {
            await base.Where_query_escapes_parameter(isAsync);

            AssertSql(
                @"@__companyName_0='Back\slash's Operation' (Size = 4000)

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE ((`c`.`CompanyName` = @__companyName_0) AND (`c`.`CompanyName` IS NOT NULL AND @__companyName_0 IS NOT NULL)) OR (`c`.`CompanyName` IS NULL AND @__companyName_0 IS NULL)");
        }

        public override async Task Where_contains_query_escapes(bool isAsync)
        {
            await base.Where_contains_query_escapes(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` IN ('Back\slash''s Operation', 'B''s Beverages')");
        }
        */
    }
}
