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
            RecordLog();

            Assert.Equal(
                @"SELECT `p`.`Id`, `p`.`AlternateId`, `p`.`Discriminator`
FROM `Parent` AS `p`
ORDER BY `p`.`Id`
LIMIT 1

@__p_0='707' (Nullable = true)

SELECT `s`.`Id`, `s`.`ParentId`
FROM `Single` AS `s`
WHERE `s`.`ParentId` = @__p_0",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        protected override void RecordLog() =>
            Sql = Fixture.TestSqlLoggerFactory.Sql;

        private string Sql { get; set; }

        public class LoadMySqlFixture : LoadFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
