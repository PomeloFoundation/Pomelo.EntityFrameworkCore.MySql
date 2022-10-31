using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Internal;
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
        }

        protected /*override*/ virtual string Schema { get; } = null;

        public override void AddColumnOperation_with_unicode_overridden()
        {
            base.AddColumnOperation_with_unicode_overridden();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` longtext NULL;");
        }

        public override void AddColumnOperation_with_unicode_no_model()
        {
            base.AddColumnOperation_with_unicode_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` longtext NULL;");
        }

        public override void AddColumnOperation_with_fixed_length_no_model()
        {
            base.AddColumnOperation_with_fixed_length_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` char(100) NULL;");
        }

        public override void AddColumnOperation_with_maxLength_no_model()
        {
            base.AddColumnOperation_with_maxLength_no_model();

            AssertSql(
                @"ALTER TABLE `Person` ADD `Name` varchar(30) NULL;");
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
VALUES (0, NULL, NULL),
(1, 'Daenerys Targaryen', NULL),
(2, 'John Snow', NULL),
(3, 'Arya Stark', NULL),
(4, 'Harry Strickland', NULL),
(5, 'The Imp', NULL),
(6, 'The Kingslayer', NULL),
(7, 'Aemon Targaryen', X'E61000000107000000080000000102000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F6666666666661C40CDCCCCCCCCCC1C400102000000040000006666666666661C40CDCCCCCCCCCC1C403333333333333440333333333333344033333333333334409A9999999999F13F6666666666865140CDCCCCCCCC8C514001040000000300000001010000009A9999999999F13F9A9999999999014001010000009A999999999901409A9999999999014001010000009A999999999901409A9999999999F13F010300000001000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F9A9999999999F13F9A99999999990140010300000001000000040000003333333333332440333333333333344033333333333334403333333333333440333333333333344033333333333324403333333333332440333333333333344001010000009A9999999999F13F9A999999999901400105000000020000000102000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F6666666666661C40CDCCCCCCCCCC1C400102000000040000006666666666661C40CDCCCCCCCCCC1C403333333333333440333333333333344033333333333334409A9999999999F13F6666666666865140CDCCCCCCCC8C51400106000000020000000103000000010000000400000033333333333324403333333333333440333333333333344033333333333334403333333333333440333333333333244033333333333324403333333333333440010300000001000000040000009A9999999999F13F9A999999999901409A999999999901409A999999999901409A999999999901409A9999999999F13F9A9999999999F13F9A99999999990140');");
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
VALUES ('John'),
('Daenerys');");
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

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public override void DefaultValue_with_line_breaks(bool isUnicode)
        {
            base.DefaultValue_with_line_breaks(isUnicode);

            AssertSql(
                @"CREATE TABLE `TestLineBreaks` (
    `TestDefaultValue` longtext NOT NULL DEFAULT (CONCAT('', CHAR(13, 10), 'Various Line', CHAR(13), 'Breaks', CHAR(10), ''))
);");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public override void DefaultValue_with_line_breaks_2(bool isUnicode)
        {
            base.DefaultValue_with_line_breaks_2(isUnicode);

        AssertSql(
            @"CREATE TABLE `TestLineBreaks` (
    `TestDefaultValue` longtext NOT NULL DEFAULT (CONCAT('0', CHAR(13, 10), '1', CHAR(13, 10), '2', CHAR(13, 10), '3', CHAR(13, 10), '4', CHAR(13, 10), '5', CHAR(13, 10), '6', CHAR(13, 10), '7', CHAR(13, 10), '8', CHAR(13, 10), '9', CHAR(13, 10), '10', CHAR(13, 10), '11', CHAR(13, 10), '12', CHAR(13, 10), '13', CHAR(13, 10), '14', CHAR(13, 10), '15', CHAR(13, 10), '16', CHAR(13, 10), '17', CHAR(13, 10), '18', CHAR(13, 10), '19', CHAR(13, 10), '20', CHAR(13, 10), '21', CHAR(13, 10), '22', CHAR(13, 10), '23', CHAR(13, 10), '24', CHAR(13, 10), '25', CHAR(13, 10), '26', CHAR(13, 10), '27', CHAR(13, 10), '28', CHAR(13, 10), '29', CHAR(13, 10), '30', CHAR(13, 10), '31', CHAR(13, 10), '32', CHAR(13, 10), '33', CHAR(13, 10), '34', CHAR(13, 10), '35', CHAR(13, 10), '36', CHAR(13, 10), '37', CHAR(13, 10), '38', CHAR(13, 10), '39', CHAR(13, 10), '40', CHAR(13, 10), '41', CHAR(13, 10), '42', CHAR(13, 10), '43', CHAR(13, 10), '44', CHAR(13, 10), '45', CHAR(13, 10), '46', CHAR(13, 10), '47', CHAR(13, 10), '48', CHAR(13, 10), '49', CHAR(13, 10), '50', CHAR(13, 10), '51', CHAR(13, 10), '52', CHAR(13, 10), '53', CHAR(13, 10), '54', CHAR(13, 10), '55', CHAR(13, 10), '56', CHAR(13, 10), '57', CHAR(13, 10), '58', CHAR(13, 10), '59', CHAR(13, 10), '60', CHAR(13, 10), '61', CHAR(13, 10), '62', CHAR(13, 10), '63', CHAR(13, 10), '64', CHAR(13, 10), '65', CHAR(13, 10), '66', CHAR(13, 10), '67', CHAR(13, 10), '68', CHAR(13, 10), '69', CHAR(13, 10), '70', CHAR(13, 10), '71', CHAR(13, 10), '72', CHAR(13, 10), '73', CHAR(13, 10), '74', CHAR(13, 10), '75', CHAR(13, 10), '76', CHAR(13, 10), '77', CHAR(13, 10), '78', CHAR(13, 10), '79', CHAR(13, 10), '80', CHAR(13, 10), '81', CHAR(13, 10), '82', CHAR(13, 10), '83', CHAR(13, 10), '84', CHAR(13, 10), '85', CHAR(13, 10), '86', CHAR(13, 10), '87', CHAR(13, 10), '88', CHAR(13, 10), '89', CHAR(13, 10), '90', CHAR(13, 10), '91', CHAR(13, 10), '92', CHAR(13, 10), '93', CHAR(13, 10), '94', CHAR(13, 10), '95', CHAR(13, 10), '96', CHAR(13, 10), '97', CHAR(13, 10), '98', CHAR(13, 10), '99', CHAR(13, 10), '100', CHAR(13, 10), '101', CHAR(13, 10), '102', CHAR(13, 10), '103', CHAR(13, 10), '104', CHAR(13, 10), '105', CHAR(13, 10), '106', CHAR(13, 10), '107', CHAR(13, 10), '108', CHAR(13, 10), '109', CHAR(13, 10), '110', CHAR(13, 10), '111', CHAR(13, 10), '112', CHAR(13, 10), '113', CHAR(13, 10), '114', CHAR(13, 10), '115', CHAR(13, 10), '116', CHAR(13, 10), '117', CHAR(13, 10), '118', CHAR(13, 10), '119', CHAR(13, 10), '120', CHAR(13, 10), '121', CHAR(13, 10), '122', CHAR(13, 10), '123', CHAR(13, 10), '124', CHAR(13, 10), '125', CHAR(13, 10), '126', CHAR(13, 10), '127', CHAR(13, 10), '128', CHAR(13, 10), '129', CHAR(13, 10), '130', CHAR(13, 10), '131', CHAR(13, 10), '132', CHAR(13, 10), '133', CHAR(13, 10), '134', CHAR(13, 10), '135', CHAR(13, 10), '136', CHAR(13, 10), '137', CHAR(13, 10), '138', CHAR(13, 10), '139', CHAR(13, 10), '140', CHAR(13, 10), '141', CHAR(13, 10), '142', CHAR(13, 10), '143', CHAR(13, 10), '144', CHAR(13, 10), '145', CHAR(13, 10), '146', CHAR(13, 10), '147', CHAR(13, 10), '148', CHAR(13, 10), '149', CHAR(13, 10), '150', CHAR(13, 10), '151', CHAR(13, 10), '152', CHAR(13, 10), '153', CHAR(13, 10), '154', CHAR(13, 10), '155', CHAR(13, 10), '156', CHAR(13, 10), '157', CHAR(13, 10), '158', CHAR(13, 10), '159', CHAR(13, 10), '160', CHAR(13, 10), '161', CHAR(13, 10), '162', CHAR(13, 10), '163', CHAR(13, 10), '164', CHAR(13, 10), '165', CHAR(13, 10), '166', CHAR(13, 10), '167', CHAR(13, 10), '168', CHAR(13, 10), '169', CHAR(13, 10), '170', CHAR(13, 10), '171', CHAR(13, 10), '172', CHAR(13, 10), '173', CHAR(13, 10), '174', CHAR(13, 10), '175', CHAR(13, 10), '176', CHAR(13, 10), '177', CHAR(13, 10), '178', CHAR(13, 10), '179', CHAR(13, 10), '180', CHAR(13, 10), '181', CHAR(13, 10), '182', CHAR(13, 10), '183', CHAR(13, 10), '184', CHAR(13, 10), '185', CHAR(13, 10), '186', CHAR(13, 10), '187', CHAR(13, 10), '188', CHAR(13, 10), '189', CHAR(13, 10), '190', CHAR(13, 10), '191', CHAR(13, 10), '192', CHAR(13, 10), '193', CHAR(13, 10), '194', CHAR(13, 10), '195', CHAR(13, 10), '196', CHAR(13, 10), '197', CHAR(13, 10), '198', CHAR(13, 10), '199', CHAR(13, 10), '200', CHAR(13, 10), '201', CHAR(13, 10), '202', CHAR(13, 10), '203', CHAR(13, 10), '204', CHAR(13, 10), '205', CHAR(13, 10), '206', CHAR(13, 10), '207', CHAR(13, 10), '208', CHAR(13, 10), '209', CHAR(13, 10), '210', CHAR(13, 10), '211', CHAR(13, 10), '212', CHAR(13, 10), '213', CHAR(13, 10), '214', CHAR(13, 10), '215', CHAR(13, 10), '216', CHAR(13, 10), '217', CHAR(13, 10), '218', CHAR(13, 10), '219', CHAR(13, 10), '220', CHAR(13, 10), '221', CHAR(13, 10), '222', CHAR(13, 10), '223', CHAR(13, 10), '224', CHAR(13, 10), '225', CHAR(13, 10), '226', CHAR(13, 10), '227', CHAR(13, 10), '228', CHAR(13, 10), '229', CHAR(13, 10), '230', CHAR(13, 10), '231', CHAR(13, 10), '232', CHAR(13, 10), '233', CHAR(13, 10), '234', CHAR(13, 10), '235', CHAR(13, 10), '236', CHAR(13, 10), '237', CHAR(13, 10), '238', CHAR(13, 10), '239', CHAR(13, 10), '240', CHAR(13, 10), '241', CHAR(13, 10), '242', CHAR(13, 10), '243', CHAR(13, 10), '244', CHAR(13, 10), '245', CHAR(13, 10), '246', CHAR(13, 10), '247', CHAR(13, 10), '248', CHAR(13, 10), '249', CHAR(13, 10), '250', CHAR(13, 10), '251', CHAR(13, 10), '252', CHAR(13, 10), '253', CHAR(13, 10), '254', CHAR(13, 10), '255', CHAR(13, 10), '256', CHAR(13, 10), '257', CHAR(13, 10), '258', CHAR(13, 10), '259', CHAR(13, 10), '260', CHAR(13, 10), '261', CHAR(13, 10), '262', CHAR(13, 10), '263', CHAR(13, 10), '264', CHAR(13, 10), '265', CHAR(13, 10), '266', CHAR(13, 10), '267', CHAR(13, 10), '268', CHAR(13, 10), '269', CHAR(13, 10), '270', CHAR(13, 10), '271', CHAR(13, 10), '272', CHAR(13, 10), '273', CHAR(13, 10), '274', CHAR(13, 10), '275', CHAR(13, 10), '276', CHAR(13, 10), '277', CHAR(13, 10), '278', CHAR(13, 10), '279', CHAR(13, 10), '280', CHAR(13, 10), '281', CHAR(13, 10), '282', CHAR(13, 10), '283', CHAR(13, 10), '284', CHAR(13, 10), '285', CHAR(13, 10), '286', CHAR(13, 10), '287', CHAR(13, 10), '288', CHAR(13, 10), '289', CHAR(13, 10), '290', CHAR(13, 10), '291', CHAR(13, 10), '292', CHAR(13, 10), '293', CHAR(13, 10), '294', CHAR(13, 10), '295', CHAR(13, 10), '296', CHAR(13, 10), '297', CHAR(13, 10), '298', CHAR(13, 10), '299', CHAR(13, 10), ''))
);");
        }

        [ConditionalFact]
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public virtual void DefaultValue_not_generated_for_unlimited_text_column_missing_default_expression_support()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "History",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Table = "History",
                            Name = "Event",
                            ClrType = typeof(string),
                            DefaultValue = "The Battle of Waterloo"
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `History` (
    `Event` longtext NOT NULL
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public virtual void DefaultValue_generated_for_unlimited_text_column()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "History",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Table = "History",
                            Name = "Event",
                            ClrType = typeof(string),
                            DefaultValue = "The Battle of Waterloo"
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `History` (
    `Event` longtext NOT NULL DEFAULT ('The Battle of Waterloo')
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void DefaultValue_generated_for_limited_text_column()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "History",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Table = "History",
                            Name = "Event",
                            ClrType = typeof(string),
                            MaxLength = 128,
                            DefaultValue = "The Battle of Waterloo"
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `History` (
    `Event` varchar(128) NOT NULL DEFAULT 'The Battle of Waterloo'
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
        public virtual void CreateDatabaseOperation_with_charset()
        {
            Generate(new MySqlCreateDatabaseOperation { Name = "Northwind", CharSet = "latin1"});

            Assert.Equal(
                @"CREATE DATABASE `Northwind` CHARACTER SET latin1;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void CreateDatabaseOperation_with_collation()
        {
            Generate(new MySqlCreateDatabaseOperation { Name = "Northwind", Collation = "latin1_general_ci"});

            Assert.Equal(
                @"CREATE DATABASE `Northwind` COLLATE latin1_general_ci;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void AlterDatabaseOperation_with_charset()
        {
            Generate(
                new MySqlCreateDatabaseOperation {Name = "Northwind", CharSet = "latin1"},
                new AlterDatabaseOperation {[MySqlAnnotationNames.CharSet] = "utf8mb4"});

            Assert.Equal(
                @"CREATE DATABASE `Northwind` CHARACTER SET latin1;

ALTER DATABASE CHARACTER SET utf8mb4;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void AlterDatabaseOperation_with_collation()
        {
            Generate(
                new MySqlCreateDatabaseOperation {Name = "Northwind", Collation = "latin1_general_ci"},
                new AlterDatabaseOperation {Collation = "latin1_swedish_ci"});

            Assert.Equal(
                @"CREATE DATABASE `Northwind` COLLATE latin1_general_ci;

ALTER DATABASE COLLATE latin1_swedish_ci;" + EOL,
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
        [InlineData(false, false, "Latin1")]
        [InlineData(false, false, null)]
        [InlineData(false, true, "Latin1")]
        [InlineData(false, true, null)]
        [InlineData(null, false, "Latin1")]
        [InlineData(null, false, "Utf8Mb4")]
        [InlineData(null, false, null)]
        [InlineData(null, true, "Latin1")]
        [InlineData(null, true, "Utf8Mb4")]
        [InlineData(null, true, null)]
        [InlineData(true, false, "Latin1")]
        [InlineData(true, false, null)]
        [InlineData(true, true, "Latin1")]
        [InlineData(true, true, null)]
        public virtual void AddColumnOperation_with_charset_implicit(bool? isUnicode, bool isIndex, string charSetName)
        {
            var charSet = CharSet.GetCharSetFromName(charSetName);
            var expectedCharSetName = charSet != null ? $" CHARACTER SET {charSet}" : null;

            Generate(
                modelBuilder =>
                {
                    modelBuilder.HasCharSet(charSet);
                    modelBuilder.Entity(
                        "Person", eb =>
                        {
                            eb.Property<int>("Id");

                            var pb = eb.Property<string>("Name");

                            if (isUnicode.HasValue)
                            {
                                pb.IsUnicode(isUnicode.Value);
                            }

                            if (isIndex)
                            {
                                eb.HasIndex("Name");
                            }
                        });
                },
                modelBuilder =>
                {
                    var addColumn = modelBuilder.AddColumn<string>(name: "Name", table: "Person", nullable: true, unicode: isUnicode);

                    if (charSet != null)
                    {
                        addColumn.Annotation(MySqlAnnotationNames.CharSet, charSet);
                    }
                }
            );

            var columnType = isIndex
                ? $"varchar({MySqlTestHelpers.Instance.GetIndexedStringPropertyDefaultLength})"
                : "longtext";

            Assert.Equal(
                $"ALTER TABLE `Person` ADD `Name` {columnType}{expectedCharSetName} NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            Assert.Equal(
                @"ALTER TABLE `People` ADD `Alias` longtext NOT NULL;" + EOL,
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
                @"ALTER TABLE `Person` ADD `Name` varchar(32) NULL;" + EOL,
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
                        eb.Property<int>("Id");
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
                        eb.Property<int>("Id");
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
        public virtual void CreateIndexOperation_fulltext_with_parser()
        {
            Generate(
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Columns = new[] { "FirstName", "LastName" },
                    [MySqlAnnotationNames.FullTextIndex] = true,
                    [MySqlAnnotationNames.FullTextParser] = "ngram",
                });

            Assert.Equal(
                "CREATE FULLTEXT INDEX `IX_People_Name` ON `People` (`FirstName`, `LastName`) /*!50700 WITH PARSER `ngram` */;" + EOL,
                Sql);
        }

        [ConditionalFact]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.SpatialIndexes))]
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
        public virtual void RenameIndexOperations_throws_when_no_table()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.RenameIndex(
                name: "IX_OldIndex",
                newName: "IX_NewIndex");

            var ex = Assert.Throws<InvalidOperationException>(
                () => Generate(migrationBuilder.Operations.ToArray()));

            Assert.Equal(MySqlStrings.IndexTableRequired, ex.Message);
        }

        [ConditionalFact]
        public virtual void DropIndexOperations_throws_when_no_table()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.DropIndex(
                name: "IX_Name");

            var ex = Assert.Throws<InvalidOperationException>(
                () => Generate(migrationBuilder.Operations.ToArray()));

            Assert.Equal(MySqlStrings.IndexTableRequired, ex.Message);
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
                    x =>
                    {
                        x.Property<int>("Id");
                        x.Property<string>("FullName");
                    }),
                migrationBuilder.Operations.ToArray());

            Assert.Equal(
                AppConfig.ServerVersion.Supports.RenameColumn
                    ? "ALTER TABLE `Person` RENAME COLUMN `Name` TO `FullName`;" + EOL
                    : "ALTER TABLE `Person` CHANGE `Name` `FullName` longtext NULL;" + EOL,
                Sql);
        }

        [ConditionalFact]
        public virtual void RenameColumnOperation_with_model_required_without_default_value()
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
                        x.Property<int>("Id");
                        x.Property<string>("FullName")
                            .HasMaxLength(64)
                            .IsRequired();
                    }),
                migrationBuilder.Operations.ToArray());

            Assert.Equal(
                AppConfig.ServerVersion.Supports.RenameColumn
                    ? "ALTER TABLE `Person` RENAME COLUMN `Name` TO `FullName`;" + EOL
                    : "ALTER TABLE `Person` CHANGE `Name` `FullName` varchar(64) NOT NULL;" + EOL,
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
        public virtual void AddColumnOperation_with_charset_annotation()
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
        public virtual void CreateTableOperation_with_collation()
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
                            Collation = "latin1_swedish_ci"
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
                    [RelationalAnnotationNames.Collation] = "latin1_general_ci",
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreams` (
    `Brand` longtext COLLATE latin1_swedish_ci NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`Name`, `Brand`(20))
) COLLATE=latin1_general_ci;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void AlterTableOperation_with_collation()
        {
            Generate(
                modelBuilder =>
                {
                    modelBuilder.Entity(
                        "IceCreams",
                        entity =>
                        {
                            entity.Property<int>("Id");

                            entity.Property<string>("Brand")
                                .HasColumnType("longtext")
                                .UseCollation("latin1_swedish_ci");

                            entity.Property<string>("Name")
                                .HasColumnType("varchar(255)");

                            entity.UseCollation("latin1_general_ci");
                        });
                },
                migrationBuilder =>
                {
                    migrationBuilder.AlterTable("IceCreams")
                        .OldAnnotation(RelationalAnnotationNames.Collation, "latin1_general_ci")
                        .Annotation(RelationalAnnotationNames.Collation, "latin1_general_cs");
                });

            Assert.Equal(
                @"ALTER TABLE `IceCreams` COLLATE=latin1_general_cs;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void AlterTableOperation_with_collation_reset()
        {
            Generate(
                modelBuilder =>
                {
                    modelBuilder.Entity(
                        "IceCreams",
                        entity =>
                        {
                            entity.Property<int>("Id");

                            entity.Property<string>("Brand")
                                .HasColumnType("longtext")
                                .UseCollation("latin1_swedish_ci");

                            entity.Property<string>("Name")
                                .HasColumnType("varchar(255)");

                            entity.UseCollation("latin1_general_ci");
                        });
                },
                migrationBuilder =>
                {
                    migrationBuilder.AlterTable("IceCreams")
                        .OldAnnotation(RelationalAnnotationNames.Collation, "latin1_general_ci");
                });

            Assert.Equal(
                @"set @__pomelo_TableCharset = (
    SELECT `ccsa`.`CHARACTER_SET_NAME` as `TABLE_CHARACTER_SET`
    FROM `INFORMATION_SCHEMA`.`TABLES` as `t`
    LEFT JOIN `INFORMATION_SCHEMA`.`COLLATION_CHARACTER_SET_APPLICABILITY` as `ccsa` ON `ccsa`.`COLLATION_NAME` = `t`.`TABLE_COLLATION`
    WHERE `TABLE_SCHEMA` = SCHEMA() AND `TABLE_NAME` = 'IceCreams' AND `TABLE_TYPE` IN ('BASE TABLE', 'VIEW'));

SET @__pomelo_SqlExpr = CONCAT('ALTER TABLE `IceCreams` CHARACTER SET = ', @__pomelo_TableCharset, ';');
PREPARE __pomelo_SqlExprExecute FROM @__pomelo_SqlExpr;
EXECUTE __pomelo_SqlExprExecute;
DEALLOCATE PREPARE __pomelo_SqlExprExecute;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void CreateTableOperation_with_charset()
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
                            [MySqlAnnotationNames.CharSet] = "utf8mb4"
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
                    [MySqlAnnotationNames.CharSet] = "latin1",
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreams` (
    `Brand` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`Name`, `Brand`(20))
) CHARACTER SET=latin1;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void AlterTableOperation_with_charset()
        {
            Generate(
                modelBuilder =>
                {
                    modelBuilder.Entity(
                        "IceCreams",
                        entity =>
                        {
                            entity.Property<int>("Id");

                            entity.Property<string>("Brand")
                                .HasColumnType("longtext")
                                .HasCharSet("utf8mb4");

                            entity.Property<string>("Name")
                                .HasColumnType("varchar(255)");

                            entity.HasCharSet("latin1");
                        });
                },
                migrationBuilder =>
                {
                    migrationBuilder.AlterTable("IceCreams")
                        .OldAnnotation(MySqlAnnotationNames.CharSet, "latin1")
                        .Annotation(MySqlAnnotationNames.CharSet, "utf8mb4");
                });

            Assert.Equal(
                @"ALTER TABLE `IceCreams` CHARACTER SET=utf8mb4;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void AlterTableOperation_with_charset_reset()
        {
            Generate(
                modelBuilder =>
                {
                    modelBuilder.Entity(
                        "IceCreams",
                        entity =>
                        {
                            entity.Property<int>("Id");

                            entity.Property<string>("Brand")
                                .HasColumnType("longtext")
                                .HasCharSet("utf8mb4");

                            entity.Property<string>("Name")
                                .HasColumnType("varchar(255)");

                            entity.HasCharSet("latin1");
                        });
                },
                migrationBuilder =>
                {
                    migrationBuilder.AlterTable("IceCreams")
                        .OldAnnotation(MySqlAnnotationNames.CharSet, "latin1");
                });

            Assert.Equal(
                @"set @__pomelo_TableCharset = (
    SELECT `ccsa`.`CHARACTER_SET_NAME` as `TABLE_CHARACTER_SET`
    FROM `INFORMATION_SCHEMA`.`TABLES` as `t`
    LEFT JOIN `INFORMATION_SCHEMA`.`COLLATION_CHARACTER_SET_APPLICABILITY` as `ccsa` ON `ccsa`.`COLLATION_NAME` = `t`.`TABLE_COLLATION`
    WHERE `TABLE_SCHEMA` = SCHEMA() AND `TABLE_NAME` = 'IceCreams' AND `TABLE_TYPE` IN ('BASE TABLE', 'VIEW'));

SET @__pomelo_SqlExpr = CONCAT('ALTER TABLE `IceCreams` CHARACTER SET = ', @__pomelo_TableCharset, ';');
PREPARE __pomelo_SqlExprExecute FROM @__pomelo_SqlExpr;
EXECUTE __pomelo_SqlExprExecute;
DEALLOCATE PREPARE __pomelo_SqlExprExecute;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }


        [ConditionalFact]
        public virtual void CreateTableOperation_with_table_options()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "IceCreams",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Name",
                            ColumnType = "varchar(128)",
                            ClrType = typeof(string),
                        },
                    },
                    PrimaryKey = new AddPrimaryKeyOperation
                    {
                        Columns = new[] { "Name" }
                    },
                    [MySqlAnnotationNames.StoreOptions] = "CHECKSUM=1,MAX_ROWS=100",
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreams` (
    `Name` varchar(128) NOT NULL,
    PRIMARY KEY (`Name`)
) CHECKSUM=1 MAX_ROWS=100;" + EOL,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void AlterTableOperation_with_table_options()
        {
            Generate(
                modelBuilder =>
                {
                    modelBuilder.Entity(
                        "IceCreams",
                        entity =>
                        {
                            entity.Property<string>("Name")
                                .HasColumnType("varchar(128)");

                            entity.HasKey("Name");

                            entity.HasTableOption("CHECKSUM", "1");
                            entity.HasTableOption("MAX_ROWS", "100");
                        });
                },
                migrationBuilder =>
                {
                    migrationBuilder.AlterTable("IceCreams")
                        .OldAnnotation(MySqlAnnotationNames.StoreOptions, "CHECKSUM=1,MAX_ROWS=100")
                        .Annotation(MySqlAnnotationNames.StoreOptions, "CHECKSUM=1,MIN_ROWS=20,MAX_ROWS=200");
                });

            Assert.Equal(
                @"ALTER TABLE `IceCreams` MIN_ROWS=20 MAX_ROWS=200;" + EOL,
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

        protected override void Generate(params MigrationOperation[] operation)
            => Generate(null, operation);

        protected override void Generate(
            Action<ModelBuilder> buildAction,
            MigrationOperation[] operation,
            MigrationsSqlGenerationOptions options)
            => Generate(null, buildAction, operation, options);

        protected virtual void Generate(
            Action<MySqlDbContextOptionsBuilder> optionsAction,
            Action<ModelBuilder> buildAction,
            MigrationOperation[] operations,
            MigrationsSqlGenerationOptions options)
        {
            // Might not be needed if we just set SchemaBehavior below.
            // ResetSchemaProperties(operations);

            var optionsBuilder = new DbContextOptionsBuilder(ContextOptions);
            var mySqlOptionsBuilder = new MySqlDbContextOptionsBuilder(optionsBuilder);

            mySqlOptionsBuilder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            optionsAction?.Invoke(mySqlOptionsBuilder);

            var services = TestHelpers.CreateContextServices(CustomServices, optionsBuilder.Options);

            IModel model = null;
            if (buildAction != null)
            {
                var modelBuilder = TestHelpers.CreateConventionBuilder();
                modelBuilder.Model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);
                buildAction(modelBuilder);

                model = services.GetService<IModelRuntimeInitializer>().Initialize(
                    modelBuilder.FinalizeModel(), designTime: true, validationLogger: null);
            }

            var batch = services.GetRequiredService<IMigrationsSqlGenerator>().Generate(operations, model, options);

            Sql = string.Join(
                EOL,
                batch.Select(b => b.CommandText));
        }

        private static void ResetSchemaProperties(MigrationOperation[] operations)
        {
            foreach (var operation in operations)
            {
                var schemaProperties = operation.GetType().GetRuntimeProperties().Where(p => p.Name.Contains("Schema"));
                foreach (var schemaProperty in schemaProperties)
                {
                    schemaProperty.SetValue(operation, null);
                }
            }
        }
    }
}
