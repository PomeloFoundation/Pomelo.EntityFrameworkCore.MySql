using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
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

        public override Task GroupBy_Property_Select_Count_with_predicate(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(
                () => base.GroupBy_Property_Select_Count_with_predicate(isAsync));
        }

        public override Task GroupBy_Property_Select_LongCount_with_predicate(bool isAsync)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(
                () => base.GroupBy_Property_Select_LongCount_with_predicate(isAsync));
        }
    }
}
