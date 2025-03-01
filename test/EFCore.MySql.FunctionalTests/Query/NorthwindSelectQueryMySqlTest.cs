using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindSelectQueryMySqlTest : NorthwindSelectQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindSelectQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        public override async Task Select_datetime_year_component(bool async)
        {
            await base.Select_datetime_year_component(async);

            AssertSql(
                @"SELECT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_month_component(bool async)
        {
            await base.Select_datetime_month_component(async);

        AssertSql(
"""
SELECT EXTRACT(month FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
""");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_day_of_year_component(bool async)
        {
            await base.Select_datetime_day_of_year_component(async);

            AssertSql(
                @"SELECT DAYOFYEAR(`o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_day_component(bool async)
        {
            await base.Select_datetime_day_component(async);

            AssertSql(
                @"SELECT EXTRACT(day FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_hour_component(bool async)
        {
            await base.Select_datetime_hour_component(async);

            AssertSql(
                @"SELECT EXTRACT(hour FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_minute_component(bool async)
        {
            await base.Select_datetime_minute_component(async);

            AssertSql(
                @"SELECT EXTRACT(minute FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_second_component(bool async)
        {
            await base.Select_datetime_second_component(async);

            AssertSql(
                @"SELECT EXTRACT(second FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [ConditionalTheory]
        public override async Task Select_datetime_millisecond_component(bool async)
        {
            await base.Select_datetime_millisecond_component(async);

            AssertSql(
                @"SELECT (EXTRACT(microsecond FROM `o`.`OrderDate`)) DIV (1000)
FROM `Orders` AS `o`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash), Skip = "This test/query crashes MySQL 8 even with inlined parameters every single time. Could be related to LATERAL.")]
        public override async Task Correlated_collection_after_distinct_not_containing_original_identifier(bool async)
        {
            await base.Correlated_collection_after_distinct_not_containing_original_identifier(async);

            AssertSql(
"""
SELECT `t`.`OrderDate`, `t`.`CustomerID`, `t0`.`Outer1`, `t0`.`Outer2`, `t0`.`Inner`, `t0`.`OrderDate`
FROM (
    SELECT DISTINCT `o`.`OrderDate`, `o`.`CustomerID`
    FROM `Orders` AS `o`
) AS `t`
LEFT JOIN LATERAL (
    SELECT `t`.`OrderDate` AS `Outer1`, `t`.`CustomerID` AS `Outer2`, `o0`.`OrderID` AS `Inner`, `o0`.`OrderDate`
    FROM `Orders` AS `o0`
    WHERE ((`o0`.`CustomerID` = `t`.`CustomerID`) OR (`o0`.`CustomerID` IS NULL AND (`t`.`CustomerID` IS NULL))) AND `o0`.`OrderID` IN (
        SELECT `f`.`value`
        FROM JSON_TABLE('[10248,10249,10250]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `f`
    )
) AS `t0` ON TRUE
ORDER BY `t`.`OrderDate`, `t`.`CustomerID`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash), Skip = "This test/query crashes MySQL 8 even with inlined parameters every single time. Could be related to LATERAL.")]
        public override async Task Correlated_collection_after_groupby_with_complex_projection_not_containing_original_identifier(bool async)
        {
            await base.Correlated_collection_after_groupby_with_complex_projection_not_containing_original_identifier(async);

            AssertSql(
"""
                SELECT `t0`.`CustomerID`, `t0`.`Complex`, `t1`.`Outer`, `t1`.`Inner`, `t1`.`OrderDate`
                FROM (
                    SELECT `t`.`CustomerID`, `t`.`Complex`
                    FROM (
                        SELECT `o`.`CustomerID`, EXTRACT(month FROM `o`.`OrderDate`) AS `Complex`
                        FROM `Orders` AS `o`
                    ) AS `t`
                    GROUP BY `t`.`CustomerID`, `t`.`Complex`
                ) AS `t0`
                LEFT JOIN LATERAL (
                    SELECT `t0`.`CustomerID` AS `Outer`, `o0`.`OrderID` AS `Inner`, `o0`.`OrderDate`
                    FROM `Orders` AS `o0`
                    WHERE ((`o0`.`CustomerID` = `t0`.`CustomerID`) OR (`o0`.`CustomerID` IS NULL AND (`t0`.`CustomerID` IS NULL))) AND `o0`.`OrderID` IN (
                        SELECT `f`.`value`
                        FROM JSON_TABLE('[10248,10249,10250]', '$[*]' COLUMNS (
                            `key` FOR ORDINALITY,
                            `value` int PATH '$[0]'
                        )) AS `f`
                    )
                ) AS `t1` ON TRUE
                ORDER BY `t0`.`CustomerID`, `t0`.`Complex`
""");
        }

        public override async Task Correlated_collection_after_distinct_with_complex_projection_not_containing_original_identifier(bool async)
        {
            // Identifier set for Distinct. Issue #24440.
            var message = (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Correlated_collection_after_distinct_with_complex_projection_not_containing_original_identifier(async)))
                .Message;

            if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
            {
                Assert.Equal(RelationalStrings.InsufficientInformationToIdentifyElementOfCollectionJoin, message);
            }
            else
            {
                Assert.Contains("Primitive collections support has not been enabled.", message);
            }

            AssertSql();
        }

        public override async Task SelectMany_with_collection_being_correlated_subquery_which_references_non_mapped_properties_from_inner_and_outer_entity(bool async)
        {
            await AssertUnableToTranslateEFProperty(
                () => base
                    .SelectMany_with_collection_being_correlated_subquery_which_references_non_mapped_properties_from_inner_and_outer_entity(
                        async));

            AssertSql();
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        public override Task SelectMany_correlated_with_outer_2(bool async)
        {
            return base.SelectMany_correlated_with_outer_2(async);
        }

        // TODO:
        // [SupportedServerVersionCondition(ServerVersion.CrossApplySupportKey)ey)]
        [ConditionalTheory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        public override Task SelectMany_correlated_with_outer_4(bool async)
        {
            return base.SelectMany_correlated_with_outer_4(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Project_single_element_from_collection_with_OrderBy_Distinct_and_FirstOrDefault_followed_by_projecting_length(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Distinct_and_FirstOrDefault_followed_by_projecting_length(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_SingleOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_SingleOrDefault(async);
        }

        [ConditionalTheory]
        public override Task Member_binding_after_ctor_arguments_fails_with_client_eval(bool async)
        {
            return AssertTranslationFailed(() => base.Member_binding_after_ctor_arguments_fails_with_client_eval(async));
        }

        [ConditionalTheory(Skip = "TODO: Seems to be a MySQL bug. Needs to be verified and reported, if not already.")]
        public override Task Take_on_top_level_and_on_collection_projection_with_outer_apply(bool async)
        {
            return base.Take_on_top_level_and_on_collection_projection_with_outer_apply(async);
        }

        [ConditionalTheory(Skip = "Needs proper TimeSpan support, with a wider range than the current TIME mapping can provide.")]
        public override Task Projection_containing_DateTime_subtraction(bool async)
        {
            return base.Projection_containing_DateTime_subtraction(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
