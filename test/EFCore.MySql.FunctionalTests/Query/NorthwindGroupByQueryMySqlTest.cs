using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
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

        protected override bool CanExecuteQueryString
            => true;

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_uncorrelated_collection_with_groupby_works(bool async)
        {
            return base.Select_uncorrelated_collection_with_groupby_works(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_uncorrelated_collection_with_groupby_multiple_collections_work(bool async)
        {
            return base.Select_uncorrelated_collection_with_groupby_multiple_collections_work(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_uncorrelated_collection_with_groupby_when_outer_is_distinct(bool async)
        {
            return base.Select_uncorrelated_collection_with_groupby_when_outer_is_distinct(async);
        }

        [ConditionalTheory(Skip = "Does not work when using ONLY_FULL_GROUP_BY. See https://github.com/dotnet/efcore/issues/19027")]
        public override Task GroupBy_scalar_subquery(bool async)
        {
            return base.GroupBy_scalar_subquery(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql", "0.0.0-mariadb")]
        public override Task GroupBy_group_Where_Select_Distinct_aggregate(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/898.
            return base.GroupBy_group_Where_Select_Distinct_aggregate(async);
        }

        [SupportedServerVersionCondition("8.0.0-mysql", "0.0.0-mariadb")] // Is an issue issue in MySQL 5.7.34, but not in 8.0.25.
        public override Task GroupBy_constant_with_where_on_grouping_with_aggregate_operators(bool async)
        {
            // See https://github.com/mysql-net/MySqlConnector/issues/980.
            return base.GroupBy_constant_with_where_on_grouping_with_aggregate_operators(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
