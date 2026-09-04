using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.BasicTypesModel;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query.Translations.Temporal;

public class DateTimeOffsetTranslationsMySqlTest : DateTimeOffsetTranslationsTestBase<BasicTypesQueryMySqlFixture>
{
    public DateTimeOffsetTranslationsMySqlTest(BasicTypesQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Now()
    {
        await base.Now();

        AssertSql(
            """
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE `b`.`DateTimeOffset` <> UTC_TIMESTAMP(6)
""");
    }

    public override async Task UtcNow()
    {
        await base.UtcNow();

        AssertSql(
            """
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE `b`.`DateTimeOffset` <> UTC_TIMESTAMP(6)
""");
    }

    public override async Task Date()
    {
        await base.Date();

        AssertSql(
"""
@Date='0001-01-01T00:00:00.0000000' (DbType = DateTime)

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CONVERT(`b`.`DateTimeOffset`, date) > @Date
""");
    }

    public override async Task Year()
    {
        await base.Year();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(year FROM `b`.`DateTimeOffset`) = 1998
""");
    }

    public override async Task Month()
    {
        await base.Month();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(month FROM `b`.`DateTimeOffset`) = 5
""");
    }

    public override async Task DayOfYear()
    {
        await base.DayOfYear();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE DAYOFYEAR(`b`.`DateTimeOffset`) = 124
""");
    }

    public override async Task Day()
    {
        await base.Day();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(day FROM `b`.`DateTimeOffset`) = 4
""");
    }

    public override async Task Hour()
    {
        await base.Hour();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(hour FROM `b`.`DateTimeOffset`) = 15
""");
    }

    public override async Task Minute()
    {
        await base.Minute();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(minute FROM `b`.`DateTimeOffset`) = 30
""");
    }

    public override async Task Second()
    {
        await base.Second();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(second FROM `b`.`DateTimeOffset`) = 10
""");
    }

    public override async Task Millisecond()
    {
        await base.Millisecond();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (EXTRACT(microsecond FROM `b`.`DateTimeOffset`)) DIV (1000) = 123
""");
    }

    // MySQL supports Microsecond via EXTRACT(MICROSECOND ...) % 1000.
    public override async Task Microsecond()
    {
        await base.Microsecond();

        AssertSql(
            """
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (EXTRACT(MICROSECOND FROM `b`.`DateTimeOffset`) % 1000) = 456
""");
    }

    // MySQL has max microsecond precision; Nanosecond is always 0.
    public override async Task Nanosecond()
    {
        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>().Where(b => b.DateTimeOffset.Nanosecond != 0),
            assertEmpty: true);

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE FALSE
""");
    }

    public override async Task TimeOfDay()
    {
        await base.TimeOfDay();

        AssertSql(
"""
SELECT CAST(`b`.`DateTimeOffset` AS time(6))
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddYears()
    {
        await base.AddYears();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL CAST(1 AS signed) year)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddMonths()
    {
        await base.AddMonths();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL CAST(1 AS signed) month)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddDays()
    {
        await base.AddDays();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL CAST(1.0 AS signed) day)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddHours()
    {
        await base.AddHours();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL CAST(1.0 AS signed) hour)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddMinutes()
    {
        await base.AddMinutes();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL CAST(1.0 AS signed) minute)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddSeconds()
    {
        await base.AddSeconds();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL CAST(1.0 AS signed) second)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task AddMilliseconds()
    {
        await base.AddMilliseconds();

        AssertSql(
"""
SELECT DATE_ADD(`b`.`DateTimeOffset`, INTERVAL 1000 * CAST(300.0 AS signed) microsecond)
FROM `BasicTypesEntities` AS `b`
""");
    }

    public override async Task ToUnixTimeMilliseconds()
    {
        await base.ToUnixTimeMilliseconds();

        AssertSql(
"""
@unixEpochMilliseconds='894295810000'

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE (TIMESTAMPDIFF(microsecond, TIMESTAMP '1970-01-01 00:00:00', `b`.`DateTimeOffset`)) DIV (1000) = @unixEpochMilliseconds
""");
    }

    public override async Task ToUnixTimeSecond()
    {
        await base.ToUnixTimeSecond();

        AssertSql(
"""
@unixEpochSeconds='894295810'

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE TIMESTAMPDIFF(second, TIMESTAMP '1970-01-01 00:00:00', `b`.`DateTimeOffset`) = @unixEpochSeconds
""");
    }

    public override async Task Milliseconds_parameter_and_constant()
    {
        await base.Milliseconds_parameter_and_constant();

        AssertSql(
"""
SELECT COUNT(*)
FROM `BasicTypesEntities` AS `b`
WHERE `b`.`DateTimeOffset` = TIMESTAMP '1902-01-02 08:30:00'
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
