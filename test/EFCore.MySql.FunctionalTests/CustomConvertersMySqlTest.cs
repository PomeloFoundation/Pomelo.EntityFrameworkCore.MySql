using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
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

        [ConditionalFact(Skip = "EF Core Issue#11929")]
        public override void Can_query_using_any_data_type_nullable_shadow()
        {
            base.Can_query_using_any_data_type_nullable_shadow();
        }

        [ConditionalFact(Skip = "EF Core Issue#11929")]
        public override void Can_perform_query_with_ansi_strings_test()
        {
            base.Can_perform_query_with_ansi_strings_test();
        }

        [ConditionalFact(Skip = "EF Core Issue#18147")]
        public override void Value_conversion_is_appropriately_used_for_join_condition()
        {
            base.Value_conversion_is_appropriately_used_for_join_condition();
        }

        [ConditionalFact(Skip = "EF Core Issue#18147")]
        public override void Value_conversion_is_appropriately_used_for_left_join_condition()
        {
            base.Value_conversion_is_appropriately_used_for_left_join_condition();
        }

        [ConditionalFact(Skip = "EF Core Issue#18147")]
        public override void Where_bool_gets_converted_to_equality_when_value_conversion_is_used()
        {
            base.Where_bool_gets_converted_to_equality_when_value_conversion_is_used();
        }
        
        public class CustomConvertersMySqlFixture : CustomConvertersFixtureBase
        {
            public override bool StrictEquality => true;

            public override bool SupportsAnsi => true;

            public override bool SupportsUnicodeToAnsiConversion => false;

            public override bool SupportsLargeStringComparisons => true;

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override bool SupportsBinaryKeys => true;

            public override DateTime DefaultDateTime => new DateTime();

            public override bool SupportsDecimalComparisons => true;
        }
    }
}
