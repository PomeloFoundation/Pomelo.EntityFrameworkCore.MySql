// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ReSharper disable InconsistentNaming

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Scaffolding;

// TODO: Add more Pomelo specific cases.
public class CompiledModelMySqlTest : CompiledModelRelationalTestBase
{
    protected override void BuildBigModel(ModelBuilder modelBuilder, bool jsonColumns)
    {
        base.BuildBigModel(modelBuilder, jsonColumns);

        modelBuilder.HasCharSet("latin1");
        modelBuilder.UseCollation("latin1_general_ci");

        modelBuilder.Entity<Data>(
            eb =>
            {
                eb.HasCharSet("cp1250");

                eb.Property<int>("Id");
                eb.HasKey("Id");

                eb.Property<Point>("Point")
                    .HasSpatialReferenceSystem(4326);

                eb.Property<string>("StringWithCharSet")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("String having charset")
                    .HasCharSet("ascii")
                    .HasConversion<CastingConverter<string, string>, CustomValueComparer<string>, CustomValueComparer<string>>();

                eb.Property<string>("StringWithCollation")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("String using collation")
                    .UseCollation("cp1250_general_ci")
                    .HasConversion<CastingConverter<string, string>, CustomValueComparer<string>, CustomValueComparer<string>>();
            });

        modelBuilder.Entity<PrincipalBase>(
            eb =>
            {
                eb.UseCollation("ascii_general_ci");

                eb.Property<string>("StringWithCharSet")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("String having charset")
                    .HasCharSet("ascii")
                    .HasConversion<CastingConverter<string, string>, CustomValueComparer<string>, CustomValueComparer<string>>();

                eb.Property<string>("StringWithCollation")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("String using collation")
                    .UseCollation("cp1250_general_ci")
                    .HasConversion<CastingConverter<string, string>, CustomValueComparer<string>, CustomValueComparer<string>>();

                eb.Property<Point>("Point")
                    .HasColumnType("geometry")
                    .HasDefaultValue(
                        NtsGeometryServices.Instance.CreateGeometryFactory(srid: 0).CreatePoint(new CoordinateZM(0, 0, 0, 0)))
                    .HasConversion<CastingConverter<Point, Point>, CustomValueComparer<Point>, CustomValueComparer<Point>>();
            });
    }

    protected override void AssertBigModel(IModel model, bool jsonColumns)
    {
        base.AssertBigModel(model, jsonColumns);

        Assert.Equal(
            CoreStrings.RuntimeModelMissingData,
            Assert.Throws<InvalidOperationException>(model.GetCharSet).Message);
        Assert.Equal(
            CoreStrings.RuntimeModelMissingData,
            Assert.Throws<InvalidOperationException>(() => model.GetCharSetDelegation()).Message);
        Assert.Equal(
            CoreStrings.RuntimeModelMissingData,
            Assert.Throws<InvalidOperationException>(model.GetCollation).Message);
        Assert.Equal(
            CoreStrings.RuntimeModelMissingData,
            Assert.Throws<InvalidOperationException>(() => model.GetCollationDelegation()).Message);

        //
        // Data:
        //
        {
            var dataEntity = model.FindEntityType(typeof(Data))!;
            Assert.Equal(typeof(Data).FullName, dataEntity.Name);
            Assert.False(dataEntity.HasSharedClrType);
            Assert.False(dataEntity.IsPropertyBag);
            Assert.False(dataEntity.IsOwned());
            Assert.IsType<ConstructorBinding>(dataEntity.ConstructorBinding);
            Assert.Null(dataEntity.FindIndexerPropertyInfo());
            Assert.Equal(ChangeTrackingStrategy.Snapshot, dataEntity.GetChangeTrackingStrategy());
            Assert.Equal("Data", dataEntity.GetTableName());
            Assert.Null(dataEntity.GetSchema());
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(dataEntity.GetCharSet).Message);
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(() => dataEntity.GetCharSetDelegation()).Message);

            var pointProperty = dataEntity.FindProperty("Point")!;
            Assert.Equal(typeof(Point), pointProperty.ClrType);
            Assert.True(pointProperty.IsNullable);
            Assert.Equal(ValueGenerated.Never, pointProperty.ValueGenerated);
            Assert.Equal("Point", pointProperty.GetColumnName());
            Assert.Equal("point", pointProperty.GetColumnType());
            Assert.Null(pointProperty.GetValueConverter());
            Assert.IsType<GeometryValueComparer<Point>>(pointProperty.GetValueComparer());
            Assert.IsType<GeometryValueComparer<Point>>(pointProperty.GetKeyValueComparer());
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(() => pointProperty.GetSpatialReferenceSystem()).Message);

            var stringWithCharSetProperty = dataEntity.FindProperty("StringWithCharSet")!;
            Assert.Equal(typeof(string), stringWithCharSetProperty.ClrType);
            Assert.True(stringWithCharSetProperty.IsNullable);
            Assert.Equal(ValueGenerated.OnAdd, stringWithCharSetProperty.ValueGenerated);
            Assert.Equal("StringWithCharSet", stringWithCharSetProperty.GetColumnName());
            Assert.Equal("varchar(128)", stringWithCharSetProperty.GetColumnType());
            Assert.Equal("String having charset", (string?)stringWithCharSetProperty.GetDefaultValue());
            Assert.IsType<CastingConverter<string, string>>(stringWithCharSetProperty.GetValueConverter());
            Assert.IsType<CustomValueComparer<string>>(stringWithCharSetProperty.GetValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCharSetProperty.GetKeyValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCharSetProperty.GetProviderValueComparer());
            Assert.Null(stringWithCharSetProperty[CoreAnnotationNames.PropertyAccessMode]);
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(stringWithCharSetProperty.GetCharSet).Message);

            var stringWithCollationProperty = dataEntity.FindProperty("StringWithCollation")!;
            Assert.Equal(typeof(string), stringWithCollationProperty.ClrType);
            Assert.True(stringWithCollationProperty.IsNullable);
            Assert.Equal(ValueGenerated.OnAdd, stringWithCollationProperty.ValueGenerated);
            Assert.Equal("StringWithCollation", stringWithCollationProperty.GetColumnName());
            Assert.Equal("varchar(128)", stringWithCollationProperty.GetColumnType());
            Assert.Equal("String using collation", (string?)stringWithCollationProperty.GetDefaultValue());
            Assert.IsType<CastingConverter<string, string>>(stringWithCollationProperty.GetValueConverter());
            Assert.IsType<CustomValueComparer<string>>(stringWithCollationProperty.GetValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCollationProperty.GetKeyValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCollationProperty.GetProviderValueComparer());
            Assert.Null(stringWithCollationProperty[CoreAnnotationNames.PropertyAccessMode]);
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(stringWithCollationProperty.GetCollation).Message);
        }

        //
        // PrincipalBase:
        //
        {
            var principalBaseEntity = model.FindEntityType(typeof(PrincipalBase))!;
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(principalBaseEntity.GetCollation).Message);
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(() => principalBaseEntity.GetCollationDelegation()).Message);

            var pointProperty = principalBaseEntity.FindProperty("Point")!;
            Assert.Equal(typeof(Point), pointProperty.ClrType);
            Assert.True(pointProperty.IsNullable);
            Assert.Equal(ValueGenerated.OnAdd, pointProperty.ValueGenerated);
            Assert.Equal("Point", pointProperty.GetColumnName());
            Assert.Equal("geometry", pointProperty.GetColumnType());
            Assert.Equal(0, ((Point)pointProperty.GetDefaultValue()!).SRID);
            Assert.IsType<CastingConverter<Point, Point>>(pointProperty.GetValueConverter());
            Assert.IsType<CustomValueComparer<Point>>(pointProperty.GetValueComparer());
            Assert.IsType<CustomValueComparer<Point>>(pointProperty.GetKeyValueComparer());
            Assert.IsType<CustomValueComparer<Point>>(pointProperty.GetProviderValueComparer());
            Assert.Null(pointProperty[CoreAnnotationNames.PropertyAccessMode]);

            var stringWithCharSetProperty = principalBaseEntity.FindProperty("StringWithCharSet")!;
            Assert.Equal(typeof(string), stringWithCharSetProperty.ClrType);
            Assert.True(stringWithCharSetProperty.IsNullable);
            Assert.Equal(ValueGenerated.OnAdd, stringWithCharSetProperty.ValueGenerated);
            Assert.Equal("StringWithCharSet", stringWithCharSetProperty.GetColumnName());
            Assert.Equal("varchar(128)", stringWithCharSetProperty.GetColumnType());
            Assert.Equal("String having charset", (string?)stringWithCharSetProperty.GetDefaultValue());
            Assert.IsType<CastingConverter<string, string>>(stringWithCharSetProperty.GetValueConverter());
            Assert.IsType<CustomValueComparer<string>>(stringWithCharSetProperty.GetValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCharSetProperty.GetKeyValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCharSetProperty.GetProviderValueComparer());
            Assert.Null(stringWithCharSetProperty[CoreAnnotationNames.PropertyAccessMode]);
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(stringWithCharSetProperty.GetCharSet).Message);

            var stringWithCollationProperty = principalBaseEntity.FindProperty("StringWithCollation")!;
            Assert.Equal(typeof(string), stringWithCollationProperty.ClrType);
            Assert.True(stringWithCollationProperty.IsNullable);
            Assert.Equal(ValueGenerated.OnAdd, stringWithCollationProperty.ValueGenerated);
            Assert.Equal("StringWithCollation", stringWithCollationProperty.GetColumnName());
            Assert.Equal("varchar(128)", stringWithCollationProperty.GetColumnType());
            Assert.Equal("String using collation", (string?)stringWithCollationProperty.GetDefaultValue());
            Assert.IsType<CastingConverter<string, string>>(stringWithCollationProperty.GetValueConverter());
            Assert.IsType<CustomValueComparer<string>>(stringWithCollationProperty.GetValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCollationProperty.GetKeyValueComparer());
            Assert.IsType<CustomValueComparer<string>>(stringWithCollationProperty.GetProviderValueComparer());
            Assert.Null(stringWithCollationProperty[CoreAnnotationNames.PropertyAccessMode]);
            Assert.Equal(
                CoreStrings.RuntimeModelMissingData,
                Assert.Throws<InvalidOperationException>(stringWithCollationProperty.GetCollation).Message);
        }
    }

    public override void Tpc()
    {
        // The CompiledModelRelationalTestBase implementation uses stored procedures with return values and result columns, which are not
        // supported in MySQL.
        // We either need to change the model by overriding `BuildTcpModel` (not an issue) and then fully override the very long `AssertTcp`
        // method (possible maintenance issue, because the class is likely to have a higher volatility), or we need to skip the stored procedure
        // validation to allow the unsupported stored procedure features.
        // We do the latter here.
        Test(
            BuildTpcModel,
            AssertTpc,
            options: new CompiledModelCodeGenerationOptions { UseNullableReferenceTypes = true },
            addServices: s => s.AddSingleton<IModelValidator, StoredProcedureValidationExceptionIgnoringMySqlModelValidator>());
    }

    // protected override void BuildTpcModel(ModelBuilder modelBuilder)
    // {
    //     base.BuildTpcModel(modelBuilder);
    //
    //     modelBuilder.Entity<PrincipalBase>(
    //         eb =>
    //         {
    //             // The base implementation uses `HasRowsAffectedReturnValue` which is not supported by MySQL.
    //             // We use a parameter instead.
    //             new StoredProcedureBuilder(eb.Metadata.GetDeleteStoredProcedure()!, eb)
    //                 .HasRowsAffectedReturnValue(false)
    //                 .HasRowsAffectedParameter(
    //                     p => p.HasName("RowsAffected"));
    //         });
    //
    //     modelBuilder.Entity<PrincipalDerived<DependentBase<byte?>>>(
    //         eb =>
    //         {
    //             // The base implementation uses `HasResultColumn` which is not supported by MySQL.
    //             // We use an output parameter instead.
    //             var originalInsertStoredProcedure = eb.Metadata.RemoveInsertStoredProcedure()!;
    //             eb.InsertUsingStoredProcedure(
    //                 "Derived_Insert",
    //                 s =>
    //                 {
    //                     foreach (var parameter in originalInsertStoredProcedure.Parameters)
    //                     {
    //                         s.HasParameter(parameter.PropertyName!);
    //                     }
    //
    //                     s.HasParameter(
    //                         p => p.Enum1,
    //                         pb => pb
    //                             .HasName("DerivedEnum")
    //                             .HasAnnotation("foo", "bar3")
    //                             .IsOutput());
    //                 });
    //         });
    // }

    // public override void ComplexTypes()
    //     => Test(
    //         BuildComplexTypesModel,
    //         AssertComplexTypes,
    //         c =>
    //         {
    //             c.Set<PrincipalDerived<DependentBase<byte?>>>().Add(
    //                 new PrincipalDerived<DependentBase<byte?>>
    //                 {
    //                     Id = 1,
    //                     AlternateId = new Guid(),
    //                     Dependent = new DependentBase<byte?>(1),
    //                     Owned = new OwnedType(c) { Principal = new PrincipalBase() }
    //                 });
    //
    //             //c.SaveChanges();
    //         },
    //         options: new CompiledModelCodeGenerationOptions { UseNullableReferenceTypes = true },
    //         addServices: s => s.AddSingleton<IModelValidator, StoredProcedureValidationExceptionIgnoringMySqlModelValidator>());

    protected override void BuildComplexTypesModel(ModelBuilder modelBuilder)
    {
        base.BuildComplexTypesModel(modelBuilder);

        // For some reason, the AssertComplexTypes base implementation expects the schema of the `PrincipalBaseTvf` db function to be `dbo`.
        // We therefore just set the default schema here to `dbo`, which works.
        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<PrincipalBase>(
            eb =>
            {
                // The base implementation uses `HasRowsAffectedReturnValue` which is not supported by MySQL.
                // We use a parameter instead.
                new StoredProcedureBuilder(eb.Metadata.GetDeleteStoredProcedure()!, eb)
                    .HasRowsAffectedReturnValue(false)
                    .HasRowsAffectedParameter(
                        p => p.HasName("RowsAffected"));
            });
    }

    public override void BigModel_with_JSON_columns()
    {
        Assert.Equal(
            MySqlStrings.Ef7CoreJsonMappingNotSupported,
            Assert.Throws<InvalidOperationException>(() => base.BigModel_with_JSON_columns()).Message);
    }

    protected override TestHelpers TestHelpers => MySqlTestHelpers.Instance;
    protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

    protected override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    {
        builder = base.AddOptions(builder);

        new MySqlDbContextOptionsBuilder(builder)
            .UseNetTopologySuite()
            .SchemaBehavior(MySqlSchemaBehavior.Ignore);

        return builder;
    }

    protected override void AddDesignTimeServices(IServiceCollection services)
        => new MySqlNetTopologySuiteDesignTimeServices().ConfigureDesignTimeServices(services);

    protected override BuildSource AddReferences(BuildSource build, [CallerFilePath] string filePath = "")
    {
        base.AddReferences(build);
        build.References.Add(BuildReference.ByName("Pomelo.EntityFrameworkCore.MySql"));
        build.References.Add(BuildReference.ByName("Pomelo.EntityFrameworkCore.MySql.NetTopologySuite"));
        build.References.Add(BuildReference.ByName("NetTopologySuite"));
        return build;
    }

    // The CompiledModelRelationalTestBase implementation uses stored procedures with return values and result columns, which are not
    // supported in MySQL.
    // We either need to change the model by overriding `BuildTcpModel` (not an issue) and then fully override the very long `AssertTcp`
    // method (possible maintenance issue, because the class is likely to have a higher volatility), or we need to skip the stored procedure
    // validation to allow the unsupported stored procedure features.
    // We do the latter here.
    public class StoredProcedureValidationExceptionIgnoringMySqlModelValidator(
        ModelValidatorDependencies dependencies,
        RelationalModelValidatorDependencies relationalDependencies)
        : MySqlModelValidator(dependencies, relationalDependencies)
    {
        protected override void ValidateStoredProcedures(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            try
            {
                base.ValidateStoredProcedures(model, logger);
            }
            catch (InvalidOperationException e) when (Regex.IsMatch(e.Message, Regex.Escape(MySqlStrings.StoredProcedureResultColumnsNotSupported("::::", "::::")).Replace("::::", ".*")) ||
                                                      Regex.IsMatch(e.Message, Regex.Escape(MySqlStrings.StoredProcedureReturnValueNotSupported("::::", "::::")).Replace("::::", ".*")))
            {
            }
        }
    }
}
