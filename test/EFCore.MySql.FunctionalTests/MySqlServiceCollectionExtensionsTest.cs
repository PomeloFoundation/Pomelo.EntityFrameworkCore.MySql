using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlServiceCollectionExtensionsTest : RelationalServiceCollectionExtensionsTestBase
    {
        public MySqlServiceCollectionExtensionsTest()
            : base(MySqlTestHelpers.Instance)
        {
        }
    }
}
