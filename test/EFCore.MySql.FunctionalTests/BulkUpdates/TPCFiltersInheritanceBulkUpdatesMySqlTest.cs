using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class TPCFiltersInheritanceBulkUpdatesMySqlTest : TPCFiltersInheritanceBulkUpdatesTestBase<
    TPCFiltersInheritanceBulkUpdatesMySqlFixture>
{
    public TPCFiltersInheritanceBulkUpdatesMySqlTest(
        TPCFiltersInheritanceBulkUpdatesMySqlFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        ClearLog();
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Delete_where_hierarchy(bool async)
    {
        await base.Delete_where_hierarchy(async);

        AssertSql();
    }

    public override async Task Delete_where_hierarchy_derived(bool async)
    {
        await base.Delete_where_hierarchy_derived(async);

        AssertSql(
"""
DELETE `k`
FROM `Kiwi` AS `k`
WHERE (`k`.`CountryId` = 1) AND (`k`.`Name` = 'Great spotted kiwi')
""");
    }

    public override async Task Delete_where_using_hierarchy(bool async)
    {
        await base.Delete_where_using_hierarchy(async);

        AssertSql(
"""
DELETE `c`
FROM `Countries` AS `c`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `e`.`CountryId`
        FROM `Eagle` AS `e`
        UNION ALL
        SELECT `k`.`CountryId`
        FROM `Kiwi` AS `k`
    ) AS `u`
    WHERE ((`u`.`CountryId` = 1) AND (`c`.`Id` = `u`.`CountryId`)) AND (`u`.`CountryId` > 0)) > 0
""");
    }

    public override async Task Delete_where_using_hierarchy_derived(bool async)
    {
        await base.Delete_where_using_hierarchy_derived(async);

        AssertSql(
"""
DELETE `c`
FROM `Countries` AS `c`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `k`.`CountryId`
        FROM `Kiwi` AS `k`
    ) AS `u`
    WHERE ((`u`.`CountryId` = 1) AND (`c`.`Id` = `u`.`CountryId`)) AND (`u`.`CountryId` > 0)) > 0
""");
    }

    public override async Task Delete_where_keyless_entity_mapped_to_sql_query(bool async)
    {
        await base.Delete_where_keyless_entity_mapped_to_sql_query(async);

        AssertSql();
    }

    public override async Task Delete_where_hierarchy_subquery(bool async)
    {
        await base.Delete_where_hierarchy_subquery(async);

        AssertSql();
    }

    public override async Task Delete_GroupBy_Where_Select_First(bool async)
    {
        await base.Delete_GroupBy_Where_Select_First(async);

        AssertSql();
    }

    public override async Task Delete_GroupBy_Where_Select_First_2(bool async)
    {
        await base.Delete_GroupBy_Where_Select_First_2(async);

        AssertSql();
    }

    public override async Task Delete_GroupBy_Where_Select_First_3(bool async)
    {
        await base.Delete_GroupBy_Where_Select_First_3(async);

        AssertSql();
    }

    public override async Task Update_where_hierarchy_subquery(bool async)
    {
        await base.Update_where_hierarchy_subquery(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_where_using_hierarchy(bool async)
    {
        await base.Update_where_using_hierarchy(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Countries` AS `c`
SET `c`.`Name` = 'Monovia'
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `e`.`CountryId`
        FROM `Eagle` AS `e`
        UNION ALL
        SELECT `k`.`CountryId`
        FROM `Kiwi` AS `k`
    ) AS `u`
    WHERE ((`u`.`CountryId` = 1) AND (`c`.`Id` = `u`.`CountryId`)) AND (`u`.`CountryId` > 0)) > 0
""");
    }

    public override async Task Update_where_using_hierarchy_derived(bool async)
    {
        await base.Update_where_using_hierarchy_derived(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Countries` AS `c`
SET `c`.`Name` = 'Monovia'
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `k`.`CountryId`
        FROM `Kiwi` AS `k`
    ) AS `u`
    WHERE ((`u`.`CountryId` = 1) AND (`c`.`Id` = `u`.`CountryId`)) AND (`u`.`CountryId` > 0)) > 0
""");
    }

    public override async Task Update_where_keyless_entity_mapped_to_sql_query(bool async)
    {
        await base.Update_where_keyless_entity_mapped_to_sql_query(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_base_type(bool async)
    {
        await base.Update_base_type(async);

        AssertSql(
"""
SELECT `u`.`Id`, `u`.`CountryId`, `u`.`Name`, `u`.`Species`, `u`.`EagleId`, `u`.`IsFlightless`, `u`.`Group`, `u`.`FoundOn`, `u`.`Discriminator`
FROM (
    SELECT `e`.`Id`, `e`.`CountryId`, `e`.`Name`, `e`.`Species`, `e`.`EagleId`, `e`.`IsFlightless`, `e`.`Group`, NULL AS `FoundOn`, 'Eagle' AS `Discriminator`
    FROM `Eagle` AS `e`
    UNION ALL
    SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, NULL AS `Group`, `k`.`FoundOn`, 'Kiwi' AS `Discriminator`
    FROM `Kiwi` AS `k`
) AS `u`
WHERE (`u`.`CountryId` = 1) AND (`u`.`Name` = 'Great spotted kiwi')
""");
    }

    public override async Task Update_base_type_with_OfType(bool async)
    {
        await base.Update_base_type_with_OfType(async);

        AssertSql(
"""
SELECT `u`.`Id`, `u`.`CountryId`, `u`.`Name`, `u`.`Species`, `u`.`EagleId`, `u`.`IsFlightless`, `u`.`FoundOn`, `u`.`Discriminator`
FROM (
    SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`, 'Kiwi' AS `Discriminator`
    FROM `Kiwi` AS `k`
) AS `u`
WHERE `u`.`CountryId` = 1
""");
    }

    public override async Task Update_base_property_on_derived_type(bool async)
    {
        await base.Update_base_property_on_derived_type(async);

        AssertSql(
"""
SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`
FROM `Kiwi` AS `k`
WHERE `k`.`CountryId` = 1
""",
                //
                """
UPDATE `Kiwi` AS `k`
SET `k`.`Name` = 'SomeOtherKiwi'
WHERE `k`.`CountryId` = 1
""",
                //
                """
SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`
FROM `Kiwi` AS `k`
WHERE `k`.`CountryId` = 1
""");
    }

    public override async Task Update_derived_property_on_derived_type(bool async)
    {
        await base.Update_derived_property_on_derived_type(async);

        AssertSql(
"""
SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`
FROM `Kiwi` AS `k`
WHERE `k`.`CountryId` = 1
""",
                //
                """
UPDATE `Kiwi` AS `k`
SET `k`.`FoundOn` = 0
WHERE `k`.`CountryId` = 1
""",
                //
                """
SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`
FROM `Kiwi` AS `k`
WHERE `k`.`CountryId` = 1
""");
    }

    public override async Task Update_base_and_derived_types(bool async)
    {
        await base.Update_base_and_derived_types(async);

        AssertSql(
"""
SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`
FROM `Kiwi` AS `k`
WHERE `k`.`CountryId` = 1
""",
                //
                """
UPDATE `Kiwi` AS `k`
SET `k`.`FoundOn` = 0,
    `k`.`Name` = 'Kiwi'
WHERE `k`.`CountryId` = 1
""",
                //
                """
SELECT `k`.`Id`, `k`.`CountryId`, `k`.`Name`, `k`.`Species`, `k`.`EagleId`, `k`.`IsFlightless`, `k`.`FoundOn`
FROM `Kiwi` AS `k`
WHERE `k`.`CountryId` = 1
""");
    }

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}
