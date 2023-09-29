using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class TPTTableSplittingMySqlTest : TPTTableSplittingTestBase
    {
        public TPTTableSplittingMySqlTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        public override Task Can_insert_dependent_with_just_one_parent()
        {
            // This scenario is not valid for TPT
            return Task.CompletedTask;
        }

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
