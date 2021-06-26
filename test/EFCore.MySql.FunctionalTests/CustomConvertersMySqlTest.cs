using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class CustomConvertersMySqlTest : CustomConvertersTestBase<CustomConvertersMySqlTest.CustomConvertersMySqlFixture>
    {
        public CustomConvertersMySqlTest(CustomConvertersMySqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Value_conversion_on_enum_collection_contains()
        {
            Assert.Contains(
                CoreStrings.TranslationFailed("").Substring(47),
                Assert.Throws<InvalidOperationException>(() => base.Value_conversion_on_enum_collection_contains()).Message);
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

                var ciCollation = ((MySqlTestStore)TestStore).GetCaseInsensitiveUtf8Mb4Collation();

                // Needed to make Can_insert_and_read_back_with_case_insensitive_string_key() work.
                modelBuilder.Entity<StringForeignKeyDataType>()
                    .Property(e => e.StringKeyDataTypeId)
                    .UseCollation(ciCollation);
                modelBuilder.Entity<StringKeyDataType>()
                    .Property(e => e.Id)
                    .UseCollation(ciCollation);
            }
        }
    }
}
