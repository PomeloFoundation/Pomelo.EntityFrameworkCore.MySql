using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindGroupByQueryMySqlTest : NorthwindGroupByQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindGroupByQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        public override Task Select_uncorrelated_collection_with_groupby_works(bool async)
        {
            return base.Select_uncorrelated_collection_with_groupby_works(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        public override Task Select_uncorrelated_collection_with_groupby_multiple_collections_work(bool async)
        {
            return base.Select_uncorrelated_collection_with_groupby_multiple_collections_work(async);
        }

        [ConditionalTheory(Skip = "Does not work when using ONLY_FULL_GROUP_BY. See https://github.com/dotnet/efcore/issues/19027")]
        public override Task GroupBy_scalar_subquery(bool async)
        {
            return base.GroupBy_scalar_subquery(async);
        }

        [SupportedServerVersionTheory("8.0.22-mysql", "0.0.0-mariadb")]
        public override Task GroupBy_group_Where_Select_Distinct_aggregate(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/898.
            return base.GroupBy_group_Where_Select_Distinct_aggregate(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
