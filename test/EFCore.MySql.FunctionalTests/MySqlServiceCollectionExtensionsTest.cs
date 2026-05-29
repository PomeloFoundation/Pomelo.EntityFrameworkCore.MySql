using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlServiceCollectionExtensionsTest : RelationalServiceCollectionExtensionsTestBase
    {
        public MySqlServiceCollectionExtensionsTest()
            : base(MySqlTestHelpers.Instance)
        {
        }
    }
}
