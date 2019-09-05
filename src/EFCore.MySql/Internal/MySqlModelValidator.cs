using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton"/>. This means a single instance
    ///         is used by many <see cref="DbContext"/> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped"/>.
    ///     </para>
    /// </summary>
    public class MySqlModelValidator : RelationalModelValidator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlModelValidator(
            [NotNull] ModelValidatorDependencies dependencies,
            [NotNull] RelationalModelValidatorDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.Validate(model, logger);

            ValidateDefaultDecimalMapping(model, logger);
            ValidateByteIdentityMapping(model, logger);
            ValidateNonKeyValueGeneration(model, logger);
            ValidateIndexIncludeProperties(model, logger);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual void ValidateDefaultDecimalMapping([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => p.ClrType.UnwrapNullableType() == typeof(decimal)
                         && !p.IsForeignKey()))
            {
                var typeConfigurationSource = (property as IConventionProperty)?.GetColumnTypeConfigurationSource();
                var typeMappingConfigurationSource = (property as IConventionProperty)?.GetTypeMappingConfigurationSource(); // TODO: ?
                if ((typeConfigurationSource == null
                     && ConfigurationSource.Convention.Overrides(typeMappingConfigurationSource))
                    || (typeConfigurationSource != null
                     && ConfigurationSource.Convention.Overrides(typeConfigurationSource)))
                {
                    logger.DecimalTypeDefaultWarning(property);
                }
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual void ValidateByteIdentityMapping([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => p.ClrType.UnwrapNullableType() == typeof(byte)
                         && p.GetValueGenerationStrategy() == MySqlValueGenerationStrategy.IdentityColumn))
            {
                logger.ByteIdentityColumnWarning(property);
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual void ValidateNonKeyValueGeneration([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => ((IConventionProperty)p).GetValueGenerationStrategyConfigurationSource() != null
                         && !p.IsKey()
                         && p.ValueGenerated != ValueGenerated.Never
                         && (!(p.FindAnnotation(MySqlAnnotationNames.ValueGenerationStrategy) is ConventionAnnotation strategy)
                             || !ConfigurationSource.Convention.Overrides(strategy.GetConfigurationSource()))))
            {
                throw new InvalidOperationException(
                    MySqlStrings.NonKeyValueGeneration(property.Name, property.DeclaringEntityType.DisplayName()));
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual void ValidateIndexIncludeProperties([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            // TODO: ?
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void ValidateSharedTableCompatibility(
            IReadOnlyList<IEntityType> mappedTypes, string tableName, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            // TODO: ?
            base.ValidateSharedTableCompatibility(mappedTypes, tableName, logger);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void ValidateSharedColumnsCompatibility(
            IReadOnlyList<IEntityType> mappedTypes, string tableName, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.ValidateSharedColumnsCompatibility(mappedTypes, tableName, logger);

            var identityColumns = new List<IProperty>();
            var propertyMappings = new Dictionary<string, IProperty>();

            foreach (var property in mappedTypes.SelectMany(et => et.GetDeclaredProperties()))
            {
                var columnName = property.GetColumnName();
                if (propertyMappings.TryGetValue(columnName, out var duplicateProperty))
                {
                    var propertyStrategy = property.GetValueGenerationStrategy();
                    var duplicatePropertyStrategy = duplicateProperty.GetValueGenerationStrategy();
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
                    if (property.GetValueGenerationStrategy() == MySqlValueGenerationStrategy.IdentityColumn)
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
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void ValidateSharedKeysCompatibility(
            IReadOnlyList<IEntityType> mappedTypes, string tableName, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.ValidateSharedKeysCompatibility(mappedTypes, tableName, logger);

            var keyMappings = new Dictionary<string, IKey>();

            foreach (var key in mappedTypes.SelectMany(et => et.GetDeclaredKeys()))
            {
                var keyName = key.GetName();

                if (!keyMappings.TryGetValue(keyName, out var duplicateKey))
                {
                    keyMappings[keyName] = key;
                    continue;
                }

                // TODO: ?
                //if (key.IsClustered()
                //    != duplicateKey.IsClustered())
                //{
                //    throw new InvalidOperationException(
                //        MySqlStrings.DuplicateKeyMismatchedClustering(
                //            key.Properties.Format(),
                //            key.DeclaringEntityType.DisplayName(),
                //            duplicateKey.Properties.Format(),
                //            duplicateKey.DeclaringEntityType.DisplayName(),
                //            tableName,
                //            keyName));
                //}
            }
        }
    }
}
