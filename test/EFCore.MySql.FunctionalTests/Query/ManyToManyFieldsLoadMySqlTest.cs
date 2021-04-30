using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.ManyToManyFieldsModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ManyToManyFieldsLoadMySqlTest : ManyToManyFieldsLoadTestBase<
        ManyToManyFieldsLoadMySqlTest.ManyToManyFieldsLoadMySqlFixture>
    {
        public ManyToManyFieldsLoadMySqlTest(ManyToManyFieldsLoadMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class ManyToManyFieldsLoadMySqlFixture : ManyToManyFieldsLoadFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder
                    .Entity<JoinOneSelfPayload>()
                    .Property(e => e.Payload)
                    .ValueGeneratedOnAdd();

                modelBuilder
                    .SharedTypeEntity<Dictionary<string, object>>("JoinOneToThreePayloadFullShared")
                    .IndexerProperty<string>("Payload")
                    .HasDefaultValue("Generated");

                modelBuilder
                    .Entity<JoinOneToThreePayloadFull>()
                    .Property(e => e.Payload)
                    .HasDefaultValue("Generated");
            }
        }
    }
}
