using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="IProperty" /> for SQL Server-specific metadata.
    /// </summary>
    public static class MySqlPropertyExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns the <see cref="SqlServerValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        ///     </para>
        /// </summary>
        /// <returns> The strategy, or <see cref="SqlServerValueGenerationStrategy.None"/> if none was set. </returns>
        public static MySqlValueGenerationStrategy? GetValueGenerationStrategy([NotNull] this IProperty property)
        {
            var annotation = property[MySqlAnnotationNames.ValueGenerationStrategy];
            if (annotation != null)
            {
                return (MySqlValueGenerationStrategy?)annotation;
            }

            if (property.GetDefaultValue() != null
                || property.GetDefaultValueSql() != null
                || property.GetComputedColumnSql() != null)
            {
                return null;
            }

            if (property.ValueGenerated == ValueGenerated.Never)
            {
                var sharedTablePrincipalPrimaryKeyProperty = property.FindSharedTableRootPrimaryKeyProperty();
                return sharedTablePrincipalPrimaryKeyProperty?.GetValueGenerationStrategy();
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
        ///     Sets the <see cref="SqlServerValueGenerationStrategy" /> to use for the property.
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
        /// <returns> <c>true</c> if compatible. </returns>
        public static bool IsCompatibleIdentityColumn(IProperty property)
        {
            var type = property.ClrType;

            return (type.IsInteger()
                        || type == typeof(decimal)
                        || type == typeof(DateTime)
                        || type == typeof(DateTimeOffset))
                   && !HasConverter(property);
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="MySqlValueGenerationStrategy.ComputedColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <c>true</c> if compatible. </returns>
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
        /// Returns the <see cref="CharSet"/> used by the property's column.
        /// </summary>
        /// <param name="property">The property of which get its column's <see cref="CharSet"/> from.</param>
        /// <returns>The <see cref="CharSet"/> or null, if no explicit <see cref="CharSet"/> was set.</returns>
        public static CharSet GetCharSet([NotNull] this IProperty property)
        {
            var charSetName = property[MySqlAnnotationNames.CharSet] as string;

            if (string.IsNullOrEmpty(charSetName))
            {
                return null;
            }

            return CharSet.GetCharSetFromName(charSetName);
        }

        /// <summary>
        /// Sets the <see cref="CharSet"/> in use by the property's column.
        /// </summary>
        /// <param name="property">The property to set the <see cref="CharSet"/> for.</param>
        /// <param name="charSet">The <see cref="CharSet"/> used by the property's column.</param>
        public static void SetCharSet([NotNull] this IMutableProperty property, CharSet charSet)
        {
            if (charSet != null &&
                !property.IsUnicode().HasValue)
            {
                property.SetIsUnicode(charSet.IsUnicode);
            }

            property.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSet, charSet);
        }

        /// <summary>
        /// Sets a predefined <see cref="CharSet"/> by its name, that is in use by the property's column.
        /// </summary>
        /// <param name="property">The property to set the <see cref="CharSet"/> for.</param>
        /// <param name="predefinedCharSetName">The charset name used by the property's column, that is
        /// being resolved into a <see cref="CharSet"/> object.</param>
        public static void SetCharSet([NotNull] this IMutableProperty property, string predefinedCharSetName)
        {
            Check.NotNull(property, nameof(property));
            Check.NotEmpty(predefinedCharSetName, nameof(predefinedCharSetName));

            var charSet = CharSet.GetCharSetFromName(predefinedCharSetName);

            if (charSet == null)
            {
                throw new ArgumentOutOfRangeException($"Cannot find a predefined charset with the name of \"{predefinedCharSetName}\".");
            }

            property.SetCharSet(charSet);
        }
    }
}
