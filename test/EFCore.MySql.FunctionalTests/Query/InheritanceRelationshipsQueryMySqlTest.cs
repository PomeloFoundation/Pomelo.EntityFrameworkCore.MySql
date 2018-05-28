using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class InheritanceRelationshipsQueryMySqlTest : InheritanceRelationshipsQueryTestBase<InheritanceRelationshipsQueryMySqlFixture>
    {
        public InheritanceRelationshipsQueryMySqlTest(InheritanceRelationshipsQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
