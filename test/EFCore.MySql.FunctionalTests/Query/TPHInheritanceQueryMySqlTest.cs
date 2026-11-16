using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlTest : TPHInheritanceQueryTestBase<TPHInheritanceQueryMySqlFixture>
{
    public TPHInheritanceQueryMySqlTest(TPHInheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }
}
