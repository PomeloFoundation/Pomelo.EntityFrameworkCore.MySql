using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsSharedTypeQueryMySqlTest : ComplexNavigationsSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQueryMySqlTest.ComplexNavigationsSharedTypeQueryMySqlFixture>
    {
        // ReSharper disable once UnusedParameter.Local
        public ComplexNavigationsSharedTypeQueryMySqlTest(
            ComplexNavigationsSharedTypeQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            // Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
        }

        public override Task Distinct_take_without_orderby(bool async)
        {
            return AssertQuery(
                async,
                ss => from l1 in ss.Set<Level1>()
                    where l1.Id < 3
                    select (from l3 in ss.Set<Level3>()
                        select l3).Distinct().OrderBy(e => e.Id).Take(1).FirstOrDefault().Name); // Apply OrderBy before Skip
        }

        public override Task Distinct_skip_without_orderby(bool async)
        {
            return AssertQuery(
                async,
                ss => from l1 in ss.Set<Level1>()
                    where l1.Id < 3
                    select (from l3 in ss.Set<Level3>()
                        select l3).Distinct().OrderBy(e => e.Id).Skip(1).FirstOrDefault().Name); // Apply OrderBy before Skip
        }

        public override async Task SelectMany_subquery_with_custom_projection(bool async)
        {
            // TODO: Fix test in EF Core upstream.
            //           ORDER BY `l`.`Id`
            //       is ambiguous, since all 5 queried rows have an `Id` of `1`.
            await AssertQuery(
                async,
                ss => ss.Set<Level1>()/*.OrderBy(l1 => l1.Id)*/.SelectMany( // <-- has no effect anymore
                        l1 => l1.OneToMany_Optional1.Select(
                            l2 => new { l2.Name }))
                    .OrderBy(e => e.Name) // <-- fix
                    .Take(1));

            AssertSql(
                @"@__p_0='1'

SELECT `t`.`Name`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT `l0`.`Level2_Name` AS `Name`, `l0`.`OneToMany_Optional_Inverse2Id`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`OneToMany_Optional_Inverse2Id`
ORDER BY `t`.`Name`
LIMIT @__p_0");
        }

        public override async Task Method_call_on_optional_navigation_translates_to_null_conditional_properly_for_arguments(bool async)
        {
            await base.Method_call_on_optional_navigation_translates_to_null_conditional_properly_for_arguments(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`
FROM `Level1` AS `l`
LEFT JOIN (
    SELECT `l0`.`Level1_Optional_Id`, `l0`.`Level2_Name`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`Level1_Optional_Id`
WHERE `t`.`Level2_Name` IS NOT NULL AND (LEFT(`t`.`Level2_Name`, CHAR_LENGTH(`t`.`Level2_Name`)) = `t`.`Level2_Name`)
""");
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/26104")]
        public override Task GroupBy_aggregate_where_required_relationship(bool async)
            => base.GroupBy_aggregate_where_required_relationship(async);

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/26104")]
        public override Task GroupBy_aggregate_where_required_relationship_2(bool async)
            => base.GroupBy_aggregate_where_required_relationship_2(async);

        public override async Task GroupJoin_client_method_in_OrderBy(bool async)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await base.GroupJoin_client_method_in_OrderBy(async));

            AssertSql();
        }

        public override async Task Join_with_result_selector_returning_queryable_throws_validation_error(bool async)
        {
            // Expression cannot be used for return type. Issue #23302.
            await Assert.ThrowsAsync<ArgumentException>(
                () => base.Join_with_result_selector_returning_queryable_throws_validation_error(async));

            AssertSql();
        }

        [ConditionalTheory(Skip = "Does not throw an EqualException, but still does not work.")]
        public override async Task Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(bool async)
        {
            // DefaultIfEmpty on child collection. Issue #19095.
            await Assert.ThrowsAsync<EqualException>(
                async () => await base.Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(async));

            AssertSql();
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class ComplexNavigationsSharedTypeQueryMySqlFixture : ComplexNavigationsSharedTypeQueryRelationalFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
