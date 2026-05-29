using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class F1MySqlFixture : F1MySqlFixture<byte[]>
    {
    }

    public abstract class F1MySqlFixture<TRowVersion> : F1RelationalFixture<TRowVersion>
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public override TestHelpers TestHelpers
            => MySqlTestHelpers.Instance;
    }
}
