// <auto-generated />
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
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
                propertyCount: 2,
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

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var trigger1 = runtimeEntityType.AddTrigger(
                "Trigger1");

            var trigger2 = runtimeEntityType.AddTrigger(
                "Trigger2");

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
