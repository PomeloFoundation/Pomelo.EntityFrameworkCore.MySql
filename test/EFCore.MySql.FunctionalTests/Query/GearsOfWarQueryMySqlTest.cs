using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GearsOfWarQueryMySqlTest : GearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_now(bool isAsync)
        {
            return base.Where_datetimeoffset_now(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_utcnow(bool isAsync)
        {
            return base.Where_datetimeoffset_utcnow(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_date_component(bool isAsync)
        {
            return base.Where_datetimeoffset_date_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_year_component(bool isAsync)
        {
            return base.Where_datetimeoffset_year_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_month_component(bool isAsync)
        {
            return base.Where_datetimeoffset_month_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_dayofyear_component(bool isAsync)
        {
            return base.Where_datetimeoffset_dayofyear_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_day_component(bool isAsync)
        {
            return base.Where_datetimeoffset_day_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_hour_component(bool isAsync)
        {
            return base.Where_datetimeoffset_hour_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_minute_component(bool isAsync)
        {
            return base.Where_datetimeoffset_minute_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_second_component(bool isAsync)
        {
            return base.Where_datetimeoffset_second_component(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_millisecond_component(bool isAsync)
        {
            return base.Where_datetimeoffset_millisecond_component(isAsync);
        }

        [ConditionalFact(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override void Where_datetimeoffset_milliseconds_parameter_and_constant()
        {
            base.Where_datetimeoffset_milliseconds_parameter_and_constant();
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddYears(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddYears(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddMonths(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddMonths(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddDays(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddDays(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddHours(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddHours(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddMinutes(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddMinutes(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddSeconds(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddSeconds(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddMilliseconds(bool isAsync)
        {
            return base.DateTimeOffset_DateAdd_AddMilliseconds(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task Time_of_day_datetimeoffset(bool isAsync)
        {
            return base.Time_of_day_datetimeoffset(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #819")]
        [MemberData("IsAsyncData")]
        public override Task GetValueOrDefault_on_DateTimeOffset(bool isAsync)
        {
            return base.GetValueOrDefault_on_DateTimeOffset(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString)]
        [MemberData("IsAsyncData")]
        public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool isAsync)
        {
            return base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString)]
        [MemberData("IsAsyncData")]
        public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool isAsync)
        {
            return base.Correlated_collections_inner_subquery_selector_references_outer_qsre(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData("IsAsyncData")]
        public override Task Outer_parameter_in_join_key_inner_and_outer(bool isAsync)
        {
            return base.Outer_parameter_in_join_key_inner_and_outer(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString)]
        [MemberData("IsAsyncData")]
        public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool isAsync)
        {
            return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString)]
        [MemberData("IsAsyncData")]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool isAsync)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(isAsync);
        }

        [SupportedServerVersionLessThanTheory("8.0.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_basic_projecting_constant(bool isAsync)
        {
            return base.Correlated_collections_basic_projecting_constant(isAsync);
        }
    }
}
