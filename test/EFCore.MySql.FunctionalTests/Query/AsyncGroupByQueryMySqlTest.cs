using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncGroupByQueryMySqlTest : AsyncGroupByQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public AsyncGroupByQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Sum_Min_Key_Max_Avg()
        {
            return base.GroupBy_Composite_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task OrderBy_Skip_GroupBy_Aggregate()
        {
            return base.OrderBy_Skip_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Join_GroupBy_Aggregate()
        {
            return base.Join_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_anonymous_element_selector_Sum_Min_Max_Avg()
        {
            return base.GroupBy_Property_anonymous_element_selector_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Average()
        {
            return base.GroupBy_Composite_Select_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_Select_Average()
        {
            return base.GroupBy_Property_Select_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_Select_Sum_Min_Max_Avg()
        {
            return base.GroupBy_Property_Select_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_Select_Sum_Min_Key_Max_Avg()
        {
            return base.GroupBy_Property_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupJoin_GroupBy_Aggregate()
        {
            return base.GroupJoin_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_anonymous_element_selector_Average()
        {
            return base.GroupBy_Property_anonymous_element_selector_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupJoin_complex_GroupBy_Aggregate()
        {
            return base.GroupJoin_complex_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Sum_Min_Max_Avg()
        {
            return base.GroupBy_Composite_Select_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Sum_Min_Key_flattened_Max_Avg()
        {
            return base.GroupBy_Composite_Select_Sum_Min_Key_flattened_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_anonymous_Select_Average()
        {
            return base.GroupBy_anonymous_Select_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Self_join_GroupBy_Aggregate()
        {
            return base.Self_join_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Join_complex_GroupBy_Aggregate()
        {
            return base.Join_complex_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_anonymous_Select_Sum_Min_Max_Avg()
        {
            return base.GroupBy_anonymous_Select_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_param_Select_Sum_Min_Key_Max_Avg()
        {
            return base.GroupBy_param_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Select_anonymous_GroupBy_Aggregate()
        {
            return base.Select_anonymous_GroupBy_Aggregate();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Dto_Sum_Min_Key_flattened_Max_Avg()
        {
            return base.GroupBy_Composite_Select_Dto_Sum_Min_Key_flattened_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_with_result_selector()
        {
            return base.GroupBy_with_result_selector();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Constant_Select_Sum_Min_Key_Max_Avg()
        {
            return base.GroupBy_Constant_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_Select_Key_Sum_Min_Max_Avg()
        {
            return base.GroupBy_Property_Select_Key_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupJoin_GroupBy_Aggregate_5()
        {
            return base.GroupJoin_GroupBy_Aggregate_5();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_scalar_element_selector_Average()
        {
            return base.GroupBy_Property_scalar_element_selector_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Key_Average()
        {
            return base.GroupBy_Composite_Select_Key_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupJoin_GroupBy_Aggregate_3()
        {
            return base.GroupJoin_GroupBy_Aggregate_3();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_after_predicate_Constant_Select_Sum_Min_Key_Max_Avg()
        {
            return base.GroupBy_after_predicate_Constant_Select_Sum_Min_Key_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Sum_Min_part_Key_flattened_Max_Avg()
        {
            return base.GroupBy_Composite_Select_Sum_Min_part_Key_flattened_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_Select_Key_Average()
        {
            return base.GroupBy_Property_Select_Key_Average();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Property_scalar_element_selector_Sum_Min_Max_Avg()
        {
            return base.GroupBy_Property_scalar_element_selector_Sum_Min_Max_Avg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task GroupBy_Composite_Select_Key_Sum_Min_Max_Avg()
        {
            return base.GroupBy_Composite_Select_Key_Sum_Min_Max_Avg();
        }
    }
}
