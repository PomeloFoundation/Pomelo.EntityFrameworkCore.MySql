using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlTest : TPHInheritanceQueryTestBase<TPHInheritanceQueryMySqlFixture>
{
    public TPHInheritanceQueryMySqlTest(TPHInheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }
}
