using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Storage;
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_now(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline != DateTimeOffset.Now
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_utcnow(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline != DateTimeOffset.UtcNow
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_date_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Date > new DateTimeOffset().Date
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_year_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Year == 2
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_month_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Month == 1
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_dayofyear_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.DayOfYear == 2
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_day_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Day == 2
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_hour_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Hour == 10
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_minute_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Minute == 0
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_second_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Second == 0
                    select m,
                ss => from m in ss.Set<Mission>()
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
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_datetimeoffset_millisecond_component(bool isAsync)
        {
            return AssertQuery<Mission>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Millisecond == 0
                    select m,
                ss => from m in ss.Set<Mission>()
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

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddYears(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddYears(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddMonths(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddMonths(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddDays(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddDays(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddHours(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddHours(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddMinutes(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddMinutes(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddSeconds(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddSeconds(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddMilliseconds(bool isAsync)
        {
            return AssertQueryScalar(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddMilliseconds(300));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Time_of_day_datetimeoffset(bool isAsync)
        {
            return AssertQueryScalar<TimeSpan>(
                isAsync,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.TimeOfDay,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).TimeOfDay);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool isAsync)
        {
            var dto = new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0));
            var start = dto.AddDays(-1);
            var end = dto.AddDays(1);
            var dates = new DateTimeOffset[] { dto };

            return AssertQuery<Mission>(
                isAsync,
                ss => ss.Set<Mission>().Where(m => start <= m.Timeline.Date &&
                                    m.Timeline < end &&
                                    dates.Contains(m.Timeline)),
                ss => ss.Set<Mission>().Where(m => start.SimulateDatabaseRoundtrip(_typeMappingSource) <= m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource).Date &&
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

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool isAsync)
        {
            return base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool isAsync)
        {
            return base.Correlated_collections_inner_subquery_selector_references_outer_qsre(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Outer_parameter_in_join_key_inner_and_outer(bool isAsync)
        {
            return base.Outer_parameter_in_join_key_inner_and_outer(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool isAsync)
        {
            return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Outer_parameter_in_join_key(bool isAsync)
        {
            return base.Outer_parameter_in_join_key(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool isAsync)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_firstordefault(bool isAsync)
        {
            return base.Select_subquery_distinct_firstordefault(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean1(bool isAsync)
        {
            return base.Select_subquery_distinct_singleordefault_boolean1(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty1(bool isAsync)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty1(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool isAsync)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool isAsync)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_first_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_first_boolean(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_singleordefault_boolean1(bool isAsync)
        {
            return base.Where_subquery_distinct_singleordefault_boolean1(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool isAsync)
        {
            return base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(isAsync);
        }
        
        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool isAsync)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_collection_navigation_nested_with_take_composite_key(bool isAsync)
        {
            return base.Project_collection_navigation_nested_with_take_composite_key(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_entity_and_collection_element(bool isAsync)
        {
            return base.Project_entity_and_collection_element(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_one_value_type_with_client_projection_from_empty_collection(bool isAsync)
        {
            return base.Project_one_value_type_with_client_projection_from_empty_collection(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_multiple_constants_inside_anonymous(bool isAsync)
        {
            return base.Select_subquery_projecting_multiple_constants_inside_anonymous(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_single_constant_inside_anonymous(bool isAsync)
        {
            return base.Select_subquery_projecting_single_constant_inside_anonymous(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_single_constant_null_of_non_mapped_type(bool isAsync)
        {
            return base.Select_subquery_projecting_single_constant_null_of_non_mapped_type(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_single_constant_of_non_mapped_type(bool isAsync)
        {
            return base.Select_subquery_projecting_single_constant_of_non_mapped_type(isAsync);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_firstordefault_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_firstordefault_boolean(isAsync);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool isAsync)
        {
            return base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(isAsync);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_lastordefault_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_lastordefault_boolean(isAsync);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_last_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_last_boolean(isAsync);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean(bool isAsync)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean(isAsync);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool isAsync)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(isAsync);
        }

        [SupportedServerVersionLessThanTheory("5.6.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_basic_projecting_constant(bool isAsync)
        {
            return base.Correlated_collections_basic_projecting_constant(isAsync);
        }

        [SupportedServerVersionLessThanTheory("5.6.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_basic_projecting_constant_bool(bool isAsync)
        {
            return base.Correlated_collections_basic_projecting_constant_bool(isAsync);
        }

        [ConditionalTheory(/*Skip = "https://github.com/mysql-net/MySqlConnector/pull/707"*/)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Optional_Navigation_Null_Coalesce_To_Clr_Type(bool isAsync)
        {
            return base.Optional_Navigation_Null_Coalesce_To_Clr_Type(isAsync);
        }

        [ConditionalTheory(/*Skip = "https://github.com/mysql-net/MySqlConnector/pull/707"*/)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Projecting_nullable_bool_in_conditional_works(bool isAsync)
        {
            return base.Projecting_nullable_bool_in_conditional_works(isAsync);
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

        protected Task AssertQueryScalar(bool isAsync,
            Func<ISetSource, IQueryable<DateTimeOffset>> query,
            bool assertOrder = false) => AssertQueryScalar(
                isAsync,
                query,
                ms => query(ms).Select(d => d.SimulateDatabaseRoundtrip(_typeMappingSource)),
                assertOrder);
    }
}
