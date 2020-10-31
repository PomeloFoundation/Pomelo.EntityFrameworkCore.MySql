using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ManyToManyNoTrackingQueryMySqlTest
        : ManyToManyNoTrackingQueryRelationalTestBase<ManyToManyQueryMySqlFixture>
    {
        public ManyToManyNoTrackingQueryMySqlTest(ManyToManyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // TODO: CanExecuteQueryString
        // protected override bool CanExecuteQueryString => true;

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
