using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class TableSplittingMySqlTest : TableSplittingTestBase
    {
        public TableSplittingMySqlTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
