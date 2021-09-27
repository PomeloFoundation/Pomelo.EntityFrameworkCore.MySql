using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
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

        [ConditionalTheory(Skip = "TODO: Syntax issue in MySQL 7 only.")]
        public override Task Alter_check_constraint()
        {
            return base.Alter_check_constraint();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_make_computed(bool? stored)
        {
            return base.Alter_column_make_computed(stored);
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_computed_with_collation()
        {
            return base.Add_column_computed_with_collation();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_with_collation()
        {
            return base.Add_column_with_collation();
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

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_primary_key()
        {
            return base.Add_primary_key();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_primary_key_composite_with_name()
        {
            return base.Add_primary_key_composite_with_name();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_primary_key_with_name()
        {
            return base.Add_primary_key_with_name();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_unique_constraint()
        {
            return base.Add_unique_constraint();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_unique_constraint_composite_with_name()
        {
            return base.Add_unique_constraint_composite_with_name();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_change_computed_type()
        {
            return base.Alter_column_change_computed_type();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_change_type()
        {
            return base.Alter_column_change_type();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_set_collation()
        {
            return base.Alter_column_set_collation();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_sequence_all_settings()
        {
            return base.Alter_sequence_all_settings();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_sequence_increment_by()
        {
            return base.Alter_sequence_increment_by();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_table_add_comment_non_default_schema()
        {
            return base.Alter_table_add_comment_non_default_schema();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_index_with_filter()
        {
            return base.Create_index_with_filter();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_schema()
        {
            return base.Create_schema();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_sequence()
        {
            return base.Create_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_sequence_all_settings()
        {
            return base.Create_sequence_all_settings();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_table_all_settings()
        {
            return base.Create_table_all_settings();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_table_with_multiline_comments()
        {
            return base.Create_table_with_multiline_comments();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_unique_index_with_filter()
        {
            return base.Create_unique_index_with_filter();
        }

        [ConditionalTheory(Skip = "TODO: Syntax issue in MySQL 7 only.")]
        public override Task Drop_check_constraint()
        {
            return base.Drop_check_constraint();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Drop_column_primary_key()
        {
            return base.Drop_column_primary_key();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Drop_primary_key()
        {
            return base.Drop_primary_key();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Drop_sequence()
        {
            return base.Drop_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Move_sequence()
        {
            return base.Move_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Move_table()
        {
            return base.Move_table();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Rename_sequence()
        {
            return base.Rename_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Rename_table_with_primary_key()
        {
            return base.Rename_table_with_primary_key();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
        public override Task Add_column_with_computedSql(bool? stored)
            => base.Add_column_with_computedSql(stored);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
        public override Task Create_table_with_computed_column(bool? stored)
            => base.Create_table_with_computed_column(stored);

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
        public override Task Alter_column_change_computed()
            => base.Alter_column_change_computed();

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
    `IceCreamId` char(36) COLLATE ascii_general_ci NOT NULL
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
    `IceCreamId` char(36) COLLATE {NonDefaultCollation} NOT NULL
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
    `IceCreamId` char(36) COLLATE {NonDefaultCollation} NOT NULL
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
    `IceCreamId` char(36) NOT NULL
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
                $@"ALTER TABLE `IceCream` COLLATE={DefaultCollation};",
                //
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
    `Brand` longtext CHARACTER SET {NonDefaultCharSet} NULL,
    `IceCreamId` int NOT NULL,
    `Name` longtext COLLATE {DefaultCollation} NULL
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
    `Brand` longtext COLLATE {NonDefaultCollation2} NULL,
    `IceCreamId` int NOT NULL,
    `Name` longtext CHARACTER SET {NonDefaultCharSet} NULL
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
    `IceCreamId` int NOT NULL,
    `Name` longtext CHARACTER SET {NonDefaultCharSet} NULL
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
                @"ALTER TABLE `Foo` ADD CONSTRAINT `FK_Foo_Bar_BarFK` FOREIGN KEY (`BarFK`) REFERENCES `Bar` (`BarPK`) ON DELETE RESTRICT;");
        }

        protected virtual string DefaultCollation => ((MySqlTestStore)Fixture.TestStore).DatabaseCollation;

        protected override string NonDefaultCollation
            => DefaultCollation == ((MySqlTestStore)Fixture.TestStore).GetCaseSensitiveUtf8Mb4Collation()
                ? ((MySqlTestStore)Fixture.TestStore).GetCaseInsensitiveUtf8Mb4Collation()
                : ((MySqlTestStore)Fixture.TestStore).GetCaseSensitiveUtf8Mb4Collation();

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
            Action<DatabaseModel> asserter)
        {
            var services = TestHelpers.CreateContextServices();

            // Build the source and target models. Add current/latest product version if one wasn't set.
            var sourceModelBuilder = CreateConventionlessModelBuilder();
            buildCommonAction(sourceModelBuilder);
            buildSourceAction(sourceModelBuilder);
            var sourceModel = services.GetRequiredService<IModelRuntimeInitializer>()
                .Initialize(sourceModelBuilder.FinalizeModel(), designTime: true, validationLogger: null);

            var targetModelBuilder = CreateConventionlessModelBuilder();
            buildCommonAction(targetModelBuilder);
            buildTargetAction(targetModelBuilder);
            var targetModel = services.GetRequiredService<IModelRuntimeInitializer>()
                .Initialize(targetModelBuilder.FinalizeModel(), designTime: true, validationLogger: null);

            var migrationBuilder = new MigrationBuilder(null);
            migrationBuilderAction(migrationBuilder);

            return Test(sourceModel, targetModel, migrationBuilder.Operations, asserter);
        }

        public class MigrationsMySqlFixture : MigrationsFixtureBase
        {
            protected override string StoreName { get; } = nameof(MigrationsMySqlTest);
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public override TestHelpers TestHelpers => MySqlTestHelpers.Instance;

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                => base.AddServices(serviceCollection)
                    .AddScoped<IDatabaseModelFactory, MySqlDatabaseModelFactory>();
        }
    }
}
