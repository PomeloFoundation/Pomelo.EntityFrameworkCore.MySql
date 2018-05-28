using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class MySqlServiceCollectionExtensionsTest : RelationalServiceCollectionExtensionsTestBase
    {
        public MySqlServiceCollectionExtensionsTest()
            : base(MySqlTestHelpers.Instance)
        {
        }
    }
}
