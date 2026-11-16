using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
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

        [Theory]
        public override async Task Select_datetime_year_component(bool async)
        {
            await base.Select_datetime_year_component(async);

            AssertSql(
                @"SELECT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [Theory]
        public override async Task Select_datetime_month_component(bool async)
        {
            await base.Select_datetime_month_component(async);

        AssertSql(
"""
SELECT EXTRACT(month FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
""");
        }

        [Theory]
        public override async Task Select_datetime_day_of_year_component(bool async)
        {
            await base.Select_datetime_day_of_year_component(async);

            AssertSql(
                @"SELECT DAYOFYEAR(`o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [Theory]
        public override async Task Select_datetime_day_component(bool async)
        {
            await base.Select_datetime_day_component(async);

            AssertSql(
                @"SELECT EXTRACT(day FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [Theory]
        public override async Task Select_datetime_hour_component(bool async)
        {
            await base.Select_datetime_hour_component(async);

            AssertSql(
                @"SELECT EXTRACT(hour FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [Theory]
        public override async Task Select_datetime_minute_component(bool async)
        {
            await base.Select_datetime_minute_component(async);

            AssertSql(
                @"SELECT EXTRACT(minute FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [Theory]
        public override async Task Select_datetime_second_component(bool async)
        {
            await base.Select_datetime_second_component(async);

            AssertSql(
                @"SELECT EXTRACT(second FROM `o`.`OrderDate`)
FROM `Orders` AS `o`");
        }

        [Theory]
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
@filteredOrderIds1='10248'
@filteredOrderIds2='10249'
@filteredOrderIds3='10250'

SELECT `o0`.`OrderDate`, `o0`.`CustomerID`, `o2`.`Outer1`, `o2`.`Outer2`, `o2`.`Inner`, `o2`.`OrderDate`
FROM (
    SELECT DISTINCT `o`.`OrderDate`, `o`.`CustomerID`
    FROM `Orders` AS `o`
) AS `o0`
LEFT JOIN LATERAL (
    SELECT `o0`.`OrderDate` AS `Outer1`, `o0`.`CustomerID` AS `Outer2`, `o1`.`OrderID` AS `Inner`, `o1`.`OrderDate`
    FROM `Orders` AS `o1`
    WHERE ((`o1`.`CustomerID` = `o0`.`CustomerID`) OR (`o1`.`CustomerID` IS NULL AND (`o0`.`CustomerID` IS NULL))) AND `o1`.`OrderID` IN (@filteredOrderIds1, @filteredOrderIds2, @filteredOrderIds3)
) AS `o2` ON TRUE
ORDER BY `o0`.`OrderDate`, `o0`.`CustomerID`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash), Skip = "This test/query crashes MySQL 8 even with inlined parameters every single time. Could be related to LATERAL.")]
        public override async Task Correlated_collection_after_groupby_with_complex_projection_not_containing_original_identifier(bool async)
        {
            await base.Correlated_collection_after_groupby_with_complex_projection_not_containing_original_identifier(async);

            AssertSql(
                """
@filteredOrderIds1='10248'
@filteredOrderIds2='10249'
@filteredOrderIds3='10250'

SELECT `o2`.`CustomerID`, `o2`.`Complex`, `o3`.`Outer`, `o3`.`Inner`, `o3`.`OrderDate`
FROM (
    SELECT `o0`.`CustomerID`, `o0`.`Complex`
    FROM (
        SELECT `o`.`CustomerID`, EXTRACT(month FROM `o`.`OrderDate`) AS `Complex`
        FROM `Orders` AS `o`
    ) AS `o0`
    GROUP BY `o0`.`CustomerID`, `o0`.`Complex`
) AS `o2`
LEFT JOIN LATERAL (
    SELECT `o2`.`CustomerID` AS `Outer`, `o1`.`OrderID` AS `Inner`, `o1`.`OrderDate`
    FROM `Orders` AS `o1`
    WHERE ((`o1`.`CustomerID` = `o2`.`CustomerID`) OR (`o1`.`CustomerID` IS NULL AND (`o2`.`CustomerID` IS NULL))) AND `o1`.`OrderID` IN (@filteredOrderIds1, @filteredOrderIds2, @filteredOrderIds3)
) AS `o3` ON TRUE
ORDER BY `o2`.`CustomerID`, `o2`.`Complex`
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
                // EF Core 10 changed the error message
                Assert.Contains("Unable to translate a collection subquery", message);
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

        [Theory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault(async);
        }

        [Theory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_Take_and_FirstOrDefault_with_parameter(async);
        }

        [Theory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault(async);
        }

        [Theory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_followed_by_projection_of_length_property(async);
        }

        [Theory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(bool async)
        {
            return base.Project_single_element_from_collection_with_multiple_OrderBys_Take_and_FirstOrDefault_2(async);
        }

        [Theory(Skip = "issue #573")]
        public override Task Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(bool async)
        {
            return base.Project_single_element_from_collection_with_OrderBy_over_navigation_Take_and_FirstOrDefault(async);
        }

        [Theory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
        public override Task SelectMany_correlated_with_outer_2(bool async)
        {
            return base.SelectMany_correlated_with_outer_2(async);
        }

        // TODO:
        // [SupportedServerVersionCondition(ServerVersion.CrossApplySupportKey)ey)]
        [Theory(Skip = "Leads to a different result set in CI on Linux with MySQL 8.0.17. TODO: Needs investigation!")]
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

        [Theory]
        public override Task Member_binding_after_ctor_arguments_fails_with_client_eval(bool async)
        {
            return AssertTranslationFailed(() => base.Member_binding_after_ctor_arguments_fails_with_client_eval(async));
        }

        [Theory(Skip = "TODO: Seems to be a MySQL bug. Needs to be verified and reported, if not already.")]
        public override Task Take_on_top_level_and_on_collection_projection_with_outer_apply(bool async)
        {
            return base.Take_on_top_level_and_on_collection_projection_with_outer_apply(async);
        }

        [Theory(Skip = "Needs proper TimeSpan support, with a wider range than the current TIME mapping can provide.")]
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
