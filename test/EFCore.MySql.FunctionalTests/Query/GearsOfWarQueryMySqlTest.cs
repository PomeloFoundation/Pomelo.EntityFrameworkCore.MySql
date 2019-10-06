using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
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

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_subquery_distinct_firstordefault_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_firstordefault_boolean(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool isAsync)
        {
            return base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool isAsync)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(isAsync);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        [MemberData("IsAsyncData")]
        public override Task Select_subquery_distinct_firstordefault(bool isAsync)
        {
            return base.Select_subquery_distinct_firstordefault(isAsync);
        }
    }
}
