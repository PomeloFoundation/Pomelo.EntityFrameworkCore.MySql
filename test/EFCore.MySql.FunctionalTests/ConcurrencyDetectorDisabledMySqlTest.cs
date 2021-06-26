using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ConcurrencyDetectorDisabledMySqlTest : ConcurrencyDetectorDisabledRelationalTestBase<
        ConcurrencyDetectorDisabledMySqlTest.ConcurrencyDetectorMySqlFixture>
    {
        public ConcurrencyDetectorDisabledMySqlTest(ConcurrencyDetectorMySqlFixture fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        protected override async Task ConcurrencyDetectorTest(Func<ConcurrencyDetectorDbContext, Task<object>> test)
        {
            await base.ConcurrencyDetectorTest(test);

            Assert.NotEmpty(Fixture.TestSqlLoggerFactory.SqlStatements);
        }

        // TODO: Will be fixed by https://github.com/dotnet/efcore/pull/24819
        public override Task FromSql(bool async)
            => ConcurrencyDetectorTest(async c => async
                ? await c.Products.FromSqlRaw("SELECT * FROM `Products`").ToListAsync()
                : c.Products.FromSqlRaw("SELECT * FROM `Products`").ToList());

        public class ConcurrencyDetectorMySqlFixture : ConcurrencyDetectorFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => builder.EnableThreadSafetyChecks(false);
        }
    }
}
