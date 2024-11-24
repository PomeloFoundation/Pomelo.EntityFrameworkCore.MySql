using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    /// <summary>
    /// Based on the MigrationsInfrastructureTestBase implementation.
    /// </summary>
    public class FullInfrastructureMigrationsTest : IClassFixture<FullInfrastructureMigrationsTest.FullInfrastructureMigrationsFixture>
    {
        protected FullInfrastructureMigrationsFixture Fixture { get; }

        public FullInfrastructureMigrationsTest(FullInfrastructureMigrationsFixture fixture)
        {
            Fixture = fixture;
            Fixture.TestStore.CloseConnection();
        }

        protected virtual string Sql { get; set; }

        protected virtual void SetSql(string value)
            => Sql = value.Replace(ProductInfo.GetVersion(), "7.0.0-test");

        [ConditionalFact]
        public virtual void Can_create_stored_procedure_script_without_custom_delimiter_statements()
        {
            using var db = Fixture.CreateContext<FullInfrastructureMigrationsFixture.MigrationPrimaryKeyChangeContext>(
                new ServiceCollection()
                    .AddScoped<IMigrator, MySqlTestMigrator>());

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var migrator = (MySqlTestMigrator)db.GetService<IMigrator>();
            migrator.MigrationsSqlGenerationOptionsOverrider = options => options & ~MigrationsSqlGenerationOptions.Script;

            SetSql(
                migrator.GenerateScript(
                    options: MigrationsSqlGenerationOptions.Default,
                    fromMigration: Migration.InitialDatabase,
                    toMigration: "00000000000002_MigrationPrimaryKeyChange2"));

            // TODO: 9.0
            // Pomelo helper stored procedure statements should be inside the transaction scope.
            Assert.Equal(
"""
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
CREATE TABLE `Table1` (
    `Id` int NOT NULL,
    `AlternatePK` int NOT NULL,
    CONSTRAINT `PK_Table1_Id` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_MigrationPrimaryKeyChange1', '7.0.0-test');

DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END;
DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END;
CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Table1');
ALTER TABLE `Table1` DROP PRIMARY KEY;

ALTER TABLE `Table1` ADD CONSTRAINT `PK_Table1_AlternatePK` PRIMARY KEY (`AlternatePK`);
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'Table1', 'AlternatePK');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_MigrationPrimaryKeyChange2', '7.0.0-test');

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;
COMMIT;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void Can_generate_idempotent_up_scripts_with_primary_key_related_stored_procedures()
        {
            using var db = Fixture.CreateContext<FullInfrastructureMigrationsFixture.MigrationPrimaryKeyChangeContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var migrator = db.GetService<IMigrator>();

            SetSql(
                migrator.GenerateScript(
                    options: MigrationsSqlGenerationOptions.Idempotent,
                    fromMigration: Migration.InitialDatabase,
                    toMigration: "00000000000002_MigrationPrimaryKeyChange2"));

            ApplySqlScript(db);

            var history = db.GetService<IHistoryRepository>();
            Assert.Collection(
                history.GetAppliedMigrations(),
                h => Assert.Equal("00000000000001_MigrationPrimaryKeyChange1", h.MigrationId),
                h => Assert.Equal("00000000000002_MigrationPrimaryKeyChange2", h.MigrationId));

            Assert.Equal(
"""
DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_MigrationPrimaryKeyChange1') THEN

    CREATE TABLE `Table1` (
        `Id` int NOT NULL,
        `AlternatePK` int NOT NULL,
        CONSTRAINT `PK_Table1_Id` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_MigrationPrimaryKeyChange1') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000001_MigrationPrimaryKeyChange1', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_MigrationPrimaryKeyChange2') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Table1');
    ALTER TABLE `Table1` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_MigrationPrimaryKeyChange2') THEN

    ALTER TABLE `Table1` ADD CONSTRAINT `PK_Table1_AlternatePK` PRIMARY KEY (`AlternatePK`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'Table1', 'AlternatePK');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_MigrationPrimaryKeyChange2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000002_MigrationPrimaryKeyChange2', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void Drop_primary_key_with_recreating_foreign_keys()
        {
            using var db = Fixture.CreateContext<FullInfrastructureMigrationsFixture.MigrationDropPrimaryKeyWithRecreatingForeignKeysContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var migrator = db.GetService<IMigrator>();

            SetSql(
                migrator.GenerateScript(
                    fromMigration: Migration.InitialDatabase,
                    toMigration: "00000000000002_MigrationDropPrimaryKeyWithRecreatingForeignKeys2"));

            ApplySqlScript(db);

            var history = db.GetService<IHistoryRepository>();
            Assert.Collection(
                history.GetAppliedMigrations(),
                h => Assert.Equal("00000000000001_MigrationDropPrimaryKeyWithRecreatingForeignKeys1", h.MigrationId),
                h => Assert.Equal("00000000000002_MigrationDropPrimaryKeyWithRecreatingForeignKeys2", h.MigrationId));

            Assert.Equal(
"""
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
CREATE TABLE `Foo` (
    `FooId` int NOT NULL,
    CONSTRAINT `PK_Foo` PRIMARY KEY (`FooId`)
);

CREATE TABLE `Bar` (
    `BarId` int NOT NULL,
    CONSTRAINT `PK_Bar` PRIMARY KEY (`BarId`)
);

CREATE TABLE `FooBar` (
    `FooId` int NOT NULL,
    `BarId` int NOT NULL,
    CONSTRAINT `PK_FooBar` PRIMARY KEY (`FooId`, `BarId`),
    CONSTRAINT `FK_FooBar_Foo_FooId` FOREIGN KEY (`FooId`) REFERENCES `Foo` (`FooId`),
    CONSTRAINT `FK_FooBar_Bar_BarId` FOREIGN KEY (`BarId`) REFERENCES `Bar` (`BarId`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_MigrationDropPrimaryKeyWithRecreatingForeignKeys1', '7.0.0-test');

DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

ALTER TABLE `FooBar` ADD `ThirdColumn` int NOT NULL;

ALTER TABLE `FooBar` DROP FOREIGN KEY `FK_FooBar_Bar_BarId`;

ALTER TABLE `FooBar` DROP FOREIGN KEY `FK_FooBar_Foo_FooId`;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'FooBar');
ALTER TABLE `FooBar` DROP PRIMARY KEY;

ALTER TABLE `FooBar` ADD CONSTRAINT `FK_FooBar_Bar_BarId` FOREIGN KEY (`BarId`) REFERENCES `Bar` (`BarId`);

ALTER TABLE `FooBar` ADD CONSTRAINT `FK_FooBar_Foo_FooId` FOREIGN KEY (`FooId`) REFERENCES `Foo` (`FooId`);

ALTER TABLE `FooBar` ADD CONSTRAINT `PK_FooBar` PRIMARY KEY (`FooId`, `BarId`, `ThirdColumn`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_MigrationDropPrimaryKeyWithRecreatingForeignKeys2', '7.0.0-test');

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

COMMIT;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        protected virtual void ApplySqlScript(DbContext db)
        {
            // Noncapturing groups work as usual in Regex.Split() calls (while capturing groups get ignored and will be kept in the result).
            var sqlScriptParts = Regex.Split(Sql, @"[\r\n]*[^\S\r\n]*DELIMITER[^\S\r\n]+(?://|;)[^\S\r\n]*[\r\n]*", RegexOptions.IgnoreCase)
                .Where(s => s.Length > 0)
                .Select(
                    (s, n) => n % 2 == 1
                        ? Regex.Replace(
                            s,
                            @"\s*//\s*$",
                            @";",
                            RegexOptions.IgnoreCase | RegexOptions.Multiline)
                        : s)
                .ToList();

            foreach (var sqlScriptPart in sqlScriptParts)
            {
                db.Database.ExecuteSqlRaw(sqlScriptPart);
            }
        }

        /// <summary>
        /// Based on the MigrationsInfrastructureFixtureBase implementation.
        /// </summary>
        public class FullInfrastructureMigrationsFixture : SharedStoreFixtureBase<FullInfrastructureMigrationsFixture.MigrationPrimaryKeyChangeContext>
        {
            public static string ActiveProvider { get; set; }

            protected override ITestStoreFactory TestStoreFactory
                => MySqlConnectionStringTestStoreFactory.Instance;

            public new RelationalTestStore TestStore
                => (RelationalTestStore)base.TestStore;

            protected override string StoreName { get; } = "FullInfrastructureMigrationsTest";

            public TContext CreateContext<TContext>(IServiceCollection serviceCollection = null)
                where TContext : DbContext
                => (TContext)Activator.CreateInstance(
                    typeof(TContext),
                    TestStore.AddProviderOptions(
                            new DbContextOptionsBuilder())
                        .UseInternalServiceProvider(
                            TestStoreFactory.AddProviderServices(
                                    serviceCollection ?? new ServiceCollection())
                                .BuildServiceProvider())
                        .Options);

            public class EmptyMigrationsContext : DbContext
            {
                public EmptyMigrationsContext(DbContextOptions options)
                    : base(options)
                {
                }
            }

            #region MigrationPrimaryKeyChange

            public class MigrationPrimaryKeyChangeContext : DbContext
            {
                public MigrationPrimaryKeyChangeContext(DbContextOptions options)
                    : base(options)
                {
                }
            }

            [DbContext(typeof(MigrationPrimaryKeyChangeContext))]
            [Migration("00000000000001_MigrationPrimaryKeyChange1")]
            private class MigrationPrimaryKeyChange1 : Migration
            {
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                    MigrationsInfrastructureFixtureBase.ActiveProvider = migrationBuilder.ActiveProvider;

                    migrationBuilder
                        .CreateTable(
                            name: "Table1",
                            columns: x => new { Id = x.Column<int>(), AlternatePK = x.Column<int>() })
                        .PrimaryKey(
                            name: "PK_Table1_Id",
                            columns: x => x.Id);
                }

                protected override void Down(MigrationBuilder migrationBuilder)
                    => migrationBuilder.DropTable("Table1");
            }

            [DbContext(typeof(MigrationPrimaryKeyChangeContext))]
            [Migration("00000000000002_MigrationPrimaryKeyChange2")]
            private class MigrationPrimaryKeyChange2 : Migration
            {
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.DropPrimaryKey(
                        name: "PK_Table1_Id",
                        table: "Table1");

                    migrationBuilder.AddPrimaryKey(
                        name: "PK_Table1_AlternatePK",
                        table: "Table1",
                        column: "AlternatePK");
                }

                protected override void Down(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.DropPrimaryKey(
                        name: "PK_Table1_AlternatePK",
                        table: "Table1");

                    migrationBuilder.AddPrimaryKey(
                        name: "PK_Table1_Id",
                        table: "Table1",
                        column: "Id");
                }
            }

            #endregion MigrationPrimaryKeyChange

            #region MigrationDropPrimaryKeyWithRecreatingForeignKeys

            public class MigrationDropPrimaryKeyWithRecreatingForeignKeysContext : PoolableDbContext
            {
                public MigrationDropPrimaryKeyWithRecreatingForeignKeysContext(DbContextOptions options)
                    : base(options)
                {
                }
            }

            [DbContext(typeof(MigrationDropPrimaryKeyWithRecreatingForeignKeysContext))]
            [Migration("00000000000001_MigrationDropPrimaryKeyWithRecreatingForeignKeys1")]
            private class MigrationDropPrimaryKeyWithRecreatingForeignKeys1 : Migration
            {
                protected override void BuildTargetModel(ModelBuilder modelBuilder)
                {
                    modelBuilder
                        .HasAnnotation("Relational:MaxIdentifierLength", 64)
                        .HasAnnotation("ProductVersion", "7.0.0-dev");

                    modelBuilder.Entity("Foo", b =>
                    {
                        b.Property<int>("FooId");
                        b.HasKey("FooId");
                        b.ToTable("Foo");
                    });

                    modelBuilder.Entity("Bar", b =>
                    {
                        b.Property<int>("BarId");
                        b.HasKey("BarId");
                        b.ToTable("Bar");
                    });

                    modelBuilder.Entity("FooBar", b =>
                    {
                        b.Property<int>("FooId");
                        b.Property<int>("BarId");

                        b.HasKey("FooId", "BarId");

                        b.HasOne("Foo", "Foo")
                            .WithMany()
                            .HasForeignKey("FooId")
                            .HasConstraintName("FK_FooBar_Foo_FooId");
                        b.HasOne("Bar", "Bar")
                            .WithMany()
                            .HasForeignKey("BarId")
                            .HasConstraintName("FK_FooBar_Bar_BarId");
                    });
                }

                protected override void Up(MigrationBuilder migrationBuilder)
                {
                    MigrationsInfrastructureFixtureBase.ActiveProvider = migrationBuilder.ActiveProvider;

                    var fooTable = migrationBuilder.CreateTable(
                        name: "Foo",
                        columns: x => new { FooId = x.Column<int>() });

                    fooTable.PrimaryKey(
                        name: "PK_Foo",
                        columns: x => x.FooId);

                    var barTable = migrationBuilder.CreateTable(
                        name: "Bar",
                        columns: x => new { BarId = x.Column<int>() });

                    barTable.PrimaryKey(
                        name: "PK_Bar",
                        columns: x => x.BarId);

                    var fooBarTable = migrationBuilder.CreateTable(
                        name: "FooBar",
                        columns: x => new { FooId = x.Column<int>(), BarId = x.Column<int>() });

                    fooBarTable.PrimaryKey(
                        name: "PK_FooBar",
                        columns: x => new {x.FooId, x.BarId});

                    fooBarTable.ForeignKey(
                        name: "FK_FooBar_Foo_FooId",
                        columns: x => x.FooId,
                        principalTable: "Foo",
                        principalColumns: new[] { "FooId" });

                    fooBarTable.ForeignKey(
                        name: "FK_FooBar_Bar_BarId",
                        columns: x => x.BarId,
                        principalTable: "Bar",
                        principalColumns: new[] { "BarId" });
                }

                protected override void Down(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.DropTable("FooBar");
                    migrationBuilder.DropTable("Bar");
                    migrationBuilder.DropTable("Foo");
                }
            }

            [DbContext(typeof(MigrationDropPrimaryKeyWithRecreatingForeignKeysContext))]
            [Migration("00000000000002_MigrationDropPrimaryKeyWithRecreatingForeignKeys2")]
            private class MigrationDropPrimaryKeyWithRecreatingForeignKeys2 : Migration
            {
                protected override void BuildTargetModel(ModelBuilder modelBuilder)
                {
                    modelBuilder
                        .HasAnnotation("Relational:MaxIdentifierLength", 64)
                        .HasAnnotation("ProductVersion", "7.0.0-dev");

                    modelBuilder.Entity("Foo", b =>
                    {
                        b.Property<int>("FooId");
                        b.HasKey("FooId");
                        b.ToTable("Foo");
                    });

                    modelBuilder.Entity("Bar", b =>
                    {
                        b.Property<int>("BarId");
                        b.HasKey("BarId");
                        b.ToTable("Bar");
                    });

                    modelBuilder.Entity("FooBar", b =>
                    {
                        b.Property<int>("FooId");
                        b.Property<int>("BarId");
                        b.Property<int>("ThirdColumn");

                        b.HasKey("FooId", "BarId", "ThirdColumn");

                        b.HasOne("Foo", "Foo")
                            .WithMany()
                            .HasForeignKey("FooId")
                            .HasConstraintName("FK_FooBar_Foo_FooId");
                        b.HasOne("Bar", "Bar")
                            .WithMany()
                            .HasForeignKey("BarId")
                            .HasConstraintName("FK_FooBar_Bar_BarId");

                        b.ToTable("FooBar");
                    });
                }

                protected override void Up(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.AddColumn<int>(
                        name: "ThirdColumn",
                        table: "FooBar",
                        type: "int");

                    migrationBuilder.DropPrimaryKey(
                        name: "PK_FooBar",
                        table: "FooBar",
                        recreateForeignKeys: true);

                    migrationBuilder.AddPrimaryKey(
                        name: "PK_FooBar",
                        table: "FooBar",
                        columns: new[] { "FooId", "BarId", "ThirdColumn" });
                }

                protected override void Down(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.DropPrimaryKey(
                        name: "PK_FooBar",
                        table: "FooBar",
                        recreateForeignKeys: true);

                    migrationBuilder.AddPrimaryKey(
                        name: "PK_FooBar",
                        table: "FooBar",
                        columns: new[] { "FooId", "BarId" });

                    migrationBuilder.DropColumn(
                        name: "ThirdColumn",
                        table: "FooBar");
                }
            }

            #endregion MigrationDropPrimaryKeyWithRecreatingForeignKeys
        }
    }
}
