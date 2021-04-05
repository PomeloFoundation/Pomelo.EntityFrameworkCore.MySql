using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class CustomConvertersMySqlTest : CustomConvertersTestBase<CustomConvertersMySqlTest.CustomConvertersMySqlFixture>
    {
        public CustomConvertersMySqlTest(CustomConvertersMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class CustomConvertersMySqlFixture : CustomConvertersFixtureBase
        {
            public override bool StrictEquality => false;

            public override bool SupportsAnsi => true;

            public override bool SupportsUnicodeToAnsiConversion => false;

            public override bool SupportsLargeStringComparisons => true;

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override bool SupportsBinaryKeys => true;

            public override DateTime DefaultDateTime => new DateTime();

            public override bool SupportsDecimalComparisons => false;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // Needed to make Can_insert_and_read_back_with_case_insensitive_string_key() work.
                modelBuilder.Entity<StringForeignKeyDataType>()
                    .Property(e => e.StringKeyDataTypeId)
                    .HasCollation("utf8_general_ci");
                modelBuilder.Entity<StringKeyDataType>()
                    .Property(e => e.Id)
                    .HasCollation("utf8_general_ci");
            }
        }
    }
}
