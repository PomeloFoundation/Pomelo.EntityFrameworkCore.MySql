using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class MathTranslationsMySqlTest : MathTranslationsTestBase<BasicTypesQueryMySqlFixture>
{
    public MathTranslationsMySqlTest(BasicTypesQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Abs_decimal()
    {
        await base.Abs_decimal();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ABS(`b`.`Decimal`) = 9.5
""");
    }

    public override async Task Abs_int()
    {
        await base.Abs_int();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ABS(`b`.`Int`) = 9
""");
    }

    public override async Task Abs_double()
    {
        await base.Abs_double();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ABS(`b`.`Double`) = 9.5
""");
    }

    public override async Task Abs_float()
    {
        await base.Abs_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(ABS(`b`.`Float`) AS double) = 9.5
""");
    }

    public override async Task Ceiling()
    {
        await base.Ceiling();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CEILING(`b`.`Double`) = 9.0
""");
    }

    public override async Task Ceiling_float()
    {
        await base.Ceiling_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CEILING(`b`.`Float`) = 9
""");
    }

    public override async Task Floor_decimal()
    {
        await base.Floor_decimal();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE FLOOR(`b`.`Decimal`) = 8.0
""");
    }

    public override async Task Floor_double()
    {
        await base.Floor_double();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE FLOOR(`b`.`Double`) = 8.0
""");
    }

    public override async Task Floor_float()
    {
        await base.Floor_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE FLOOR(`b`.`Float`) = 8
""");
    }

    public override async Task Power()
    {
        await base.Power();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE POWER(CAST(`b`.`Int` AS double), 2.0) = 64.0
""");
    }

    public override async Task Power_float()
    {
        await base.Power_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (POWER(`b`.`Float`, 2) > 73) AND (POWER(`b`.`Float`, 2) < 74)
""");
    }

    public override async Task Round_decimal()
    {
        await base.Round_decimal();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ROUND(`b`.`Decimal`) = 9.0
""",
                //
                """
SELECT ROUND(`b`.`Decimal`)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Round_double()
    {
        await base.Round_double();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ROUND(`b`.`Double`) = 9.0
""",
                //
                """
SELECT ROUND(`b`.`Double`)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Round_float()
    {
        await base.Round_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ROUND(`b`.`Float`) = 9
""",
                //
                """
SELECT ROUND(`b`.`Float`)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Round_with_digits_decimal()
    {
        await base.Round_with_digits_decimal();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ROUND(`b`.`Decimal`, 1) = 255.1
""");
    }

    // PostgreSQL only has round(v, s) over numeric, may be possible to cast back and forth though
    public override Task Round_with_digits_double()
        => AssertTranslationFailed(() => base.Round_with_digits_double());

    // PostgreSQL only has round(v, s) over numeric, may be possible to cast back and forth though
    public override Task Round_with_digits_float()
        => AssertTranslationFailed(() => base.Round_with_digits_float());

    public override async Task Truncate_decimal()
    {
        await base.Truncate_decimal();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE TRUNCATE(`b`.`Decimal`, 0) = 8.0
""",
                //
                """
SELECT TRUNCATE(`b`.`Decimal`, 0)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Truncate_double()
    {
        await base.Truncate_double();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE TRUNCATE(`b`.`Double`, 0) = 8.0
""",
                //
                """
SELECT TRUNCATE(`b`.`Double`, 0)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Truncate_float()
    {
        await base.Truncate_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE TRUNCATE(`b`.`Float`, 0) = 8
""",
                //
                """
SELECT TRUNCATE(`b`.`Float`, 0)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task Truncate_project_and_order_by_it_twice()
    {
        await base.Truncate_project_and_order_by_it_twice();

        AssertSql(
"""
SELECT TRUNCATE(`b`.`Double`, 0) AS `A`
FROM `BasicTypesEntities` AS `b`
ORDER BY TRUNCATE(`b`.`Double`, 0)
""");
    }

    // issue #16038
    //            AssertSql(
    //                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0, 1) AS [A]
    //FROM [Orders] AS [o]
    //WHERE [o].[OrderID] < 10250
    //ORDER BY [A]");
    public override async Task Truncate_project_and_order_by_it_twice2()
    {
        await base.Truncate_project_and_order_by_it_twice2();

        AssertSql(
"""
SELECT TRUNCATE(`b`.`Double`, 0) AS `A`
FROM `BasicTypesEntities` AS `b`
ORDER BY TRUNCATE(`b`.`Double`, 0) DESC
""");
    }

    // issue #16038
    //            AssertSql(
    //                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0, 1) AS [A]
    //FROM [Orders] AS [o]
    //WHERE [o].[OrderID] < 10250
    //ORDER BY [A] DESC");
    public override async Task Truncate_project_and_order_by_it_twice3()
    {
        await base.Truncate_project_and_order_by_it_twice3();

        AssertSql(
"""
SELECT TRUNCATE(`b`.`Double`, 0) AS `A`
FROM `BasicTypesEntities` AS `b`
ORDER BY TRUNCATE(`b`.`Double`, 0) DESC
""");
    }

    public override async Task Exp()
    {
        await base.Exp();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXP(`b`.`Double`) > 1.0
""");
    }

    public override async Task Exp_float()
    {
        await base.Exp_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXP(`b`.`Float`) > 1
""");
    }

    public override async Task Log()
    {
        await base.Log();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Double` > 0.0) AND (LOG(`b`.`Double`) <> 0.0)
""");
    }

    public override async Task Log_float()
    {
        await base.Log_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Float` > 0) AND (LOG(`b`.`Float`) <> 0)
""");
    }

    // PostgreSQL only has log(x, base) over numeric, may be possible to cast back and forth though
    public override Task Log_with_newBase()
        => AssertTranslationFailed(() => base.Log_with_newBase());

    // PostgreSQL only has log(x, base) over numeric, may be possible to cast back and forth though
    public override Task Log_with_newBase_float()
        => AssertTranslationFailed(() => base.Log_with_newBase_float());

    public override async Task Log10()
    {
        await base.Log10();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Double` > 0.0) AND ((LOG10(`b`.`Double`) <> 0.0) OR LOG10(`b`.`Double`) IS NULL)
""");
    }

    public override async Task Log10_float()
    {
        await base.Log10_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Float` > 0) AND ((LOG10(`b`.`Float`) <> 0) OR LOG10(`b`.`Float`) IS NULL)
""");
    }

    public override async Task Log2()
        => await AssertTranslationFailed(() => base.Log2());

    public override async Task Sqrt()
    {
        await base.Sqrt();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Double` > 0.0) AND (SQRT(`b`.`Double`) > 0.0)
""");
    }

    public override async Task Sqrt_float()
    {
        await base.Sqrt_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (`b`.`Float` > 0) AND (SQRT(`b`.`Float`) > 0)
""");
    }

    public override async Task Sign()
    {
        await base.Sign();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE SIGN(`b`.`Double`) > 0
""");
    }

    public override async Task Sign_float()
    {
        await base.Sign_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE SIGN(`b`.`Float`) > 0
""");
    }

    public override async Task Max()
    {
        await base.Max();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE GREATEST(`b`.`Int`, `b`.`Short` - 3) = `b`.`Int`
""");
    }

    public override async Task Max_nested()
    {
        await base.Max_nested();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE GREATEST(`b`.`Short` - 3, `b`.`Int`, 1) = `b`.`Int`
""");
    }

    public override async Task Max_nested_twice()
    {
        await base.Max_nested_twice();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE GREATEST(1, `b`.`Int`, 2, `b`.`Short` - 3) = `b`.`Int`
""");
    }

    public override async Task Min()
    {
        await base.Min();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE LEAST(`b`.`Int`, `b`.`Short` + 3) = `b`.`Int`
""");
    }

    public override async Task Min_nested()
    {
        await base.Min_nested();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE LEAST(`b`.`Short` + 3, `b`.`Int`, 99999) = `b`.`Int`
""");
    }

    public override async Task Min_nested_twice()
    {
        await base.Min_nested_twice();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE LEAST(99999, `b`.`Int`, 99998, `b`.`Short` + 3) = `b`.`Int`
""");
    }

    public override async Task Degrees()
    {
        await base.Degrees();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE DEGREES(`b`.`Double`) > 0.0
""");
    }

    public override async Task Degrees_float()
    {
        await base.Degrees_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE DEGREES(`b`.`Float`) > 0
""");
    }

    public override async Task Radians()
    {
        await base.Radians();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE RADIANS(`b`.`Double`) > 0.0
""");
    }

    public override async Task Radians_float()
    {
        await base.Radians_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE RADIANS(`b`.`Float`) > 0
""");
    }

    #region Trigonometry

    public override async Task Acos()
    {
        await base.Acos();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Double` >= -1.0) AND (`b`.`Double` <= 1.0)) AND (ACOS(`b`.`Double`) > 1.0)
""");
    }

    public override async Task Acos_float()
    {
        await base.Acos_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Float` >= -1) AND (`b`.`Float` <= 1)) AND (ACOS(`b`.`Float`) > 0)
""");
    }

    public override async Task Acosh()
        => await AssertTranslationFailed(() => base.Acosh());

    public override async Task Asin()
    {
        await base.Asin();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Double` >= -1.0) AND (`b`.`Double` <= 1.0)) AND (ASIN(`b`.`Double`) > -1.7976931348623157E+308)
""");
    }

    public override async Task Asin_float()
    {
        await base.Asin_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ((`b`.`Float` >= -1) AND (`b`.`Float` <= 1)) AND (CAST(ASIN(`b`.`Float`) AS double) > -1.7976931348623157E+308)
""");
    }

    public override async Task Asinh()
        => await AssertTranslationFailed(() => base.Asinh());

    public override async Task Atan()
    {
        await base.Atan();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ATAN(`b`.`Double`) > 0.0
""");
    }

    public override async Task Atan_float()
    {
        await base.Atan_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ATAN(`b`.`Float`) > 0
""");
    }

    public override async Task Atanh()
        => await AssertTranslationFailed(() => base.Atanh());

    public override async Task Atan2()
    {
        await base.Atan2();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ATAN2(`b`.`Double`, 1.0) > 0.0
""");
    }

    public override async Task Atan2_float()
    {
        await base.Atan2_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE ATAN2(`b`.`Float`, 1) > 0
""");
    }

    public override async Task Cos()
    {
        await base.Cos();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE COS(`b`.`Double`) > 0.0
""");
    }

    public override async Task Cos_float()
    {
        await base.Cos_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE COS(`b`.`Float`) > 0
""");
    }

    public override async Task Cosh()
        => await AssertTranslationFailed(() => base.Cosh());

    public override async Task Sin()
    {
        await base.Sin();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE SIN(`b`.`Double`) > 0.0
""");
    }

    public override async Task Sin_float()
    {
        await base.Sin_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE SIN(`b`.`Float`) > 0
""");
    }

    public override async Task Sinh()
        => await AssertTranslationFailed(() => base.Sinh());

    public override async Task Tan()
    {
        await base.Tan();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE TAN(`b`.`Double`) > 0.0
""");
    }

    public override async Task Tan_float()
    {
        await base.Tan_float();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE TAN(`b`.`Float`) > 0
""");
    }

    public override async Task Tanh()
        => await AssertTranslationFailed(() => base.Tanh());

    #endregion Trigonometry

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
