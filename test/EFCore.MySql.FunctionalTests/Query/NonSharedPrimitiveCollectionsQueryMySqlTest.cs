using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class NonSharedPrimitiveCollectionsQueryMySqlTest : NonSharedPrimitiveCollectionsQueryRelationalTestBase
{
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

    public override async Task Column_collection_inside_json_owned_entity()
    {
        await base.Column_collection_inside_json_owned_entity();

        AssertSql(
            """
SELECT TOP(2) [t].[Id], [t].[Owned]
FROM [TestOwner] AS [t]
WHERE (
    SELECT COUNT(*)
    FROM OPENJSON(JSON_VALUE([t].[Owned], '$.Strings')) AS [s]) = 2
""",
            //
            """
SELECT TOP(2) [t].[Id], [t].[Owned]
FROM [TestOwner] AS [t]
WHERE JSON_VALUE(JSON_VALUE([t].[Owned], '$.Strings'), '$[1]') = N'bar'
""");
    }

    #endregion Type mapping inference

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_constants()
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_constants();

        AssertSql();
    }

    public override async Task Parameter_collection_of_ints_Contains_int_with_default_constants()
    {
        await base.Parameter_collection_of_ints_Contains_int_with_default_constants();

        AssertSql();
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_constants_EF_Parameter()
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_constants_EF_Parameter();

        AssertSql();
    }

    public override async Task Parameter_collection_of_ints_Contains_int_with_default_constants_EF_Parameter()
    {
        await base.Parameter_collection_of_ints_Contains_int_with_default_constants_EF_Parameter();

        AssertSql();
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_parameters()
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_parameters();

        AssertSql();
    }

    public override async Task Parameter_collection_of_ints_Contains_int_with_default_parameters()
    {
        await base.Parameter_collection_of_ints_Contains_int_with_default_parameters();

        AssertSql();
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_default_parameters_EF_Constant()
    {
        await base.Parameter_collection_Count_with_column_predicate_with_default_parameters_EF_Constant();

        AssertSql();
    }

    public override async Task Parameter_collection_of_ints_Contains_int_with_default_parameters_EF_Constant()
    {
        await base.Parameter_collection_of_ints_Contains_int_with_default_parameters_EF_Constant();

        AssertSql();
    }

    public override async Task Project_collection_from_entity_type_with_owned()
    {
        await base.Project_collection_from_entity_type_with_owned();

        AssertSql();
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    protected override DbContextOptionsBuilder SetTranslateParameterizedCollectionsToConstants(DbContextOptionsBuilder optionsBuilder)
    {
        new MySqlDbContextOptionsBuilder(optionsBuilder).TranslateParameterizedCollectionsToConstants();

        return optionsBuilder;
    }

    protected override DbContextOptionsBuilder SetTranslateParameterizedCollectionsToParameters(DbContextOptionsBuilder optionsBuilder)
    {
        new MySqlDbContextOptionsBuilder(optionsBuilder).TranslateParameterizedCollectionsToParameters();

        return optionsBuilder;
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
