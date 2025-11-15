using System;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Query.Translations.Temporal;

public class DateTimeOffsetTranslationsMySqlTest : DateTimeOffsetTranslationsTestBase<BasicTypesQueryMySqlFixture>
{
    public DateTimeOffsetTranslationsMySqlTest(BasicTypesQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // Not supported by design (DateTimeOffset with non-zero offset)
    public override Task Now()
        => Assert.ThrowsAsync<InvalidOperationException>(() => base.Now());

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

    // The test compares with new DateTimeOffset().Date, which MySql sends as -infinity, causing a discrepancy with the client behavior
    // which uses 1/1/1:0:0:0
    public override Task Date()
        => Assert.ThrowsAsync<EqualException>(() => base.Date());

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

    // SQL translation not implemented, too annoying
    public override Task Millisecond()
        => AssertTranslationFailed(() => base.Millisecond());

    // TODO: #3406
    public override Task Microsecond()
        => AssertTranslationFailed(() => base.Microsecond());

    // TODO: #3406
    public override Task Nanosecond()
        => AssertTranslationFailed(() => base.Nanosecond());

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

    public override Task ToUnixTimeMilliseconds()
        => AssertTranslationFailed(() => base.ToUnixTimeMilliseconds());

    public override Task ToUnixTimeSecond()
        => AssertTranslationFailed(() => base.ToUnixTimeSecond());

    public override async Task Milliseconds_parameter_and_constant()
    {
        await base.Milliseconds_parameter_and_constant();

        AssertSql(
"""
SELECT COUNT(*)
FROM `BasicTypesEntities` AS `b`
WHERE `b`.`DateTimeOffset` = TIMESTAMP '1902-01-02 08:30:00.123456'
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
