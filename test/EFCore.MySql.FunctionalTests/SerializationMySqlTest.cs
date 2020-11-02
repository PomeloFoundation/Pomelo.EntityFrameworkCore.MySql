using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SerializationMySqlTest : SerializationTestBase<F1MySqlFixture>
    {
        public SerializationMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
