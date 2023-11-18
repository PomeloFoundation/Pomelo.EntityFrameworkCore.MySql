using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

// Currently skipped by using `internal` instead of `public` as class access modifier.
[SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
internal class OptionalDependentQueryMySqlTest : OptionalDependentQueryTestBase<OptionalDependentQueryMySqlFixture>
{
    public OptionalDependentQueryMySqlTest(OptionalDependentQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        // Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Basic_projection_entity_with_all_optional(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Basic_projection_entity_with_all_optional(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesAllOptional] AS [e]
// """);
    }

    public override async Task Basic_projection_entity_with_some_required(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Basic_projection_entity_with_some_required(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesSomeRequired] AS [e]
// """);
    }

    public override async Task Filter_optional_dependent_with_all_optional_compared_to_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_optional_dependent_with_all_optional_compared_to_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesAllOptional] AS [e]
// WHERE [e].[Json] IS NULL
// """);
    }

    public override async Task Filter_optional_dependent_with_all_optional_compared_to_not_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_optional_dependent_with_all_optional_compared_to_not_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesAllOptional] AS [e]
// WHERE [e].[Json] IS NOT NULL
// """);
    }

    public override async Task Filter_optional_dependent_with_some_required_compared_to_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_optional_dependent_with_some_required_compared_to_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesSomeRequired] AS [e]
// WHERE [e].[Json] IS NULL
// """);
    }

    public override async Task Filter_optional_dependent_with_some_required_compared_to_not_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_optional_dependent_with_some_required_compared_to_not_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesSomeRequired] AS [e]
// WHERE [e].[Json] IS NOT NULL
// """);
    }

    public override async Task Filter_nested_optional_dependent_with_all_optional_compared_to_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_nested_optional_dependent_with_all_optional_compared_to_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesAllOptional] AS [e]
// WHERE JSON_QUERY([e].[Json], '$.OpNav1') IS NULL
// """);
    }

    public override async Task Filter_nested_optional_dependent_with_all_optional_compared_to_not_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_nested_optional_dependent_with_all_optional_compared_to_not_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesAllOptional] AS [e]
// WHERE JSON_QUERY([e].[Json], '$.OpNav2') IS NOT NULL
// """);
    }

    public override async Task Filter_nested_optional_dependent_with_some_required_compared_to_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_nested_optional_dependent_with_some_required_compared_to_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesSomeRequired] AS [e]
// WHERE JSON_QUERY([e].[Json], '$.ReqNav1') IS NULL
// """);
    }

    public override async Task Filter_nested_optional_dependent_with_some_required_compared_to_not_null(bool async)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => base.Filter_nested_optional_dependent_with_some_required_compared_to_not_null(async));

//         AssertSql(
// """
// SELECT [e].[Id], [e].[Name], [e].[Json]
// FROM [EntitiesSomeRequired] AS [e]
// WHERE JSON_QUERY([e].[Json], '$.ReqNav2') IS NOT NULL
// """);
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
