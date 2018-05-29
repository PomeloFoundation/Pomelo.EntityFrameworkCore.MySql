using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsWeakQueryMySqlTest : ComplexNavigationsWeakQueryTestBase<ComplexNavigationsWeakQueryMySqlFixture>
    {
        public ComplexNavigationsWeakQueryMySqlTest(ComplexNavigationsWeakQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
        }
    }
}
