using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class PrimitiveCollectionsQueryMySqlTest : PrimitiveCollectionsQueryRelationalTestBase<
    PrimitiveCollectionsQueryMySqlTest.PrimitiveCollectionsQueryMySqlFixture>
{
    public PrimitiveCollectionsQueryMySqlTest(PrimitiveCollectionsQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Inline_collection_of_ints_Contains(bool async)
    {
        await base.Inline_collection_of_ints_Contains(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (10, 999)
""");
    }

    public override async Task Inline_collection_of_nullable_ints_Contains(bool async)
    {
        await base.Inline_collection_of_nullable_ints_Contains(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableInt` IN (10, 999)
""");
    }

    public override async Task Inline_collection_of_nullable_ints_Contains_null(bool async)
    {
        await base.Inline_collection_of_nullable_ints_Contains_null(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableInt` IS NULL OR (`p`.`NullableInt` = 999)
""");
    }

    public override async Task Inline_collection_Count_with_zero_values(bool async)
    {
        await base.Inline_collection_Count_with_zero_values(async);

        AssertSql();
    }

    public override async Task Inline_collection_Count_with_one_value(bool async)
    {
        await base.Inline_collection_Count_with_one_value(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value`) AS `v`
    WHERE `v`.`Value` > `p`.`Id`) = 1
""");
    }

    public override async Task Inline_collection_Count_with_two_values(bool async)
    {
        await base.Inline_collection_Count_with_two_values(async);

        if (AppConfig.ServerVersion.Supports.ValuesWithRows)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value` UNION ALL VALUES ROW(999)) AS `v`
    WHERE `v`.`Value` > `p`.`Id`) = 1
""");
        }
        else if (AppConfig.ServerVersion.Supports.Values)
        {

        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value` UNION ALL SELECT 999) AS `v`
    WHERE `v`.`Value` > `p`.`Id`) = 1
""");
        }
    }

    public override async Task Inline_collection_Count_with_three_values(bool async)
    {
        await base.Inline_collection_Count_with_three_values(async);

        if (AppConfig.ServerVersion.Supports.ValuesWithRows)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value` UNION ALL VALUES ROW(999), ROW(1000)) AS `v`
    WHERE `v`.`Value` > `p`.`Id`) = 2
""");
        }
        else if (AppConfig.ServerVersion.Supports.Values)
        {
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(2 AS signed) AS `Value` UNION ALL SELECT 999 UNION ALL SELECT 1000) AS `v`
    WHERE `v`.`Value` > `p`.`Id`) = 2
""");
        }
    }

    public override async Task Inline_collection_Contains_with_zero_values(bool async)
    {
        await base.Inline_collection_Contains_with_zero_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE FALSE
""");
    }

    public override async Task Inline_collection_Contains_with_one_value(bool async)
    {
        await base.Inline_collection_Contains_with_one_value(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` = 2
""");
    }

    public override async Task Inline_collection_Contains_with_two_values(bool async)
    {
        await base.Inline_collection_Contains_with_two_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` IN (2, 999)
""");
    }

    public override async Task Inline_collection_Contains_with_three_values(bool async)
    {
        await base.Inline_collection_Contains_with_three_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` IN (2, 999, 1000)
""");
    }

    public override async Task Inline_collection_Contains_with_all_parameters(bool async)
    {
        await base.Inline_collection_Contains_with_all_parameters(async);

        AssertSql(
"""
@__i_0='2'
@__j_1='999'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` IN (@__i_0, @__j_1)
""");
    }

    public override async Task Inline_collection_Contains_with_constant_and_parameter(bool async)
    {
        await base.Inline_collection_Contains_with_constant_and_parameter(async);

        AssertSql(
"""
@__j_0='999'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` IN (2, @__j_0)
""");
    }

    public override async Task Inline_collection_Contains_with_mixed_value_types(bool async)
    {
        await base.Inline_collection_Contains_with_mixed_value_types(async);

        AssertSql(
"""
@__i_0='11'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (999, @__i_0, `p`.`Id`, `p`.`Id` + `p`.`Int`)
""");
    }

    public override async Task Inline_collection_negated_Contains_as_All(bool async)
    {
        await base.Inline_collection_negated_Contains_as_All(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` NOT IN (2, 999)
""");
    }

    public override async Task Parameter_collection_Count(bool async)
    {
        await base.Parameter_collection_Count(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE('[2,999]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
    WHERE `i`.`value` > `p`.`Id`) = 1
""");
    }

    public override async Task Parameter_collection_of_nullable_ints_Contains_int(bool async)
    {
        await base.Parameter_collection_of_nullable_ints_Contains_int(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (
    SELECT `n`.`value`
    FROM JSON_TABLE('[10,999]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
)
""");
        }
        else
        {
        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (10, 999)
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` NOT IN (10, 999)
""");
        }
    }

    public override async Task Parameter_collection_of_nullable_ints_Contains_nullable_int(bool async)
    {
        await base.Parameter_collection_of_nullable_ints_Contains_nullable_int(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE EXISTS (
    SELECT 1
    FROM JSON_TABLE('[null,999]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
    WHERE (`n`.`value` = `p`.`NullableInt`) OR (`n`.`value` IS NULL AND (`p`.`NullableInt` IS NULL)))
""");
        }
        else
        {
        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableInt` IS NULL OR (`p`.`NullableInt` = 999)
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableInt` IS NOT NULL AND (`p`.`NullableInt` <> 999)
""");
        }
    }

    public override async Task Parameter_collection_of_strings_Contains_nullable_string(bool async)
    {
        await base.Parameter_collection_of_strings_Contains_nullable_string(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE EXISTS (
    SELECT 1
    FROM JSON_TABLE('["999",null]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` longtext PATH '$[0]'
    )) AS `s`
    WHERE (`s`.`value` = `p`.`NullableString`) OR (`s`.`value` IS NULL AND (`p`.`NullableString` IS NULL)))
""");
        }
        else
        {
        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableString` IN ('10', '999')
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableString` NOT IN ('10', '999') OR (`p`.`NullableString` IS NULL)
""");
        }
    }

    public override async Task Parameter_collection_of_DateTimes_Contains(bool async)
    {
        await base.Parameter_collection_of_DateTimes_Contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`DateTime` IN (
    SELECT `d`.`value`
    FROM JSON_TABLE('["2020-01-10T12:30:00Z","9999-01-01T00:00:00Z"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `d`
)
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`DateTime` IN (TIMESTAMP '2020-01-10 12:30:00', TIMESTAMP '9999-01-01 00:00:00')
""");
        }
    }

    public override async Task Parameter_collection_of_bools_Contains(bool async)
    {
        await base.Parameter_collection_of_bools_Contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Bool` IN (
    SELECT `b`.`value`
    FROM JSON_TABLE('[true]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` tinyint(1) PATH '$[0]'
    )) AS `b`
)
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Bool`
""");
        }
    }

    public override async Task Parameter_collection_of_enums_Contains(bool async)
    {
        await base.Parameter_collection_of_enums_Contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Enum` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[0,3]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Enum` IN (0, 3)
""");
        }
    }

    public override async Task Parameter_collection_null_Contains(bool async)
    {
        await base.Parameter_collection_null_Contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (
    SELECT `i`.`value`
    FROM JSON_TABLE(NULL, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
)
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE FALSE
""");
        }
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationWithoutMySqlBugs))]
    public override async Task Column_collection_of_ints_Contains(bool async)
    {
        await base.Column_collection_of_ints_Contains(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE 10 IN (
    SELECT `i`.`value`
    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationWithoutMySqlBugs))]
    public override async Task Column_collection_of_nullable_ints_Contains(bool async)
    {
        await base.Column_collection_of_nullable_ints_Contains(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE 10 IN (
    SELECT `n`.`value`
    FROM JSON_TABLE(`p`.`NullableInts`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationWithoutMySqlBugs))]
    public override async Task Column_collection_of_nullable_ints_Contains_null(bool async)
    {
        await base.Column_collection_of_nullable_ints_Contains_null(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE array_position(p."NullableInts", NULL) IS NOT NULL
""");
    }

    public override async Task Column_collection_of_strings_contains_null(bool async)
    {
        await base.Column_collection_of_strings_contains_null(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE FALSE
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationWithoutMySqlBugs))]
    public override async Task Column_collection_of_nullable_strings_contains_null(bool async)
    {
        await base.Column_collection_of_nullable_strings_contains_null(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE array_position(p."NullableStrings", NULL) IS NOT NULL
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationWithoutMySqlBugs))]
    public override async Task Column_collection_of_bools_Contains(bool async)
    {
        await base.Column_collection_of_bools_Contains(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE TRUE IN (
    SELECT `b`.`value`
    FROM JSON_TABLE(`p`.`Bools`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` tinyint(1) PATH '$[0]'
    )) AS `b`
)
""");
    }

    public override async Task Column_collection_Count_method(bool async)
    {
        await base.Column_collection_Count_method(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`) = 2
""");
    }

    public override async Task Column_collection_Length(bool async)
    {
        await base.Column_collection_Length(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`) = 2
""");
    }

    public override async Task Column_collection_index_int(bool async)
    {
        await base.Column_collection_index_int(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_VALUE(`p`.`Ints`, '$[1]') AS signed) = 10
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Ints`, '$[1]')) AS signed) = 10
""");
        }
    }

    public override async Task Column_collection_index_string(bool async)
    {
        await base.Column_collection_index_string(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_VALUE(`p`.`Strings`, '$[1]') AS char) = '10'
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Strings`, '$[1]')) AS char) = '10'
""");
        }
    }

    public override async Task Column_collection_index_datetime(bool async)
    {
        await base.Column_collection_index_datetime(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_VALUE(`p`.`DateTimes`, '$[1]') AS datetime(6)) = TIMESTAMP '2020-01-10 12:30:00'
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`DateTimes`, '$[1]')) AS datetime(6)) = TIMESTAMP '2020-01-10 12:30:00'
""");
        }
    }

    public override async Task Column_collection_index_beyond_end(bool async)
    {
        await base.Column_collection_index_beyond_end(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_VALUE(`p`.`Ints`, '$[999]') AS signed) = 10
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Ints`, '$[999]')) AS signed) = 10
""");
        }
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonValue), Skip = "TODO: Fix NULL handling of JSON_EXTRACT().")]
    public override async Task Nullable_reference_column_collection_index_equals_nullable_column(bool async)
    {
        await base.Nullable_reference_column_collection_index_equals_nullable_column(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (CAST(JSON_VALUE(`p`.`NullableStrings`, '$[2]') AS char) = `p`.`NullableString`) OR ((CAST(JSON_VALUE(`p`.`NullableStrings`, '$[2]') AS char)) IS NULL AND (`p`.`NullableString` IS NULL))
""");
    }

    public override async Task Non_nullable_reference_column_collection_index_equals_nullable_column(bool async)
    {
        await base.Non_nullable_reference_column_collection_index_equals_nullable_column(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (JSON_LENGTH(`p`.`Strings`) > 0) AND (CAST(JSON_VALUE(`p`.`Strings`, '$[1]') AS char) = `p`.`NullableString`)
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (JSON_LENGTH(`p`.`Strings`) > 0) AND (CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Strings`, '$[1]')) AS char) = `p`.`NullableString`)
""");
        }
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
    public override async Task Inline_collection_index_Column(bool async)
    {
        await base.Inline_collection_index_Column(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE (
    SELECT v."Value"
    FROM (VALUES (0, 1::int), (1, 2), (2, 3)) AS v(_ord, "Value")
    ORDER BY v._ord NULLS FIRST
    LIMIT 1 OFFSET p."Int") = 1
""");
    }

    public override async Task Parameter_collection_index_Column_equal_Column(bool async)
    {
        await base.Parameter_collection_index_Column_equal_Column(async);

        AssertSql(
"""
@__ints_0='[0,2,3]' (Size = 4000)

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(@__ints_0, CONCAT('$[', CAST(`p`.`Int` AS char), ']'))) AS signed) = `p`.`Int`
""");
    }

    public override async Task Parameter_collection_index_Column_equal_constant(bool async)
    {
        await base.Parameter_collection_index_Column_equal_constant(async);

        AssertSql(
"""
@__ints_0='[1,2,3]' (Size = 4000)

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(@__ints_0, CONCAT('$[', CAST(`p`.`Int` AS char), ']'))) AS signed) = 1
""");
    }

    public override async Task Column_collection_ElementAt(bool async)
    {
        await base.Column_collection_ElementAt(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_VALUE(`p`.`Ints`, '$[1]') AS signed) = 10
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Ints`, '$[1]')) AS signed) = 10
""");
        }
    }

    public override async Task Column_collection_Skip(bool async)
    {
        await base.Column_collection_Skip(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `i`.`key`
        FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i`
        ORDER BY `i`.`key`
        LIMIT 18446744073709551610 OFFSET 1
    ) AS `t`) = 2
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.LimitWithinInAllAnySomeSubquery))]
    public override async Task Column_collection_Take(bool async)
    {
        await base.Column_collection_Take(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE 11 = ANY (p."Ints"[:2])
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.LimitWithinInAllAnySomeSubquery))]
    public override async Task Column_collection_Skip_Take(bool async)
    {
        await base.Column_collection_Skip_Take(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE 11 = ANY (p."Ints"[2:3])
""");
    }

    public override async Task Column_collection_OrderByDescending_ElementAt(bool async)
    {
        await base.Column_collection_OrderByDescending_ElementAt(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT `i`.`value`
    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
    ORDER BY `i`.`value` DESC
    LIMIT 1 OFFSET 0) = 111
""");
    }

    public override async Task Column_collection_Any(bool async)
    {
        await base.Column_collection_Any(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE JSON_LENGTH(`p`.`Ints`) > 0
""");
    }

    public override async Task Column_collection_Distinct(bool async)
    {
        await base.Column_collection_Distinct(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT DISTINCT `i`.`value`
        FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i`
    ) AS `t`) = 3
""");
    }

    public override async Task Column_collection_projection_from_top_level(bool async)
    {
        await base.Column_collection_projection_from_top_level(async);

        AssertSql(
"""
SELECT `p`.`Ints`
FROM `PrimitiveCollectionsEntity` AS `p`
ORDER BY `p`.`Id`
""");
    }

    public override async Task Column_collection_Join_parameter_collection(bool async)
    {
        await base.Column_collection_Join_parameter_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
    INNER JOIN JSON_TABLE('[11,111]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i0` ON `i`.`value` = `i0`.`value`) = 2
""");
    }

    public override async Task Inline_collection_Join_ordered_column_collection(bool async)
    {
        await base.Inline_collection_Join_ordered_column_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(11 AS signed) AS `Value` UNION ALL VALUES ROW(111)) AS `v`
    INNER JOIN JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i` ON `v`.`Value` = `i`.`value`) = 2
""");
    }

    public override async Task Parameter_collection_Concat_column_collection(bool async)
    {
        await base.Parameter_collection_Concat_column_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `i`.`value`
        FROM JSON_TABLE('[11,111]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i`
        UNION ALL
        SELECT `i0`.`value`
        FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i0`
    ) AS `t`) = 2
""");
    }

    public override async Task Column_collection_Union_parameter_collection(bool async)
    {
        await base.Column_collection_Union_parameter_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `i`.`value`
        FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i`
        UNION
        SELECT `i0`.`value`
        FROM JSON_TABLE('[11,111]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i0`
    ) AS `t`) = 2
""");
    }

    public override async Task Column_collection_Intersect_inline_collection(bool async)
    {
        await base.Column_collection_Intersect_inline_collection(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE (
    SELECT count(*)::int
    FROM (
        SELECT i.value
        FROM unnest(p."Ints") AS i(value)
        INTERSECT
        VALUES (11::int), (111)
    ) AS t) = 2
""");
    }

    public override async Task Inline_collection_Except_column_collection(bool async)
    {
        await base.Inline_collection_Except_column_collection(async);

        AssertSql(
"""
SELECT p."Id", p."Bool", p."Bools", p."DateTime", p."DateTimes", p."Enum", p."Enums", p."Int", p."Ints", p."NullableInt", p."NullableInts", p."NullableString", p."NullableStrings", p."String", p."Strings"
FROM "PrimitiveCollectionsEntity" AS p
WHERE (
    SELECT count(*)::int
    FROM (
        SELECT v."Value"
        FROM (VALUES (11::int), (111)) AS v("Value")
        EXCEPT
        SELECT i.value AS "Value"
        FROM unnest(p."Ints") AS i(value)
    ) AS t
    WHERE t."Value" % 2 = 1) = 2
""");
    }

    public override async Task Column_collection_equality_parameter_collection(bool async)
    {
        await base.Column_collection_equality_parameter_collection(async);

        AssertSql(
"""
@__ints_0='[1,10]' (Size = 4000)

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Ints` = @__ints_0
""");
    }

    public override async Task Column_collection_equality_inline_collection(bool async)
    {
        await base.Column_collection_equality_inline_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Ints` = '[1,10]'
""");
    }

    public override async Task Column_collection_equality_inline_collection_with_parameters(bool async)
    {
        await base.Column_collection_equality_inline_collection_with_parameters(async);

        AssertSql();
    }

    public override async Task Parameter_collection_in_subquery_Union_column_collection_as_compiled_query(bool async)
    {
        await base.Parameter_collection_in_subquery_Union_column_collection_as_compiled_query(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `t`.`value`
        FROM (
            SELECT `i`.`value`, `i`.`key`
            FROM JSON_TABLE('[10,111]', '$[*]' COLUMNS (
                `key` FOR ORDINALITY,
                `value` int PATH '$[0]'
            )) AS `i`
            ORDER BY `i`.`key`
            LIMIT 18446744073709551610 OFFSET 1
        ) AS `t`
        UNION
        SELECT `i0`.`value`
        FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i0`
    ) AS `t0`) = 3
""");
    }

    public override async Task Parameter_collection_in_subquery_Union_column_collection(bool async)
    {
        await base.Parameter_collection_in_subquery_Union_column_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `s`.`value`
        FROM JSON_TABLE('[111]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `s`
        UNION
        SELECT `i`.`value`
        FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i`
    ) AS `t`) = 3
""");
    }

    public override async Task Parameter_collection_in_subquery_Union_column_collection_nested(bool async)
    {
        await base.Parameter_collection_in_subquery_Union_column_collection_nested(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `s`.`value`
        FROM JSON_TABLE('[111]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `s`
        UNION
        SELECT `t1`.`value`
        FROM (
            SELECT `t0`.`value`
            FROM (
                SELECT DISTINCT `t2`.`value`
                FROM (
                    SELECT `i`.`value`, `i`.`key`
                    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
                        `key` FOR ORDINALITY,
                        `value` int PATH '$[0]'
                    )) AS `i`
                    ORDER BY `i`.`value`
                    LIMIT 18446744073709551610 OFFSET 1
                ) AS `t2`
            ) AS `t0`
            ORDER BY `t0`.`value` DESC
            LIMIT 20
        ) AS `t1`
    ) AS `t`) = 3
""");
    }

    public override void Parameter_collection_in_subquery_and_Convert_as_compiled_query()
    {
        // base.Parameter_collection_in_subquery_and_Convert_as_compiled_query();
        //
        // AssertSql();

        // The array indexing is translated as a subquery over e.g. OPENJSON with LIMIT/OFFSET.
        // Since there's a CAST over that, the type mapping inference from the other side (p.String) doesn't propagate inside to the
        // subquery. In this case, the CAST operand gets the default CLR type mapping, but that's object in this case.
        // We should apply the default type mapping to the parameter, but need to figure out the exact rules when to do this.
        var query = EF.CompileQuery(
            (PrimitiveCollectionsContext context, object[] parameters)
                => context.Set<PrimitiveCollectionsEntity>().Where(p => p.String == (string)parameters[0]));

        using var context = Fixture.CreateContext();

        var exception = Assert.Throws<InvalidOperationException>(() => query(context, new[] { "foo" }).ToList());

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            Assert.Contains("in the SQL tree does not have a type mapping assigned", exception.Message);
        }
        else
        {
            Assert.Contains("Primitive collections support has not been enabled.", exception.Message);
        }
    }

    public override async Task Parameter_collection_in_subquery_Union_another_parameter_collection_as_compiled_query(bool async)
    {
        var message = (await Assert.ThrowsAsync<EqualException>(
            () => base.Parameter_collection_in_subquery_Union_another_parameter_collection_as_compiled_query(async))).Message;

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            Assert.Equal(RelationalStrings.SetOperationsRequireAtLeastOneSideWithValidTypeMapping("Union"), message);
        }
    }

    public override async Task Parameter_collection_in_subquery_Count_as_compiled_query(bool async)
    {
        await base.Parameter_collection_in_subquery_Count_as_compiled_query(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `i`.`value`, `i`.`key`, `i`.`value` AS `value0`
        FROM JSON_TABLE('[10,111]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i`
        ORDER BY `i`.`key`
        LIMIT 18446744073709551610 OFFSET 1
    ) AS `t`
    WHERE `t`.`value0` > `p`.`Id`) = 1
""");
    }

    public override async Task Column_collection_in_subquery_Union_parameter_collection(bool async)
    {
        await base.Column_collection_in_subquery_Union_parameter_collection(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (
        SELECT `t`.`value`
        FROM (
            SELECT `i`.`value`, `i`.`key`
            FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
                `key` FOR ORDINALITY,
                `value` int PATH '$[0]'
            )) AS `i`
            ORDER BY `i`.`key`
            LIMIT 18446744073709551610 OFFSET 1
        ) AS `t`
        UNION
        SELECT `i0`.`value`
        FROM JSON_TABLE('[10,111]', '$[*]' COLUMNS (
            `key` FOR ORDINALITY,
            `value` int PATH '$[0]'
        )) AS `i0`
    ) AS `t0`) = 3
""");
    }

    public override async Task Project_collection_of_ints_simple(bool async)
    {
        await base.Project_collection_of_ints_simple(async);

        AssertSql(
"""
SELECT `p`.`Ints`
FROM `PrimitiveCollectionsEntity` AS `p`
ORDER BY `p`.`Id`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationStable))]
    public override async Task Project_collection_of_ints_ordered(bool async)
    {
        await base.Project_collection_of_ints_ordered(async);

        AssertSql(
"""
SELECT `p`.`Id`, `i`.`value`, `i`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
    `key` FOR ORDINALITY,
    `value` int PATH '$[0]'
)) AS `i` ON TRUE
ORDER BY `p`.`Id`, `i`.`value` DESC
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTable))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
    public override async Task Project_collection_of_datetimes_filtered(bool async)
    {
        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            await base.Project_collection_of_datetimes_filtered(async);

            AssertSql(
"""
SELECT `p`.`Id`, `t`.`value`, `t`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT `d`.`value`, `d`.`key`
    FROM JSON_TABLE(`p`.`DateTimes`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `d`
    WHERE (EXTRACT(day FROM `d`.`value`) <> 1) OR EXTRACT(day FROM `d`.`value`) IS NULL
) AS `t` ON TRUE
ORDER BY `p`.`Id`, `t`.`key`
""");
        }
        else
        {
            await Assert.ThrowsAsync<InvalidOperationException>(()
                => base.Project_collection_of_datetimes_filtered(async));
        }
    }

    public override async Task Project_collection_of_nullable_ints_with_paging(bool async)
    {
        await base.Project_collection_of_nullable_ints_with_paging(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `t`.`value`, `t`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT `n`.`value`, `n`.`key`
    FROM JSON_TABLE(`p`.`NullableInts`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
    ORDER BY `n`.`key`
    LIMIT 20
) AS `t` ON TRUE
ORDER BY `p`.`Id`, `t`.`key`
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`NullableInts`
FROM `PrimitiveCollectionsEntity` AS `p`
ORDER BY `p`.`Id`
""");
        }
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTable))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
    public override async Task Project_collection_of_nullable_ints_with_paging2(bool async)
    {
        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            await base.Project_collection_of_nullable_ints_with_paging2(async);

            AssertSql(
"""
SELECT `p`.`Id`, `t`.`value`, `t`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT `n`.`value`, `n`.`key`
    FROM JSON_TABLE(`p`.`NullableInts`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
    ORDER BY `n`.`value`
    LIMIT 18446744073709551610 OFFSET 1
) AS `t` ON TRUE
ORDER BY `p`.`Id`, `t`.`value`
""");
        }
        else
        {
            await Assert.ThrowsAsync<InvalidOperationException>(()
                => base.Project_collection_of_nullable_ints_with_paging2(async));
        }
    }

    public override async Task Project_collection_of_nullable_ints_with_paging3(bool async)
    {
        await base.Project_collection_of_nullable_ints_with_paging3(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `t`.`value`, `t`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT `n`.`value`, `n`.`key`
    FROM JSON_TABLE(`p`.`NullableInts`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
    ORDER BY `n`.`key`
    LIMIT 18446744073709551610 OFFSET 2
) AS `t` ON TRUE
ORDER BY `p`.`Id`, `t`.`key`
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`NullableInts`
FROM `PrimitiveCollectionsEntity` AS `p`
ORDER BY `p`.`Id`
""");
        }
    }

    public override async Task Project_collection_of_ints_with_distinct(bool async)
    {
        await base.Project_collection_of_ints_with_distinct(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `p`.`Id`, `t`.`value`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT DISTINCT `i`.`value`
    FROM JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
) AS `t` ON TRUE
ORDER BY `p`.`Id`
""");
        }
        else
        {
            AssertSql(
"""
SELECT `p`.`Ints`
FROM `PrimitiveCollectionsEntity` AS `p`
ORDER BY `p`.`Id`
""");
        }
    }

    public override async Task Project_collection_of_nullable_ints_with_distinct(bool async)
    {
        await base.Project_collection_of_nullable_ints_with_distinct(async);

        AssertSql();
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTable))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
    public override async Task Project_empty_collection_of_nullables_and_collection_only_containing_nulls(bool async)
    {
        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            await base.Project_empty_collection_of_nullables_and_collection_only_containing_nulls(async);

            AssertSql(
"""
SELECT `p`.`Id`, `t`.`value`, `t`.`key`, `t0`.`value`, `t0`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT `n`.`value`, `n`.`key`
    FROM JSON_TABLE(`p`.`NullableInts`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n`
    WHERE FALSE
) AS `t` ON TRUE
LEFT JOIN LATERAL (
    SELECT `n0`.`value`, `n0`.`key`
    FROM JSON_TABLE(`p`.`NullableInts`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `n0`
    WHERE `n0`.`value` IS NULL
) AS `t0` ON TRUE
ORDER BY `p`.`Id`, `t`.`key`, `t0`.`key`
""");
        }
        else
        {
            await Assert.ThrowsAsync<InvalidOperationException>(()
                => base.Project_empty_collection_of_nullables_and_collection_only_containing_nulls(async));
        }
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationStable))]
    public override async Task Project_multiple_collections(bool async)
    {
        // Base implementation currently uses an Unspecified DateTime in the query, but we require a Utc one.
        await AssertQuery(
            async,
            ss => ss.Set<PrimitiveCollectionsEntity>().OrderBy(x => x.Id).Select(x => new
            {
                Ints = x.Ints.ToList(),
                OrderedInts = x.Ints.OrderByDescending(xx => xx).ToList(),
                FilteredDateTimes = x.DateTimes.Where(xx => xx.Day != 1).ToList(),
                FilteredDateTimes2 = x.DateTimes.Where(xx => xx > new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)).ToList()
            }),
            elementAsserter: (e, a) =>
            {
                AssertCollection(e.Ints, a.Ints, ordered: true);
                AssertCollection(e.OrderedInts, a.OrderedInts, ordered: true);
                AssertCollection(e.FilteredDateTimes, a.FilteredDateTimes, elementSorter: ee => ee);
                AssertCollection(e.FilteredDateTimes2, a.FilteredDateTimes2, elementSorter: ee => ee);
            },
            assertOrder: true);

        AssertSql(
"""
SELECT `p`.`Id`, `i`.`value`, `i`.`key`, `i0`.`value`, `i0`.`key`, `t`.`value`, `t`.`key`, `t0`.`value`, `t0`.`key`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
    `key` FOR ORDINALITY,
    `value` int PATH '$[0]'
)) AS `i` ON TRUE
LEFT JOIN JSON_TABLE(`p`.`Ints`, '$[*]' COLUMNS (
    `key` FOR ORDINALITY,
    `value` int PATH '$[0]'
)) AS `i0` ON TRUE
LEFT JOIN LATERAL (
    SELECT `d`.`value`, `d`.`key`
    FROM JSON_TABLE(`p`.`DateTimes`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `d`
    WHERE (EXTRACT(day FROM `d`.`value`) <> 1) OR EXTRACT(day FROM `d`.`value`) IS NULL
) AS `t` ON TRUE
LEFT JOIN LATERAL (
    SELECT `d0`.`value`, `d0`.`key`
    FROM JSON_TABLE(`p`.`DateTimes`, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `d0`
    WHERE `d0`.`value` > TIMESTAMP '2000-01-01 00:00:00'
) AS `t0` ON TRUE
ORDER BY `p`.`Id`, `i`.`key`, `i0`.`value` DESC, `i0`.`key`, `t`.`key`, `t0`.`key`
""");
    }

    public override async Task Project_primitive_collections_element(bool async)
    {
        await base.Project_primitive_collections_element(async);

        if (AppConfig.ServerVersion.Supports.JsonValue)
        {
            AssertSql(
"""
SELECT CAST(JSON_VALUE(`p`.`Ints`, '$[0]') AS signed) AS `Indexer`, CAST(JSON_VALUE(`p`.`DateTimes`, '$[0]') AS datetime(6)) AS `EnumerableElementAt`, CAST(JSON_VALUE(`p`.`Strings`, '$[1]') AS char) AS `QueryableElementAt`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` < 4
ORDER BY `p`.`Id`
""");
        }
        else
        {
            AssertSql(
"""
SELECT CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Ints`, '$[0]')) AS signed) AS `Indexer`, CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`DateTimes`, '$[0]')) AS datetime(6)) AS `EnumerableElementAt`, CAST(JSON_UNQUOTE(JSON_EXTRACT(`p`.`Strings`, '$[1]')) AS char) AS `QueryableElementAt`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` < 4
ORDER BY `p`.`Id`
""");
        }
    }

    public override async Task Inline_collection_Contains_as_Any_with_predicate(bool async)
    {
        await base.Inline_collection_Contains_as_Any_with_predicate(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` IN (2, 999)
""");
    }

    public override async Task Column_collection_Concat_parameter_collection_equality_inline_collection(bool async)
    {
        await base.Column_collection_Concat_parameter_collection_equality_inline_collection(async);

        AssertSql();
    }

    public override async Task Nested_contains_with_Lists_and_no_inferred_type_mapping(bool async)
    {
        await base.Nested_contains_with_Lists_and_no_inferred_type_mapping(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CASE
    WHEN `p`.`Int` IN (1, 2, 3) THEN 'one'
    ELSE 'two'
END IN ('one', 'two', 'three')
""");
    }

    public override async Task Nested_contains_with_arrays_and_no_inferred_type_mapping(bool async)
    {
        await base.Nested_contains_with_arrays_and_no_inferred_type_mapping(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE CASE
    WHEN `p`.`Int` IN (1, 2, 3) THEN 'one'
    ELSE 'two'
END IN ('one', 'two', 'three')
""");
    }

    public override async Task Inline_collection_with_single_parameter_element_Contains(bool async)
    {
        await base.Inline_collection_with_single_parameter_element_Contains(async);

        AssertSql(
"""
@__i_0='2'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` = @__i_0
""");
    }

    public override async Task Inline_collection_with_single_parameter_element_Count(bool async)
    {
        await base.Inline_collection_with_single_parameter_element_Count(async);

        AssertSql(
"""
@__i_0='2'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT CAST(@__i_0 AS signed) AS `Value`) AS `v`
    WHERE `v`.`Value` > `p`.`Id`) = 1
""");
    }

    public override async Task Parameter_collection_Contains_with_EF_Constant(bool async)
    {
        await base.Parameter_collection_Contains_with_EF_Constant(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Id` IN (2, 999, 1000)
""");
    }

    public override async Task Parameter_collection_Where_with_EF_Constant_Where_Any(bool async)
    {
        await base.Parameter_collection_Where_with_EF_Constant_Where_Any(async);

        var rowSql = AppConfig.ServerVersion.Supports.ValuesWithRows ? "ROW" : string.Empty;

        AssertSql(
$"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE EXISTS (
    SELECT 1
    FROM (SELECT 2 AS `Value` UNION ALL VALUES {rowSql}(999), {rowSql}(1000)) AS `i`
    WHERE `i`.`Value` > 0)
""");
    }

    public override async Task Parameter_collection_Count_with_column_predicate_with_EF_Constant(bool async)
    {
        await base.Parameter_collection_Count_with_column_predicate_with_EF_Constant(async);

        var rowSql = AppConfig.ServerVersion.Supports.ValuesWithRows ? "ROW" : string.Empty;

        AssertSql(
$"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT COUNT(*)
    FROM (SELECT 2 AS `Value` UNION ALL VALUES {rowSql}(999), {rowSql}(1000)) AS `i`
    WHERE `i`.`Value` > `p`.`Id`) = 2
""");
    }

    public override async Task Inline_collection_Min_with_two_values(bool async)
    {
        await base.Inline_collection_Min_with_two_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE LEAST(30, `p`.`Int`) = 30
""");
    }

    public override async Task Inline_collection_Max_with_two_values(bool async)
    {
        await base.Inline_collection_Max_with_two_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE GREATEST(30, `p`.`Int`) = 30
""");
    }

    public override async Task Inline_collection_Min_with_three_values(bool async)
    {
        await base.Inline_collection_Min_with_three_values(async);

        AssertSql(
"""
@__i_0='25'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE LEAST(30, `p`.`Int`, @__i_0) = 25
""");
    }

    public override async Task Inline_collection_Max_with_three_values(bool async)
    {
        await base.Inline_collection_Max_with_three_values(async);

        AssertSql(
"""
@__i_0='35'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE GREATEST(30, `p`.`Int`, @__i_0) = 35
""");
    }

    public override async Task Parameter_collection_of_ints_Contains_int(bool async)
    {
        await base.Parameter_collection_of_ints_Contains_int(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (10, 999)
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` NOT IN (10, 999)
""");
    }

    public override async Task Parameter_collection_of_ints_Contains_nullable_int(bool async)
    {
        await base.Parameter_collection_of_ints_Contains_nullable_int(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableInt` IN (10, 999)
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableInt` NOT IN (10, 999) OR (`p`.`NullableInt` IS NULL)
""");
    }

    public override async Task Parameter_collection_of_strings_Contains_string(bool async)
    {
        await base.Parameter_collection_of_strings_Contains_string(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`String` IN ('10', '999')
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`String` NOT IN ('10', '999')
""");
    }

    public override async Task Parameter_collection_of_nullable_strings_Contains_string(bool async)
    {
        await base.Parameter_collection_of_nullable_strings_Contains_string(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`String` = '10'
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`String` <> '10'
""");
    }

    public override async Task Parameter_collection_of_nullable_strings_Contains_nullable_string(bool async)
    {
        await base.Parameter_collection_of_nullable_strings_Contains_nullable_string(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableString` IS NULL OR (`p`.`NullableString` = '999')
""",
                //
                """
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`NullableString` IS NOT NULL AND (`p`.`NullableString` <> '999')
""");
    }

    public override async Task Column_collection_SelectMany(bool async)
    {
        await base.Column_collection_SelectMany(async);

        AssertSql("");
    }

    public override async Task Project_collection_of_ints_with_ToList_and_FirstOrDefault(bool async)
    {
        await base.Project_collection_of_ints_with_ToList_and_FirstOrDefault(async);

        AssertSql(
"""
SELECT `p`.`Ints`
FROM `PrimitiveCollectionsEntity` AS `p`
ORDER BY `p`.`Id`
LIMIT 1
""");
    }

    public override async Task Project_inline_collection_with_Concat(bool async)
    {
        await base.Project_inline_collection_with_Concat(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_equality_inline_collection(bool async)
    {
        await base.Column_collection_Where_equality_inline_collection(async);

        AssertSql();
    }

    public override async Task Inline_collection_List_Contains_with_mixed_value_types(bool async)
    {
        await base.Inline_collection_List_Contains_with_mixed_value_types(async);

        AssertSql(
"""
@__i_0='11'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE `p`.`Int` IN (999, @__i_0, `p`.`Id`, `p`.`Id` + `p`.`Int`)
""");
    }

    public override async Task Inline_collection_List_Min_with_two_values(bool async)
    {
        await base.Inline_collection_List_Min_with_two_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE LEAST(30, `p`.`Int`) = 30
""");
    }

    public override async Task Inline_collection_List_Max_with_two_values(bool async)
    {
        await base.Inline_collection_List_Max_with_two_values(async);

        AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE GREATEST(30, `p`.`Int`) = 30
""");
    }

    public override async Task Inline_collection_List_Min_with_three_values(bool async)
    {
        await base.Inline_collection_List_Min_with_three_values(async);

        AssertSql(
"""
@__i_0='25'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE LEAST(30, `p`.`Int`, @__i_0) = 25
""");
    }

    public override async Task Inline_collection_List_Max_with_three_values(bool async)
    {
        await base.Inline_collection_List_Max_with_three_values(async);

        AssertSql(
"""
@__i_0='35'

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE GREATEST(30, `p`.`Int`, @__i_0) = 35
""");
    }

    public override async Task Inline_collection_of_nullable_value_type_Min(bool async)
    {
        if (AppConfig.ServerVersion.Supports.FieldReferenceInTableValueConstructor)
        {
            await base.Inline_collection_of_nullable_value_type_Min(async);

            AssertSql(
"""
@__i_0='25' (Nullable = true)

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT MIN(`v`.`Value`)
    FROM (SELECT CAST(30 AS signed) AS `Value` UNION ALL VALUES ROW(`p`.`Int`), ROW(@__i_0)) AS `v`) = 25
""");
        }
        else
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Inline_collection_of_nullable_value_type_Min(async));
            Assert.True(exception.Message is "Field reference 'p.Int' can't be used in table value constructor"
                                          or "Unknown table 'p' in order clause");
        }
    }

    public override async Task Inline_collection_of_nullable_value_type_Max(bool async)
    {
        if (AppConfig.ServerVersion.Supports.FieldReferenceInTableValueConstructor)
        {
            await base.Inline_collection_of_nullable_value_type_Max(async);

            AssertSql(
"""
@__i_0='35' (Nullable = true)

SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT MAX(`v`.`Value`)
    FROM (SELECT CAST(30 AS signed) AS `Value` UNION ALL VALUES ROW(`p`.`Int`), ROW(@__i_0)) AS `v`) = 35
""");
        }
        else
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Inline_collection_of_nullable_value_type_Max(async));
            Assert.True(exception.Message is "Field reference 'p.Int' can't be used in table value constructor"
                                          or "Unknown table 'p' in order clause");
        }
    }

    public override async Task Inline_collection_of_nullable_value_type_with_null_Min(bool async)
    {
        if (AppConfig.ServerVersion.Supports.FieldReferenceInTableValueConstructor)
        {
            await base.Inline_collection_of_nullable_value_type_with_null_Min(async);

            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT MIN(`v`.`Value`)
    FROM (SELECT CAST(30 AS signed) AS `Value` UNION ALL VALUES ROW(`p`.`NullableInt`), ROW(NULL)) AS `v`) = 30
""");
        }
        else
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Inline_collection_of_nullable_value_type_with_null_Min(async));
            Assert.True(exception.Message is "Field reference 'p.NullableInt' can't be used in table value constructor"
                                          or "Unknown table 'p' in order clause");
        }
    }

    public override async Task Inline_collection_of_nullable_value_type_with_null_Max(bool async)
    {
        if (AppConfig.ServerVersion.Supports.FieldReferenceInTableValueConstructor)
        {
            await base.Inline_collection_of_nullable_value_type_with_null_Max(async);

            AssertSql(
"""
SELECT `p`.`Id`, `p`.`Bool`, `p`.`Bools`, `p`.`DateTime`, `p`.`DateTimes`, `p`.`Enum`, `p`.`Enums`, `p`.`Int`, `p`.`Ints`, `p`.`NullableInt`, `p`.`NullableInts`, `p`.`NullableString`, `p`.`NullableStrings`, `p`.`String`, `p`.`Strings`
FROM `PrimitiveCollectionsEntity` AS `p`
WHERE (
    SELECT MAX(`v`.`Value`)
    FROM (SELECT CAST(30 AS signed) AS `Value` UNION ALL VALUES ROW(`p`.`NullableInt`), ROW(NULL)) AS `v`) = 30
""");
        }
        else
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Inline_collection_of_nullable_value_type_with_null_Max(async));
            Assert.True(exception.Message is "Field reference 'p.NullableInt' can't be used in table value constructor"
                                          or "Unknown table 'p' in order clause");
        }
    }

    public override async Task Inline_collection_Contains_with_EF_Parameter(bool async)
    {
        await base.Inline_collection_Contains_with_EF_Parameter(async);

        AssertSql();
    }

    public override async Task Inline_collection_Count_with_column_predicate_with_EF_Parameter(bool async)
    {
        await base.Inline_collection_Count_with_column_predicate_with_EF_Parameter(async);

        AssertSql();
    }

    public override async Task Parameter_collection_HashSet_of_ints_Contains_int(bool async)
    {
        await base.Parameter_collection_HashSet_of_ints_Contains_int(async);

        AssertSql();
    }

    public override async Task Column_collection_Count_with_predicate(bool async)
    {
        await base.Column_collection_Count_with_predicate(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_Count(bool async)
    {
        await base.Column_collection_Where_Count(async);

        AssertSql();
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
    public override async Task Inline_collection_value_index_Column(bool async)
    {
        await base.Inline_collection_value_index_Column(async);

        AssertSql();
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
    public override async Task Inline_collection_List_value_index_Column(bool async)
    {
        await base.Inline_collection_List_value_index_Column(async);

        AssertSql();
    }

    public override async Task Column_collection_First(bool async)
    {
        await base.Column_collection_First(async);

        AssertSql();
    }

    public override async Task Column_collection_FirstOrDefault(bool async)
    {
        await base.Column_collection_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Column_collection_Single(bool async)
    {
        await base.Column_collection_Single(async);

        AssertSql();
    }

    public override async Task Column_collection_SingleOrDefault(bool async)
    {
        await base.Column_collection_SingleOrDefault(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_Skip(bool async)
    {
        await base.Column_collection_Where_Skip(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_Take(bool async)
    {
        await base.Column_collection_Where_Take(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_Skip_Take(bool async)
    {
        await base.Column_collection_Where_Skip_Take(async);

        AssertSql();
    }

    public override async Task Column_collection_Contains_over_subquery(bool async)
    {
        await base.Column_collection_Contains_over_subquery(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_ElementAt(bool async)
    {
        await base.Column_collection_Where_ElementAt(async);

        AssertSql();
    }

    public override async Task Column_collection_SelectMany_with_filter(bool async)
    {
        await base.Column_collection_SelectMany_with_filter(async);

        AssertSql();
    }

    public override async Task Column_collection_SelectMany_with_Select_to_anonymous_type(bool async)
    {
        await base.Column_collection_SelectMany_with_Select_to_anonymous_type(async);

        AssertSql();
    }

    public override async Task Parameter_collection_with_type_inference_for_JsonScalarExpression(bool async)
    {
        await base.Parameter_collection_with_type_inference_for_JsonScalarExpression(async);

        AssertSql();
    }

    public override async Task Column_collection_Where_Union(bool async)
    {
        await base.Column_collection_Where_Union(async);

        AssertSql();
    }

    public override async Task Project_inline_collection(bool async)
    {
        await base.Project_inline_collection(async);

        AssertSql(
"""
SELECT `p`.`String`
FROM `PrimitiveCollectionsEntity` AS `p`
""");
    }

    public override async Task Project_inline_collection_with_Union(bool async)
    {
        await base.Project_inline_collection_with_Union(async);

        AssertSql(
"""
SELECT `p`.`Id`, `u`.`Value`
FROM `PrimitiveCollectionsEntity` AS `p`
LEFT JOIN LATERAL (
    SELECT `p`.`String` AS `Value`
    UNION
    SELECT `p0`.`String` AS `Value`
    FROM `PrimitiveCollectionsEntity` AS `p0`
) AS `u` ON TRUE
ORDER BY `p`.`Id`
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    private PrimitiveCollectionsContext CreateContext()
        => Fixture.CreateContext();

    public class PrimitiveCollectionsQueryMySqlFixture : PrimitiveCollectionsQueryFixtureBase, ITestSqlLoggerFactory
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // modelBuilder.Entity<PrimitiveCollectionsEntity>().Property(p => p.Bools).HasColumnType("json");
        }
    }
}
