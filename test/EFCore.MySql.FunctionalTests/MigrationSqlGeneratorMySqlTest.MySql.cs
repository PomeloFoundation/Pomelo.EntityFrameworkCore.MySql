using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public partial class MigrationSqlGeneratorMySqlTest
    {
        [ConditionalFact]
        public virtual void DropUniqueConstraintOperation_temporarily_drops_foreign_keys()
        {
            // A foreign key might reuse the alternate key for its own purposes and prohibit its deletion,
            // if the foreign key columns are listed as the first columns and in the same order as in the foreign key (#678).
            // We therefore drop and later recreate all foreign keys to ensure, that no other dependencies on the
            // alternate key exist.
            Generate(
                modelBuilder => SetupModel(modelBuilder),
                new DropUniqueConstraintOperation
                {
                    Table = "Cars",
                    Name = "AK_Cars_LicensePlateNumber",
                });

            AssertSql(@"ALTER TABLE `Cars` DROP FOREIGN KEY `FK_Cars_LicensePlates_LicensePlateNumber`;

ALTER TABLE `Cars` DROP KEY `AK_Cars_LicensePlateNumber`;

ALTER TABLE `Cars` ADD CONSTRAINT `FK_Cars_LicensePlates_LicensePlateNumber` FOREIGN KEY (`LicensePlateNumber`) REFERENCES `LicensePlates` (`LicensePlateNumber`) ON DELETE CASCADE;
");
        }

        [ConditionalFact]
        public virtual void DropPrimaryKeyOperation_temporarily_drops_foreign_keys()
        {
            // A foreign key might reuse the primary key for its own purposes and prohibit its deletion,
            // if the foreign key columns are listed as the first columns and in the same order as in the foreign key (#678).
            // We therefore drop and later recreate all foreign keys to ensure, that no other dependencies on the
            // primary key exist.
            Generate(
                modelBuilder => SetupModel(modelBuilder),
                new DropPrimaryKeyOperation
                {
                    Table = "Cars",
                    Name = "PK_Cars_CarId",
                });

            AssertSql(@"ALTER TABLE `Cars` DROP FOREIGN KEY `FK_Cars_LicensePlates_LicensePlateNumber`;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Cars');
ALTER TABLE `Cars` DROP PRIMARY KEY;

ALTER TABLE `Cars` ADD CONSTRAINT `FK_Cars_LicensePlates_LicensePlateNumber` FOREIGN KEY (`LicensePlateNumber`) REFERENCES `LicensePlates` (`LicensePlateNumber`) ON DELETE CASCADE;
");
        }

        [ConditionalFact]
        public virtual void CreateTable_uses_srid()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "IceCreamShops",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Location",
                            ClrType = typeof(Point),
                            ColumnType = "GEOMETRY",
                            [MySqlAnnotationNames.SpatialReferenceSystemId] = 0,
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreamShops` (
    `Location` GEOMETRY NOT NULL /*!80003 SRID 0 */
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void CreateTable_uses_srid_geometry_derived()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "IceCreamShops",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Location",
                            ClrType = typeof(Point),
                            ColumnType = "POINT",
                            [MySqlAnnotationNames.SpatialReferenceSystemId] = 4326,
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreamShops` (
    `Location` POINT NOT NULL /*!80003 SRID 4326 */
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void CreateTable_MySqlSchemaBehavior_Throw()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                    Generate(
                        options => options.SchemaBehavior(MySqlSchemaBehavior.Throw),
                        buildAction: null,
                        resetSchema: false,
                        new CreateTableOperation
                        {
                            Name = "IceCreamShops",
                            Schema = "IceCreamInc",
                            Columns =
                            {
                                new AddColumnOperation
                                {
                                    Name = "Name",
                                    ClrType = typeof(string),
                                    ColumnType = "varchar(255)",
                                }
                            }
                        }));
        }

        [ConditionalFact]
        public virtual void CreateTable_MySqlSchemaBehavior_Ignore()
        {
            Generate(
                options => options.SchemaBehavior(MySqlSchemaBehavior.Ignore),
                buildAction: null,
                resetSchema: false,
                new CreateTableOperation
                {
                    Name = "IceCreamShops",
                    Schema = "IceCreamInc",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Name",
                            ClrType = typeof(string),
                            ColumnType = "varchar(255)",
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreamShops` (
    `Name` varchar(255) NOT NULL
);",
                Sql.Trim(),
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void CreateTable_MySqlSchemaBehavior_Translate()
        {
            Generate(
                options => options.SchemaBehavior(
                    MySqlSchemaBehavior.Translate,
                    (schemaName, objectName) => $"{schemaName}_{objectName}"),
                buildAction: null,
                resetSchema: false,
                new CreateTableOperation
                {
                    Name = "IceCreamShops",
                    Schema = "IceCreamInc",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Name",
                            ClrType = typeof(string),
                            ColumnType = "varchar(255)",
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreamInc_IceCreamShops` (
    `Name` varchar(255) NOT NULL
);",
                Sql.Trim(),
                ignoreLineEndingDifferences: true);
        }

        [ConditionalFact]
        public virtual void CreateTable_with_ValueGenerationStrategy_int_value()
        {
            Generate(
                new CreateTableOperation
                {
                    Name = "IceCreamShops",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "IceCreamShopId",
                            ClrType = typeof(int),
                            ColumnType = "int",
                            [MySqlAnnotationNames.ValueGenerationStrategy] = (int)MySqlValueGenerationStrategy.IdentityColumn,
                        }
                    }
                });

            Assert.Equal(
                @"CREATE TABLE `IceCreamShops` (
    `IceCreamShopId` int NOT NULL AUTO_INCREMENT
);
",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        private static void SetupModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("Cars");

                entity.HasKey(e => e.CarId);
                entity.HasAlternateKey(e => e.LicensePlateNumber);

                entity.Property(e => e.LicensePlateNumber)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.HasOne(e => e.LicensePlate)
                    .WithMany()
                    .HasForeignKey(e => e.LicensePlateNumber);
            });

            modelBuilder.Entity<LicensePlate>(entity =>
            {
                entity.ToTable("LicensePlates");

                entity.HasKey(e => e.LicensePlateNumber);

                entity.Property(e => e.LicensePlateNumber)
                    .HasMaxLength(255)
                    .IsRequired();
            });
        }

        protected class Car
        {
            public int CarId { get; set; }
            public string LicensePlateNumber { get; set; }

            public LicensePlate LicensePlate { get; set; }
        }

        protected class LicensePlate
        {
            public string LicensePlateNumber { get; set; }
        }
    }
}
