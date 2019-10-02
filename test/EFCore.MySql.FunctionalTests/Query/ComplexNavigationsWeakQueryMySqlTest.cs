using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsWeakQueryMySqlTest : ComplexNavigationsWeakQueryTestBase<ComplexNavigationsWeakQueryMySqlFixture>
    {
        public ComplexNavigationsWeakQueryMySqlTest(ComplexNavigationsWeakQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsMySqlSupportVersionString)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(bool isAsync)
        {
            return base.SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(isAsync);
        }
    }
}
