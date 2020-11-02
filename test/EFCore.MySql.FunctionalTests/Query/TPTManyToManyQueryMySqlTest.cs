using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTManyToManyQueryMySqlTest : TPTManyToManyQueryRelationalTestBase<TPTManyToManyQueryMySqlFixture>
    {
        public TPTManyToManyQueryMySqlTest(TPTManyToManyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // TODO: CanExecuteQueryString
        // protected override bool CanExecuteQueryString => true;

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_split(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_skip_split(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_skip_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_take_split(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_take_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_where_then_include_skip_navigation_order_by_skip_take_split(bool async)
        {
            return base.Filtered_include_skip_navigation_where_then_include_skip_navigation_order_by_skip_take_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_skip_take_split(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_skip_take_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where_split(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filter_include_on_skip_navigation_combined_with_filtered_then_includes_split(bool async)
        {
            return base.Filter_include_on_skip_navigation_combined_with_filtered_then_includes_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_then_include_skip_navigation_order_by_skip_take_split(bool async)
        {
            return base.Filtered_then_include_skip_navigation_order_by_skip_take_split(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Skip_navigation_order_by_first_or_default(bool async)
        {
            return base.Skip_navigation_order_by_first_or_default(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Skip_navigation_order_by_last_or_default(bool async)
        {
            return base.Skip_navigation_order_by_last_or_default(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Skip_navigation_order_by_reverse_first_or_default(bool async)
        {
            return base.Skip_navigation_order_by_reverse_first_or_default(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Select_many_over_skip_navigation_order_by_skip(bool async)
        {
            return base.Select_many_over_skip_navigation_order_by_skip(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Select_many_over_skip_navigation_order_by_take(bool async)
        {
            return base.Select_many_over_skip_navigation_order_by_take(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Select_many_over_skip_navigation_order_by_skip_take(bool async)
        {
            return base.Select_many_over_skip_navigation_order_by_skip_take(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Select_skip_navigation_first_or_default(bool async)
        {
            return base.Select_skip_navigation_first_or_default(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_skip(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_skip(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_take(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_take(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_skip_take(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_skip_take(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_then_include_skip_navigation_order_by_skip_take(bool async)
        {
            return base.Filtered_then_include_skip_navigation_order_by_skip_take(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filtered_include_skip_navigation_where_then_include_skip_navigation_order_by_skip_take(bool async)
        {
            return base.Filtered_include_skip_navigation_where_then_include_skip_navigation_order_by_skip_take(async);
        }

        [SupportedServerVersionCondition(ServerVersion.WindowFunctionsSupportKey)]
        public override Task Filter_include_on_skip_navigation_combined_with_filtered_then_includes(bool async)
        {
            return base.Filter_include_on_skip_navigation_combined_with_filtered_then_includes(async);
        }

        [SupportedServerVersionCondition(ServerVersion.OuterApplySupportKey)]
        public override Task Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where(bool async)
        {
            return base.Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where(async);
        }

        [SupportedServerVersionCondition(ServerVersion.OuterApplySupportKey)]
        public override Task Skip_navigation_order_by_single_or_default(bool async)
        {
            return base.Skip_navigation_order_by_single_or_default(async);
        }
    }
}
