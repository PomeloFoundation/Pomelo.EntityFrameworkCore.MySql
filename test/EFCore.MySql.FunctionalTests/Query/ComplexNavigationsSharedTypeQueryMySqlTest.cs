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
"""
@__p_0='1'

SELECT `l1`.`Name`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT `l0`.`Level2_Name` AS `Name`, `l0`.`OneToMany_Optional_Inverse2Id`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `l1` ON `l`.`Id` = `l1`.`OneToMany_Optional_Inverse2Id`
ORDER BY `l1`.`Name`
LIMIT @__p_0
""");
        }

        public override async Task Max_in_multi_level_nested_subquery(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Level1>()
                    .OrderBy(l1 => l1.Id) // <-- ensure order is deterministic
                    .Take(2).Select(x => new
                {
                    x.Id,
                    LevelTwos = x.OneToMany_Optional1.AsQueryable().Select(xx => new
                    {
                        xx.Id,
                        LevelThree = new
                        {
                            xx.OneToOne_Required_FK2.Id,
                            LevelFour = new
                            {
                                xx.OneToOne_Required_FK2.OneToOne_Required_FK3.Id,
                                Result = (xx.OneToOne_Required_FK2.OneToMany_Optional3.Max(xxx => (int?)xxx.Id) ?? 0) > 1
                            }
                        }
                    }).ToList()
                }),
                elementSorter: e => e.Id,
                elementAsserter: (e, a) =>
                {
                    AssertEqual(e.Id, a.Id);
                    AssertCollection(
                        e.LevelTwos,
                        a.LevelTwos,
                        elementSorter: ee => ee.Id,
                        elementAsserter: (ee, aa) =>
                        {
                            AssertEqual(ee.Id, aa.Id);
                            AssertEqual(ee.LevelThree.Id, aa.LevelThree.Id);
                            AssertEqual(ee.LevelThree.LevelFour.Id, aa.LevelThree.LevelFour.Id);
                            AssertEqual(ee.LevelThree.LevelFour.Result, aa.LevelThree.LevelFour.Result);
                        });
                });

            AssertSql(
"""
@__p_0='2'

SELECT `l6`.`Id`, `s`.`Id`, `s`.`Id0`, `s`.`Id1`, `s`.`Result`, `s`.`Id2`, `s`.`Id3`, `s`.`Id4`
FROM (
    SELECT `l`.`Id`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0
) AS `l6`
LEFT JOIN (
    SELECT CASE
        WHEN (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l0`.`Id`
    END AS `Id`, CASE
        WHEN `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL) THEN `l2`.`Id`
    END AS `Id0`, CASE
        WHEN `l4`.`Level3_Required_Id` IS NOT NULL AND (`l4`.`OneToMany_Required_Inverse4Id` IS NOT NULL) THEN `l4`.`Id`
    END AS `Id1`, COALESCE((
        SELECT MAX(CASE
            WHEN `l5`.`Level3_Required_Id` IS NOT NULL AND (`l5`.`OneToMany_Required_Inverse4Id` IS NOT NULL) THEN `l5`.`Id`
        END)
        FROM `Level1` AS `l5`
        WHERE (`l5`.`Level3_Required_Id` IS NOT NULL AND (`l5`.`OneToMany_Required_Inverse4Id` IS NOT NULL)) AND (CASE
            WHEN `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL) THEN `l2`.`Id`
        END IS NOT NULL AND ((CASE
            WHEN `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL) THEN `l2`.`Id`
        END = `l5`.`OneToMany_Optional_Inverse4Id`) OR (CASE
            WHEN `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL) THEN `l2`.`Id`
        END IS NULL AND (`l5`.`OneToMany_Optional_Inverse4Id` IS NULL))))), 0) > 1 AS `Result`, `l0`.`Id` AS `Id2`, `l2`.`Id` AS `Id3`, `l4`.`Id` AS `Id4`, `l0`.`OneToMany_Optional_Inverse2Id`
    FROM `Level1` AS `l0`
    LEFT JOIN (
        SELECT `l1`.`Id`, `l1`.`Level2_Required_Id`, `l1`.`OneToMany_Required_Inverse3Id`
        FROM `Level1` AS `l1`
        WHERE `l1`.`Level2_Required_Id` IS NOT NULL AND (`l1`.`OneToMany_Required_Inverse3Id` IS NOT NULL)
    ) AS `l2` ON CASE
        WHEN (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l0`.`Id`
    END = `l2`.`Level2_Required_Id`
    LEFT JOIN (
        SELECT `l3`.`Id`, `l3`.`Level3_Required_Id`, `l3`.`OneToMany_Required_Inverse4Id`
        FROM `Level1` AS `l3`
        WHERE `l3`.`Level3_Required_Id` IS NOT NULL AND (`l3`.`OneToMany_Required_Inverse4Id` IS NOT NULL)
    ) AS `l4` ON CASE
        WHEN `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL) THEN `l2`.`Id`
    END = `l4`.`Level3_Required_Id`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `s` ON `l6`.`Id` = `s`.`OneToMany_Optional_Inverse2Id`
ORDER BY `l6`.`Id`, `s`.`Id2`, `s`.`Id3`
""");
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
) AS `l1` ON `l`.`Id` = `l1`.`Level1_Optional_Id`
WHERE `l1`.`Level2_Name` IS NOT NULL AND (LEFT(`l1`.`Level2_Name`, CHAR_LENGTH(`l1`.`Level2_Name`)) = `l1`.`Level2_Name`)
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
