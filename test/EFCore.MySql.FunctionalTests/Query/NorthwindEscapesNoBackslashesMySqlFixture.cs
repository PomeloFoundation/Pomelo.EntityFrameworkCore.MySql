using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class
        NorthwindEscapesNoBackslashesMySqlFixture<TModelCustomizer> : NorthwindEscapesMySqlFixture<TModelCustomizer> where TModelCustomizer : IModelCustomizer, new()
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.NoBackslashEscapesInstance;
    }
}
