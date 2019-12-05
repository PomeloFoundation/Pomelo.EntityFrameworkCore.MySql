using System;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MigrationSqlGeneratorMySql55Test : MigrationSqlGeneratorMySqlTest
    {
        [ConditionalFact]
        public override void RenameIndexOperation()
        {
            Assert.Throws<InvalidOperationException>(() => base.RenameIndexOperation());
        }

        [ConditionalFact]
        public override void RenameIndexOperation_with_model()
        {
            Generate(
                modelBuilder => modelBuilder.Entity(
                    "Person",
                    x =>
                    {
                        x.Property<string>("FullName");
                        x.HasIndex("FullName");
                    }),
                new RenameIndexOperation
                {
                    Table = "Person",
                    Name = "IX_Person_Name",
                    NewName = "IX_Person_FullName"
                });

            Assert.Equal(
                @"ALTER TABLE `Person` DROP INDEX `IX_Person_Name`;" + EOL +
                EOL +
                @"CREATE INDEX `IX_Person_FullName` ON `Person` (`FullName`);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void AddColumnOperation_with_datetime6()
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
                "ALTER TABLE `People` ADD `Birthday` timestamp(6) NOT NULL DEFAULT '" + new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).ToString("yyyy-MM-dd HH:mm:ss") + "';" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void DefaultValue_formats_literal_correctly()
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
    `Event` TEXT NOT NULL DEFAULT '2015-04-12 17:05:00'
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public override void RenameColumnOperation()
        {
            var migrationBuilder = new MigrationBuilder("MySql");

            migrationBuilder.RenameColumn(
                    table: "Person",
                    name: "Name",
                    newName: "FullName")
                .Annotation(RelationalAnnotationNames.ColumnType, "VARCHAR(4000)");

            Generate(migrationBuilder.Operations.ToArray());

            Assert.Equal(
                "ALTER TABLE `Person` CHANGE `Name` `FullName` VARCHAR(4000);" + EOL,
                Sql);
        }

        [ConditionalFact]
        public override void RenameColumnOperation_with_model()
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
                "ALTER TABLE `Person` CHANGE `Name` `FullName` longtext CHARACTER SET utf8mb4 NULL;" + EOL,
                Sql);
        }

        protected override void Generate(Action<ModelBuilder> buildAction, params MigrationOperation[] operations)
        {
            var services = MySqlTestHelpers.Instance.CreateContextServices(new ServerVersion("5.5.2-mysql"));
            var modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder(services);
            buildAction(modelBuilder);

            var batch = services.GetRequiredService<IMigrationsSqlGenerator>()
                .Generate(ResetSchema(operations), modelBuilder.Model);

            Sql = string.Join(
                EOL,
                batch.Select(b => b.CommandText));
        }
    }
}
