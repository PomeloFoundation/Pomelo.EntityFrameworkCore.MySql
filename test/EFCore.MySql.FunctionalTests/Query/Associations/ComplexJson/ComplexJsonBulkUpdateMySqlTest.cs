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
public class ComplexJsonBulkUpdateMySqlTest : ComplexJsonBulkUpdateRelationalTestBase<ComplexJsonBulkUpdateMySqlTest.ComplexJsonBulkUpdateMySqlFixture>
{
    public ComplexJsonBulkUpdateMySqlTest(ComplexJsonBulkUpdateMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove these skips once JSON bulk update operations are fully supported for MySQL.
    // The MySQL provider currently does not support partial updates with ExecuteUpdate within JSON columns.

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_property_inside_associate()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_property_inside_nested_associate()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_multiple_projected_associates_via_anonymous_type()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_primitive_collection_to_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_nested_collection_to_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_nested_collection_to_inline_with_lambda()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_nested_associate_to_parameter()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_multiple_properties_inside_associates_and_on_entity_type()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_primitive_collection_to_another_collection()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_property_on_projected_associate_with_OrderBy_Skip()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_nested_associate_to_another_nested_associate()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_property_inside_associate_with_special_chars()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_nested_collection_to_another_nested_collection()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_multiple_properties_inside_same_associate()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_property_on_projected_associate()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_nested_associate_to_inline_with_lambda()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Bulk update operations on JSON collections not yet supported")]
    public override Task Update_primitive_collection_to_constant()
    {
        return Task.CompletedTask;
    }

    public class ComplexJsonBulkUpdateMySqlFixture : ComplexJsonRelationalFixtureBase
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
