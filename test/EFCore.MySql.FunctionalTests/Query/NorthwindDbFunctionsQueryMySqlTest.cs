using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindDbFunctionsQueryMySqlTest : NorthwindDbFunctionsQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindDbFunctionsQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Like_literal(bool async)
        {
            await base.Like_literal(async);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '%M%'");
        }

        public override async Task Like_identity(bool async)
        {
            await base.Like_identity(async);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE `c`.`ContactName`");
        }

        public override async Task Like_literal_with_escape(bool async)
        {
            await base.Like_literal_with_escape(async);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '!%' ESCAPE '!'");
        }

        protected override string CaseInsensitiveCollation
            => "utf8mb4_general_ci";

        protected override string CaseSensitiveCollation
            => "utf8mb4_bin";

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
