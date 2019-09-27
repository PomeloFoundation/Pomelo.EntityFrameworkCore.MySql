using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class QueryFilterFuncletizationMySqlTest
        : QueryFilterFuncletizationTestBase<QueryFilterFuncletizationMySqlTest.QueryFilterFuncletizationMySqlFixture>
    {
        public QueryFilterFuncletizationMySqlTest(
            QueryFilterFuncletizationMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class QueryFilterFuncletizationMySqlFixture : QueryFilterFuncletizationRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
