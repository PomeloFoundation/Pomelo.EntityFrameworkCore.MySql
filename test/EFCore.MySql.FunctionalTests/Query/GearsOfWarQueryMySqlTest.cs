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

        protected override bool CanExecuteQueryString
            => true;

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
    }
}
