using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using NameSpace1;
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

    public override async Task Multiple_different_entity_type_from_different_namespaces(bool async)
    {
        // The only change is the FromSqlRaw SQL string:
        //     Original: SELECT cast(null as int) AS MyValue
        //     Changed:  SELECT cast(null as signed) AS MyValue
        // The other comments are part of the base implementation.

        var contextFactory = await InitializeAsync<Context23981>();
        using var context = contextFactory.CreateContext();
        //var good1 = context.Set<NameSpace1.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        //var good2 = context.Set<NameSpace2.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        var bad = context.Set<TestQuery>().FromSqlRaw(@"SELECT cast(null as signed) AS MyValue").ToList(); // Exception
    }
}
