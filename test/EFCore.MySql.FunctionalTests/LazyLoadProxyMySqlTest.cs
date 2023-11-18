using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class LazyLoadProxyMySqlTest : LazyLoadProxyTestBase<LazyLoadProxyMySqlTest.LoadMySqlFixture>
    {
        public LazyLoadProxyMySqlTest(LoadMySqlFixture fixture)
            : base(fixture)
        {
            ClearLog();
        }

        public override void Top_level_projection_track_entities_before_passing_to_client_method()
        {
            base.Top_level_projection_track_entities_before_passing_to_client_method();

            Assert.Equal(
"""
SELECT `p`.`Id`, `p`.`AlternateId`, `p`.`Discriminator`, `p`.`Culture_Rating`, `p`.`Culture_Species`, `p`.`Culture_Subspecies`, `p`.`Culture_Validation`, `p`.`Culture_License_Charge`, `p`.`Culture_License_Title`, `p`.`Culture_License_Tag_Text`, `p`.`Culture_License_Tog_Text`, `p`.`Culture_Manufacturer_Name`, `p`.`Culture_Manufacturer_Rating`, `p`.`Culture_Manufacturer_Tag_Text`, `p`.`Culture_Manufacturer_Tog_Text`, `p`.`Milk_Rating`, `p`.`Milk_Species`, `p`.`Milk_Subspecies`, `p`.`Milk_Validation`, `p`.`Milk_License_Charge`, `p`.`Milk_License_Title`, `p`.`Milk_License_Tag_Text`, `p`.`Milk_License_Tog_Text`, `p`.`Milk_Manufacturer_Name`, `p`.`Milk_Manufacturer_Rating`, `p`.`Milk_Manufacturer_Tag_Text`, `p`.`Milk_Manufacturer_Tog_Text`
FROM `Parent` AS `p`
ORDER BY `p`.`Id`
LIMIT 1

@__p_0='707' (Nullable = true)

SELECT `s`.`Id`, `s`.`ParentId`, `s`.`Culture_Rating`, `s`.`Culture_Species`, `s`.`Culture_Subspecies`, `s`.`Culture_Validation`, `s`.`Culture_License_Charge`, `s`.`Culture_License_Title`, `s`.`Culture_License_Tag_Text`, `s`.`Culture_License_Tog_Text`, `s`.`Culture_Manufacturer_Name`, `s`.`Culture_Manufacturer_Rating`, `s`.`Culture_Manufacturer_Tag_Text`, `s`.`Culture_Manufacturer_Tog_Text`, `s`.`Milk_Rating`, `s`.`Milk_Species`, `s`.`Milk_Subspecies`, `s`.`Milk_Validation`, `s`.`Milk_License_Charge`, `s`.`Milk_License_Title`, `s`.`Milk_License_Tag_Text`, `s`.`Milk_License_Tog_Text`, `s`.`Milk_Manufacturer_Name`, `s`.`Milk_Manufacturer_Rating`, `s`.`Milk_Manufacturer_Tag_Text`, `s`.`Milk_Manufacturer_Tog_Text`
FROM `Single` AS `s`
WHERE `s`.`ParentId` = @__p_0
LIMIT 1
""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        protected override void RecordLog() =>
            Sql = Fixture.TestSqlLoggerFactory.Sql;

        private string Sql { get; set; }

        #region Expected JSON override

        // TODO: Tiny discrepancy in decimal representation (Charge: 1.0000000000000000000000000000 instead of 1.00)
        protected override string SerializedBlogs1
            => base.SerializedBlogs1.Replace("1.00", "1.0000000000000000000000000000");

        protected override string SerializedBlogs2
            => base.SerializedBlogs2.Replace("1.00", "1.0000000000000000000000000000");

        #endregion Expected JSON override

        public class LoadMySqlFixture : LoadFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
