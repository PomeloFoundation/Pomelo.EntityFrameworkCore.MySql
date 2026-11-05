using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsQueryMySqlTest : ComplexNavigationsQueryRelationalTestBase<ComplexNavigationsQueryMySqlFixture>
    {
        public ComplexNavigationsQueryMySqlTest(ComplexNavigationsQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
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
                    .OrderBy(l0 => l0.Name) // <-- fix
                    .Take(1));

            AssertSql(
"""
@p='1'

SELECT `l0`.`Name`
FROM `LevelOne` AS `l`
INNER JOIN `LevelTwo` AS `l0` ON `l`.`Id` = `l0`.`OneToMany_Optional_Inverse2Id`
ORDER BY `l0`.`Name`
LIMIT @p
""");
        }

        public override async Task GroupJoin_client_method_in_OrderBy(bool async)
        {
            await AssertTranslationFailedWithDetails(
                () => base.GroupJoin_client_method_in_OrderBy(async),
                CoreStrings.QueryUnableToTranslateMethod(
                    "Microsoft.EntityFrameworkCore.Query.ComplexNavigationsQueryTestBase<Microting.EntityFrameworkCore.MySql.FunctionalTests.Query.ComplexNavigationsQueryMySqlFixture>",
                    "ClientMethodNullableInt"));

            AssertSql();
        }

        public override async Task Join_with_result_selector_returning_queryable_throws_validation_error(bool async)
        {
            // Expression cannot be used for return type. Issue #23302.
            await Assert.ThrowsAsync<ArgumentException>(
                () => base.Join_with_result_selector_returning_queryable_throws_validation_error(async));

            AssertSql();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override async Task Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(bool async)
        {
            // DefaultIfEmpty on child collection. Issue #19095.
            // Query now works correctly in MySQL 8.0.40+
            await base.Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(async);

            AssertSql(
                """
SELECT `s0`.`l1Name`, `s0`.`l2Name`, `s0`.`l3Name`
FROM `LevelOne` AS `l`
JOIN LATERAL (
    SELECT `s`.`l1Name`, `s`.`l2Name`, `s`.`l3Name`
    FROM (
        SELECT 1
    ) AS `e`
    LEFT JOIN (
        SELECT `l0`.`Id`
        FROM `LevelTwo` AS `l0`
        WHERE `l`.`Id` = `l0`.`OneToMany_Optional_Inverse2Id`
    ) AS `l1` ON TRUE
    LEFT JOIN `LevelThree` AS `l2` ON `l1`.`Id` = `l2`.`Id`
    JOIN LATERAL (
        SELECT `l`.`Name` AS `l1Name`, `l2`.`Name` AS `l2Name`, `l5`.`Name` AS `l3Name`
        FROM (
            SELECT 1
        ) AS `e0`
        LEFT JOIN (
            SELECT `l3`.`OneToOne_Optional_PK_Inverse4Id`
            FROM `LevelFour` AS `l3`
            WHERE `l2`.`Id` IS NOT NULL AND (`l2`.`Id` = `l3`.`OneToMany_Optional_Inverse4Id`)
        ) AS `l4` ON TRUE
        LEFT JOIN `LevelThree` AS `l5` ON `l4`.`OneToOne_Optional_PK_Inverse4Id` = `l5`.`Id`
    ) AS `s` ON TRUE
) AS `s0` ON TRUE
""");
        }

        public override async Task Method_call_on_optional_navigation_translates_to_null_conditional_properly_for_arguments(bool async)
        {
            await base.Method_call_on_optional_navigation_translates_to_null_conditional_properly_for_arguments(async);

            AssertSql(
"""
SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`, `l`.`OneToMany_Optional_Self_Inverse1Id`, `l`.`OneToMany_Required_Self_Inverse1Id`, `l`.`OneToOne_Optional_Self1Id`
FROM `LevelOne` AS `l`
LEFT JOIN `LevelTwo` AS `l0` ON `l`.`Id` = `l0`.`Level1_Optional_Id`
WHERE `l0`.`Name` IS NOT NULL AND (LEFT(`l0`.`Name`, CHAR_LENGTH(`l0`.`Name`)) = `l0`.`Name`)
""");
        }

        // CHECK: Flaky only on MySQL 5.7.
        [SupportedServerVersionCondition("8.0.0-mysql", "0.0.0-mariadb")]
        public override async Task Member_pushdown_with_multiple_collections(bool async)
        {
            await base.Member_pushdown_with_multiple_collections(async);

        AssertSql(
"""
SELECT (
    SELECT `l0`.`Name`
    FROM `LevelThree` AS `l0`
    WHERE (
        SELECT `l1`.`Id`
        FROM `LevelTwo` AS `l1`
        WHERE `l`.`Id` = `l1`.`OneToMany_Optional_Inverse2Id`
        ORDER BY `l1`.`Id`
        LIMIT 1) IS NOT NULL AND (((
        SELECT `l2`.`Id`
        FROM `LevelTwo` AS `l2`
        WHERE `l`.`Id` = `l2`.`OneToMany_Optional_Inverse2Id`
        ORDER BY `l2`.`Id`
        LIMIT 1) = `l0`.`OneToMany_Optional_Inverse3Id`) OR ((
        SELECT `l2`.`Id`
        FROM `LevelTwo` AS `l2`
        WHERE `l`.`Id` = `l2`.`OneToMany_Optional_Inverse2Id`
        ORDER BY `l2`.`Id`
        LIMIT 1) IS NULL AND (`l0`.`OneToMany_Optional_Inverse3Id` IS NULL)))
    ORDER BY `l0`.`Id`
    LIMIT 1)
FROM `LevelOne` AS `l`
""");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
