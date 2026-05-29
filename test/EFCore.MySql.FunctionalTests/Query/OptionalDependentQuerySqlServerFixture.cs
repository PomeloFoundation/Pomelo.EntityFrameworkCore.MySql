using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class OptionalDependentQueryMySqlFixture : OptionalDependentQueryFixtureBase
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
