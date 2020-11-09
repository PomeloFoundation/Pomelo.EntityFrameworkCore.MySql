using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsWeakQueryMySqlTest : ComplexNavigationsWeakQueryRelationalTestBase<ComplexNavigationsWeakQueryMySqlFixture>
    {
        public ComplexNavigationsWeakQueryMySqlTest(ComplexNavigationsWeakQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Including_reference_navigation_and_projecting_collection_navigation_2(bool async)
        {
            return base.Including_reference_navigation_and_projecting_collection_navigation_2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Lift_projection_mapping_when_pushing_down_subquery(bool async)
        {
            return base.Lift_projection_mapping_when_pushing_down_subquery(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(bool async)
        {
            return base.SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Select_subquery_single_nested_subquery(bool async)
        {
            await base.Select_subquery_single_nested_subquery(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Select_subquery_single_nested_subquery2(bool async)
        {
            await base.Select_subquery_single_nested_subquery2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_basic_OrderBy_Take(bool async)
        {
            await base.Filtered_include_basic_OrderBy_Take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_basic_OrderBy_Skip(bool async)
        {
            await base.Filtered_include_basic_OrderBy_Skip(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_basic_OrderBy_Skip_Take(bool async)
        {
            await base.Filtered_include_basic_OrderBy_Skip_Take(async);
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
        public override async Task Filtered_include_on_ThenInclude(bool async)
        {
            await base.Filtered_include_on_ThenInclude(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_after_reference_navigation(bool async)
        {
            await base.Filtered_include_after_reference_navigation(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_after_different_filtered_include_same_level(bool async)
        {
            await base.Filtered_include_after_different_filtered_include_same_level(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_same_filter_set_on_same_navigation_twice(bool async)
        {
            await base.Filtered_include_same_filter_set_on_same_navigation_twice(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_and_non_filtered_include_on_same_navigation1(bool async)
        {
            await base.Filtered_include_and_non_filtered_include_on_same_navigation1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_and_non_filtered_include_on_same_navigation2(bool async)
        {
            await base.Filtered_include_and_non_filtered_include_on_same_navigation2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_variable_used_inside_filter()
        {
            base.Filtered_include_variable_used_inside_filter();
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
        public override async Task Filtered_include_OrderBy_split(bool async)
        {
            await base.Filtered_include_OrderBy_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_ThenInclude_OrderBy_split(bool async)
        {
            await base.Filtered_ThenInclude_OrderBy_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_ThenInclude_OrderBy_split(bool async)
        {
            await base.Filtered_include_ThenInclude_OrderBy_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_basic_OrderBy_Take_split(bool async)
        {
            await base.Filtered_include_basic_OrderBy_Take_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_basic_OrderBy_Skip_split(bool async)
        {
            await base.Filtered_include_basic_OrderBy_Skip_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_basic_OrderBy_Skip_Take_split(bool async)
        {
            await base.Filtered_include_basic_OrderBy_Skip_Take_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_Skip_without_OrderBy_split()
        {
            base.Filtered_include_Skip_without_OrderBy_split();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_Take_without_OrderBy_split()
        {
            base.Filtered_include_Take_without_OrderBy_split();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_on_ThenInclude_split(bool async)
        {
            await base.Filtered_include_on_ThenInclude_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_after_reference_navigation_split(bool async)
        {
            await base.Filtered_include_after_reference_navigation_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_after_different_filtered_include_same_level_split(bool async)
        {
            await base.Filtered_include_after_different_filtered_include_same_level_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_after_different_filtered_include_different_level_split(bool async)
        {
            await base.Filtered_include_after_different_filtered_include_different_level_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_same_filter_set_on_same_navigation_twice_split(bool async)
        {
            await base.Filtered_include_same_filter_set_on_same_navigation_twice_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_and_non_filtered_include_on_same_navigation1_split(bool async)
        {
            await base.Filtered_include_and_non_filtered_include_on_same_navigation1_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_and_non_filtered_include_on_same_navigation2_split(bool async)
        {
            await base.Filtered_include_and_non_filtered_include_on_same_navigation2_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_complex_three_level_with_middle_having_filter1_split(bool async)
        {
            await base.Filtered_include_complex_three_level_with_middle_having_filter1_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override async Task Filtered_include_complex_three_level_with_middle_having_filter2_split(bool async)
        {
            await base.Filtered_include_complex_three_level_with_middle_having_filter2_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_variable_used_inside_filter_split()
        {
            base.Filtered_include_variable_used_inside_filter_split();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_context_accessed_inside_filter_split()
        {
            base.Filtered_include_context_accessed_inside_filter_split();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_context_accessed_inside_filter_correlated_split()
        {
            base.Filtered_include_context_accessed_inside_filter_correlated_split();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_is_considered_loaded_split()
        {
            base.Filtered_include_is_considered_loaded_split();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Filtered_include_is_considered_loaded()
        {
            base.Filtered_include_is_considered_loaded();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation_split(bool async)
        {
            return base.Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only_split(bool async)
        {
            return base.Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes_split(bool async)
        {
            return base.Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes_split(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Project_collection_navigation_nested_with_take(bool async)
        {
            return base.Project_collection_navigation_nested_with_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task SelectMany_with_outside_reference_to_joined_table_correctly_translated_to_apply(bool async)
        {
            return base.SelectMany_with_outside_reference_to_joined_table_correctly_translated_to_apply(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_after_different_filtered_include_different_level(bool async)
        {
            return base.Filtered_include_after_different_filtered_include_different_level(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(bool async)
        {
            return base.Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(bool async)
        {
            return base.Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(bool async)
        {
            return base.Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Let_let_contains_from_outer_let(bool async)
        {
            return base.Let_let_contains_from_outer_let(async);
        }

        [ConditionalTheory(Skip = "https://bugs.mysql.com/bug.php?id=101276")]
        public override Task Filtered_include_complex_three_level_with_middle_having_filter1(bool async)
        {
            return base.Filtered_include_complex_three_level_with_middle_having_filter1(async);
        }

        [ConditionalTheory(Skip = "https://bugs.mysql.com/bug.php?id=101276")]
        public override Task Filtered_include_complex_three_level_with_middle_having_filter2(bool async)
        {
            return base.Filtered_include_complex_three_level_with_middle_having_filter2(async);
        }

        [ConditionalTheory]
        public override async Task SelectMany_with_navigation_and_Distinct(bool async)
        {
            var message = (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.SelectMany_with_navigation_and_Distinct(async))).Message;

            Assert.Equal(RelationalStrings.InsufficientInformationToIdentifyOuterElementOfCollectionJoin, message);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
        }
    }
}
