using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class ComplexTypesTrackingMySqlTest : ComplexTypesTrackingTestBase<ComplexTypesTrackingMySqlTest.MySqlFixture>
{
    public ComplexTypesTrackingMySqlTest(MySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public class MySqlFixture : FixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder);

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // Configure complex collections as JSON columns (EF Core 10 .ToJson() support).
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                foreach (var complexProperty in entityType.GetComplexProperties().ToList())
                {
                    if (complexProperty.IsCollection)
                    {
                        // Set ContainerColumnName on the ComplexType (this is what .ToJson() does internally).
                        // Do NOT set JsonPropertyName — that conflicts with ContainerColumnName.
                        ((IMutableTypeBase)complexProperty.ComplexType)
                            .SetContainerColumnName(complexProperty.Name);
                    }
                }
            }
        }
    }
}
