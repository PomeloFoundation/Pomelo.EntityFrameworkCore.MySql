// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// VENDORED from EF Core release/9.0 test base (methods removed from the EF Core 10.0 shared base).

using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query;

#nullable disable

public abstract class NorthwindMiscellaneousQueryMySqlTestBase<TFixture>(TFixture fixture) : NorthwindMiscellaneousQueryRelationalTestBase<TFixture>(fixture)
    where TFixture : NorthwindQueryFixtureBase<NoopModelCustomizer>, new()
{
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_bitwise_and(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Select(
                c => new { c.CustomerID, Value = c.CustomerID == "ALFKI" & c.CustomerID == "ANATR" }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_bitwise_and_or(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().OrderBy(c => c.CustomerID)
                .Select(
                    c => new { c.CustomerID, Value = c.CustomerID == "ALFKI" & c.CustomerID == "ANATR" | c.CustomerID == "ANTON" }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_bitwise_and_with_logical_and(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Select(
                c => new { c.CustomerID, Value = c.CustomerID == "ALFKI" & c.CustomerID == "ANATR" && c.CustomerID == "ANTON" }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_bitwise_or(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Select(
                c => new { c.CustomerID, Value = c.CustomerID == "ALFKI" | c.CustomerID == "ANATR" }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_bitwise_or_multiple(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().OrderBy(c => c.CustomerID)
                .Select(
                    c => new { c.CustomerID, Value = c.CustomerID == "ALFKI" | c.CustomerID == "ANATR" | c.CustomerID == "ANTON" }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Select_bitwise_or_with_logical_or(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Select(
                c => new { c.CustomerID, Value = c.CustomerID == "ALFKI" | c.CustomerID == "ANATR" || c.CustomerID == "ANTON" }));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_and_with_logical_and(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID == "ALFKI" & c.CustomerID == "ANATR" && c.CustomerID == "ANTON"),
            assertEmpty: true);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_and_with_logical_or(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID == "ALFKI" & c.CustomerID == "ANATR" || c.CustomerID == "ANTON"));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_binary_and(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => (o.OrderID & 10248) == 10248));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_binary_not(bool async)
    {
        var negatedId = ~10248;

        return AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => ~o.OrderID == negatedId));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_binary_or(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Order>().Where(o => (o.OrderID | 10248) == 10248));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_or_with_logical_and(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID == "ALFKI" | c.CustomerID == "ANATR" && c.Country == "Germany"));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Where_bitwise_or_with_logical_or(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID == "ALFKI" | c.CustomerID == "ANATR" || c.CustomerID == "ANTON"));

}

