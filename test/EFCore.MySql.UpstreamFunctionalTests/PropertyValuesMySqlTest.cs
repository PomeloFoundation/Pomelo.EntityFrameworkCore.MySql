using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class PropertyValuesMySqlTest : PropertyValuesTestBase<PropertyValuesMySqlTest.PropertyValuesMySqlFixture>
    {
        public PropertyValuesMySqlTest(PropertyValuesMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class PropertyValuesMySqlFixture : PropertyValuesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
