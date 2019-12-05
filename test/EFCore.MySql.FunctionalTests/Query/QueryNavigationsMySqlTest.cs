using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_on_navigation(bool isAsync)
        {
            return base.Where_subquery_on_navigation(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_on_navigation2(bool isAsync)
        {
            return base.Where_subquery_on_navigation2(isAsync);
        }

        [ConditionalFact(Skip = "Issue #573")]
        public override void Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count()
        {
            base.Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count();
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Collection_select_nav_prop_first_or_default(bool isAsync)
        {
            return base.Collection_select_nav_prop_first_or_default(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Collection_select_nav_prop_first_or_default_then_nav_prop(bool isAsync)
        {
            return base.Collection_select_nav_prop_first_or_default_then_nav_prop(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_single_entity_value_subquery_works(bool isAsync)
        {
            return base.Project_single_entity_value_subquery_works(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_collection_FirstOrDefault_project_anonymous_type(bool isAsync)
        {
            return base.Select_collection_FirstOrDefault_project_anonymous_type(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_collection_FirstOrDefault_project_entity(bool isAsync)
        {
            return base.Select_collection_FirstOrDefault_project_entity(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Skip_Select_Navigation(bool isAsync)
        {
            return base.Skip_Select_Navigation(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Take_Select_Navigation(bool isAsync)
        {
            return base.Take_Select_Navigation(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_collection_FirstOrDefault_project_anonymous_type_client_eval(bool isAsync)
        {
            return base.Select_collection_FirstOrDefault_project_anonymous_type_client_eval(isAsync);
        }
    }
}
