using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsCollectionsSharedTypeQueryMySqlTest : ComplexNavigationsCollectionsSharedQueryTypeRelationalTestBase<
        ComplexNavigationsSharedTypeQueryMySqlTest.ComplexNavigationsSharedTypeQueryMySqlFixture>
    {
        public ComplexNavigationsCollectionsSharedTypeQueryMySqlTest(
            ComplexNavigationsSharedTypeQueryMySqlTest.ComplexNavigationsSharedTypeQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task SelectMany_with_Include1(bool async)
        {
            await base.SelectMany_with_Include1(async);

            AssertSql(
                @"SELECT `t`.`Id`, `t`.`OneToOne_Required_PK_Date`, `t`.`Level1_Optional_Id`, `t`.`Level1_Required_Id`, `t`.`Level2_Name`, `t`.`OneToMany_Optional_Inverse2Id`, `t`.`OneToMany_Required_Inverse2Id`, `t`.`OneToOne_Optional_PK_Inverse2Id`, `l`.`Id`, `t`.`Id0`, `t0`.`Id`, `t0`.`Level2_Optional_Id`, `t0`.`Level2_Required_Id`, `t0`.`Level3_Name`, `t0`.`OneToMany_Optional_Inverse3Id`, `t0`.`OneToMany_Required_Inverse3Id`, `t0`.`OneToOne_Optional_PK_Inverse3Id`, `t0`.`Id0`, `t0`.`Id00`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Optional_Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Optional_Inverse2Id`, `l0`.`OneToMany_Required_Inverse2Id`, `l0`.`OneToOne_Optional_PK_Inverse2Id`, `l1`.`Id` AS `Id0`
    FROM `Level1` AS `l0`
    INNER JOIN `Level1` AS `l1` ON `l0`.`Id` = `l1`.`Id`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`OneToMany_Optional_Inverse2Id`
LEFT JOIN (
    SELECT `l2`.`Id`, `l2`.`Level2_Optional_Id`, `l2`.`Level2_Required_Id`, `l2`.`Level3_Name`, `l2`.`OneToMany_Optional_Inverse3Id`, `l2`.`OneToMany_Required_Inverse3Id`, `l2`.`OneToOne_Optional_PK_Inverse3Id`, `t1`.`Id` AS `Id0`, `t1`.`Id0` AS `Id00`
    FROM `Level1` AS `l2`
    INNER JOIN (
        SELECT `l3`.`Id`, `l4`.`Id` AS `Id0`
        FROM `Level1` AS `l3`
        INNER JOIN `Level1` AS `l4` ON `l3`.`Id` = `l4`.`Id`
        WHERE (`l3`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l3`.`Level1_Required_Id` IS NOT NULL)) AND `l3`.`OneToMany_Required_Inverse2Id` IS NOT NULL
    ) AS `t1` ON `l2`.`Id` = `t1`.`Id`
    WHERE `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL)
) AS `t0` ON `t`.`Id` = `t0`.`OneToMany_Optional_Inverse3Id`
ORDER BY `l`.`Id`, `t`.`Id`, `t`.`Id0`, `t0`.`Id`, `t0`.`Id0`, `t0`.`Id00`");
        }

        public override async Task SelectMany_with_navigation_and_Distinct(bool async)
        {
            await base.SelectMany_with_navigation_and_Distinct(async);

            AssertSql(
                @"SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`, `t`.`Id`, `t0`.`Id`, `t0`.`OneToOne_Required_PK_Date`, `t0`.`Level1_Optional_Id`, `t0`.`Level1_Required_Id`, `t0`.`Level2_Name`, `t0`.`OneToMany_Optional_Inverse2Id`, `t0`.`OneToMany_Required_Inverse2Id`, `t0`.`OneToOne_Optional_PK_Inverse2Id`, `t0`.`Id0`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT DISTINCT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Optional_Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Optional_Inverse2Id`, `l0`.`OneToMany_Required_Inverse2Id`, `l0`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l0`
    INNER JOIN `Level1` AS `l1` ON `l0`.`Id` = `l1`.`Id`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`OneToMany_Optional_Inverse2Id`
LEFT JOIN (
    SELECT `l2`.`Id`, `l2`.`OneToOne_Required_PK_Date`, `l2`.`Level1_Optional_Id`, `l2`.`Level1_Required_Id`, `l2`.`Level2_Name`, `l2`.`OneToMany_Optional_Inverse2Id`, `l2`.`OneToMany_Required_Inverse2Id`, `l2`.`OneToOne_Optional_PK_Inverse2Id`, `l3`.`Id` AS `Id0`
    FROM `Level1` AS `l2`
    INNER JOIN `Level1` AS `l3` ON `l2`.`Id` = `l3`.`Id`
    WHERE (`l2`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l2`.`Level1_Required_Id` IS NOT NULL)) AND `l2`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t0` ON `l`.`Id` = `t0`.`OneToMany_Optional_Inverse2Id`
WHERE (`t`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`t`.`Level1_Required_Id` IS NOT NULL)) AND `t`.`OneToMany_Required_Inverse2Id` IS NOT NULL
ORDER BY `l`.`Id`, `t`.`Id`, `t0`.`Id`, `t0`.`Id0`");
        }

        public override async Task SelectMany_with_navigation_and_Distinct_projecting_columns_including_join_key(bool async)
        {
            await base.SelectMany_with_navigation_and_Distinct_projecting_columns_including_join_key(async);

            AssertSql(
                @"SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`, `t`.`Id`, `t`.`Name`, `t`.`FK`, `t0`.`Id`, `t0`.`OneToOne_Required_PK_Date`, `t0`.`Level1_Optional_Id`, `t0`.`Level1_Required_Id`, `t0`.`Level2_Name`, `t0`.`OneToMany_Optional_Inverse2Id`, `t0`.`OneToMany_Required_Inverse2Id`, `t0`.`OneToOne_Optional_PK_Inverse2Id`, `t0`.`Id0`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT DISTINCT `l0`.`Id`, `l0`.`Level2_Name` AS `Name`, `l0`.`OneToMany_Optional_Inverse2Id` AS `FK`
    FROM `Level1` AS `l0`
    INNER JOIN `Level1` AS `l1` ON `l0`.`Id` = `l1`.`Id`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`FK`
LEFT JOIN (
    SELECT `l2`.`Id`, `l2`.`OneToOne_Required_PK_Date`, `l2`.`Level1_Optional_Id`, `l2`.`Level1_Required_Id`, `l2`.`Level2_Name`, `l2`.`OneToMany_Optional_Inverse2Id`, `l2`.`OneToMany_Required_Inverse2Id`, `l2`.`OneToOne_Optional_PK_Inverse2Id`, `l3`.`Id` AS `Id0`
    FROM `Level1` AS `l2`
    INNER JOIN `Level1` AS `l3` ON `l2`.`Id` = `l3`.`Id`
    WHERE (`l2`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l2`.`Level1_Required_Id` IS NOT NULL)) AND `l2`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t0` ON `l`.`Id` = `t0`.`OneToMany_Optional_Inverse2Id`
ORDER BY `l`.`Id`, `t`.`Id`, `t`.`Name`, `t`.`FK`, `t0`.`Id`, `t0`.`Id0`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override async Task Take_Select_collection_Take(bool async)
        {
            await base.Take_Select_collection_Take(async);

            AssertSql(
                @"@__p_0='1'

SELECT `t`.`Id`, `t`.`Name`, `t0`.`Id`, `t0`.`Name`, `t0`.`Level1Id`, `t0`.`Level2Id`, `t0`.`Id0`, `t0`.`Date`, `t0`.`Name0`, `t0`.`Id00`
FROM (
    SELECT `l`.`Id`, `l`.`Name`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0
) AS `t`
LEFT JOIN LATERAL (
    SELECT `t1`.`Id`, `t1`.`Level2_Name` AS `Name`, `t1`.`OneToMany_Required_Inverse2Id` AS `Level1Id`, `t1`.`Level1_Required_Id` AS `Level2Id`, `l1`.`Id` AS `Id0`, `l1`.`Date`, `l1`.`Name` AS `Name0`, `t1`.`Id0` AS `Id00`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Required_Inverse2Id`, `l2`.`Id` AS `Id0`
        FROM `Level1` AS `l0`
        INNER JOIN `Level1` AS `l2` ON `l0`.`Id` = `l2`.`Id`
        WHERE ((`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL) AND (`t`.`Id` = `l0`.`OneToMany_Required_Inverse2Id`)
        ORDER BY `l0`.`Id`
        LIMIT 3
    ) AS `t1`
    INNER JOIN `Level1` AS `l1` ON `t1`.`Level1_Required_Id` = `l1`.`Id`
) AS `t0` ON TRUE
ORDER BY `t`.`Id`, `t0`.`Id`, `t0`.`Id00`, `t0`.`Id0`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override async Task Skip_Take_Select_collection_Skip_Take(bool async)
        {
            await base.Skip_Take_Select_collection_Skip_Take(async);

            AssertSql(
                @"@__p_0='1'

SELECT `t`.`Id`, `t`.`Name`, `t0`.`Id`, `t0`.`Name`, `t0`.`Level1Id`, `t0`.`Level2Id`, `t0`.`Id0`, `t0`.`Date`, `t0`.`Name0`, `t0`.`Id00`
FROM (
    SELECT `l`.`Id`, `l`.`Name`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0 OFFSET @__p_0
) AS `t`
LEFT JOIN LATERAL (
    SELECT `t1`.`Id`, `t1`.`Level2_Name` AS `Name`, `t1`.`OneToMany_Required_Inverse2Id` AS `Level1Id`, `t1`.`Level1_Required_Id` AS `Level2Id`, `l1`.`Id` AS `Id0`, `l1`.`Date`, `l1`.`Name` AS `Name0`, `t1`.`Id0` AS `Id00`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Required_Inverse2Id`, `l2`.`Id` AS `Id0`
        FROM `Level1` AS `l0`
        INNER JOIN `Level1` AS `l2` ON `l0`.`Id` = `l2`.`Id`
        WHERE ((`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL) AND (`t`.`Id` = `l0`.`OneToMany_Required_Inverse2Id`)
        ORDER BY `l0`.`Id`
        LIMIT 3 OFFSET 1
    ) AS `t1`
    INNER JOIN `Level1` AS `l1` ON `t1`.`Level1_Required_Id` = `l1`.`Id`
) AS `t0` ON TRUE
ORDER BY `t`.`Id`, `t0`.`Id`, `t0`.`Id00`, `t0`.`Id0`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Complex_query_with_let_collection_projection_FirstOrDefault(bool async)
        {
            return base.Complex_query_with_let_collection_projection_FirstOrDefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_after_different_filtered_include_different_level(bool async)
        {
            return base.Filtered_include_after_different_filtered_include_different_level(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(bool async)
        {
            return base.Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(async);
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
        public override Task Filtered_include_complex_three_level_with_middle_having_filter1(bool async)
        {
            return base.Filtered_include_complex_three_level_with_middle_having_filter1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_complex_three_level_with_middle_having_filter2(bool async)
        {
            return base.Filtered_include_complex_three_level_with_middle_having_filter2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_Take_with_another_Take_on_top_level(bool async)
        {
            return base.Filtered_include_Take_with_another_Take_on_top_level(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Filtered_include_Skip_Take_with_another_Skip_Take_on_top_level(bool async)
        {
            return base.Filtered_include_Skip_Take_with_another_Skip_Take_on_top_level(async);
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

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
