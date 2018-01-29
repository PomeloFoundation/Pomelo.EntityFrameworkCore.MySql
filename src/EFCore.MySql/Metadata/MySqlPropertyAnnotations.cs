// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlPropertyAnnotations : RelationalPropertyAnnotations, IMySqlPropertyAnnotations
    {
        public MySqlPropertyAnnotations([NotNull] IProperty property)
            : base(property)
        {
        }

        protected MySqlPropertyAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        public virtual MySqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get { return GetMySqlValueGenerationStrategy(fallbackToModel: true); }
            [param: CanBeNull] set { SetValueGenerationStrategy(value); }
        }

        public virtual MySqlValueGenerationStrategy? GetMySqlValueGenerationStrategy(bool fallbackToModel)
        {
            if (GetDefaultValue(false) != null
                || GetDefaultValueSql(false) != null
                || GetComputedColumnSql(false) != null)
            {
                return null;
            }

            var value = (MySqlValueGenerationStrategy?)Annotations.Metadata[MySqlAnnotationNames.ValueGenerationStrategy];

            if (value != null)
            {
                return value;
            }

            var relationalProperty = Property.Relational();
            if (!fallbackToModel
                || Property.ValueGenerated != ValueGenerated.OnAdd
                || relationalProperty.DefaultValue != null
                || relationalProperty.DefaultValueSql != null
                || relationalProperty.ComputedColumnSql != null)
            {
                return null;
            }

            var modelStrategy = Property.DeclaringEntityType.Model.MySql().ValueGenerationStrategy;

            if (modelStrategy == MySqlValueGenerationStrategy.IdentityColumn
                && IsCompatibleIdentityColumn(Property.ClrType))
            {
                return MySqlValueGenerationStrategy.IdentityColumn;
            }

            if (modelStrategy == MySqlValueGenerationStrategy.ComputedColumn
                && IsCompatibleIdentityColumn(Property.ClrType))
            {
                return MySqlValueGenerationStrategy.ComputedColumn;
            }

            return null;
        }

        protected virtual bool SetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
        {
            if (value != null)
            {
                var propertyType = Property.ClrType;

                if (value == MySqlValueGenerationStrategy.IdentityColumn
                    && !IsCompatibleIdentityColumn(propertyType))
                {
                    if (ShouldThrowOnInvalidConfiguration)
                    {
                        throw new ArgumentException(
                            Property.Name + " " + Property.DeclaringEntityType.DisplayName() + " " + propertyType.ShortDisplayName());
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

        protected override object GetDefaultValue(bool fallback)
        {
            if (fallback
                && ValueGenerationStrategy != null)
            {
                return null;
            }

            return base.GetDefaultValue(fallback);
        }

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

        protected override string GetDefaultValueSql(bool fallback)
        {
            if (fallback
                && ValueGenerationStrategy != null)
            {
                return null;
            }

            return base.GetDefaultValueSql(fallback);
        }

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

        protected override string GetComputedColumnSql(bool fallback)
        {
            if (fallback
                && ValueGenerationStrategy != null)
            {
                return null;
            }

            return base.GetComputedColumnSql(fallback);
        }

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

        protected override void ClearAllServerGeneratedValues()
        {
            SetValueGenerationStrategy(null);

            base.ClearAllServerGeneratedValues();
        }

        private static bool IsCompatibleIdentityColumn(Type type)
            => type.IsInteger() || type == typeof(DateTime);

    }
}
