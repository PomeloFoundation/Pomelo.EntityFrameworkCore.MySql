using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class NonSharedModelBulkUpdatesMySqlTest : NonSharedModelBulkUpdatesRelationalTestBase
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Delete_aggregate_root_when_eager_loaded_owned_collection(bool async)
    {
        await base.Delete_aggregate_root_when_eager_loaded_owned_collection(async);

        AssertSql(
"""
DELETE `o`
FROM `Owner` AS `o`
""");
    }

    public override async Task Delete_aggregate_root_when_table_sharing_with_owned(bool async)
    {
        await base.Delete_aggregate_root_when_table_sharing_with_owned(async);

        AssertSql(
"""
DELETE `o`
FROM `Owner` AS `o`
""");
    }

    public override async Task Delete_aggregate_root_when_table_sharing_with_non_owned_throws(bool async)
    {
        await base.Delete_aggregate_root_when_table_sharing_with_non_owned_throws(async);

        AssertSql();
    }

    public override async Task Delete_predicate_based_on_optional_navigation(bool async)
    {
        await base.Delete_predicate_based_on_optional_navigation(async);

        AssertSql(
"""
DELETE `p`
FROM `Posts` AS `p`
LEFT JOIN `Blogs` AS `b` ON `p`.`BlogId` = `b`.`Id`
WHERE `b`.`Title` LIKE 'Arthur%'
""");
    }

    public override async Task Update_non_owned_property_on_entity_with_owned(bool async)
    {
        await base.Update_non_owned_property_on_entity_with_owned(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
SET `o`.`Title` = 'SomeValue'
""");
    }

    public override async Task Update_non_owned_property_on_entity_with_owned2(bool async)
    {
        await base.Update_non_owned_property_on_entity_with_owned2(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
SET `o`.`Title` = CONCAT(COALESCE(`o`.`Title`, ''), '_Suffix')
""");
    }

    public override async Task Update_owned_and_non_owned_properties_with_table_sharing(bool async)
    {
        await base.Update_owned_and_non_owned_properties_with_table_sharing(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
SET `o`.`OwnedReference_Number` = CHAR_LENGTH(`o`.`Title`),
    `o`.`Title` = COALESCE(CAST(`o`.`OwnedReference_Number` AS char), '')
""");
    }

    public override async Task Update_main_table_in_entity_with_entity_splitting(bool async)
    {
        await base.Update_main_table_in_entity_with_entity_splitting(async);

        AssertSql(
"""
UPDATE `Blogs` AS `b`
SET `b`.`CreationTimestamp` = TIMESTAMP '2020-01-01 00:00:00'
""");
    }

    public override async Task Update_non_main_table_in_entity_with_entity_splitting(bool async)
    {
        await base.Update_non_main_table_in_entity_with_entity_splitting(async);

        AssertSql(
"""
UPDATE `Blogs` AS `b`
INNER JOIN `BlogsPart1` AS `b0` ON `b`.`Id` = `b0`.`Id`
SET `b0`.`Rating` = CHAR_LENGTH(`b0`.`Title`),
    `b0`.`Title` = CAST(`b0`.`Rating` AS char)
""");
    }

    public override async Task Delete_entity_with_auto_include(bool async)
    {
        await base.Delete_entity_with_auto_include(async);

        AssertSql(
"""
DELETE `c`
FROM `Context30572_Principal` AS `c`
LEFT JOIN `Context30572_Dependent` AS `c0` ON `c`.`DependentId` = `c0`.`Id`
""");
    }

    public override async Task Update_with_alias_uniquification_in_setter_subquery(bool async)
    {
        await base.Update_with_alias_uniquification_in_setter_subquery(async);

        AssertSql(
"""
UPDATE `Orders` AS `o`
SET `o`.`Total` = (
    SELECT COALESCE(SUM(`o0`.`Amount`), 0)
    FROM `OrderProduct` AS `o0`
    WHERE `o`.`Id` = `o0`.`OrderId`)
WHERE `o`.`Id` = 1
""");
    }

    public override Task Delete_with_owned_collection_and_non_natively_translatable_query(bool async)
        => Assert.ThrowsAsync<MySqlException>(() =>base.Delete_with_owned_collection_and_non_natively_translatable_query(async));

    public override async Task Update_non_owned_property_on_entity_with_owned_in_join(bool async)
    {
        await base.Update_non_owned_property_on_entity_with_owned_in_join(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
INNER JOIN `Owner` AS `o0` ON `o`.`Id` = `o0`.`Id`
SET `o`.`Title` = 'NewValue'
""");
    }

    public override async Task Replace_ColumnExpression_in_column_setter(bool async)
    {
        await base.Replace_ColumnExpression_in_column_setter(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
INNER JOIN `OwnedCollection` AS `o0` ON `o`.`Id` = `o0`.`OwnerId`
SET `o0`.`Value` = 'SomeValue'
""");
    }

    private void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}
