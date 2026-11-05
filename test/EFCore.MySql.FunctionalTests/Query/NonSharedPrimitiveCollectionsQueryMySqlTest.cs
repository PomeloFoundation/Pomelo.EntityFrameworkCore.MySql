using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Internal;
using Microting.EntityFrameworkCore.MySql.Tests;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class NonSharedPrimitiveCollectionsQueryMySqlTest : NonSharedPrimitiveCollectionsQueryRelationalTestBase
{
    public NonSharedPrimitiveCollectionsQueryMySqlTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    #region Support for specific element types

    public override async Task Array_of_string()
    {
        await base.Array_of_string();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` longtext PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 'a') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_int()
    {
        await base.Array_of_int();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 1) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_long()
    {
        await base.Array_of_long();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` bigint PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 1) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_short()
    {
        await base.Array_of_short();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` smallint PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 1) = 2
LIMIT 2
""");
    }

    [ConditionalFact]
    public override Task Array_of_byte()
        => base.Array_of_byte();

    public override async Task Array_of_double()
    {
        await base.Array_of_double();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` double PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 1.0) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_float()
    {
        await base.Array_of_float();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` float PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 1) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_decimal()
    {
        await base.Array_of_decimal();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` decimal(65,30) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 1.0) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_DateTime()
    {
        await base.Array_of_DateTime();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIMESTAMP '2023-01-01 12:30:00') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_DateTime_with_milliseconds()
    {
        await base.Array_of_DateTime_with_milliseconds();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIMESTAMP '2023-01-01 12:30:00.123') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_DateTime_with_microseconds()
    {
        await base.Array_of_DateTime_with_microseconds();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIMESTAMP '2023-01-01 12:30:00.123456') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_DateOnly()
    {
        await base.Array_of_DateOnly();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` date PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = DATE '2023-01-01') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_TimeOnly()
    {
        await base.Array_of_TimeOnly();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` time(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIME '12:30:00') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_TimeOnly_with_milliseconds()
    {
        await base.Array_of_TimeOnly_with_milliseconds();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` time(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIME '12:30:00.123') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_TimeOnly_with_microseconds()
    {
        await base.Array_of_TimeOnly_with_microseconds();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` time(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIME '12:30:00.123456') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_DateTimeOffset()
    {
        await base.Array_of_DateTimeOffset();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = TIMESTAMP '2023-01-01 10:30:00') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_bool()
    {
        await base.Array_of_bool();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` tinyint(1) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value`) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_Guid()
    {
        await base.Array_of_Guid();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` char(36) PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 'dc8c903d-d655-4144-a0fd-358099d40ae1') = 2
LIMIT 2
""");
    }

    public override async Task Array_of_byte_array()
    {
        // This does not work, because the byte array is base64 encoded for some reason.
        await base.Array_of_byte_array();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` longblob PATH '$[0]'
    )) AS `s`
    WHERE FROM_BASE64(`s`.`value`) = 0x0102) = 2
LIMIT 2
""");
    }

    public override async Task Array_of_enum()
    {
        await base.Array_of_enum();

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`SomeArray`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`t`.`SomeArray`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `s`
    WHERE `s`.`value` = 0) = 2
LIMIT 2
""");
    }

    [ConditionalFact]
    public override Task Multidimensional_array_is_not_supported()
        => base.Multidimensional_array_is_not_supported();

    #endregion Support for specific element types

    [ConditionalFact]
    public override Task Column_with_custom_converter()
        => base.Column_with_custom_converter();

    public override async Task Parameter_with_inferred_value_converter()
    {
        await base.Parameter_with_inferred_value_converter();

        AssertSql("");
    }

    #region Type mapping inference

    public override async Task Constant_with_inferred_value_converter()
    {
        await base.Constant_with_inferred_value_converter();

        if (AppConfig.ServerVersion.Supports.ValuesWithRows)
        {
            AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`, `t`.`PropertyWithValueConverter`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(1 AS signed) AS `Value` UNION ALL VALUES ROW(8)) AS `v`
    WHERE `v`.`Value` = `t`.`PropertyWithValueConverter`) = 1
LIMIT 2
""");
        }
        else if (AppConfig.ServerVersion.Supports.Values)
        {

        }
        else
        {
            AssertSql(
                """
                SELECT `t`.`Id`, `t`.`Ints`, `t`.`PropertyWithValueConverter`
                FROM `TestEntity` AS `t`
                WHERE (
                    SELECT COUNT(*)
                    FROM (SELECT CAST(1 AS signed) AS `Value` UNION ALL SELECT 8) AS `v`
                    WHERE `v`.`Value` = `t`.`PropertyWithValueConverter`) = 1
                LIMIT 2
                """);
        }
    }

    public override async Task Inline_collection_in_query_filter()
    {
        await base.Inline_collection_in_query_filter();

        if (AppConfig.ServerVersion.Supports.ValuesWithRows)
        {
            AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(1 AS signed) AS `Value` UNION ALL VALUES ROW(2), ROW(3)) AS `v`
    WHERE `v`.`Value` > `t`.`Id`) = 1
LIMIT 2
""");
        }
        else if (AppConfig.ServerVersion.Supports.Values)
        {
        }
        else
        {
            AssertSql(
"""
SELECT `t`.`Id`, `t`.`Ints`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(1 AS signed) AS `Value` UNION ALL SELECT 2 UNION ALL SELECT 3) AS `v`
    WHERE `v`.`Value` > `t`.`Id`) = 1
LIMIT 2
""");
        }
    }

    [SupportedServerVersionCondition("Returning")]
    public override async Task Column_collection_inside_json_owned_entity()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => base.Column_collection_inside_json_owned_entity());
        Assert.Contains("The LINQ expression 't.Owned -> Strings'", exception.Message);
    }

    #endregion Type mapping inference

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_mode(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_mode(mode);

        var rowSql = AppConfig.ServerVersion.Supports.ValuesWithRows ? "ROW" : string.Empty;

        switch (mode)
        {
            case ParameterTranslationMode.MultipleParameters:
                AssertSql(
$"""
@ids1='2'
@ids2='999'

SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT @ids1 AS `Value` UNION ALL VALUES {rowSql}(@ids2)) AS `i`
    WHERE `i`.`Value` > `t`.`Id`) = 1
""");
                break;
            case ParameterTranslationMode.Constant:
                AssertSql(
$"""
SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value` UNION ALL VALUES {rowSql}(999)) AS `i`
    WHERE `i`.`Value` > `t`.`Id`) = 1
""");
                break;
            case ParameterTranslationMode.Parameter:
                AssertSql();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public override async Task Project_collection_from_entity_type_with_owned()
    {
        await base.Project_collection_from_entity_type_with_owned();

        AssertSql(
"""
SELECT `t`.`Ints`
FROM `TestEntityWithOwned` AS `t`
""");
    }

    public override async Task Parameter_collection_Contains_with_default_mode(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Contains_with_default_mode(mode);

        // Similar to Count, generates SQL based on mode
        switch (mode)
        {
            case ParameterTranslationMode.Constant:
                AssertSql(
"""
SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE `t`.`Id` IN (2, 999)
""");
                break;
            case ParameterTranslationMode.MultipleParameters:
                AssertSql(
"""
@ints1='2'
@ints2='999'

SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE `t`.`Id` IN (@ints1, @ints2)
""");
                break;
            case ParameterTranslationMode.Parameter:
                // Parameter mode skips - no SQL assertion needed
                AssertSql();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_mode_EF_Constant(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_mode_EF_Constant(mode);

        // EF.Constant forces constant translation regardless of mode
        var rowSql = AppConfig.ServerVersion.Supports.ValuesWithRows ? "ROW" : string.Empty;

        // EF.Constant works in all modes  
        AssertSql(
$"""
SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value` UNION ALL VALUES {rowSql}(999)) AS `i`
    WHERE `i`.`Value` > `t`.`Id`) = 1
""");
    }

    public override async Task Parameter_collection_Contains_with_default_mode_EF_Constant(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Contains_with_default_mode_EF_Constant(mode);

        // EF.Constant forces constant translation in all modes
        AssertSql(
"""
SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE `t`.`Id` IN (2, 999)
""");
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_mode_EF_Parameter(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_mode_EF_Parameter(mode);

        AssertSql();
    }

    public override async Task Parameter_collection_Contains_with_default_mode_EF_Parameter(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Contains_with_default_mode_EF_Parameter(mode);

        AssertSql();
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_mode_EF_MultipleParameters(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_mode_EF_MultipleParameters(mode);

        var rowSql = AppConfig.ServerVersion.Supports.ValuesWithRows ? "ROW" : string.Empty;

        // EF.Parameters works in all modes - it always generates SQL with parameters
        AssertSql(
$"""
@ids1='2'
@ids2='999'

SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT @ids1 AS `Value` UNION ALL VALUES {rowSql}(@ids2)) AS `i`
    WHERE `i`.`Value` > `t`.`Id`) = 1
""");
    }

    public override async Task Parameter_collection_Contains_with_default_mode_EF_MultipleParameters(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_Contains_with_default_mode_EF_MultipleParameters(mode);

        // EF.Parameters works in all modes (the collection is named 'ints' not 'ids')
        // Note: When mode is Parameter, it still translates unlike _EF_Parameter variant
        AssertSql(
"""
@ints1='2'
@ints2='999'

SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE `t`.`Id` IN (@ints1, @ints2)
""");
    }

    public override async Task Parameter_collection_Contains_parameter_bucketization()
    {
        await base.Parameter_collection_Contains_parameter_bucketization();

        AssertSql(
"""
@ints1='2'
@ints2='999'
@ints3='2'
@ints4='2'
@ints5='2'
@ints6='2'
@ints7='2'
@ints8='2'
@ints9='2'
@ints10='2'
@ints11='2'
@ints12='2'
@ints13='2'
@ints14='2'
@ints15='2'
@ints16='2'
@ints17='2'
@ints18='2'
@ints19='2'
@ints20='2'

SELECT `t`.`Id`
FROM `TestEntity` AS `t`
WHERE `t`.`Id` IN (@ints1, @ints2, @ints3, @ints4, @ints5, @ints6, @ints7, @ints8, @ints9, @ints10, @ints11, @ints12, @ints13, @ints14, @ints15, @ints16, @ints17, @ints18, @ints19, @ints20)
""");
    }

    public override async Task Subquery_over_primitive_collection_on_inheritance_derived_type()
    {
        await base.Subquery_over_primitive_collection_on_inheritance_derived_type();

        AssertSql();
    }

    public override async Task Parameter_collection_of_enum_Cast_from_different_enum_type(ParameterTranslationMode mode)
    {
        await base.Parameter_collection_of_enum_Cast_from_different_enum_type(mode);

        if (mode == ParameterTranslationMode.Parameter)
        {
            AssertSql();
        }
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    protected override DbContextOptionsBuilder SetParameterizedCollectionMode(
        DbContextOptionsBuilder optionsBuilder,
        ParameterTranslationMode parameterizedCollectionMode)
    {
        new MySqlDbContextOptionsBuilder(optionsBuilder).UseParameterizedCollectionMode(parameterizedCollectionMode);

        return optionsBuilder;
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
