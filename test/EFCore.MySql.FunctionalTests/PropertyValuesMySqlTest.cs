using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    // Disabled via internal access. EF Core 10 requires complex-type collections (e.g. School.Departments) to be mapped
    // to a JSON column via ToJson(); the Pomelo MySQL provider does not implement EF JSON mapping
    // (MySqlModelValidator throws Ef7CoreJsonMappingNotSupported), so the shared model cannot be built.
    internal class PropertyValuesMySqlTest : PropertyValuesTestBase<PropertyValuesMySqlTest.PropertyValuesMySqlFixture>
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
