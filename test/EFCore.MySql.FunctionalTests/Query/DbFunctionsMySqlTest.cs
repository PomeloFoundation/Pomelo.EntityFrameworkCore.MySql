using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class DbFunctionsMySqlTest : DbFunctionsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public DbFunctionsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        [ConditionalFact]
        public override void Like_literal()
        {
            base.Like_literal();

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '%M%'");
        }

        [ConditionalFact]
        public override void Like_identity()
        {
            base.Like_identity();

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE `c`.`ContactName`");
        }

        [ConditionalFact]
        public override void Like_literal_with_escape()
        {
            base.Like_literal_with_escape();

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '!%' ESCAPE '!'");
        }

        private void AssertSql(params string[] expected) => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
