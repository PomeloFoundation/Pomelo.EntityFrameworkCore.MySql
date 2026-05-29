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
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.BulkUpdates;

#nullable disable

public abstract class NorthwindBulkUpdatesMySqlTestBase<TFixture>(TFixture fixture, ITestOutputHelper testOutputHelper) : NorthwindBulkUpdatesRelationalTestBase<TFixture>(fixture, testOutputHelper)
    where TFixture : NorthwindBulkUpdatesRelationalFixture<NoopModelCustomizer>, new()
{
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Delete_with_left_join(bool async)
        => AssertDelete(
            async,
            ss => from od in ss.Set<OrderDetail>().Where(e => e.OrderID < 10276)
                  join o in ss.Set<Order>().Where(o => o.OrderID < 10300).OrderBy(e => e.OrderID).Skip(0).Take(100)
                      on od.OrderID equals o.OrderID into grouping
                  from o in grouping.DefaultIfEmpty()
                  select od,
            rowsAffectedCount: 74);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Update_with_invalid_lambda_throws(bool async)
        => AssertUpdate(
            async,
            ss => ss.Set<OrderDetail>().Where(od => od.OrderID < 10250),
            e => e,
            s => s.Maybe(e => e),
            rowsAffectedCount: 0);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Update_with_left_join_set_constant(bool async)
        => AssertUpdate(
            async,
            ss => from c in ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("F"))
                  join o in ss.Set<Order>().Where(o => o.OrderID < 10300)
                      on c.CustomerID equals o.CustomerID into grouping
                  from o in grouping.DefaultIfEmpty()
                  select new { c, o },
            e => e.c,
            s => s.SetProperty(c => c.c.ContactName, "Updated"),
            rowsAffectedCount: 8,
            (b, a) => Assert.All(a, c => Assert.Equal("Updated", c.ContactName)));

}

