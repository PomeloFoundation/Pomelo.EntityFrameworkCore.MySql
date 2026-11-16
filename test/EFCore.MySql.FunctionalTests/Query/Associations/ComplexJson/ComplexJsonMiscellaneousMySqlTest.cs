using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
// Skip on MariaDB due to JsonDataTypeEmulation limitations
[SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.JsonDataTypeEmulation))]
public class ComplexJsonMiscellaneousMySqlTest : ComplexJsonMiscellaneousRelationalTestBase<ComplexJsonMiscellaneousMySqlTest.ComplexJsonMiscellaneousMySqlFixture>
{
    public ComplexJsonMiscellaneousMySqlTest(ComplexJsonMiscellaneousMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: 11.0 - Re-enable once the NullReferenceException in EF Core has been resolved.
    // RelationalQueryableMethodTranslatingExpressionVisitor.GenerateComplexJsonShaper(IComplexProperty, ITableBase, string, bool)
    // does `table.FindColumn(containerColumnName).Name` without a null check. When the query root is a FromSql
    // expression, the lookup returns null and the shaper generation throws before the provider is ever consulted.
    [Fact(Skip = "EF Core 11 throws a NullReferenceException in GenerateComplexJsonShaper when the query root is a FromSql expression.")]
    public override Task FromSql_on_root()
        => Task.CompletedTask;

    public class ComplexJsonMiscellaneousMySqlFixture : ComplexJsonRelationalFixtureBase
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
