using System;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class EscapesMySqlTest : EscapesMySqlTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        private readonly ITestOutputHelper _output;

        public EscapesMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper) : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);

            _output = testOutputHelper;
        }

        [Fact]
        public override void Input_query_escapes_parameter()
        {
            base.Input_query_escapes_parameter();
            AssertBaseline(@"@p0='ESCAPETEST' (Nullable = false) (Size = 255)
@p1='' (Size = 4000)
@p2='' (Size = 4000)
@p3='Back\slash's Operation' (Size = 4000)
@p4='' (Size = 4000)
@p5='' (Size = 4000)
@p6='' (Size = 4000)
@p7='' (Size = 4000)
@p8='' (Size = 4000)
@p9='' (Size = 4000)
@p10='' (Size = 4000)

INSERT INTO `Customers` (`CustomerID`, `Address`, `City`, `CompanyName`, `ContactName`, `ContactTitle`, `Country`, `Fax`, `Phone`, `PostalCode`, `Region`)
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10);",
                @"@__companyName_0='Back\slash's Operation' (Size = 4000)

SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` = @__companyName_0");
        }

        [Fact]
        public override void Where_query_escapes_literal()
        {
            base.Where_query_escapes_literal();
            AssertBaseline(@"SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` = 'B''s Beverages'");
        }

        [Fact]
        public void Where_query_escapes_parameter()
        {
            base.Where_query_escapes_parameter(@"Back\slash's Operation");
            AssertBaseline(@"@__companyName_0='Back\slash's Operation' (Size = 4000)

SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` = @__companyName_0");
        }

        [Fact]
        public void Where_contains_query_escapes()
        {
            base.Where_contains_query_escapes(@"Back\slash's Operation", "B's Beverages");
            AssertBaseline(@"SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` IN ('Back\\slash''s Operation', 'B''s Beverages')");
        }
    }
}
