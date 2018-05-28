using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class ComplexNavigationsWeakQueryMySqlTest : ComplexNavigationsWeakQueryTestBase<ComplexNavigationsWeakQueryMySqlFixture>
    {
        public ComplexNavigationsWeakQueryMySqlTest(ComplexNavigationsWeakQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
        }
    }
}
