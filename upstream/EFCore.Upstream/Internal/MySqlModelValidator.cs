// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlModelValidator : RelationalModelValidator
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlModelValidator(
            [NotNull] ModelValidatorDependencies dependencies,
            [NotNull] RelationalModelValidatorDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void Validate(IModel model)
        {
            base.Validate(model);

            ValidateDefaultDecimalMapping(model);
            ValidateByteIdentityMapping(model);
            ValidateNonKeyValueGeneration(model);
            ValidateIndexIncludeProperties(model);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateDefaultDecimalMapping([NotNull] IModel model)
        {
            foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => p.ClrType.UnwrapNullableType() == typeof(decimal)
                         && !p.IsForeignKey()))
            {
#pragma warning disable IDE0019 // Use pattern matching
                var type = property.FindAnnotation(RelationalAnnotationNames.ColumnType) as ConventionalAnnotation;
#pragma warning restore IDE0019 // Use pattern matching
                var typeMapping = property.FindAnnotation(CoreAnnotationNames.TypeMapping) as ConventionalAnnotation;
                if ((type == null
                     && (typeMapping == null
                         || ConfigurationSource.Convention.Overrides(typeMapping.GetConfigurationSource())))
                    || (type != null
                        && ConfigurationSource.Convention.Overrides(type.GetConfigurationSource())))
                {
                    Dependencies.Logger.DecimalTypeDefaultWarning(property);
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateByteIdentityMapping([NotNull] IModel model)
        {
            foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => p.ClrType.UnwrapNullableType() == typeof(byte)
                         && p.MySql().ValueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn))
            {
                Dependencies.Logger.ByteIdentityColumnWarning(property);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateNonKeyValueGeneration([NotNull] IModel model)
        {
            foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => ((MySqlPropertyAnnotations)p.MySql()).GetMySqlValueGenerationStrategy(fallbackToModel: false)
                         == MySqlValueGenerationStrategy.SequenceHiLo
                         && !p.IsKey()
                         && p.ValueGenerated != ValueGenerated.Never
                         && (!(p.FindAnnotation(MySqlAnnotationNames.ValueGenerationStrategy) is ConventionalAnnotation strategy)
                             || !ConfigurationSource.Convention.Overrides(strategy.GetConfigurationSource()))))
            {
                throw new InvalidOperationException(
                    MySqlStrings.NonKeyValueGeneration(property.Name, property.DeclaringEntityType.DisplayName()));
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateIndexIncludeProperties([NotNull] IModel model)
        {
            foreach (var index in model.GetEntityTypes().SelectMany(t => t.GetDeclaredIndexes()))
            {
                var includeProperties = index.MySql().IncludeProperties;
                if (includeProperties?.Count > 0)
                {
                    var notFound = includeProperties
                        .Where(i => index.DeclaringEntityType.FindProperty(i) == null)
                        .FirstOrDefault();

                    if (notFound != null)
                    {
                        throw new InvalidOperationException(
                            MySqlStrings.IncludePropertyNotFound(index.DeclaringEntityType.DisplayName(), notFound));
                    }

                    var duplicate = includeProperties
                        .GroupBy(i => i)
                        .Where(g => g.Count() > 1)
                        .Select(y => y.Key)
                        .FirstOrDefault();

                    if (duplicate != null)
                    {
                        throw new InvalidOperationException(
                            MySqlStrings.IncludePropertyDuplicated(index.DeclaringEntityType.DisplayName(), duplicate));
                    }

                    var inIndex = includeProperties
                        .Where(i => index.Properties.Any(p => i == p.Name))
                        .FirstOrDefault();

                    if (inIndex != null)
                    {
                        throw new InvalidOperationException(
                            MySqlStrings.IncludePropertyInIndex(index.DeclaringEntityType.DisplayName(), inIndex));
                    }
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ValidateSharedTableCompatibility(
            IReadOnlyList<IEntityType> mappedTypes, string tableName)
        {
            var firstMappedType = mappedTypes[0];
            var isMemoryOptimized = firstMappedType.MySql().IsMemoryOptimized;

            foreach (var otherMappedType in mappedTypes.Skip(1))
            {
                if (isMemoryOptimized != otherMappedType.MySql().IsMemoryOptimized)
                {
                    throw new InvalidOperationException(
                        MySqlStrings.IncompatibleTableMemoryOptimizedMismatch(
                            tableName, firstMappedType.DisplayName(), otherMappedType.DisplayName(),
                            isMemoryOptimized ? firstMappedType.DisplayName() : otherMappedType.DisplayName(),
                            !isMemoryOptimized ? firstMappedType.DisplayName() : otherMappedType.DisplayName()));
                }
            }

            base.ValidateSharedTableCompatibility(mappedTypes, tableName);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ValidateSharedColumnsCompatibility(IReadOnlyList<IEntityType> mappedTypes, string tableName)
        {
            base.ValidateSharedColumnsCompatibility(mappedTypes, tableName);

            var identityColumns = new List<IProperty>();
            var propertyMappings = new Dictionary<string, IProperty>();

            foreach (var property in mappedTypes.SelectMany(et => et.GetDeclaredProperties()))
            {
                var propertyAnnotations = property.Relational();
                var columnName = propertyAnnotations.ColumnName;
                if (propertyMappings.TryGetValue(columnName, out var duplicateProperty))
                {
                    var propertyStrategy = property.MySql().ValueGenerationStrategy;
                    var duplicatePropertyStrategy = duplicateProperty.MySql().ValueGenerationStrategy;
                    if (propertyStrategy != duplicatePropertyStrategy
                        && (propertyStrategy == MySqlValueGenerationStrategy.IdentityColumn
                            || duplicatePropertyStrategy == MySqlValueGenerationStrategy.IdentityColumn))
                    {
                        throw new InvalidOperationException(
                            MySqlStrings.DuplicateColumnNameValueGenerationStrategyMismatch(
                                duplicateProperty.DeclaringEntityType.DisplayName(),
                                duplicateProperty.Name,
                                property.DeclaringEntityType.DisplayName(),
                                property.Name,
                                columnName,
                                tableName));
                    }
                }
                else
                {
                    propertyMappings[columnName] = property;
                    if (property.MySql().ValueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn)
                    {
                        identityColumns.Add(property);
                    }
                }
            }

            if (identityColumns.Count > 1)
            {
                var sb = new StringBuilder()
                    .AppendJoin(identityColumns.Select(p => "'" + p.DeclaringEntityType.DisplayName() + "." + p.Name + "'"));
                throw new InvalidOperationException(MySqlStrings.MultipleIdentityColumns(sb, tableName));
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ValidateSharedKeysCompatibility(
            IReadOnlyList<IEntityType> mappedTypes, string tableName)
        {
            base.ValidateSharedKeysCompatibility(mappedTypes, tableName);

            var keyMappings = new Dictionary<string, IKey>();

            foreach (var key in mappedTypes.SelectMany(et => et.GetDeclaredKeys()))
            {
                var keyName = key.Relational().Name;

                if (!keyMappings.TryGetValue(keyName, out var duplicateKey))
                {
                    keyMappings[keyName] = key;
                    continue;
                }

                if (key.MySql().IsClustered
                     != duplicateKey.MySql().IsClustered)
                {
                    throw new InvalidOperationException(
                        MySqlStrings.DuplicateKeyMismatchedClustering(
                            Property.Format(key.Properties),
                            key.DeclaringEntityType.DisplayName(),
                            Property.Format(duplicateKey.Properties),
                            duplicateKey.DeclaringEntityType.DisplayName(),
                            tableName,
                            keyName));
                }
            }
        }
    }
}
