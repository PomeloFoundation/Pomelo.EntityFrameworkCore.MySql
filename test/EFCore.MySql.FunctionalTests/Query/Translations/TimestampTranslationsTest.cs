using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

// The EF Core specification test suite has:
//
// * Northwind Orders.OrderDate and others: DateTime mapped to 'timestamp without time zone' (so Unspecified/Local)
// * GearsOfWar Mission.Timeline: DateTimeOffset (mapped to 'timestamp with time zone')
//
// But there's no DateTime mapped to 'timestamp with time zone' (so Utc).
// This test suite checks this and various PG-specific things.

namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class TimestampTranslationsTest : QueryTestBase<TimestampTranslationsTest.TimestampQueryFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public TimestampTranslationsTest(TimestampQueryFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    #region Basic mapping

    [ConditionalFact]
    public void DateTime_maps_to_timestamptz_by_default()
    {
        using var ctx = CreateContext();

        Assert.Equal(
            "timestamp with time zone",
            ctx.Model.GetEntityTypes().Single().GetProperty(nameof(Entity.TimestamptzDateTime)).GetColumnType());
    }

    [ConditionalFact]
    public void DateTime_array_maps_to_timestamptz_by_default()
    {
        using var ctx = CreateContext();

        Assert.Equal(
            "timestamp with time zone[]",
            ctx.Model.GetEntityTypes().Single().GetProperty(nameof(Entity.TimestamptzDateTimeArray)).GetColumnType());
    }

    [ConditionalFact]
    public async Task Cannot_insert_utc_datetime_into_timestamp()
    {
        await using var ctx = CreateContext();

        ctx.Entities.Add(new Entity { TimestampDateTime = DateTime.UtcNow });
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
        Assert.IsType<ArgumentException>(exception.InnerException);
    }

    [ConditionalFact]
    public async Task Cannot_insert_unspecified_datetime_into_timestamptz()
    {
        await using var ctx = CreateContext();

        ctx.Entities.Add(new Entity { TimestamptzDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified) });
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
        Assert.IsType<ArgumentException>(exception.InnerException);
    }

    [ConditionalFact]
    public async Task Cannot_insert_local_datetime_into_timestamptz()
    {
        await using var ctx = CreateContext();

        ctx.Entities.Add(new Entity { TimestamptzDateTime = DateTime.Now });
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
        Assert.IsType<ArgumentException>(exception.InnerException);
    }

    #endregion Basic mapping

    #region Comparisons

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task Compare_timestamp_column_to_local_DateTime_literal(bool async)
    {
        // Note that we're in the Europe/Berlin timezone (see MySqlTestStore below)
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestampDateTime == new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTime" = TIMESTAMP '1998-04-12T15:26:38'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task Compare_timestamp_column_to_local_DateTime_parameter(bool async)
    {
        var dateTime = new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local);

        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestampDateTime == dateTime));

        // The string representation of our local DateTime is generated with the local time zone (by the EF Core test infra),
        // so we can't assert on it.
        Assert.Contains(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTime" = @dateTime
""",
            Fixture.TestSqlLoggerFactory.SqlStatements.Single());
    }

    // Compare_timestamp_column_to_unspecified_DateTime_literal: requires translating DateTime.SpecifyKind

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task Compare_timestamp_column_to_unspecified_DateTime_parameter(bool async)
    {
        var dateTime = new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Unspecified);

        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestampDateTime == dateTime));

        AssertSql(
            """
@dateTime='1998-04-12T15:26:38.0000000'

SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTime" = @dateTime
""");
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamp_column_to_utc_DateTime_literal()
    {
        await using var ctx = CreateContext();

        await Assert.ThrowsAsync<ArgumentException>(
            () => ctx.Entities.Where(e => e.TimestampDateTime == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc))
                .ToListAsync());
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamp_column_to_utc_DateTime_parameter()
    {
        await using var ctx = CreateContext();

        var dateTime = new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc);

        await Assert.ThrowsAsync<ArgumentException>(
            () => ctx.Entities.Where(e => e.TimestampDateTime == dateTime)
                .ToListAsync());
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task Compare_timestamptz_column_to_utc_DateTime_literal(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestamptzDateTime == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestamptzDateTime" = TIMESTAMPTZ '1998-04-12T13:26:38Z'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task Compare_timestamptz_column_to_utc_DateTime_parameter(bool async)
    {
        var dateTime = new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc);

        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestamptzDateTime == dateTime));

        AssertSql(
            """
@dateTime='1998-04-12T13:26:38.0000000Z' (DbType = DateTime)

SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestamptzDateTime" = @dateTime
""");
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamptz_column_to_local_DateTime_literal()
    {
        await using var ctx = CreateContext();

        await Assert.ThrowsAsync<ArgumentException>(
            () => ctx.Entities.Where(e => e.TimestamptzDateTime == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local))
                .ToListAsync());
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamptz_column_to_local_DateTime_parameter()
    {
        await using var ctx = CreateContext();

        var dateTime = new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local);

        await Assert.ThrowsAsync<ArgumentException>(
            () => ctx.Entities.Where(e => e.TimestamptzDateTime == dateTime)
                .ToListAsync());
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamptz_column_to_unspecified_DateTime_literal()
    {
        await using var ctx = CreateContext();

        await Assert.ThrowsAsync<ArgumentException>(
            () => ctx.Entities.Where(e => e.TimestamptzDateTime == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified))
                .ToListAsync());
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamptz_column_to_unspecified_DateTime_parameter()
    {
        await using var ctx = CreateContext();

        var dateTime = new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified);

        await Assert.ThrowsAsync<ArgumentException>(
            () => ctx.Entities.Where(e => e.TimestamptzDateTime == dateTime)
                .ToListAsync());
    }

    [ConditionalFact]
    public async Task Cannot_compare_timestamptz_column_to_timestamp_column()
    {
        await using var ctx = CreateContext();

        await Assert.ThrowsAsync<NotSupportedException>(
            () => ctx.Entities.Where(e => e.TimestamptzDateTime == e.TimestampDateTime)
                .ToListAsync());
    }

    [ConditionalFact]
    public async Task Compare_timestamptz_column_to_timestamp_column_with_ToUniversalTime()
    {
        await using var ctx = CreateContext();

        // We can't use AssertQuery since the local (expected) evaluation is dependent on the machine's timezone, which is out of
        // our control.
        var count = await ctx.Set<Entity>()
            .Where(e => e.TimestamptzDateTime == e.TimestampDateTime.ToUniversalTime())
            .CountAsync();

        Assert.Equal(1, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "Entities" AS e
WHERE e."TimestamptzDateTime" = e."TimestampDateTime"::timestamptz
""");
    }

    [ConditionalFact]
    public async Task Compare_timestamptz_column_to_timestamp_column_with_ToLocalTime()
    {
        await using var ctx = CreateContext();

        // We can't use AssertQuery since the local (expected) evaluation is dependent on the machine's timezone, which is out of
        // our control.
        var count = await ctx.Set<Entity>()
            .Where(e => e.TimestamptzDateTime.ToLocalTime() == e.TimestampDateTime)
            .CountAsync();

        Assert.Equal(1, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "Entities" AS e
WHERE e."TimestamptzDateTime"::timestamp = e."TimestampDateTime"
""");
    }

    #endregion Comparisons

    #region Now

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_now(bool async)
    {
        var myDatetime = new DateTime(2015, 4, 10);

        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(c => DateTime.Now != myDatetime));

        AssertSql(
            """
@myDatetime='2015-04-10T00:00:00.0000000'

SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE now()::timestamp <> @myDatetime
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_utcnow(bool async)
    {
        var myDatetime = new DateTime(2015, 4, 10, 0, 0, 0, DateTimeKind.Utc);

        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(c => DateTime.UtcNow != myDatetime));

        AssertSql(
            """
@myDatetime='2015-04-10T00:00:00.0000000Z' (DbType = DateTime)

SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE now() <> @myDatetime
""");
    }

    #endregion Now

    #region Date member

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_date_on_timestamptz(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestamptzDateTime.Date == new DateTime(1998, 4, 12, 0, 0, 0, DateTimeKind.Utc)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE date_trunc('day', e."TimestamptzDateTime", 'UTC') = TIMESTAMPTZ '1998-04-12T00:00:00Z'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_date_on_timestamp(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => e.TimestampDateTime.Date == new DateTime(1998, 4, 12, 0, 0, 0, DateTimeKind.Local)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE date_trunc('day', e."TimestampDateTime") = TIMESTAMP '1998-04-12T00:00:00'
""");
    }

    #endregion

    #region DateTimeOffset

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateTimeOffset_DateTime(bool async)
    {
        // We only support UTC DateTimeOffset, so DateTimeOffset.DateTime is just a matter of converting to timestamp without time zone
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => e.TimestampDateTimeOffset.DateTime == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTimeOffset" AT TIME ZONE 'UTC' = TIMESTAMP '1998-04-12T13:26:38'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateTimeOffset_UtcDateTime(bool async)
    {
        // We only support UTC DateTimeOffset, so DateTimeOffset.UtcDateTime does nothing (type change on CLR change, no change on the
        // PG side.
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => e.TimestampDateTimeOffset.UtcDateTime == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTimeOffset" = TIMESTAMPTZ '1998-04-12T13:26:38Z'
""");
    }

    [ConditionalFact]
    public async Task DateTimeOffset_LocalDateTime()
    {
        // Note that we're in the Europe/Berlin timezone (see MySqlTestStore below)

        // We can't use AssertQuery since the local (expected) evaluation is dependent on the machine's timezone, which is out of
        // our control.
        await using var ctx = CreateContext();

        var count = await ctx.Set<Entity>()
            .Where(e => e.TimestampDateTimeOffset.LocalDateTime == new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local))
            .CountAsync();

        Assert.Equal(1, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "Entities" AS e
WHERE e."TimestampDateTimeOffset"::timestamp = TIMESTAMP '1998-04-12T15:26:38'
""");
    }

    [ConditionalTheory] // #2220
    [MemberData(nameof(IsAsyncData))]
    public async Task DateTimeOffset_Date_project(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Select(e => new { e.TimestampDateTimeOffset.Date }));

        AssertSql(
            """
SELECT date_trunc('day', e."TimestampDateTimeOffset" AT TIME ZONE 'UTC') AS "Date"
FROM "Entities" AS e
""");

        // Additional check to make sure Unspecified DateTimes are returned, as per the .NET DateTimeOffset.Date behavior
        // (AssertQuery above doesn't confirm this since Kind isn't taken into account for DateTime comparisons)
        await using var ctx = CreateContext();

        foreach (var date in ctx.Set<Entity>().Select(e => e.TimestampDateTimeOffset.Date))
        {
            Assert.Equal(DateTimeKind.Unspecified, date.Kind);
        }
    }

    #endregion DateTimeOffset

    #region DateTime constructors

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_ctor1(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e =>
                    new DateTime(e.TimestampDateTime.Year, e.TimestampDateTime.Month, 1) == new DateTime(1998, 4, 12)),
            assertEmpty: true);

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE make_date(date_part('year', e."TimestampDateTime")::int, date_part('month', e."TimestampDateTime")::int, 1) = TIMESTAMP '1998-04-12T00:00:00'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_ctor2(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e =>
                    new DateTime(e.TimestampDateTime.Year, e.TimestampDateTime.Month, 1, 0, 0, 0) == new DateTime(1998, 4, 12)),
            assertEmpty: true);

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE make_timestamp(date_part('year', e."TimestampDateTime")::int, date_part('month', e."TimestampDateTime")::int, 1, 0, 0, 0::double precision) = TIMESTAMP '1998-04-12T00:00:00'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_ctor3_local(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e =>
                    new DateTime(e.TimestampDateTime.Year, e.TimestampDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local)
                    == new DateTime(1996, 9, 11)),
            assertEmpty: true);

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE make_timestamp(date_part('year', e."TimestampDateTime")::int, date_part('month', e."TimestampDateTime")::int, 1, 0, 0, 0::double precision) = TIMESTAMP '1996-09-11T00:00:00'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_datetime_ctor3_utc(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                o =>
                    new DateTime(o.TimestamptzDateTime.Year, o.TimestamptzDateTime.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                    == new DateTime(1998, 4, 1, 0, 0, 0, DateTimeKind.Utc)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE make_timestamptz(date_part('year', e."TimestamptzDateTime" AT TIME ZONE 'UTC')::int, date_part('month', e."TimestamptzDateTime" AT TIME ZONE 'UTC')::int, 1, 0, 0, 0::double precision, 'UTC') = TIMESTAMPTZ '1998-04-01T00:00:00Z'
""");
    }

    #endregion DateTime constructors

    #region SpecifyKind

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateTime_SpecifyKind_on_timestamp_to_utc(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateTime.SpecifyKind(e.TimestampDateTime, DateTimeKind.Utc)
                    == new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTime" AT TIME ZONE 'UTC' = TIMESTAMPTZ '1998-04-12T15:26:38Z'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateTime_SpecifyKind_on_timestamptz_to_unspecified(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateTime.SpecifyKind(e.TimestamptzDateTime, DateTimeKind.Unspecified)
                    == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestamptzDateTime" AT TIME ZONE 'UTC' = TIMESTAMP '1998-04-12T13:26:38'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateTime_SpecifyKind_on_timestamptz_to_local(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateTime.SpecifyKind(e.TimestamptzDateTime, DateTimeKind.Local)
                    == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestamptzDateTime" AT TIME ZONE 'UTC' = TIMESTAMP '1998-04-12T13:26:38'
""");
    }

    [ConditionalFact]
    public async Task DateTime_SpecifyKind_with_parameter_kind_throws()
    {
        await using var ctx = CreateContext();

        var kind = DateTimeKind.Local;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => ctx.Set<Entity>().Where(e => DateTime.SpecifyKind(e.TimestamptzDateTime, kind) == default).ToListAsync());

        Assert.Equal("Translating SpecifyKind is only supported with a constant Kind argument", exception.Message);
    }

    #endregion SpecifyKind

    #region Time zone conversions

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_ConvertTimeBySystemTimeZoneId_on_DateTime_timestamptz_column(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                c => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(c.TimestamptzDateTime, "Europe/Berlin")
                    == new DateTime(1998, 4, 12, 15, 26, 38)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestamptzDateTime" AT TIME ZONE 'Europe/Berlin' = TIMESTAMP '1998-04-12T15:26:38'
""");
    }

    [ConditionalFact]
    public virtual async Task Where_ConvertTimeBySystemTimeZoneId_fails_on_DateTime_timestamp_column()
    {
        await using var ctx = CreateContext();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ctx.Set<Entity>().Where(
                c => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(c.TimestampDateTime, "Europe/Berlin")
                    == new DateTime(1998, 4, 12, 15, 26, 38)).ToListAsync());
    }

    [ConditionalFact]
    public virtual async Task Where_ConvertTimeToUtc_on_DateTime_timestamp_column()
    {
        // We can't use AssertQuery since the local (expected) evaluation is dependent on the machine's timezone, which is out of
        // our control.
        await using var ctx = CreateContext();

        var count = await ctx.Set<Entity>()
            .Where(e => TimeZoneInfo.ConvertTimeToUtc(e.TimestampDateTime) == new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc))
            .CountAsync();

        Assert.Equal(1, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "Entities" AS e
WHERE e."TimestampDateTime"::timestamptz = TIMESTAMPTZ '1998-04-12T13:26:38Z'
""");
    }

    [ConditionalFact]
    public virtual async Task Where_ConvertTimeToUtc_fails_on_DateTime_timestamptz_column()
    {
        await using var ctx = CreateContext();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ctx.Set<Entity>().Where(
                    c => TimeZoneInfo.ConvertTimeToUtc(c.TimestamptzDateTime)
                        == new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc))
                .ToListAsync());
    }

    #endregion Time zone conversions

    #region DateOnly

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task DateOnly_FromDateTime_with_timestamptz(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateOnly.FromDateTime(e.TimestamptzDateTime) == new DateOnly(1998, 4, 12)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE CAST(e."TimestamptzDateTime" AT TIME ZONE 'UTC' AS date) = DATE '1998-04-12'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task DateOnly_FromDateTime_with_timestamp(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateOnly.FromDateTime(e.TimestampDateTime) == new DateOnly(1998, 4, 12)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTime"::date = DATE '1998-04-12'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task DateOnly_ToDateTime_with_timestamptz(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateOnly.FromDateTime(e.TimestamptzDateTime).ToDateTime(new TimeOnly(15, 26, 38))
                    == new DateTime(1998, 4, 12, 15, 26, 38)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE CAST(e."TimestamptzDateTime" AT TIME ZONE 'UTC' AS date) + TIME '15:26:38' = TIMESTAMP '1998-04-12T15:26:38'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task DateOnly_DayNumber(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => DateOnly.FromDateTime(e.TimestamptzDateTime).DayNumber == 729490));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE CAST(e."TimestamptzDateTime" AT TIME ZONE 'UTC' AS date) - DATE '0001-01-01' = 729490
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task DateOnly_DayNumber_subtraction(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => DateOnly.FromDateTime(e.TimestamptzDateTime).DayNumber -
                    DateOnly.FromDateTime(e.TimestamptzDateTime - TimeSpan.FromDays(3)).DayNumber == 3));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE CAST(e."TimestamptzDateTime" AT TIME ZONE 'UTC' AS date) - CAST((e."TimestamptzDateTime" - INTERVAL '3 00:00:00') AT TIME ZONE 'UTC' AS date) = 3
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task DateOnly_FromDayNumber(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(e => DateOnly.FromDayNumber(e.Id) == new DateOnly(0001, 01, 03)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE DATE '0001-01-01' + e."Id" = DATE '0001-01-03'
""");
    }

    #endregion DateOnly

    #region TimeOnly

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task TimeOnly_FromDateTime_with_timestamp(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => TimeOnly.FromDateTime(e.TimestampDateTime) == new TimeOnly(15, 26, 38)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE e."TimestampDateTime"::time without time zone = TIME '15:26:38'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task TimeOnly_FromDateTime_with_timestamptz(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => TimeOnly.FromDateTime(e.TimestamptzDateTime) == new TimeOnly(13, 26, 38)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE CAST(e."TimestamptzDateTime" AT TIME ZONE 'UTC' AS time without time zone) = TIME '13:26:38'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task TimeOnly_ToTimeSpan(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Entity>().Where(
                e => TimeOnly.FromDateTime(e.TimestamptzDateTime) == new TimeOnly(13, 26, 38)));

        AssertSql(
            """
SELECT e."Id", e."TimestampDateTime", e."TimestampDateTimeArray", e."TimestampDateTimeOffset", e."TimestampDateTimeOffsetArray", e."TimestampDateTimeRange", e."TimestamptzDateTime", e."TimestamptzDateTimeArray", e."TimestamptzDateTimeRange"
FROM "Entities" AS e
WHERE CAST(e."TimestamptzDateTime" AT TIME ZONE 'UTC' AS time without time zone) = TIME '13:26:38'
""");
    }

    #endregion TimeOnly

    #region Support

    private TimestampQueryContext CreateContext()
        => Fixture.CreateContext();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    public class TimestampQueryContext(DbContextOptions options) : PoolableDbContext(options)
    {
        public DbSet<Entity> Entities { get; set; }

        public static async Task SeedAsync (TimestampQueryContext context)
        {
            context.Entities.AddRange(TimestampData.CreateEntities());
            await context.SaveChangesAsync();
        }
    }

    public class Entity
    {
        public int Id { get; set; }

        public DateTime TimestamptzDateTime { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime TimestampDateTime { get; set; }

        public DateTimeOffset TimestampDateTimeOffset { get; set; }

        public DateTime[] TimestamptzDateTimeArray { get; set; }

        [Column(TypeName = "timestamp without time zone[]")]
        public DateTime[] TimestampDateTimeArray { get; set; }

        public DateTimeOffset[] TimestampDateTimeOffsetArray { get; set; }
    }

    public class TimestampQueryFixture : SharedStoreFixtureBase<TimestampQueryContext>, IQueryFixtureBase, ITestSqlLoggerFactory
    {
        protected override string StoreName
            => "TimestampQueryTest";

        // Set the PostgreSQL TimeZone parameter to something local, to ensure that operations which take TimeZone into account
        // don't depend on the database's time zone, and also that operations which shouldn't take TimeZone into account indeed
        // don't.
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        private TimestampData _expectedData;

        protected override Task SeedAsync(TimestampQueryContext context)
            => TimestampQueryContext.SeedAsync(context);

        public Func<DbContext> GetContextCreator()
            => CreateContext;

        public ISetSource GetExpectedData()
            => _expectedData ??= new TimestampData();

        public IReadOnlyDictionary<Type, object> EntitySorters
            => new Dictionary<Type, Func<object, object>> { { typeof(Entity), e => ((Entity)e)?.Id } }
                .ToDictionary(e => e.Key, e => (object)e.Value);

        public IReadOnlyDictionary<Type, object> EntityAsserters
            => new Dictionary<Type, Action<object, object>>
            {
                {
                    typeof(Entity), (e, a) =>
                    {
                        Assert.Equal(e is null, a is null);
                        if (a is not null)
                        {
                            var ee = (Entity)e!;
                            var aa = (Entity)a;

                            Assert.Equal(ee.Id, aa.Id);

                            Assert.Equal(ee.TimestamptzDateTime, aa.TimestamptzDateTime);
                            Assert.Equal(ee.TimestampDateTime, aa.TimestampDateTime);
                            Assert.Equal(ee.TimestampDateTimeOffset, aa.TimestampDateTimeOffset);

                            Assert.Equal(ee.TimestamptzDateTimeArray, aa.TimestamptzDateTimeArray);
                            Assert.Equal(ee.TimestampDateTimeArray, aa.TimestampDateTimeArray);
                            Assert.Equal(ee.TimestampDateTimeOffsetArray, aa.TimestampDateTimeOffsetArray);
                        }
                    }
                }
            }.ToDictionary(e => e.Key, e => (object)e.Value);
    }

    protected class TimestampData : ISetSource
    {
        public IReadOnlyList<Entity> Entities { get; } = CreateEntities();

        public IQueryable<TEntity> Set<TEntity>()
            where TEntity : class
        {
            if (typeof(TEntity) == typeof(Entity))
            {
                return (IQueryable<TEntity>)Entities.AsQueryable();
            }

            throw new InvalidOperationException("Invalid entity type: " + typeof(TEntity));
        }

        public static IReadOnlyList<Entity> CreateEntities()
        {
            var utcDateTime1 = new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc);
            var localDateTime1 = new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local);
            var utcDateTime2 = new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Utc);
            var unspecifiedDateTime2 = new DateTime(2015, 1, 27, 10, 45, 12, 345, DateTimeKind.Unspecified);

            var utcDateTimeArray1 = new[]
            {
                new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc), new DateTime(1998, 4, 12, 17, 26, 38, DateTimeKind.Utc)
            };

            var localDateTimeArray1 = new[]
            {
                new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local), new DateTime(1998, 4, 13, 15, 26, 38, DateTimeKind.Local)
            };

            var utcDateTimeArray2 = new[]
            {
                new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Utc), new DateTime(2015, 1, 28, 8, 45, 12, 345, DateTimeKind.Utc)
            };

            var localDateTimeArray2 = new[]
            {
                new DateTime(2015, 1, 27, 10, 45, 12, 345, DateTimeKind.Unspecified),
                new DateTime(2015, 1, 28, 10, 45, 12, 345, DateTimeKind.Unspecified)
            };

            return new List<Entity>
            {
                new()
                {
                    Id = 1,
                    TimestamptzDateTime = utcDateTime1,
                    TimestampDateTime = localDateTime1,
                    TimestampDateTimeOffset = new DateTimeOffset(utcDateTime1),
                    TimestamptzDateTimeArray = utcDateTimeArray1,
                    TimestampDateTimeArray = localDateTimeArray1,
                    TimestampDateTimeOffsetArray = [new(utcDateTimeArray1[0]), new(utcDateTimeArray1[1])],
                },
                new()
                {
                    Id = 2,
                    TimestamptzDateTime = utcDateTime2,
                    TimestampDateTime = unspecifiedDateTime2,
                    TimestampDateTimeOffset = new DateTimeOffset(utcDateTime2),
                    TimestamptzDateTimeArray = utcDateTimeArray2,
                    TimestampDateTimeArray = localDateTimeArray2,
                    TimestampDateTimeOffsetArray = [new(utcDateTimeArray2[0]), new(utcDateTimeArray2[1])],
                }
            };
        }
    }

    #endregion
}
