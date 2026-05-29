using Microsoft.EntityFrameworkCore;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SerializationMySqlTest : SerializationTestBase<F1MySqlFixture>
    {
        public SerializationMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
