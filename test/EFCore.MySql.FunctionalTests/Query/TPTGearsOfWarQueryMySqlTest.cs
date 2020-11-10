﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTGearsOfWarQueryMySqlTest : TPTGearsOfWarQueryRelationalTestBase<TPTGearsOfWarQueryMySqlFixture>
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public TPTGearsOfWarQueryMySqlTest(TPTGearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
#pragma warning restore IDE0060 // Remove unused parameter
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
        {
            var dto = GearsOfWarQueryMySqlFixture.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));
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
            var dateTimeOffset = GearsOfWarQueryMySqlFixture.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));

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

        // TODO: Implement strategy as discussed with @roji (including emails) for EF Core 5.
        [ConditionalTheory(Skip = "#996")]
        public override Task Client_member_and_unsupported_string_Equals_in_the_same_query(bool async)
        {
            return base.Client_member_and_unsupported_string_Equals_in_the_same_query(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(bool async)
        {
            return base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(bool async)
        {
            return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(bool async)
        {
            return base.SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool async)
        {
            return base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool async)
        {
            return base.Correlated_collections_inner_subquery_selector_references_outer_qsre(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply), Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        public override Task Outer_parameter_in_join_key_inner_and_outer(bool async)
        {
            return base.Outer_parameter_in_join_key_inner_and_outer(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply), Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool async)
        {
            return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply), Skip = "https://bugs.mysql.com/bug.php?id=96946")]
        public override Task Outer_parameter_in_join_key(bool async)
        {
            return base.Outer_parameter_in_join_key(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool async)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_subquery_distinct_firstordefault(bool async)
        {
            return base.Select_subquery_distinct_firstordefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Where_subquery_distinct_first_boolean(bool async)
        {
            return base.Where_subquery_distinct_first_boolean(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Where_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool async)
        {
            return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Project_collection_navigation_nested_with_take_composite_key(bool async)
        {
            return base.Project_collection_navigation_nested_with_take_composite_key(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Project_entity_and_collection_element(bool async)
        {
            return base.Project_entity_and_collection_element(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Project_one_value_type_with_client_projection_from_empty_collection(bool async)
        {
            return base.Project_one_value_type_with_client_projection_from_empty_collection(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_subquery_projecting_multiple_constants_inside_anonymous(bool async)
        {
            return base.Select_subquery_projecting_multiple_constants_inside_anonymous(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_subquery_projecting_single_constant_inside_anonymous(bool async)
        {
            return base.Select_subquery_projecting_single_constant_inside_anonymous(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_subquery_projecting_single_constant_null_of_non_mapped_type(bool async)
        {
            return base.Select_subquery_projecting_single_constant_null_of_non_mapped_type(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Select_subquery_projecting_single_constant_of_non_mapped_type(bool async)
        {
            return base.Select_subquery_projecting_single_constant_of_non_mapped_type(async);
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

        [SupportedServerVersionLessThanCondition("5.6.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        public override Task Correlated_collections_basic_projecting_constant(bool async)
        {
            return base.Correlated_collections_basic_projecting_constant(async);
        }

        [SupportedServerVersionLessThanCondition("5.6.0", Skip = "https://bugs.mysql.com/bug.php?id=96947")]
        public override Task Correlated_collections_basic_projecting_constant_bool(bool async)
        {
            return base.Correlated_collections_basic_projecting_constant_bool(async);
        }

        [ConditionalTheory( /*Skip = "https://github.com/mysql-net/MySqlConnector/pull/707"*/)]
        public override Task Optional_Navigation_Null_Coalesce_To_Clr_Type(bool async)
        {
            return base.Optional_Navigation_Null_Coalesce_To_Clr_Type(async);
        }

        [ConditionalTheory( /*Skip = "https://github.com/mysql-net/MySqlConnector/pull/707"*/)]
        public override Task Projecting_nullable_bool_in_conditional_works(bool async)
        {
            return base.Projecting_nullable_bool_in_conditional_works(async);
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
    }
}
