using System;
using System.Collections.Generic;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;

// ReSharper disable InconsistentNaming
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Scaffolding
{
    public class MySqlDatabaseModelFactoryTest : IClassFixture<MySqlDatabaseModelFactoryTest.MySqlDatabaseModelFixture>
    {
        protected MySqlDatabaseModelFixture Fixture { get; }

        public MySqlDatabaseModelFactoryTest(MySqlDatabaseModelFixture fixture)
        {
            Fixture = fixture;
            Fixture.ListLoggerFactory.Clear();
        }

        protected readonly List<(LogLevel Level, EventId Id, string Message)> Log = new List<(LogLevel Level, EventId Id, string Message)>();

        private void Test(string createSql, IEnumerable<string> tables, IEnumerable<string> schemas, Action<DatabaseModel> asserter, string cleanupSql)
        {
            Fixture.TestStore.ExecuteNonQuery(createSql);

            try
            {
                var databaseModelFactory = new MySqlDatabaseModelFactory(
                    new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                        Fixture.ListLoggerFactory,
                        new LoggingOptions(),
                        new DiagnosticListener("Fake"),
                        new MySqlLoggingDefinitions(),
                        new NullDbContextLogger()),
                    Fixture.ServiceProvider.GetService<IMySqlOptions>());

                var databaseModel = databaseModelFactory.Create(Fixture.TestStore.ConnectionString,
                    new DatabaseModelFactoryOptions(tables, schemas));
                Assert.NotNull(databaseModel);
                asserter(databaseModel);
            }
            finally
            {
                if (!string.IsNullOrEmpty(cleanupSql))
                {
                    Fixture.TestStore.ExecuteNonQuery(cleanupSql);
                }
            }
        }

        #region FilteringSchemaTable

        [Fact(Skip = "Issue #582")]
        public void Filter_tables()
        {
            Test(
                @"
CREATE TABLE Everest ( id int );
CREATE TABLE Denali ( id int );",
                new[] { "Everest" },
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var table = Assert.Single(dbModel.Tables);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("Everest", table.Name);
                    },
                @"
DROP TABLE Everest;
DROP TABLE Denali;");
        }

        [Fact(Skip = "Issue #582")]
        public void Filter_tables_is_case_insensitive()
        {
            Test(
                @"
CREATE TABLE Everest ( id int );
CREATE TABLE Denali ( id int );",
                new[] { "eVeReSt" },
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var table = Assert.Single(dbModel.Tables);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("Everest", table.Name);
                    },
                @"
DROP TABLE Everest;
DROP TABLE Denali;");
        }

        #endregion

        #region Table

        [Fact(Skip = "Issue #582")]
        public void Create_tables()
        {
            Test(
                @"
CREATE TABLE Everest ( id int );
CREATE TABLE Denali ( id int );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        Assert.Collection(
                            dbModel.Tables.OrderBy(t => t.Name),
                            d => Assert.Equal("Denali", d.Name),
                            e => Assert.Equal("Everest", e.Name));
                    },
                @"
DROP TABLE Everest;
DROP TABLE Denali;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_columns()
        {
            Test(
                @"
CREATE TABLE MountainsColumns (
    Id integer primary key,
    Name text NOT NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var table = dbModel.Tables.Single();

                        Assert.Equal(2, table.Columns.Count);
                        Assert.All(
                            table.Columns, c => { Assert.Equal("MountainsColumns", c.Table.Name); });

                        Assert.Single(table.Columns.Where(c => c.Name == "Id"));
                        Assert.Single(table.Columns.Where(c => c.Name == "Name"));
                    },
                @"DROP TABLE MountainsColumns;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_primary_key()
        {
            Test(
                @"CREATE TABLE Place ( Id int PRIMARY KEY );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var pk = dbModel.Tables.Single().PrimaryKey;

                        Assert.Equal("Place", pk.Table.Name);
                        Assert.Equal(new List<string> { "Id" }, pk.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE Place;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_unique_constraints()
        {
            Test(
                @"
CREATE TABLE Place (
    Id int PRIMARY KEY,
    Name int UNIQUE,
    Location int
);

CREATE INDEX IX_Location_Name ON Place (Location, Name);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var uniqueConstraint = Assert.Single(dbModel.Tables.Single().UniqueConstraints);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("Place", uniqueConstraint.Table.Name);
                        Assert.Equal(new List<string> { "Name" }, uniqueConstraint.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE Place;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_indexes()
        {
            Test(
                @"
CREATE TABLE IndexTable (
    Id int,
    Name int,
    IndexProperty int
);

CREATE INDEX IX_NAME on IndexTable ( Name );
CREATE INDEX IX_INDEX on IndexTable ( IndexProperty );",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var table = dbModel.Tables.Single();

                        Assert.Equal(2, table.Indexes.Count);
                        Assert.All(
                            table.Indexes, c => { Assert.Equal("IndexTable", c.Table.Name); });

                        Assert.Single(table.Indexes.Where(c => c.Name == "IX_NAME"));
                        Assert.Single(table.Indexes.Where(c => c.Name == "IX_INDEX"));
                    },
                @"DROP TABLE IndexTable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_foreign_keys()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE FirstDependent (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE CASCADE
);

CREATE TABLE SecondDependent (
    Id int PRIMARY KEY,
    FOREIGN KEY (Id) REFERENCES PrincipalTable(Id) ON DELETE NO ACTION
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var firstFk = Assert.Single(dbModel.Tables.Single(t => t.Name == "FirstDependent").ForeignKeys);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("FirstDependent", firstFk.Table.Name);
                        Assert.Equal("PrincipalTable", firstFk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId" }, firstFk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id" }, firstFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.Cascade, firstFk.OnDelete);

                        var secondFk = Assert.Single(dbModel.Tables.Single(t => t.Name == "SecondDependent").ForeignKeys);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("SecondDependent", secondFk.Table.Name);
                        Assert.Equal("PrincipalTable", secondFk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "Id" }, secondFk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id" }, secondFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.NoAction, secondFk.OnDelete);
                    },
                @"
DROP TABLE SecondDependent;
DROP TABLE FirstDependent;
DROP TABLE PrincipalTable;");
        }

        #endregion

        #region ColumnFacets

        [Fact]
        public void Column_storetype_is_set()
        {
            Test(
                @"
CREATE TABLE StoreType (
    /* IntegerProperty int,
    RealProperty real,
    TextProperty text,
    BlobProperty blob,*/
    GeometryProperty geometry,
    PointProperty point/*,
    RandomProperty randomType*/
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var columns = dbModel.Tables.Single().Columns;

                        //Assert.Equal("integer", columns.Single(c => c.Name == "IntegerProperty").StoreType);
                        //Assert.Equal("real", columns.Single(c => c.Name == "RealProperty").StoreType);
                        //Assert.Equal("text", columns.Single(c => c.Name == "TextProperty").StoreType);
                        //Assert.Equal("blob", columns.Single(c => c.Name == "BlobProperty").StoreType);
                        Assert.Equal("geometry", columns.Single(c => c.Name == "GeometryProperty").StoreType);
                        Assert.Equal("point", columns.Single(c => c.Name == "PointProperty").StoreType);
                        //Assert.Equal("randomType", columns.Single(c => c.Name == "RandomProperty").StoreType);
                    },
                @"DROP TABLE StoreType;");
        }

        [Fact(Skip = "Issue #582")]
        public void Column_nullability_is_set()
        {
            Test(
                @"
CREATE TABLE Nullable (
    Id int,
    NullableInt int NULL,
    NonNullString text NOT NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var columns = dbModel.Tables.Single().Columns;

                        Assert.True(columns.Single(c => c.Name == "NullableInt").IsNullable);
                        Assert.False(columns.Single(c => c.Name == "NonNullString").IsNullable);
                    },
                @"DROP TABLE Nullable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Column_default_value_is_set()
        {
            Test(
                @"
CREATE TABLE DefaultValue (
    Id int,
    SomeText text DEFAULT 'Something',
    RealColumn real DEFAULT 3.14,
    Created datetime DEFAULT 'October 20, 2015 11am'
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var columns = dbModel.Tables.Single().Columns;

                        Assert.Equal("'Something'", columns.Single(c => c.Name == "SomeText").DefaultValueSql);
                        Assert.Equal("3.14", columns.Single(c => c.Name == "RealColumn").DefaultValueSql);
                        Assert.Equal("'October 20, 2015 11am'", columns.Single(c => c.Name == "Created").DefaultValueSql);
                    },
                @"DROP TABLE DefaultValue;");
        }

        [Theory(Skip = "Issue #582")]
        [InlineData("DOUBLE NOT NULL DEFAULT 0")]
        [InlineData("FLOAT NOT NULL DEFAULT 0")]
        [InlineData("INT NOT NULL DEFAULT 0")]
        [InlineData("INTEGER NOT NULL DEFAULT 0")]
        [InlineData("REAL NOT NULL DEFAULT 0")]
        [InlineData("NULL DEFAULT NULL")]
        [InlineData("NOT NULL DEFAULT NULL")]
        public void Column_default_value_is_ignored_when_clr_default(string columnSql)
        {
            Test(
                $"CREATE TABLE DefaultValueClr (IgnoredDefault {columnSql})",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var column = Assert.Single(Assert.Single(dbModel.Tables).Columns);
                    Assert.Null(column.DefaultValueSql);
                },
                "DROP TABLE DefaultValueClr");
        }

        #endregion

        #region PrimaryKeyFacets

        [Fact(Skip = "Issue #582")]
        public void Create_composite_primary_key()
        {
            Test(
                @"
CREATE TABLE CompositePrimaryKey (
    Id1 int,
    Id2 text,
    PRIMARY KEY ( Id2, Id1 )
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var pk = dbModel.Tables.Single().PrimaryKey;

                        Assert.Equal("CompositePrimaryKey", pk.Table.Name);
                        Assert.Equal(new List<string> { "Id2", "Id1" }, pk.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE CompositePrimaryKey;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_primary_key_when_integer_primary_key_alised_to_rowid()
        {
            Test(
                @"
CREATE TABLE RowidPrimaryKey (
    Id integer PRIMARY KEY
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var pk = dbModel.Tables.Single().PrimaryKey;

                        Assert.Equal("RowidPrimaryKey", pk.Table.Name);
                        Assert.Equal(new List<string> { "Id" }, pk.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE RowidPrimaryKey;");
        }

        [Fact(Skip = "Issue #582")]
        public void Set_name_for_primary_key()
        {
            Test(
                @"
CREATE TABLE PrimaryKeyName (
    Id int,
    CONSTRAINT PK PRIMARY KEY (Id)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("PrimaryKeyName", pk.Table.Name);
                    Assert.Equal("PK", pk.Name);
                    Assert.Equal(new List<string> { "Id" }, pk.Columns.Select(ic => ic.Name).ToList());
                },
                @"DROP TABLE PrimaryKeyName;");
        }

        [Fact]
        public void Prefix_lengths_for_primary_key()
        {
            Test(
                @"
CREATE TABLE `IceCreams` (
    `Brand` longtext NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`Name`, `Brand`(20))
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var pk = dbModel.Tables.Single().PrimaryKey;

                    Assert.Equal("IceCreams", pk.Table.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(2, pk.Columns.Count);
                    Assert.Equal("Name", pk.Columns[0].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("Brand", pk.Columns[1].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(new [] { 0, 20 }, pk.FindAnnotation(MySqlAnnotationNames.IndexPrefixLength)?.Value);
                },
                @"DROP TABLE IF EXISTS `IceCreams`;");
        }

        [Fact]
        public void Column_srid_value_is_set()
        {
            Test(
                @"
CREATE TABLE `IceCreamShop` (
    `IceCreamShopId` int NOT NULL,
    `Location` geometry NOT NULL /*!80003 SRID 0 */,
    PRIMARY KEY (`IceCreamShopId`)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var columns = dbModel.Tables.Single().Columns;

                    if (AppConfig.ServerVersion.Supports.SpatialReferenceSystemRestrictedColumns)
                    {
                        Assert.Equal(
                            0, columns.Single(c => c.Name == "Location")
                                .FindAnnotation(MySqlAnnotationNames.SpatialReferenceSystemId)
                                ?.Value);
                    }
                    else
                    {
                        Assert.Null(
                            columns.Single(c => c.Name == "Location")
                                .FindAnnotation(MySqlAnnotationNames.SpatialReferenceSystemId)
                                ?.Value);
                    }
                },
                @"DROP TABLE IF EXISTS `IceCreamShop`;");
        }

        #endregion

        #region UniqueConstraintFacets

        [Fact(Skip = "Issue #582")]
        public void Create_composite_unique_constraint()
        {
            Test(
                @"
CREATE TABLE CompositeUniqueConstraint (
    Id1 int,
    Id2 text,
    UNIQUE ( Id2, Id1 )
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var constraint = Assert.Single(dbModel.Tables.Single().UniqueConstraints);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("CompositeUniqueConstraint", constraint.Table.Name);
                        Assert.Equal(new List<string> { "Id2", "Id1" }, constraint.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE CompositeUniqueConstraint;");
        }

        [Fact(Skip = "Issue #582")]
        public void Set_name_for_unique_constraint()
        {
            Test(
                @"
CREATE TABLE UniqueConstraintName (
    Id int,
    CONSTRAINT UK UNIQUE (Id)
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var constraint = Assert.Single(dbModel.Tables.Single().UniqueConstraints);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("UniqueConstraintName", constraint.Table.Name);
                        Assert.Equal("UK", constraint.Name);
                        Assert.Equal(new List<string> { "Id" }, constraint.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE UniqueConstraintName;");
        }

        #endregion

        #region IndexFacets

        [Fact(Skip = "Issue #582")]
        public void Create_composite_index()
        {
            Test(
                @"
CREATE TABLE CompositeIndex (
    Id1 int,
    Id2 text
);

CREATE INDEX IX_COMPOSITE on CompositeIndex (Id2, Id1);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var index = Assert.Single(dbModel.Tables.Single().Indexes);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("CompositeIndex", index.Table.Name);
                        Assert.Equal("IX_COMPOSITE", index.Name);
                        Assert.Equal(new List<string> { "Id2", "Id1" }, index.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE CompositeIndex;");
        }

        [Fact(Skip = "Issue #582")]
        public void Set_unique_for_unique_index()
        {
            Test(
                @"
CREATE TABLE UniqueIndex (
    Id1 int,
    Id2 text
);

CREATE UNIQUE INDEX IX_UNIQUE on UniqueIndex (Id2);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var index = Assert.Single(dbModel.Tables.Single().Indexes);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("UniqueIndex", index.Table.Name);
                        Assert.Equal("IX_UNIQUE", index.Name);
                        Assert.True(index.IsUnique);
                        Assert.Equal(new List<string> { "Id2" }, index.Columns.Select(ic => ic.Name).ToList());
                    },
                @"DROP TABLE UniqueIndex;");
        }

        [Fact]
        public void Prefix_lengths_for_index()
        {
            Test(
                @"
CREATE TABLE `IceCreams` (
    `IceCreamId` int NOT NULL,
    `Brand` longtext NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`IceCreamId`)
);

CREATE INDEX `IX_IceCreams_Brand_Name` ON `IceCreams` (`Name`, `Brand`(20));
",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    Assert.Equal("IceCreams", index.Table.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("IX_IceCreams_Brand_Name", index.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(2, index.Columns.Count);
                    Assert.Equal("Name", index.Columns[0].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("Brand", index.Columns[1].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(new [] { 0, 20 }, index.FindAnnotation(MySqlAnnotationNames.IndexPrefixLength)?.Value);
                },
                @"DROP TABLE IF EXISTS `IceCreams`;");
        }

        [Fact]
        public void Prefix_lengths_for_multiple_indexes_same_colums()
        {
            Test(
                @"
CREATE TABLE `IceCreams` (
    `IceCreamId` int NOT NULL,
    `Brand` varchar(255) NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`IceCreamId`)
);

CREATE INDEX `IX_IceCreams_Brand_Name_1` ON `IceCreams` (`Name`, `Brand`(20));
CREATE UNIQUE INDEX `IX_IceCreams_Brand_Name_2` ON `IceCreams` (`Brand`(40), `Name`(120));
",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    Assert.Equal("IceCreams", index.Table.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("IX_IceCreams_Brand_Name_1", index.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.True(index.IsUnique);
                    Assert.Equal(2, index.Columns.Count);
                    Assert.Equal("Name", index.Columns[0].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("Brand", index.Columns[1].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(new [] { 0, 40 }, index[MySqlAnnotationNames.IndexPrefixLength]);
                },
                @"DROP TABLE IF EXISTS `IceCreams`;");
        }

        [Fact]
        public void Prefix_lengths_for_multiple_indexes_same_columns_without_prefix_lengths()
        {
            Test(
                @"
CREATE TABLE `IceCreams` (
    `IceCreamId` int NOT NULL,
    `Brand` varchar(255) NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`IceCreamId`)
);

CREATE INDEX `IX_IceCreams_Brand_Name_1` ON `IceCreams` (`Name`(120), `Brand`(20));
CREATE UNIQUE INDEX `IX_IceCreams_Brand_Name_2` ON `IceCreams` (`Brand`, `Name`);
",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    Assert.Equal("IceCreams", index.Table.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("IX_IceCreams_Brand_Name_1", index.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.True(index.IsUnique);
                    Assert.Equal(2, index.Columns.Count);
                    Assert.Equal("Name", index.Columns[0].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal("Brand", index.Columns[1].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Null(index[MySqlAnnotationNames.IndexPrefixLength]);
                },
                @"DROP TABLE IF EXISTS `IceCreams`;");
        }

        [Fact]
        public void Set_fulltext_for_fulltext_index()
        {
            Test(
                @"
CREATE TABLE `IceCreams` (
    `IceCreamId` int NOT NULL,
    `Name` varchar(255) NOT NULL,
    PRIMARY KEY (`IceCreamId`)
);

CREATE FULLTEXT INDEX `IX_IceCreams_Name` ON `IceCreams` (`Name`);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    Assert.Equal("IceCreams", index.Table.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(1, index.Columns.Count);
                    Assert.Equal("Name", index.Columns[0].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(true, index.FindAnnotation(MySqlAnnotationNames.FullTextIndex)?.Value);
                },
                @"DROP TABLE IF EXISTS `IceCreams`;");
        }

        [Fact]
        public void Set_spatial_for_spatial_index()
        {
            Test(
                @"
CREATE TABLE `IceCreamShop` (
    `IceCreamShopId` int NOT NULL,
    `Location` geometry NOT NULL /*!80003 SRID 0 */,
    PRIMARY KEY (`IceCreamShopId`)
);

CREATE SPATIAL INDEX `IX_IceCreams_Location` ON `IceCreamShop` (`Location`);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var index = Assert.Single(dbModel.Tables.Single().Indexes);

                    Assert.Equal("IceCreamShop", index.Table.Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(1, index.Columns.Count);
                    Assert.Equal("Location", index.Columns[0].Name, StringComparer.OrdinalIgnoreCase);
                    Assert.Equal(true, index.FindAnnotation(MySqlAnnotationNames.SpatialIndex)?.Value);
                },
                @"DROP TABLE IF EXISTS `IceCreamShop`;");
        }

        #endregion

        #region ForeignKeyFacets

        [Fact(Skip = "Issue #582")]
        public void Create_composite_foreign_key()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id1 int,
    Id2 int,
    PRIMARY KEY (Id1, Id2)
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId1 int,
    ForeignKeyId2 int,
    FOREIGN KEY (ForeignKeyId1, ForeignKeyId2) REFERENCES PrincipalTable(Id1, Id2) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "DependentTable").ForeignKeys);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("DependentTable", fk.Table.Name);
                        Assert.Equal("PrincipalTable", fk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId1", "ForeignKeyId2" }, fk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id1", "Id2" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_multiple_foreign_key_in_same_table()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE AnotherPrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId1 int,
    ForeignKeyId2 int,
    FOREIGN KEY (ForeignKeyId1) REFERENCES PrincipalTable(Id) ON DELETE CASCADE,
    FOREIGN KEY (ForeignKeyId2) REFERENCES AnotherPrincipalTable(Id) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var foreignKeys = dbModel.Tables.Single(t => t.Name == "DependentTable").ForeignKeys;

                        Assert.Equal(2, foreignKeys.Count);

                        var principalFk = Assert.Single(foreignKeys.Where(f => f.PrincipalTable.Name == "PrincipalTable"));

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("DependentTable", principalFk.Table.Name);
                        Assert.Equal("PrincipalTable", principalFk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId1" }, principalFk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id" }, principalFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.Cascade, principalFk.OnDelete);

                        var anotherPrincipalFk = Assert.Single(foreignKeys.Where(f => f.PrincipalTable.Name == "AnotherPrincipalTable"));

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("DependentTable", anotherPrincipalFk.Table.Name);
                        Assert.Equal("AnotherPrincipalTable", anotherPrincipalFk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId2" }, anotherPrincipalFk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id" }, anotherPrincipalFk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.Cascade, anotherPrincipalFk.OnDelete);
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE AnotherPrincipalTable;
DROP TABLE PrincipalTable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Create_foreign_key_referencing_unique_constraint()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id1 int,
    Id2 int UNIQUE
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id2) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "DependentTable").ForeignKeys);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("DependentTable", fk.Table.Name);
                        Assert.Equal("PrincipalTable", fk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId" }, fk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id2" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Set_name_for_foreign_key()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    CONSTRAINT MYFK FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "DependentTable").ForeignKeys);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("DependentTable", fk.Table.Name);
                        Assert.Equal("PrincipalTable", fk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId" }, fk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
                        Assert.Equal("MYFK", fk.Name);
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Set_referential_action_for_foreign_key()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE SET NULL
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var fk = Assert.Single(dbModel.Tables.Single(t => t.Name == "DependentTable").ForeignKeys);

                        // ReSharper disable once PossibleNullReferenceException
                        Assert.Equal("DependentTable", fk.Table.Name);
                        Assert.Equal("PrincipalTable", fk.PrincipalTable.Name);
                        Assert.Equal(new List<string> { "ForeignKeyId" }, fk.Columns.Select(ic => ic.Name).ToList());
                        Assert.Equal(new List<string> { "Id" }, fk.PrincipalColumns.Select(ic => ic.Name).ToList());
                        Assert.Equal(ReferentialAction.SetNull, fk.OnDelete);
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");
        }

        [Fact]
        public void Ensure_constraints_scaffold_with_case_mismatch()
        {
            // The lower case table reference to a mixed cased table will only be accepted under certain conditions
            // (lower_case_table_names <> 0).
            Test(
                @"
CREATE TABLE `PrincipalTable` (
  `Id` INT NOT NULL,
  PRIMARY KEY (`Id`));

set @sql = concat('
CREATE TABLE `DependentTable` (
  `Id` INT NOT NULL,
  `ForeignKeyId` INT NOT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `ForeignKey_Id`
    FOREIGN KEY (`ForeignKeyId`)
    REFERENCES `', IF(@@lower_case_table_names <> 0, LOWER('PrincipalTable'), 'PrincipalTable'), '` (`Id`)
)');

PREPARE dynamic_statement FROM @sql;
EXECUTE dynamic_statement;
DEALLOCATE PREPARE dynamic_statement;",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                {
                    var principal = dbModel.Tables.FirstOrDefault(t => string.Equals(t.Name, "PrincipalTable", StringComparison.OrdinalIgnoreCase));
                    var dependent = dbModel.Tables.FirstOrDefault(t => string.Equals(t.Name, "DependentTable", StringComparison.OrdinalIgnoreCase));

                    Assert.NotNull(principal);
                    Assert.NotNull(dependent);

                    Assert.Contains(dependent.ForeignKeys, t => t.PrincipalTable.Name == principal.Name);
                },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;"
            );
        }

        #endregion

        #region Warnings

        [Fact(Skip = "Issue #582")]
        public void Warn_for_schema_filtering()
        {
            Test(
                @"CREATE TABLE Everest ( id int );",
                Enumerable.Empty<string>(),
                new[] { "dbo" },
                dbModel =>
                    {
                        var (Level, Id, Message) = Assert.Single(Log.Where(t => t.Level == LogLevel.Warning));
                    },
                @"DROP TABLE Everest;");
        }

        [Fact(Skip = "Issue #582")]
        public void Warn_missing_table()
        {
            Test(
                @"CREATE TABLE Blank ( Id int );",
                new[] { "MyTable" },
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        Assert.Empty(dbModel.Tables);

                        var (Level, Id, Message) = Assert.Single(Log.Where(t => t.Level == LogLevel.Warning));
                    },
                @"DROP TABLE Blank;");
        }

        [Fact(Skip = "Issue #582")]
        public void Warn_missing_principal_table_for_foreign_key()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    CONSTRAINT MYFK FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(Id) ON DELETE CASCADE
);",
                new[] { "DependentTable" },
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var (Level, Id, Message) = Assert.Single(Log.Where(t => t.Level == LogLevel.Warning));
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");
        }

        [Fact(Skip = "Issue #582")]
        public void Warn_missing_principal_column_for_foreign_key()
        {
            Test(
                @"
CREATE TABLE PrincipalTable (
    Id int PRIMARY KEY
);

CREATE TABLE DependentTable (
    Id int PRIMARY KEY,
    ForeignKeyId int,
    CONSTRAINT MYFK FOREIGN KEY (ForeignKeyId) REFERENCES PrincipalTable(ImaginaryId) ON DELETE CASCADE
);",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                dbModel =>
                    {
                        var (Level, Id, Message) = Assert.Single(Log.Where(t => t.Level == LogLevel.Warning));
                    },
                @"
DROP TABLE DependentTable;
DROP TABLE PrincipalTable;");
        }

        #endregion

        public class MySqlDatabaseModelFixture : SharedStoreFixtureBase<PoolableDbContext>
        {
            protected override string StoreName { get; } = nameof(MySqlDatabaseModelFactoryTest);
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public new MySqlTestStore TestStore => (MySqlTestStore)base.TestStore;

            protected override bool ShouldLogCategory(string logCategory)
                => logCategory == DbLoggerCategory.Scaffolding.Name;
        }
    }
}
