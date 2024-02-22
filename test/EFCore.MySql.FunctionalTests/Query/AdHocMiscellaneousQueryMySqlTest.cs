using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocMiscellaneousQueryMySqlTest : AdHocMiscellaneousQueryRelationalTestBase
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    protected override void Seed2951(Context2951 context)
        => context.Database.ExecuteSqlRaw(
            """
CREATE TABLE `ZeroKey` (`Id` int);
INSERT INTO `ZeroKey` VALUES (NULL)
""");
}
