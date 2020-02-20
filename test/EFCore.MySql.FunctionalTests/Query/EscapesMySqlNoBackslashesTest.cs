using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class EscapesMySqlNoBackslashesTest : EscapesMySqlTestBase<EscapesMySqlNoBackslashesTest.EscapesMySqlNoBackslashesFixture>
    {
        public EscapesMySqlNoBackslashesTest(EscapesMySqlNoBackslashesFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact]
        public override void Input_query_escapes_parameter()
        {
            base.Input_query_escapes_parameter();

            AssertSql(
                @"@p0='Back\slash's Garden Party' (Nullable = false) (Size = 4000)

INSERT INTO `Artists` (`Name`)
VALUES (@p0);
SELECT `ArtistId`
FROM `Artists`
WHERE ROW_COUNT() = 1 AND `ArtistId` = LAST_INSERT_ID();",
                //
                @"SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE RIGHT(`a`.`Name`, CHAR_LENGTH(CONVERT(' Garden Party' USING utf8mb4) COLLATE utf8mb4_bin)) = CONVERT(' Garden Party' USING utf8mb4) COLLATE utf8mb4_bin");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_query_escapes_literal(bool isAsync)
        {
            await base.Where_query_escapes_literal(isAsync);

            AssertSql(
                @"SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` = 'Back\slasher''s'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_query_escapes_parameter(bool isAsync)
        {
            await base.Where_query_escapes_parameter(isAsync);

            AssertSql(
                @"@__artistName_0='Back\slasher's' (Size = 4000)

SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` = @__artistName_0");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Where_contains_query_escapes(bool isAsync)
        {
            await base.Where_contains_query_escapes(isAsync);

            AssertSql(
                @"SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` IN ('Back\slasher''s', 'John''s Chill Box')");
        }

        public class EscapesMySqlNoBackslashesFixture : EscapesMySqlFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.NoBackslashEscapesInstance;
        }
    }
}
