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
    public class NorthwindNavigationsQueryMySqlTest : NorthwindNavigationsQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindNavigationsQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [ConditionalTheory(Skip = "Issue #573")]
        public override Task Where_subquery_on_navigation(bool async)
        {
            return base.Where_subquery_on_navigation(async);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        public override Task Where_subquery_on_navigation2(bool async)
        {
            return base.Where_subquery_on_navigation2(async);
        }

        [ConditionalFact(Skip = "Issue #573")]
        public override void Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count()
        {
            base.Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Collection_select_nav_prop_first_or_default(bool async)
        {
            return base.Collection_select_nav_prop_first_or_default(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Collection_select_nav_prop_first_or_default_then_nav_prop(bool async)
        {
            return base.Collection_select_nav_prop_first_or_default_then_nav_prop(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Project_single_entity_value_subquery_works(bool async)
        {
            return base.Project_single_entity_value_subquery_works(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_collection_FirstOrDefault_project_anonymous_type(bool async)
        {
            return base.Select_collection_FirstOrDefault_project_anonymous_type(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_collection_FirstOrDefault_project_entity(bool async)
        {
            return base.Select_collection_FirstOrDefault_project_entity(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Skip_Select_Navigation(bool async)
        {
            return base.Skip_Select_Navigation(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Take_Select_Navigation(bool async)
        {
            return base.Take_Select_Navigation(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_collection_FirstOrDefault_project_anonymous_type_client_eval(bool async)
        {
            return base.Select_collection_FirstOrDefault_project_anonymous_type_client_eval(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
