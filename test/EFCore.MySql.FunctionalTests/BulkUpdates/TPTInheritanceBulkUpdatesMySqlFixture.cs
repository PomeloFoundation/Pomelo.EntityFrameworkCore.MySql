using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.BulkUpdates.Inheritance;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class TPTInheritanceBulkUpdatesMySqlFixture : TPTInheritanceBulkUpdatesFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
