using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class GearsOfWarQueryMySqlTest : GearsOfWarQueryRelationalTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
        {
            var dto = MySqlTestHelpers.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));
            var start = dto.AddDays(-1);
            var end = dto.AddDays(1);
            var dates = new[] { dto };

            return AssertQuery(
                async,
                ss => ss.Set<Mission>().Where(
                    m => start <= m.Timeline.Date && m.Timeline < end && dates.Contains(m.Timeline)));
        }

        public override Task Where_datetimeoffset_milliseconds_parameter_and_constant(bool async)
        {
            var dateTimeOffset = MySqlTestHelpers.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));

            // Literal where clause
            var p = Expression.Parameter(typeof(Mission), "i");
            var dynamicWhere = Expression.Lambda<Func<Mission, bool>>(
                Expression.Equal(
                    Expression.Property(p, "Timeline"),
                    Expression.Constant(dateTimeOffset)
                ), p);

            return AssertCount(
                async,
                ss => ss.Set<Mission>().Where(dynamicWhere),
                ss => ss.Set<Mission>().Where(m => m.Timeline == dateTimeOffset));
        }

        [ConditionalTheory(Skip = "TODO: Does not work as expected, probably due to some test definition issues.")]
        public override async Task DateTimeOffsetNow_minus_timespan(bool async)
        {
            var timeSpan = new TimeSpan(10000); // <-- changed from 1000 to 10000 ticks

            await AssertQuery(
                async,
                ss => ss.Set<Mission>().Where(e => e.Timeline > DateTimeOffset.Now - timeSpan));

            AssertSql(
"""
@__timeSpan_0='00:00:00.0010000' (DbType = DateTimeOffset)

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Timeline` > (UTC_TIMESTAMP() - @__timeSpan_0)
""");
        }

        // TODO: Implement strategy as discussed with @roji (including emails) for EF Core 5.
        [ConditionalTheory(Skip = "#996")]
        public override Task Client_member_and_unsupported_string_Equals_in_the_same_query(bool async)
        {
            return base.Client_member_and_unsupported_string_Equals_in_the_same_query(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_firstordefault(bool async)
        {
            return base.Select_subquery_distinct_firstordefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Where_subquery_distinct_first_boolean(bool async)
        {
            return base.Where_subquery_distinct_first_boolean(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Where_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_distinct_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_firstordefault_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_distinct_lastordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_lastordefault_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_distinct_last_boolean(bool async)
        {
            return base.Where_subquery_distinct_last_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Concat_with_collection_navigations(bool async)
        {
            return base.Concat_with_collection_navigations(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Select_navigation_with_concat_and_count(bool async)
        {
            return base.Select_navigation_with_concat_and_count(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Union_with_collection_navigations(bool async)
        {
            return base.Union_with_collection_navigations(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_concat_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_concat_firstordefault_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_join_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_join_firstordefault_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_left_join_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_left_join_firstordefault_boolean(async);
        }

        [SupportedServerVersionCondition("8.0.18-mysql", Skip = "TODO: Pinpoint exact version number! Referencing outer column from WHERE subquery does not work in previous versions. Inverse of #573")]
        public override Task Where_subquery_union_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_union_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool async)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(async);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool async)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql")]
        public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(bool async)
        {
            return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql")]
        public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(bool async)
        {
            return base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(async);
        }

        [ConditionalTheory(Skip = "Another LATERAL JOIN bug in MySQL. Grouping leads to unexpected result set.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
        {
            return base.Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);
        }

        public override async Task Group_by_on_StartsWith_with_null_parameter_as_argument(bool async)
        {
            await base.Group_by_on_StartsWith_with_null_parameter_as_argument(async);

        AssertSql(
"""
SELECT `g0`.`Key`
FROM (
    SELECT FALSE AS `Key`
    FROM `Gears` AS `g`
) AS `g0`
GROUP BY `g0`.`Key`
""");
        }

        public override async Task Array_access_on_byte_array(bool async)
        {
            await base.Array_access_on_byte_array(async);

            AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE ASCII(SUBSTRING(`s`.`Banner5`, 2 + 1, 1)) = 6
""");
        }

        public override async Task DateTimeOffset_to_unix_time_milliseconds(bool async)
        {
            await base.DateTimeOffset_to_unix_time_milliseconds(async);

            AssertSql(
"""
@__unixEpochMilliseconds_0='0'

SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`Discriminator`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `s1`.`SquadId`, `s1`.`MissionId`
FROM `Gears` AS `g`
INNER JOIN `Squads` AS `s` ON `g`.`SquadId` = `s`.`Id`
LEFT JOIN `SquadMissions` AS `s1` ON `s`.`Id` = `s1`.`SquadId`
WHERE NOT EXISTS (
    SELECT 1
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    WHERE (`s`.`Id` = `s0`.`SquadId`) AND (@__unixEpochMilliseconds_0 = (TIMESTAMPDIFF(microsecond, TIMESTAMP '1970-01-01 00:00:00', `m`.`Timeline`)) DIV (1000)))
ORDER BY `g`.`Nickname`, `g`.`SquadId`, `s`.`Id`, `s1`.`SquadId`
""");
        }

        public override async Task DateTimeOffset_to_unix_time_seconds(bool async)
        {
            await base.DateTimeOffset_to_unix_time_seconds(async);

            AssertSql(
"""
@__unixEpochSeconds_0='0'

SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`Discriminator`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `s1`.`SquadId`, `s1`.`MissionId`
FROM `Gears` AS `g`
INNER JOIN `Squads` AS `s` ON `g`.`SquadId` = `s`.`Id`
LEFT JOIN `SquadMissions` AS `s1` ON `s`.`Id` = `s1`.`SquadId`
WHERE NOT EXISTS (
    SELECT 1
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    WHERE (`s`.`Id` = `s0`.`SquadId`) AND (@__unixEpochSeconds_0 = TIMESTAMPDIFF(second, TIMESTAMP '1970-01-01 00:00:00', `m`.`Timeline`)))
ORDER BY `g`.`Nickname`, `g`.`SquadId`, `s`.`Id`, `s1`.`SquadId`
""");
        }

        public override async Task Group_by_with_having_StartsWith_with_null_parameter_as_argument(bool async)
        {
            await base.Group_by_with_having_StartsWith_with_null_parameter_as_argument(async);

            AssertSql(
"""
SELECT `g`.`FullName`
FROM `Gears` AS `g`
GROUP BY `g`.`FullName`
HAVING FALSE
""");
        }

        public override async Task Select_StartsWith_with_null_parameter_as_argument(bool async)
        {
            await base.Select_StartsWith_with_null_parameter_as_argument(async);

            AssertSql(
"""
SELECT FALSE
FROM `Gears` AS `g`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.LimitWithNonConstantValue))]
        public override async Task Where_subquery_with_ElementAt_using_column_as_index(bool async)
        {
            await base.Where_subquery_with_ElementAt_using_column_as_index(async);

            AssertSql("");
        }

        public override async Task Where_datetimeoffset_hour_component(bool async)
        {
            await AssertQuery(
                async,
                ss => from m in ss.Set<Mission>()
                    where m.Timeline.Hour == /* 10 */ 8
                    select m);

            AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(hour FROM `m`.`Timeline`) = 8
""");
        }
    }
}
