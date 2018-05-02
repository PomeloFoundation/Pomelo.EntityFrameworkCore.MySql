using Microsoft.EntityFrameworkCore;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class DatabindingMySqlTest : DatabindingTestBase<F1MySqlFixture>
    {
        public DatabindingMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
