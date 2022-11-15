using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public abstract class FindMySqlTest : FindTestBase<FindMySqlTest.FindMySqlFixture>
{
    protected FindMySqlTest(FindMySqlFixture fixture)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
    }

    public class FindMySqlTestSet : FindMySqlTest
    {
        public FindMySqlTestSet(FindMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override TestFinder Finder { get; } = new FindViaSetFinder();
    }

    public class FindMySqlTestContext : FindMySqlTest
    {
        public FindMySqlTestContext(FindMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override TestFinder Finder { get; } = new FindViaContextFinder();
    }

    public class FindMySqlTestNonGeneric : FindMySqlTest
    {
        public FindMySqlTestNonGeneric(FindMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override TestFinder Finder { get; } = new FindViaNonGenericContextFinder();
    }

    public class FindMySqlFixture : FindFixtureBase
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
