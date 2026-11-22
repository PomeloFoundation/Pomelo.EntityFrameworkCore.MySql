using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query.Translations.Operators;

public class BitwiseOperatorTranslationsMySqlTest : BitwiseOperatorTranslationsTestBase<BasicTypesQueryMySqlFixture>
{
    public BitwiseOperatorTranslationsMySqlTest(BasicTypesQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Or()
    {
        await base.Or();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(CAST(`b`.`Int` AS signed) | `b`.`Long` AS signed) = 7
""",
                //
                """
SELECT CAST(CAST(`b`.`Int` AS signed) | `b`.`Long` AS signed)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Or_over_boolean()
    {
        await base.Or_over_boolean();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Int` = 12) OR (`b`.`String` = 'Seattle')
""",
                //
                """
SELECT (`b`.`Int` = 12) OR (`b`.`String` = 'Seattle')
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Or_multiple()
    {
        await base.Or_multiple();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(CAST(`b`.`Int` | `b`.`Short` AS signed) | `b`.`Long` AS signed) = 7
""");
    }

    public override async Task And()
    {
        await base.And();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(`b`.`Int` & `b`.`Short` AS signed) = 2
""",
                //
                """
SELECT CAST(`b`.`Int` & `b`.`Short` AS signed)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task And_over_boolean()
    {
        await base.And_over_boolean();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Int` = 8) AND (`b`.`String` = 'Seattle')
""",
                //
                """
SELECT (`b`.`Int` = 8) AND (`b`.`String` = 'Seattle')
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Xor()
    {
        await base.Xor();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(`b`.`Int` ^ `b`.`Short` AS signed) = 1
""",
                //
                """
SELECT CAST(`b`.`Int` ^ `b`.`Short` AS signed)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Xor_over_boolean()
    {
        await base.Xor_over_boolean();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Int` = `b`.`Short`) ^ (`b`.`String` = 'Seattle')
""");
    }

    public override async Task Complement()
    {
        await base.Complement();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(~`b`.`Int` AS signed) = -9
""");
    }

    public override async Task And_or_over_boolean()
    {
        await base.And_or_over_boolean();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Int` = 12) AND (`b`.`Short` = 12)) OR (`b`.`String` = 'Seattle')
""");
    }

    public override async Task Or_with_logical_or()
    {
        await base.Or_with_logical_or();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Int` = 12) OR (`b`.`Short` = 12)) OR (`b`.`String` = 'Seattle')
""");
    }

    public override async Task And_with_logical_and()
    {
        await base.And_with_logical_and();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Int` = 8) AND (`b`.`Short` = 8)) AND (`b`.`String` = 'Seattle')
""");
    }

    public override async Task Or_with_logical_and()
    {
        await base.Or_with_logical_and();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Int` = 8) OR (`b`.`Short` = 9)) AND (`b`.`String` = 'Seattle')
""");
    }

    public override async Task And_with_logical_or()
    {
        await base.And_with_logical_or();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Int` = 12) AND (`b`.`Short` = 12)) OR (`b`.`String` = 'Seattle')
""");
    }

    public override Task Left_shift()
        => AssertTranslationFailed(() => base.Left_shift());

    public override Task Right_shift()
        => AssertTranslationFailed(() => base.Right_shift());

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
