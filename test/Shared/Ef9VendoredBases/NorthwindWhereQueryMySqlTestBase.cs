// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// VENDORED from EF Core release/9.0 test base (methods removed from the EF Core 10.0 shared base).

using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query;

#nullable disable

public abstract class NorthwindWhereQueryMySqlTestBase<TFixture>(TFixture fixture) : NorthwindWhereQueryRelationalTestBase<TFixture>(fixture)
    where TFixture : NorthwindQueryFixtureBase<NoopModelCustomizer>, new()
{
    [ConditionalTheory]
    [InlineData(false)]
    public virtual Task Where_bitwise_xor(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => (c.CustomerID == "ALFKI") ^ true));

    //see issue #31917
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_concat_string_int_comparison1(bool async)
    {
        var i = 10;

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID + i == c.CompanyName).Select(c => c.CustomerID),
            assertEmpty: true);
    }

    //see issue #31917
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_concat_string_int_comparison2(bool async)
    {
        var i = 10;

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => i + c.CustomerID == c.CompanyName).Select(c => c.CustomerID),
            assertEmpty: true);
    }

    //see issue #31917
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_concat_string_int_comparison3(bool async)
    {
        var i = 10;
        var j = 21;

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => i + 20 + c.CustomerID + j + 42 == c.CompanyName).Select(c => c.CustomerID),
            assertEmpty: true);
    }

    //see issue #31917
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_concat_string_int_comparison4(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderID + o.CustomerID == o.CustomerID).Select(c => c.CustomerID),
            assertEmpty: true);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_date_component(bool async)
    {
        var myDatetime = new DateTime(1998, 5, 4);

        return AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Date == myDatetime));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_day_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Day == 4));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_dayOfYear_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.DayOfYear == 68));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_hour_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Hour == 0));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_millisecond_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Millisecond == 0));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_minute_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Minute == 0));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_month_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Month == 4));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_now(bool async)
    {
        var myDatetime = new DateTime(2015, 4, 10);

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => DateTime.Now != myDatetime));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_second_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Second == 0));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_today(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Employee>().Where(e => DateTime.Now.Date == DateTime.Today));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_utcnow(bool async)
    {
        var myDatetime = new DateTime(2015, 4, 10);

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => DateTime.UtcNow != myDatetime));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_datetime_year_component(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => o.OrderDate.Value.Year == 1998));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_string_concat_method_comparison_2(bool async)
    {
        var i = "A";
        var j = "B";

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => string.Concat(i, j, c.CustomerID) == "ABANATR").Select(c => c.CustomerID));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_string_concat_method_comparison_3(bool async)
    {
        var i = "A";
        var j = "B";
        var k = "C";

        return AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => string.Concat(i, j, k, c.CustomerID) == "ABCANTON").Select(c => c.CustomerID));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_string_indexof(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.City.IndexOf("Sea") != -1));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_string_length(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.City.Length == 6));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_string_replace(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.City.Replace("Sea", "Rea") == "Reattle"));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_string_substring(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.City.Substring(1, 2) == "ea"));

}

