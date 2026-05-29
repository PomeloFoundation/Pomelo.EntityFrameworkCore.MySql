// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// VENDORED from EF Core release/9.0 test base (methods removed from the EF Core 10.0 shared base).

using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Xunit.Sdk;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query;

#nullable disable

public abstract class TPCGearsOfWarQueryMySqlTestBase<TFixture>(TFixture fixture) : TPCGearsOfWarQueryRelationalTestBase<TFixture>(fixture)
    where TFixture : TPCGearsOfWarQueryRelationalFixture, new()
{
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Array_access_on_byte_array(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(e => e.Banner5[2] == 0x06));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Bitwise_projects_values_in_select(bool async)
        => AssertFirst(
            async,
            ss => ss.Set<Gear>()
                .Where(g => (g.Rank & MilitaryRank.Corporal) == MilitaryRank.Corporal)
                .Select(
                    b => new
                    {
                        BitwiseTrue = (b.Rank & MilitaryRank.Corporal) == MilitaryRank.Corporal,
                        BitwiseFalse = (b.Rank & MilitaryRank.Corporal) == MilitaryRank.Sergeant,
                        BitwiseValue = b.Rank & MilitaryRank.Corporal
                    }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Byte_array_contains_literal(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(s => s.Banner.Contains((byte)1)),
            ss => ss.Set<Squad>().Where(s => s.Banner != null && s.Banner.Contains((byte)1)));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Byte_array_contains_parameter(bool async)
    {
        var someByte = (byte)1;
        return AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(s => s.Banner.Contains(someByte)),
            ss => ss.Set<Squad>().Where(s => s.Banner != null && s.Banner.Contains(someByte)));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Byte_array_filter_by_length_literal(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(w => w.Banner.Length == 2),
            ss => ss.Set<Squad>().Where(w => w.Banner != null && w.Banner.Length == 2));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Byte_array_filter_by_length_parameter(bool async)
    {
        var someByteArr = new[] { (byte)42, (byte)24 };
        return AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(w => w.Banner.Length == someByteArr.Length),
            ss => ss.Set<Squad>().Where(w => w.Banner != null && w.Banner.Length == someByteArr.Length));
    }

    [ConditionalFact]
    public virtual void Byte_array_filter_by_length_parameter_compiled()
    {
        var query = EF.CompileQuery(
            (GearsOfWarContext context, byte[] byteArrayParam)
                => context.Squads.Where(w => w.Banner.Length == byteArrayParam.Length).Count());

        using var context = CreateContext();
        var byteQueryParam = new[] { (byte)42, (byte)128 };

        Assert.Equal(2, query(context, byteQueryParam));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Byte_array_filter_by_SequenceEqual(bool async)
    {
        var byteArrayParam = new byte[] { 0x04, 0x05, 0x06, 0x07, 0x08 };

        return AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(s => s.Banner5.SequenceEqual(byteArrayParam)));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Contains_on_byte_array_property_using_byte_column(bool async)
        => AssertQuery(
            async,
            ss => from s in ss.Set<Squad>()
                  from l in ss.Set<LocustLeader>()
                  where s.Banner.Contains(l.ThreatLevelByte)
                  select new { s, l },
            elementSorter: e => (e.s.Id, e.l.Name),
            elementAsserter: (e, a) =>
            {
                AssertEqual(e.s, a.s);
                AssertEqual(e.l, a.l);
            });

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddDays(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddDays(1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddHours(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddHours(1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddMilliseconds(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddMilliseconds(300));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddMinutes(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddMinutes(1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddMonths(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddMonths(1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddSeconds(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddSeconds(1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_DateAdd_AddYears(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.AddYears(1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_to_unix_time_milliseconds(bool async)
    {
        var unixEpochMilliseconds = DateTimeOffset.UnixEpoch.ToUnixTimeMilliseconds();

        return AssertQuery(
            async,
            ss => ss.Set<Gear>()
                .Include(g => g.Squad.Missions)
                .Where(
                    s => s.Squad.Missions
                            .Where(m => unixEpochMilliseconds == m.Mission.Timeline.ToUnixTimeMilliseconds())
                            .FirstOrDefault()
                        == null));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task DateTimeOffset_to_unix_time_seconds(bool async)
    {
        var unixEpochSeconds = DateTimeOffset.UnixEpoch.ToUnixTimeSeconds();

        return AssertQuery(
            async,
            ss => ss.Set<Gear>()
                .Include(g => g.Squad.Missions)
                .Where(
                    s => s.Squad.Missions
                            .Where(m => unixEpochSeconds == m.Mission.Timeline.ToUnixTimeSeconds())
                            .FirstOrDefault()
                        == null));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Filter_with_new_Guid(bool async)
        => AssertQuery(
            async,
            ss => from t in ss.Set<CogTag>()
                  where t.Id == new Guid("DF36F493-463F-4123-83F9-6B135DEEB7BA")
                  select t);

    public virtual async Task Filter_with_new_Guid_closure(bool async)
    {
        var guid = "DF36F493-463F-4123-83F9-6B135DEEB7BD";

        await AssertQuery(
            async,
            ss => from t in ss.Set<CogTag>()
                  where t.Id == new Guid(guid)
                  select t);

        guid = "B39A6FBA-9026-4D69-828E-FD7068673E57";

        await AssertQuery(
            async,
            ss => from t in ss.Set<CogTag>()
                  where t.Id == new Guid(guid)
                  select t);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task First_on_byte_array(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Squad>().Where(e => e.Banner.First() == 0x02));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Order_by_TimeOnly_FromTimeSpan(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().OrderBy(x => TimeOnly.FromTimeSpan(x.Duration)),
            assertOrder: true);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_enum_has_flag(bool async)
        => AssertFirst(
            async,
            ss => ss.Set<Gear>()
                .Where(g => g.Rank.HasFlag(MilitaryRank.Corporal))
                .Select(
                    b => new
                    {
                        hasFlagTrue = b.Rank.HasFlag(MilitaryRank.Corporal), hasFlagFalse = b.Rank.HasFlag(MilitaryRank.Sergeant)
                    }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Time_of_day_datetimeoffset(bool async)
        => AssertQueryScalar(
            async,
            ss => from m in ss.Set<Mission>()
                  select m.Timeline.TimeOfDay);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task TimeSpan_Hours(bool async)
        => AssertQueryScalar(
            async,
            ss => ss.Set<Mission>()
                .Select(m => m.Duration.Hours));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task TimeSpan_Milliseconds(bool async)
        => AssertQueryScalar(
            async,
            ss => ss.Set<Mission>()
                .Select(m => m.Duration.Milliseconds));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task TimeSpan_Minutes(bool async)
        => AssertQueryScalar(
            async,
            ss => ss.Set<Mission>()
                .Select(m => m.Duration.Minutes));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task TimeSpan_Seconds(bool async)
        => AssertQueryScalar(
            async,
            ss => ss.Set<Mission>()
                .Select(m => m.Duration.Seconds));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task ToString_guid_property_projection(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<CogTag>().Select(
                ct => new { A = ct.GearNickName, B = ct.Id.ToString() }),
            elementSorter: e => e.B,
            elementAsserter: (e, a) =>
            {
                Assert.Equal(e.A, a.A);
                Assert.Equal(e.B.ToLower(), a.B.ToLower());
            });

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_bitwise_and_enum(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => (g.Rank & MilitaryRank.Corporal) > 0));

        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => (g.Rank & MilitaryRank.Corporal) == MilitaryRank.Corporal));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_bitwise_and_integral(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => ((int)g.Rank & 1) == 1));

        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => ((long)g.Rank & 1L) == 1L));

        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => ((short)g.Rank & 1) == 1));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_and_nullable_enum_with_constant(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => (w.AmmunitionType & AmmunitionType.Cartridge) > 0));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_and_nullable_enum_with_non_nullable_parameter(bool async)
    {
        var ammunitionType = AmmunitionType.Cartridge;

        return AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => (w.AmmunitionType & ammunitionType) > 0));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_and_nullable_enum_with_null_constant(bool async)
    {
        return AssertQuery(
            async,
#pragma warning disable CS0458 // The result of the expression is always 'null'
            ss => ss.Set<Weapon>().Where(w => (w.AmmunitionType & null) > 0),
#pragma warning restore CS0458 // The result of the expression is always 'null'
            assertEmpty: true);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_bitwise_and_nullable_enum_with_nullable_parameter(bool async)
    {
        AmmunitionType? ammunitionType = AmmunitionType.Cartridge;

        await AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => (w.AmmunitionType & ammunitionType) > 0));

        ammunitionType = null;

        await AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => (w.AmmunitionType & ammunitionType) > 0),
            assertEmpty: true);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_or_enum(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => (g.Rank | MilitaryRank.Corporal) > 0));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_AddDays(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.AddDays(3) == new DateOnly(1990, 11, 13)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_AddMonths(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.AddMonths(3) == new DateOnly(1991, 2, 10)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_AddYears(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.AddYears(3) == new DateOnly(1993, 11, 10)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_Day(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.Day == 10).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_DayOfWeek(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.DayOfWeek == DayOfWeek.Saturday).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_DayOfYear(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.DayOfYear == 314).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_FromDateTime_compared_to_constant_and_parameter(bool async)
    {
        var prm = new DateOnly(2, 10, 11);

        return AssertQuery(
            async,
            ss => ss.Set<CogTag>().Where(x => new[] { prm, new DateOnly(15, 3, 7) }.Contains(DateOnly.FromDateTime(x.IssueDate))));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_FromDateTime_compared_to_property(bool async)
        => AssertQuery(
            async,
            ss => from t in ss.Set<CogTag>()
                  from m in ss.Set<Mission>()
                  where DateOnly.FromDateTime(t.IssueDate) > m.Date
                  select new { TagId = t.Id, MissionId = m.Id },
            elementSorter: e => (e.TagId, e.MissionId));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_Month(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.Month == 11).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_DateOnly_Year(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Date.Year == 1990).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_date_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Date > new DateTimeOffset().Date
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_day_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Day == 2
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_dayofyear_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.DayOfYear == 2
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_hour_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Hour == 10
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_millisecond_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Millisecond == 0
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_milliseconds_parameter_and_constant(bool async)
    {
        var dateTimeOffset = new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0));

        // Literal where clause
        var p = Expression.Parameter(typeof(Mission), "i");
        var dynamicWhere = Expression.Lambda<Func<Mission, bool>>(
            Expression.Equal(
                Expression.Property(p, "Timeline"),
                Expression.Constant(dateTimeOffset)
            ), p);

        return AssertCount(
            async,
            ss => ss.Set<Mission>().Where(dynamicWhere),
            ss => ss.Set<Mission>().Where(m => m.Timeline == dateTimeOffset));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_minute_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Minute == 0
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_month_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Month == 1
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_now(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline != DateTimeOffset.Now
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_second_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Second == 0
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_utcnow(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline != DateTimeOffset.UtcNow
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetimeoffset_year_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Year == 2
                  select m);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_enum(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank == MilitaryRank.Sergeant));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_enum_has_flag(bool async)
    {
        // Constant
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank.HasFlag(MilitaryRank.Corporal)));

        // Expression
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank.HasFlag(MilitaryRank.Corporal | MilitaryRank.Captain)),
            assertEmpty: true);

        // Casting
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank.HasFlag((MilitaryRank)1)));

        // Casting to nullable
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank.HasFlag((MilitaryRank?)1)));

        // QuerySource
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => MilitaryRank.Corporal.HasFlag(g.Rank)));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_enum_has_flag_subquery(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(
                g => g.Rank.HasFlag(
                    ss.Set<Gear>().OrderBy(x => x.Nickname).ThenBy(x => x.SquadId).Select(x => x.Rank).FirstOrDefault())));

        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(
                g => MilitaryRank.Corporal.HasFlag(
                    ss.Set<Gear>().OrderBy(x => x.Nickname).ThenBy(x => x.SquadId).Select(x => x.Rank).FirstOrDefault())));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_enum_has_flag_subquery_client_eval(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(
                g => g.Rank.HasFlag(ss.Set<Gear>().OrderBy(x => x.Nickname).ThenBy(x => x.SquadId).First().Rank)));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_enum_has_flag_subquery_with_pushdown(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(
                g => g.Rank.HasFlag(ss.Set<Gear>().OrderBy(x => x.Nickname).ThenBy(x => x.SquadId).FirstOrDefault().Rank)));

        await AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(
                g => MilitaryRank.Corporal.HasFlag(
                    ss.Set<Gear>().OrderBy(x => x.Nickname).ThenBy(x => x.SquadId).FirstOrDefault().Rank)));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_enum_has_flag_with_non_nullable_parameter(bool async)
    {
        var parameter = MilitaryRank.Corporal;

        return AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank.HasFlag(parameter)));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_has_flag_with_nullable_parameter(bool async)
    {
        MilitaryRank? parameter = MilitaryRank.Corporal;

        return AssertQuery(
            async,
            ss => ss.Set<Gear>().Where(g => g.Rank.HasFlag(parameter)));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_nullable_enum_with_constant(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => w.AmmunitionType == AmmunitionType.Cartridge));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_nullable_enum_with_non_nullable_parameter(bool async)
    {
        var ammunitionType = AmmunitionType.Cartridge;

        return AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => w.AmmunitionType == ammunitionType));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_nullable_enum_with_null_constant(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => w.AmmunitionType == null));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Where_nullable_enum_with_nullable_parameter(bool async)
    {
        AmmunitionType? ammunitionType = AmmunitionType.Cartridge;

        await AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => w.AmmunitionType == ammunitionType));

        ammunitionType = null;

        await AssertQuery(
            async,
            ss => ss.Set<Weapon>().Where(w => w.AmmunitionType == ammunitionType));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_Add_TimeSpan(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.Add(new TimeSpan(3, 0, 0)) == new TimeOnly(13, 15, 50, 500)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_AddHours(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.AddHours(3) == new TimeOnly(13, 15, 50, 500)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_AddMinutes(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.AddMinutes(3) == new TimeOnly(10, 18, 50, 500)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_FromDateTime_compared_to_constant(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<CogTag>().Where(x => TimeOnly.FromDateTime(x.IssueDate.AddHours(x.Note.Length)) > new TimeOnly(9, 0, 0)));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_FromDateTime_compared_to_parameter(bool async)
    {
        var time = new TimeOnly(2, 0, 0);

        return AssertQuery(
            async,
            ss => ss.Set<CogTag>().Where(x => x.Gear != null && TimeOnly.FromDateTime(x.IssueDate.AddHours(x.Gear.SquadId)) == time));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_FromDateTime_compared_to_property(bool async)
        => AssertQuery(
            async,
            ss => from t in ss.Set<CogTag>()
                  from m in ss.Set<Mission>()
                  where TimeOnly.FromDateTime(t.IssueDate) == m.Time
                  select new { TagId = t.Id, MissionId = m.Id },
            elementSorter: e => (e.TagId, e.MissionId));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_FromTimeSpan_compared_to_parameter(bool async)
    {
        var time = new TimeOnly(1, 2, 3);

        return AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(x => TimeOnly.FromTimeSpan(x.Duration) == time));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_FromTimeSpan_compared_to_property(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(x => TimeOnly.FromTimeSpan(x.Duration) < x.Time));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_Hour(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.Hour == 10).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_IsBetween(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.IsBetween(new TimeOnly(10, 0, 0), new TimeOnly(11, 0, 0))).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_Millisecond(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.Millisecond == 500).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_Minute(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.Minute == 15).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_Second(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time.Second == 50).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeOnly_subtract_TimeOnly(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Time - new TimeOnly(10, 0, 0) == new TimeSpan(0, 0, 15, 50, 500)).AsTracking());

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeSpan_Hours(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>()
                .Where(m => m.Duration.Hours == 1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeSpan_Milliseconds(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>()
                .Where(m => m.Duration.Milliseconds == 456));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeSpan_Minutes(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>()
                .Where(m => m.Duration.Minutes == 2));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_TimeSpan_Seconds(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Mission>()
                .Where(m => m.Duration.Seconds == 3));

}

