using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;
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

        protected override string Schema { get; } = null;

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
                var serverVersion = new ServerVersion();
                var columnSize = Math.Min(serverVersion.MaxKeyLength / (charSet.MaxBytesPerChar * 2), 255);
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
        public override void SqlOperation()
        {
            base.SqlOperation();

            Assert.Equal(
                "-- I <3 DDL" + EOL,
                Sql);
        }

        // The test data we're using is geographic but is represented in NTS as a GeometryCollection
        protected override string GetGeometryCollectionStoreType()
            => "geometry";

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
            [CanBeNull] CharSet charSet)
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

            var batch = services.GetRequiredService<IMigrationsSqlGenerator>().Generate(ResetSchema(operation), model, options);

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
