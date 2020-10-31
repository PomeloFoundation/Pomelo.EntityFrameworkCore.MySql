using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindJoinQueryMySqlTest : NorthwindJoinQueryRelationalTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindJoinQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_with_client_eval(bool async)
        {
            return base.SelectMany_with_client_eval(async);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_with_client_eval_with_collection_shaper(bool async)
        {
            return base.SelectMany_with_client_eval_with_collection_shaper(async);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_with_client_eval_with_collection_shaper_ignored(bool async)
        {
            return base.SelectMany_with_client_eval_with_collection_shaper_ignored(async);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_with_selecting_outer_entity(bool async)
        {
            return base.SelectMany_with_selecting_outer_entity(async);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_with_selecting_outer_element(bool async)
        {
            return base.SelectMany_with_selecting_outer_element(async);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_with_selecting_outer_entity_column_and_inner_column(bool async)
        {
            return base.SelectMany_with_selecting_outer_entity_column_and_inner_column(async);
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
