// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="IProperty" /> for MySQL-specific metadata.
    /// </summary>
    public static class MySqlPropertyExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns the <see cref="MySqlValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        ///     </para>
        /// </summary>
        /// <returns> The strategy, or <see cref="MySqlValueGenerationStrategy.None"/> if none was set. </returns>
        public static MySqlValueGenerationStrategy? GetValueGenerationStrategy([NotNull] this IProperty property, StoreObjectIdentifier storeObject = default)
        {
            var annotation = property[MySqlAnnotationNames.ValueGenerationStrategy];
            if (annotation != null)
            {
                return (MySqlValueGenerationStrategy?)annotation;
            }

            if (property.GetDefaultValue() != null || property.GetDefaultValueSql() != null || property.GetComputedColumnSql() != null)
            {
                return null;
            }

            if (storeObject != default &&
                property.ValueGenerated == ValueGenerated.Never)
            {
                return property.FindSharedStoreObjectRootProperty(storeObject)
                    ?.GetValueGenerationStrategy(storeObject);
            }

            if (IsCompatibleIdentityColumn(property) && property.ValueGenerated == ValueGenerated.OnAdd)
            {
                return MySqlValueGenerationStrategy.IdentityColumn;
            }

            if (IsCompatibleComputedColumn(property) && property.ValueGenerated == ValueGenerated.OnAddOrUpdate)
            {
                return MySqlValueGenerationStrategy.ComputedColumn;
            }

            return null;
        }

        /// <summary>
        ///     Sets the <see cref="MySqlValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        public static void SetValueGenerationStrategy(
            [NotNull] this IMutableProperty property, MySqlValueGenerationStrategy? value)
        {
            CheckValueGenerationStrategy(property, value);

            property.SetOrRemoveAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);
        }

        private static void CheckValueGenerationStrategy(IProperty property, MySqlValueGenerationStrategy? value)
        {
            if (value != null)
            {
                var propertyType = property.ClrType;

                if (value == MySqlValueGenerationStrategy.IdentityColumn
                    && !IsCompatibleIdentityColumn(property))
                {
                    throw new ArgumentException(
                            MySqlStrings.IdentityBadType(
                                property.Name, property.DeclaringEntityType.DisplayName(), propertyType.ShortDisplayName()));
                }

                if (value == MySqlValueGenerationStrategy.ComputedColumn
                    && !IsCompatibleComputedColumn(property))
                {
                    throw new ArgumentException(
                            MySqlStrings.ComputedBadType(
                                property.Name, property.DeclaringEntityType.DisplayName(), propertyType.ShortDisplayName()));
                }
            }
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="MySqlValueGenerationStrategy.IdentityColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleIdentityColumn(IProperty property)
        {
            var valueConverter = GetConverter(property);
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
            return type.IsInteger()
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(DateTimeOffset);
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="MySqlValueGenerationStrategy.ComputedColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleComputedColumn(IProperty property)
        {
            var type = property.ClrType;

            // RowVersion uses byte[] and the BytesToDateTimeConverter.
            return (type == typeof(DateTime) || type == typeof(DateTimeOffset)) && !HasConverter(property)
                   || type == typeof(byte[]) && !HasExternalConverter(property);
        }

        private static bool HasConverter(IProperty property)
            => GetConverter(property) != null;

        private static bool HasExternalConverter(IProperty property)
        {
            var converter = GetConverter(property);
            return converter != null && !(converter is BytesToDateTimeConverter);
        }

        private static ValueConverter GetConverter(IProperty property)
            => property.FindTypeMapping()?.Converter ?? property.GetValueConverter();

        /// <summary>
        /// Returns the name of the charset used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns charset from.</param>
        /// <returns>The name of the charset or null, if no explicit charset was set.</returns>
        public static string GetCharSet([NotNull] this IProperty property)
            => property[MySqlAnnotationNames.CharSet] as string;

        /// <summary>
        /// Sets the name of the charset in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns charset for.</param>
        /// <param name="charSet">The name of the charset used for the column of the property.</param>
        public static void SetCharSet([NotNull] this IMutableProperty property, string charSet)
            => property.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSet, charSet);

        /// <summary>
        /// Returns the name of the collation used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns collation from.</param>
        /// <returns>The name of the collation or null, if no explicit collation was set.</returns>
        public static string GetCollation([NotNull] this IProperty property)
            => property[MySqlAnnotationNames.Collation] as string;

        /// <summary>
        /// Sets the name of the collation in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns collation for.</param>
        /// <param name="collation">The name of the collation used for the column of the property.</param>
        public static void SetCollation([NotNull] this IMutableProperty property, string collation)
            => property.SetOrRemoveAnnotation(MySqlAnnotationNames.Collation, collation);

        /// <summary>
        /// Returns the Spatial Reference System Identifier (SRID) used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns SRID from.</param>
        /// <returns>The SRID or null, if no explicit SRID has been set.</returns>
        public static int? GetSpatialReferenceSystem([NotNull] this IProperty property)
            => (int?)property[MySqlAnnotationNames.SpatialReferenceSystemId];

        /// <summary>
        /// Sets the Spatial Reference System Identifier (SRID) in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns SRID for.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        public static void SetSpatialReferenceSystem([NotNull] this IMutableProperty property, int? srid)
            => property.SetOrRemoveAnnotation(MySqlAnnotationNames.SpatialReferenceSystemId, srid);
    }
}
