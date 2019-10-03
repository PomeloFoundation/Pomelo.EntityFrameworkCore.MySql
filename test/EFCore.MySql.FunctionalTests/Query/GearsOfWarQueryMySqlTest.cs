using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GearsOfWarQueryMySqlTest : GearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        private readonly MySqlTypeMappingSource _typeMappingSource;

        public GearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            using var context = CreateContext();
            _typeMappingSource = (MySqlTypeMappingSource)context.GetService<IRelationalTypeMappingSource>();

            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_now(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline != DateTimeOffset.Now
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource) != DateTimeOffset.Now
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_utcnow(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline != DateTimeOffset.UtcNow
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource) != DateTimeOffset.UtcNow
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_date_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Date > new DateTimeOffset().Date
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Date > new DateTimeOffset().SimulateDatabaseRoundtrip(_typeMappingSource).Date
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_year_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Year == 2
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Year == 2
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_month_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Month == 1
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Month == 1
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_dayofyear_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.DayOfYear == 2
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).DayOfYear == 2
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_day_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Day == 2
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Day == 2
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_hour_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Hour == 10
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Hour == 10
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_minute_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Minute == 0
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Minute == 0
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_second_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Second == 0
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Second == 0
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Where_datetimeoffset_millisecond_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ms => from m in ms
                    where m.Timeline.Millisecond == 0
                    select m,
                ms => from m in ms
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Millisecond == 0
                    select new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    });
        }

        [ConditionalFact]
        public override void Where_datetimeoffset_milliseconds_parameter_and_constant()
        {
            base.Where_datetimeoffset_milliseconds_parameter_and_constant();
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddYears(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddYears(1));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddMonths(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddMonths(1));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddDays(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddDays(1));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddHours(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddHours(1));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddMinutes(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddMinutes(1));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddSeconds(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddSeconds(1));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_DateAdd_AddMilliseconds(bool isAsync)
        {
            return AssertQueryScalar<Mission>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.AddMilliseconds(300));
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task Time_of_day_datetimeoffset(bool isAsync)
        {
            return AssertQueryScalar<Mission, TimeSpan>(
                isAsync,
                ms => from m in ms
                    select m.Timeline.TimeOfDay,
                ms => from m in ms
                    select m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).TimeOfDay);
        }

        [ConditionalTheory]
        [MemberData("IsAsyncData")]
        public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool isAsync)
        {
            var dto = new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0));
            var start = dto.AddDays(-1);
            var end = dto.AddDays(1);
            var dates = new DateTimeOffset[] { dto };

            return AssertQuery<Mission>(
                isAsync,
                ms => ms.Where(m => start <= m.Timeline.Date &&
                                    m.Timeline < end &&
                                    dates.Contains(m.Timeline)),
                ms => ms.Where(m => start.SimulateDatabaseRoundtrip(_typeMappingSource) <= m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Date &&
                                    m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource) < end.SimulateDatabaseRoundtrip(_typeMappingSource) &&
                                    dates.Select(d => d.SimulateDatabaseRoundtrip(_typeMappingSource)).Contains(m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)))
                    .Select(m => new Mission()
                    {
                        Id = m.Id,
                        CodeName = m.CodeName,
                        Rating = m.Rating,
                        Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                        ParticipatingSquads = m.ParticipatingSquads,
                    }));
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

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData("IsAsyncData")]
        public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool isAsync)
        {
            return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportVersionString, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData("IsAsyncData")]
        public override Task Outer_parameter_in_join_key(bool isAsync)
        {
            return base.Outer_parameter_in_join_key(isAsync);
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

        [ConditionalTheory(Skip = "https://github.com/mysql-net/MySqlConnector/pull/707")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Optional_Navigation_Null_Coalesce_To_Clr_Type(bool isAsync)
        {
            return base.Optional_Navigation_Null_Coalesce_To_Clr_Type(isAsync);
        }

        [ConditionalTheory(Skip = "https://github.com/mysql-net/MySqlConnector/pull/707")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Projecting_nullable_bool_in_conditional_works(bool isAsync)
        {
            return base.Projecting_nullable_bool_in_conditional_works(isAsync);
        }

        [ConditionalTheory(Skip = "https://github.com/mysql-net/MySqlConnector/issues/708")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_boolean(bool isAsync)
        {
            return base.Select_subquery_boolean(isAsync);
        }

        [ConditionalTheory(Skip = "https://github.com/mysql-net/MySqlConnector/issues/708")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_boolean_with_pushdown(bool isAsync)
        {
            return base.Select_subquery_boolean_with_pushdown(isAsync);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool isAsync)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(isAsync);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool isAsync)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(isAsync);
        }

        public Task AssertQueryScalar<TItem>(bool isAsync,
            Func<IQueryable<TItem>, IQueryable<DateTimeOffset>> query,
            bool assertOrder = false)
            where TItem : class
            => AssertQueryScalar(
                isAsync,
                query,
                ms => query(ms).Select(d => d.SimulateDatabaseRoundtrip(_typeMappingSource)),
                assertOrder);
    }
}
