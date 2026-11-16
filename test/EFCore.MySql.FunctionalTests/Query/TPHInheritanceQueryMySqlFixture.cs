using Microting.EntityFrameworkCore.MySql.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlFixture : TPHInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory =>  MySqlTestStoreFactory.Instance;
}
