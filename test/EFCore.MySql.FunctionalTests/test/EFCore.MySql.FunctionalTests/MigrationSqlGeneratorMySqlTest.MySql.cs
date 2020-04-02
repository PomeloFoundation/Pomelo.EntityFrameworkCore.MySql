﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
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
