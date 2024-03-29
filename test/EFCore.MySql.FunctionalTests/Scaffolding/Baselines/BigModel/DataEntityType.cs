// <auto-generated />
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace TestNamespace
{
    internal partial class DataEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Microsoft.EntityFrameworkCore.Scaffolding.CompiledModelTestBase+Data",
                typeof(CompiledModelTestBase.Data),
                baseEntityType,
                propertyCount: 5,
                keyCount: 1);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(int),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0);
            id.TypeMapping = MySqlIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            id.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            var blob = runtimeEntityType.AddProperty(
                "Blob",
                typeof(byte[]),
                propertyInfo: typeof(CompiledModelTestBase.Data).GetProperty("Blob", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompiledModelTestBase.Data).GetField("<Blob>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            blob.TypeMapping = MySqlByteArrayTypeMapping.Default.Clone(
                comparer: new ValueComparer<byte[]>(
                    (Byte[] v1, Byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals((object)v1, (object)v2),
                    (Byte[] v) => v.GetHashCode(),
                    (Byte[] v) => v),
                keyComparer: new ValueComparer<byte[]>(
                    (Byte[] v1, Byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals((object)v1, (object)v2),
                    (Byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode((object)v),
                    (Byte[] source) => source.ToArray()),
                providerValueComparer: new ValueComparer<byte[]>(
                    (Byte[] v1, Byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals((object)v1, (object)v2),
                    (Byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode((object)v),
                    (Byte[] source) => source.ToArray()));
            blob.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var point = runtimeEntityType.AddProperty(
                "Point",
                typeof(Point),
                nullable: true);
            point.TypeMapping = null;
            point.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var stringWithCharSet = runtimeEntityType.AddProperty(
                "StringWithCharSet",
                typeof(string),
                nullable: true,
                valueGenerated: ValueGenerated.OnAdd,
                valueConverter: new CastingConverter<string, string>(),
                valueComparer: new CompiledModelTestBase.CustomValueComparer<string>(),
                providerValueComparer: new CompiledModelTestBase.CustomValueComparer<string>());
            stringWithCharSet.TypeMapping = MySqlStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(128)",
                    size: 128),
                converter: new ValueConverter<string, string>(
                    (string v) => (string)v,
                    (string v) => (string)v),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<string, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<string, string>(
                        (string v) => (string)v,
                        (string v) => (string)v)));
            stringWithCharSet.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            stringWithCharSet.AddAnnotation("Relational:ColumnType", "varchar(128)");
            stringWithCharSet.AddAnnotation("Relational:DefaultValue", "String having charset");

            var stringWithCollation = runtimeEntityType.AddProperty(
                "StringWithCollation",
                typeof(string),
                nullable: true,
                valueGenerated: ValueGenerated.OnAdd,
                valueConverter: new CastingConverter<string, string>(),
                valueComparer: new CompiledModelTestBase.CustomValueComparer<string>(),
                providerValueComparer: new CompiledModelTestBase.CustomValueComparer<string>());
            stringWithCollation.TypeMapping = MySqlStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "varchar(128)",
                    size: 128),
                converter: new ValueConverter<string, string>(
                    (string v) => (string)v,
                    (string v) => (string)v),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<string, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<string, string>(
                        (string v) => (string)v,
                        (string v) => (string)v)));
            stringWithCollation.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);
            stringWithCollation.AddAnnotation("Relational:ColumnType", "varchar(128)");
            stringWithCollation.AddAnnotation("Relational:DefaultValue", "String using collation");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Data");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
