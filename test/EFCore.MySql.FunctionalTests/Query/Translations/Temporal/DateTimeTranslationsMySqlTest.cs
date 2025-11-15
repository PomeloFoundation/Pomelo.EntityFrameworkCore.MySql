using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.BasicTypesModel;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query.Translations.Temporal;

/// <remarks>
///     Note that <see cref="BasicTypesEntity.DateTime" /> is mapped to PG <c>timestamp with time zone</c>, as is the provider default;
///     this causes issues with various tests. See also <see cref="DateTimeTranslationsWithoutTimeZoneTest" />, which
///     explicitly maps <see cref="BasicTypesEntity.DateTime" /> to <c>timestamp without time zone</c>.
/// </remarks>
public class DateTimeTranslationsMySqlTest : DateTimeTranslationsTestBase<BasicTypesQueryMySqlFixture>
{
    public DateTimeTranslationsMySqlTest(BasicTypesQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
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
@myDatetime='2015-04-10T00:00:00.0000000' (DbType = DateTime)

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CURRENT_TIMESTAMP(6) <> @myDatetime
""");
    }

    public override async Task UtcNow()
    {
        // Overriding to set Kind=Utc for timestamptz
        var myDatetime = DateTime.SpecifyKind(new DateTime(2015, 4, 10), DateTimeKind.Utc);

        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>().Where(c => DateTime.UtcNow != myDatetime));

        AssertSql(
"""
@myDatetime='2015-04-10T00:00:00.0000000Z' (DbType = DateTime)

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE UTC_TIMESTAMP(6) <> @myDatetime
""");
    }

    // DateTime.Today returns a Local DateTime, which can't be compared with timestamptz
    // (see TemporalTranslationsMySqlTimestampWithoutTimeZoneTest for a working version of this test)
    public override Task Today()
        => Assert.ThrowsAsync<NotSupportedException>(() => base.Today());

    public override async Task Date()
    {
        // Overriding to set Kind=Utc for timestamptz
        var myDatetime = DateTime.SpecifyKind(new DateTime(1998, 5, 4), DateTimeKind.Utc);

        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>().Where(o => o.DateTime.Date == myDatetime));

        AssertSql(
"""
@myDatetime='1998-05-04T00:00:00.0000000Z' (DbType = DateTime)

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CONVERT(`b`.`DateTime`, date) = @myDatetime
""");
    }

    public override async Task AddYear()
    {
        await base.AddYear();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(year FROM DATE_ADD(`b`.`DateTime`, INTERVAL CAST(1 AS signed) year)) = 1999
""");
    }

    public override async Task Year()
    {
        await base.Year();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(year FROM `b`.`DateTime`) = 1998
""");
    }

    public override async Task Month()
    {
        await base.Month();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(month FROM `b`.`DateTime`) = 5
""");
    }

    public override async Task DayOfYear()
    {
        await base.DayOfYear();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE DAYOFYEAR(`b`.`DateTime`) = 124
""");
    }

    public override async Task Day()
    {
        await base.Day();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(day FROM `b`.`DateTime`) = 4
""");
    }

    public override async Task Hour()
    {
        await base.Hour();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(hour FROM `b`.`DateTime`) = 15
""");
    }

    public override async Task Minute()
    {
        await base.Minute();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(minute FROM `b`.`DateTime`) = 30
""");
    }

    public override async Task Second()
    {
        await base.Second();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE EXTRACT(second FROM `b`.`DateTime`) = 10
""");
    }

    // SQL translation not implemented, too annoying
    public override Task Millisecond()
        => AssertTranslationFailed(() => base.Millisecond());

    public override async Task TimeOfDay()
    {
        await base.TimeOfDay();

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE CAST(`b`.`DateTime` AS time(6)) = TIME '00:00:00'
""");
    }

    public override async Task subtract_and_TotalDays()
    {
        // Overriding to set Kind=Utc for timestamptz
        var date = DateTime.SpecifyKind(new DateTime(1997, 1, 1), DateTimeKind.Utc);

        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>().Where(o => (o.DateTime - date).TotalDays > 365));

        AssertSql(
            """
@date='1997-01-01T00:00:00.0000000Z' (DbType = DateTime)

SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE date_part('epoch', b."DateTime" - @date) / 86400.0 > 365.0
""");
    }

    // DateTime.Parse() returns either a Local or Unspecified DateTime, which can't be compared with timestamptz
    // (see TemporalTranslationsMySqlTimestampWithoutTimeZoneTest for a working version of this test)
    public override Task Parse_with_constant()
        => Assert.ThrowsAsync<ArgumentException>(() => base.Parse_with_constant());

    // DateTime.Parse() returns either a Local or Unspecified DateTime, which can't be compared with timestamptz
    // (see TemporalTranslationsMySqlTimestampWithoutTimeZoneTest for a working version of this test)
    public override Task Parse_with_parameter()
        => Assert.ThrowsAsync<ArgumentException>(() => base.Parse_with_parameter());

    public override async Task New_with_constant()
    {
        // Overriding to set Kind=Utc for timestamptz
        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>().Where(o => o.DateTime == new DateTime(1998, 5, 4, 15, 30, 10, DateTimeKind.Utc)));

        AssertSql(
"""
SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE `b`.`DateTime` = TIMESTAMP '1998-05-04 15:30:10'
""");
    }

    public override async Task New_with_parameters()
    {
        // Overriding to set Kind=Utc for timestamptz
        var year = 1998;
        var month = 5;
        var date = 4;
        var hour = 15;

        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>().Where(o => o.DateTime == new DateTime(year, month, date, hour, 30, 10, DateTimeKind.Utc)));

        AssertSql(
"""
@p='1998-05-04T15:30:10.0000000Z' (DbType = DateTime)

SELECT `b`.`Id`, `b`.`Bool`, `b`.`Byte`, `b`.`ByteArray`, `b`.`DateOnly`, `b`.`DateTime`, `b`.`DateTimeOffset`, `b`.`Decimal`, `b`.`Double`, `b`.`Enum`, `b`.`FlagsEnum`, `b`.`Float`, `b`.`Guid`, `b`.`Int`, `b`.`Long`, `b`.`Short`, `b`.`String`, `b`.`TimeOnly`, `b`.`TimeSpan`
FROM `BasicTypesEntities` AS `b`
WHERE `b`.`DateTime` = @p
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
