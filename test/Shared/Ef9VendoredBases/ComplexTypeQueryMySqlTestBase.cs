// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// VENDORED from EF Core release/9.0 test base (methods removed from the EF Core 10.0 shared base).

using Microsoft.EntityFrameworkCore.TestModels.ComplexTypeModel;
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

public abstract class ComplexTypeQueryMySqlTestBase<TFixture>(TFixture fixture) : ComplexTypeQueryRelationalTestBase<TFixture>(fixture)
    where TFixture : ComplexTypeQueryRelationalFixtureBase, new()
{
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Complex_type_equals_null(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.ShippingAddress == null),
            assertEmpty: true);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Filter_on_property_inside_complex_type(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.ShippingAddress.ZipCode == 07728));

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Filter_on_property_inside_nested_complex_type(bool async)
        => AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.ShippingAddress.Country.Code == "DE"));

}

