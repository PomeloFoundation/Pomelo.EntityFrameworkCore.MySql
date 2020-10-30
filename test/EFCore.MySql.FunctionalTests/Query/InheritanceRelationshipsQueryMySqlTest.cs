using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class InheritanceRelationshipsQueryMySqlTest : InheritanceRelationshipsQueryRelationalTestBase<InheritanceRelationshipsQueryMySqlFixture>
    {
        public InheritanceRelationshipsQueryMySqlTest(InheritanceRelationshipsQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
