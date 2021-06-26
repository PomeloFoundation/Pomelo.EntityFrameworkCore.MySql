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

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_after_different_filtered_include_different_level(bool async)
        {
            return base.Filtered_include_after_different_filtered_include_different_level(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_after_different_filtered_include_same_level(bool async)
        {
            return base.Filtered_include_after_different_filtered_include_same_level(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_after_reference_navigation(bool async)
        {
            return base.Filtered_include_after_reference_navigation(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_and_non_filtered_include_on_same_navigation1(bool async)
        {
            return base.Filtered_include_and_non_filtered_include_on_same_navigation1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_and_non_filtered_include_on_same_navigation2(bool async)
        {
            return base.Filtered_include_and_non_filtered_include_on_same_navigation2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_basic_OrderBy_Skip(bool async)
        {
            return base.Filtered_include_basic_OrderBy_Skip(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_basic_OrderBy_Skip_Take(bool async)
        {
            return base.Filtered_include_basic_OrderBy_Skip_Take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_basic_OrderBy_Take(bool async)
        {
            return base.Filtered_include_basic_OrderBy_Take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_complex_three_level_with_middle_having_filter1(bool async)
        {
            return base.Filtered_include_complex_three_level_with_middle_having_filter1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_complex_three_level_with_middle_having_filter2(bool async)
        {
            return base.Filtered_include_complex_three_level_with_middle_having_filter2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_on_ThenInclude(bool async)
        {
            return base.Filtered_include_on_ThenInclude(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_same_filter_set_on_same_navigation_twice(bool async)
        {
            return base.Filtered_include_same_filter_set_on_same_navigation_twice(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Lift_projection_mapping_when_pushing_down_subquery(bool async)
        {
            return base.Lift_projection_mapping_when_pushing_down_subquery(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Project_collection_navigation_nested_with_take(bool async)
        {
            return base.Project_collection_navigation_nested_with_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_subquery_single_nested_subquery(bool async)
        {
            return base.Select_subquery_single_nested_subquery(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_subquery_single_nested_subquery2(bool async)
        {
            return base.Select_subquery_single_nested_subquery2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_context_accessed_inside_filter()
        {
            base.Filtered_include_context_accessed_inside_filter();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_context_accessed_inside_filter_correlated()
        {
            base.Filtered_include_context_accessed_inside_filter_correlated();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_is_considered_loaded()
        {
            base.Filtered_include_is_considered_loaded();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_Skip_without_OrderBy()
        {
            base.Filtered_include_Skip_without_OrderBy();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_Take_without_OrderBy()
        {
            base.Filtered_include_Take_without_OrderBy();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_variable_used_inside_filter()
        {
            base.Filtered_include_variable_used_inside_filter();
        }
    }
}
