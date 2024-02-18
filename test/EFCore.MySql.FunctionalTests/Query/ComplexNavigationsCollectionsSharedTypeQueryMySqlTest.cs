using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsCollectionsSharedTypeQueryMySqlTest : ComplexNavigationsCollectionsSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQueryMySqlTest.ComplexNavigationsSharedTypeQueryMySqlFixture>
    {
        public ComplexNavigationsCollectionsSharedTypeQueryMySqlTest(
            ComplexNavigationsSharedTypeQueryMySqlTest.ComplexNavigationsSharedTypeQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task SelectMany_with_Include1(bool async)
        {
            await base.SelectMany_with_Include1(async);

        AssertSql(
"""
SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Optional_Id`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Optional_Inverse2Id`, `l1`.`OneToMany_Required_Inverse2Id`, `l1`.`OneToOne_Optional_PK_Inverse2Id`, `l`.`Id`, `l3`.`Id`, `l3`.`Level2_Optional_Id`, `l3`.`Level2_Required_Id`, `l3`.`Level3_Name`, `l3`.`OneToMany_Optional_Inverse3Id`, `l3`.`OneToMany_Required_Inverse3Id`, `l3`.`OneToOne_Optional_PK_Inverse3Id`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Optional_Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Optional_Inverse2Id`, `l0`.`OneToMany_Required_Inverse2Id`, `l0`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `l1` ON `l`.`Id` = `l1`.`OneToMany_Optional_Inverse2Id`
LEFT JOIN (
    SELECT `l2`.`Id`, `l2`.`Level2_Optional_Id`, `l2`.`Level2_Required_Id`, `l2`.`Level3_Name`, `l2`.`OneToMany_Optional_Inverse3Id`, `l2`.`OneToMany_Required_Inverse3Id`, `l2`.`OneToOne_Optional_PK_Inverse3Id`
    FROM `Level1` AS `l2`
    WHERE `l2`.`Level2_Required_Id` IS NOT NULL AND (`l2`.`OneToMany_Required_Inverse3Id` IS NOT NULL)
) AS `l3` ON CASE
    WHEN (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l1`.`Id`
END = `l3`.`OneToMany_Optional_Inverse3Id`
ORDER BY `l`.`Id`, `l1`.`Id`
""");
        }

        public override async Task SelectMany_with_navigation_and_Distinct(bool async)
        {
            await base.SelectMany_with_navigation_and_Distinct(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`, `l1`.`Id`, `l3`.`Id`, `l3`.`OneToOne_Required_PK_Date`, `l3`.`Level1_Optional_Id`, `l3`.`Level1_Required_Id`, `l3`.`Level2_Name`, `l3`.`OneToMany_Optional_Inverse2Id`, `l3`.`OneToMany_Required_Inverse2Id`, `l3`.`OneToOne_Optional_PK_Inverse2Id`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT DISTINCT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Optional_Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Optional_Inverse2Id`, `l0`.`OneToMany_Required_Inverse2Id`, `l0`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `l1` ON `l`.`Id` = `l1`.`OneToMany_Optional_Inverse2Id`
LEFT JOIN (
    SELECT `l2`.`Id`, `l2`.`OneToOne_Required_PK_Date`, `l2`.`Level1_Optional_Id`, `l2`.`Level1_Required_Id`, `l2`.`Level2_Name`, `l2`.`OneToMany_Optional_Inverse2Id`, `l2`.`OneToMany_Required_Inverse2Id`, `l2`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l2`
    WHERE (`l2`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l2`.`Level1_Required_Id` IS NOT NULL)) AND `l2`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `l3` ON `l`.`Id` = `l3`.`OneToMany_Optional_Inverse2Id`
WHERE (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL
ORDER BY `l`.`Id`, `l1`.`Id`
""");
        }

        public override async Task Take_Select_collection_Take(bool async)
        {
            await base.Take_Select_collection_Take(async);

        AssertSql(
"""
@__p_0='1'

SELECT `l3`.`Id`, `l3`.`Name`, `s`.`Id`, `s`.`Name`, `s`.`Level1Id`, `s`.`Level2Id`, `s`.`Id0`, `s`.`Date`, `s`.`Name0`, `s`.`Id1`
FROM (
    SELECT `l`.`Id`, `l`.`Name`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0
) AS `l3`
LEFT JOIN LATERAL (
    SELECT CASE
        WHEN (`l2`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l2`.`Level1_Required_Id` IS NOT NULL)) AND `l2`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l2`.`Id`
    END AS `Id`, `l2`.`Level2_Name` AS `Name`, `l2`.`OneToMany_Required_Inverse2Id` AS `Level1Id`, `l2`.`Level1_Required_Id` AS `Level2Id`, `l1`.`Id` AS `Id0`, `l1`.`Date`, `l1`.`Name` AS `Name0`, `l2`.`Id` AS `Id1`, `l2`.`c`
    FROM (
        SELECT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Required_Inverse2Id`, CASE
            WHEN (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l0`.`Id`
        END AS `c`
        FROM `Level1` AS `l0`
        WHERE ((`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL) AND (`l3`.`Id` = `l0`.`OneToMany_Required_Inverse2Id`)
        ORDER BY CASE
            WHEN (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l0`.`Id`
        END
        LIMIT 3
    ) AS `l2`
    INNER JOIN `Level1` AS `l1` ON `l2`.`Level1_Required_Id` = `l1`.`Id`
) AS `s` ON TRUE
ORDER BY `l3`.`Id`, `s`.`c`, `s`.`Id1`
""");
        }

        public override async Task Skip_Take_Select_collection_Skip_Take(bool async)
        {
            await base.Skip_Take_Select_collection_Skip_Take(async);

        AssertSql(
"""
@__p_0='1'

SELECT `l3`.`Id`, `l3`.`Name`, `s`.`Id`, `s`.`Name`, `s`.`Level1Id`, `s`.`Level2Id`, `s`.`Id0`, `s`.`Date`, `s`.`Name0`, `s`.`Id1`
FROM (
    SELECT `l`.`Id`, `l`.`Name`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0 OFFSET @__p_0
) AS `l3`
LEFT JOIN LATERAL (
    SELECT CASE
        WHEN (`l2`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l2`.`Level1_Required_Id` IS NOT NULL)) AND `l2`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l2`.`Id`
    END AS `Id`, `l2`.`Level2_Name` AS `Name`, `l2`.`OneToMany_Required_Inverse2Id` AS `Level1Id`, `l2`.`Level1_Required_Id` AS `Level2Id`, `l1`.`Id` AS `Id0`, `l1`.`Date`, `l1`.`Name` AS `Name0`, `l2`.`Id` AS `Id1`, `l2`.`c`
    FROM (
        SELECT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Required_Inverse2Id`, CASE
            WHEN (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l0`.`Id`
        END AS `c`
        FROM `Level1` AS `l0`
        WHERE ((`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL) AND (`l3`.`Id` = `l0`.`OneToMany_Required_Inverse2Id`)
        ORDER BY CASE
            WHEN (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l0`.`Id`
        END
        LIMIT 3 OFFSET 1
    ) AS `l2`
    INNER JOIN `Level1` AS `l1` ON `l2`.`Level1_Required_Id` = `l1`.`Id`
) AS `s` ON TRUE
ORDER BY `l3`.`Id`, `s`.`c`, `s`.`Id1`
""");
        }

        public override async Task Skip_Take_on_grouping_element_inside_collection_projection(bool async)
        {
            await base.Skip_Take_on_grouping_element_inside_collection_projection(async);

        AssertSql(
"""
SELECT `l`.`Id`, `s`.`Date`, `s`.`Id`, `s`.`Date0`, `s`.`Name`
FROM `Level1` AS `l`
LEFT JOIN LATERAL (
    SELECT `l2`.`Date`, `l4`.`Id`, `l4`.`Date` AS `Date0`, `l4`.`Name`
    FROM (
        SELECT `l0`.`Date`
        FROM `Level1` AS `l0`
        WHERE (`l0`.`Name` = `l`.`Name`) OR (`l0`.`Name` IS NULL AND (`l`.`Name` IS NULL))
        GROUP BY `l0`.`Date`
    ) AS `l2`
    LEFT JOIN (
        SELECT `l3`.`Id`, `l3`.`Date`, `l3`.`Name`
        FROM (
            SELECT `l1`.`Id`, `l1`.`Date`, `l1`.`Name`, ROW_NUMBER() OVER(PARTITION BY `l1`.`Date` ORDER BY `l1`.`Name`) AS `row`
            FROM `Level1` AS `l1`
            WHERE (`l1`.`Name` = `l`.`Name`) OR (`l1`.`Name` IS NULL AND (`l`.`Name` IS NULL))
        ) AS `l3`
        WHERE (1 < `l3`.`row`) AND (`l3`.`row` <= 6)
    ) AS `l4` ON `l2`.`Date` = `l4`.`Date`
) AS `s` ON TRUE
ORDER BY `l`.`Id`, `s`.`Date`, `s`.`Date0`, `s`.`Name`
""");
        }

        public override async Task Skip_Take_Distinct_on_grouping_element(bool async)
        {
            await base.Skip_Take_Distinct_on_grouping_element(async);

        AssertSql(
"""
SELECT `l2`.`Date`, `l3`.`Id`, `l3`.`Date`, `l3`.`Name`
FROM (
    SELECT `l`.`Date`
    FROM `Level1` AS `l`
    GROUP BY `l`.`Date`
) AS `l2`
LEFT JOIN LATERAL (
    SELECT DISTINCT `l1`.`Id`, `l1`.`Date`, `l1`.`Name`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Date`, `l0`.`Name`
        FROM `Level1` AS `l0`
        WHERE `l2`.`Date` = `l0`.`Date`
        ORDER BY `l0`.`Name`
        LIMIT 5 OFFSET 1
    ) AS `l1`
) AS `l3` ON TRUE
ORDER BY `l2`.`Date`
""");
        }

        public override async Task Skip_Take_on_grouping_element_with_collection_include(bool async)
        {
            await base.Skip_Take_on_grouping_element_with_collection_include(async);

        AssertSql(
"""
SELECT `l2`.`Date`, `s`.`Id`, `s`.`Date`, `s`.`Name`, `s`.`Id0`, `s`.`OneToOne_Required_PK_Date`, `s`.`Level1_Optional_Id`, `s`.`Level1_Required_Id`, `s`.`Level2_Name`, `s`.`OneToMany_Optional_Inverse2Id`, `s`.`OneToMany_Required_Inverse2Id`, `s`.`OneToOne_Optional_PK_Inverse2Id`
FROM (
    SELECT `l`.`Date`
    FROM `Level1` AS `l`
    GROUP BY `l`.`Date`
) AS `l2`
LEFT JOIN LATERAL (
    SELECT `l3`.`Id`, `l3`.`Date`, `l3`.`Name`, `l4`.`Id` AS `Id0`, `l4`.`OneToOne_Required_PK_Date`, `l4`.`Level1_Optional_Id`, `l4`.`Level1_Required_Id`, `l4`.`Level2_Name`, `l4`.`OneToMany_Optional_Inverse2Id`, `l4`.`OneToMany_Required_Inverse2Id`, `l4`.`OneToOne_Optional_PK_Inverse2Id`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Date`, `l0`.`Name`
        FROM `Level1` AS `l0`
        WHERE `l2`.`Date` = `l0`.`Date`
        ORDER BY `l0`.`Name`
        LIMIT 5 OFFSET 1
    ) AS `l3`
    LEFT JOIN (
        SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Optional_Id`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Optional_Inverse2Id`, `l1`.`OneToMany_Required_Inverse2Id`, `l1`.`OneToOne_Optional_PK_Inverse2Id`
        FROM `Level1` AS `l1`
        WHERE (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL
    ) AS `l4` ON `l3`.`Id` = `l4`.`OneToMany_Optional_Inverse2Id`
) AS `s` ON TRUE
ORDER BY `l2`.`Date`, `s`.`Name`, `s`.`Id`
""");
        }

        public override async Task Skip_Take_on_grouping_element_with_reference_include(bool async)
        {
            await base.Skip_Take_on_grouping_element_with_reference_include(async);

        AssertSql(
"""
SELECT `l4`.`Date`, `s`.`Id`, `s`.`Date`, `s`.`Name`, `s`.`Id0`, `s`.`OneToOne_Required_PK_Date`, `s`.`Level1_Optional_Id`, `s`.`Level1_Required_Id`, `s`.`Level2_Name`, `s`.`OneToMany_Optional_Inverse2Id`, `s`.`OneToMany_Required_Inverse2Id`, `s`.`OneToOne_Optional_PK_Inverse2Id`
FROM (
    SELECT `l`.`Date`
    FROM `Level1` AS `l`
    GROUP BY `l`.`Date`
) AS `l4`
LEFT JOIN LATERAL (
    SELECT `l2`.`Id`, `l2`.`Date`, `l2`.`Name`, `l3`.`Id` AS `Id0`, `l3`.`OneToOne_Required_PK_Date`, `l3`.`Level1_Optional_Id`, `l3`.`Level1_Required_Id`, `l3`.`Level2_Name`, `l3`.`OneToMany_Optional_Inverse2Id`, `l3`.`OneToMany_Required_Inverse2Id`, `l3`.`OneToOne_Optional_PK_Inverse2Id`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Date`, `l0`.`Name`
        FROM `Level1` AS `l0`
        WHERE `l4`.`Date` = `l0`.`Date`
        ORDER BY `l0`.`Name`
        LIMIT 5 OFFSET 1
    ) AS `l2`
    LEFT JOIN (
        SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Optional_Id`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Optional_Inverse2Id`, `l1`.`OneToMany_Required_Inverse2Id`, `l1`.`OneToOne_Optional_PK_Inverse2Id`
        FROM `Level1` AS `l1`
        WHERE (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL
    ) AS `l3` ON `l2`.`Id` = `l3`.`Level1_Optional_Id`
) AS `s` ON TRUE
ORDER BY `l4`.`Date`, `s`.`Name`, `s`.`Id`
""");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
