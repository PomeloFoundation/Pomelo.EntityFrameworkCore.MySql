using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class TPTFiltersInheritanceBulkUpdatesMySqlTest : TPTFiltersInheritanceBulkUpdatesTestBase<
    TPTFiltersInheritanceBulkUpdatesMySqlFixture>
{
    public TPTFiltersInheritanceBulkUpdatesMySqlTest(
        TPTFiltersInheritanceBulkUpdatesMySqlFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        ClearLog();
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Delete_where_hierarchy(bool async)
    {
        await base.Delete_where_hierarchy(async);

        AssertSql();
    }

    public override async Task Delete_where_hierarchy_derived(bool async)
    {
        await base.Delete_where_hierarchy_derived(async);

        AssertSql();
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
    FROM `Animals` AS `a`
    WHERE ((`a`.`CountryId` = 1) AND (`c`.`Id` = `a`.`CountryId`)) AND (`a`.`CountryId` > 0)) > 0
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
    FROM `Animals` AS `a`
    LEFT JOIN `Kiwi` AS `k` ON `a`.`Id` = `k`.`Id`
    WHERE (((`a`.`CountryId` = 1) AND (`c`.`Id` = `a`.`CountryId`)) AND `k`.`Id` IS NOT NULL) AND (`a`.`CountryId` > 0)) > 0
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
    FROM `Animals` AS `a`
    WHERE ((`a`.`CountryId` = 1) AND (`c`.`Id` = `a`.`CountryId`)) AND (`a`.`CountryId` > 0)) > 0
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
    FROM `Animals` AS `a`
    LEFT JOIN `Kiwi` AS `k` ON `a`.`Id` = `k`.`Id`
    WHERE (((`a`.`CountryId` = 1) AND (`c`.`Id` = `a`.`CountryId`)) AND `k`.`Id` IS NOT NULL) AND (`a`.`CountryId` > 0)) > 0
""");
    }

    public override async Task Update_where_keyless_entity_mapped_to_sql_query(bool async)
    {
        await base.Update_where_keyless_entity_mapped_to_sql_query(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_base_and_derived_types(bool async)
    {
        await base.Update_base_and_derived_types(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_base_type(bool async)
    {
        await base.Update_base_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Animals` AS `a`
SET `a`.`Name` = 'Animal'
WHERE (`a`.`CountryId` = 1) AND (`a`.`Name` = 'Great spotted kiwi')
""");
    }

    public override async Task Update_base_type_with_OfType(bool async)
    {
        await base.Update_base_type_with_OfType(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Animals` AS `a`
LEFT JOIN `Kiwi` AS `k` ON `a`.`Id` = `k`.`Id`
SET `a`.`Name` = 'NewBird'
WHERE (`a`.`CountryId` = 1) AND `k`.`Id` IS NOT NULL
""");
    }

    public override async Task Update_base_property_on_derived_type(bool async)
    {
        await base.Update_base_property_on_derived_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Animals` AS `a`
INNER JOIN `Birds` AS `b` ON `a`.`Id` = `b`.`Id`
INNER JOIN `Kiwi` AS `k` ON `a`.`Id` = `k`.`Id`
SET `a`.`Name` = 'SomeOtherKiwi'
WHERE `a`.`CountryId` = 1
""");
    }

    public override async Task Update_derived_property_on_derived_type(bool async)
    {
        await base.Update_derived_property_on_derived_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Animals` AS `a`
INNER JOIN `Birds` AS `b` ON `a`.`Id` = `b`.`Id`
INNER JOIN `Kiwi` AS `k` ON `a`.`Id` = `k`.`Id`
SET `k`.`FoundOn` = 0
WHERE `a`.`CountryId` = 1
""");
    }

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}






