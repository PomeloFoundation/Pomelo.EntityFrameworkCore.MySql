using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class DatabindingMySqlTest : DatabindingTestBase<F1MySqlFixture>
    {
        public DatabindingMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }

        public override void DbSet_Local_calls_DetectChanges()
        {
            base.DbSet_Local_calls_DetectChanges();
        }
    }
}
