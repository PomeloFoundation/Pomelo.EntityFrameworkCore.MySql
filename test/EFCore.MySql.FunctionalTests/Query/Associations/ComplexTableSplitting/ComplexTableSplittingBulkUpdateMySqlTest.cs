// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;


namespace Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting;

public class ComplexTableSplittingBulkUpdateMySqlTest(
    ComplexTableSplittingMySqlFixture fixture,
    ITestOutputHelper testOutputHelper)
    : ComplexTableSplittingBulkUpdateRelationalTestBase<ComplexTableSplittingMySqlFixture>(fixture, testOutputHelper)
{
    #region Delete

    public override async Task Delete_entity_with_associations()
    {
        await base.Delete_entity_with_associations();

        AssertSql(
            """
@deletableEntity_Name='?' (Size = 4000)

DELETE `r`
FROM `RootEntity` AS `r`
WHERE `r`.`Name` = @deletableEntity_Name
""");
    }

    public override async Task Delete_required_associate()
    {
        await base.Delete_required_associate();

        AssertSql();
    }

    public override async Task Delete_optional_associate()
    {
        await base.Delete_optional_associate();

        AssertSql();
    }

    #endregion Delete

    #region Update properties

    public override async Task Update_property_inside_associate()
    {
        await base.Update_property_inside_associate();

        AssertExecuteUpdateSql(
            """
@p='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_String` = @p
""");
    }

    public override async Task Update_property_inside_associate_with_special_chars()
    {
        await base.Update_property_inside_associate_with_special_chars();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_String` = '{ Some other/JSON:like text though it [isn''t]: ממש ממש לאéèéè }'
WHERE `r`.`RequiredAssociate_String` = '{ this may/look:like JSON but it [isn''t]: ממש ממש לאéèéè }'
""");
    }

    public override async Task Update_property_inside_nested_associate()
    {
        await base.Update_property_inside_nested_associate();

        AssertExecuteUpdateSql(
            """
@p='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_RequiredNestedAssociate_String` = @p
""");
    }

    public override async Task Update_property_on_projected_associate()
    {
        await base.Update_property_on_projected_associate();

        AssertExecuteUpdateSql(
            """
@p='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_String` = @p
""");
    }

    public override async Task Update_property_on_projected_associate_with_OrderBy_Skip()
    {
        await base.Update_property_on_projected_associate_with_OrderBy_Skip();

        AssertExecuteUpdateSql();
    }

    public override async Task Update_associate_with_null_required_property()
    {
        await base.Update_associate_with_null_required_property();

        AssertExecuteUpdateSql();
    }

    #endregion Update properties

    #region Update association

    public override async Task Update_associate_to_parameter()
    {
        await base.Update_associate_to_parameter();

        AssertExecuteUpdateSql(
            """
@complex_type_p_Id='?' (DbType = Int32)
@complex_type_p_Int='?' (DbType = Int32)
@complex_type_p_Ints='?' (Size = 4000)
@complex_type_p_Name='?' (Size = 4000)
@complex_type_p_String='?' (Size = 4000)
@complex_type_p_RequiredNestedAssociate_Id='?' (DbType = Int32)
@complex_type_p_RequiredNestedAssociate_Int='?' (DbType = Int32)
@complex_type_p_RequiredNestedAssociate_Ints='?' (Size = 4000)
@complex_type_p_RequiredNestedAssociate_Name='?' (Size = 4000)
@complex_type_p_RequiredNestedAssociate_String='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_Id` = @complex_type_p_Id,
    `r`.`RequiredAssociate_Int` = @complex_type_p_Int,
    `r`.`RequiredAssociate_Ints` = @complex_type_p_Ints,
    `r`.`RequiredAssociate_Name` = @complex_type_p_Name,
    `r`.`RequiredAssociate_String` = @complex_type_p_String,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Id` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Int` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Ints` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Name` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_String` = NULL,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Id` = @complex_type_p_RequiredNestedAssociate_Id,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Int` = @complex_type_p_RequiredNestedAssociate_Int,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Ints` = @complex_type_p_RequiredNestedAssociate_Ints,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Name` = @complex_type_p_RequiredNestedAssociate_Name,
    `r`.`RequiredAssociate_RequiredNestedAssociate_String` = @complex_type_p_RequiredNestedAssociate_String
""");
    }

    public override async Task Update_nested_associate_to_parameter()
    {
        await base.Update_nested_associate_to_parameter();

        AssertExecuteUpdateSql(
            """
@complex_type_p_Id='?' (DbType = Int32)
@complex_type_p_Int='?' (DbType = Int32)
@complex_type_p_Ints='?' (Size = 4000)
@complex_type_p_Name='?' (Size = 4000)
@complex_type_p_String='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_RequiredNestedAssociate_Id` = @complex_type_p_Id,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Int` = @complex_type_p_Int,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Ints` = @complex_type_p_Ints,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Name` = @complex_type_p_Name,
    `r`.`RequiredAssociate_RequiredNestedAssociate_String` = @complex_type_p_String
""");
    }

    public override async Task Update_associate_to_another_associate()
    {
        await base.Update_associate_to_another_associate();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`OptionalAssociate_Id` = `r`.`RequiredAssociate_Id`,
    `r`.`OptionalAssociate_Int` = `r`.`RequiredAssociate_Int`,
    `r`.`OptionalAssociate_Ints` = `r`.`RequiredAssociate_Ints`,
    `r`.`OptionalAssociate_Name` = `r`.`RequiredAssociate_Name`,
    `r`.`OptionalAssociate_String` = `r`.`RequiredAssociate_String`,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Id` = `r`.`OptionalAssociate_OptionalNestedAssociate_Id`,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Int` = `r`.`OptionalAssociate_OptionalNestedAssociate_Int`,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Ints` = `r`.`OptionalAssociate_OptionalNestedAssociate_Ints`,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Name` = `r`.`OptionalAssociate_OptionalNestedAssociate_Name`,
    `r`.`OptionalAssociate_OptionalNestedAssociate_String` = `r`.`OptionalAssociate_OptionalNestedAssociate_String`,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Id` = `r`.`OptionalAssociate_RequiredNestedAssociate_Id`,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Int` = `r`.`OptionalAssociate_RequiredNestedAssociate_Int`,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Ints` = `r`.`OptionalAssociate_RequiredNestedAssociate_Ints`,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Name` = `r`.`OptionalAssociate_RequiredNestedAssociate_Name`,
    `r`.`OptionalAssociate_RequiredNestedAssociate_String` = `r`.`OptionalAssociate_RequiredNestedAssociate_String`
""");
    }

    public override async Task Update_nested_associate_to_another_nested_associate()
    {
        await base.Update_nested_associate_to_another_nested_associate();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_OptionalNestedAssociate_Id` = `r`.`RequiredAssociate_RequiredNestedAssociate_Id`,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Int` = `r`.`RequiredAssociate_RequiredNestedAssociate_Int`,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Ints` = `r`.`RequiredAssociate_RequiredNestedAssociate_Ints`,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Name` = `r`.`RequiredAssociate_RequiredNestedAssociate_Name`,
    `r`.`RequiredAssociate_OptionalNestedAssociate_String` = `r`.`RequiredAssociate_RequiredNestedAssociate_String`
""");
    }

    public override async Task Update_associate_to_inline()
    {
        await base.Update_associate_to_inline();

        AssertExecuteUpdateSql(
            """
@complex_type_p_Id='?' (DbType = Int32)
@complex_type_p_Int='?' (DbType = Int32)
@complex_type_p_Ints='?' (Size = 4000)
@complex_type_p_Name='?' (Size = 4000)
@complex_type_p_String='?' (Size = 4000)
@complex_type_p_RequiredNestedAssociate_Id='?' (DbType = Int32)
@complex_type_p_RequiredNestedAssociate_Int='?' (DbType = Int32)
@complex_type_p_RequiredNestedAssociate_Ints='?' (Size = 4000)
@complex_type_p_RequiredNestedAssociate_Name='?' (Size = 4000)
@complex_type_p_RequiredNestedAssociate_String='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_Id` = @complex_type_p_Id,
    `r`.`RequiredAssociate_Int` = @complex_type_p_Int,
    `r`.`RequiredAssociate_Ints` = @complex_type_p_Ints,
    `r`.`RequiredAssociate_Name` = @complex_type_p_Name,
    `r`.`RequiredAssociate_String` = @complex_type_p_String,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Id` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Int` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Ints` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Name` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_String` = NULL,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Id` = @complex_type_p_RequiredNestedAssociate_Id,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Int` = @complex_type_p_RequiredNestedAssociate_Int,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Ints` = @complex_type_p_RequiredNestedAssociate_Ints,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Name` = @complex_type_p_RequiredNestedAssociate_Name,
    `r`.`RequiredAssociate_RequiredNestedAssociate_String` = @complex_type_p_RequiredNestedAssociate_String
""");
    }

    public override async Task Update_associate_to_inline_with_lambda()
    {
        await base.Update_associate_to_inline_with_lambda();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_Id` = 1000,
    `r`.`RequiredAssociate_Int` = 70,
    `r`.`RequiredAssociate_Ints` = '[1,2,4]',
    `r`.`RequiredAssociate_Name` = 'Updated associate name',
    `r`.`RequiredAssociate_String` = 'Updated associate string',
    `r`.`RequiredAssociate_OptionalNestedAssociate_Id` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Int` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Ints` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_Name` = NULL,
    `r`.`RequiredAssociate_OptionalNestedAssociate_String` = NULL,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Id` = 1000,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Int` = 80,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Ints` = '[1,2,4]',
    `r`.`RequiredAssociate_RequiredNestedAssociate_Name` = 'Updated nested name',
    `r`.`RequiredAssociate_RequiredNestedAssociate_String` = 'Updated nested string'
""");
    }

    public override async Task Update_nested_associate_to_inline_with_lambda()
    {
        await base.Update_nested_associate_to_inline_with_lambda();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_RequiredNestedAssociate_Id` = 1000,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Int` = 80,
    `r`.`RequiredAssociate_RequiredNestedAssociate_Ints` = '[1,2,4]',
    `r`.`RequiredAssociate_RequiredNestedAssociate_Name` = 'Updated nested name',
    `r`.`RequiredAssociate_RequiredNestedAssociate_String` = 'Updated nested string'
""");
    }

    public override async Task Update_associate_to_null()
    {
        await base.Update_associate_to_null();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`OptionalAssociate_Id` = NULL,
    `r`.`OptionalAssociate_Int` = NULL,
    `r`.`OptionalAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_Name` = NULL,
    `r`.`OptionalAssociate_String` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Id` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Int` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Name` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_String` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Id` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Int` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Name` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_String` = NULL
""");
    }

    public override async Task Update_associate_to_null_with_lambda()
    {
        await base.Update_associate_to_null_with_lambda();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`OptionalAssociate_Id` = NULL,
    `r`.`OptionalAssociate_Int` = NULL,
    `r`.`OptionalAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_Name` = NULL,
    `r`.`OptionalAssociate_String` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Id` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Int` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Name` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_String` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Id` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Int` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Name` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_String` = NULL
""");
    }

    public override async Task Update_associate_to_null_parameter()
    {
        await base.Update_associate_to_null_parameter();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`OptionalAssociate_Id` = NULL,
    `r`.`OptionalAssociate_Int` = NULL,
    `r`.`OptionalAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_Name` = NULL,
    `r`.`OptionalAssociate_String` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Id` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Int` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_Name` = NULL,
    `r`.`OptionalAssociate_OptionalNestedAssociate_String` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Id` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Int` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Ints` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_Name` = NULL,
    `r`.`OptionalAssociate_RequiredNestedAssociate_String` = NULL
""");
    }

    public override async Task Update_required_nested_associate_to_null()
    {
        await base.Update_required_nested_associate_to_null();

        AssertExecuteUpdateSql();
    }

    #endregion Update association

    #region Update collection

    public override async Task Update_collection_to_parameter()
    {
        await base.Update_collection_to_parameter();

        AssertExecuteUpdateSql();
    }

    public override async Task Update_nested_collection_to_parameter()
    {
        await base.Update_nested_collection_to_parameter();

        AssertExecuteUpdateSql();
    }

    public override async Task Update_nested_collection_to_inline_with_lambda()
    {
        await base.Update_nested_collection_to_inline_with_lambda();

        AssertExecuteUpdateSql();
    }

    public override async Task Update_collection_referencing_the_original_collection()
    {
        await base.Update_collection_referencing_the_original_collection();

        AssertExecuteUpdateSql();
    }

    public override async Task Update_nested_collection_to_another_nested_collection()
    {
        await base.Update_nested_collection_to_another_nested_collection();

        AssertExecuteUpdateSql();
    }

    public override async Task Update_inside_structural_collection()
    {
        await base.Update_inside_structural_collection();

        AssertExecuteUpdateSql();
    }

    #endregion Update collection

    #region Update primitive collection

    public override async Task Update_primitive_collection_to_constant()
    {
        await base.Update_primitive_collection_to_constant();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_Ints` = '[1,2,4]'
""");
    }

    public override async Task Update_primitive_collection_to_parameter()
    {
        await base.Update_primitive_collection_to_parameter();

        AssertExecuteUpdateSql(
            """
@ints='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_Ints` = @ints
""");
    }

    public override async Task Update_primitive_collection_to_another_collection()
    {
        await base.Update_primitive_collection_to_another_collection();

        AssertExecuteUpdateSql(
            """
UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_OptionalNestedAssociate_Ints` = `r`.`RequiredAssociate_RequiredNestedAssociate_Ints`
""");
    }

    public override async Task Update_inside_primitive_collection()
    {
        await base.Update_inside_primitive_collection();

        AssertExecuteUpdateSql();
    }

    #endregion Update primitive collection

    #region Multiple updates

    public override async Task Update_multiple_properties_inside_same_associate()
    {
        await base.Update_multiple_properties_inside_same_associate();

        AssertExecuteUpdateSql(
            """
@p='?' (Size = 4000)
@p0='?' (DbType = Int32)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_String` = @p,
    `r`.`RequiredAssociate_Int` = @p0
""");
    }

    public override async Task Update_multiple_properties_inside_associates_and_on_entity_type()
    {
        await base.Update_multiple_properties_inside_associates_and_on_entity_type();

        AssertExecuteUpdateSql(
            """
@p='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`Name` = CONCAT(`r`.`Name`, 'Modified'),
    `r`.`RequiredAssociate_String` = `r`.`OptionalAssociate_String`,
    `r`.`OptionalAssociate_RequiredNestedAssociate_String` = @p
WHERE `r`.`OptionalAssociate_Id` IS NOT NULL
""");
    }

    public override async Task Update_multiple_projected_associates_via_anonymous_type()
    {
        await base.Update_multiple_projected_associates_via_anonymous_type();

        AssertExecuteUpdateSql(
            """
@p='?' (Size = 4000)

UPDATE `RootEntity` AS `r`
SET `r`.`RequiredAssociate_String` = `r`.`OptionalAssociate_String`,
    `r`.`OptionalAssociate_String` = @p
WHERE `r`.`OptionalAssociate_Id` IS NOT NULL
""");
    }

    #endregion Multiple updates

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());
}
