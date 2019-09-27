using System; 
using System.Data.Common; 
using Microsoft.EntityFrameworkCore; 
using Xunit; 
 
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests 
{
    // Made internal to skip all tests.
    internal class MigrationsMySqlTest : MigrationsTestBase<MigrationsMySqlFixture> 
    { 
        public MigrationsMySqlTest(MigrationsMySqlFixture fixture) 
            : base(fixture) 
        { 
        } 
 
        public override void Can_generate_migration_from_initial_database_to_initial() 
        { 
            base.Can_generate_migration_from_initial_database_to_initial(); 
 
            Assert.Equal( 
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_no_migration_script() 
        { 
            base.Can_generate_no_migration_script(); 
 
            Assert.Equal( 
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_up_scripts() 
        { 
            base.Can_generate_up_scripts(); 
 
            Assert.Equal( 
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Table1` (
    `Id` int NOT NULL,
    CONSTRAINT `PK_Table1` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000001_Migration1', '7.0.0-test');

ALTER TABLE `Table1` RENAME `Table2`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000003_Migration3', '7.0.0-test');

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_one_up_script() 
        { 
            base.Can_generate_one_up_script(); 
 
            Assert.Equal( 
                @"ALTER TABLE `Table1` RENAME `Table2`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_up_script_using_names() 
        { 
            base.Can_generate_up_script_using_names(); 
 
            Assert.Equal( 
                @"ALTER TABLE `Table1` RENAME `Table2`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000002_Migration2', '7.0.0-test');

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_idempotent_up_scripts() 
        { 
            base.Can_generate_idempotent_up_scripts(); 
 
            Assert.Equal(@"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000001_Migration1') THEN

    CREATE TABLE `Table1` (
        `Id` int NOT NULL,
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

    ALTER TABLE `Table1` RENAME `Table2`;

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
 
        public override void Can_generate_down_scripts() 
        { 
            base.Can_generate_down_scripts(); 
 
            Assert.Equal( 
                @"ALTER TABLE `Table2` RENAME `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

DROP TABLE `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000001_Migration1';

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_one_down_script() 
        { 
            base.Can_generate_one_down_script(); 
 
            Assert.Equal( 
                @"ALTER TABLE `Table2` RENAME `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_down_script_using_names() 
        { 
            base.Can_generate_down_script_using_names(); 
 
            Assert.Equal( 
                @"ALTER TABLE `Table2` RENAME `Table1`;

DELETE FROM `__EFMigrationsHistory`
WHERE `MigrationId` = '00000000000002_Migration2';

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_generate_idempotent_down_scripts() 
        { 
            base.Can_generate_idempotent_down_scripts();
 
            Assert.Equal(
                @"
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '00000000000002_Migration2') THEN

    ALTER TABLE `Table2` RENAME `Table1`;

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

", 
                Sql, 
                ignoreLineEndingDifferences: true); 
        } 
 
        public override void Can_get_active_provider() 
        { 
            base.Can_get_active_provider(); 
 
            Assert.Equal("Pomelo.EntityFrameworkCore.MySql", ActiveProvider); 
        } 
 
        protected override void AssertFirstMigration(DbConnection connection) 
        { 
            // TODO: Add assert 
        } 
 
        protected override void AssertSecondMigration(DbConnection connection) 
        { 
            // TODO: Add assert 
        }

        public override void Can_diff_against_2_2_model()
        {
            // TODO: Add diff
            throw new NotImplementedException();
        }

        public override void Can_diff_against_3_0_ASP_NET_Identity_model()
        {
            // TODO: Add diff
            throw new NotImplementedException();
        }

        public override void Can_diff_against_2_2_ASP_NET_Identity_model()
        {
            // TODO: Add diff
            throw new NotImplementedException();
        }

        public override void Can_diff_against_2_1_ASP_NET_Identity_model()
        {
            // TODO: Add diff 
            throw new NotImplementedException();
        }
    }
} 
