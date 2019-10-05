using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class FindMySqlTest : FindTestBase<FindMySqlTest.FindMySqlFixture>
    {
        public FindMySqlTest(FindMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
            => context.Set<TEntity>().Find(keyValues);

        protected override ValueTask<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
            => context.Set<TEntity>().FindAsync(keyValues);

        public class FindMySqlFixture : FindFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
