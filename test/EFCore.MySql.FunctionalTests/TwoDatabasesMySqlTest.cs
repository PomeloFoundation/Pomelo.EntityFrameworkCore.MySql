using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class TwoDatabasesMySqlTest : TwoDatabasesTestBase, IClassFixture<MySqlFixture>
    {
        public TwoDatabasesMySqlTest(MySqlFixture fixture)
            : base(fixture)
        {
        }

        protected new MySqlFixture Fixture
            => (MySqlFixture)base.Fixture;

        protected override DbContextOptionsBuilder CreateTestOptions(
            DbContextOptionsBuilder optionsBuilder, bool withConnectionString = false)
            => withConnectionString
                ? optionsBuilder.UseMySql(DummyConnectionString, AppConfig.ServerVersion)
                : optionsBuilder.UseMySql(AppConfig.ServerVersion);

        protected override TwoDatabasesWithDataContext CreateBackingContext(string databaseName)
            => new TwoDatabasesWithDataContext(Fixture.CreateOptions(MySqlTestStore.Create(databaseName)));

        protected override string DummyConnectionString { get; } = "Server=localhost;Database=DoesNotExist;AllowUserVariables=True;Use Affected Rows=False";
    }
}
