using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindGroupByQueryMySqlTest : NorthwindGroupByQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindGroupByQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task AsEnumerable_in_subquery_for_GroupBy(bool async)
        {
            await base.AsEnumerable_in_subquery_for_GroupBy(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `s`.`OrderID`, `s`.`CustomerID`, `s`.`EmployeeID`, `s`.`OrderDate`, `s`.`CustomerID0`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o3`.`OrderID`, `o3`.`CustomerID`, `o3`.`EmployeeID`, `o3`.`OrderDate`, `o1`.`CustomerID` AS `CustomerID0`
    FROM (
        SELECT `o`.`CustomerID`
        FROM `Orders` AS `o`
        WHERE `o`.`CustomerID` = `c`.`CustomerID`
        GROUP BY `o`.`CustomerID`
    ) AS `o1`
    LEFT JOIN (
        SELECT `o2`.`OrderID`, `o2`.`CustomerID`, `o2`.`EmployeeID`, `o2`.`OrderDate`
        FROM (
            SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`, ROW_NUMBER() OVER(PARTITION BY `o0`.`CustomerID` ORDER BY `o0`.`OrderDate` DESC) AS `row`
            FROM `Orders` AS `o0`
            WHERE `o0`.`CustomerID` = `c`.`CustomerID`
        ) AS `o2`
        WHERE `o2`.`row` <= 1
    ) AS `o3` ON `o1`.`CustomerID` = `o3`.`CustomerID`
) AS `s` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`, `s`.`CustomerID0`
""");
        }

        public override async Task Complex_query_with_groupBy_in_subquery1(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery1(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `o0`.`Sum`, `o0`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `o0` ON TRUE
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Complex_query_with_groupBy_in_subquery2(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery2(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `o0`.`Max`, `o0`.`Sum`, `o0`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `o0` ON TRUE
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Complex_query_with_groupBy_in_subquery3(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery3(async);

        AssertSql(
"""
SELECT `c`.`CustomerID`, `o0`.`Max`, `o0`.`Sum`, `o0`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
) AS `o0` ON TRUE
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Select_nested_collection_with_groupby(bool async)
        {
            await base.Select_nested_collection_with_groupby(async);

        AssertSql(
"""
SELECT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`), `c`.`CustomerID`, `o1`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o0`.`OrderID`
    FROM `Orders` AS `o0`
    WHERE `c`.`CustomerID` = `o0`.`CustomerID`
    GROUP BY `o0`.`OrderID`
) AS `o1` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override async Task GroupBy_group_Distinct_Select_Distinct_aggregate(bool async)
        {
            await base.GroupBy_group_Distinct_Select_Distinct_aggregate(async);

            AssertSql(
                @"SELECT `o`.`CustomerID` AS `Key`, MAX(DISTINCT (`o`.`OrderDate`)) AS `Max`
FROM `Orders` AS `o`
GROUP BY `o`.`CustomerID`");
        }

        public override async Task GroupBy_Property_Select_Average(bool async)
        {
            await base.GroupBy_Property_Select_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Average_with_group_enumerable_projected(bool async)
        {
            await base.GroupBy_Property_Select_Average_with_group_enumerable_projected(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Count(bool async)
        {
            await base.GroupBy_Property_Select_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_LongCount(bool async)
        {
            await base.GroupBy_Property_Select_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Count_with_nulls(bool async)
        {
            await base.GroupBy_Property_Select_Count_with_nulls(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_LongCount_with_nulls(bool async)
        {
            await base.GroupBy_Property_Select_LongCount_with_nulls(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Max(bool async)
        {
            await base.GroupBy_Property_Select_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Min(bool async)
        {
            await base.GroupBy_Property_Select_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Sum(bool async)
        {
            await base.GroupBy_Property_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_Property_Select_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_Average(bool async)
        {
            await base.GroupBy_Property_Select_Key_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_Count(bool async)
        {
            await base.GroupBy_Property_Select_Key_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_LongCount(bool async)
        {
            await base.GroupBy_Property_Select_Key_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_Max(bool async)
        {
            await base.GroupBy_Property_Select_Key_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_Min(bool async)
        {
            await base.GroupBy_Property_Select_Key_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_Sum(bool async)
        {
            await base.GroupBy_Property_Select_Key_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_Property_Select_Key_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_Property_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_key_multiple_times_and_aggregate(bool async)
        {
            await base.GroupBy_Property_Select_key_multiple_times_and_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Key_with_constant(bool async)
        {
            await base.GroupBy_Property_Select_Key_with_constant(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_projecting_conditional_expression(bool async)
        {
            await base.GroupBy_aggregate_projecting_conditional_expression(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_projecting_conditional_expression_based_on_group_key(bool async)
        {
            await base.GroupBy_aggregate_projecting_conditional_expression_based_on_group_key(async);

            AssertSql();
        }

        public override async Task GroupBy_with_group_key_access_thru_navigation(bool async)
        {
            await base.GroupBy_with_group_key_access_thru_navigation(async);

            AssertSql();
        }

        public override async Task GroupBy_with_group_key_access_thru_nested_navigation(bool async)
        {
            await base.GroupBy_with_group_key_access_thru_nested_navigation(async);

            AssertSql();
        }

        public override async Task GroupBy_with_grouping_key_using_Like(bool async)
        {
            await base.GroupBy_with_grouping_key_using_Like(async);

            AssertSql();
        }

        public override async Task GroupBy_with_grouping_key_DateTime_Day(bool async)
        {
            await base.GroupBy_with_grouping_key_DateTime_Day(async);

            AssertSql();
        }

        public override async Task GroupBy_with_cast_inside_grouping_aggregate(bool async)
        {
            await base.GroupBy_with_cast_inside_grouping_aggregate(async);

            AssertSql();
        }

        public override async Task Group_by_with_arithmetic_operation_inside_aggregate(bool async)
        {
            await base.Group_by_with_arithmetic_operation_inside_aggregate(async);

            AssertSql();
        }

        public override async Task Group_by_with_projection_into_DTO(bool async)
        {
            await base.Group_by_with_projection_into_DTO(async);

            AssertSql();
        }

        public override async Task Where_select_function_groupby_followed_by_another_select_with_aggregates(bool async)
        {
            await base.Where_select_function_groupby_followed_by_another_select_with_aggregates(async);

            AssertSql();
        }

        public override async Task Group_by_column_project_constant(bool async)
        {
            await base.Group_by_column_project_constant(async);

            AssertSql();
        }

        public override async Task Key_plus_key_in_projection(bool async)
        {
            await base.Key_plus_key_in_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_with_aggregate_through_navigation_property(bool async)
        {
            await base.GroupBy_with_aggregate_through_navigation_property(async);

            AssertSql();
        }

        public override async Task GroupBy_with_aggregate_containing_complex_where(bool async)
        {
            await base.GroupBy_with_aggregate_containing_complex_where(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_Average(bool async)
        {
            await base.GroupBy_anonymous_Select_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_Count(bool async)
        {
            await base.GroupBy_anonymous_Select_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_LongCount(bool async)
        {
            await base.GroupBy_anonymous_Select_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_Max(bool async)
        {
            await base.GroupBy_anonymous_Select_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_Min(bool async)
        {
            await base.GroupBy_anonymous_Select_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_Sum(bool async)
        {
            await base.GroupBy_anonymous_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_Select_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_anonymous_Select_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_with_alias_Select_Key_Sum(bool async)
        {
            await base.GroupBy_anonymous_with_alias_Select_Key_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Average(bool async)
        {
            await base.GroupBy_Composite_Select_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Count(bool async)
        {
            await base.GroupBy_Composite_Select_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_LongCount(bool async)
        {
            await base.GroupBy_Composite_Select_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Max(bool async)
        {
            await base.GroupBy_Composite_Select_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Min(bool async)
        {
            await base.GroupBy_Composite_Select_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Sum(bool async)
        {
            await base.GroupBy_Composite_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_Composite_Select_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_Average(bool async)
        {
            await base.GroupBy_Composite_Select_Key_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_Count(bool async)
        {
            await base.GroupBy_Composite_Select_Key_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_LongCount(bool async)
        {
            await base.GroupBy_Composite_Select_Key_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_Max(bool async)
        {
            await base.GroupBy_Composite_Select_Key_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_Min(bool async)
        {
            await base.GroupBy_Composite_Select_Key_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_Sum(bool async)
        {
            await base.GroupBy_Composite_Select_Key_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Key_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_Composite_Select_Key_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_Composite_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Sum_Min_Key_flattened_Max_Avg(bool async)
        {
            await base.GroupBy_Composite_Select_Sum_Min_Key_flattened_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Dto_as_key_Select_Sum(bool async)
        {
            await base.GroupBy_Dto_as_key_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Dto_as_element_selector_Select_Sum(bool async)
        {
            await base.GroupBy_Dto_as_element_selector_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Dto_Sum_Min_Key_flattened_Max_Avg(bool async)
        {
            await base.GroupBy_Composite_Select_Dto_Sum_Min_Key_flattened_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Composite_Select_Sum_Min_part_Key_flattened_Max_Avg(bool async)
        {
            await base.GroupBy_Composite_Select_Sum_Min_part_Key_flattened_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Constant_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_Constant_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Constant_with_element_selector_Select_Sum(bool async)
        {
            await base.GroupBy_Constant_with_element_selector_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Constant_with_element_selector_Select_Sum2(bool async)
        {
            await base.GroupBy_Constant_with_element_selector_Select_Sum2(async);

            AssertSql();
        }

        public override async Task GroupBy_Constant_with_element_selector_Select_Sum3(bool async)
        {
            await base.GroupBy_Constant_with_element_selector_Select_Sum3(async);

            AssertSql();
        }

        public override async Task GroupBy_after_predicate_Constant_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_after_predicate_Constant_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Constant_with_element_selector_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_Constant_with_element_selector_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_param_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_param_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_param_with_element_selector_Select_Sum(bool async)
        {
            await base.GroupBy_param_with_element_selector_Select_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_param_with_element_selector_Select_Sum2(bool async)
        {
            await base.GroupBy_param_with_element_selector_Select_Sum2(async);

            AssertSql();
        }

        public override async Task GroupBy_param_with_element_selector_Select_Sum3(bool async)
        {
            await base.GroupBy_param_with_element_selector_Select_Sum3(async);

            AssertSql();
        }

        public override async Task GroupBy_param_with_element_selector_Select_Sum_Min_Key_Max_Avg(bool async)
        {
            await base.GroupBy_param_with_element_selector_Select_Sum_Min_Key_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_anonymous_key_type_mismatch_with_aggregate(bool async)
        {
            await base.GroupBy_anonymous_key_type_mismatch_with_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_based_on_renamed_property_simple(bool async)
        {
            await base.GroupBy_based_on_renamed_property_simple(async);

            AssertSql();
        }

        public override async Task GroupBy_based_on_renamed_property_complex(bool async)
        {
            await base.GroupBy_based_on_renamed_property_complex(async);

            AssertSql();
        }

        public override async Task Join_groupby_anonymous_orderby_anonymous_projection(bool async)
        {
            await base.Join_groupby_anonymous_orderby_anonymous_projection(async);

            AssertSql();
        }

        public override async Task Odata_groupby_empty_key(bool async)
        {
            await base.Odata_groupby_empty_key(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_Average(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_Count(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_LongCount(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_Max(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_Min(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_Sum(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_scalar_element_selector_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_Property_scalar_element_selector_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_Average(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_Count(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_LongCount(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_Max(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_Min(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_Sum(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_anonymous_element_selector_Sum_Min_Max_Avg(bool async)
        {
            await base.GroupBy_Property_anonymous_element_selector_Sum_Min_Max_Avg(async);

            AssertSql();
        }

        public override async Task GroupBy_element_selector_complex_aggregate(bool async)
        {
            await base.GroupBy_element_selector_complex_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_element_selector_complex_aggregate2(bool async)
        {
            await base.GroupBy_element_selector_complex_aggregate2(async);

            AssertSql();
        }

        public override async Task GroupBy_element_selector_complex_aggregate3(bool async)
        {
            await base.GroupBy_element_selector_complex_aggregate3(async);

            AssertSql();
        }

        public override async Task GroupBy_element_selector_complex_aggregate4(bool async)
        {
            await base.GroupBy_element_selector_complex_aggregate4(async);

            AssertSql();
        }

        public override async Task Element_selector_with_case_block_repeated_inside_another_case_block_in_projection(bool async)
        {
            await base.Element_selector_with_case_block_repeated_inside_another_case_block_in_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_conditional_properties(bool async)
        {
            await base.GroupBy_conditional_properties(async);

            AssertSql();
        }

        public override async Task GroupBy_empty_key_Aggregate(bool async)
        {
            await base.GroupBy_empty_key_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_empty_key_Aggregate_Key(bool async)
        {
            await base.GroupBy_empty_key_Aggregate_Key(async);

            AssertSql();
        }

        public override async Task OrderBy_GroupBy_Aggregate(bool async)
        {
            await base.OrderBy_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task OrderBy_Skip_GroupBy_Aggregate(bool async)
        {
            await base.OrderBy_Skip_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task OrderBy_Take_GroupBy_Aggregate(bool async)
        {
            await base.OrderBy_Take_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task OrderBy_Skip_Take_GroupBy_Aggregate(bool async)
        {
            await base.OrderBy_Skip_Take_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task Distinct_GroupBy_Aggregate(bool async)
        {
            await base.Distinct_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task Anonymous_projection_Distinct_GroupBy_Aggregate(bool async)
        {
            await base.Anonymous_projection_Distinct_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task SelectMany_GroupBy_Aggregate(bool async)
        {
            await base.SelectMany_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate(bool async)
        {
            await base.Join_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_required_navigation_member_Aggregate(bool async)
        {
            await base.GroupBy_required_navigation_member_Aggregate(async);

            AssertSql();
        }

        public override async Task Join_complex_GroupBy_Aggregate(bool async)
        {
            await base.Join_complex_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupJoin_GroupBy_Aggregate(bool async)
        {
            await base.GroupJoin_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupJoin_GroupBy_Aggregate_2(bool async)
        {
            await base.GroupJoin_GroupBy_Aggregate_2(async);

            AssertSql();
        }

        public override async Task GroupJoin_GroupBy_Aggregate_3(bool async)
        {
            await base.GroupJoin_GroupBy_Aggregate_3(async);

            AssertSql();
        }

        public override async Task GroupJoin_GroupBy_Aggregate_4(bool async)
        {
            await base.GroupJoin_GroupBy_Aggregate_4(async);

            AssertSql();
        }

        public override async Task GroupJoin_GroupBy_Aggregate_5(bool async)
        {
            await base.GroupJoin_GroupBy_Aggregate_5(async);

            AssertSql();
        }

        public override async Task GroupBy_optional_navigation_member_Aggregate(bool async)
        {
            await base.GroupBy_optional_navigation_member_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupJoin_complex_GroupBy_Aggregate(bool async)
        {
            await base.GroupJoin_complex_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task Self_join_GroupBy_Aggregate(bool async)
        {
            await base.Self_join_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_multi_navigation_members_Aggregate(bool async)
        {
            await base.GroupBy_multi_navigation_members_Aggregate(async);

            AssertSql();
        }

        public override async Task Union_simple_groupby(bool async)
        {
            await base.Union_simple_groupby(async);

            AssertSql();
        }

        public override async Task Select_anonymous_GroupBy_Aggregate(bool async)
        {
            await base.Select_anonymous_GroupBy_Aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_principal_key_property_optimization(bool async)
        {
            await base.GroupBy_principal_key_property_optimization(async);

            AssertSql();
        }

        public override async Task GroupBy_after_anonymous_projection_and_distinct_followed_by_another_anonymous_projection(bool async)
        {
            await base.GroupBy_after_anonymous_projection_and_distinct_followed_by_another_anonymous_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_complex_key_aggregate(bool async)
        {
            await base.GroupBy_complex_key_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_complex_key_aggregate_2(bool async)
        {
            await base.GroupBy_complex_key_aggregate_2(async);

            AssertSql();
        }

        public override async Task Select_collection_of_scalar_before_GroupBy_aggregate(bool async)
        {
            await base.Select_collection_of_scalar_before_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_OrderBy_key(bool async)
        {
            await base.GroupBy_OrderBy_key(async);

            AssertSql();
        }

        public override async Task GroupBy_OrderBy_count(bool async)
        {
            await base.GroupBy_OrderBy_count(async);

            AssertSql();
        }

        public override async Task GroupBy_OrderBy_count_Select_sum(bool async)
        {
            await base.GroupBy_OrderBy_count_Select_sum(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_Contains(bool async)
        {
            await base.GroupBy_aggregate_Contains(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_Pushdown(bool async)
        {
            await base.GroupBy_aggregate_Pushdown(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_using_grouping_key_Pushdown(bool async)
        {
            await base.GroupBy_aggregate_using_grouping_key_Pushdown(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_Pushdown_followed_by_projecting_Length(bool async)
        {
            await base.GroupBy_aggregate_Pushdown_followed_by_projecting_Length(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_Pushdown_followed_by_projecting_constant(bool async)
        {
            await base.GroupBy_aggregate_Pushdown_followed_by_projecting_constant(async);

            AssertSql();
        }

        public override async Task GroupBy_filter_key(bool async)
        {
            await base.GroupBy_filter_key(async);

            AssertSql();
        }

        public override async Task GroupBy_filter_count(bool async)
        {
            await base.GroupBy_filter_count(async);

            AssertSql();
        }

        public override async Task GroupBy_count_filter(bool async)
        {
            await base.GroupBy_count_filter(async);

            AssertSql();
        }

        public override async Task GroupBy_filter_count_OrderBy_count_Select_sum(bool async)
        {
            await base.GroupBy_filter_count_OrderBy_count_Select_sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Aggregate_Join(bool async)
        {
            await base.GroupBy_Aggregate_Join(async);

            AssertSql();
        }

        public override async Task GroupBy_Aggregate_Join_converted_from_SelectMany(bool async)
        {
            await base.GroupBy_Aggregate_Join_converted_from_SelectMany(async);

            AssertSql();
        }

        public override async Task GroupBy_Aggregate_LeftJoin_converted_from_SelectMany(bool async)
        {
            await base.GroupBy_Aggregate_LeftJoin_converted_from_SelectMany(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_multijoins(bool async)
        {
            await base.Join_GroupBy_Aggregate_multijoins(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_single_join(bool async)
        {
            await base.Join_GroupBy_Aggregate_single_join(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_with_another_join(bool async)
        {
            await base.Join_GroupBy_Aggregate_with_another_join(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_distinct_single_join(bool async)
        {
            await base.Join_GroupBy_Aggregate_distinct_single_join(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_with_left_join(bool async)
        {
            await base.Join_GroupBy_Aggregate_with_left_join(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_in_subquery(bool async)
        {
            await base.Join_GroupBy_Aggregate_in_subquery(async);

            AssertSql();
        }

        public override async Task Join_GroupBy_Aggregate_on_key(bool async)
        {
            await base.Join_GroupBy_Aggregate_on_key(async);

            AssertSql();
        }

        public override async Task GroupBy_with_result_selector(bool async)
        {
            await base.GroupBy_with_result_selector(async);

            AssertSql();
        }

        public override async Task GroupBy_Sum_constant(bool async)
        {
            await base.GroupBy_Sum_constant(async);

            AssertSql();
        }

        public override async Task GroupBy_Sum_constant_cast(bool async)
        {
            await base.GroupBy_Sum_constant_cast(async);

            AssertSql();
        }

        public override async Task Distinct_GroupBy_OrderBy_key(bool async)
        {
            await base.Distinct_GroupBy_OrderBy_key(async);

            AssertSql();
        }

        public override async Task Select_uncorrelated_collection_with_groupby_works(bool async)
        {
            await base.Select_uncorrelated_collection_with_groupby_works(async);

            AssertSql();
        }

        public override async Task Select_uncorrelated_collection_with_groupby_multiple_collections_work(bool async)
        {
            await base.Select_uncorrelated_collection_with_groupby_multiple_collections_work(async);

            AssertSql();
        }

        public override async Task Select_GroupBy_All(bool async)
        {
            await base.Select_GroupBy_All(async);

            AssertSql();
        }

        public override async Task GroupBy_multiple_Count_with_predicate(bool async)
        {
            await base.GroupBy_multiple_Count_with_predicate(async);

            AssertSql();
        }

        public override async Task GroupBy_multiple_Sum_with_conditional_projection(bool async)
        {
            await base.GroupBy_multiple_Sum_with_conditional_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_Key_as_part_of_element_selector(bool async)
        {
            await base.GroupBy_Key_as_part_of_element_selector(async);

            AssertSql();
        }

        public override async Task GroupBy_composite_Key_as_part_of_element_selector(bool async)
        {
            await base.GroupBy_composite_Key_as_part_of_element_selector(async);

            AssertSql();
        }

        public override async Task GroupBy_with_order_by_skip_and_another_order_by(bool async)
        {
            await base.GroupBy_with_order_by_skip_and_another_order_by(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_Count_with_predicate(bool async)
        {
            await base.GroupBy_Property_Select_Count_with_predicate(async);

            AssertSql();
        }

        public override async Task GroupBy_Property_Select_LongCount_with_predicate(bool async)
        {
            await base.GroupBy_Property_Select_LongCount_with_predicate(async);

            AssertSql();
        }

        public override async Task GroupBy_orderby_projection_with_coalesce_operation(bool async)
        {
            await base.GroupBy_orderby_projection_with_coalesce_operation(async);

            AssertSql();
        }

        public override async Task GroupBy_let_orderby_projection_with_coalesce_operation(bool async)
        {
            await base.GroupBy_let_orderby_projection_with_coalesce_operation(async);

            AssertSql();
        }

        public override async Task GroupBy_Min_Where_optional_relationship(bool async)
        {
            await base.GroupBy_Min_Where_optional_relationship(async);

            AssertSql();
        }

        public override async Task GroupBy_Min_Where_optional_relationship_2(bool async)
        {
            await base.GroupBy_Min_Where_optional_relationship_2(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_over_a_subquery(bool async)
        {
            await base.GroupBy_aggregate_over_a_subquery(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_join_with_grouping_key(bool async)
        {
            await base.GroupBy_aggregate_join_with_grouping_key(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_join_with_group_result(bool async)
        {
            await base.GroupBy_aggregate_join_with_group_result(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_from_right_side_of_join(bool async)
        {
            await base.GroupBy_aggregate_from_right_side_of_join(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_join_another_GroupBy_aggregate(bool async)
        {
            await base.GroupBy_aggregate_join_another_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_after_skip_0_take_0(bool async)
        {
            await base.GroupBy_aggregate_after_skip_0_take_0(async);

            AssertSql();
        }

        public override async Task GroupBy_skip_0_take_0_aggregate(bool async)
        {
            await base.GroupBy_skip_0_take_0_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_followed_another_GroupBy_aggregate(bool async)
        {
            await base.GroupBy_aggregate_followed_another_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_SelectMany(bool async)
        {
            await base.GroupBy_aggregate_SelectMany(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_without_selectMany_selecting_first(bool async)
        {
            await base.GroupBy_aggregate_without_selectMany_selecting_first(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_left_join_GroupBy_aggregate_left_join(bool async)
        {
            await base.GroupBy_aggregate_left_join_GroupBy_aggregate_left_join(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Average(bool async)
        {
            await base.GroupBy_Where_Average(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Count(bool async)
        {
            await base.GroupBy_Where_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_LongCount(bool async)
        {
            await base.GroupBy_Where_LongCount(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Max(bool async)
        {
            await base.GroupBy_Where_Max(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Min(bool async)
        {
            await base.GroupBy_Where_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Sum(bool async)
        {
            await base.GroupBy_Where_Sum(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Count_with_predicate(bool async)
        {
            await base.GroupBy_Where_Count_with_predicate(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Where_Count(bool async)
        {
            await base.GroupBy_Where_Where_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Select_Where_Count(bool async)
        {
            await base.GroupBy_Where_Select_Where_Count(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_Select_Where_Select_Min(bool async)
        {
            await base.GroupBy_Where_Select_Where_Select_Min(async);

            AssertSql();
        }

        public override async Task GroupBy_multiple_Sum_with_Select_conditional_projection(bool async)
        {
            await base.GroupBy_multiple_Sum_with_Select_conditional_projection(async);

            AssertSql();
        }

        public override async Task LongCount_after_GroupBy_aggregate(bool async)
        {
            await base.LongCount_after_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_Select_Distinct_aggregate(bool async)
        {
            await base.GroupBy_Select_Distinct_aggregate(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_entity(bool async)
        {
            await base.Final_GroupBy_property_entity(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_entity(bool async)
        {
            await base.Final_GroupBy_entity(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_entity_non_nullable(bool async)
        {
            await base.Final_GroupBy_property_entity_non_nullable(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_anonymous_type(bool async)
        {
            await base.Final_GroupBy_property_anonymous_type(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_multiple_properties_entity(bool async)
        {
            await base.Final_GroupBy_multiple_properties_entity(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_complex_key_entity(bool async)
        {
            await base.Final_GroupBy_complex_key_entity(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_nominal_type_entity(bool async)
        {
            await base.Final_GroupBy_nominal_type_entity(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_anonymous_type_element_selector(bool async)
        {
            await base.Final_GroupBy_property_anonymous_type_element_selector(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_entity_Include_collection(bool async)
        {
            await base.Final_GroupBy_property_entity_Include_collection(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_entity_projecting_collection(bool async)
        {
            await base.Final_GroupBy_property_entity_projecting_collection(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_entity_projecting_collection_composed(bool async)
        {
            await base.Final_GroupBy_property_entity_projecting_collection_composed(async);

            AssertSql();
        }

        public override async Task Final_GroupBy_property_entity_projecting_collection_and_single_result(bool async)
        {
            await base.Final_GroupBy_property_entity_projecting_collection_and_single_result(async);

            AssertSql();
        }

        public override async Task GroupBy_Where_with_grouping_result(bool async)
        {
            await base.GroupBy_Where_with_grouping_result(async);

            AssertSql();
        }

        public override async Task GroupBy_OrderBy_with_grouping_result(bool async)
        {
            await base.GroupBy_OrderBy_with_grouping_result(async);

            AssertSql();
        }

        public override async Task GroupBy_SelectMany(bool async)
        {
            await base.GroupBy_SelectMany(async);

            AssertSql();
        }

        public override async Task OrderBy_GroupBy_SelectMany(bool async)
        {
            await base.OrderBy_GroupBy_SelectMany(async);

            AssertSql();
        }

        public override async Task OrderBy_GroupBy_SelectMany_shadow(bool async)
        {
            await base.OrderBy_GroupBy_SelectMany_shadow(async);

            AssertSql();
        }

        public override async Task GroupBy_with_orderby_take_skip_distinct_followed_by_group_key_projection(bool async)
        {
            await base.GroupBy_with_orderby_take_skip_distinct_followed_by_group_key_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_Distinct(bool async)
        {
            await base.GroupBy_Distinct(async);

            AssertSql();
        }

        public override async Task GroupBy_complex_key_without_aggregate(bool async)
        {
            await base.GroupBy_complex_key_without_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_selecting_grouping_key_list(bool async)
        {
            await base.GroupBy_selecting_grouping_key_list(async);

            AssertSql();
        }

        public override async Task Select_GroupBy_SelectMany(bool async)
        {
            await base.Select_GroupBy_SelectMany(async);

            AssertSql();
        }

        public override async Task GroupBy_Shadow(bool async)
        {
            await base.GroupBy_Shadow(async);

            AssertSql();
        }

        public override async Task GroupBy_Shadow2(bool async)
        {
            await base.GroupBy_Shadow2(async);

            AssertSql();
        }

        public override async Task GroupBy_Shadow3(bool async)
        {
            await base.GroupBy_Shadow3(async);

            AssertSql();
        }

        public override async Task GroupBy_select_grouping_list(bool async)
        {
            await base.GroupBy_select_grouping_list(async);

            AssertSql();
        }

        public override async Task GroupBy_select_grouping_array(bool async)
        {
            await base.GroupBy_select_grouping_array(async);

            AssertSql();
        }

        public override async Task GroupBy_select_grouping_composed_list(bool async)
        {
            await base.GroupBy_select_grouping_composed_list(async);

            AssertSql();
        }

        public override async Task GroupBy_select_grouping_composed_list_2(bool async)
        {
            await base.GroupBy_select_grouping_composed_list_2(async);

            AssertSql();
        }

        public override async Task GroupBy_with_group_key_being_navigation(bool async)
        {
            await base.GroupBy_with_group_key_being_navigation(async);

            AssertSql();
        }

        public override async Task GroupBy_with_group_key_being_nested_navigation(bool async)
        {
            await base.GroupBy_with_group_key_being_nested_navigation(async);

            AssertSql();
        }

        public override async Task GroupBy_with_group_key_being_navigation_with_entity_key_projection(bool async)
        {
            await base.GroupBy_with_group_key_being_navigation_with_entity_key_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_with_group_key_being_navigation_with_complex_projection(bool async)
        {
            await base.GroupBy_with_group_key_being_navigation_with_complex_projection(async);

            AssertSql();
        }

        public override async Task Count_after_GroupBy_aggregate(bool async)
        {
            await base.Count_after_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task MinMax_after_GroupBy_aggregate(bool async)
        {
            await base.MinMax_after_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task All_after_GroupBy_aggregate(bool async)
        {
            await base.All_after_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task All_after_GroupBy_aggregate2(bool async)
        {
            await base.All_after_GroupBy_aggregate2(async);

            AssertSql();
        }

        public override async Task Any_after_GroupBy_aggregate(bool async)
        {
            await base.Any_after_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task Count_after_GroupBy_without_aggregate(bool async)
        {
            await base.Count_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task Count_with_predicate_after_GroupBy_without_aggregate(bool async)
        {
            await base.Count_with_predicate_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task LongCount_after_GroupBy_without_aggregate(bool async)
        {
            await base.LongCount_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task LongCount_with_predicate_after_GroupBy_without_aggregate(bool async)
        {
            await base.LongCount_with_predicate_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task Any_after_GroupBy_without_aggregate(bool async)
        {
            await base.Any_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task Any_with_predicate_after_GroupBy_without_aggregate(bool async)
        {
            await base.Any_with_predicate_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task All_with_predicate_after_GroupBy_without_aggregate(bool async)
        {
            await base.All_with_predicate_after_GroupBy_without_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_followed_by_another_GroupBy_aggregate(bool async)
        {
            await base.GroupBy_aggregate_followed_by_another_GroupBy_aggregate(async);

            AssertSql();
        }

        public override async Task GroupBy_nominal_type_count(bool async)
        {
            await base.GroupBy_nominal_type_count(async);

            AssertSql();
        }

        public override async Task Complex_query_with_groupBy_in_subquery4(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery4(async);

            AssertSql();
        }

        public override async Task Complex_query_with_group_by_in_subquery5(bool async)
        {
            await base.Complex_query_with_group_by_in_subquery5(async);

            AssertSql();
        }

        public override async Task GroupBy_scalar_subquery(bool async)
        {
            await base.GroupBy_scalar_subquery(async);

            AssertSql();
        }

        public override async Task GroupBy_scalar_aggregate_in_set_operation(bool async)
        {
            await base.GroupBy_scalar_aggregate_in_set_operation(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_from_multiple_query_in_same_projection(bool async)
        {
            await base.GroupBy_aggregate_from_multiple_query_in_same_projection(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_from_multiple_query_in_same_projection_2(bool async)
        {
            await base.GroupBy_aggregate_from_multiple_query_in_same_projection_2(async);

            AssertSql();
        }

        public override async Task GroupBy_aggregate_from_multiple_query_in_same_projection_3(bool async)
        {
            await base.GroupBy_aggregate_from_multiple_query_in_same_projection_3(async);

            AssertSql();
        }

        public override async Task Select_uncorrelated_collection_with_groupby_when_outer_is_distinct(bool async)
        {
            await base.Select_uncorrelated_collection_with_groupby_when_outer_is_distinct(async);

            AssertSql();
        }

        public override async Task Select_correlated_collection_after_GroupBy_aggregate_when_identifier_does_not_change(bool async)
        {
            await base.Select_correlated_collection_after_GroupBy_aggregate_when_identifier_does_not_change(async);

            AssertSql();
        }

        public override async Task Select_correlated_collection_after_GroupBy_aggregate_when_identifier_changes(bool async)
        {
            await base.Select_correlated_collection_after_GroupBy_aggregate_when_identifier_changes(async);

            AssertSql();
        }

        public override async Task Select_correlated_collection_after_GroupBy_aggregate_when_identifier_changes_to_complex(bool async)
        {
            await base.Select_correlated_collection_after_GroupBy_aggregate_when_identifier_changes_to_complex(async);

            AssertSql();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task GroupBy_Count_in_projection(bool async)
        {
            return base.GroupBy_Count_in_projection(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql", "0.0.0-mariadb")]
        public override Task GroupBy_group_Where_Select_Distinct_aggregate(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/898.
            return base.GroupBy_group_Where_Select_Distinct_aggregate(async);
        }

        [SupportedServerVersionCondition("8.0.0-mysql", "0.0.0-mariadb")] // Is an issue issue in MySQL 5.7.34, but not in 8.0.25.
        public override Task GroupBy_constant_with_where_on_grouping_with_aggregate_operators(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/980.
            return base.GroupBy_constant_with_where_on_grouping_with_aggregate_operators(async);
        }

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
