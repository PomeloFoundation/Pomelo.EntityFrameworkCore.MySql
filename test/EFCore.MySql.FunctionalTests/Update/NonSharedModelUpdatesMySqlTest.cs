using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Tests;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Update;

public class NonSharedModelUpdatesMySqlTest : NonSharedModelUpdatesTestBase
{
    public NonSharedModelUpdatesMySqlTest([NotNull] NonSharedFixture fixture)
        : base(fixture)
    {
    }

    public override async Task Principal_and_dependent_roundtrips_with_cycle_breaking(bool async)
    {
        await base.Principal_and_dependent_roundtrips_with_cycle_breaking(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
@p0='AC South' (Size = 4000)

SET AUTOCOMMIT = 1;
INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0)
RETURNING `Id`;
""",
                //
                """
@p1='1'
@p2='Alice' (Size = 4000)

SET AUTOCOMMIT = 1;
INSERT INTO `Author` (`AuthorsClubId`, `Name`)
VALUES (@p1, @p2)
RETURNING `Id`;
""",
                //
                """
@p3='1'
@p4=NULL (Size = 4000)

SET AUTOCOMMIT = 1;
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

SET AUTOCOMMIT = 1;
INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0)
RETURNING `Id`;
""",
                //
                """
@p1='2'
@p2='Author of the year 2023' (Size = 4000)

SET AUTOCOMMIT = 1;
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
WHERE `Id` = @p5
RETURNING 1;
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

    // Regression test for https://github.com/microting/Pomelo.EntityFrameworkCore.MySql/issues/353
    // A database-generated value (here a timestamp) that is part of a composite primary key must be read
    // back after INSERT. The only mechanism that can do that on MySQL/MariaDB is INSERT ... RETURNING;
    // the LAST_INSERT_ID() fallback only works for AUTO_INCREMENT integer keys, so it matched 0 rows and
    // threw DbUpdateConcurrencyException. RETURNING requires MariaDB 10.5+ (MySQL has no RETURNING at all).
    [ConditionalFact]
    [SupportedServerVersionCondition("Returning")]
    public async Task Insert_reads_back_database_generated_key_in_composite_key()
    {
        var contextFactory = await InitializeAsync<DownloadContext>();
        await using var context = contextFactory.CreateContext();

        var download = new Download { UserId = 1, Version = 1 };
        context.Add(download);

        // Scope the SQL assertions below to the INSERT only (exclude schema-creation SQL).
        TestSqlLoggerFactory.Clear();

        // Must not throw DbUpdateConcurrencyException (the symptom of issue #353).
        await context.SaveChangesAsync();

        // The generated timestamp must have been propagated back into the tracked entity.
        Assert.NotEqual(default(DateTime), download.DateTime);

        // It must have been read back via RETURNING, not the LAST_INSERT_ID() fallback.
        Assert.Contains("RETURNING", TestSqlLoggerFactory.Sql);
        Assert.DoesNotContain("LAST_INSERT_ID", TestSqlLoggerFactory.Sql);

        // And it must round-trip from the database.
        context.ChangeTracker.Clear();
        var reloaded = await context.Set<Download>().SingleAsync();
        Assert.Equal(download.UserId, reloaded.UserId);
        Assert.Equal(download.Version, reloaded.Version);
        Assert.Equal(download.DateTime, reloaded.DateTime);
    }

    private class DownloadContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Download>(
                b =>
                {
                    b.HasKey(e => new { e.UserId, e.Version, e.DateTime });
                    b.Property(e => e.DateTime)
                        .HasColumnType("timestamp(6)")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                        .ValueGeneratedOnAdd();
                });
    }

    private class Download
    {
        public int UserId { get; set; }
        public int Version { get; set; }
        public DateTime DateTime { get; set; }
    }

    private void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
