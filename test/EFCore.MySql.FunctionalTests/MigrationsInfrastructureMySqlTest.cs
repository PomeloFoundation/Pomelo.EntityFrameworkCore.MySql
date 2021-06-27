using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.TestUtilities;
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

        public override void Can_generate_up_scripts()
        {
            base.Can_generate_up_scripts();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

CREATE TABLE `Table1` (
    `Id` int NOT NULL,
    `Foo` int NOT NULL,
    CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_Migration1', '7.0.0-test');

COMMIT;

START TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000003_Migration3', '7.0.0-test');

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_one_up_script()
        {
            base.Can_generate_one_up_script();

            Assert.Equal(
                @"START TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_up_script_using_names()
        {
            base.Can_generate_up_script_using_names();

            Assert.Equal(
                @"START TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_idempotent_up_scripts()
        {
            base.Can_generate_idempotent_up_scripts();

            Assert.Equal(@"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
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

COMMIT;

START TRANSACTION;


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

START TRANSACTION;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000003_Migration3') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000003_Migration3', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_down_scripts()
        {
            base.Can_generate_down_scripts();

            Assert.Equal(
                @"START TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT;

START TRANSACTION;

DROP TABLE `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000001_Migration1';

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_one_down_script()
        {
            base.Can_generate_one_down_script();

            Assert.Equal(
                @"START TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_down_script_using_names()
        {
            base.Can_generate_down_script_using_names();

            Assert.Equal(
                @"START TRANSACTION;

ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_idempotent_down_scripts()
        {
            base.Can_generate_idempotent_down_scripts();

            Assert.Equal(
                @"START TRANSACTION;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    ALTER TABLE `Table1` RENAME COLUMN `Bar` TO `Foo`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    DELETE FROM `__EFMigrationsHistory`
    WHERE `MigrationId` = '00000000000002_Migration2';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    DROP TABLE `Table1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    DELETE FROM `__EFMigrationsHistory`
    WHERE `MigrationId` = '00000000000001_Migration1';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_get_active_provider()
        {
            base.Can_get_active_provider();

            Assert.Equal("Pomelo.EntityFrameworkCore.MySql", ActiveProvider);
        }

        public override void Can_generate_up_scripts_noTransactions()
        {
            base.Can_generate_up_scripts_noTransactions();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Table1` (
    `Id` int NOT NULL,
    `Foo` int NOT NULL,
    CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_Migration1', '7.0.0-test');

ALTER TABLE `Table1` RENAME COLUMN `Foo` TO `Bar`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000003_Migration3', '7.0.0-test');

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_idempotent_up_scripts_noTransactions()
        {
            base.Can_generate_idempotent_up_scripts_noTransactions();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
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


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000003_Migration3') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('00000000000003_Migration3', '7.0.0-test');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

",
                Sql,
                ignoreLineEndingDifferences: true);
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

        public class MigrationsInfrastructureMySqlFixture : MigrationsInfrastructureFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlConnectionStringTestStoreFactory.Instance;
        }
    }
}
