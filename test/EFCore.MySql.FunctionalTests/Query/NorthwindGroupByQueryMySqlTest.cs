using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindGroupByQueryMySqlTest : NorthwindGroupByQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public NorthwindGroupByQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override Task GroupBy_Property_Select_Count_with_predicate(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(
                () => base.GroupBy_Property_Select_Count_with_predicate(async));
        }

        public override Task GroupBy_Property_Select_LongCount_with_predicate(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(
                () => base.GroupBy_Property_Select_LongCount_with_predicate(async));
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
