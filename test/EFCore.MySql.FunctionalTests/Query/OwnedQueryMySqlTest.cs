using System.Threading.Tasks;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class OwnedQueryMySqlTest : OwnedQueryRelationalTestBase<OwnedQueryMySqlTest.OwnedQueryMySqlFixture>
    {
        public OwnedQueryMySqlTest(OwnedQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
        public override Task Union_over_owned_collection(bool async)
        {
            return base.Union_over_owned_collection(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
        public override Task Distinct_over_owned_collection(bool async)
        {
            return base.Distinct_over_owned_collection(async);
        }

        public class OwnedQueryMySqlFixture : RelationalOwnedQueryFixture
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
