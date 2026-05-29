using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class ComplexTypeBulkUpdatesMySqlTest : ComplexTypeBulkUpdatesRelationalTestBase<
    ComplexTypeBulkUpdatesMySqlTest.ComplexTypeBulkUpdatesMySqlFixture>
{
    public ComplexTypeBulkUpdatesMySqlTest(ComplexTypeBulkUpdatesMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public override async Task Delete_entity_type_with_complex_type(bool async)
    {
        await base.Delete_entity_type_with_complex_type(async);

        AssertSql(
"""
DELETE `c`
FROM `Customer` AS `c`
WHERE `c`.`Name` = 'Monty Elias'
""");
    }

    public override async Task Delete_complex_type(bool async)
    {
        await base.Delete_complex_type(async);

        AssertSql();
    }

    public override async Task Update_property_inside_complex_type(bool async)
    {
        await base.Update_property_inside_complex_type(async);

        AssertExecuteUpdateSql(
            """
@p='12345'

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_ZipCode` = @p
WHERE `c`.`ShippingAddress_ZipCode` = 7728
""");
    }

    public override async Task Update_property_inside_nested_complex_type(bool async)
    {
        await base.Update_property_inside_nested_complex_type(async);

        AssertExecuteUpdateSql(
            """
@p='United States Modified' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_Country_FullName` = @p
WHERE `c`.`ShippingAddress_Country_Code` = 'US'
""");
    }

    public override async Task Update_multiple_properties_inside_multiple_complex_types_and_on_entity_type(bool async)
    {
        await base.Update_multiple_properties_inside_multiple_complex_types_and_on_entity_type(async);

        AssertExecuteUpdateSql(
            """
@p='54321'

UPDATE `Customer` AS `c`
SET `c`.`Name` = CONCAT(`c`.`Name`, 'Modified'),
    `c`.`ShippingAddress_ZipCode` = `c`.`BillingAddress_ZipCode`,
    `c`.`BillingAddress_ZipCode` = @p
WHERE `c`.`ShippingAddress_ZipCode` = 7728
""");
    }

    public override async Task Update_projected_complex_type(bool async)
    {
        await base.Update_projected_complex_type(async);

        AssertExecuteUpdateSql(
            """
@p='12345'

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_ZipCode` = @p
""");
    }

    public override async Task Update_multiple_projected_complex_types_via_anonymous_type(bool async)
    {
        await base.Update_multiple_projected_complex_types_via_anonymous_type(async);

        AssertExecuteUpdateSql(
            """
@p='54321'

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_ZipCode` = `c`.`BillingAddress_ZipCode`,
    `c`.`BillingAddress_ZipCode` = @p
""");
    }

    public override async Task Update_projected_complex_type_via_OrderBy_Skip(bool async)
    {
        await base.Update_projected_complex_type_via_OrderBy_Skip(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_complex_type_to_parameter(bool async)
    {
        await base.Update_complex_type_to_parameter(async);

        AssertSql(
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
            //
            """
@complex_type_p_AddressLine1='New AddressLine1' (Size = 4000)
@complex_type_p_AddressLine2='New AddressLine2' (Size = 4000)
@complex_type_p_Tags='["new_tag1","new_tag2"]' (Size = 4000)
@complex_type_p_ZipCode='99999' (Nullable = true)
@complex_type_p_Country_Code='FR' (Size = 4000)
@complex_type_p_Country_FullName='France' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_AddressLine1` = @complex_type_p_AddressLine1,
    `c`.`ShippingAddress_AddressLine2` = @complex_type_p_AddressLine2,
    `c`.`ShippingAddress_Tags` = @complex_type_p_Tags,
    `c`.`ShippingAddress_ZipCode` = @complex_type_p_ZipCode,
    `c`.`ShippingAddress_Country_Code` = @complex_type_p_Country_Code,
    `c`.`ShippingAddress_Country_FullName` = @complex_type_p_Country_FullName
""",
            //
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_nested_complex_type_to_parameter(bool async)
    {
        await base.Update_nested_complex_type_to_parameter(async);

        AssertSql(
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
            //
            """
@complex_type_p_Code='FR' (Size = 4000)
@complex_type_p_FullName='France' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_Country_Code` = @complex_type_p_Code,
    `c`.`ShippingAddress_Country_FullName` = @complex_type_p_FullName
""",
            //
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_another_database_complex_type(bool async)
    {
        await base.Update_complex_type_to_another_database_complex_type(async);

        AssertSql(
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
            //
            """
UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_AddressLine1` = `c`.`BillingAddress_AddressLine1`,
    `c`.`ShippingAddress_AddressLine2` = `c`.`BillingAddress_AddressLine2`,
    `c`.`ShippingAddress_Tags` = `c`.`BillingAddress_Tags`,
    `c`.`ShippingAddress_ZipCode` = `c`.`BillingAddress_ZipCode`,
    `c`.`ShippingAddress_Country_Code` = `c`.`ShippingAddress_Country_Code`,
    `c`.`ShippingAddress_Country_FullName` = `c`.`ShippingAddress_Country_FullName`
""",
            //
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_inline_without_lambda(bool async)
    {
        await base.Update_complex_type_to_inline_without_lambda(async);

        AssertSql(
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
            //
            """
@complex_type_p_AddressLine1='New AddressLine1' (Size = 4000)
@complex_type_p_AddressLine2='New AddressLine2' (Size = 4000)
@complex_type_p_Tags='["new_tag1","new_tag2"]' (Size = 4000)
@complex_type_p_ZipCode='99999' (Nullable = true)
@complex_type_p_Country_Code='FR' (Size = 4000)
@complex_type_p_Country_FullName='France' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_AddressLine1` = @complex_type_p_AddressLine1,
    `c`.`ShippingAddress_AddressLine2` = @complex_type_p_AddressLine2,
    `c`.`ShippingAddress_Tags` = @complex_type_p_Tags,
    `c`.`ShippingAddress_ZipCode` = @complex_type_p_ZipCode,
    `c`.`ShippingAddress_Country_Code` = @complex_type_p_Country_Code,
    `c`.`ShippingAddress_Country_FullName` = @complex_type_p_Country_FullName
""",
            //
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_inline_with_lambda(bool async)
    {
        await base.Update_complex_type_to_inline_with_lambda(async);

        AssertSql(
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
            //
            """
UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_AddressLine1` = 'New AddressLine1',
    `c`.`ShippingAddress_AddressLine2` = 'New AddressLine2',
    `c`.`ShippingAddress_Tags` = '["new_tag1","new_tag2"]',
    `c`.`ShippingAddress_ZipCode` = 99999,
    `c`.`ShippingAddress_Country_Code` = 'FR',
    `c`.`ShippingAddress_Country_FullName` = 'France'
""",
            //
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_another_database_complex_type_with_subquery(bool async)
    {
        await base.Update_complex_type_to_another_database_complex_type_with_subquery(async);

        AssertSql(
            """
@p='1'

SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
ORDER BY `c`.`Id`
LIMIT 18446744073709551610 OFFSET @p
""",
            //
            """
@p='1'

UPDATE `Customer` AS `c0`
INNER JOIN (
    SELECT `c`.`Id`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
    FROM `Customer` AS `c`
    ORDER BY `c`.`Id`
    LIMIT 18446744073709551610 OFFSET @p
) AS `c1` ON `c0`.`Id` = `c1`.`Id`
SET `c0`.`ShippingAddress_AddressLine1` = `c1`.`BillingAddress_AddressLine1`,
    `c0`.`ShippingAddress_AddressLine2` = `c1`.`BillingAddress_AddressLine2`,
    `c0`.`ShippingAddress_Tags` = `c1`.`BillingAddress_Tags`,
    `c0`.`ShippingAddress_ZipCode` = `c1`.`BillingAddress_ZipCode`,
    `c0`.`ShippingAddress_Country_Code` = `c1`.`ShippingAddress_Country_Code`,
    `c0`.`ShippingAddress_Country_FullName` = `c1`.`ShippingAddress_Country_FullName`
""",
            //
            """
@p='1'

SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
ORDER BY `c`.`Id`
LIMIT 18446744073709551610 OFFSET @p
""");
    }

    public override async Task Update_collection_inside_complex_type(bool async)
    {
        await base.Update_collection_inside_complex_type(async);

        AssertSql(
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
            //
            """
@p='["new_tag1","new_tag2"]' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_Tags` = @p
""",
            //
            """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`OptionalAddress_AddressLine1`, `c`.`OptionalAddress_AddressLine2`, `c`.`OptionalAddress_Tags`, `c`.`OptionalAddress_ZipCode`, `c`.`OptionalAddress_Country_Code`, `c`.`OptionalAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
    {
        MySqlTestHelpers.AssertAllMethodsOverridden(GetType());
    }

    private void AssertExecuteUpdateSql(params string[] expected)
    {
        Fixture.TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
    }

    private void AssertSql(params string[] expected)
    {
        Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }

    protected void ClearLog()
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public class ComplexTypeBulkUpdatesMySqlFixture : ComplexTypeBulkUpdatesRelationalFixtureBase, ITestSqlLoggerFactory
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
