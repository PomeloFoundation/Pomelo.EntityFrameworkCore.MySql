// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;


namespace Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations;

public class OwnedNavigationsPrimitiveCollectionMySqlTest(OwnedNavigationsMySqlFixture fixture, ITestOutputHelper testOutputHelper)
    : OwnedNavigationsPrimitiveCollectionRelationalTestBase<OwnedNavigationsMySqlFixture>(fixture, testOutputHelper)
{
    public override async Task Count()
    {
        await base.Count();

        AssertSql();
    }

    public override async Task Index()
    {
        await base.Index();

        AssertSql();
    }

    public override async Task Contains()
    {
        await base.Contains();

        AssertSql();
    }

    public override async Task Any_predicate()
    {
        await base.Any_predicate();

        AssertSql();
    }

    public override async Task Nested_Count()
    {
        await base.Nested_Count();

        AssertSql();
    }

    public override async Task Select_Sum()
    {
        await base.Select_Sum();

        AssertSql();
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());
}
