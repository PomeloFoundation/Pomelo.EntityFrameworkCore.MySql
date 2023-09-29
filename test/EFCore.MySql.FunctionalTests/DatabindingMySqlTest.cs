using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class DatabindingMySqlTest : DataBindingTestBase<F1MySqlFixture>
    {
        public DatabindingMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
