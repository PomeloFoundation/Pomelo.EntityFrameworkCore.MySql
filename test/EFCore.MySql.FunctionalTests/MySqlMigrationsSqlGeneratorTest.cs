using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public partial class MySqlMigrationsSqlGeneratorTest : MigrationsSqlGeneratorTestBase
    {
        public MySqlMigrationsSqlGeneratorTest()
            : base(
                MySqlTestHelpers.Instance,
                new ServiceCollection().AddEntityFrameworkMySqlNetTopologySuite(),
                MySqlTestHelpers.Instance.AddProviderOptions(
                    ((IRelationalDbContextOptionsBuilderInfrastructure)
                        new MySqlDbContextOptionsBuilder(new DbContextOptionsBuilder()).UseNetTopologySuite())
                    .OptionsBuilder).Options)
        {
            //MigrationsSqlGeneratorTestBase
        }

        protected override string Schema { get; } = null;

        public override void AddColumnOperation_with_unicode_overridden()
        {
            base.AddColumnOperation_with_unicode_overridden();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` longtext CHARACTER SET utf8mb4 NULL;");
        }

        public override void AddColumnOperation_with_unicode_no_model()
        {
            base.AddColumnOperation_with_unicode_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` longtext CHARACTER SET utf8mb4 NULL;");
        }

        public override void AddColumnOperation_with_fixed_length_no_model()
        {
            base.AddColumnOperation_with_fixed_length_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` char(100) CHARACTER SET utf8mb4 NULL;");
        }

        public override void AddColumnOperation_with_maxLength_no_model()
        {
            base.AddColumnOperation_with_maxLength_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` varchar(30) CHARACTER SET utf8mb4 NULL;");
        }

        public override void AddColumnOperation_with_precision_and_scale_overridden()
        {
            base.AddColumnOperation_with_precision_and_scale_overridden();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Pi` decimal(15,10) NOT NULL;");
        }

        public override void AddColumnOperation_with_precision_and_scale_no_model()
        {
            base.AddColumnOperation_with_precision_and_scale_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Pi` decimal(20,7) NOT NULL;");
        }

        public override void AddForeignKeyOperation_without_principal_columns()
        {
            base.AddForeignKeyOperation_without_principal_columns();

            AssertSql(
                @"ALTER TABLE `People` ADD FOREIGN KEY (`SpouseId`) REFERENCES `People`;");
        }

        public override void AlterColumnOperation_without_column_type()
        {
            base.AlterColumnOperation_without_column_type();

            AssertSql(
                @"ALTER TABLE `People` MODIFY COLUMN `LuckyNumber` int NOT NULL;");
        }

        public override void RenameTableOperation_legacy()
        {
            base.RenameTableOperation_legacy();

            AssertSql(
                @"ALTER TABLE `People` RENAME `Person`;");
        }

        public override void RenameTableOperation()
        {
            base.RenameTableOperation();

            AssertSql(
                @"ALTER TABLE `People` RENAME `Person`;");
        }

        public override void InsertDataOperation_all_args_spatial()
        {
            base.InsertDataOperation_all_args_spatial();

            AssertSql(
                @"INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (0, NULL, NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (1, 'Daenerys Targaryen', NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (2, 'John Snow', NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (3, 'Arya Stark', NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (4, 'Harry Strickland', NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (5, 'The Imp', NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (6, 'The Kingslayer', NULL);
INSERT INTO `People` (`Id`, `Full Name`, `Geometry`)
VALUES (7, 'Aemon Targaryen', X'E61000000107000000080000000102000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F6666666666661C40CDCCCCCCCCCC1C400102000000040000006666666666661C40CDCCCCCCCCCC1C403333333333333440333333333333344033333333333334409A9999999999F13F6666666666865140CDCCCCCCCC8C514001040000000300000001010000009A9999999999F13F9A9999999999014001010000009A999999999901409A9999999999014001010000009A999999999901409A9999999999F13F010300000001000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F9A9999999999F13F9A99999999990140010300000001000000040000003333333333332440333333333333344033333333333334403333333333333440333333333333344033333333333324403333333333332440333333333333344001010000009A9999999999F13F9A999999999901400105000000020000000102000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F6666666666661C40CDCCCCCCCCCC1C400102000000040000006666666666661C40CDCCCCCCCCCC1C403333333333333440333333333333344033333333333334409A9999999999F13F6666666666865140CDCCCCCCCC8C51400106000000020000000103000000010000000400000033333333333324403333333333333440333333333333344033333333333334403333333333333440333333333333244033333333333324403333333333333440010300000001000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F9A9999999999F13F9A99999999990140');");
        }

        public override void InsertDataOperation_required_args()
        {
            base.InsertDataOperation_required_args();

            AssertSql(
                @"INSERT INTO `People` (`First Name`)
VALUES ('John');");
        }

        public override void InsertDataOperation_required_args_composite()
        {
            base.InsertDataOperation_required_args_composite();

            AssertSql(
                @"INSERT INTO `People` (`First Name`, `Last Name`)
VALUES ('John', 'Snow');");
        }

        public override void InsertDataOperation_required_args_multiple_rows()
        {
            base.InsertDataOperation_required_args_multiple_rows();

            AssertSql(
                @"INSERT INTO `People` (`First Name`)
VALUES ('John');
INSERT INTO `People` (`First Name`)
VALUES ('Daenerys');");
        }

        public override void InsertDataOperation_throws_for_unsupported_column_types()
        {
            base.InsertDataOperation_throws_for_unsupported_column_types();
        }

        public override void DeleteDataOperation_all_args()
        {
            base.DeleteDataOperation_all_args();

            AssertSql(
                @"DELETE FROM `People`
WHERE `First Name` = 'Hodor';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'Daenerys';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'John';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'Arya';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'Harry';
SELECT ROW_COUNT();");
        }

        public override void DeleteDataOperation_all_args_composite()
        {
            base.DeleteDataOperation_all_args_composite();

            AssertSql(
                @"DELETE FROM `People`
WHERE `First Name` = 'Hodor' AND `Last Name` IS NULL;
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'Daenerys' AND `Last Name` = 'Targaryen';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'John' AND `Last Name` = 'Snow';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'Arya' AND `Last Name` = 'Stark';
SELECT ROW_COUNT();

DELETE FROM `People`
WHERE `First Name` = 'Harry' AND `Last Name` = 'Strickland';
SELECT ROW_COUNT();");
        }

        public override void DeleteDataOperation_required_args()
        {
            base.DeleteDataOperation_required_args();

            AssertSql(
                @"DELETE FROM `People`
WHERE `Last Name` = 'Snow';
SELECT ROW_COUNT();");
        }

        public override void DeleteDataOperation_required_args_composite()
        {
            base.DeleteDataOperation_required_args_composite();

            AssertSql(
                @"DELETE FROM `People`
WHERE `First Name` = 'John' AND `Last Name` = 'Snow';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_all_args()
        {
            base.UpdateDataOperation_all_args();

            AssertSql(
                @"UPDATE `People` SET `Birthplace` = 'Winterfell', `House Allegiance` = 'Stark', `Culture` = 'Northmen'
WHERE `First Name` = 'Hodor';
SELECT ROW_COUNT();

UPDATE `People` SET `Birthplace` = 'Dragonstone', `House Allegiance` = 'Targaryen', `Culture` = 'Valyrian'
WHERE `First Name` = 'Daenerys';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_all_args_composite()
        {
            base.UpdateDataOperation_all_args_composite();

            AssertSql(
                @"UPDATE `People` SET `House Allegiance` = 'Stark'
WHERE `First Name` = 'Hodor' AND `Last Name` IS NULL;
SELECT ROW_COUNT();

UPDATE `People` SET `House Allegiance` = 'Targaryen'
WHERE `First Name` = 'Daenerys' AND `Last Name` = 'Targaryen';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_all_args_composite_multi()
        {
            base.UpdateDataOperation_all_args_composite_multi();

            AssertSql(
                @"UPDATE `People` SET `Birthplace` = 'Winterfell', `House Allegiance` = 'Stark', `Culture` = 'Northmen'
WHERE `First Name` = 'Hodor' AND `Last Name` IS NULL;
SELECT ROW_COUNT();

UPDATE `People` SET `Birthplace` = 'Dragonstone', `House Allegiance` = 'Targaryen', `Culture` = 'Valyrian'
WHERE `First Name` = 'Daenerys' AND `Last Name` = 'Targaryen';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_all_args_multi()
        {
            base.UpdateDataOperation_all_args_multi();

            AssertSql(
                @"UPDATE `People` SET `Birthplace` = 'Dragonstone', `House Allegiance` = 'Targaryen', `Culture` = 'Valyrian'
WHERE `First Name` = 'Daenerys';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_required_args()
        {
            base.UpdateDataOperation_required_args();

            AssertSql(
                @"UPDATE `People` SET `House Allegiance` = 'Targaryen'
WHERE `First Name` = 'Daenerys';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_required_args_multiple_rows()
        {
            base.UpdateDataOperation_required_args_multiple_rows();

            AssertSql(
                @"UPDATE `People` SET `House Allegiance` = 'Stark'
WHERE `First Name` = 'Hodor';
SELECT ROW_COUNT();

UPDATE `People` SET `House Allegiance` = 'Targaryen'
WHERE `First Name` = 'Daenerys';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_required_args_composite()
        {
            base.UpdateDataOperation_required_args_composite();

            AssertSql(
                @"UPDATE `People` SET `House Allegiance` = 'Targaryen'
WHERE `First Name` = 'Daenerys' AND `Last Name` = 'Targaryen';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_required_args_composite_multi()
        {
            base.UpdateDataOperation_required_args_composite_multi();

            AssertSql(
                @"UPDATE `People` SET `Birthplace` = 'Dragonstone', `House Allegiance` = 'Targaryen', `Culture` = 'Valyrian'
WHERE `First Name` = 'Daenerys' AND `Last Name` = 'Targaryen';
SELECT ROW_COUNT();");
        }

        public override void UpdateDataOperation_required_args_multi()
        {
            base.UpdateDataOperation_required_args_multi();

            AssertSql(
                @"UPDATE `People` SET `Birthplace` = 'Dragonstone', `House Allegiance` = 'Targaryen', `Culture` = 'Valyrian'
WHERE `First Name` = 'Daenerys';
SELECT ROW_COUNT();");
        }

        public override void DefaultValue_with_line_breaks(bool isUnicode)
        {
            base.DefaultValue_with_line_breaks(isUnicode);

            AssertSql(
                @"CREATE TABLE `TestLineBreaks` (
    `TestDefaultValue` longtext CHARACTER SET utf8mb4 NOT NULL
);");
        }

        [ConditionalFact]
        public virtual void DefaultValue_not_generated_for_text_column()
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
    `Event` TEXT NOT NULL
);
",
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
                            ColumnType = "VARCHAR(255)",
                            DefaultValue = new DateTime(2015, 4, 12, 17, 5, 0)
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `History` (
    `Event` VARCHAR(255) NOT NULL DEFAULT '2015-04-12 17:05:00'
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
                new AddColumnOperation
                {
                    Table = "Person",
                    Name = "Name",
                    ClrType = typeof(string),
                    IsUnicode = isUnicode,
                    IsNullable = true
                },
                charSetBehavior,
                charSet);

            var columnType = "longtext";
            if (isIndex)
            {
                var columnSize = Math.Min(AppConfig.ServerVersion.MaxKeyLength / (charSet.MaxBytesPerChar * 2), 255);
                columnType = $"varchar({columnSize})";
            }

            Assert.Equal(
                $"ALTER TABLE `Person` ADD `Name` {columnType}{expectedCharSetName} NULL;" + EOL,
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
                new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).ToString("yyyy-MM-dd HH:mm:ss.FFFFFF") +
                "';" + EOL,
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
                    OldColumn = new AddColumnOperation
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
                    OldColumn = new AddColumnOperation
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
                    OldColumn = new AddColumnOperation
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
        public void AlterColumnOperation_ComputedColumnSql_stored()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "Universes",
                    Name = "AnswerToEverything",
                    ClrType = typeof(int),
                    ColumnType = "int",
                    ComputedColumnSql = "6 * 9",
                    IsStored = true,
                });

            Assert.Equal(
                "ALTER TABLE `Universes` ADD `AnswerToEverything` int AS (6 * 9) STORED;" + EOL,
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
            // TODO: Use meaningful column names.
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
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.RenameIndex))]
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
                AppConfig.ServerVersion.Supports.RenameIndex
                ? @"ALTER TABLE `Person` RENAME INDEX `IX_Person_Name` TO `IX_Person_FullName`;" + EOL
                : @"ALTER TABLE `Person` DROP INDEX `IX_Person_Name`;" + EOL + EOL + "CREATE UNIQUE INDEX `IX_Person_FullName` ON `Person` (`FullName`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.RenameColumn))]
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
                AppConfig.ServerVersion.Supports.RenameColumn
                    ? "ALTER TABLE `Person` RENAME COLUMN `Name` TO `FullName`;" + EOL
                    : "ALTER TABLE `Person` CHANGE `Name` `FullName` longtext CHARACTER SET utf8mb4 NULL;" + EOL,
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

        protected override string GetGeometryCollectionStoreType()
            => "geometrycollection";

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

        [ConditionalFact]
        public virtual void CreateIndexOperation_with_prefix_lengths()
        {
            Generate(
                builder => builder.Entity(
                    "IceCreams",
                    entity =>
                    {
                        entity.Property<int>("IceCreamId");
                        entity.Property<string>("Name")
                            .HasMaxLength(255);
                        entity.Property<string>("Brand");

                        entity.HasKey("IceCreamId");
                    }),
                new CreateIndexOperation
                {
                    Name = "IX_IceCreams_Brand_Name",
                    Table = "IceCreams",
                    Columns = new[] { "Name", "Brand" },
                    [MySqlAnnotationNames.IndexPrefixLength] = new [] { 0, 20 }
                });

            Assert.Equal(
                @"CREATE INDEX `IX_IceCreams_Brand_Name` ON `IceCreams` (`Name`, `Brand`(20));" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void CreateTableOperation_primary_key_with_prefix_lengths()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "IceCreams",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Brand",
                            ColumnType = "longtext",
                            ClrType = typeof(string),
                        },
                        new AddColumnOperation
                        {
                            Name = "Name",
                            ColumnType = "varchar(255)",
                            ClrType = typeof(string),
                        },
                    },
                    PrimaryKey = new AddPrimaryKeyOperation
                    {
                        Columns = new[] { "Name", "Brand" },
                        [MySqlAnnotationNames.IndexPrefixLength] = new [] { 0, 20 }
                    },
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreams` (
    `Brand` longtext NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`Name`, `Brand`(20))
);" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public virtual void AlterSequenceOperation_with_minValue_and_maxValue()
        {
            Generate(
                new AlterSequenceOperation {
                    Name = "MySequence",
                    Schema = Schema,
                    IncrementBy=1,
                    IsCyclic=false,
                    MinValue = 10,
                    MaxValue = 20
                });

            Assert.Equal(
               @"ALTER SEQUENCE `MySequence` INCREMENT BY 1 MINVALUE 10 MAXVALUE 20 NOCYCLE;" + EOL,
               Sql,
               ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public virtual void AlterSequenceOperation_without_minValue_and_maxValue()
        {
            Generate(
                new AlterSequenceOperation
                {
                    Name = "MySequence",
                    Schema = Schema,
                    IncrementBy = 1,
                    IsCyclic = false
                });

            Assert.Equal(
               @"ALTER SEQUENCE `MySequence` INCREMENT BY 1 NO MINVALUE NO MAXVALUE NOCYCLE;" + EOL,
               Sql,
               ignoreLineEndingDifferences: true);

        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public virtual void CreateSequenceOperation_with_minValue_and_maxValue()
        {
            Generate(
              new CreateSequenceOperation
              {
                  Name = "MySequence",
                  Schema = Schema,
                  IncrementBy = 1,
                  IsCyclic = false,
                  StartValue=10,
                  MinValue = 10,
                  MaxValue = 20,

              });

            Assert.Equal(
               @"CREATE SEQUENCE `MySequence` START WITH 10 INCREMENT BY 1 MINVALUE 10 MAXVALUE 20 NOCYCLE;" + EOL,
               Sql,
               ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public virtual void CreateSequenceOperation_without_minValue_and_maxValue()
        {

            Generate(
              new CreateSequenceOperation
              {
                  Name = "MySequence",
                  Schema = Schema,
                  IncrementBy = 1,
                  IsCyclic = false
              });

            Assert.Equal(
               @"CREATE SEQUENCE `MySequence` START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NOCYCLE;" + EOL,
               Sql,
               ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public virtual void DropSequenceOperation()
        {
             Generate(
             new DropSequenceOperation
             {
                 Name = "MySequence",
                 Schema = Schema
             });

            Assert.Equal(
               @"DROP SEQUENCE `MySequence`;" + EOL,
               Sql,
               ignoreLineEndingDifferences: true);
        }

        protected new void AssertSql(string expected)
        {
            var testSqlLoggerFactory = new TestSqlLoggerFactory();
            var logger = testSqlLoggerFactory.CreateLogger(nameof(MySqlMigrationsSqlGeneratorTest));
            logger.Log(
                LogLevel.Information,
                RelationalEventId.CommandExecuted.Id,
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("commandText", Sql.Trim()),
                    new KeyValuePair<string, object>("parameters", string.Empty)
                }.AsReadOnly(),
                null,
                (pairs, exception) => (string) pairs.First(kvp => kvp.Key == "commandText").Value);
            testSqlLoggerFactory.AssertBaseline(new[] {expected});
        }

        protected override void Generate(params MigrationOperation[] operations)
            => base.Generate(operations);

        private void Generate(
            Action<ModelBuilder> buildAction,
            MigrationOperation operation,
            CharSetBehavior charSetBehavior,
            CharSet charSet)
            => Generate(buildAction, new[] {operation}, default, charSetBehavior, charSet);

        protected override void Generate(
            Action<ModelBuilder> buildAction,
            MigrationOperation[] operation,
            MigrationsSqlGenerationOptions options)
            => Generate(buildAction, operation, options, null, null);

        protected virtual void Generate(
            Action<ModelBuilder> buildAction,
            MigrationOperation[] operation,
            MigrationsSqlGenerationOptions options,
            CharSetBehavior? charSetBehavior,
            [CanBeNull] CharSet charSet,
            MySqlSchemaNameTranslator schemaNameTranslator = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder(ContextOptions);
            var mySqlOptionsBuilder = new MySqlDbContextOptionsBuilder(optionsBuilder);

            if (charSetBehavior != null)
            {
                mySqlOptionsBuilder.CharSetBehavior(charSetBehavior.Value);
            }

            if (charSet != null)
            {
                mySqlOptionsBuilder.CharSet(charSet);
            }

            if (schemaNameTranslator != null)
            {
                mySqlOptionsBuilder.SchemaBehavior(MySqlSchemaBehavior.Translate, schemaNameTranslator);
            }

            var contextOptions = optionsBuilder.Options;

            var services = ContextOptions != null
                ? TestHelpers.CreateContextServices(CustomServices, contextOptions)
                : TestHelpers.CreateContextServices(CustomServices);

            IModel model = null;
            if (buildAction != null)
            {
                var modelBuilder = TestHelpers.CreateConventionBuilder();
                modelBuilder.Model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);
                buildAction(modelBuilder);

                model = modelBuilder.Model;
                var conventionSet = services.GetRequiredService<IConventionSetBuilder>().CreateConventionSet();

                var typeMappingConvention = conventionSet.ModelFinalizingConventions.OfType<TypeMappingConvention>().FirstOrDefault();
                typeMappingConvention.ProcessModelFinalizing(((IConventionModel)model).Builder, null);

                var relationalModelConvention = conventionSet.ModelFinalizedConventions.OfType<RelationalModelConvention>().First();
                model = relationalModelConvention.ProcessModelFinalized((IConventionModel)model);
            }

            var batch = services.GetRequiredService<IMigrationsSqlGenerator>().Generate(operation, model, options);

            Sql = string.Join(
                EOL,
                batch.Select(b => b.CommandText));
        }
    }
}
