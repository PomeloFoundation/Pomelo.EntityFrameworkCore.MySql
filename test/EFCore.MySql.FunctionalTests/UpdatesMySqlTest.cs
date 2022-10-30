using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class UpdatesMySqlTest : UpdatesRelationalTestBase<UpdatesMySqlTest.UpdatesMySqlFixture>
    {
        public UpdatesMySqlTest(UpdatesMySqlFixture fixture)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        [ConditionalFact]
        public override void Identifiers_are_generated_correctly()
        {
            using (var context = CreateContext())
            {
                var entityType = context.Model.FindEntityType(typeof(
                    LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly));
                Assert.Equal("LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIs~", entityType.GetTableName());
                Assert.Equal("PK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~", entityType.GetKeys().Single().GetName());
                Assert.Equal("FK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~", entityType.GetForeignKeys().Single().GetConstraintName());
                Assert.Equal("IX_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~", entityType.GetIndexes().Single().GetDatabaseName());
            }
        }

        public class UpdatesMySqlFixture : UpdatesRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

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
