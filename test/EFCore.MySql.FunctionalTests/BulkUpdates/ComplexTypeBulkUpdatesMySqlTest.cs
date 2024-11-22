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
UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_ZipCode` = 12345
WHERE `c`.`ShippingAddress_ZipCode` = 7728
""");
    }

    public override async Task Update_property_inside_nested_complex_type(bool async)
    {
        await base.Update_property_inside_nested_complex_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_Country_FullName` = 'United States Modified'
WHERE `c`.`ShippingAddress_Country_Code` = 'US'
""");
    }

    public override async Task Update_multiple_properties_inside_multiple_complex_types_and_on_entity_type(bool async)
    {
        await base.Update_multiple_properties_inside_multiple_complex_types_and_on_entity_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customer` AS `c`
SET `c`.`BillingAddress_ZipCode` = 54321,
    `c`.`ShippingAddress_ZipCode` = `c`.`BillingAddress_ZipCode`,
    `c`.`Name` = CONCAT(`c`.`Name`, 'Modified')
WHERE `c`.`ShippingAddress_ZipCode` = 7728
""");
    }

    public override async Task Update_projected_complex_type(bool async)
    {
        await base.Update_projected_complex_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_ZipCode` = 12345
""");
    }

    public override async Task Update_multiple_projected_complex_types_via_anonymous_type(bool async)
    {
        await base.Update_multiple_projected_complex_types_via_anonymous_type(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customer` AS `c`
SET `c`.`BillingAddress_ZipCode` = 54321,
    `c`.`ShippingAddress_ZipCode` = `c`.`BillingAddress_ZipCode`
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
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
                //
                """
@__complex_type_newAddress_0_AddressLine1='New AddressLine1' (Size = 4000)
@__complex_type_newAddress_0_AddressLine2='New AddressLine2' (Size = 4000)
@__complex_type_newAddress_0_Tags='["new_tag1","new_tag2"]' (Size = 4000)
@__complex_type_newAddress_0_ZipCode='99999' (Nullable = true)
@__complex_type_newAddress_0_Code='FR' (Size = 4000)
@__complex_type_newAddress_0_FullName='France' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_AddressLine1` = @__complex_type_newAddress_0_AddressLine1,
    `c`.`ShippingAddress_AddressLine2` = @__complex_type_newAddress_0_AddressLine2,
    `c`.`ShippingAddress_Tags` = @__complex_type_newAddress_0_Tags,
    `c`.`ShippingAddress_ZipCode` = @__complex_type_newAddress_0_ZipCode,
    `c`.`ShippingAddress_Country_Code` = @__complex_type_newAddress_0_Code,
    `c`.`ShippingAddress_Country_FullName` = @__complex_type_newAddress_0_FullName
""",
                //
                """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_nested_complex_type_to_parameter(bool async)
    {
        await base.Update_nested_complex_type_to_parameter(async);

        AssertSql(
"""
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
                //
                """
@__complex_type_newCountry_0_Code='FR' (Size = 4000)
@__complex_type_newCountry_0_FullName='France' (Size = 4000)

UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_Country_Code` = @__complex_type_newCountry_0_Code,
    `c`.`ShippingAddress_Country_FullName` = @__complex_type_newCountry_0_FullName
""",
                //
                """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_another_database_complex_type(bool async)
    {
        await base.Update_complex_type_to_another_database_complex_type(async);

        AssertSql(
"""
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
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
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_inline_without_lambda(bool async)
    {
        await base.Update_complex_type_to_inline_without_lambda(async);

        AssertSql(
"""
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
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
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_inline_with_lambda(bool async)
    {
        await base.Update_complex_type_to_inline_with_lambda(async);

        AssertSql(
"""
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
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
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    public override async Task Update_complex_type_to_another_database_complex_type_with_subquery(bool async)
    {
        await base.Update_complex_type_to_another_database_complex_type_with_subquery(async);

        AssertSql(
"""
@__p_0='1'

SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
ORDER BY `c`.`Id`
LIMIT 18446744073709551610 OFFSET @__p_0
""",
                //
                """
@__p_0='1'

UPDATE `Customer` AS `c0`
INNER JOIN (
    SELECT `c`.`Id`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
    FROM `Customer` AS `c`
    ORDER BY `c`.`Id`
    LIMIT 18446744073709551610 OFFSET @__p_0
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
@__p_0='1'

SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
ORDER BY `c`.`Id`
LIMIT 18446744073709551610 OFFSET @__p_0
""");
    }

    public override async Task Update_collection_inside_complex_type(bool async)
    {
        await base.Update_collection_inside_complex_type(async);

        AssertSql(
"""
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""",
                //
                """
UPDATE `Customer` AS `c`
SET `c`.`ShippingAddress_Tags` = '["new_tag1","new_tag2"]'
""",
                //
                """
SELECT `c`.`Id`, `c`.`Name`, `c`.`BillingAddress_AddressLine1`, `c`.`BillingAddress_AddressLine2`, `c`.`BillingAddress_Tags`, `c`.`BillingAddress_ZipCode`, `c`.`BillingAddress_Country_Code`, `c`.`BillingAddress_Country_FullName`, `c`.`ShippingAddress_AddressLine1`, `c`.`ShippingAddress_AddressLine2`, `c`.`ShippingAddress_Tags`, `c`.`ShippingAddress_ZipCode`, `c`.`ShippingAddress_Country_Code`, `c`.`ShippingAddress_Country_FullName`
FROM `Customer` AS `c`
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
    {
        TestHelpers.AssertAllMethodsOverridden(GetType());
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

    public class ComplexTypeBulkUpdatesMySqlFixture : ComplexTypeBulkUpdatesRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
