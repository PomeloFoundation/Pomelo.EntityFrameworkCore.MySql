using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FiltersMySqlTest : FiltersTestBase<NorthwindQueryMySqlFixture<NorthwindFiltersCustomizer>>
    {
        public FiltersMySqlTest(NorthwindQueryMySqlFixture<NorthwindFiltersCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void Count_query()
        {
            base.Count_query();

            AssertSql(
                @"@__ef_filter__TenantPrefix_0='B' (Size = 4000)

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE (@__ef_filter__TenantPrefix_0 = '') OR (`c`.`CompanyName` IS NOT NULL AND ((`c`.`CompanyName` LIKE CONCAT(@__ef_filter__TenantPrefix_0, '%')) AND ((LEFT(`c`.`CompanyName`, CHAR_LENGTH(CONVERT(@__ef_filter__TenantPrefix_0 USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(@__ef_filter__TenantPrefix_0 USING utf8mb4) COLLATE utf8mb4_bin) OR (LEFT(`c`.`CompanyName`, CHAR_LENGTH(CONVERT(@__ef_filter__TenantPrefix_0 USING utf8mb4) COLLATE utf8mb4_bin)) IS NULL AND CONVERT(@__ef_filter__TenantPrefix_0 USING utf8mb4) COLLATE utf8mb4_bin IS NULL))))");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        private void AssertContainsSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
