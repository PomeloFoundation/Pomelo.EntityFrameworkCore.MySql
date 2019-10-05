// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class InternalMixedEntityEntry : InternalEntityEntry
    {
        private readonly ISnapshot _shadowValues;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public InternalMixedEntityEntry(
            [NotNull] IStateManager stateManager,
            [NotNull] IEntityType entityType,
            [NotNull] object entity)
            : base(stateManager, entityType)
        {
            Entity = entity;
            _shadowValues = entityType.GetEmptyShadowValuesFactory()();

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            MarkShadowPropertiesNotSet(entityType);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public InternalMixedEntityEntry(
            [NotNull] IStateManager stateManager,
            [NotNull] IEntityType entityType,
            [NotNull] object entity,
            in ValueBuffer valueBuffer)
            : base(stateManager, entityType)
        {
            Entity = entity;
            _shadowValues = ((EntityType)entityType).ShadowValuesFactory(valueBuffer);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override object Entity { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override T ReadShadowValue<T>(int shadowIndex)
            => _shadowValues.GetValue<T>(shadowIndex);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override object ReadPropertyValue(IPropertyBase propertyBase)
            => !propertyBase.IsShadowProperty()
                ? base.ReadPropertyValue(propertyBase)
                : _shadowValues[propertyBase.GetShadowIndex()];

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override bool PropertyHasDefaultValue(IPropertyBase propertyBase)
            => !propertyBase.IsShadowProperty()
                ? base.PropertyHasDefaultValue(propertyBase)
                : propertyBase.ClrType.IsDefaultValue(_shadowValues[propertyBase.GetShadowIndex()]);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void WritePropertyValue(IPropertyBase propertyBase, object value, bool forMaterialization)
        {
            if (!propertyBase.IsShadowProperty())
            {
                base.WritePropertyValue(propertyBase, value, forMaterialization);
            }
            else
            {
                _shadowValues[propertyBase.GetShadowIndex()] = value;
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override object GetOrCreateCollection(INavigation navigation, bool forMaterialization)
            => navigation.IsShadowProperty()
                ? GetOrCreateCollectionTyped(navigation)
                : base.GetOrCreateCollection(navigation,forMaterialization);

        private ICollection<object> GetOrCreateCollectionTyped(INavigation navigation)
        {
            if (!(_shadowValues[navigation.GetShadowIndex()] is ICollection<object> collection))
            {
                collection = new HashSet<object>();
                _shadowValues[navigation.GetShadowIndex()] = collection;
            }

            return collection;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool CollectionContains(INavigation navigation, InternalEntityEntry value)
            => navigation.IsShadowProperty()
                ? GetOrCreateCollectionTyped(navigation).Contains(value.Entity)
                : base.CollectionContains(navigation, value);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool AddToCollection(INavigation navigation, InternalEntityEntry value, bool forMaterialization)
        {
            if (!navigation.IsShadowProperty())
            {
                return base.AddToCollection(navigation, value, forMaterialization);
            }

            if (navigation.GetTargetType().ClrType == null)
            {
                return false;
            }

            var collection = GetOrCreateCollectionTyped(navigation);
            if (!collection.Contains(value.Entity))
            {
                collection.Add(value.Entity);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool RemoveFromCollection(INavigation navigation, InternalEntityEntry value)
            => navigation.IsShadowProperty()
                ? GetOrCreateCollectionTyped(navigation).Remove(value.Entity)
                : base.RemoveFromCollection(navigation, value);
    }
}
