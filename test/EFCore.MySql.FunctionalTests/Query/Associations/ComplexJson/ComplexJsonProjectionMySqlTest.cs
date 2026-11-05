using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
// Skip on MariaDB due to JsonDataTypeEmulation limitations
[SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.JsonDataTypeEmulation))]
public class ComplexJsonProjectionMySqlTest : ComplexJsonProjectionRelationalTestBase<ComplexJsonProjectionMySqlTest.ComplexJsonProjectionMySqlFixture>
{
    public ComplexJsonProjectionMySqlTest(ComplexJsonProjectionMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove these skips once TransformJsonQueryToTable is fully implemented for MySQL.
    // Currently, SelectMany over JSON collections of structural types is not supported.
    // The MySQL provider's TransformJsonQueryToTable method throws InvalidOperationException because
    // EF Core's SelectExpression.AddCrossJoin does not properly generate the CROSS JOIN keyword when
    // combining table expressions with JSON_TABLE functions, resulting in invalid SQL syntax.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable (line 248).

    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task SelectMany_associate_collection(QueryTrackingBehavior queryTrackingBehavior)
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task SelectMany_nested_collection_on_required_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task SelectMany_nested_collection_on_optional_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "JSON operations over structural types not yet fully supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Select_nested_collection_on_optional_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "JSON operations over structural types not yet fully supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Select_required_nested_on_optional_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "JSON operations over structural types not yet fully supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Select_nested_collection_on_required_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "JSON operations over structural types not yet fully supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Select_optional_nested_on_optional_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "JSON operations over structural types not yet fully supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Select_optional_nested_on_required_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "JSON operations over structural types not yet fully supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Select_required_nested_on_required_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    public class ComplexJsonProjectionMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new MySqlDbContextOptionsBuilder(optionsBuilder).EnablePrimitiveCollectionsSupport();
            return optionsBuilder;
        }
    }
}
