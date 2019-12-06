using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class DataAnnotationMySqlTest : DataAnnotationTestBase<DataAnnotationMySqlTest.DataAnnotationMySqlFixture>
    {
        // ReSharper disable once UnusedParameter.Local
        public DataAnnotationMySqlTest(DataAnnotationMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public override ModelBuilder Non_public_annotations_are_enabled()
        {
            var modelBuilder = base.Non_public_annotations_are_enabled();

            var relational = GetProperty<PrivateMemberAnnotationClass>(modelBuilder, "PersonFirstName");
            Assert.Equal("dsdsd", relational.GetColumnName());
            Assert.Equal("nvarchar(128)", relational.GetColumnType());

            return modelBuilder;
        }

        public override ModelBuilder Field_annotations_are_enabled()
        {
            var modelBuilder = base.Field_annotations_are_enabled();

            var relational = GetProperty<FieldAnnotationClass>(modelBuilder, "_personFirstName");
            Assert.Equal("dsdsd", relational.GetColumnName());
            Assert.Equal("nvarchar(128)", relational.GetColumnType());

            return modelBuilder;
        }

        public override ModelBuilder Key_and_column_work_together()
        {
            var modelBuilder = base.Key_and_column_work_together();

            var relational = GetProperty<ColumnKeyAnnotationClass1>(modelBuilder, "PersonFirstName");
            Assert.Equal("dsdsd", relational.GetColumnName());
            Assert.Equal("nvarchar(128)", relational.GetColumnType());

            return modelBuilder;
        }

        public override ModelBuilder Key_and_MaxLength_64_produce_nvarchar_64()
        {
            var modelBuilder = base.Key_and_MaxLength_64_produce_nvarchar_64();

            var property = GetProperty<ColumnKeyAnnotationClass2>(modelBuilder, "PersonFirstName");

            var storeType = property.FindRelationalMapping().StoreType;

            Assert.Equal("varchar(64) CHARACTER SET utf8mb4", storeType);

            return modelBuilder;
        }

        public override ModelBuilder Timestamp_takes_precedence_over_MaxLength()
        {
            var modelBuilder = base.Timestamp_takes_precedence_over_MaxLength();

            var property = GetProperty<TimestampAndMaxlen>(modelBuilder, "MaxTimestamp");

            var storeTypeNameBase = property.FindRelationalMapping().StoreTypeNameBase;

            Assert.Equal("timestamp", storeTypeNameBase);

            return modelBuilder;
        }

        public override ModelBuilder TableNameAttribute_affects_table_name_in_TPH()
        {
            var modelBuilder = base.TableNameAttribute_affects_table_name_in_TPH();

            var relational = modelBuilder.Model.FindEntityType(typeof(TNAttrBase));
            Assert.Equal("A", relational.GetTableName());

            return modelBuilder;
        }

        public override void ConcurrencyCheckAttribute_throws_if_value_in_database_changed()
        {
            base.ConcurrencyCheckAttribute_throws_if_value_in_database_changed();

            AssertSql(
                @"SELECT `s`.`UniqueNo`, `s`.`MaxLengthProperty`, `s`.`Name`, `s`.`RowVersion`, `t`.`UniqueNo`, `t`.`AdditionalDetails_Name`, `t0`.`UniqueNo`, `t0`.`Details_Name`
FROM `Sample` AS `s`
LEFT JOIN (
    SELECT `s0`.`UniqueNo`, `s0`.`AdditionalDetails_Name`, `s1`.`UniqueNo` AS `UniqueNo0`
    FROM `Sample` AS `s0`
    INNER JOIN `Sample` AS `s1` ON `s0`.`UniqueNo` = `s1`.`UniqueNo`
    WHERE `s0`.`AdditionalDetails_Name` IS NOT NULL
) AS `t` ON `s`.`UniqueNo` = `t`.`UniqueNo`
LEFT JOIN (
    SELECT `s2`.`UniqueNo`, `s2`.`Details_Name`, `s3`.`UniqueNo` AS `UniqueNo0`
    FROM `Sample` AS `s2`
    INNER JOIN `Sample` AS `s3` ON `s2`.`UniqueNo` = `s3`.`UniqueNo`
    WHERE `s2`.`Details_Name` IS NOT NULL
) AS `t0` ON `s`.`UniqueNo` = `t0`.`UniqueNo`
WHERE `s`.`UniqueNo` = 1
LIMIT 1",
                //
                @"SELECT `s`.`UniqueNo`, `s`.`MaxLengthProperty`, `s`.`Name`, `s`.`RowVersion`, `t`.`UniqueNo`, `t`.`AdditionalDetails_Name`, `t0`.`UniqueNo`, `t0`.`Details_Name`
FROM `Sample` AS `s`
LEFT JOIN (
    SELECT `s0`.`UniqueNo`, `s0`.`AdditionalDetails_Name`, `s1`.`UniqueNo` AS `UniqueNo0`
    FROM `Sample` AS `s0`
    INNER JOIN `Sample` AS `s1` ON `s0`.`UniqueNo` = `s1`.`UniqueNo`
    WHERE `s0`.`AdditionalDetails_Name` IS NOT NULL
) AS `t` ON `s`.`UniqueNo` = `t`.`UniqueNo`
LEFT JOIN (
    SELECT `s2`.`UniqueNo`, `s2`.`Details_Name`, `s3`.`UniqueNo` AS `UniqueNo0`
    FROM `Sample` AS `s2`
    INNER JOIN `Sample` AS `s3` ON `s2`.`UniqueNo` = `s3`.`UniqueNo`
    WHERE `s2`.`Details_Name` IS NOT NULL
) AS `t0` ON `s`.`UniqueNo` = `t0`.`UniqueNo`
WHERE `s`.`UniqueNo` = 1
LIMIT 1",
                //
                @"@p2='1'
@p0='ModifiedData' (Nullable = false) (Size = 4000)
@p1='00000000-0000-0000-0003-000000000001'
@p3='00000001-0000-0000-0000-000000000001'

UPDATE `Sample` SET `Name` = @p0, `RowVersion` = @p1
WHERE `UniqueNo` = @p2 AND `RowVersion` = @p3;
SELECT ROW_COUNT();",
                //
                @"@p2='1'
@p0='ChangedData' (Nullable = false) (Size = 4000)
@p1='00000000-0000-0000-0002-000000000001'
@p3='00000001-0000-0000-0000-000000000001'

UPDATE `Sample` SET `Name` = @p0, `RowVersion` = @p1
WHERE `UniqueNo` = @p2 AND `RowVersion` = @p3;
SELECT ROW_COUNT();");
        }

        public override void DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity()
        {
            base.DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity();

            AssertSql(@"@p0=NULL (Size = 10)
@p1='Third' (Nullable = false) (Size = 4000)
@p2='00000000-0000-0000-0000-000000000003'
@p3='Third Additional Name' (Size = 4000)
@p4='Third Name' (Size = 4000)

INSERT INTO `Sample` (`MaxLengthProperty`, `Name`, `RowVersion`, `AdditionalDetails_Name`, `Details_Name`)
VALUES (@p0, @p1, @p2, @p3, @p4);
SELECT `UniqueNo`
FROM `Sample`
WHERE ROW_COUNT() = 1 AND `UniqueNo` = LAST_INSERT_ID();");
        }


        public override void RequiredAttribute_for_navigation_throws_while_inserting_null_value()
        {
            base.RequiredAttribute_for_navigation_throws_while_inserting_null_value();

            AssertSql(@"@p0=NULL (DbType = Int32)
@p1='1'

INSERT INTO `BookDetails` (`AdditionalBookDetailsId`, `AnotherBookId`)
VALUES (@p0, @p1);
SELECT `Id`
FROM `BookDetails`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();",
                //////////////////
                @"@p0=NULL (DbType = Int32)
@p1=NULL (Nullable = false) (DbType = Int32)

INSERT INTO `BookDetails` (`AdditionalBookDetailsId`, `AnotherBookId`)
VALUES (@p0, @p1);
SELECT `Id`
FROM `BookDetails`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();");
        }

        public override void RequiredAttribute_for_property_throws_while_inserting_null_value()
        {
            base.RequiredAttribute_for_property_throws_while_inserting_null_value();

            AssertSql(@"@p0=NULL (Size = 10)
@p1='ValidString' (Nullable = false) (Size = 4000)
@p2='00000000-0000-0000-0000-000000000001'
@p3='Two' (Size = 4000)
@p4='One' (Size = 4000)

INSERT INTO `Sample` (`MaxLengthProperty`, `Name`, `RowVersion`, `AdditionalDetails_Name`, `Details_Name`)
VALUES (@p0, @p1, @p2, @p3, @p4);
SELECT `UniqueNo`
FROM `Sample`
WHERE ROW_COUNT() = 1 AND `UniqueNo` = LAST_INSERT_ID();",
                ///////////////////
                @"@p0=NULL (Size = 10)
@p1=NULL (Nullable = false) (Size = 4000)
@p2='00000000-0000-0000-0000-000000000002'
@p3='Two' (Size = 4000)
@p4='One' (Size = 4000)

INSERT INTO `Sample` (`MaxLengthProperty`, `Name`, `RowVersion`, `AdditionalDetails_Name`, `Details_Name`)
VALUES (@p0, @p1, @p2, @p3, @p4);
SELECT `UniqueNo`
FROM `Sample`
WHERE ROW_COUNT() = 1 AND `UniqueNo` = LAST_INSERT_ID();");
        }

        private static readonly string _eol = Environment.NewLine;

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class DataAnnotationMySqlFixture : DataAnnotationFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
        }
    }
}
