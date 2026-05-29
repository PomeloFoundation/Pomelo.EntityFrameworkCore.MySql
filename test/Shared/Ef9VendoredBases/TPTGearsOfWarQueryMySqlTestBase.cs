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

public abstract class TPTGearsOfWarQueryMySqlTestBase<TFixture>(TFixture fixture) : TPTGearsOfWarQueryRelationalTestBase<TFixture>(fixture)
    where TFixture : TPTGearsOfWarQueryRelationalFixture, new()
{
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
    public virtual Task Where_datetimeoffset_hour_component(bool async)
        => AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                  where m.Timeline.Hour == 10
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

}

