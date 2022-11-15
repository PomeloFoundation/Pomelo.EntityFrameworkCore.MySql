using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Update;

public class NonSharedModelUpdatesMySqlTest : NonSharedModelUpdatesTestBase
{
    public override async Task Principal_and_dependent_roundtrips_with_cycle_breaking(bool async)
    {
        await base.Principal_and_dependent_roundtrips_with_cycle_breaking(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
@p0='AC South' (Size = 4000)

INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0)
RETURNING `Id`;
""",
                //
                """
@p1='1'
@p2='Alice' (Size = 4000)

INSERT INTO `Author` (`AuthorsClubId`, `Name`)
VALUES (@p1, @p2)
RETURNING `Id`;
""",
                //
                """
@p3='1'
@p4=NULL (Size = 4000)

INSERT INTO `Book` (`AuthorId`, `Title`)
VALUES (@p3, @p4)
RETURNING `Id`;
""",
                //
                """
SELECT `b`.`Id`, `b`.`AuthorId`, `b`.`Title`, `a`.`Id`, `a`.`AuthorsClubId`, `a`.`Name`
FROM `Book` AS `b`
INNER JOIN `Author` AS `a` ON `b`.`AuthorId` = `a`.`Id`
LIMIT 2
""",
                //
                """
@p0='AC North' (Size = 4000)

INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0)
RETURNING `Id`;
""",
                //
                """
@p1='2'
@p2='Author of the year 2023' (Size = 4000)

INSERT INTO `Author` (`AuthorsClubId`, `Name`)
VALUES (@p1, @p2)
RETURNING `Id`;
""",
                //
                """
@p4='1'
@p3='2'
@p5='1'

UPDATE `Book` SET `AuthorId` = @p3
WHERE `Id` = @p4;
SELECT ROW_COUNT();

DELETE FROM `Author`
WHERE `Id` = @p5;
SELECT ROW_COUNT();
""");
        }
        else
        {
            AssertSql(
                """
@p0='AC South' (Size = 4000)

INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0);
SELECT `Id`
FROM `AuthorsClub`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""",
                //
                """
@p1='1'
@p2='Alice' (Size = 4000)

INSERT INTO `Author` (`AuthorsClubId`, `Name`)
VALUES (@p1, @p2);
SELECT `Id`
FROM `Author`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""",
                //
                """
@p3='1'
@p4=NULL (Size = 4000)

INSERT INTO `Book` (`AuthorId`, `Title`)
VALUES (@p3, @p4);
SELECT `Id`
FROM `Book`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""",
                //
                """
SELECT `b`.`Id`, `b`.`AuthorId`, `b`.`Title`, `a`.`Id`, `a`.`AuthorsClubId`, `a`.`Name`
FROM `Book` AS `b`
INNER JOIN `Author` AS `a` ON `b`.`AuthorId` = `a`.`Id`
LIMIT 2
""",
                //
                """
@p0='AC North' (Size = 4000)

INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0);
SELECT `Id`
FROM `AuthorsClub`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""",
                //
                """
@p1='2'
@p2='Author of the year 2023' (Size = 4000)

INSERT INTO `Author` (`AuthorsClubId`, `Name`)
VALUES (@p1, @p2);
SELECT `Id`
FROM `Author`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""",
                //
                """
@p4='1'
@p3='2'
@p5='1'

UPDATE `Book` SET `AuthorId` = @p3
WHERE `Id` = @p4;
SELECT ROW_COUNT();

DELETE FROM `Author`
WHERE `Id` = @p5;
SELECT ROW_COUNT();
""");
        }
    }

    private void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
