using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class NorthwindQueryMySqlFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        // TODO: Consider using an existing database/initialize from script
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
