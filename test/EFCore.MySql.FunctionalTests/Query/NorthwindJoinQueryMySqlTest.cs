using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindJoinQueryMySqlTest : NorthwindJoinQueryRelationalTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindJoinQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task SelectMany_correlated_subquery_take(bool async)
        {
            return base.SelectMany_correlated_subquery_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Distinct_SelectMany_correlated_subquery_take(bool async)
        {
            return base.Distinct_SelectMany_correlated_subquery_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Distinct_SelectMany_correlated_subquery_take_2(bool async)
        {
            return base.Distinct_SelectMany_correlated_subquery_take_2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Take_SelectMany_correlated_subquery_take(bool async)
        {
            return base.Take_SelectMany_correlated_subquery_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_client_eval(bool async)
        {
            return base.SelectMany_with_client_eval(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_client_eval_with_collection_shaper(bool async)
        {
            return base.SelectMany_with_client_eval_with_collection_shaper(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_client_eval_with_collection_shaper_ignored(bool async)
        {
            return base.SelectMany_with_client_eval_with_collection_shaper_ignored(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_selecting_outer_entity(bool async)
        {
            return base.SelectMany_with_selecting_outer_entity(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_selecting_outer_element(bool async)
        {
            return base.SelectMany_with_selecting_outer_element(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_selecting_outer_entity_column_and_inner_column(bool async)
        {
            return base.SelectMany_with_selecting_outer_entity_column_and_inner_column(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Take_in_collection_projection_with_FirstOrDefault_on_top_level(bool async)
        {
            return base.Take_in_collection_projection_with_FirstOrDefault_on_top_level(async);
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
