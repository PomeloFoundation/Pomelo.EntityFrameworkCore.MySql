using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class QueryNavigationsMySqlTest : QueryNavigationsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public QueryNavigationsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_subquery_on_navigation(bool isAsync)
        {
            return base.Where_subquery_on_navigation(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_subquery_on_navigation2(bool isAsync)
        {
            return base.Where_subquery_on_navigation2(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override void Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count()
        {
            base.Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count();
        }
    }
}
