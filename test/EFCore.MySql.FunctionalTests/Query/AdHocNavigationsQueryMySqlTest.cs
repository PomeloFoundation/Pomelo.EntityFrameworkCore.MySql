using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocNavigationsQueryMySqlTest : AdHocNavigationsQueryRelationalTestBase
{
    public AdHocNavigationsQueryMySqlTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
