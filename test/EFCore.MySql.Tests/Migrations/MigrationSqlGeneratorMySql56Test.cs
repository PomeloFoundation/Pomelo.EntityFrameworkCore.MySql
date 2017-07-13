using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Xunit;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Migrations
{
    public class MigrationSqlGeneratorMySql56Test : MigrationSqlGeneratorTestBase
    {
        protected override IMigrationsSqlGenerator SqlGenerator
        {
            get
            {
                // type mapper
                var typeMapper = new MySqlTypeMapper(new RelationalTypeMapperDependencies());

                // migrationsSqlGeneratorDependencies
                var commandBuilderFactory = new RelationalCommandBuilderFactory(
                    new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>(),
                    typeMapper);
                var migrationsSqlGeneratorDependencies = new MigrationsSqlGeneratorDependencies(
                    commandBuilderFactory,
                    new MySqlSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
                    typeMapper);

                var mySqlOptions = new Mock<IMySqlOptions>();
                mySqlOptions.SetupGet(opts => opts.ConnectionSettings).Returns(
                    new MySqlConnectionSettings(new MySqlConnectionStringBuilder(), new ServerVersion("5.6.2")));
                mySqlOptions
                    .Setup(fn =>
                        fn.GetCreateTable(It.IsAny<ISqlGenerationHelper>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(@"
CREATE TABLE `People` (
 `Id` int(11) NOT NULL AUTO_INCREMENT,
 `Discriminator` varchar(63) NOT NULL,
 `FamilyId` int(11) DEFAULT NULL,
 `Name` longtext,
 `TeacherId` int(11) DEFAULT NULL,
 `Grade` int(11) DEFAULT NULL,
 `Occupation` longtext,
 `OnPta` bit(1) DEFAULT NULL,
 PRIMARY KEY (`Id`),
 KEY `IX_People_FamilyId` (`FamilyId`),
 KEY `IX_People_Discriminator` (`Discriminator`),
 KEY `IX_People_TeacherId` (`TeacherId`),
 CONSTRAINT `FK_People_PeopleFamilies_FamilyId` FOREIGN KEY (`FamilyId`) REFERENCES `PeopleFamilies` (`Id`) ON DELETE NO ACTION,
 CONSTRAINT `FK_People_People_TeacherId` FOREIGN KEY (`TeacherId`) REFERENCES `People` (`Id`) ON DELETE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1
");
                
                return new MySqlMigrationsSqlGenerator(
                    migrationsSqlGeneratorDependencies,
                    mySqlOptions.Object);
            }
        }

        private static FakeRelationalConnection CreateConnection(IDbContextOptions options = null)
            => new FakeRelationalConnection(options ?? CreateOptions());

        private static IDbContextOptions CreateOptions(RelationalOptionsExtension optionsExtension = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(optionsExtension
                                      ?? new FakeRelationalOptionsExtension().WithConnectionString("test"));

            return optionsBuilder.Options;
        }


        public override void RenameIndexOperation_works()
        {
            base.RenameIndexOperation_works();
            
            Assert.Equal("ALTER TABLE `People` DROP INDEX `IX_People_Discriminator`;" + EOL 
                         + "ALTER TABLE `People` ADD KEY `IX_People_DiscriminatorNew` (`Discriminator`);" + EOL,
                Sql);
        }
    }
}
