// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    /// <summary>
    ///     <para>
    ///         The validator that enforces core rules common for all providers.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
    ///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class ModelValidator : IModelValidator
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ModelValidator" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        public ModelValidator([NotNull] ModelValidatorDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        ///     Dependencies used to create a <see cref="ModelValidator" />
        /// </summary>
        protected virtual ModelValidatorDependencies Dependencies { get; }

        /// <summary>
        ///     Validates a model, throwing an exception if any errors are found.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        public virtual void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            ((Model)model).MakeReadonly();

            ValidateNoShadowEntities(model, logger);
            ValidateIgnoredMembers(model, logger);
            ValidatePropertyMapping(model, logger);
            ValidateRelationships(model, logger);
            ValidateDefiningNavigations(model, logger);
            ValidateOwnership(model, logger);
            ValidateNonNullPrimaryKeys(model, logger);
            ValidateNoShadowKeys(model, logger);
            ValidateNoMutableKeys(model, logger);
            ValidateNoCycles(model, logger);
            ValidateClrInheritance(model, logger);
            ValidateDiscriminatorValues(model, logger);
            ValidateChangeTrackingStrategy(model, logger);
            ValidateForeignKeys(model, logger);
            ValidateFieldMapping(model, logger);
            ValidateKeylessTypes(model, logger);
            ValidateQueryFilters(model, logger);
            ValidateDefiningQuery(model, logger);
            ValidateData(model, logger);
            LogShadowProperties(model, logger);
        }

        /// <summary>
        ///     Validates relationships.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateRelationships([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                foreach (var foreignKey in entityType.GetDeclaredForeignKeys())
                {
                    if (foreignKey.IsUnique
                        && foreignKey is IConventionForeignKey concreteFk
                        && concreteFk.GetPrincipalEndConfigurationSource() == null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.AmbiguousOneToOneRelationship(
                                foreignKey.DeclaringEntityType.DisplayName() + (foreignKey.DependentToPrincipal == null
                                    ? ""
                                    : "." + foreignKey.DependentToPrincipal.Name),
                                foreignKey.PrincipalEntityType.DisplayName() + (foreignKey.PrincipalToDependent == null
                                    ? ""
                                    : "." + foreignKey.PrincipalToDependent.Name)));
                    }
                }
            }
        }

        /// <summary>
        ///     Validates property mappings.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidatePropertyMapping([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            if (!(model is IConventionModel conventionModel))
            {
                return;
            }

            foreach (var entityType in conventionModel.GetEntityTypes())
            {
                var unmappedProperty = entityType.GetProperties().FirstOrDefault(
                    p => (!ConfigurationSource.Convention.Overrides(p.GetConfigurationSource())
                          || !p.IsShadowProperty())
                         && p.FindTypeMapping() == null);

                if (unmappedProperty != null)
                {
                    throw new InvalidOperationException(
                        CoreStrings.PropertyNotMapped(
                            entityType.DisplayName(), unmappedProperty.Name, unmappedProperty.ClrType.ShortDisplayName()));
                }

                if (!entityType.HasClrType())
                {
                    continue;
                }

                var clrProperties = new HashSet<string>(StringComparer.Ordinal);

                var runtimeProperties = entityType.AsEntityType().GetRuntimeProperties();

                clrProperties.UnionWith(
                    runtimeProperties.Values
                        .Where(pi => pi.IsCandidateProperty())
                        .Select(pi => pi.GetSimpleMemberName()));

                clrProperties.ExceptWith(entityType.GetProperties().Select(p => p.Name));
                clrProperties.ExceptWith(entityType.GetNavigations().Select(p => p.Name));
                clrProperties.ExceptWith(entityType.GetServiceProperties().Select(p => p.Name));
                clrProperties.RemoveWhere(p => entityType.FindIgnoredConfigurationSource(p) != null);

                if (clrProperties.Count <= 0)
                {
                    continue;
                }

                foreach (var clrProperty in clrProperties)
                {
                    var actualProperty = runtimeProperties[clrProperty];
                    var propertyType = actualProperty.PropertyType;
                    var targetSequenceType = propertyType.TryGetSequenceType();

                    if (conventionModel.FindIgnoredConfigurationSource(propertyType.DisplayName()) != null
                        || targetSequenceType != null
                        && conventionModel.FindIgnoredConfigurationSource(targetSequenceType.DisplayName()) != null)
                    {
                        continue;
                    }

                    var targetType = FindCandidateNavigationPropertyType(actualProperty);

                    var isTargetWeakOrOwned
                        = targetType != null
                          && (conventionModel.HasEntityTypeWithDefiningNavigation(targetType)
                              || conventionModel.IsOwned(targetType));

                    if (targetType?.IsValidEntityType() == true
                        && (isTargetWeakOrOwned
                            || conventionModel.FindEntityType(targetType) != null
                            || targetType.GetRuntimeProperties().Any(p => p.IsCandidateProperty())))
                    {
                        // ReSharper disable CheckForReferenceEqualityInstead.1
                        // ReSharper disable CheckForReferenceEqualityInstead.3
                        if ((!entityType.IsKeyless
                             || targetSequenceType == null)
                            && entityType.GetDerivedTypes().All(
                                dt => dt.GetDeclaredNavigations().FirstOrDefault(n => n.Name == actualProperty.GetSimpleMemberName()) == null)
                            && (!isTargetWeakOrOwned
                                || (!targetType.Equals(entityType.ClrType)
                                    && (!entityType.IsInOwnershipPath(targetType)
                                        || (entityType.FindOwnership().PrincipalEntityType.ClrType.Equals(targetType)
                                            && targetSequenceType == null))
                                    && (!entityType.IsInDefinitionPath(targetType)
                                        || (entityType.DefiningEntityType.ClrType.Equals(targetType)
                                            && targetSequenceType == null)))))
                        {
                            if (conventionModel.IsOwned(entityType.ClrType)
                                && conventionModel.IsOwned(targetType))
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.AmbiguousOwnedNavigation(entityType.ClrType.ShortDisplayName(), targetType.ShortDisplayName()));
                            }

                            throw new InvalidOperationException(
                                CoreStrings.NavigationNotAdded(
                                    entityType.DisplayName(), actualProperty.Name, propertyType.ShortDisplayName()));
                        }
                        // ReSharper restore CheckForReferenceEqualityInstead.3
                        // ReSharper restore CheckForReferenceEqualityInstead.1
                    }
                    else if (targetSequenceType == null && propertyType.GetTypeInfo().IsInterface
                             || targetSequenceType?.GetTypeInfo().IsInterface == true)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.InterfacePropertyNotAdded(
                                entityType.DisplayName(), actualProperty.Name, propertyType.ShortDisplayName()));
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            CoreStrings.PropertyNotAdded(
                                entityType.DisplayName(), actualProperty.Name, propertyType.ShortDisplayName()));
                    }
                }
            }
        }

        private Type FindCandidateNavigationPropertyType(PropertyInfo propertyInfo)
            => Dependencies.MemberClassifier.FindCandidateNavigationPropertyType(propertyInfo);

        /// <summary>
        ///     Validates that no attempt is made to ignore inherited properties.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateIgnoredMembers([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            if (!(model is IConventionModel conventionModel))
            {
                return;
            }

            foreach (var entityType in conventionModel.GetEntityTypes())
            {
                foreach (var ignoredMember in entityType.GetIgnoredMembers())
                {
                    if (entityType.FindIgnoredConfigurationSource(ignoredMember) != ConfigurationSource.Explicit)
                    {
                        continue;
                    }

                    var property = entityType.FindProperty(ignoredMember);
                    if (property != null)
                    {
                        if (property.DeclaringEntityType != entityType)
                        {
                            throw new InvalidOperationException(
                                CoreStrings.InheritedPropertyCannotBeIgnored(
                                    ignoredMember, entityType.DisplayName(), property.DeclaringEntityType.DisplayName()));
                        }

                        Debug.Assert(false);
                    }

                    var navigation = entityType.FindNavigation(ignoredMember);
                    if (navigation != null)
                    {
                        if (navigation.DeclaringEntityType != entityType)
                        {
                            throw new InvalidOperationException(
                                CoreStrings.InheritedPropertyCannotBeIgnored(
                                    ignoredMember, entityType.DisplayName(), navigation.DeclaringEntityType.DisplayName()));
                        }

                        Debug.Assert(false);
                    }
                }
            }
        }

        /// <summary>
        ///     Validates that the model does not contain any entity types without a corresponding CLR type.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateNoShadowEntities([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var firstShadowEntity = model.GetEntityTypes().FirstOrDefault(entityType => !entityType.HasClrType());
            if (firstShadowEntity != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.ShadowEntity(firstShadowEntity.DisplayName()));
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of shadow keys in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateNoShadowKeys([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes().Where(t => t.ClrType != null))
            {
                foreach (var key in entityType.GetDeclaredKeys())
                {
                    if (key.Properties.Any(p => p.IsShadowProperty())
                        && key is Key concreteKey
                        && ConfigurationSource.Convention.Overrides(concreteKey.GetConfigurationSource())
                        && !key.IsPrimaryKey())
                    {
                        var referencingFk = key.GetReferencingForeignKeys().FirstOrDefault();

                        if (referencingFk != null)
                        {
                            throw new InvalidOperationException(
                                CoreStrings.ReferencedShadowKey(
                                    referencingFk.DeclaringEntityType.DisplayName() +
                                    (referencingFk.DependentToPrincipal == null
                                        ? ""
                                        : "." + referencingFk.DependentToPrincipal.Name),
                                    entityType.DisplayName() +
                                    (referencingFk.PrincipalToDependent == null
                                        ? ""
                                        : "." + referencingFk.PrincipalToDependent.Name),
                                    referencingFk.Properties.Format(includeTypes: true),
                                    entityType.FindPrimaryKey().Properties.Format(includeTypes: true)));
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of mutable in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateNoMutableKeys([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                foreach (var key in entityType.GetDeclaredKeys())
                {
                    var mutableProperty = key.Properties.FirstOrDefault(p => p.ValueGenerated.HasFlag(ValueGenerated.OnUpdate));
                    if (mutableProperty != null)
                    {
                        throw new InvalidOperationException(CoreStrings.MutableKeyProperty(mutableProperty.Name));
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of the model for cycles.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateNoCycles([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var unvalidatedEntityTypes = new HashSet<IEntityType>(model.GetEntityTypes());
            foreach (var entityType in model.GetEntityTypes())
            {
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey != null)
                {
                    var identifyingForeignKeys = new Queue<IForeignKey>(
                        entityType.FindForeignKeys(primaryKey.Properties).Where(fk => fk.PrincipalEntityType != entityType));
                    while (identifyingForeignKeys.Count > 0)
                    {
                        var fk = identifyingForeignKeys.Dequeue();
                        if (!fk.PrincipalKey.IsPrimaryKey()
                            || !unvalidatedEntityTypes.Contains(fk.PrincipalEntityType))
                        {
                            continue;
                        }

                        if (fk.PrincipalEntityType == entityType)
                        {
                            throw new InvalidOperationException(CoreStrings.IdentifyingRelationshipCycle(entityType.DisplayName()));
                        }

                        foreach (var principalFk in fk.PrincipalEntityType.FindForeignKeys(fk.PrincipalKey.Properties))
                        {
                            if (principalFk.PrincipalEntityType != principalFk.DeclaringEntityType)
                            {
                                identifyingForeignKeys.Enqueue(principalFk);
                            }
                        }
                    }
                }

                unvalidatedEntityTypes.Remove(entityType);
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of primary key nullability in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateNonNullPrimaryKeys([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var entityTypeWithNullPk
                = model.GetEntityTypes()
                    .FirstOrDefault(et => !((EntityType)et).IsKeyless && et.BaseType == null && et.FindPrimaryKey() == null);

            if (entityTypeWithNullPk != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.EntityRequiresKey(entityTypeWithNullPk.DisplayName()));
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of inheritance in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateClrInheritance([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var validEntityTypes = new HashSet<IEntityType>();
            foreach (var entityType in model.GetEntityTypes())
            {
                ValidateClrInheritance(model, entityType, validEntityTypes);
            }
        }

        private void ValidateClrInheritance(
            [NotNull] IModel model,
            [NotNull] IEntityType entityType,
            [NotNull] HashSet<IEntityType> validEntityTypes)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(validEntityTypes, nameof(validEntityTypes));

            if (validEntityTypes.Contains(entityType))
            {
                return;
            }

            if (!entityType.HasDefiningNavigation()
                && entityType.FindDeclaredOwnership() == null
                && entityType.BaseType != null)
            {
                var baseClrType = entityType.ClrType?.GetTypeInfo().BaseType;
                while (baseClrType != null)
                {
                    var baseEntityType = model.FindEntityType(baseClrType);
                    if (baseEntityType != null)
                    {
                        if (!baseEntityType.IsAssignableFrom(entityType))
                        {
                            throw new InvalidOperationException(
                                CoreStrings.InconsistentInheritance(entityType.DisplayName(), baseEntityType.DisplayName()));
                        }

                        break;
                    }

                    baseClrType = baseClrType.GetTypeInfo().BaseType;
                }
            }

            if (entityType.ClrType?.IsInstantiable() == false
                && !entityType.GetDerivedTypes().Any())
            {
                throw new InvalidOperationException(
                    CoreStrings.AbstractLeafEntityType(entityType.DisplayName()));
            }

            validEntityTypes.Add(entityType);
        }

        /// <summary>
        ///     Validates the mapping/configuration of inheritance in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateDiscriminatorValues([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            foreach (var rootEntityType in model.GetRootEntityTypes())
            {
                ValidateDiscriminatorValues(rootEntityType);
            }
        }

        private static void ValidateDiscriminatorValues(IEntityType rootEntityType)
        {
            var discriminatorValues = new Dictionary<object, IEntityType>();
            var derivedTypes = rootEntityType.GetDerivedTypesInclusive().ToList();
            if (derivedTypes.Count == 1)
            {
                return;
            }

            if (rootEntityType.GetDiscriminatorProperty() == null)
            {
                throw new InvalidOperationException(
                    CoreStrings.NoDiscriminatorProperty(rootEntityType.DisplayName()));
            }

            foreach (var derivedType in derivedTypes)
            {
                if (derivedType.ClrType?.IsInstantiable() != true)
                {
                    continue;
                }

                var discriminatorValue = derivedType.GetDiscriminatorValue();
                if (discriminatorValue == null)
                {
                    throw new InvalidOperationException(
                        CoreStrings.NoDiscriminatorValue(derivedType.DisplayName()));
                }

                if (discriminatorValues.TryGetValue(discriminatorValue, out var duplicateEntityType))
                {
                    throw new InvalidOperationException(
                        CoreStrings.DuplicateDiscriminatorValue(
                            derivedType.DisplayName(), discriminatorValue, duplicateEntityType.DisplayName()));
                }

                discriminatorValues[discriminatorValue] = derivedType;
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of change tracking in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateChangeTrackingStrategy([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                var errorMessage = entityType.AsEntityType().CheckChangeTrackingStrategy(entityType.GetChangeTrackingStrategy());
                if (errorMessage != null)
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of ownership in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateOwnership([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                var ownerships = entityType.GetForeignKeys().Where(fk => fk.IsOwnership).ToList();
                if (ownerships.Count > 1)
                {
                    throw new InvalidOperationException(CoreStrings.MultipleOwnerships(entityType.DisplayName()));
                }

                if (ownerships.Count == 1)
                {
                    var ownership = ownerships[0];
                    if (entityType.BaseType != null
                        && ownership.DeclaringEntityType == entityType)
                    {
                        throw new InvalidOperationException(CoreStrings.OwnedDerivedType(entityType.DisplayName()));
                    }

                    foreach (var referencingFk in entityType.GetReferencingForeignKeys().Where(
                        fk => !fk.IsOwnership
                              && !Contains(fk.DeclaringEntityType.FindOwnership(), fk)))
                    {
                        throw new InvalidOperationException(
                            CoreStrings.PrincipalOwnedType(
                                referencingFk.DeclaringEntityType.DisplayName() +
                                (referencingFk.DependentToPrincipal == null
                                    ? ""
                                    : "." + referencingFk.DependentToPrincipal.Name),
                                referencingFk.PrincipalEntityType.DisplayName() +
                                (referencingFk.PrincipalToDependent == null
                                    ? ""
                                    : "." + referencingFk.PrincipalToDependent.Name),
                                entityType.DisplayName()));
                    }

                    foreach (var fk in entityType.GetDeclaredForeignKeys().Where(
                        fk => !fk.IsOwnership && fk.PrincipalToDependent != null
                                              && !Contains(fk.DeclaringEntityType.FindOwnership(), fk)))
                    {
                        throw new InvalidOperationException(
                            CoreStrings.InverseToOwnedType(
                                fk.PrincipalEntityType.DisplayName(),
                                fk.PrincipalToDependent.Name,
                                entityType.DisplayName(),
                                ownership.PrincipalEntityType.DisplayName()));
                    }
                }
                else if (entityType.HasClrType() && ((IMutableModel)model).IsOwned(entityType.ClrType))
                {
                    throw new InvalidOperationException(CoreStrings.OwnerlessOwnedType(entityType.DisplayName()));
                }
            }
        }

        private bool Contains(IForeignKey inheritedFk, IForeignKey derivedFk)
            => inheritedFk != null
               && inheritedFk.PrincipalEntityType.IsAssignableFrom(derivedFk.PrincipalEntityType)
               && PropertyListComparer.Instance.Equals(inheritedFk.Properties, derivedFk.Properties);

        /// <summary>
        ///     Validates the mapping/configuration of foreign keys in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateForeignKeys([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                foreach (var declaredForeignKey in entityType.GetDeclaredForeignKeys())
                {
                    if (declaredForeignKey.PrincipalEntityType == declaredForeignKey.DeclaringEntityType
                        && PropertyListComparer.Instance.Equals(declaredForeignKey.PrincipalKey.Properties, declaredForeignKey.Properties))
                    {
                        logger.RedundantForeignKeyWarning(declaredForeignKey);
                    }

                    if (entityType.BaseType == null)
                    {
                        continue;
                    }

                    var inheritedKey = declaredForeignKey.Properties.Where(p => p.ValueGenerated != ValueGenerated.Never)
                        .SelectMany(p => p.GetContainingKeys().Where(k => k.DeclaringEntityType != entityType)).FirstOrDefault();
                    if (inheritedKey != null)
                    {
                        var generatedProperty = declaredForeignKey.Properties.First(
                            p => p.ValueGenerated != ValueGenerated.Never && inheritedKey.Properties.Contains(p));

                        if (entityType.BaseType.ClrType.IsAbstract
                            && entityType.BaseType.GetDerivedTypes().All(
                                d => d.GetDeclaredForeignKeys().Any(fk => fk.Properties.Contains(generatedProperty))))
                        {
                            continue;
                        }

                        throw new InvalidOperationException(
                            CoreStrings.ForeignKeyPropertyInKey(
                                generatedProperty.Name,
                                entityType.DisplayName(),
                                inheritedKey.Properties.Format(),
                                inheritedKey.DeclaringEntityType.DisplayName()));
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of defining navigations in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateDefiningNavigations([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.DefiningEntityType != null)
                {
                    if (entityType.FindDefiningNavigation() == null
                        || (entityType.DefiningEntityType as EntityType)?.Builder == null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.NoDefiningNavigation(
                                entityType.DefiningNavigationName, entityType.DisplayName(), entityType.DefiningEntityType.DisplayName()));
                    }

                    var ownership = entityType.GetForeignKeys().SingleOrDefault(fk => fk.IsOwnership);
                    if (ownership != null)
                    {
                        if (ownership.PrincipalToDependent?.Name != entityType.DefiningNavigationName)
                        {
                            var ownershipNavigation = ownership.PrincipalToDependent == null
                                ? ""
                                : "." + ownership.PrincipalToDependent.Name;
                            throw new InvalidOperationException(
                                CoreStrings.NonDefiningOwnership(
                                    ownership.PrincipalEntityType.DisplayName() + ownershipNavigation,
                                    entityType.DefiningNavigationName,
                                    entityType.DisplayName()));
                        }

                        foreach (var otherEntityType in model.GetEntityTypes()
                            .Where(et => et.ClrType == entityType.ClrType && et != entityType))
                        {
                            if (!otherEntityType.GetForeignKeys().Any(fk => fk.IsOwnership))
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.InconsistentOwnership(entityType.DisplayName(), otherEntityType.DisplayName()));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of properties mapped to fields in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateFieldMapping([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                var properties = new HashSet<IPropertyBase>(
                    entityType
                        .GetDeclaredProperties()
                        .Cast<IPropertyBase>()
                        .Concat(entityType.GetDeclaredNavigations())
                        .Where(p => !p.IsShadowProperty()));

                var constructorBinding = (InstantiationBinding)entityType[CoreAnnotationNames.ConstructorBinding];

                if (constructorBinding != null)
                {
                    foreach (var consumedProperty in constructorBinding.ParameterBindings.SelectMany(p => p.ConsumedProperties))
                    {
                        properties.Remove(consumedProperty);
                    }
                }

                foreach (var propertyBase in properties)
                {
                    if (!propertyBase.TryGetMemberInfo(
                        forConstruction: true,
                        forSet: true,
                        memberInfo: out _,
                        errorMessage: out var errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }

                    if (!propertyBase.TryGetMemberInfo(
                        forConstruction: false,
                        forSet: true,
                        memberInfo: out _,
                        errorMessage: out errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }

                    if (!propertyBase.TryGetMemberInfo(
                        forConstruction: false,
                        forSet: false,
                        memberInfo: out _,
                        errorMessage: out errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of entity types without keys in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateKeylessTypes([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.GetDefiningQuery() != null)
                {
                    if (entityType.BaseType != null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.DerivedTypeDefiningQuery(entityType.DisplayName(), entityType.BaseType.DisplayName()));
                    }

                    if (entityType.FindPrimaryKey() != null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.DefiningQueryWithKey(entityType.DisplayName()));
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of query filters in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateQueryFilters([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.GetQueryFilter() != null
                    && entityType.BaseType != null)
                {
                    throw new InvalidOperationException(
                        CoreStrings.BadFilterDerivedType(entityType.GetQueryFilter(), entityType.DisplayName()));
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of defining queries in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateDefiningQuery(
            [NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.GetDefiningQuery() != null
                    && entityType.FindPrimaryKey() != null)
                {
                    throw new InvalidOperationException(
                        CoreStrings.DefiningQueryWithKey(entityType.DisplayName()));
                }
            }
        }

        /// <summary>
        ///     Validates the mapping/configuration of data (e.g. seed data) in the model.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void ValidateData([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var identityMaps = new Dictionary<IKey, IIdentityMap>();
            var sensitiveDataLogged = logger.ShouldLogSensitiveData();

            foreach (var entityType in model.GetEntityTypes())
            {
                var key = entityType.FindPrimaryKey();
                if (key == null)
                {
                    continue;
                }

                IIdentityMap identityMap = null;
                foreach (var seedDatum in entityType.GetSeedData())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (!seedDatum.TryGetValue(property.Name, out var value)
                            || value == null)
                        {
                            if (!property.IsNullable
                                && ((!property.RequiresValueGenerator()
                                     && (property.ValueGenerated & ValueGenerated.OnAdd) == 0)
                                    || property.IsPrimaryKey()))
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.SeedDatumMissingValue(entityType.DisplayName(), property.Name));
                            }
                        }
                        else if (property.RequiresValueGenerator()
                                 && property.IsPrimaryKey()
                                 && property.ClrType.IsDefaultValue(value))
                        {
                            if (property.ClrType.IsSignedInteger())
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.SeedDatumSignedNumericValue(entityType.DisplayName(), property.Name));
                            }

                            throw new InvalidOperationException(
                                CoreStrings.SeedDatumDefaultValue(
                                    entityType.DisplayName(), property.Name, property.ClrType.GetDefaultValue()));
                        }
                        else if (!property.ClrType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
                        {
                            if (sensitiveDataLogged)
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.SeedDatumIncompatibleValueSensitive(
                                        entityType.DisplayName(), value, property.Name, property.ClrType.DisplayName()));
                            }

                            throw new InvalidOperationException(
                                CoreStrings.SeedDatumIncompatibleValue(
                                    entityType.DisplayName(), property.Name, property.ClrType.DisplayName()));
                        }
                    }

                    var keyValues = new object[key.Properties.Count];
                    for (var i = 0; i < key.Properties.Count; i++)
                    {
                        keyValues[i] = seedDatum[key.Properties[i].Name];
                    }

                    foreach (var navigation in entityType.GetNavigations())
                    {
                        if (seedDatum.TryGetValue(navigation.Name, out var value)
                            && ((navigation.IsCollection() && value is IEnumerable collection && collection.Any())
                                || (!navigation.IsCollection() && value != null)))
                        {
                            if (sensitiveDataLogged)
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.SeedDatumNavigationSensitive(
                                        entityType.DisplayName(),
                                        string.Join(", ", key.Properties.Select((p, i) => p.Name + ":" + keyValues[i])),
                                        navigation.Name,
                                        navigation.GetTargetType().DisplayName(),
                                        navigation.ForeignKey.Properties.Format()));
                            }

                            throw new InvalidOperationException(
                                CoreStrings.SeedDatumNavigation(
                                    entityType.DisplayName(),
                                    navigation.Name,
                                    navigation.GetTargetType().DisplayName(),
                                    navigation.ForeignKey.Properties.Format()));
                        }
                    }

                    if (identityMap == null)
                    {
                        if (!identityMaps.TryGetValue(key, out identityMap))
                        {
                            identityMap = key.GetIdentityMapFactory()(sensitiveDataLogged);
                            identityMaps[key] = identityMap;
                        }
                    }

                    var entry = identityMap.TryGetEntry(keyValues);
                    if (entry != null)
                    {
                        if (sensitiveDataLogged)
                        {
                            throw new InvalidOperationException(
                                CoreStrings.SeedDatumDuplicateSensitive(
                                    entityType.DisplayName(),
                                    string.Join(", ", key.Properties.Select((p, i) => p.Name + ":" + keyValues[i]))));
                        }

                        throw new InvalidOperationException(
                            CoreStrings.SeedDatumDuplicate(
                                entityType.DisplayName(), key.Properties.Format()));
                    }

                    entry = new InternalShadowEntityEntry(null, entityType);

                    identityMap.Add(keyValues, entry);
                }
            }
        }

        /// <summary>
        ///     Logs all shadow properties that were created because there was no matching CLR member.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        /// <param name="logger"> The logger to use. </param>
        protected virtual void LogShadowProperties([NotNull] IModel model, [NotNull] IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes().Where(t => t.ClrType != null))
            {
                foreach (var property in entityType.GetDeclaredProperties())
                {
                    if (property.IsShadowProperty())
                    {
                        logger.ShadowPropertyCreated(property);
                    }
                }
            }
        }
    }
}
