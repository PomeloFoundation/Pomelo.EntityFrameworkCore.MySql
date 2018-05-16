using System;
using EFCore.MySql.Metadata.Internal;
using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Xunit;

// ReSharper disable InconsistentNaming
namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class MigrationSqlGeneratorMySqlTest : MigrationSqlGeneratorTestBase
    {
        [Fact]
        public virtual void It_lifts_foreign_key_additions()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "Pie",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            ClrType = typeof(int),
                            Name = "FlavorId",
                            ColumnType = "INT"
                        }
                    }
                }, new AddForeignKeyOperation
                {
                    Table = "Pie",
                    PrincipalTable = "Flavor",
                    Columns = new[] {"FlavorId"},
                    PrincipalColumns = new[] {"Id"}
                });

            Assert.Equal(
                @"CREATE TABLE `Pie` (
    `FlavorId` INT NOT NULL" + EOL +
                ");" + EOL +
                "GO" + EOL + EOL +
                @"ALTER TABLE `Pie` ADD FOREIGN KEY (`FlavorId`) REFERENCES `Flavor` (`Id`);" + EOL
                ,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [Fact]
        public virtual void DefaultValue_formats_literal_correctly()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "History",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Event",
                            ClrType = typeof(string),
                            ColumnType = "TEXT",
                            DefaultValue = new DateTime(2015, 4, 12, 17, 5, 0)
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `History` (
    `Event` TEXT NOT NULL DEFAULT '2015-04-12 17:05:00.000000'
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void EnsureSchemaOperation()
        {
            Generate(new EnsureSchemaOperation
            {
                Name = "mySchema"
            });

            Assert.Equal(
                @"CREATE DATABASE IF NOT EXISTS `mySchema`;"+ EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [Fact]
        public virtual void CreateDatabaseOperation()
        {
            Generate(new MySqlCreateDatabaseOperation { Name = "Northwind" });

            Assert.Equal(
                @"CREATE DATABASE `Northwind`;" + EOL,
                Sql);
        }

        [Fact]
        public override void CreateTableOperation()
        {
            base.CreateTableOperation();

            Assert.Equal(
                "CREATE TABLE `dbo`.`People` (" + EOL +
                "    `Id` int NOT NULL," + EOL +
                "    `EmployerId` int," + EOL +
                "    `SSN` char(11)," + EOL +
                "    PRIMARY KEY (`Id`)," + EOL +
                "    UNIQUE (`SSN`)," + EOL +
                "    FOREIGN KEY (`EmployerId`) REFERENCES `Companies` (`Id`)" + EOL +
                ");" + EOL,
                Sql);
        }

        [Fact]
        public virtual void CreateTableUlongAutoincrement()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "TestUlongAutoIncrement",
                    Schema = "dbo",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Id",
                            Table = "TestUlongAutoIncrement",
                            ClrType = typeof(ulong),
                            ColumnType = "bigint unsigned",
                            IsNullable = false,
                            [MySqlAnnotationNames.ValueGenerationStrategy] = MySqlValueGenerationStrategy.IdentityColumn
                        }
                    },
                    PrimaryKey = new AddPrimaryKeyOperation
                    {
                        Columns = new[] { "Id" }
                    }
                });

            Assert.Equal(
                "CREATE TABLE `dbo`.`TestUlongAutoIncrement` (" + EOL +
                "    `Id` bigint unsigned NOT NULL AUTO_INCREMENT," + EOL +
                "    PRIMARY KEY (`Id`)" + EOL +
                ");" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_ansi()
        {
            base.AddColumnOperation_with_ansi();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` longtext CHARACTER SET latin1;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_defaultValue()
        {
            base.AddColumnOperation_with_defaultValue();

            Assert.Equal(
                @"ALTER TABLE `dbo`.`People` ADD `Name` varchar(30) NOT NULL DEFAULT N'John Doe';" + EOL,
                Sql);
        }

        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Alias` longtext CHARACTER SET ucs2 NOT NULL;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_defaultValueSql()
        {
            base.AddColumnOperation_with_defaultValueSql();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Birthday` date DEFAULT CURRENT_TIMESTAMP;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_with_datetime6()
        {
            Generate(new AddColumnOperation
            {
                Table = "People",
                Name = "Birthday",
                ClrType = typeof(DateTime),
                ColumnType = "timestamp(6)",
                IsNullable = false,
                DefaultValue = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            });

            Assert.Equal(
                "ALTER TABLE `People` ADD `Birthday` timestamp(6) NOT NULL DEFAULT '" + new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "';" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_maxLength()
        {
            base.AddColumnOperation_with_maxLength();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(30) CHARACTER SET ucs2;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_maxLength_overridden()
        {
            base.AddColumnOperation_with_maxLength_overridden();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(32) CHARACTER SET ucs2;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_maxLength_on_derived()
        {
            base.AddColumnOperation_with_maxLength_on_derived();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(30) CHARACTER SET ucs2;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_shared_column()
        {
            base.AddColumnOperation_with_shared_column();

            Assert.Equal(
                @"ALTER TABLE `Base` ADD `Foo` longtext CHARACTER SET ucs2;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_computed_column_SQL()
        {
            base.AddColumnOperation_with_computed_column_SQL();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Birthday` date AS CURRENT_TIMESTAMP;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_with_computed_column()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "Birthday",
                    ClrType = typeof(DateTime),
                    ColumnType = "timestamp",
                    IsNullable = true,
                    [MySqlAnnotationNames.ValueGenerationStrategy] = MySqlValueGenerationStrategy.ComputedColumn
                });

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Birthday` timestamp DEFAULT CURRENT_TIMESTAMP() ON UPDATE CURRENT_TIMESTAMP();" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_serial()
        {
            Generate(new AddColumnOperation
            {
                Table = "People",
                Name = "foo",
                ClrType = typeof(int),
                ColumnType = "int",
                IsNullable = false,
                [MySqlAnnotationNames.ValueGenerationStrategy] = MySqlValueGenerationStrategy.IdentityColumn
            });

            Assert.Equal(
                "ALTER TABLE `People` ADD `foo` int NOT NULL AUTO_INCREMENT;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_with_int_defaultValue_isnt_serial()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "foo",
                    ClrType = typeof(int),
                    ColumnType = "int",
                    IsNullable = false,
                    DefaultValue = 8
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `foo` int NOT NULL DEFAULT 8;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_with_dbgenerated_uuid()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "foo",
                    ClrType = typeof(Guid),
                    ColumnType = "varchar(38)",
                    [MySqlAnnotationNames.ValueGenerationStrategy] = MySqlValueGenerationStrategy.IdentityColumn
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `foo` varchar(38) NOT NULL;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddDefaultDatetimeOperation_with_valueOnUpdate()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "Birthday",
                    ClrType = typeof(DateTime),
                    ColumnType = "timestamp(6)",
                    IsNullable = true,
                    [MySqlAnnotationNames.ValueGenerationStrategy] = MySqlValueGenerationStrategy.ComputedColumn
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `Birthday` timestamp(6) DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AddDefaultBooleanOperation()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "IsLeader",
                    ClrType = typeof(Boolean),
                    ColumnType = "bit",
                    IsNullable = true,
                    DefaultValue = true
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `IsLeader` bit DEFAULT TRUE;" + EOL,
                Sql);
        }

        public override void AlterColumnOperation()
        {
            base.AlterColumnOperation();

            Assert.Equal(
                @"ALTER TABLE `dbo`.`People` MODIFY COLUMN `LuckyNumber` int NOT NULL;" + EOL +
                @"ALTER TABLE `dbo`.`People` ALTER COLUMN `LuckyNumber` SET DEFAULT 7" + EOL,
                Sql, false, true, true);
        }


        public override void AlterColumnOperation_without_column_type()
        {
            base.AlterColumnOperation_without_column_type();

            Assert.Equal(
                @"ALTER TABLE `People` MODIFY COLUMN `LuckyNumber` int NOT NULL;" + EOL +
                @"ALTER TABLE `People` ALTER COLUMN `LuckyNumber` DROP DEFAULT;",
                Sql);
        }

        [Fact]
        public virtual void AlterColumnOperation_dbgenerated_uuid()
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "GuidKey",
                    ClrType = typeof(int),
                    ColumnType = "char(38)",
                    IsNullable = false,
                    [MySqlAnnotationNames.ValueGenerationStrategy] = MySqlValueGenerationStrategy.IdentityColumn
                });

            Assert.Equal(
                @"ALTER TABLE `People` MODIFY COLUMN `GuidKey` char(38) NOT NULL;" + EOL +
                @"ALTER TABLE `People` ALTER COLUMN `GuidKey` DROP DEFAULT;",
                Sql, false , true, true);
        }

        [Theory]
        [InlineData("tinyblob")]
        [InlineData("blob")]
        [InlineData("mediumblob")]
        [InlineData("longblob")]

        [InlineData("tinytext")]
        [InlineData("text")]
        [InlineData("mediumtext")]
        [InlineData("longtext")]

        [InlineData("geometry")]
        [InlineData("point")]
        [InlineData("linestring")]
        [InlineData("polygon")]
        [InlineData("multipoint")]
        [InlineData("multilinestring")]
        [InlineData("multipolygon")]
        [InlineData("geometrycollection")]

        [InlineData("json")]
        public virtual void AlterColumnOperation_with_no_default_value_column_types(string type)
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Blob",
                    ClrType = typeof(string),
                    ColumnType = type,
                    IsNullable = true,
                });

            Assert.Equal(
                $"ALTER TABLE `People` MODIFY COLUMN `Blob` {type} NULL;" + EOL,
                Sql);
        }

        public override void AddForeignKeyOperation_with_name()
        {
            base.AddForeignKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` ADD CONSTRAINT `FK_People_Companies` FOREIGN KEY (`EmployerId1`, `EmployerId2`) REFERENCES `hr`.`Companies` (`Id1`, `Id2`) ON DELETE CASCADE;" + EOL,
                Sql);
        }

        public override void AddForeignKeyOperation_without_name()
        {
            base.AddForeignKeyOperation_without_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD FOREIGN KEY (`SpouseId`) REFERENCES `People` (`Id`);" + EOL,
                Sql);
        }

        [Fact]
        public override void AddPrimaryKeyOperation_with_name()
        {
            base.AddPrimaryKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` ADD CONSTRAINT `PK_People` PRIMARY KEY (`Id1`, `Id2`);" + EOL,
                Sql);
        }

        [Fact]
        public override void AddPrimaryKeyOperation_without_name()
        {
            base.AddPrimaryKeyOperation_without_name();

            var test =
                "ALTER TABLE `People` ADD PRIMARY KEY (`Id`);" + EOL +
                @"DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;
CREATE PROCEDURE POMELO_AFTER_ADD_PRIMARY_KEY(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
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
CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'People', 'Id');
DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;".Replace("\r", string.Empty).Replace("\n", EOL) + EOL;

            Assert.Equal(test,
                Sql);
        }

        [Fact]
        public override void AddUniqueConstraintOperation_with_name()
        {
            base.AddUniqueConstraintOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` ADD CONSTRAINT `AK_People_DriverLicense` UNIQUE (`DriverLicense_State`, `DriverLicense_Number`);" + EOL,
                Sql);
        }

        [Fact]
        public override void AddUniqueConstraintOperation_without_name()
        {
            base.AddUniqueConstraintOperation_without_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD UNIQUE (`SSN`);" + EOL,
                Sql);
        }

        [Fact]
        public void DropSchemaOperation()
        {
            Generate(new DropSchemaOperation
            {
                Name = "mySchema"
            });

            Assert.Equal(
                @"DROP SCHEMA `mySchema`;" + EOL,
                Sql);
        }

        [Fact]
        public override void CreateIndexOperation_unique()
        {
            base.CreateIndexOperation_unique();

            Assert.Equal(
                "CREATE UNIQUE INDEX `IX_People_Name` ON `dbo`.`People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        [Fact]
        public virtual void CreateIndexOperation_fulltext()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Schema = "dbo",
                    Columns = new[] { "FirstName", "LastName" },
                    [MySqlAnnotationNames.FullTextIndex] = "FULLTEXT"
                });

            Assert.Equal(
                "CREATE FULLTEXT INDEX `IX_People_Name` ON `dbo`.`People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        [Fact]
        public virtual void CreateIndexOperation_spatial()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Schema = "dbo",
                    Columns = new[] { "FirstName", "LastName" },
                    [MySqlAnnotationNames.SpatialIndex] = "SPATIAL"
                });

            Assert.Equal(
                "CREATE SPATIAL INDEX `IX_People_Name` ON `dbo`.`People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        [Fact]
        public override void CreateIndexOperation_nonunique()
        {
            base.CreateIndexOperation_nonunique();

            Assert.Equal(
                "CREATE INDEX `IX_People_Name` ON `People` (`Name`);" + EOL,
                Sql);
        }

        [Fact]
        public virtual void RenameIndexOperation()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.RenameIndex(
                table: "Person",
                name: "IX_Person_Name",
                newName: "IX_Person_FullName");

            Generate(migrationBuilder.Operations.ToArray());

            Assert.Equal(
                @"ALTER TABLE `Person` RENAME INDEX `IX_Person_Name` TO `IX_Person_FullName`;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void RenameIndexOperation_with_model()
        {
            Generate(
                modelBuilder => modelBuilder.Entity(
                    "Person",
                    x =>
                    {
                        x.Property<string>("FullName");
                        x.HasIndex("FullName").IsUnique().HasFilter("`Id` > 2");
                    }),
                new RenameIndexOperation
                {
                    Table = "Person",
                    Name = "IX_Person_Name",
                    NewName = "IX_Person_FullName"
                });

            Assert.Equal(
                @"ALTER TABLE `Person` RENAME INDEX `IX_Person_Name` TO `IX_Person_FullName`;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void RenameColumnOperation()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.RenameColumn(
                table: "Person",
                name: "Name",
                newName: "FullName")
                .Annotation(RelationalAnnotationNames.ColumnType, "VARCHAR(4000)");

            Generate(migrationBuilder.Operations.ToArray());

            Assert.Equal(
                "ALTER TABLE `Person` CHANGE `Name` `FullName` VARCHAR(4000)",
                Sql);
        }

        [Fact]
        public virtual void RenameColumnOperation_with_model()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.RenameColumn(
                table: "Person",
                name: "Name",
                newName: "FullName");

            Generate(
                modelBuilder => modelBuilder.Entity(
                    "Person",
                    x =>
                    {
                        x.Property<string>("FullName");
                    }),
                migrationBuilder.Operations.ToArray());

            Assert.Equal(
                "ALTER TABLE `Person` CHANGE `Name` `FullName` longtext CHARACTER SET ucs2",
                Sql);
        }

        [Fact]
        public override void DropColumnOperation()
        {
            base.DropColumnOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP COLUMN `LuckyNumber`;",
                Sql);
        }

        [Fact]
        public override void DropForeignKeyOperation()
        {
            base.DropForeignKeyOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP FOREIGN KEY `FK_People_Companies`;" + EOL,
                Sql);
        }

        [Fact]
        public override void DropPrimaryKeyOperation()
        {
            base.DropPrimaryKeyOperation();

            Assert.Equal(
                @"DROP PROCEDURE IF EXISTS POMELO_BEFORE_DROP_PRIMARY_KEY;
CREATE PROCEDURE POMELO_BEFORE_DROP_PRIMARY_KEY(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
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
CALL POMELO_BEFORE_DROP_PRIMARY_KEY('dbo', 'People');
DROP PROCEDURE IF EXISTS POMELO_BEFORE_DROP_PRIMARY_KEY;
ALTER TABLE `dbo`.`People` DROP PRIMARY KEY;".Replace("\r", string.Empty).Replace("\n", EOL) + EOL,
                Sql);
        }

        [Fact]
        public override void DropTableOperation()
        {
            base.DropTableOperation();

            Assert.Equal(
                "DROP TABLE `dbo`.`People`;" + EOL,
                Sql);
        }

        [Fact]
        public override void DropUniqueConstraintOperation()
        {
            base.DropUniqueConstraintOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP CONSTRAINT `AK_People_SSN`;" + EOL,
                Sql);
        }

        public override void DropIndexOperation()
        {
            base.DropIndexOperation();

            Assert.Equal(
                @"ALTER TABLE `dbo`.`People` DROP INDEX `IX_People_Name`;" + EOL,
                Sql);
        }

        [Fact]
        public override void SqlOperation()
        {
            base.SqlOperation();

            Assert.Equal(
                "-- I <3 DDL" + EOL,
                Sql);
        }

        // MySql doesn't support sequence
        public override void AlterSequenceOperation_with_minValue_and_maxValue()
        {
        }

        public override void AlterSequenceOperation_without_minValue_and_maxValue()
        {
        }

        public override void CreateSequenceOperation_with_minValue_and_maxValue()
        {
        }

        public override void CreateSequenceOperation_with_minValue_and_maxValue_not_long()
        {
        }

        public override void CreateSequenceOperation_without_minValue_and_maxValue()
        {
        }

        public override void DropSequenceOperation()
        {
        }

        public MigrationSqlGeneratorMySqlTest()
            : base(MySqlTestHelpers.Instance)
        {
        }
    }
}
