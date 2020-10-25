using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class InheritanceRelationshipsQueryMySqlTest : InheritanceRelationshipsQueryTestBase<InheritanceRelationshipsQueryMySqlFixture>
    {
        public InheritanceRelationshipsQueryMySqlTest(InheritanceRelationshipsQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
