using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class UpdatesMySqlTest : UpdatesRelationalTestBase<UpdatesMySqlTest.UpdatesMySqlFixture>
    {
        public UpdatesMySqlTest(UpdatesMySqlFixture fixture)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        [Fact]
        public override void Identifiers_are_generated_correctly()
        {
            using (var context = CreateContext())
            {
                var entityType = context.Model.FindEntityType(typeof(
                    LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly));

                // MySQL's maximum identifier length is 64 characters, so EF Core truncates these identifiers to 63
                // characters followed by '~'. Identifiers that still collide after truncation lose one more character
                // and get a numeric uniquifier appended instead (e.g. '...~1').
                var expected = string.Join(
                    Environment.NewLine,
                    "LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIs~",
                    "PK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~",
                    "FK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~",
                    "IX_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTh~1");

                var actual = string.Join(
                    Environment.NewLine,
                    entityType.GetTableName(),
                    entityType.GetKeys().Single().GetName(),
                    entityType.GetForeignKeys().Single().GetConstraintName(),
                    entityType.GetIndexes().Single().GetDatabaseName());

                // Assert.True is used instead of Assert.Equal, because its failure message is not truncated. That way
                // all four generated identifiers are visible at once whenever EF Core changes how they are generated.
                Assert.True(
                    expected == actual,
                    $"Expected identifiers:{Environment.NewLine}{expected}{Environment.NewLine}{Environment.NewLine}Actual identifiers:{Environment.NewLine}{actual}");
            }
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Returning))]
        public override Task Save_with_shared_foreign_key()
        {
            return base.Save_with_shared_foreign_key();
        }

        public class UpdatesMySqlFixture : UpdatesRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // Necessary for test `Save_with_shared_foreign_key` to run correctly.
                if (AppConfig.ServerVersion.Supports.DefaultExpression ||
                    AppConfig.ServerVersion.Supports.AlternativeDefaultExpression)
                {
                    modelBuilder.Entity<ProductBase>()
                        .Property(p => p.Id).HasDefaultValueSql("(UUID())");
                }

                Models.Issue1300.Setup(modelBuilder, context);
            }

            public static class Models
            {
                public static class Issue1300
                {
                    public static void Setup(ModelBuilder modelBuilder, DbContext context)
                    {
                        modelBuilder.Entity<Flavor>(
                            entity =>
                            {
                                entity.HasKey(e => new {e.FlavorId, e.DiscoveryDate});
                                entity.Property(e => e.FlavorId)
                                    .ValueGeneratedOnAdd();
                                entity.Property(e => e.DiscoveryDate)
                                    .ValueGeneratedOnAdd();
                            });
                    }

                    public class Flavor
                    {
                        public int FlavorId { get; set; }
                        public DateTime DiscoveryDate { get; set; }
                    }
                }
            }
        }
    }
}
