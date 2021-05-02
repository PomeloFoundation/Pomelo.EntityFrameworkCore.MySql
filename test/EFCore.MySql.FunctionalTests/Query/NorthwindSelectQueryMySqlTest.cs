using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindSelectQueryMySqlTest : NorthwindSelectQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindSelectQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [ConditionalTheory]
        public override async Task Select_datetime_year_component(bool async)
        {
            await base.Select_datetime_year_component(async);

            AssertSql(
                @"SELECT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_month_component(bool async)
        {
            await base.Select_datetime_month_component(async);

            AssertSql(
                @"SELECT EXTRACT(month FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_day_of_year_component(bool async)
        {
            await base.Select_datetime_day_of_year_component(async);

            AssertSql(
                @"SELECT DAYOFYEAR(`o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_day_component(bool async)
        {
            await base.Select_datetime_day_component(async);

            AssertSql(
                @"SELECT EXTRACT(day FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_hour_component(bool async)
        {
            await base.Select_datetime_hour_component(async);

            AssertSql(
                @"SELECT EXTRACT(hour FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_minute_component(bool async)
        {
            await base.Select_datetime_minute_component(async);

            AssertSql(
                @"SELECT EXTRACT(minute FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_second_component(bool async)
        {
            await base.Select_datetime_second_component(async);

            AssertSql(
                @"SELECT EXTRACT(second FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_millisecond_component(bool async)
        {
            await base.Select_datetime_millisecond_component(async);

            AssertSql(
                @"SELECT (EXTRACT(microsecond FROM `o`.`OrderDate`)) DIV (1000)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Project_keyless_entity_FirstOrDefault_without_orderby(bool async)
        {
            return base.Project_keyless_entity_FirstOrDefault_without_orderby(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Collection_include_over_result_of_single_non_scalar(bool async)
        {
            return base.Collection_include_over_result_of_single_non_scalar(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_correlated_with_outer_1(bool async)
        {
            return base.SelectMany_correlated_with_outer_1(async);
        }

        // [SupportedServerVersionCondition(ServerVersion.CrossApplySupportKey)]
        [ConditionalTheory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        public override Task SelectMany_correlated_with_outer_2(bool async)
        {
            return base.SelectMany_correlated_with_outer_2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task SelectMany_correlated_with_outer_3(bool async)
        {
            return base.SelectMany_correlated_with_outer_3(async);
        }

        // TODO:
        // [SupportedServerVersionCondition(ServerVersion.CrossApplySupportKey)ey)]
        [ConditionalTheory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        public override Task SelectMany_correlated_with_outer_4(bool async)
        {
            return base.SelectMany_correlated_with_outer_4(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task SelectMany_correlated_with_outer_5(bool async)
        {
            return base.SelectMany_correlated_with_outer_5(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task SelectMany_correlated_with_outer_6(bool async)
        {
            return base.SelectMany_correlated_with_outer_6(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task SelectMany_correlated_with_outer_7(bool async)
        {
            return base.SelectMany_correlated_with_outer_7(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault_2(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault_2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_nested_collection_deep(bool async)
        {
            return base.Select_nested_collection_deep(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override void Select_nested_collection_multi_level()
        {
            base.Select_nested_collection_multi_level();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Project_single_element_from_collection_with_OrderBy_Distinct_and_FirstOrDefault_followed_by_projecting_length(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Distinct_and_FirstOrDefault_followed_by_projecting_length(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_SingleOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_SingleOrDefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Collection_projection_selecting_outer_element_followed_by_take(bool async)
        {
            return base.Collection_projection_selecting_outer_element_followed_by_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collection_after_distinct_not_containing_original_identifier(bool async)
        {
            return base.Correlated_collection_after_distinct_not_containing_original_identifier(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collection_after_distinct_with_complex_projection_containing_original_identifier(bool async)
        {
            return base.Correlated_collection_after_distinct_with_complex_projection_containing_original_identifier(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collection_after_groupby_with_complex_projection_containing_original_identifier(bool async)
        {
            return base.Correlated_collection_after_groupby_with_complex_projection_containing_original_identifier(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Projecting_after_navigation_and_distinct(bool async)
        {
            return base.Projecting_after_navigation_and_distinct(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_nested_collection_deep_distinct_no_identifiers(bool async)
        {
            return base.Select_nested_collection_deep_distinct_no_identifiers(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Take_on_correlated_collection_in_first(bool async)
        {
            return base.Take_on_correlated_collection_in_first(async);
        }

        [ConditionalTheory]
        public override Task Member_binding_after_ctor_arguments_fails_with_client_eval(bool async)
        {
            return AssertTranslationFailed(() => base.Member_binding_after_ctor_arguments_fails_with_client_eval(async));
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_whose_selector_references_outer_source(bool async)
        {
            return base.SelectMany_whose_selector_references_outer_source(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_collection_being_correlated_subquery_which_references_inner_and_outer_entity(bool async)
        {
            return base.SelectMany_with_collection_being_correlated_subquery_which_references_inner_and_outer_entity(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Do_not_erase_projection_mapping_when_adding_single_projection(bool async)
        {
            return base.Do_not_erase_projection_mapping_when_adding_single_projection(async);
        }

        [ConditionalTheory(Skip = "TODO: Seems to be a MySQL bug. Needs to be verified and reported, if not already.")]
        public override Task Take_on_top_level_and_on_collection_projection_with_outer_apply(bool async)
        {
            return base.Take_on_top_level_and_on_collection_projection_with_outer_apply(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
