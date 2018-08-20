using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class EscapesMySqlNoBackslashesTest : EscapesMySqlTestBase<NorthwindQueryMySqlNoBackslashesFixture<NoopModelCustomizer>>
    {
        public EscapesMySqlNoBackslashesTest(NorthwindQueryMySqlNoBackslashesFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper) : base(fixture)
        {
            SetSqlMode("NO_BACKSLASH_ESCAPES");

            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "see issue #667")]
        public override void Input_query_escapes_parameter()
        {

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
            AssertBaseline(@"SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` = 'Back\slash''s Operation'");
        }

        [Fact]
        public void Where_contains_query_escapes()
        {
            base.Where_contains_query_escapes(@"Back\slash's Operation", "B's Beverages");
            AssertBaseline(@"SELECT COUNT(*)
FROM `Customers` AS `x`
WHERE `x`.`CompanyName` IN ('Back\slash''s Operation', 'B''s Beverages')");
        }
    }
}
