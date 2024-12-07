using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

#nullable enable

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.RenameColumn))]
    public class MigrationsInfrastructureMySqlTest : MigrationsInfrastructureTestBase<MigrationsInfrastructureMySqlTest.MigrationsInfrastructureMySqlFixture>
    {
        public MigrationsInfrastructureMySqlTest(MigrationsInfrastructureMySqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Can_generate_migration_from_initial_database_to_initial()
        {
            base.Can_generate_migration_from_initial_database_to_initial();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_no_migration_script()
        {
            base.Can_generate_no_migration_script();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_apply_one_migration()
        {
            base.Can_apply_one_migration();

            Assert.Null(Sql);
        }

        public override void Can_apply_one_migration_in_parallel()
        {
            base.Can_apply_one_migration_in_parallel();

            Assert.Null(Sql);
        }

        public override void Can_apply_second_migration_in_parallel()
        {
            base.Can_apply_second_migration_in_parallel();

            Assert.Null(Sql);
        }

        public override async Task Can_apply_one_migration_in_parallel_async()
        {
            await base.Can_apply_one_migration_in_parallel_async();

            Assert.Null(Sql);
        }

        public override async Task Can_apply_second_migration_in_parallel_async()
        {
            await base.Can_apply_second_migration_in_parallel_async();

            Assert.Null(Sql);
        }

        public override async Task Can_generate_up_and_down_scripts()
        {
            await base.Can_generate_up_and_down_scripts();

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
    `Foo` int NOT NULL,
    `Description` longtext NOT NULL,
    CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_Migration1', '7.0.0-test');

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000003_Migration3', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000004_Migration4', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000005_Migration5', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000006_Migration6', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000007_Migration7', '7.0.0-test');

COMMIT;

START TRANSACTION;
DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000007_Migration7';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000006_Migration6';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000005_Migration5';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000004_Migration4';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000003_Migration3';

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

DROP TABLE `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000001_Migration1';

COMMIT;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_up_and_down_scripts_noTransactions()
        {
            await base.Can_generate_up_and_down_scripts_noTransactions();

            Assert.Equal(
"""
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Table1` (
    `Id` int NOT NULL,
    `Foo` int NOT NULL,
    `Description` longtext NOT NULL,
    CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_Migration1', '7.0.0-test');

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000003_Migration3', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000004_Migration4', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000005_Migration5', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000006_Migration6', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000007_Migration7', '7.0.0-test');

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000007_Migration7';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000006_Migration6';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000005_Migration5';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000004_Migration4';

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000003_Migration3';

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

DROP TABLE `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000001_Migration1';


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_one_up_and_down_script()
        {
            await base.Can_generate_one_up_and_down_script();

            Assert.Equal(
"""
START TRANSACTION;
ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT;

START TRANSACTION;
ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_up_and_down_script_using_names()
        {
            await base.Can_generate_up_and_down_script_using_names();

            Assert.Equal(
"""
START TRANSACTION;
ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT;

START TRANSACTION;
ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_idempotent_up_and_down_scripts()
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Can_generate_idempotent_up_and_down_scripts());

            Assert.Equal("'DELIMITER' should not be used with MySqlConnector. See https://mysqlconnector.net/delimiter", exception.Message);
            Assert.Equal(
"""
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    CREATE TABLE `Table1` (
        `Id` int NOT NULL,
        `Foo` int NOT NULL,
        `Description` longtext NOT NULL,
        CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000001_Migration1', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000002_Migration2', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_idempotent_up_and_down_scripts_noTransactions()
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Can_generate_idempotent_up_and_down_scripts_noTransactions());

            Assert.Equal("'DELIMITER' should not be used with MySqlConnector. See https://mysqlconnector.net/delimiter", exception.Message);
            Assert.Equal(
"""
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    CREATE TABLE `Table1` (
        `Id` int NOT NULL,
        `Foo` int NOT NULL,
        `Description` longtext NOT NULL,
        CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000001_Migration1', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000002_Migration2', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_get_active_provider()
        {
            base.Can_get_active_provider();

            Assert.Equal("Pomelo.EntityFrameworkCore.MySql", ActiveProvider);
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_2_2_model()
        {
            throw new NotImplementedException();
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_3_0_ASP_NET_Identity_model()
        {
            throw new NotImplementedException();
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_2_2_ASP_NET_Identity_model()
        {
            throw new NotImplementedException();
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_2_1_ASP_NET_Identity_model()
        {
            throw new NotImplementedException();
        }

        public override void Can_apply_all_migrations()
        {
            base.Can_apply_all_migrations();

            Assert.Null(Sql);
        }

        public override void Can_apply_range_of_migrations()
        {
            base.Can_apply_range_of_migrations();

            Assert.Null(Sql);
        }

        public override void Can_revert_all_migrations()
        {
            base.Can_revert_all_migrations();

            Assert.Null(Sql);
        }

        public override void Can_revert_one_migrations()
        {
            base.Can_revert_one_migrations();

            Assert.Null(Sql);
        }

        protected override Task ExecuteSqlAsync(string value)
            => ((MySqlTestStore)Fixture.TestStore).ExecuteNonQueryAsync(value);

        public override async Task Can_apply_all_migrations_async()
        {
            await base.Can_apply_all_migrations_async();

            Assert.Null(Sql);
        }

        public class MigrationsInfrastructureMySqlFixture : MigrationsInfrastructureFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlConnectionStringTestStoreFactory.Instance;
        }
    }
}
