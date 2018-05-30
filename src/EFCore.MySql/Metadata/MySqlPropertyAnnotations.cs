// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     Properties for MySql-specific annotations accessed through
    ///     <see cref="MySqlMetadataExtensions.MySql(IMutableProperty)" />.
    /// </summary>
    public class MySqlPropertyAnnotations : RelationalPropertyAnnotations, IMySqlPropertyAnnotations
    {
        /// <summary>
        ///     Constructs an instance for annotations of the given <see cref="IProperty" />.
        /// </summary>
        /// <param name="property"> The <see cref="IProperty" /> to use. </param>
        public MySqlPropertyAnnotations([NotNull] IProperty property)
            : base(property)
        {
        }

        /// <summary>
        ///     Constructs an instance for annotations of the <see cref="IProperty" />
        ///     represented by the given annotation helper.
        /// </summary>
        /// <param name="annotations">
        ///     The <see cref="RelationalAnnotations" /> helper representing the <see cref="IProperty" /> to annotate.
        /// </param>
        protected MySqlPropertyAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        /// <summary>
        ///     <para>
        ///         Gets or sets the <see cref="MySqlValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />
        ///     </para>
        /// </summary>
        public virtual MySqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get => GetMySqlValueGenerationStrategy(fallbackToModel: true);
            set => SetValueGenerationStrategy(value);
        }

        /// <summary>
        ///     Gets or sets the <see cref="MySqlValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="fallbackToModel">
        ///     If <c>true</c>, then if no strategy is set for the property,
        ///     then the strategy to use will be taken from the <see cref="IModel" />.
        /// </param>
        /// <returns> The strategy, or <c>null</c> if none was set. </returns>
        public virtual MySqlValueGenerationStrategy? GetMySqlValueGenerationStrategy(bool fallbackToModel)
        {
            var annotation = Annotations.Metadata.FindAnnotation(MySqlAnnotationNames.ValueGenerationStrategy);
            if (annotation != null)
            {
                return (MySqlValueGenerationStrategy?)annotation.Value;
            }

            var relationalProperty = Property.Relational();
            if (!fallbackToModel
                || relationalProperty.DefaultValue != null
                || relationalProperty.DefaultValueSql != null
                || relationalProperty.ComputedColumnSql != null)
            {
                return null;
            }

            if (Property.ValueGenerated != ValueGenerated.OnAdd)
            {
                var sharedTablePrincipalPrimaryKeyProperty = Property.FindSharedTableRootPrimaryKeyProperty();
                if (sharedTablePrincipalPrimaryKeyProperty != null
                    && sharedTablePrincipalPrimaryKeyProperty.MySql().ValueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn)
                {
                    return MySqlValueGenerationStrategy.IdentityColumn;
                }

                return null;
            }

            var modelStrategy = Property.DeclaringEntityType.Model.MySql().ValueGenerationStrategy;
            if (modelStrategy.HasValue)
            {
                switch (modelStrategy.Value)
                {
                    case MySqlValueGenerationStrategy.IdentityColumn when IsCompatibleIdentityColumn(Property):
                        return MySqlValueGenerationStrategy.IdentityColumn;
                    case MySqlValueGenerationStrategy.ComputedColumn:
                        return MySqlValueGenerationStrategy.ComputedColumn;
                }
            }

            return null;
        }

        /// <summary>
        ///     Sets the <see cref="MySqlValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="value"> The strategy to use. </param>
        /// <returns> <c>True</c> if the annotation was set; <c>false</c> otherwise. </returns>
        protected virtual bool SetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
        {
            if (value != null)
            {
                var propertyType = Property.ClrType;

                if (value == MySqlValueGenerationStrategy.IdentityColumn
                    && !IsCompatibleIdentityColumn(Property))
                {
                    if (ShouldThrowOnInvalidConfiguration)
                    {
                        throw new ArgumentException(
                            MySqlStrings.IdentityBadType(
                                Property.Name, Property.DeclaringEntityType.DisplayName(), propertyType.ShortDisplayName()));
                    }

                    return false;
                }
            }

            if (!CanSetValueGenerationStrategy(value))
            {
                return false;
            }

            if (!ShouldThrowOnConflict
                && ValueGenerationStrategy != value
                && value != null)
            {
                ClearAllServerGeneratedValues();
            }

            return Annotations.SetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);
        }

        /// <summary>
        ///     Checks whether or not it is valid to set the given <see cref="MySqlValueGenerationStrategy" />
        ///     for the property.
        /// </summary>
        /// <param name="value"> The strategy to check. </param>
        /// <returns> <c>True</c> if it is valid to set; <c>false</c> otherwise. </returns>
        protected virtual bool CanSetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
        {
            if (GetMySqlValueGenerationStrategy(fallbackToModel: false) == value)
            {
                return true;
            }

            if (!Annotations.CanSetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value))
            {
                return false;
            }

            if (ShouldThrowOnConflict)
            {
                if (GetDefaultValue(false) != null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.ConflictingColumnServerGeneration(nameof(ValueGenerationStrategy), Property.Name, nameof(DefaultValue)));
                }

                if (GetDefaultValueSql(false) != null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.ConflictingColumnServerGeneration(nameof(ValueGenerationStrategy), Property.Name, nameof(DefaultValueSql)));
                }

                if (GetComputedColumnSql(false) != null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.ConflictingColumnServerGeneration(nameof(ValueGenerationStrategy), Property.Name, nameof(ComputedColumnSql)));
                }
            }
            else if (value != null
                     && (!CanSetDefaultValue(null)
                         || !CanSetDefaultValueSql(null)
                         || !CanSetComputedColumnSql(null)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets the default value set for the property.
        /// </summary>
        /// <param name="fallback">
        ///     If <c>true</c>, and some MySql specific
        ///     <see cref="ValueGenerationStrategy" /> has been set, then this method will always
        ///     return <c>null</c> because these strategies do not use default values.
        /// </param>
        /// <returns> The default value, or <c>null</c> if none has been set. </returns>
        protected override object GetDefaultValue(bool fallback)
        {
            if (fallback
                && ValueGenerationStrategy != null)
            {
                return null;
            }

            return base.GetDefaultValue(fallback);
        }

        /// <summary>
        ///     Checks whether or not it is valid to set a default value for the property.
        /// </summary>
        /// <param name="value"> The value to check. </param>
        /// <returns> <c>True</c> if it is valid to set this value; <c>false</c> otherwise. </returns>
        protected override bool CanSetDefaultValue(object value)
        {
            if (ShouldThrowOnConflict)
            {
                if (ValueGenerationStrategy != null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.ConflictingColumnServerGeneration(nameof(DefaultValue), Property.Name, nameof(ValueGenerationStrategy)));
                }
            }
            else if (value != null
                     && !CanSetValueGenerationStrategy(null))
            {
                return false;
            }

            return base.CanSetDefaultValue(value);
        }

        /// <summary>
        ///     Gets the default SQL expression set for the property.
        /// </summary>
        /// <param name="fallback">
        ///     If <c>true</c>, and some MySql specific
        ///     <see cref="ValueGenerationStrategy" /> has been set, then this method will always
        ///     return <c>null</c> because these strategies do not use default expressions.
        /// </param>
        /// <returns> The default expression, or <c>null</c> if none has been set. </returns>
        protected override string GetDefaultValueSql(bool fallback)
        {
            if (fallback
                && ValueGenerationStrategy != null)
            {
                return null;
            }

            return base.GetDefaultValueSql(fallback);
        }

        /// <summary>
        ///     Checks whether or not it is valid to set a default SQL expression for the property.
        /// </summary>
        /// <param name="value"> The expression to check. </param>
        /// <returns> <c>True</c> if it is valid to set this expression; <c>false</c> otherwise. </returns>
        protected override bool CanSetDefaultValueSql(string value)
        {
            if (ShouldThrowOnConflict)
            {
                if (ValueGenerationStrategy != null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.ConflictingColumnServerGeneration(nameof(DefaultValueSql), Property.Name, nameof(ValueGenerationStrategy)));
                }
            }
            else if (value != null
                     && !CanSetValueGenerationStrategy(null))
            {
                return false;
            }

            return base.CanSetDefaultValueSql(value);
        }

        /// <summary>
        ///     Gets the computed SQL expression set for the property.
        /// </summary>
        /// <param name="fallback">
        ///     If <c>true</c>, and some MySql specific
        ///     <see cref="ValueGenerationStrategy" /> has been set, then this method will always
        ///     return <c>null</c> because these strategies do not use computed expressions.
        /// </param>
        /// <returns> The computed expression, or <c>null</c> if none has been set. </returns>
        protected override string GetComputedColumnSql(bool fallback)
        {
            if (fallback
                && ValueGenerationStrategy != null)
            {
                return null;
            }

            return base.GetComputedColumnSql(fallback);
        }

        /// <summary>
        ///     Checks whether or not it is valid to set a computed SQL expression for the property.
        /// </summary>
        /// <param name="value"> The expression to check. </param>
        /// <returns> <c>True</c> if it is valid to set this expression; <c>false</c> otherwise. </returns>
        protected override bool CanSetComputedColumnSql(string value)
        {
            if (ShouldThrowOnConflict)
            {
                if (ValueGenerationStrategy != null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.ConflictingColumnServerGeneration(nameof(ComputedColumnSql), Property.Name, nameof(ValueGenerationStrategy)));
                }
            }
            else if (value != null
                     && !CanSetValueGenerationStrategy(null))
            {
                return false;
            }

            return base.CanSetComputedColumnSql(value);
        }

        /// <summary>
        ///     Resets value-generation for the property to defaults.
        /// </summary>
        protected override void ClearAllServerGeneratedValues()
        {
            SetValueGenerationStrategy(null);

            base.ClearAllServerGeneratedValues();
        }

        private static bool IsCompatibleIdentityColumn(IProperty property)
        {
            var type = property.ClrType;

            return (type.IsInteger() || type == typeof(decimal)) && !HasConverter(property);
        }

        private static bool HasConverter(IProperty property)
            => (property.FindMapping()?.Converter
                ?? property.GetValueConverter()) != null;
    }
}
