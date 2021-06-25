using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsCollectionsSplitSharedTypeQueryMySqlTest : ComplexNavigationsCollectionsSplitSharedQueryTypeRelationalTestBase<ComplexNavigationsSharedTypeQueryMySqlFixture>
    {
        public ComplexNavigationsCollectionsSplitSharedTypeQueryMySqlTest(
            ComplexNavigationsSharedTypeQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Complex_query_with_let_collection_projection_FirstOrDefault(bool async)
        {
            return base.Complex_query_with_let_collection_projection_FirstOrDefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Take_Select_collection_Take(bool async)
        {
            return base.Take_Select_collection_Take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Skip_Take_Select_collection_Skip_Take(bool async)
        {
            return base.Skip_Take_Select_collection_Skip_Take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(bool async)
        {
            return base.Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(bool async)
        {
            return base.Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(bool async)
        {
            return base.Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Filtered_include_Take_with_another_Take_on_top_level(bool async)
        {
            return base.Filtered_include_Take_with_another_Take_on_top_level(async);
        }
    }
}
