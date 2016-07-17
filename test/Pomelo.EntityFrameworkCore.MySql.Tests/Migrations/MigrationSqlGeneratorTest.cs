using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Migrations
{
    public class MigrationSqlGeneratorTest : MigrationSqlGeneratorTestBase
    {
        protected override IMigrationsSqlGenerator SqlGenerator
        {
            get
            {
                var typeMapper = new MySqlTypeMapper();

                return new MySqlMigrationsSqlGenerationHelper(
                    new RelationalCommandBuilderFactory(
                        new FakeSensitiveDataLogger<RelationalCommandBuilderFactory>(),
                        new DiagnosticListener("Fake"),
                        typeMapper),
                    new MySqlSqlGenerationHelper(),
                    typeMapper,
                    new MySqlAnnotationProvider());
            }
        }

        public override void AddColumnOperation_with_defaultValue()
        {
            base.AddColumnOperation_with_defaultValue();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` ADD `Name` varchar(30) NOT NULL DEFAULT 'John Doe';" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_defaultValueSql()
        {
            base.AddColumnOperation_with_defaultValueSql();

            Assert.Equal(
                "ALTER TABLE `People` ADD `Birthday` date DEFAULT CURRENT_TIMESTAMP;" + EOL,
                Sql);
        }

        [Fact]
        public override void AddColumnOperation_with_computed_column_SQL()
        {
            base.AddColumnOperation_with_computed_column_SQL();

            Assert.Equal(
                "ALTER TABLE `People` ADD `Birthday` date;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            Assert.Equal(
                "ALTER TABLE `People` ADD `Alias` text NOT NULL;" + EOL,
                Sql);
        }

        public override void AddColumnOperation_with_maxLength()
        {
            base.AddColumnOperation_with_maxLength();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(30);" + EOL,
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

        public override void AddPrimaryKeyOperation_with_name()
        {
            base.AddPrimaryKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` ADD CONSTRAINT `PK_People` PRIMARY KEY (`Id1`, `Id2`);" + EOL,
                Sql);
        }

        public override void AddPrimaryKeyOperation_without_name()
        {
            base.AddPrimaryKeyOperation_without_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD PRIMARY KEY (`Id`);" + EOL,
                Sql);
        }

        public override void AddUniqueConstraintOperation_with_name()
        {
            base.AddUniqueConstraintOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` ADD CONSTRAINT `AK_People_DriverLicense` UNIQUE (`DriverLicense_State`, `DriverLicense_Number`);" + EOL,
                Sql);
        }

        public override void AddUniqueConstraintOperation_without_name()
        {
            base.AddUniqueConstraintOperation_without_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD UNIQUE (`SSN`);" + EOL,
                Sql);
        }

        public override void CreateIndexOperation_unique()
        {
            base.CreateIndexOperation_unique();

            Assert.Equal(
                "CREATE UNIQUE INDEX `IX_People_Name` ON `dbo`.`People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        public override void CreateIndexOperation_nonunique()
        {
            base.CreateIndexOperation_nonunique();

            Assert.Equal(
                "CREATE INDEX `IX_People_Name` ON `People` (`Name`);" + EOL,
                Sql);
        }

        [Fact(Skip ="true")]
        public virtual void CreateDatabaseOperation()
        {
            Generate(new MySqlCreateDatabaseOperation { Name = "Northwind" });

            Assert.Equal(
                @"CREATE SCHEMA  `Northwind`;" + EOL,
                Sql);
        }

        [Fact(Skip="true")]
        public virtual void CreateDatabaseOperation_with_template()
        {
            Generate(new MySqlCreateDatabaseOperation
            {
                Name = "Northwind",
                Template = "MyTemplate"
            });

            Assert.Equal(
                @"CREATE DATABASE `Northwind` TEMPLATE `MyTemplate`;" + EOL,
                Sql);
        }


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

        public override void DropColumnOperation()
        {
            base.DropColumnOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP COLUMN `LuckyNumber`;" + EOL,
                Sql);
        }

        public override void DropForeignKeyOperation()
        {
            base.DropForeignKeyOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP CONSTRAINT `FK_People_Companies`;" + EOL,
                Sql);
        }

        public override void DropPrimaryKeyOperation()
        {
            base.DropPrimaryKeyOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP CONSTRAINT `PK_People`;" + EOL,
                Sql);
        }

        public override void DropTableOperation()
        {
            base.DropTableOperation();

            Assert.Equal(
                "DROP TABLE `dbo`.`People`;" + EOL,
                Sql);
        }

        public override void DropUniqueConstraintOperation()
        {
            base.DropUniqueConstraintOperation();

            Assert.Equal(
                "ALTER TABLE `dbo`.`People` DROP CONSTRAINT `AK_People_SSN`;" + EOL,
                Sql);
        }

        public override void SqlOperation()
        {
            base.SqlOperation();

            Assert.Equal(
                "-- I <3 DDL;" + EOL,
                Sql);
        }

        #region AlterColumn

        public override void AlterColumnOperation()
        {
            base.AlterColumnOperation();

            Assert.Equal(
                @"ALTER TABLE `dbo`.`People` ALTER COLUMN `LuckyNumber` TYPE int;" + EOL +
                @"ALTER TABLE `dbo`.`People` ALTER COLUMN `LuckyNumber` SET NOT NULL;" + EOL +
                @"ALTER TABLE `dbo`.`People` ALTER COLUMN `LuckyNumber` SET DEFAULT 7",
            Sql);
        }

        public override void AlterColumnOperation_without_column_type()
        {
            base.AlterColumnOperation_without_column_type();

            Assert.Equal(
                @"ALTER TABLE `People` ALTER COLUMN `LuckyNumber` TYPE int4;" + EOL +
                @"ALTER TABLE `People` ALTER COLUMN `LuckyNumber` SET NOT NULL;" + EOL +
                @"ALTER TABLE `People` ALTER COLUMN `LuckyNumber` DROP DEFAULT",
            Sql);
        }

        [Fact]
        public void AlterColumnOperation_dbgenerated_uuid()
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "GuidKey",
                    ClrType = typeof(int),
                    ColumnType = "uuid",
                    IsNullable = false,
                    [MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAdd] = true
                });

            Assert.Equal(
                @"ALTER TABLE `People` ALTER COLUMN `GuidKey` TYPE varchar(38);" + EOL +
                @"ALTER TABLE `People` ALTER COLUMN `GuidKey` SET NOT NULL;" + EOL +
                @"ALTER TABLE `People` ALTER COLUMN `GuidKey` SET DEFAULT (uuid_generate_v4())",
            Sql);
        }

        #endregion

        #region Npgsql-specific

        [Fact]
        public void CreateIndexOperation_method()
        {
            Generate(new CreateIndexOperation
            {
                Name = "IX_People_Name",
                Table = "People",
                Schema = "dbo",
                Columns = new[] { "FirstName" },
                [MySqlAnnotationNames.Prefix + MySqlAnnotationNames.IndexMethod] = "gin"
            });

            Assert.Equal(
                "CREATE INDEX `IX_People_Name` ON `dbo`.`People` USING gin (`FirstName`);" + EOL,
                Sql);
        }

        [Fact]
        public void CreatePostgresExtension()
        {
            Generate(new MySqlCreateDatabaseOperation
            {
                Name = "hstore",
            });

            Assert.Equal(
                @"CREATE EXTENSION `hstore`;" + EOL,
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
                [MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAdd] = true
            });

            Assert.Equal(
                "ALTER TABLE `People` ADD `foo` int AUTO_INCREMENT NOT NULL;" + EOL,
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
                    DefaultValue = "8"
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `foo` int NOT NULL DEFAULT '8';" + EOL,
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
                    [MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAdd] = true
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `foo` varchar(38) NOT NULL;" + EOL,
                Sql);
        }

        #endregion
    }
}
