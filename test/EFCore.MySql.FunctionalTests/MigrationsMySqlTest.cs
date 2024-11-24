using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MigrationsMySqlTest : MigrationsTestBase<MigrationsMySqlTest.MigrationsMySqlFixture>
    {
        public MigrationsMySqlTest(MigrationsMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Alter_check_constraint()
        {
            await base.Alter_check_constraint();

            AssertSql(
"""
ALTER TABLE `People` DROP CONSTRAINT `CK_People_Foo`;
""",
                //
                """
ALTER TABLE `People` ADD CONSTRAINT `CK_People_Foo` CHECK (`DriverLicense` > 1);
""");
        }

        public override async Task Alter_column_make_computed(bool? stored)
        {
            if (stored == true)
            {
                await base.Alter_column_make_computed(stored);

                var computedColumnTypeSql = stored == true ? " STORED" : "";

                AssertSql(
$"""
ALTER TABLE `People` MODIFY COLUMN `Sum` int AS (`X` + `Y`){computedColumnTypeSql};
""");
            }
            else
            {
                var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Alter_column_make_computed(stored));
                Assert.Equal("'Changing the STORED status' is not supported for generated columns.", exception.Message);
            }
        }

        public override async Task Add_column_computed_with_collation(bool stored)
        {
            await base.Add_column_computed_with_collation(stored);

            var computedColumnTypeSql = stored ? " STORED" : "";

            AssertSql(
$"""
ALTER TABLE `People` ADD `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_as_cs AS ('hello'){computedColumnTypeSql} NULL;
""");
        }

        public override async Task Add_column_with_collation()
        {
            await base.Add_column_with_collation();

            AssertSql(
"""
ALTER TABLE `People` ADD `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_as_cs NULL;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public override async Task Add_column_with_defaultValue_string()
        {
            await base.Add_column_with_defaultValue_string();

            AssertSql(
                @"ALTER TABLE `People` ADD `Name` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('John Doe');");
        }

        public override async Task Alter_column_make_required()
        {
            await base.Alter_column_make_required();

            AssertSql(
                @"UPDATE `People` SET `SomeColumn` = ''
WHERE `SomeColumn` IS NULL;
SELECT ROW_COUNT();",
                //
                @"ALTER TABLE `People` MODIFY COLUMN `SomeColumn` longtext CHARACTER SET utf8mb4 NOT NULL;");
        }

        /// <remarks>
        /// MySQL only supports changing STORED computed columns to non-computed ones.
        /// </remarks>
        [ConditionalFact]
        public override async Task Alter_column_make_non_computed()
        {
            await Test(
                builder => builder.Entity(
                    "People", e =>
                    {
                        e.Property<int>("Id");
                        e.Property<int>("X");
                        e.Property<int>("Y");
                    }),
                builder => builder.Entity("People")
                    .Property<int>("Sum")
                    .HasComputedColumnSql($"{DelimitIdentifier("X")} + {DelimitIdentifier("Y")}", stored: true), // <-- added "stored: true"
                builder => builder.Entity("People").Property<int>("Sum"),
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    var sumColumn = Assert.Single(table.Columns, c => c.Name == "Sum");
                    Assert.Null(sumColumn.ComputedColumnSql);
                    Assert.NotEqual(true, sumColumn.IsStored);
                });

            AssertSql(
                @"ALTER TABLE `People` MODIFY COLUMN `Sum` int NOT NULL;");
        }

        [ConditionalFact]
        public virtual async Task Alter_string_column_make_required_generates_update_statement_instead_of_default_value()
        {
            await Test(
                builder => builder.Entity(
                    "People", e =>
                    {
                        e.Property<int>("Id");
                        e.Property<string>("SomeColumn");
                    }),
                builder => { },
                builder => builder.Entity("People").Property<string>("SomeColumn").IsRequired(),
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    var column = Assert.Single(table.Columns, c => c.Name != "Id");
                    Assert.False(column.IsNullable);
                    Assert.Null(column.DefaultValueSql);
                });

            AssertSql(
                @"UPDATE `People` SET `SomeColumn` = ''
WHERE `SomeColumn` IS NULL;
SELECT ROW_COUNT();",
                //
                @"ALTER TABLE `People` MODIFY COLUMN `SomeColumn` longtext CHARACTER SET utf8mb4 NOT NULL;");
        }

        [ConditionalFact]
        public virtual async Task Add_column_with_defaultValue_string_limited_length()
        {
            await Test(
                builder => builder.Entity("People").Property<int>("Id"),
                builder => { },
                builder => builder.Entity("People")
                    .Property<string>("Name")
                    .HasMaxLength(128) // specify explicit length
                    .IsRequired()
                    .HasDefaultValue("John Doe"),
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    Assert.Equal(2, table.Columns.Count);
                    var nameColumn = Assert.Single(table.Columns, c => c.Name == "Name");
                    Assert.False(nameColumn.IsNullable);
                    Assert.Contains("John Doe", nameColumn.DefaultValueSql);
                });

            AssertSql(
                @"ALTER TABLE `People` ADD `Name` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'John Doe';");
        }

        [ConditionalFact]
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public virtual async Task Add_column_with_defaultValue_string_unlimited_length_without_default_value_expression_support_throws_warning()
        {
            await TestThrows<InvalidOperationException>(
                builder => builder.Entity("People").Property<int>("Id"),
                builder => { },
                builder => builder.Entity("People")
                    .Property<string>("Name")
                    .IsRequired()
                    .HasDefaultValue("John Doe"));
        }

        public override async Task Add_column_with_defaultValue_datetime()
        {
            await base.Add_column_with_defaultValue_datetime();

            AssertSql(
                @"ALTER TABLE `People` ADD `Birthday` datetime(6) NOT NULL DEFAULT '2015-04-12 17:05:00';");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public override async Task Add_column_with_defaultValueSql()
        {
            await Test(
                builder => builder.Entity("People").Property<int>("Id"),
                builder => { },
                builder => builder.Entity("People")
                    .Property<int>("Sum")
                    .HasDefaultValueSql("(1 + 2)"), // default expression needs to be wrapped in parenthesis
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    Assert.Equal(2, table.Columns.Count);
                    var sumColumn = Assert.Single(table.Columns, c => c.Name == "Sum");
                    Assert.Contains("1", sumColumn.DefaultValueSql);
                    Assert.Contains("+", sumColumn.DefaultValueSql);
                    Assert.Contains("2", sumColumn.DefaultValueSql);
                });

            AssertSql(
                @"ALTER TABLE `People` ADD `Sum` int NOT NULL DEFAULT (1 + 2);");
        }

        [ConditionalFact]
        public virtual async Task Add_column_with_defaultValueSql_simple()
        {
            await Test(
                builder => builder.Entity("People").Property<int>("Id"),
                builder => { },
                builder => builder.Entity("People")
                    .Property<int>("Sum")
                    .HasDefaultValueSql("3"), // simple value
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    Assert.Equal(2, table.Columns.Count);
                    var sumColumn = Assert.Single(table.Columns, c => c.Name == "Sum");
                    Assert.Contains("3", sumColumn.DefaultValueSql);
                });

            AssertSql(
                @"ALTER TABLE `People` ADD `Sum` int NOT NULL DEFAULT 3;");
        }

        public override async Task Rename_index()
        {
            await base.Rename_index();

            AssertSql(
                AppConfig.ServerVersion.Supports.RenameIndex
                    ? new[] { @"ALTER TABLE `People` RENAME INDEX `Foo` TO `foo`;" }
                    : new[]
                    {
                        @"ALTER TABLE `People` DROP INDEX `Foo`;",
                        //
                        "CREATE INDEX `foo` ON `People` (`FirstName`);"
                    });
        }

        [ConditionalFact]
        public virtual async Task Rename_index_with_prefix_length()
        {
            await Test(
                builder => builder.Entity(
                    "People", e =>
                    {
                        e.Property<int>("Id");
                        e.Property<string>("FirstName");
                    }),
                builder => builder.Entity("People").HasIndex(new[] { "FirstName" }, "OldIndex").HasPrefixLength(50),
                builder => builder.Entity("People").HasIndex(new[] { "FirstName" }, "NewIndex").HasPrefixLength(50),
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    var index = Assert.Single(table.Indexes);
                    Assert.Equal("NewIndex", index.Name);
                });

            AssertSql(
                AppConfig.ServerVersion.Supports.RenameIndex
                    ? new[] { @"ALTER TABLE `People` RENAME INDEX `OldIndex` TO `NewIndex`;" }
                    : new[]
                    {
                        @"ALTER TABLE `People` DROP INDEX `OldIndex`;",
                        //
                        "CREATE INDEX `NewIndex` ON `People` (`FirstName`(50));"
                    });
        }

        [ConditionalTheory(Skip = "For this to work, either MySqlMigrator needs to be involved, or the primary key related stored procedures need to be handled by MySqlMigrationsSqlGenerator instead. The later is probably the way to go. We should move the primary key related stored procedures to its own service, so ti can be potentially be customized by users.")]
        public override async Task Add_primary_key_string()
        {
            await base.Add_primary_key_string();

            AssertSql("");
        }

        public override async Task Add_primary_key_composite_with_name()
        {
            await base.Add_primary_key_composite_with_name();

            AssertSql(
"""
ALTER TABLE `People` ADD CONSTRAINT `PK_Foo` PRIMARY KEY (`SomeField1`, `SomeField2`);
""");
        }

        [ConditionalTheory(Skip = "For this to work, either MySqlMigrator needs to be involved, or the primary key related stored procedures need to be handled by MySqlMigrationsSqlGenerator instead. The later is probably the way to go. We should move the primary key related stored procedures to its own service, so ti can be potentially be customized by users.")]
        public override async Task Add_primary_key_with_name()
        {
            await base.Add_primary_key_with_name();

            AssertSql("");
        }

        [ConditionalTheory(Skip = "Are we not scaffolding unique constraints yet?")]
        public override async Task Add_unique_constraint()
        {
            await base.Add_unique_constraint();

            AssertSql("");
        }

        [ConditionalTheory(Skip = "Are we not scaffolding unique constraints yet?")]
        public override async Task Add_unique_constraint_composite_with_name()
        {
            await base.Add_unique_constraint_composite_with_name();

            AssertSql("");
        }

        public override async Task Alter_column_change_computed_type()
        {
            var exception = await Assert.ThrowsAsync<MySqlException>(() => base.Alter_column_change_computed_type());
            Assert.Equal("'Changing the STORED status' is not supported for generated columns.", exception.Message);
        }

        public override async Task Alter_column_change_type()
        {
            await base.Alter_column_change_type();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `SomeColumn` bigint NOT NULL;
""");
        }

        public override async Task Alter_column_set_collation()
        {
            await base.Alter_column_set_collation();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_as_cs NULL;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Alter_sequence_all_settings()
        {
            await base.Alter_sequence_all_settings();

            AssertSql(
"""
ALTER SEQUENCE `foo` INCREMENT BY 2 MINVALUE -5 MAXVALUE 10 CYCLE;
""",
                //
"""
ALTER SEQUENCE `foo` START WITH -3 RESTART;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Alter_sequence_increment_by()
        {
            await base.Alter_sequence_increment_by();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Alter_sequence_restart_with()
        {
            await base.Alter_sequence_restart_with();

            AssertSql(
                @"ALTER SEQUENCE `foo` START WITH 3 RESTART;");
        }

        public override async Task Alter_table_add_comment_non_default_schema()
        {
            await base.Alter_table_add_comment_non_default_schema();

            AssertSql(
"""
ALTER TABLE `People` COMMENT 'Table comment';
""");
        }

        [ConditionalFact(Skip = "MySQL does not support filtered indices.")]
        public override async Task Create_index_with_filter()
        {
            await base.Create_index_with_filter();

            AssertSql("");
        }

        public override async Task Create_schema()
        {
            await base.Create_schema();

            AssertSql(
"""
CREATE TABLE `People` (
    `Id` int NOT NULL AUTO_INCREMENT,
    CONSTRAINT `PK_People` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Create_sequence()
        {
            await base.Create_sequence();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Create_sequence_long()
        {
            await base.Create_sequence_long();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Create_sequence_short()
        {
            await base.Create_sequence_short();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Create_sequence_all_settings()
        {
            await Test(
                builder => { },
                builder => builder.HasSequence<long>("TestSequence", "dbo2")
                    .StartsAt(3)
                    .IncrementsBy(2)
                    .HasMin(2)
                    .HasMax(916)
                    .IsCyclic(),
                model =>
                {
                    var sequence = Assert.Single(model.Sequences);

                    // Assert.Equal("TestSequence", sequence.Name);
                    // Assert.Equal("dbo2", sequence.Schema);
                    Assert.Equal("dbo2_TestSequence", sequence.Name);

                    Assert.Equal(3, sequence.StartValue);
                    Assert.Equal(2, sequence.IncrementBy);
                    Assert.Equal(2, sequence.MinValue);
                    Assert.Equal(916, sequence.MaxValue);
                    Assert.True(sequence.IsCyclic);
                });

            AssertSql(
"""
CREATE SEQUENCE `dbo2_TestSequence` START WITH 3 INCREMENT BY 2 MINVALUE 2 MAXVALUE 916 CYCLE;
""");
        }

        [ConditionalFact(Skip = "Are we not scaffolding unique constraints yet?")]
        public override async Task Create_table_all_settings()
        {
            await base.Create_table_all_settings();

            AssertSql("");
        }

        public override async Task Create_table_with_multiline_comments()
        {
            await base.Create_table_with_multiline_comments();

            AssertSql(
                @"CREATE TABLE `People` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL COMMENT 'This is a multi-line
column comment.
More information can
be found in the docs.',
    CONSTRAINT `PK_People` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COMMENT='This is a multi-line
table comment.
More information can
be found in the docs.';");
        }

        [ConditionalFact(Skip = "MySQL does not support filtered indices.")]
        public override async Task Create_unique_index_with_filter()
        {
            await base.Create_unique_index_with_filter();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DescendingIndexes))]
        public override async Task Create_index_descending()
        {
            await base.Create_index_descending();

            AssertSql(
                @"CREATE INDEX `IX_People_X` ON `People` (`X` DESC);");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DescendingIndexes))]
        public override async Task Create_index_descending_mixed()
        {
            await base.Create_index_descending_mixed();

            AssertSql(
                @"CREATE INDEX `IX_People_X_Y_Z` ON `People` (`X`, `Y` DESC, `Z`);");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DescendingIndexes))]
        public override async Task Alter_index_change_sort_order()
        {
            await base.Alter_index_change_sort_order();

            AssertSql(
                @"ALTER TABLE `People` DROP INDEX `IX_People_X_Y_Z`;",
                //
                @"CREATE INDEX `IX_People_X_Y_Z` ON `People` (`X`, `Y` DESC, `Z`);");
        }

        public override async Task Drop_check_constraint()
        {
            await base.Drop_check_constraint();

            AssertSql(
"""
ALTER TABLE `People` DROP CONSTRAINT `CK_People_Foo`;
""");
        }

        [ConditionalFact(Skip = "For this to work, either MySqlMigrator needs to be involved, or the primary key related stored procedures need to be handled by MySqlMigrationsSqlGenerator instead. The later is probably the way to go. We should move the primary key related stored procedures to its own service, so ti can be potentially be customized by users.")]
        public override async Task Drop_column_primary_key()
        {
            await base.Drop_column_primary_key();

            AssertSql("");
        }

        [ConditionalFact(Skip = "For this to work, either MySqlMigrator needs to be involved, or the primary key related stored procedures need to be handled by MySqlMigrationsSqlGenerator instead. The later is probably the way to go. We should move the primary key related stored procedures to its own service, so ti can be potentially be customized by users.")]
        public override async Task Drop_primary_key_int()
        {
            await base.Drop_primary_key_int();

            AssertSql("");
        }

        [ConditionalFact(Skip = "For this to work, either MySqlMigrator needs to be involved, or the primary key related stored procedures need to be handled by MySqlMigrationsSqlGenerator instead. The later is probably the way to go. We should move the primary key related stored procedures to its own service, so ti can be potentially be customized by users.")]
        public override async Task Drop_primary_key_string()
        {
            await base.Drop_primary_key_string();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Drop_sequence()
        {
            await base.Drop_sequence();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Move_sequence()
        {
            await Test(
                builder => builder.HasSequence<int>("TestSequenceMove"),
                builder => builder.HasSequence<int>("TestSequenceMove", "TestSequenceSchema"),
                model =>
                {
                    var sequence = Assert.Single(model.Sequences);
                    // Assert.Equal("TestSequenceSchema", sequence.Schema);
                    // Assert.Equal("TestSequence", sequence.Name);
                    Assert.Equal("TestSequenceSchema_TestSequenceMove", sequence.Name);
                });

            AssertSql(
"""
ALTER TABLE `TestSequenceMove` RENAME `TestSequenceSchema_TestSequenceMove`;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        public override async Task Add_required_primitive_collection_with_custom_default_value_sql_to_existing_table()
        {
            // Classic/literal default values like `DEFAULT '[3, 2, 1]'` are not allowed for `json`, `blob` or `text` data types, but
            // default *expressions* like `DEFAULT ('[3, 2, 1]')` are.
            await base.Add_required_primitive_collection_with_custom_default_value_sql_to_existing_table_core("('[3, 2, 1]')");

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('[3, 2, 1]');
""");
        }

        public override async Task Add_required_primitve_collection_with_custom_default_value_sql_to_existing_table()
        {
            // Classic/literal default values like `DEFAULT '[3, 2, 1]'` are not allowed for `json`, `blob` or `text` data types, but
            // default *expressions* like `DEFAULT ('[3, 2, 1]')` are.
            await base.Add_required_primitve_collection_with_custom_default_value_sql_to_existing_table_core("('[3, 2, 1]')");

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('[3, 2, 1]');
""");
        }

        public override async Task Move_table()
        {
            await base.Move_table();

            AssertSql(
"""
ALTER TABLE `TestTable` RENAME `TestTable`;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Sequences))]
        public override async Task Rename_sequence()
        {
            if (OperatingSystem.IsWindows())
            {
                // On Windows, with `lower_case_table_names = 2`, renaming `TestSequence` to `testsequence` doesn't do anything, because
                // `TestSequence` is internally being transformed to lower case, before it is processes further.
                await Test(
                    builder => { },
                    builder => builder.HasSequence<int>("TestSequence"),
                    builder => builder.HasSequence<int>("testsequence2"),
                    builder => builder.RenameSequence(name: "TestSequence", newName: "testsequence2"),
                    model =>
                    {
                        var sequence = Assert.Single(model.Sequences);
                        Assert.Equal("testsequence2", sequence.Name);
                    });

               AssertSql(
"""
ALTER TABLE `TestSequence` RENAME `testsequence2`;
""");
            }
            else
            {
                await base.Rename_sequence();

               AssertSql(
"""
ALTER TABLE `TestSequence` RENAME `testsequence`;
""");
            }
        }

        [ConditionalFact(Skip = "For this to work, either MySqlMigrator needs to be involved, or the primary key related stored procedures need to be handled by MySqlMigrationsSqlGenerator instead. The later is probably the way to go. We should move the primary key related stored procedures to its own service, so ti can be potentially be customized by users.")]
        public override async Task Rename_table_with_primary_key()
        {
            await base.Rename_table_with_primary_key();

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
        public override async Task Add_column_with_computedSql(bool? stored)
        {
            await base.Add_column_with_computedSql(stored);

            var computedColumnTypeSql = stored == true ? " STORED" : "";

            AssertSql(
$"""
ALTER TABLE `People` ADD `Sum` longtext CHARACTER SET utf8mb4 AS (`X` + `Y`){computedColumnTypeSql} NULL;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
        public override async Task Create_table_with_computed_column(bool? stored)
        {
            await base.Create_table_with_computed_column(stored);

            var computedColumnTypeSql = stored == true ? " STORED" : "";

            AssertSql(
$"""
CREATE TABLE `People` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Sum` longtext CHARACTER SET utf8mb4 AS (`X` + `Y`){computedColumnTypeSql} NULL,
    `X` int NOT NULL,
    `Y` int NOT NULL,
    CONSTRAINT `PK_People` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
        public override async Task Alter_column_change_computed()
        {
            await base.Alter_column_change_computed();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Sum` int AS (`X` - `Y`);
""");
        }

        // We currently do not scaffold table options.
        //
        // [ConditionalFact]
        // public virtual async Task Create_table_with_table_options()
        // {
        //     await Test(
        //         builder => { },
        //         builder => builder.Entity(
        //             "IceCream", e =>
        //             {
        //                 e.Property<int>("IceCreamId");
        //                 e.HasTableOption("CHECKSUM", "1");
        //                 e.HasTableOption("MAX_ROWS", "100");
        //             }),
        //         model =>
        //         {
        //             var table = Assert.Single(model.Tables);
        //             var options = (IDictionary<string, string>)MySqlEntityTypeExtensions.DeserializeTableOptions(
        //                 table.FindAnnotation(MySqlAnnotationNames.StoreOptions)?.Value as string);
        //
        //             Assert.Contains("CHECKSUM", options);
        //             Assert.Equal("1", options["CHECKSUM"]);
        //
        //             Assert.Contains("MAX_ROWS", options);
        //             Assert.Equal("100", options["MAX_ROWS"]);
        //         });
        //
        //     AssertSql(@"");
        // }

        [ConditionalFact]
        public virtual async Task Add_columns_with_collations()
        {
            await Test(
                common => common
                    .UseCollation(DefaultCollation)
                    .Entity(
                        "IceCream",
                        e => e.Property<int>("IceCreamId")),
                source => { },
                target => target.Entity(
                    "IceCream", e =>
                    {
                        e.Property<string>("Name");
                        e.Property<string>("Brand")
                            .UseCollation(NonDefaultCollation);
                    }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));
                    var brandColumn = Assert.Single(table.Columns.Where(c => c.Name == "Brand"));

                    Assert.Null(nameColumn.Collation);
                    Assert.Equal(NonDefaultCollation, brandColumn.Collation);
                });

            AssertSql(
                $@"ALTER TABLE `IceCream` ADD `Brand` longtext COLLATE {NonDefaultCollation} NULL;",
                //
                $@"ALTER TABLE `IceCream` ADD `Name` longtext COLLATE {DefaultCollation} NULL;");
        }

        [ConditionalFact]
        public virtual async Task Create_table_NVARCHAR_UPPERCASE_column()
        {
            await Test(
                common => { },
                source => { },
                target => target.Entity(
                    "IceCream",
                    e =>
                    {
                        e.Property<int>("IceCreamId");
                        e.Property<string>("Name")
                            .HasColumnType("NVARCHAR") // UPPERCASE
                            .HasMaxLength(45);
                    }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));

                    Assert.True(nameColumn[MySqlAnnotationNames.CharSet] is "utf8mb3"
                        or "utf8");
                });

            AssertSql(
                @"CREATE TABLE `IceCream` (
    `IceCreamId` int NOT NULL AUTO_INCREMENT,
    `Name` NVARCHAR(45) NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) CHARACTER SET=utf8mb4;");
        }

        [ConditionalFact]
        public virtual async Task Add_guid_columns()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .UseCollation(DefaultCollation)
                    .Entity(
                        "IceCream",
                        e => e.Property<Guid>("IceCreamId")),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var iceCreamIdColumn = Assert.Single(table.Columns.Where(c => c.Name == "IceCreamId"));

                    Assert.Equal("ascii_general_ci", iceCreamIdColumn.Collation);
                });

            AssertSql(
                $@"ALTER DATABASE COLLATE {DefaultCollation};",
                //
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` char(36) COLLATE ascii_general_ci NOT NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) COLLATE={DefaultCollation};");
        }

        [ConditionalFact]
        public virtual async Task Add_guid_columns_with_collation()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .UseCollation(DefaultCollation)
                    .Entity(
                        "IceCream",
                        e => e.Property<Guid>("IceCreamId")
                            .UseCollation(NonDefaultCollation)),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var iceCreamIdColumn = Assert.Single(table.Columns.Where(c => c.Name == "IceCreamId"));

                    Assert.Equal(NonDefaultCollation, iceCreamIdColumn.Collation);
                });

            AssertSql(
                $@"ALTER DATABASE COLLATE {DefaultCollation};",
                //
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` char(36) COLLATE {NonDefaultCollation} NOT NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) COLLATE={DefaultCollation};");
        }

        [ConditionalFact]
        public virtual async Task Add_guid_columns_with_explicit_default_collation()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .UseCollation(DefaultCollation)
                    .UseGuidCollation(NonDefaultCollation)
                    .Entity(
                        "IceCream",
                        e => e.Property<Guid>("IceCreamId")),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var iceCreamIdColumn = Assert.Single(table.Columns.Where(c => c.Name == "IceCreamId"));

                    Assert.Equal(NonDefaultCollation, iceCreamIdColumn.Collation);
                });

            AssertSql(
                $@"ALTER DATABASE COLLATE {DefaultCollation};",
                //
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` char(36) COLLATE {NonDefaultCollation} NOT NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) COLLATE={DefaultCollation};");
        }

        [ConditionalFact]
        public virtual async Task Add_guid_columns_with_disabled_default_collation()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .UseCollation(DefaultCollation)
                    .UseGuidCollation(string.Empty)
                    .Entity(
                        "IceCream",
                        e => e.Property<Guid>("IceCreamId")),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var iceCreamIdColumn = Assert.Single(table.Columns.Where(c => c.Name == "IceCreamId"));

                    Assert.Null(iceCreamIdColumn.Collation);
                });

            AssertSql(
                $@"ALTER DATABASE COLLATE {DefaultCollation};",
                //
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` char(36) NOT NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) COLLATE={DefaultCollation};");
        }

        [ConditionalFact]
        public virtual async Task Alter_column_collations_with_delegation()
        {
            await Test(
                common => common
                    .UseCollation(DefaultCollation)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand");
                        }),
                source => source.Entity(
                    "IceCream", e =>
                    {
                        e.Property<string>("Brand")
                            .UseCollation(NonDefaultCollation);
                    }),
                target => target.Entity(
                    "IceCream", e =>
                    {
                        e.Property<string>("Name")
                            .UseCollation(NonDefaultCollation);
                    }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));
                    var brandColumn = Assert.Single(table.Columns.Where(c => c.Name == "Brand"));

                    Assert.Equal(NonDefaultCollation, nameColumn.Collation);
                    Assert.Null(brandColumn.Collation);
                });

            AssertSql(
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Name` longtext COLLATE {NonDefaultCollation} NULL;",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Brand` longtext COLLATE {DefaultCollation} NULL;");
        }

        [ConditionalFact]
        public virtual async Task Alter_column_collations_with_delegation2()
        {
            await Test(
                common => common
                    .UseCollation(DefaultCollation)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand");
                        }),
                source => source.Entity(
                    "IceCream", e =>
                    {
                        e.Property<string>("Brand")
                            .UseCollation(NonDefaultCollation);
                    }),
                target => target.Entity(
                    "IceCream", e =>
                    {
                        e.UseCollation(NonDefaultCollation2);
                        e.Property<string>("Name")
                            .UseCollation(NonDefaultCollation);
                    }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));
                    var brandColumn = Assert.Single(table.Columns.Where(c => c.Name == "Brand"));

                    Assert.Equal(NonDefaultCollation, nameColumn.Collation);
                    Assert.Null(brandColumn.Collation);
                });

            AssertSql(
                $"ALTER TABLE `IceCream` COLLATE={NonDefaultCollation2};",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Name` longtext COLLATE {NonDefaultCollation} NULL;",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Brand` longtext COLLATE {NonDefaultCollation2} NULL;");
        }

        [ConditionalFact]
        public virtual async Task Alter_column_collations_with_delegation_columns_only()
        {
            await Test(
                common => common
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand");
                        }),
                source => source
                    .UseCollation(DefaultCollation, DelegationModes.ApplyToColumns)
                    .Entity(
                        "IceCream", e =>
                        {
                            e.Property<string>("Brand")
                                .UseCollation(NonDefaultCollation);
                        }),
                target => target
                    .UseCollation(NonDefaultCollation2, DelegationModes.ApplyToColumns)
                    .Entity(
                        "IceCream", e =>
                        {
                            e.Property<string>("Name")
                                .UseCollation(NonDefaultCollation);
                        }),
                result => { });

            AssertSql(
                @"set @__pomelo_TableCharset = (
    SELECT `ccsa`.`CHARACTER_SET_NAME` as `TABLE_CHARACTER_SET`
    FROM `INFORMATION_SCHEMA`.`TABLES` as `t`
    LEFT JOIN `INFORMATION_SCHEMA`.`COLLATION_CHARACTER_SET_APPLICABILITY` as `ccsa` ON `ccsa`.`COLLATION_NAME` = `t`.`TABLE_COLLATION`
    WHERE `TABLE_SCHEMA` = SCHEMA() AND `TABLE_NAME` = 'IceCream' AND `TABLE_TYPE` IN ('BASE TABLE', 'VIEW'));

SET @__pomelo_SqlExpr = CONCAT('ALTER TABLE `IceCream` CHARACTER SET = ', @__pomelo_TableCharset, ';');
PREPARE __pomelo_SqlExprExecute FROM @__pomelo_SqlExpr;
EXECUTE __pomelo_SqlExprExecute;
DEALLOCATE PREPARE __pomelo_SqlExprExecute;",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Name` longtext COLLATE {NonDefaultCollation} NULL;",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Brand` longtext COLLATE {NonDefaultCollation2} NULL;");
        }

        [ConditionalFact]
        public virtual async Task Alter_column_collations_with_delegation_columns_only_with_inbetween_tableonly_collation()
        {
            await Test(
                common => common
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand");
                        }),
                source => source
                    .UseCollation(DefaultCollation, DelegationModes.ApplyToColumns)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<string>("Brand")
                                .UseCollation(NonDefaultCollation);
                        }),
                target => target
                    .UseCollation(NonDefaultCollation2, DelegationModes.ApplyToColumns)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.UseCollation(DefaultCollation, DelegationModes.ApplyToTables);
                            e.Property<string>("Name")
                                .UseCollation(NonDefaultCollation);
                        }),
                result => { });

            AssertSql(
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Name` longtext COLLATE {NonDefaultCollation} NULL;",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Brand` longtext COLLATE {NonDefaultCollation2} NULL;");
        }

        [ConditionalFact]
        public virtual void Upgrade_legacy_charset_to_annotation_charset_only_does_not_generate_alter_column_operations()
        {
            var context = MySqlTestHelpers.Instance.CreateContext();

            var sourceModel = context.GetService<IModelRuntimeInitializer>()
                .Initialize(
                    new ModelBuilder()
                        .Entity(
                            "IssueConsoleTemplate.IceCream", b =>
                            {
                                b.Property<int>("IceCreamId")
                                    .HasColumnType("int");

                                b.Property<string>("Name")
                                    .HasColumnType("longtext CHARACTER SET utf8mb4");

                                b.HasKey("IceCreamId");

                                b.ToTable("IceCreams");
                            })
                        .Model
                        .FinalizeModel(),
                    designTime: true,
                    validationLogger: null);

            var targetModel = context.GetService<IModelRuntimeInitializer>()
                .Initialize(
                    new ModelBuilder()
                        .Entity(
                            "IssueConsoleTemplate.IceCream", b =>
                            {
                                b.Property<int>("IceCreamId")
                                    .HasColumnType("int");

                                b.Property<string>("Name")
                                    .HasColumnType("longtext");

                                b.HasKey("IceCreamId");

                                b.ToTable("IceCreams");
                            })
                        .Model
                        .FinalizeModel(),
                    designTime: true,
                    validationLogger: null);

            var modelDiffer = context.GetService<IMigrationsModelDiffer>();

            var operations = modelDiffer.GetDifferences(
                sourceModel.GetRelationalModel(),
                targetModel.GetRelationalModel());

            Assert.Empty(operations);
        }

        [ConditionalFact]
        public virtual async Task Create_table_explicit_column_charset_takes_precedence_over_inherited_collation()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .UseCollation(DefaultCollation)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand")
                                .HasCharSet(NonDefaultCharSet);
                        }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));
                    var brandColumn = Assert.Single(table.Columns.Where(c => c.Name == "Brand"));

                    Assert.Null(nameColumn[MySqlAnnotationNames.CharSet]);
                    Assert.Null(nameColumn.Collation);
                    Assert.Equal(NonDefaultCharSet, brandColumn[MySqlAnnotationNames.CharSet]);
                    Assert.NotEqual(DefaultCollation, brandColumn.Collation);
                });

            AssertSql(
                $@"ALTER DATABASE COLLATE {DefaultCollation};",
                //
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` int NOT NULL AUTO_INCREMENT,
    `Brand` longtext CHARACTER SET {NonDefaultCharSet} NULL,
    `Name` longtext COLLATE {DefaultCollation} NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) COLLATE={DefaultCollation};");
        }

        [ConditionalFact]
        public virtual async Task Create_table_explicit_column_collation_takes_precedence_over_inherited_charset()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .HasCharSet(NonDefaultCharSet)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand")
                                .UseCollation(NonDefaultCollation2);
                        }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));
                    var brandColumn = Assert.Single(table.Columns.Where(c => c.Name == "Brand"));

                    Assert.Null(nameColumn[MySqlAnnotationNames.CharSet]);
                    Assert.Null(nameColumn.Collation);
                    Assert.NotEqual(NonDefaultCharSet, brandColumn[MySqlAnnotationNames.CharSet]);
                    Assert.Equal(NonDefaultCollation2, brandColumn.Collation);
                });

            AssertSql(
                $@"ALTER DATABASE CHARACTER SET {NonDefaultCharSet};",
                //
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` int NOT NULL AUTO_INCREMENT,
    `Brand` longtext COLLATE {NonDefaultCollation2} NULL,
    `Name` longtext CHARACTER SET {NonDefaultCharSet} NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) CHARACTER SET={NonDefaultCharSet};");
        }

        [ConditionalFact]
        public virtual async Task Create_table_longtext_column_with_string_length_and_legacy_charset_definition_in_column_type()
        {
            await Test(
                common => { },
                source => { },
                target => target
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name")
                                .HasColumnType($"longtext CHARACTER SET {NonDefaultCharSet}")
                                .HasMaxLength(2048);
                        }),
                result =>
                {
                    var table = Assert.Single(result.Tables);
                    var nameColumn = Assert.Single(table.Columns.Where(c => c.Name == "Name"));

                    Assert.Equal(NonDefaultCharSet, nameColumn[MySqlAnnotationNames.CharSet]);
                    Assert.Equal("longtext", nameColumn.StoreType);
                });

            AssertSql(
                $@"CREATE TABLE `IceCream` (
    `IceCreamId` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET {NonDefaultCharSet} NULL,
    CONSTRAINT `PK_IceCream` PRIMARY KEY (`IceCreamId`)
) CHARACTER SET=utf8mb4;");
        }

        [ConditionalFact]
        public virtual async Task Alter_column_charsets_using_delegated_charset_with_tableonly_charset_inbetween()
        {
            await Test(
                common => common
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<int>("IceCreamId");
                            e.Property<string>("Name");
                            e.Property<string>("Brand");
                        }),
                source => source
                    .HasCharSet(DefaultCharSet, DelegationModes.ApplyToColumns)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.Property<string>("Brand")
                                .HasCharSet(NonDefaultCharSet);
                        }),
                target => target
                    .HasCharSet(NonDefaultCharSet2, DelegationModes.ApplyToColumns)
                    .Entity(
                        "IceCream",
                        e =>
                        {
                            e.HasCharSet(DefaultCharSet, DelegationModes.ApplyToTables);
                            e.Property<string>("Name")
                                .HasCharSet(NonDefaultCharSet);
                        }),
                result => { });

            AssertSql(
                $@"ALTER TABLE `IceCream` CHARACTER SET={DefaultCharSet};",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Name` longtext CHARACTER SET {NonDefaultCharSet} NULL;",
                //
                $@"ALTER TABLE `IceCream` MODIFY COLUMN `Brand` longtext CHARACTER SET {NonDefaultCharSet2} NULL;");
        }

        [ConditionalFact]
        public virtual async Task Drop_unique_constraint_without_recreating_foreign_keys()
        {
            await Test(
                builder => builder
                    .Entity(
                        "Foo", e =>
                        {
                            e.Property<int>("FooPK");
                            e.Property<int>("FooAK");
                            e.Property<int>("BarFK");
                            e.HasKey("FooPK");
                            e.HasOne("Bar", "Bars")
                                .WithMany()
                                .HasForeignKey("BarFK");
                        })
                    .Entity(
                        "Bar", e =>
                        {
                            e.Property<int>("BarPK");
                            e.HasKey("BarPK");
                        }),
                builder => builder
                    .Entity(
                        "Foo", e => e.HasAlternateKey("FooAK")),
                builder => { },
                model => Assert.Empty(Assert.Single(model.Tables.Where(t => t.Name == "Foo"))?.UniqueConstraints));

            AssertSql(
                @"ALTER TABLE `Foo` DROP KEY `AK_Foo_FooAK`;");
        }

        [ConditionalFact]
        public virtual async Task Drop_unique_constraint_without_recreating_foreign_keys_MigrationBuilder()
        {
            await Test(
                builder => builder
                    .Entity(
                        "Foo", e =>
                        {
                            e.Property<int>("FooPK");
                            e.Property<int>("FooAK");
                            e.Property<int>("BarFK");
                            e.HasKey("FooPK");
                            e.HasOne("Bar", "Bars")
                                .WithMany()
                                .HasForeignKey("BarFK");
                        })
                    .Entity(
                        "Bar", e =>
                        {
                            e.Property<int>("BarPK");
                            e.HasKey("BarPK");
                        }),
                builder => builder
                    .Entity(
                        "Foo", e => e.HasAlternateKey("FooAK")),
                builder => { },
                migrationBuilder => migrationBuilder.DropUniqueConstraint("AK_Foo_FooAK", "Foo"),
                model => Assert.Empty(Assert.Single(model.Tables.Where(t => t.Name == "Foo"))?.UniqueConstraints));

            AssertSql(
                @"ALTER TABLE `Foo` DROP KEY `AK_Foo_FooAK`;");
        }

        [ConditionalFact]
        public virtual async Task Drop_unique_constraint_with_recreating_foreign_keys_MigrationBuilder()
        {
            await Test(
                builder => builder
                    .Entity(
                        "Foo", e =>
                        {
                            e.Property<int>("FooPK");
                            e.Property<int>("FooAK");
                            e.Property<int>("BarFK");
                            e.HasKey("FooPK");
                            e.HasOne("Bar", "Bars")
                                .WithMany()
                                .HasForeignKey("BarFK");
                        })
                    .Entity(
                        "Bar", e =>
                        {
                            e.Property<int>("BarPK");
                            e.HasKey("BarPK");
                        }),
                builder => builder
                    .Entity(
                        "Foo", e => e.HasAlternateKey("FooAK")),
                builder => { },
                migrationBuilder => migrationBuilder.DropUniqueConstraint("AK_Foo_FooAK", "Foo", recreateForeignKeys: true),
                model => Assert.Empty(Assert.Single(model.Tables.Where(t => t.Name == "Foo"))?.UniqueConstraints));

            AssertSql(
                @"ALTER TABLE `Foo` DROP FOREIGN KEY `FK_Foo_Bar_BarFK`;",
                //
                @"ALTER TABLE `Foo` DROP KEY `AK_Foo_FooAK`;",
                //
                @"ALTER TABLE `Foo` ADD CONSTRAINT `FK_Foo_Bar_BarFK` FOREIGN KEY (`BarFK`) REFERENCES `Bar` (`BarPK`) ON DELETE CASCADE;");
        }

        public override async Task Add_foreign_key()
        {
            await base.Add_foreign_key();

            AssertSql(
                @"CREATE INDEX `IX_Orders_CustomerId` ON `Orders` (`CustomerId`);",
                //
                @"ALTER TABLE `Orders` ADD CONSTRAINT `FK_Orders_Customers_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE CASCADE;");
        }

        public override Task Rename_table()
            => Test(
                builder => builder.Entity("People").Property<int>("Id"),
                builder => builder.Entity("People").ToTable("Persons").Property<int>("Id"),
                model =>
                {
                    var table = Assert.Single(model.Tables);
                    Assert.Equal("Persons", table.Name);
                },
                withConventions: false);

        public override async Task Create_table()
        {
            await base.Create_table();

            AssertSql(
"""
CREATE TABLE `People` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_People` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
""");
        }

        public override async Task Create_table_no_key()
        {
            await base.Create_table_no_key();

            AssertSql(
"""
CREATE TABLE `Anonymous` (
    `SomeColumn` int NOT NULL
) CHARACTER SET=utf8mb4;
""");
        }

        public override async Task Create_table_with_comments()
        {
            await base.Create_table_with_comments();

            AssertSql(
"""
CREATE TABLE `People` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL COMMENT 'Column comment',
    CONSTRAINT `PK_People` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COMMENT='Table comment';
""");
        }

        public override async Task Alter_table_add_comment()
        {
            await base.Alter_table_add_comment();

            AssertSql(
"""
ALTER TABLE `People` COMMENT 'Table comment';
""");
        }

        public override async Task Alter_table_change_comment()
        {
            await base.Alter_table_change_comment();

            AssertSql(
"""
ALTER TABLE `People` COMMENT 'Table comment2';
""");
        }

        public override async Task Alter_table_remove_comment()
        {
            await base.Alter_table_remove_comment();

            AssertSql(
"""
ALTER TABLE `People` COMMENT '';
""");
        }

        public override async Task Drop_table()
        {
            await base.Drop_table();

            AssertSql(
"""
DROP TABLE `People`;
""");
        }

        public override async Task Add_column_with_defaultValueSql_unspecified()
        {
            await base.Add_column_with_defaultValueSql_unspecified();

            AssertSql();
        }

        public override async Task Add_column_with_defaultValue_unspecified()
        {
            await base.Add_column_with_defaultValue_unspecified();

            AssertSql();
        }

        public override async Task Add_column_with_computedSql_unspecified()
        {
            await base.Add_column_with_computedSql_unspecified();

            AssertSql();
        }

        public override async Task Add_column_with_required()
        {
            await base.Add_column_with_required();

            AssertSql(
"""
ALTER TABLE `People` ADD `Name` longtext CHARACTER SET utf8mb4 NOT NULL;
""");
        }

        public override async Task Add_column_with_ansi()
        {
            await base.Add_column_with_ansi();

            AssertSql(
"""
ALTER TABLE `People` ADD `Name` longtext CHARACTER SET utf8mb4 NULL;
""");
        }

        public override async Task Add_column_with_max_length()
        {
            await base.Add_column_with_max_length();

            AssertSql(
"""
ALTER TABLE `People` ADD `Name` varchar(30) CHARACTER SET utf8mb4 NULL;
""");
        }

        public override async Task Add_column_with_unbounded_max_length()
        {
            await base.Add_column_with_unbounded_max_length();

            AssertSql(
"""
ALTER TABLE `People` ADD `Name` longtext CHARACTER SET utf8mb4 NULL;
""");
        }

        public override async Task Add_column_with_max_length_on_derived()
        {
            await base.Add_column_with_max_length_on_derived();

            AssertSql();
        }

        public override async Task Add_column_with_fixed_length()
        {
            await base.Add_column_with_fixed_length();

            AssertSql(
"""
ALTER TABLE `People` ADD `Name` char(100) CHARACTER SET utf8mb4 NULL;
""");
        }

        public override async Task Add_column_with_comment()
        {
            await base.Add_column_with_comment();

            AssertSql(
"""
ALTER TABLE `People` ADD `FullName` longtext CHARACTER SET utf8mb4 NULL COMMENT 'My comment';
""");
        }

        public override async Task Add_column_shared()
        {
            await base.Add_column_shared();

            AssertSql();
        }

        public override async Task Add_column_with_check_constraint()
        {
            await base.Add_column_with_check_constraint();

            AssertSql(
"""
ALTER TABLE `People` ADD `DriverLicense` int NOT NULL DEFAULT 0;
""",
                //
                """
ALTER TABLE `People` ADD CONSTRAINT `CK_People_Foo` CHECK (`DriverLicense` > 0);
""");
        }

        public override async Task Alter_column_make_required_with_null_data()
        {
            await base.Alter_column_make_required_with_null_data();

            AssertSql(
"""
UPDATE `People` SET `SomeColumn` = ''
WHERE `SomeColumn` IS NULL;
SELECT ROW_COUNT();
""",
                //
                """
ALTER TABLE `People` MODIFY COLUMN `SomeColumn` longtext CHARACTER SET utf8mb4 NOT NULL;
""");
        }

        public override async Task Alter_column_make_required_with_index()
        {
            await base.Alter_column_make_required_with_index();

            AssertSql(
"""
UPDATE `People` SET `SomeColumn` = ''
WHERE `SomeColumn` IS NULL;
SELECT ROW_COUNT();
""",
                //
                """
ALTER TABLE `People` MODIFY COLUMN `SomeColumn` varchar(255) CHARACTER SET utf8mb4 NOT NULL;
""");
        }

        public override async Task Alter_column_make_required_with_composite_index()
        {
            await base.Alter_column_make_required_with_composite_index();

            AssertSql(
"""
UPDATE `People` SET `FirstName` = ''
WHERE `FirstName` IS NULL;
SELECT ROW_COUNT();
""",
                //
                """
ALTER TABLE `People` MODIFY COLUMN `FirstName` varchar(255) CHARACTER SET utf8mb4 NOT NULL;
""");
        }

        public override async Task Alter_column_change_computed_recreates_indexes()
        {
            await base.Alter_column_change_computed_recreates_indexes();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Sum` int AS (`X` - `Y`);
""");
        }

        public override async Task Alter_column_add_comment()
        {
            await base.Alter_column_add_comment();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Id` int NOT NULL COMMENT 'Some comment' AUTO_INCREMENT;
""");
        }

        public override async Task Alter_computed_column_add_comment()
        {
            await base.Alter_computed_column_add_comment();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `SomeColumn` int AS (42) COMMENT 'Some comment';
""");
        }

        public override async Task Alter_column_change_comment()
        {
            await base.Alter_column_change_comment();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Id` int NOT NULL COMMENT 'Some comment2' AUTO_INCREMENT;
""");
        }

        public override async Task Alter_column_remove_comment()
        {
            await base.Alter_column_remove_comment();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Id` int NOT NULL AUTO_INCREMENT;
""");
        }

        public override async Task Alter_column_reset_collation()
        {
            await base.Alter_column_reset_collation();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `Name` longtext CHARACTER SET utf8mb4 NULL;
""");
        }

        public override async Task Drop_column()
        {
            await base.Drop_column();

            AssertSql(
"""
ALTER TABLE `People` DROP COLUMN `SomeColumn`;
""");
        }

        public override async Task Drop_column_computed_and_non_computed_with_dependency()
        {
            await base.Drop_column_computed_and_non_computed_with_dependency();

            AssertSql(
"""
ALTER TABLE `People` DROP COLUMN `Y`;
""",
                //
                """
ALTER TABLE `People` DROP COLUMN `X`;
""");
        }

        public override async Task Rename_column()
        {
            await base.Rename_column();

            AssertSql(
"""
ALTER TABLE `People` RENAME COLUMN `SomeColumn` TO `SomeOtherColumn`;
""");
        }

        public override async Task Create_index()
        {
            await base.Create_index();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `FirstName` varchar(255) CHARACTER SET utf8mb4 NULL;
""",
                //
                """
CREATE INDEX `IX_People_FirstName` ON `People` (`FirstName`);
""");
        }

        public override async Task Create_index_unique()
        {
            await base.Create_index_unique();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `LastName` varchar(255) CHARACTER SET utf8mb4 NULL;
""",
                //
                """
ALTER TABLE `People` MODIFY COLUMN `FirstName` varchar(255) CHARACTER SET utf8mb4 NULL;
""",
                //
                """
CREATE UNIQUE INDEX `IX_People_FirstName_LastName` ON `People` (`FirstName`, `LastName`);
""");
        }

        public override async Task Alter_index_make_unique()
        {
            await base.Alter_index_make_unique();

            AssertSql(
"""
ALTER TABLE `People` DROP INDEX `IX_People_X`;
""",
                //
                """
CREATE UNIQUE INDEX `IX_People_X` ON `People` (`X`);
""");
        }

        public override async Task Drop_index()
        {
            await base.Drop_index();

            AssertSql(
"""
ALTER TABLE `People` DROP INDEX `IX_People_SomeField`;
""");
        }

        public override async Task Add_primary_key_int()
        {
            await base.Add_primary_key_int();

            AssertSql(
"""
ALTER TABLE `People` MODIFY COLUMN `SomeField` int NOT NULL AUTO_INCREMENT,
ADD CONSTRAINT `PK_People` PRIMARY KEY (`SomeField`);
""");
        }

        public override async Task Add_foreign_key_with_name()
        {
            await base.Add_foreign_key_with_name();

            AssertSql(
"""
CREATE INDEX `IX_Orders_CustomerId` ON `Orders` (`CustomerId`);
""",
                //
                """
ALTER TABLE `Orders` ADD CONSTRAINT `FK_Foo` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE CASCADE;
""");
        }

        public override async Task Drop_foreign_key()
        {
            await base.Drop_foreign_key();

            AssertSql(
"""
ALTER TABLE `Orders` DROP FOREIGN KEY `FK_Orders_Customers_CustomerId`;
""",
                //
                """
ALTER TABLE `Orders` DROP INDEX `IX_Orders_CustomerId`;
""");
        }

        public override async Task Drop_unique_constraint()
        {
            await base.Drop_unique_constraint();

            AssertSql(
"""
ALTER TABLE `People` DROP KEY `AK_People_AlternateKeyColumn`;
""");
        }

        public override async Task Add_check_constraint_with_name()
        {
            await base.Add_check_constraint_with_name();

            AssertSql(
"""
ALTER TABLE `People` ADD CONSTRAINT `CK_People_Foo` CHECK (`DriverLicense` > 0);
""");
        }

        public override async Task InsertDataOperation()
        {
            await base.InsertDataOperation();

            AssertSql(
"""
INSERT INTO `Person` (`Id`, `Name`)
VALUES (1, 'Daenerys Targaryen'),
(2, 'John Snow'),
(3, 'Arya Stark'),
(4, 'Harry Strickland'),
(5, NULL);
""");
        }

        public override async Task DeleteDataOperation_simple_key()
        {
            await base.DeleteDataOperation_simple_key();

            AssertSql(
"""
DELETE FROM `Person`
WHERE `Id` = 2;
SELECT ROW_COUNT();
""");
        }

        public override async Task DeleteDataOperation_composite_key()
        {
            await base.DeleteDataOperation_composite_key();

            AssertSql(
"""
DELETE FROM `Person`
WHERE `AnotherId` = 12 AND `Id` = 2;
SELECT ROW_COUNT();
""");
        }

        public override async Task UpdateDataOperation_simple_key()
        {
            await base.UpdateDataOperation_simple_key();

            AssertSql(
"""
UPDATE `Person` SET `Name` = 'Another John Snow'
WHERE `Id` = 2;
SELECT ROW_COUNT();
""");
        }

        public override async Task UpdateDataOperation_composite_key()
        {
            await base.UpdateDataOperation_composite_key();

            AssertSql(
"""
UPDATE `Person` SET `Name` = 'Another John Snow'
WHERE `AnotherId` = 11 AND `Id` = 2;
SELECT ROW_COUNT();
""");
        }

        public override async Task UpdateDataOperation_multiple_columns()
        {
            await base.UpdateDataOperation_multiple_columns();

            AssertSql(
"""
UPDATE `Person` SET `Age` = 21, `Name` = 'Another John Snow'
WHERE `Id` = 2;
SELECT ROW_COUNT();
""");
        }

        public override async Task SqlOperation()
        {
            await base.SqlOperation();

            AssertSql(
"""
-- I <3 DDL
""");
        }

        public override async Task Create_table_with_complex_type_with_required_properties_on_derived_entity_in_TPH()
        {
            await base.Create_table_with_complex_type_with_required_properties_on_derived_entity_in_TPH();

            AssertSql(
"""
CREATE TABLE `Contacts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Discriminator` varchar(8) CHARACTER SET utf8mb4 NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Number` int NULL,
    `MyComplex_Prop` longtext NULL,
    `MyComplex_MyNestedComplex_Bar` datetime(6) NULL,
    `MyComplex_MyNestedComplex_Foo` int NULL,
    CONSTRAINT `PK_Contacts` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
""");
        }

        public override async Task Add_required_primitive_collection_to_existing_table()
        {
            await base.Add_required_primitive_collection_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL;
""");
        }

        public override async Task Add_required_primitive_collection_with_custom_default_value_to_existing_table()
        {
            await base.Add_required_primitive_collection_with_custom_default_value_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('[1,2,3]');
""");
        }

        public override async Task Add_required_primitive_collection_with_custom_converter_to_existing_table()
        {
            await base.Add_required_primitive_collection_with_custom_converter_to_existing_table();

            AssertSql();
        }

        public override async Task Add_required_primitive_collection_with_custom_converter_and_custom_default_value_to_existing_table()
        {
            await base.Add_required_primitive_collection_with_custom_converter_and_custom_default_value_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('some numbers');
""");
        }

        public override async Task Add_optional_primitive_collection_to_existing_table()
        {
            await base.Add_optional_primitive_collection_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NULL;
""");
        }

        public override async Task Create_table_with_required_primitive_collection()
        {
            await base.Create_table_with_required_primitive_collection();

            AssertSql(
"""
CREATE TABLE `Customers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Customers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
""");
        }

        public override async Task Create_table_with_optional_primitive_collection()
        {
            await base.Create_table_with_optional_primitive_collection();

            AssertSql(
"""
CREATE TABLE `Customers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Numbers` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Customers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
""");
        }

        public override async Task Add_required_primitve_collection_to_existing_table()
        {
            await base.Add_required_primitve_collection_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL;
""");
        }

        public override async Task Add_required_primitve_collection_with_custom_default_value_to_existing_table()
        {
            await base.Add_required_primitve_collection_with_custom_default_value_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('[1,2,3]');
""");
        }

        public override async Task Add_required_primitve_collection_with_custom_converter_to_existing_table()
        {
            await base.Add_required_primitve_collection_with_custom_converter_to_existing_table();

            AssertSql();
        }

        public override async Task Add_required_primitve_collection_with_custom_converter_and_custom_default_value_to_existing_table()
        {
            await base.Add_required_primitve_collection_with_custom_converter_and_custom_default_value_to_existing_table();

            AssertSql(
"""
ALTER TABLE `Customers` ADD `Numbers` longtext CHARACTER SET utf8mb4 NOT NULL DEFAULT ('some numbers');
""");
        }

        #region ToJson

        public override Task Create_table_with_json_column()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Create_table_with_json_column());

        public override Task Create_table_with_json_column_explicit_json_column_names()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Create_table_with_json_column_explicit_json_column_names());

        public override Task Rename_table_with_json_column()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Rename_table_with_json_column());

        public override Task Add_json_columns_to_existing_table()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Add_json_columns_to_existing_table());

        public override Task Convert_json_entities_to_regular_owned()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Convert_json_entities_to_regular_owned());

        public override Task Convert_regular_owned_entities_to_json()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Convert_regular_owned_entities_to_json());

        public override Task Convert_string_column_to_a_json_column_containing_reference()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Convert_string_column_to_a_json_column_containing_reference());

        public override Task Convert_string_column_to_a_json_column_containing_required_reference()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Convert_string_column_to_a_json_column_containing_required_reference());

        public override Task Convert_string_column_to_a_json_column_containing_collection()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Convert_string_column_to_a_json_column_containing_collection());

        public override Task Drop_json_columns_from_existing_table()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Drop_json_columns_from_existing_table());

        public override Task Rename_json_column()
            => Assert.ThrowsAsync<NullReferenceException>(() => base.Rename_json_column());

        #endregion ToJson

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

        // The constraint name for a primary key is always PRIMARY in MySQL.
        protected override bool AssertConstraintNames
            => false;

        // MySQL does not support the concept of schemas.
        protected override bool AssertSchemaNames
            => false;

        protected virtual string DefaultCollation => ((MySqlTestStore)Fixture.TestStore).DatabaseCollation;

        protected override string NonDefaultCollation
            => DefaultCollation == ((MySqlTestStore)Fixture.TestStore).ServerVersion.Value.DefaultUtf8CsCollation
                ? ((MySqlTestStore)Fixture.TestStore).ServerVersion.Value.DefaultUtf8CiCollation
                : ((MySqlTestStore)Fixture.TestStore).ServerVersion.Value.DefaultUtf8CsCollation;

        protected virtual string NonDefaultCollation2
            => "utf8mb4_german2_ci";

        protected virtual string DefaultCharSet => ((MySqlTestStore)Fixture.TestStore).DatabaseCharSet;
        protected virtual string NonDefaultCharSet => "latin1";
        protected virtual string NonDefaultCharSet2 => "ascii";

        protected virtual TestHelpers TestHelpers => MySqlTestHelpers.Instance;

        protected virtual Task Test(
            Action<ModelBuilder> buildCommonAction,
            Action<ModelBuilder> buildSourceAction,
            Action<ModelBuilder> buildTargetAction,
            Action<MigrationBuilder> migrationBuilderAction,
            Action<DatabaseModel> asserter,
            bool withConventions = true)
        {
            var services = TestHelpers.CreateContextServices();
            var modelRuntimeInitializer = services.GetRequiredService<IModelRuntimeInitializer>();

            // Build the source model, possibly with conventions
            var sourceModelBuilder = CreateModelBuilder(withConventions);
            buildCommonAction(sourceModelBuilder);
            buildSourceAction(sourceModelBuilder);
            var preSnapshotSourceModel = modelRuntimeInitializer.Initialize(
                (IModel)sourceModelBuilder.Model, designTime: true, validationLogger: null);

            // Round-trip the source model through a snapshot, compiling it and then extracting it back again.
            // This simulates the real-world migration flow and can expose errors in snapshot generation
            var migrationsCodeGenerator = Fixture.TestHelpers.CreateDesignServiceProvider().GetRequiredService<IMigrationsCodeGenerator>();
            var sourceModelSnapshot = migrationsCodeGenerator.GenerateSnapshot(
                modelSnapshotNamespace: null, typeof(DbContext), "MigrationsTestSnapshot", preSnapshotSourceModel);
            var sourceModel = BuildModelFromSnapshotSource(sourceModelSnapshot);

            // Build the target model, possibly with conventions
            var targetModelBuilder = CreateModelBuilder(withConventions);
            buildCommonAction(targetModelBuilder);
            buildTargetAction(targetModelBuilder);
            var targetModel = modelRuntimeInitializer.Initialize(
                (IModel)targetModelBuilder.Model, designTime: true, validationLogger: null);

            var migrationBuilder = new MigrationBuilder(null);
            migrationBuilderAction(migrationBuilder);

            return Test(sourceModel, targetModel, migrationBuilder.Operations, asserter);
        }

        private ModelBuilder CreateModelBuilder(bool withConventions)
            => withConventions ? Fixture.TestHelpers.CreateConventionBuilder() : new ModelBuilder(new ConventionSet());

        public class MigrationsMySqlFixture : MigrationsFixtureBase
        {
            protected override string StoreName
                => nameof(MigrationsMySqlTest);

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public override RelationalTestHelpers TestHelpers => MySqlTestHelpers.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new MySqlDbContextOptionsBuilder(builder)
                    .SchemaBehavior(MySqlSchemaBehavior.Ignore);
                return base.AddOptions(builder);
            }

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                => base.AddServices(serviceCollection)
                    .AddScoped<IDatabaseModelFactory, MySqlDatabaseModelFactory>();
        }
    }
}
