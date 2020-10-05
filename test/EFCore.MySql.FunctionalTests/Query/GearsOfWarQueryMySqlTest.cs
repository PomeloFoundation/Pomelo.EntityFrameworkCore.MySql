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
    public partial class GearsOfWarQueryMySqlTest : GearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
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
        public override Task Where_datetimeoffset_now(bool async)
        {
            return AssertQuery(
                async,
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
        public override Task Where_datetimeoffset_utcnow(bool async)
        {
            return AssertQuery(
                async,
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
        public override Task Where_datetimeoffset_date_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Date > new DateTimeOffset().Date
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Date > new DateTimeOffset().SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Date
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
        public override Task Where_datetimeoffset_year_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Year == 2
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Year == 2
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
        public override Task Where_datetimeoffset_month_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Month == 1
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Month == 1
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
        public override Task Where_datetimeoffset_dayofyear_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.DayOfYear == 2
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .DayOfYear == 2
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
        public override Task Where_datetimeoffset_day_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Day == 2
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Day == 2
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
        public override Task Where_datetimeoffset_hour_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Hour == 10
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Hour == 10
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
        public override Task Where_datetimeoffset_minute_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Minute == 0
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Minute == 0
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
        public override Task Where_datetimeoffset_second_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Second == 0
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Second == 0
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
        public override Task Where_datetimeoffset_millisecond_component(bool async)
        {
            return AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Millisecond == 0
                    select m,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .Millisecond == 0
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
        public override Task DateTimeOffset_DateAdd_AddYears(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddYears(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddMonths(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddMonths(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddDays(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddDays(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddHours(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddHours(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddMinutes(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddMinutes(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddSeconds(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddSeconds(1));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_DateAdd_AddMilliseconds(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.AddMilliseconds(300));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task Time_of_day_datetimeoffset(bool async)
        {
            return AssertQueryScalar(
                async,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.TimeOfDay,
                ss => from m in ss.Set<Mission>()
                    select m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                        .TimeOfDay);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
        {
            var dto = new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0));
            var start = dto.AddDays(-1);
            var end = dto.AddDays(1);
            var dates = new[] {dto};

            return AssertQuery(
                async,
                ss => ss.Set<Mission>()
                    .Where(
                        m => start <= m.Timeline.Date &&
                             m.Timeline < end &&
                             dates.Contains(m.Timeline)),
                ss => ss.Set<Mission>()
                    .Where(
                        m => start.SimulateDatabaseRoundtrip(_typeMappingSource) <= m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)
                                 .Date &&
                             m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource) < end.SimulateDatabaseRoundtrip(_typeMappingSource) &&
                             dates.Select(d => d.SimulateDatabaseRoundtrip(_typeMappingSource))
                                 .Contains(m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource)))
                    .Select(
                        m => new Mission()
                        {
                            Id = m.Id,
                            CodeName = m.CodeName,
                            Rating = m.Rating,
                            Timeline = m.Timeline.SimulateDatabaseRoundtrip(_typeMappingSource),
                            ParticipatingSquads = m.ParticipatingSquads,
                        }));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task DateTimeOffset_Date_returns_datetime(bool async)
        {
            var dateTimeOffset = new DateTimeOffset(2, 3, 1, 8, 0, 0, new TimeSpan(-5, 0, 0));

            return AssertQuery(
                async,
                ss => ss.Set<Mission>()
                    .Where(m => m.Timeline.Date >= dateTimeOffset.Date),
                elementAsserter: AssertMission
            );
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool async)
        {
            return base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool async)
        {
            return base.Correlated_collections_inner_subquery_selector_references_outer_qsre(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Outer_parameter_in_join_key_inner_and_outer(bool async)
        {
            return base.Outer_parameter_in_join_key_inner_and_outer(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool async)
        {
            return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey, Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Outer_parameter_in_join_key(bool async)
        {
            return base.Outer_parameter_in_join_key(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool async)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_firstordefault(bool async)
        {
            return base.Select_subquery_distinct_firstordefault(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty1(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_first_boolean(bool async)
        {
            return base.Where_subquery_distinct_first_boolean(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool async)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_collection_navigation_nested_with_take_composite_key(bool async)
        {
            return base.Project_collection_navigation_nested_with_take_composite_key(async);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_entity_and_collection_element(bool async)
        {
            return base.Project_entity_and_collection_element(async);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_one_value_type_with_client_projection_from_empty_collection(bool async)
        {
            return base.Project_one_value_type_with_client_projection_from_empty_collection(async);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_multiple_constants_inside_anonymous(bool async)
        {
            return base.Select_subquery_projecting_multiple_constants_inside_anonymous(async);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_single_constant_inside_anonymous(bool async)
        {
            return base.Select_subquery_projecting_single_constant_inside_anonymous(async);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_single_constant_null_of_non_mapped_type(bool async)
        {
            return base.Select_subquery_projecting_single_constant_null_of_non_mapped_type(async);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Select_subquery_projecting_single_constant_of_non_mapped_type(bool async)
        {
            return base.Select_subquery_projecting_single_constant_of_non_mapped_type(async);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_firstordefault_boolean(async);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_lastordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_lastordefault_boolean(async);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_last_boolean(bool async)
        {
            return base.Where_subquery_distinct_last_boolean(async);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean(async);
        }

        [SupportedServerVersionTheory("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionLessThanTheory("5.6.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_basic_projecting_constant(bool async)
        {
            return base.Correlated_collections_basic_projecting_constant(async);
        }

        [SupportedServerVersionLessThanTheory("5.6.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collections_basic_projecting_constant_bool(bool async)
        {
            return base.Correlated_collections_basic_projecting_constant_bool(async);
        }

        [ConditionalTheory( /*Skip = "https://github.com/mysql-net/MySqlConnector/pull/707"*/)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Optional_Navigation_Null_Coalesce_To_Clr_Type(bool async)
        {
            return base.Optional_Navigation_Null_Coalesce_To_Clr_Type(async);
        }

        [ConditionalTheory( /*Skip = "https://github.com/mysql-net/MySqlConnector/pull/707"*/)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Projecting_nullable_bool_in_conditional_works(bool async)
        {
            return base.Projecting_nullable_bool_in_conditional_works(async);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool async)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(async);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool async)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(async);
        }

        private void AssertMission(Mission e, Mission a)
        {
            Assert.Equal(e == null, a == null);

            if (a != null)
            {
                Assert.Equal(e.Id, a.Id);
                Assert.Equal(e.CodeName, a.CodeName);
                Assert.Equal(e.Rating, a.Rating);

                // The max. resolution for DateTime values in MySQL are 6 decimal places.
                // However, .NET's DateTime has a resolution of 7 decimal places.
                const int mySqlMaxMillisecondDecimalPlaces = 6;
                var decimalPlacesFactor = (int)Math.Pow(10, 7 - mySqlMaxMillisecondDecimalPlaces);

                Assert.Equal(
                    new DateTimeOffset((long)(Math.Truncate((decimal)e.Timeline.Ticks / decimalPlacesFactor) * decimalPlacesFactor), e.Timeline.Offset).UtcDateTime,
                    a.Timeline.UtcDateTime);
            }
        }

        protected Task AssertQueryScalar(
            bool async,
            Func<ISetSource, IQueryable<DateTimeOffset>> query,
            bool assertOrder = false) => AssertQueryScalar(
            async,
            query,
            ms => query(ms)
                .Select(d => d.SimulateDatabaseRoundtrip(_typeMappingSource)),
            assertOrder);
    }
}
