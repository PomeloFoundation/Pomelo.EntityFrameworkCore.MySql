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
public class ComplexJsonCollectionMySqlTest : ComplexJsonCollectionRelationalTestBase<ComplexJsonCollectionMySqlTest.ComplexJsonCollectionMySqlFixture>
{
    public ComplexJsonCollectionMySqlTest(ComplexJsonCollectionMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove these skips once TransformJsonQueryToTable is fully implemented for MySQL.
    // The MySQL provider's TransformJsonQueryToTable method throws InvalidOperationException because
    // EF Core's SelectExpression.AddCrossJoin does not properly generate the CROSS JOIN keyword when
    // combining table expressions with JSON_TABLE functions, resulting in invalid SQL syntax.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable (line 248).

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Count()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Where()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task OrderBy_ElementAt()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Distinct()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Distinct_over_projected_nested_collection()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task GroupBy()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Select_within_Select_within_Select_with_aggregates()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Index_constant()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Index_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Index_column()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Index_out_of_bounds()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Distinct_over_projected_filtered_nested_collection()
    {
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Distinct_projected(Microsoft.EntityFrameworkCore.QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    public class ComplexJsonCollectionMySqlFixture : ComplexJsonRelationalFixtureBase
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
