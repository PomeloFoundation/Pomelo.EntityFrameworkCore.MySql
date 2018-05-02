using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
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
                @"@__ef_filter__TenantPrefix_0='B' (Size = 1)

SELECT COUNT(*)
FROM ""Customers"" AS ""c""
WHERE (""c"".""CompanyName"" LIKE @__ef_filter__TenantPrefix_0 || '%' AND (substr(""c"".""CompanyName"", 1, length(@__ef_filter__TenantPrefix_0)) = @__ef_filter__TenantPrefix_0)) OR (@__ef_filter__TenantPrefix_0 = '')");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        private void AssertContainsSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, assertOrder: false);
    }
}
