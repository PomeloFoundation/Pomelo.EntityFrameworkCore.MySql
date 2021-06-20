using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class DataAnnotationMySqlTest : DataAnnotationRelationalTestBase<DataAnnotationMySqlTest.DataAnnotationMySqlFixture>
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
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<PrivateMemberAnnotationClass>().Property(
                    PrivateMemberAnnotationClass.PersonFirstNameExpr)
                .HasColumnType("varchar(128)");

            Validate(modelBuilder);

            Assert.True(GetProperty<PrivateMemberAnnotationClass>(modelBuilder, "PersonFirstName").IsPrimaryKey());

            return modelBuilder;
        }

        public override ModelBuilder Field_annotations_are_enabled()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<FieldAnnotationClass>()
                .Property<string>("_personFirstName")
                .HasColumnType("varchar(128)");

            Validate(modelBuilder);

            Assert.True(GetProperty<FieldAnnotationClass>(modelBuilder, "_personFirstName").IsPrimaryKey());

            return modelBuilder;
        }

        public override ModelBuilder Key_and_column_work_together()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<ColumnKeyAnnotationClass1>()
                .Property(c => c.PersonFirstName)
                .HasColumnType("varchar(128)");

            Validate(modelBuilder);

            Assert.True(GetProperty<ColumnKeyAnnotationClass1>(modelBuilder, "PersonFirstName").IsPrimaryKey());

            return modelBuilder;
        }

        public override ModelBuilder Key_and_MaxLength_64_produce_nvarchar_64()
        {
            var modelBuilder = base.Key_and_MaxLength_64_produce_nvarchar_64();

            var property = GetProperty<ColumnKeyAnnotationClass2>(modelBuilder, "PersonFirstName");

            var storeType = property.GetRelationalTypeMapping().StoreType;

            Assert.Equal("varchar(64)", storeType);

            return modelBuilder;
        }

        public override ModelBuilder Timestamp_takes_precedence_over_MaxLength()
        {
            var modelBuilder = base.Timestamp_takes_precedence_over_MaxLength();

            var property = GetProperty<TimestampAndMaxlen>(modelBuilder, "MaxTimestamp");

            var storeTypeNameBase = property.GetRelationalTypeMapping().StoreTypeNameBase;

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
                @"SELECT `s`.`Unique_No`, `s`.`MaxLengthProperty`, `s`.`Name`, `s`.`RowVersion`, `s`.`AdditionalDetails_Name`, `s`.`AdditionalDetails_Value`, `s`.`Details_Name`, `s`.`Details_Value`
FROM `Sample` AS `s`
WHERE `s`.`Unique_No` = 1
LIMIT 1",
                //
                @"SELECT `s`.`Unique_No`, `s`.`MaxLengthProperty`, `s`.`Name`, `s`.`RowVersion`, `s`.`AdditionalDetails_Name`, `s`.`AdditionalDetails_Value`, `s`.`Details_Name`, `s`.`Details_Value`
FROM `Sample` AS `s`
WHERE `s`.`Unique_No` = 1
LIMIT 1",
                //
                @"@p2='1'
@p0='ModifiedData' (Nullable = false) (Size = 4000)
@p1='00000000-0000-0000-0003-000000000001'
@p3='00000001-0000-0000-0000-000000000001'

UPDATE `Sample` SET `Name` = @p0, `RowVersion` = @p1
WHERE `Unique_No` = @p2 AND `RowVersion` = @p3;
SELECT ROW_COUNT();",
                //
                @"@p2='1'
@p0='ChangedData' (Nullable = false) (Size = 4000)
@p1='00000000-0000-0000-0002-000000000001'
@p3='00000001-0000-0000-0000-000000000001'

UPDATE `Sample` SET `Name` = @p0, `RowVersion` = @p1
WHERE `Unique_No` = @p2 AND `RowVersion` = @p3;
SELECT ROW_COUNT();");
        }

        public override void DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity()
        {
            base.DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity();

            AssertSql(
                @"@p0=NULL (Size = 10)
@p1='Third' (Nullable = false) (Size = 4000)
@p2='00000000-0000-0000-0000-000000000003'
@p3='Third Additional Name' (Size = 4000)
@p4='0' (Nullable = true)
@p5='Third Name' (Size = 4000)
@p6='0' (Nullable = true)

INSERT INTO `Sample` (`MaxLengthProperty`, `Name`, `RowVersion`, `AdditionalDetails_Name`, `AdditionalDetails_Value`, `Details_Name`, `Details_Value`)
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6);
SELECT `Unique_No`
FROM `Sample`
WHERE ROW_COUNT() = 1 AND `Unique_No` = LAST_INSERT_ID();");
        }

        [ConditionalFact]
        public virtual ModelBuilder CharSet_attribute_is_applied_to_column()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<ColumnWithCharSet>();

            Validate(modelBuilder);

            Assert.Equal("latin1", GetProperty<ColumnWithCharSet>(modelBuilder, "PersonFirstName").GetCharSet());

            return modelBuilder;
        }

        protected class ColumnWithCharSet
        {
            public int Id { get; set; }

            [MySqlCharSet("latin1")]
            public string PersonFirstName { get; set; }
        }

        [ConditionalFact]
        public virtual ModelBuilder CharSet_attribute_is_applied_to_table()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<TableWithCharSet>();

            Validate(modelBuilder);

            Assert.Equal("latin1", GetEntityType<TableWithCharSet>(modelBuilder).GetCharSet());

            return modelBuilder;
        }

        [MySqlCharSet("latin1")]
        protected class TableWithCharSet
        {
            public int Id { get; set; }
        }

        [ConditionalFact]
        public virtual ModelBuilder Collation_attribute_is_applied_to_column()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<ColumnWithCollation>();

            Validate(modelBuilder);

            Assert.Equal("latin1_bin", GetProperty<ColumnWithCollation>(modelBuilder, "PersonFirstName").GetCollation());

            return modelBuilder;
        }

        protected class ColumnWithCollation
        {
            public int Id { get; set; }

            [MySqlCollation("latin1_bin")]
            public string PersonFirstName { get; set; }
        }

        [ConditionalFact]
        public virtual ModelBuilder Collation_attribute_is_applied_to_table()
        {
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<TableWithCollation>();

            Validate(modelBuilder);

            Assert.Equal("latin1_bin", GetEntityType<TableWithCollation>(modelBuilder).GetCollation());

            return modelBuilder;
        }

        [MySqlCollation("latin1_bin")]
        protected class TableWithCollation
        {
            public int Id { get; set; }
        }

        protected static IMutableEntityType GetEntityType<TEntity>(ModelBuilder modelBuilder)
            => modelBuilder.Model.FindEntityType(typeof(TEntity));

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class DataAnnotationMySqlFixture : DataAnnotationRelationalFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
        }
    }
}
