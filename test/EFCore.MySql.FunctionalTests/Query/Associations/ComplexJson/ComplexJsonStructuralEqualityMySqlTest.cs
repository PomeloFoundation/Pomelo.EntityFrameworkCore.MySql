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
//
// Skip on MariaDB: MariaDB stores JSON as LONGTEXT and doesn't support CAST(... AS json). 
// When comparing JSON to NULL, we need to distinguish between database NULL vs JSON null value ("null"), 
// which requires CAST AS json on MySQL but isn't supported on MariaDB. This causes incorrect result counts 
// (returns 7 instead of 1) because the NULL comparison semantics differ without the CAST.
//
// Until proper JSON-null-vs-database-NULL handling is implemented for MariaDB's LONGTEXT-based JSON storage,
// all structural equality tests in this class will fail on MariaDB with JsonDataTypeEmulation enabled.
//
// See: https://github.com/microting/Pomelo.EntityFrameworkCore.MySql/issues/151
[SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.JsonDataTypeEmulation))]
public class ComplexJsonStructuralEqualityMySqlTest : ComplexJsonStructuralEqualityRelationalTestBase<ComplexJsonStructuralEqualityMySqlTest.ComplexJsonStructuralEqualityMySqlFixture>
{
    public ComplexJsonStructuralEqualityMySqlTest(ComplexJsonStructuralEqualityMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove this skip once MariaDB JSON NULL comparison semantics are properly handled.
    // See issue #151 and class-level TODO comment for details.
    //
    // Note: This test is inherited from the base class but fails on MariaDB due to JSON NULL comparison
    // semantic differences. The test should be skipped using the "SkipForMariaDb" trait at the class level.
    // Skipping individual inherited tests requires conditional skip logic in the test infrastructure.

    // TODO: Remove these skips once TransformJsonQueryToTable is fully implemented for MySQL.
    // The MySQL provider's TransformJsonQueryToTable method throws InvalidOperationException because
    // EF Core's SelectExpression.AddCrossJoin does not properly generate the CROSS JOIN keyword when
    // combining table expressions with JSON_TABLE functions, resulting in invalid SQL syntax.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable (line 248).

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Contains_with_inline()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Contains_with_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Contains_with_operators_composed_on_the_collection()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Contains_with_nested_and_composed_operators()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Nested_collection_with_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Nested_associate_with_inline()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Two_nested_associates()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Nested_associate_with_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Nested_associate_with_inline_null()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Nested_collection_with_inline()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Two_nested_collections()
    {
        return Task.CompletedTask;
    }

    public class ComplexJsonStructuralEqualityMySqlFixture : ComplexJsonRelationalFixtureBase
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
