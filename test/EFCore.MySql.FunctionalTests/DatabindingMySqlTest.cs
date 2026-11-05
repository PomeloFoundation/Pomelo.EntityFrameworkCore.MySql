using Microsoft.EntityFrameworkCore;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class DatabindingMySqlTest : DataBindingTestBase<F1MySqlFixture>
    {
        public DatabindingMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
