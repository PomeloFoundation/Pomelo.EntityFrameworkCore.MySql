using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
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

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_with_defaultValue_string()
        {
            return base.Add_column_with_defaultValue_string();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_with_defaultValueSql()
        {
            return base.Add_column_with_defaultValueSql();
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
        public virtual async Task Drop_primary_key_without_recreating_foreign_keys()
        {
            await Test(
                builder => builder
                    .Entity(
                        "Foo", e =>
                        {
                            e.Property<int>("FooPK");
                            e.Property<int>("BarFK");
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
                        "Foo", e => e.HasKey("FooPK")),
                builder => { },
                model => Assert.Null(Assert.Single(model.Tables.Where(t => t.Name == "Foo"))?.PrimaryKey));

            AssertSql(
                @"DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;

CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
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
END;",
                //
                @"CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Foo');
ALTER TABLE `Foo` DROP PRIMARY KEY;",
                //
                @"DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;");
        }

        [ConditionalFact]
        public virtual async Task Drop_primary_key_without_recreating_foreign_keys_MigrationBuilder()
        {
            await Test(
                builder => builder
                    .Entity(
                        "Foo", e =>
                        {
                            e.Property<int>("FooPK");
                            e.Property<int>("BarFK");
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
                        "Foo", e => e.HasKey("FooPK")),
                builder => { },
                migrationBuilder => migrationBuilder.DropPrimaryKey("PRIMARY", "Foo"),
                model => Assert.Null(Assert.Single(model.Tables.Where(t => t.Name == "Foo"))?.PrimaryKey));

            AssertSql(
                @"DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;

CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
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
END;",
                //
                @"CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Foo');
ALTER TABLE `Foo` DROP PRIMARY KEY;",
                //
                @"DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;");
        }

        [ConditionalFact]
        public virtual async Task Drop_primary_key_with_recreating_foreign_keys_MigrationBuilder()
        {
            await Test(
                builder => builder
                    .Entity(
                        "Foo", e =>
                        {
                            e.Property<int>("FooPK");
                            e.Property<int>("BarFK");
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
                        "Foo", e => e.HasKey("FooPK")),
                builder => { },
                migrationBuilder => migrationBuilder.DropPrimaryKey("PRIMARY", "Foo", recreateForeignKeys: true),
                model => Assert.Null(Assert.Single(model.Tables.Where(t => t.Name == "Foo"))?.PrimaryKey));

            AssertSql(
                @"DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;

CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
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
END;",
                //
                @"ALTER TABLE `Foo` DROP FOREIGN KEY `FK_Foo_Bar_BarFK`;",
                //
                @"CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Foo');
ALTER TABLE `Foo` DROP PRIMARY KEY;",
                //
                @"ALTER TABLE `Foo` ADD CONSTRAINT `FK_Foo_Bar_BarFK` FOREIGN KEY (`BarFK`) REFERENCES `Bar` (`BarPK`) ON DELETE RESTRICT;",
                //
                @"DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;");
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
