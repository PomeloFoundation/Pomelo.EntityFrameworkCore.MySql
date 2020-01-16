using System;
using System.Linq;
using System.Reflection;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MigrationSqlGeneratorMySqlTest : MigrationSqlGeneratorTestBase
    {
        [ConditionalFact]
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
                    Columns = new[] { "FlavorId" },
                    PrincipalColumns = new[] { "Id" }
                });

            Assert.Equal(
                @"CREATE TABLE `Pie` (
    `FlavorId` INT NOT NULL" + EOL +
                ");" + EOL +
                EOL +
                @"ALTER TABLE `Pie` ADD FOREIGN KEY (`FlavorId`) REFERENCES `Flavor` (`Id`);" + EOL
                ,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
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

        [ConditionalFact]
        public virtual void CreateDatabaseOperation()
        {
            Generate(new MySqlCreateDatabaseOperation { Name = "Northwind" });

            Assert.Equal(
                @"CREATE DATABASE `Northwind`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void CreateTableOperation()
        {
            base.CreateTableOperation();

            Assert.Equal(
                @"CREATE TABLE `People` (
    `Id` int NOT NULL,
    `EmployerId` int NULL COMMENT 'Employer ID comment',
    `SSN` char(11) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE (`SSN`),
    CHECK (SSN > 0),
    FOREIGN KEY (`EmployerId`) REFERENCES `Companies` (`Id`)
) COMMENT 'Table comment';
", Sql);
        }

        [ConditionalFact]
        public virtual void CreateTableUlongAutoincrement()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "TestUlongAutoIncrement",
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
                "CREATE TABLE `TestUlongAutoIncrement` (" + EOL +
                "    `Id` bigint unsigned NOT NULL AUTO_INCREMENT," + EOL +
                "    PRIMARY KEY (`Id`)" + EOL +
                ");" + EOL,
                Sql);
        }

        [ConditionalTheory]
        [InlineData(true, false, CharSetBehavior.AppendToAllAnsiColumns, "Latin1", false)]
        [InlineData(true, false, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Latin1", false)]
        [InlineData(true, false, CharSetBehavior.AppendToAllUnicodeColumns, "Latin1", true)]
        [InlineData(true, false, CharSetBehavior.AppendToAllColumns, "Latin1", true)]
        [InlineData(true, true, CharSetBehavior.AppendToAnsiIndexAndKeyColumns, "Latin1", false)]
        [InlineData(true, true, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Latin1", true)]
        [InlineData(true, true, CharSetBehavior.AppendToAllUnicodeColumns, "Latin1", true)]
        [InlineData(true, true, CharSetBehavior.AppendToAllColumns, "Latin1", true)]
        [InlineData(false, false, CharSetBehavior.AppendToAllUnicodeColumns, "Latin1", false)]
        [InlineData(false, false, CharSetBehavior.AppendToAnsiIndexAndKeyColumns, "Latin1", false)]
        [InlineData(false, false, CharSetBehavior.AppendToAllAnsiColumns, "Latin1", true)]
        [InlineData(false, false, CharSetBehavior.AppendToAllColumns, "Latin1", true)]
        [InlineData(false, true, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Latin1", false)]
        [InlineData(false, true, CharSetBehavior.AppendToAnsiIndexAndKeyColumns, "Latin1", true)]
        [InlineData(false, true, CharSetBehavior.AppendToAllAnsiColumns, "Latin1", true)]
        [InlineData(false, true, CharSetBehavior.AppendToAllColumns, "Latin1", true)]
        [InlineData(null, false, CharSetBehavior.AppendToAllAnsiColumns, "Latin1", true)]
        [InlineData(null, false, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Latin1", false)]
        [InlineData(null, false, CharSetBehavior.AppendToAllUnicodeColumns, "Latin1", false)]
        [InlineData(null, false, CharSetBehavior.AppendToAllColumns, "Latin1", true)]
        [InlineData(null, true, CharSetBehavior.AppendToAnsiIndexAndKeyColumns, "Latin1", true)]
        [InlineData(null, true, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Latin1", false)]
        [InlineData(null, true, CharSetBehavior.AppendToAllUnicodeColumns, "Latin1", false)]
        [InlineData(null, true, CharSetBehavior.AppendToAllColumns, "Latin1", true)]
        [InlineData(null, false, CharSetBehavior.AppendToAllAnsiColumns, "Utf8Mb4", false)]
        [InlineData(null, false, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Utf8Mb4", false)]
        [InlineData(null, false, CharSetBehavior.AppendToAllUnicodeColumns, "Utf8Mb4", true)]
        [InlineData(null, false, CharSetBehavior.AppendToAllColumns, "Utf8Mb4", true)]
        [InlineData(null, true, CharSetBehavior.AppendToAnsiIndexAndKeyColumns, "Utf8Mb4", false)]
        [InlineData(null, true, CharSetBehavior.AppendToUnicodeIndexAndKeyColumns, "Utf8Mb4", true)]
        [InlineData(null, true, CharSetBehavior.AppendToAllUnicodeColumns, "Utf8Mb4", true)]
        [InlineData(null, true, CharSetBehavior.AppendToAllColumns, "Utf8Mb4", true)]
        public virtual void AddColumnOperation_with_charSet_implicit(bool? isUnicode, bool isIndex, CharSetBehavior charSetBehavior,
            string charSetName, bool expectExplicitCharSet)
        {
            var charSet = CharSet.GetCharSetFromName(charSetName);
            var expectedCharSetName = expectExplicitCharSet ? $" CHARACTER SET {charSet}" : string.Empty;

            Generate(
                modelBuilder => modelBuilder.Entity("Person", eb =>
                    {
                        var pb = eb.Property<string>("Name");

                        if (isUnicode.HasValue)
                        {
                            pb.IsUnicode(isUnicode.Value);
                        }

                        if (isIndex)
                        {
                            eb.HasIndex("Name");
                        }
                    }),
                charSetBehavior,
                charSet,
                new AddColumnOperation
                {
                    Table = "Person",
                    Name = "Name",
                    ClrType = typeof(string),
                    IsUnicode = isUnicode,
                    IsNullable = true
                });

            var columnType = "longtext";
            if (isIndex)
            {
                var serverVersion = new ServerVersion();
                var columnSize = Math.Min(serverVersion.MaxKeyLength / (charSet.MaxBytesPerChar * 2), 255);
                columnType = $"varchar({columnSize})";
            }

            Assert.Equal(
                $"ALTER TABLE `Person` ADD `Name` {columnType}{expectedCharSetName} NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_ansi()
        {
            base.AddColumnOperation_with_ansi();

            Assert.Equal(
                $@"ALTER TABLE `Person` ADD `Name` longtext CHARACTER SET utf8mb4 NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_defaultValue()
        {
            base.AddColumnOperation_with_defaultValue();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Name` varchar(30) NOT NULL DEFAULT 'John Doe';" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Alias` longtext CHARACTER SET utf8mb4 NOT NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_defaultValueSql()
        {
            base.AddColumnOperation_with_defaultValueSql();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Birthday` date NULL DEFAULT CURRENT_TIMESTAMP;" + EOL,
                Sql);
        }

        [ConditionalFact]
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
                "ALTER TABLE `People` ADD `Birthday` timestamp(6) NOT NULL DEFAULT '" +
                new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).ToString("yyyy-MM-dd HH:mm:ss.ffffff") +
                "';" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_maxLength()
        {
            base.AddColumnOperation_with_maxLength();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(30) CHARACTER SET utf8mb4 NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_maxLength_overridden()
        {
            base.AddColumnOperation_with_maxLength_overridden();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(32) CHARACTER SET utf8mb4 NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_maxLength_on_derived()
        {
            base.AddColumnOperation_with_maxLength_on_derived();

            Assert.Equal(
                @"ALTER TABLE `Person` ADD `Name` varchar(30) CHARACTER SET utf8mb4 NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_shared_column()
        {
            base.AddColumnOperation_with_shared_column();

            Assert.Equal(
                @"ALTER TABLE `Base` ADD `Foo` longtext CHARACTER SET utf8mb4 NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
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
                @"ALTER TABLE `People` ADD `Birthday` timestamp NULL DEFAULT CURRENT_TIMESTAMP() ON UPDATE CURRENT_TIMESTAMP();" +
                EOL,
                Sql);
        }

        [ConditionalFact]
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

        [ConditionalFact]
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

        [ConditionalFact]
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

        [ConditionalFact]
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
                "ALTER TABLE `People` ADD `Birthday` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);" +
                EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void AddDefaultBooleanOperation()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "IsLeader",
                    ClrType = typeof(bool),
                    ColumnType = "bit",
                    IsNullable = true,
                    DefaultValue = true
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `IsLeader` bit NULL DEFAULT TRUE;" + EOL,
                Sql);
        }


        [ConditionalTheory]
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
        public void AlterColumnOperation_with_no_default_value_column_types(string type)
        {
            Generate(
                builder =>
                {
                    ((Model)builder.Model).SetProductVersion("2.1.0");
                },
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Blob",
                    ClrType = typeof(string),
                    ColumnType = type,
                    OldColumn = new ColumnOperation
                    {
                        ColumnType = type,
                    },
                    IsNullable = true,
                });

            Assert.Equal(
                $"ALTER TABLE `People` MODIFY COLUMN `Blob` {type} NULL;" + EOL,
                Sql);

            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Blob",
                    ClrType = typeof(string),
                    ColumnType = type,
                    OldColumn = new ColumnOperation
                    {
                        ColumnType = "varchar(127)",
                    },
                    IsNullable = true,
                });

            Assert.Equal(
                $"ALTER TABLE `People` MODIFY COLUMN `Blob` {type} NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public void AlterColumnOperation_type_with_index()
        {
            Generate(
                builder =>
                {
                    ((Model)builder.Model).SetProductVersion("2.1.0");
                    builder.Entity("People", eb =>
                    {
                        eb.Property<string>("Blob");
                        eb.HasIndex("Blob");
                    });
                },
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Blob",
                    ClrType = typeof(string),
                    ColumnType = "char(127)",
                    OldColumn = new ColumnOperation
                    {
                        ColumnType = "varchar(127)",
                    },
                    IsNullable = true
                });

            Assert.Equal(
                "ALTER TABLE `People` MODIFY COLUMN `Blob` char(127) NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public void AlterColumnOperation_ComputedColumnSql_with_index()
        {
            Generate(
                builder =>
                {
                    ((Model)builder.Model).SetProductVersion("2.1.0");
                    builder.Entity("People", eb =>
                    {
                        eb.Property<string>("Blob");
                        eb.HasIndex("Blob");
                    });
                },
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Blob",
                    ClrType = typeof(string),
                    ComputedColumnSql = "'TEST'",
                    ColumnType = "varchar(95)"
                });

            Assert.Equal(
                "ALTER TABLE `People` MODIFY COLUMN `Blob` varchar(95) AS ('TEST');" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddForeignKeyOperation_with_name()
        {
            base.AddForeignKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD CONSTRAINT `FK_People_Companies` FOREIGN KEY (`EmployerId1`, `EmployerId2`) REFERENCES `Companies` (`Id1`, `Id2`) ON DELETE CASCADE;" +
                EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void AddForeignKeyOperation_with_long_name()
        {
            Generate(
                new AddForeignKeyOperation
                {
                    Table = "People",
                    Name = "FK_ASuperLongForeignKeyNameThatIsDefinetelyNotGoingToFitInThe64CharactersLimit",
                    Columns = new[] { "EmployerId1", "EmployerId2" },
                    PrincipalTable = "Companies",
                    PrincipalColumns = new[] { "Id1", "Id2" },
                    OnDelete = ReferentialAction.Cascade
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD CONSTRAINT `FK_ASuperLongForeignKeyNameThatIsDefinetelyNotGoingToFitInThe64C` FOREIGN KEY (`EmployerId1`, `EmployerId2`) REFERENCES `Companies` (`Id1`, `Id2`) ON DELETE CASCADE;" +
                EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddForeignKeyOperation_without_name()
        {
            base.AddForeignKeyOperation_without_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD FOREIGN KEY (`SpouseId`) REFERENCES `People` (`Id`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddPrimaryKeyOperation_with_name()
        {
            base.AddPrimaryKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD CONSTRAINT `PK_People` PRIMARY KEY (`Id1`, `Id2`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddPrimaryKeyOperation_without_name()
        {
            base.AddPrimaryKeyOperation_without_name();

            var test =
                "ALTER TABLE `People` ADD PRIMARY KEY (`Id`);" + EOL +
                @"CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'People', 'Id');".Replace("\r", string.Empty).Replace("\n", EOL) + EOL;

            Assert.Equal(test,
                Sql);
        }

        [ConditionalFact]
        public override void AddUniqueConstraintOperation_with_name()
        {
            base.AddUniqueConstraintOperation_with_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD CONSTRAINT `AK_People_DriverLicense` UNIQUE (`DriverLicense_State`, `DriverLicense_Number`);" +
                EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddUniqueConstraintOperation_without_name()
        {
            base.AddUniqueConstraintOperation_without_name();

            Assert.Equal(
                "ALTER TABLE `People` ADD UNIQUE (`SSN`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void CreateIndexOperation_unique()
        {
            base.CreateIndexOperation_unique();

            Assert.Equal(
                "CREATE UNIQUE INDEX `IX_People_Name` ON `People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void CreateIndexOperation_fulltext()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Columns = new[] { "FirstName", "LastName" },
                    [MySqlAnnotationNames.FullTextIndex] = true
                });

            Assert.Equal(
                "CREATE FULLTEXT INDEX `IX_People_Name` ON `People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void CreateIndexOperation_spatial()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Columns = new[] { "FirstName", "LastName" },
                    [MySqlAnnotationNames.SpatialIndex] = true
                });

            Assert.Equal(
                "CREATE SPATIAL INDEX `IX_People_Name` ON `People` (`FirstName`, `LastName`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void CreateIndexOperation_nonunique()
        {
            base.CreateIndexOperation_nonunique();

            Assert.Equal(
                "CREATE INDEX `IX_People_Name` ON `People` (`Name`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void CreateIndexOperation_with_long_name()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_ASuperLongForeignKeyNameThatIsDefinetelyNotGoingToFitInThe64CharactersLimit",
                    Table = "People",
                    Columns = new[] { "Name" },
                    IsUnique = false
                });

            Assert.Equal(
                "CREATE INDEX `IX_ASuperLongForeignKeyNameThatIsDefinetelyNotGoingToFitInThe64C` ON `People` (`Name`);" + EOL,
                Sql);
        }

        [ConditionalFact]
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

        [ConditionalFact]
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

        [ConditionalFact]
        public virtual void RenameColumnOperation()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.RenameColumn(
                table: "Person",
                name: "Name",
                newName: "FullName");

            Generate(migrationBuilder.Operations.ToArray());

            Assert.Equal(
                "ALTER TABLE `Person` RENAME COLUMN `Name` TO `FullName`;" + EOL,
                Sql);
        }

        [ConditionalFact]
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
                    x => { x.Property<string>("FullName"); }),
                migrationBuilder.Operations.ToArray());

            Assert.Equal(
                "ALTER TABLE `Person` RENAME COLUMN `Name` TO `FullName`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DropColumnOperation()
        {
            base.DropColumnOperation();

            Assert.Equal(
                "ALTER TABLE `People` DROP COLUMN `LuckyNumber`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DropForeignKeyOperation()
        {
            base.DropForeignKeyOperation();

            Assert.Equal(
                "ALTER TABLE `People` DROP FOREIGN KEY `FK_People_Companies`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DropPrimaryKeyOperation()
        {
            base.DropPrimaryKeyOperation();

            Assert.Equal(
                @"CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'People');
ALTER TABLE `People` DROP PRIMARY KEY;".Replace("\r", string.Empty).Replace("\n", EOL) + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DropTableOperation()
        {
            base.DropTableOperation();

            Assert.Equal(
                "DROP TABLE `People`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DropUniqueConstraintOperation()
        {
            base.DropUniqueConstraintOperation();

            Assert.Equal(
                "ALTER TABLE `People` DROP KEY `AK_People_SSN`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DropIndexOperation()
        {
            base.DropIndexOperation();

            Assert.Equal(
                @"ALTER TABLE `People` DROP INDEX `IX_People_Name`;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void SqlOperation()
        {
            base.SqlOperation();

            Assert.Equal(
                "-- I <3 DDL" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void AddColumnOperation_with_charSet_annotation()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "Name",
                    ClrType = typeof(string),
                    ColumnType = "varchar(255)",
                    IsNullable = true,
                    [MySqlAnnotationNames.CharSet] = CharSet.SJis,
                });

            Assert.Equal(
                "ALTER TABLE `People` ADD `Name` varchar(255) CHARACTER SET sjis NULL;" +
                EOL,
                Sql);
        }

        // MySql doesn't support sequence
        [ConditionalFact]
        public override void AlterSequenceOperation_with_minValue_and_maxValue()
        {
        }

        [ConditionalFact]
        public override void AlterSequenceOperation_without_minValue_and_maxValue()
        {
        }

        [ConditionalFact]
        public override void CreateSequenceOperation_with_minValue_and_maxValue()
        {
        }

        [ConditionalFact]
        public override void CreateSequenceOperation_with_minValue_and_maxValue_not_long()
        {
        }

        [ConditionalFact]
        public override void CreateSequenceOperation_without_minValue_and_maxValue()
        {
        }

        [ConditionalFact]
        public override void DropSequenceOperation()
        {
        }

        public MigrationSqlGeneratorMySqlTest()
            : base(MySqlTestHelpers.Instance)
        {
        }

        protected override void Generate(params MigrationOperation[] operations)
            => base.Generate(ResetSchema(operations));

        protected override void Generate(Action<ModelBuilder> buildAction, params MigrationOperation[] operations)
        {
            var services = MySqlTestHelpers.Instance.CreateContextServices(ServerVersion.Default);
            var modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder(services);
            buildAction(modelBuilder);

            var batch = services.GetRequiredService<IMigrationsSqlGenerator>()
                .Generate(ResetSchema(operations), modelBuilder.Model);

            Sql = string.Join(
                EOL,
                batch.Select(b => b.CommandText));
        }

        protected virtual void Generate(Action<ModelBuilder> buildAction, CharSetBehavior charSetBehavior, CharSet charSet, params MigrationOperation[] operations)
        {
            var services = MySqlTestHelpers.Instance.CreateContextServices(charSetBehavior, charSet);
            var modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder(services);
            buildAction(modelBuilder);

            var batch = services.GetRequiredService<IMigrationsSqlGenerator>()
                .Generate(ResetSchema(operations), modelBuilder.Model);

            Sql = string.Join(
                EOL,
                batch.Select(b => b.CommandText));
        }

        /// <summary>
        /// The base class does set schema values, while MySQL does not support
        /// the EF Core concept of schemas.
        /// </summary>
        protected virtual MigrationOperation[] ResetSchema(params MigrationOperation[] operations)
        {
            foreach (var operation in operations)
            {
                var schemaPropertyInfos = operation
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                    .Where(p => p.Name.Contains(nameof(AddForeignKeyOperation.Schema), StringComparison.Ordinal));

                foreach (var schemaPropertyInfo in schemaPropertyInfos)
                {
                    schemaPropertyInfo.SetValue(operation, null);
                }
            }

            return operations;
        }
    }
}
