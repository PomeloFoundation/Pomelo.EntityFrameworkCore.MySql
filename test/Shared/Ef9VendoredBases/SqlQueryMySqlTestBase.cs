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

public abstract class SqlQueryMySqlTestBase<TFixture>(TFixture fixture) : SqlQueryTestBase<TFixture>(fixture)
    where TFixture : NorthwindQueryRelationalFixture<NoopModelCustomizer>, new()
{
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Multiple_occurrences_of_SqlQuery_with_db_parameter_adds_parameter_only_once(bool async)
    {
        using var context = CreateContext();
        var city = "Seattle";
        var qqlQuery = context.Database.SqlQueryRaw<UnmappedCustomer>(
            NormalizeDelimitersInRawString(@"SELECT * FROM [Customers] WHERE [City] = {0}"),
            CreateDbParameter("city", city));

        var query = qqlQuery.Intersect(qqlQuery);

        var actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Single(actual);
    }

}

