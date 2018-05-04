using System;
using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class CustomConvertersMySqlTest : CustomConvertersTestBase<CustomConvertersMySqlTest.CustomConvertersMySqlFixture>
    {
        public CustomConvertersMySqlTest(CustomConvertersMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class CustomConvertersMySqlFixture : CustomConvertersFixtureBase
        {
            public override bool StrictEquality => true;

            public override bool SupportsAnsi => true;

            public override bool SupportsUnicodeToAnsiConversion => true;

            public override bool SupportsLargeStringComparisons => true;

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override bool SupportsBinaryKeys => true;

            public override DateTime DefaultDateTime => new DateTime();

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base
                    .AddOptions(builder)
                    .ConfigureWarnings(
                        c => c.Log(RelationalEventId.QueryClientEvaluationWarning));
        }
    }
}
