using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GroupByQueryMySqlTest : GroupByQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public GroupByQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Sum_Min_Key_Max_Avg()
        {
            base.GroupBy_Composite_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void OrderBy_Skip_GroupBy_Aggregate()
        {
            base.OrderBy_Skip_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Join_GroupBy_Aggregate()
        {
            base.Join_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Join_GroupBy_entity_ToList()
        {
            base.Join_GroupBy_entity_ToList();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_anonymous_element_selector_Sum_Min_Max_Avg()
        {
            base.GroupBy_Property_anonymous_element_selector_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Average()
        {
            base.GroupBy_Composite_Select_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_Select_Average()
        {
            base.GroupBy_Property_Select_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_Select_Sum_Min_Max_Avg()
        {
            base.GroupBy_Property_Select_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_Select_Sum_Min_Key_Max_Avg()
        {
            base.GroupBy_Property_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupJoin_GroupBy_Aggregate()
        {
            base.GroupJoin_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_anonymous_element_selector_Average()
        {
            base.GroupBy_Property_anonymous_element_selector_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupJoin_complex_GroupBy_Aggregate()
        {
            base.GroupJoin_complex_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Sum_Min_Max_Avg()
        {
            base.GroupBy_Composite_Select_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Sum_Min_Key_flattened_Max_Avg()
        {
            base.GroupBy_Composite_Select_Sum_Min_Key_flattened_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_anonymous_Select_Average()
        {
            base.GroupBy_anonymous_Select_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Self_join_GroupBy_Aggregate()
        {
            base.Self_join_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Join_complex_GroupBy_Aggregate()
        {
            base.Join_complex_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_anonymous_Select_Sum_Min_Max_Avg()
        {
            base.GroupBy_anonymous_Select_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_param_Select_Sum_Min_Key_Max_Avg()
        {
            base.GroupBy_param_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_anonymous_GroupBy_Aggregate()
        {
            base.Select_anonymous_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Dto_Sum_Min_Key_flattened_Max_Avg()
        {
            base.GroupBy_Composite_Select_Dto_Sum_Min_Key_flattened_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_with_result_selector()
        {
            base.GroupBy_with_result_selector();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Constant_Select_Sum_Min_Key_Max_Avg()
        {
            base.GroupBy_Constant_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_Select_Key_Sum_Min_Max_Avg()
        {
            base.GroupBy_Property_Select_Key_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupJoin_GroupBy_Aggregate_5()
        {
            base.GroupJoin_GroupBy_Aggregate_5();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_scalar_element_selector_Average()
        {
            base.GroupBy_Property_scalar_element_selector_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Key_Average()
        {
            base.GroupBy_Composite_Select_Key_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupJoin_GroupBy_Aggregate_3()
        {
            base.GroupJoin_GroupBy_Aggregate_3();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_after_predicate_Constant_Select_Sum_Min_Key_Max_Avg()
        {
            base.GroupBy_after_predicate_Constant_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Sum_Min_part_Key_flattened_Max_Avg()
        {
            base.GroupBy_Composite_Select_Sum_Min_part_Key_flattened_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_Select_Key_Average()
        {
            base.GroupBy_Property_Select_Key_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Property_scalar_element_selector_Sum_Min_Max_Avg()
        {
            base.GroupBy_Property_scalar_element_selector_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void GroupBy_Composite_Select_Key_Sum_Min_Max_Avg()
        {
            base.GroupBy_Composite_Select_Key_Sum_Min_Max_Avg();
        }
    }
}
