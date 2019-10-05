// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    // This is lower-level change tracking services used by the ChangeTracker and other parts of the system
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped"/>. This means that each
    ///         <see cref="DbContext"/> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    public class StateManager : IStateManager
    {
        private readonly EntityReferenceMap _entityReferenceMap
            = new EntityReferenceMap(hasSubMap: true);

        private IDictionary<object, IList<Tuple<INavigation, InternalEntityEntry>>> _referencedUntrackedEntities;
        private IIdentityMap _identityMap0;
        private IIdentityMap _identityMap1;
        private Dictionary<IKey, IIdentityMap> _identityMaps;
        private bool _needsUnsubscribe;

        private readonly IDiagnosticsLogger<DbLoggerCategory.ChangeTracking> _changeTrackingLogger;
        private readonly IInternalEntityEntryFactory _internalEntityEntryFactory;
        private readonly IInternalEntityEntrySubscriber _internalEntityEntrySubscriber;
        private readonly IModel _model;
        private readonly IDatabase _database;
        private readonly IConcurrencyDetector _concurrencyDetector;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public StateManager([NotNull] StateManagerDependencies dependencies)
        {
            Dependencies = dependencies;

            _internalEntityEntryFactory = dependencies.InternalEntityEntryFactory;
            _internalEntityEntrySubscriber = dependencies.InternalEntityEntrySubscriber;
            InternalEntityEntryNotifier = dependencies.InternalEntityEntryNotifier;
            ValueGenerationManager = dependencies.ValueGenerationManager;
            _model = dependencies.Model;
            _database = dependencies.Database;
            _concurrencyDetector = dependencies.ConcurrencyDetector;
            Context = dependencies.CurrentContext.Context;
            EntityFinderFactory = new EntityFinderFactory(dependencies.EntityFinderSource, this, dependencies.SetSource, dependencies.CurrentContext.Context);
            EntityMaterializerSource = dependencies.EntityMaterializerSource;

            if (dependencies.LoggingOptions.IsSensitiveDataLoggingEnabled)
            {
                SensitiveLoggingEnabled = true;
            }

            UpdateLogger = dependencies.UpdateLogger;
            _changeTrackingLogger = dependencies.ChangeTrackingLogger;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual StateManagerDependencies Dependencies { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual bool SensitiveLoggingEnabled { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CascadeTiming DeleteOrphansTiming { get; set; } = CascadeTiming.Immediate;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CascadeTiming CascadeDeleteTiming { get; set; } = CascadeTiming.Immediate;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IInternalEntityEntryNotifier InternalEntityEntryNotifier { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void StateChanging(InternalEntityEntry entry, EntityState newState)
        {
            InternalEntityEntryNotifier.StateChanging(entry, newState);

            UpdateReferenceMaps(entry, newState, entry.EntityState);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IValueGenerationManager ValueGenerationManager { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DbContext Context { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IModel Model => _model;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEntityFinderFactory EntityFinderFactory { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEntityMaterializerSource EntityMaterializerSource { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry GetOrCreateEntry(object entity)
        {
            var entry = TryGetEntry(entity);
            if (entry == null)
            {
                var entityType = _model.FindRuntimeEntityType(entity.GetType());
                if (entityType == null)
                {
                    if (_model.HasEntityTypeWithDefiningNavigation(entity.GetType()))
                    {
                        throw new InvalidOperationException(
                            CoreStrings.UntrackedDependentEntity(
                                entity.GetType().ShortDisplayName(),
                                "." + nameof(EntityEntry.Reference) + "()." + nameof(ReferenceEntry.TargetEntry),
                                "." + nameof(EntityEntry.Collection) + "()." + nameof(CollectionEntry.FindEntry) +
                                "()"));
                    }

                    throw new InvalidOperationException(CoreStrings.EntityTypeNotFound(entity.GetType().ShortDisplayName()));
                }

                if (entityType.FindPrimaryKey() == null)
                {
                    throw new InvalidOperationException(CoreStrings.KeylessTypeTracked(entityType.DisplayName()));
                }

                entry = _internalEntityEntryFactory.Create(this, entityType, entity);

                UpdateReferenceMaps(entry, EntityState.Detached, null);
            }

            return entry;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry GetOrCreateEntry(object entity, IEntityType entityType)
        {
            var entry = TryGetEntry(entity, entityType);
            if (entry == null)
            {
                var runtimeEntityType = _model.FindRuntimeEntityType(entity.GetType());
                if (runtimeEntityType != null)
                {
                    if (!entityType.IsAssignableFrom(runtimeEntityType))
                    {
                        throw new InvalidOperationException(CoreStrings.TrackingTypeMismatch(
                            runtimeEntityType.DisplayName(), entityType.DisplayName()));
                    }
                    entityType = runtimeEntityType;
                }

                if (entityType.FindPrimaryKey() == null)
                {
                    throw new InvalidOperationException(CoreStrings.KeylessTypeTracked(entityType.DisplayName()));
                }

                entry = _internalEntityEntryFactory.Create(this, entityType, entity);

                UpdateReferenceMaps(entry, EntityState.Detached, null);
            }

            return entry;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry CreateEntry(IDictionary<string, object> values, IEntityType entityType)
        {
            var (i, j) = (0, 0);
            var valuesArray = new object[entityType.PropertyCount()];
            var shadowPropertyValuesArray = new object[entityType.ShadowPropertyCount()];
            foreach (var property in entityType.GetProperties())
            {
                valuesArray[i++] = values.TryGetValue(property.Name, out var value)
                    ? value
                    : property.ClrType.GetDefaultValue();

                if (property.IsShadowProperty())
                {
                    shadowPropertyValuesArray[j++] = values.TryGetValue(property.Name, out var shadowValue)
                        ? shadowValue
                        : property.ClrType.GetDefaultValue();
                }
            }

            var valueBuffer = new ValueBuffer(valuesArray);
            var entity = entityType.HasClrType()
                ? EntityMaterializerSource.GetMaterializer(entityType)(
                    new MaterializationContext(valueBuffer, Context))
                : null;

            var shadowPropertyValueBuffer = new ValueBuffer(shadowPropertyValuesArray);
            var entry = _internalEntityEntryFactory.Create(this, entityType, entity, shadowPropertyValueBuffer);

            UpdateReferenceMaps(entry, EntityState.Detached, null);

            return entry;
        }

        private void UpdateReferenceMaps(
            InternalEntityEntry entry,
            EntityState state,
            EntityState? oldState)
        {
            var entityType = entry.EntityType;
            var mapKey = entry.Entity ?? entry;
            if (entityType.HasDefiningNavigation())
            {
                foreach (var otherType in _model.GetEntityTypes(entityType.Name)
                    .Where(et => et != entityType && TryGetEntry(mapKey, et) != null))
                {
                    UpdateLogger.DuplicateDependentEntityTypeInstanceWarning(entityType, otherType);
                }
            }

            _entityReferenceMap.Update(entry, state, oldState);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry StartTrackingFromQuery(
            IEntityType baseEntityType,
            object entity,
            in ValueBuffer valueBuffer)
        {
            var existingEntry = TryGetEntry(entity);
            if (existingEntry != null)
            {
                return existingEntry;
            }

            var clrType = entity.GetType();
            var entityType = baseEntityType.ClrType == clrType
                             || baseEntityType.HasDefiningNavigation()
                ? baseEntityType
                : _model.FindRuntimeEntityType(clrType);

            var newEntry = valueBuffer.IsEmpty
                ? _internalEntityEntryFactory.Create(this, entityType, entity)
                : _internalEntityEntryFactory.Create(this, entityType, entity, valueBuffer);

            foreach (var key in baseEntityType.GetKeys())
            {
                GetOrCreateIdentityMap(key).AddOrUpdate(newEntry);
            }

            UpdateReferenceMaps(newEntry, EntityState.Unchanged, null);

            newEntry.MarkUnchangedFromQuery();

            if (_internalEntityEntrySubscriber.SnapshotAndSubscribe(newEntry))
            {
                _needsUnsubscribe = true;
            }

            return newEntry;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry TryGetEntry(IKey key, object[] keyValues)
            => FindIdentityMap(key)?.TryGetEntry(keyValues);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry TryGetEntry(IKey key, object[] keyValues, bool throwOnNullKey, out bool hasNullKey)
            => GetOrCreateIdentityMap(key).TryGetEntry(keyValues, throwOnNullKey, out hasNullKey);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry TryGetEntry(object entity, bool throwOnNonUniqueness = true)
            => _entityReferenceMap.TryGet(entity, null, out var entry, throwOnNonUniqueness)
                ? entry
                : null;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry TryGetEntry(object entity, IEntityType entityType, bool throwOnTypeMismatch = true)
        {
            var found = _entityReferenceMap.TryGet(entity, entityType, out var entry, throwOnNonUniqueness: false);
            if (found
                && !entityType.IsAssignableFrom(entry.EntityType))
            {
                if (throwOnTypeMismatch)
                {
                    throw new InvalidOperationException(CoreStrings.TrackingTypeMismatch(
                        entry.EntityType.DisplayName(), entityType.DisplayName()));
                }
                else
                {
                    return null;
                }
            }

            return found ? entry : null;
        }

        private IIdentityMap GetOrCreateIdentityMap(IKey key)
        {
            if (_identityMap0 == null)
            {
                _identityMap0 = key.GetIdentityMapFactory()(SensitiveLoggingEnabled);
                return _identityMap0;
            }

            if (_identityMap0.Key == key)
            {
                return _identityMap0;
            }

            if (_identityMap1 == null)
            {
                _identityMap1 = key.GetIdentityMapFactory()(SensitiveLoggingEnabled);
                return _identityMap1;
            }

            if (_identityMap1.Key == key)
            {
                return _identityMap1;
            }

            if (_identityMaps == null)
            {
                _identityMaps = new Dictionary<IKey, IIdentityMap>();
            }

            if (!_identityMaps.TryGetValue(key, out var identityMap))
            {
                identityMap = key.GetIdentityMapFactory()(SensitiveLoggingEnabled);
                _identityMaps[key] = identityMap;
            }

            return identityMap;
        }

        private IIdentityMap FindIdentityMap(IKey key)
        {
            if (_identityMap0 == null
                || key == null)
            {
                return null;
            }

            if (_identityMap0.Key == key)
            {
                return _identityMap0;
            }

            if (_identityMap1 == null)
            {
                return null;
            }

            if (_identityMap1.Key == key)
            {
                return _identityMap1;
            }

            return _identityMaps == null
                   || !_identityMaps.TryGetValue(key, out var identityMap)
                ? null
                : identityMap;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int GetCountForState(
            bool added = false,
            bool modified = false,
            bool deleted = false,
            bool unchanged = false)
            => _entityReferenceMap.GetCountForState(added, modified, deleted, unchanged);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int Count
            => GetCountForState(added: true, modified: true, deleted: true, unchanged: true);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<InternalEntityEntry> GetEntriesForState(
            bool added = false,
            bool modified = false,
            bool deleted = false,
            bool unchanged = false)
            => _entityReferenceMap.GetEntriesForState(added, modified, deleted, unchanged);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<InternalEntityEntry> Entries
            => GetEntriesForState(added: true, modified: true, deleted: true, unchanged: true);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<TEntity> GetNonDeletedEntities<TEntity>()
            where TEntity : class
            => _entityReferenceMap.GetNonDeletedEntities<TEntity>();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry StartTracking(InternalEntityEntry entry)
        {
            var entityType = (EntityType)entry.EntityType;

            if (entry.StateManager != this)
            {
                throw new InvalidOperationException(CoreStrings.WrongStateManager(entityType.DisplayName()));
            }

            var mapKey = entry.Entity ?? entry;
            var existingEntry = TryGetEntry(mapKey, entityType);

            if (existingEntry != null
                && existingEntry != entry)
            {
                throw new InvalidOperationException(CoreStrings.MultipleEntries(entityType.DisplayName()));
            }

            foreach (var key in entityType.GetKeys())
            {
                GetOrCreateIdentityMap(key).Add(entry);
            }

            if (_internalEntityEntrySubscriber.SnapshotAndSubscribe(entry))
            {
                _needsUnsubscribe = true;
            }

            return entry;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void StopTracking(InternalEntityEntry entry, EntityState oldState)
        {
            if (_needsUnsubscribe)
            {
                _internalEntityEntrySubscriber.Unsubscribe(entry);
            }

            var entityType = entry.EntityType;

            foreach (var key in entityType.GetKeys())
            {
                FindIdentityMap(key)?.Remove(entry);
            }

            if (_referencedUntrackedEntities != null)
            {
                var navigations = entityType.GetNavigations().ToList();

                foreach (var keyValuePair in _referencedUntrackedEntities.ToList())
                {
                    var untrackedEntityType = _model.FindRuntimeEntityType(keyValuePair.Key.GetType());
                    if (navigations.Any(n => n.GetTargetType().IsAssignableFrom(untrackedEntityType))
                        || untrackedEntityType.GetNavigations().Any(n => n.GetTargetType().IsAssignableFrom(entityType)))
                    {
                        _referencedUntrackedEntities.Remove(keyValuePair.Key);

                        var newList = keyValuePair.Value.Where(tuple => tuple.Item2 != entry).ToList();

                        if (newList.Count > 0)
                        {
                            _referencedUntrackedEntities.Add(keyValuePair.Key, newList);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void Unsubscribe()
        {
            if (_needsUnsubscribe)
            {
                foreach (var entry in Entries)
                {
                    _internalEntityEntrySubscriber.Unsubscribe(entry);
                }
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void ResetState()
        {
            Unsubscribe();
            ChangedCount = 0;
            _entityReferenceMap.Clear();
            _referencedUntrackedEntities = null;

            _identityMaps?.Clear();
            _identityMap0?.Clear();
            _identityMap1?.Clear();

            _needsUnsubscribe = false;

            Tracked = null;
            StateChanged = null;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        public virtual Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();

            return default;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void RecordReferencedUntrackedEntity(
            object referencedEntity, INavigation navigation, InternalEntityEntry referencedFromEntry)
        {
            if (_referencedUntrackedEntities == null)
            {
                _referencedUntrackedEntities
                    = new Dictionary<object, IList<Tuple<INavigation, InternalEntityEntry>>>(ReferenceEqualityComparer.Instance);
            }

            if (!_referencedUntrackedEntities.TryGetValue(referencedEntity, out var danglers))
            {
                danglers = new List<Tuple<INavigation, InternalEntityEntry>>();
                _referencedUntrackedEntities.Add(referencedEntity, danglers);
            }

            danglers.Add(Tuple.Create(navigation, referencedFromEntry));
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<Tuple<INavigation, InternalEntityEntry>> GetRecordedReferrers(object referencedEntity, bool clear)
        {
            if (_referencedUntrackedEntities != null
                && _referencedUntrackedEntities.TryGetValue(referencedEntity, out var danglers))
            {
                if (clear)
                {
                    _referencedUntrackedEntities.Remove(referencedEntity);
                }

                return danglers;
            }

            return Enumerable.Empty<Tuple<INavigation, InternalEntityEntry>>();
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry FindPrincipal(
            InternalEntityEntry dependentEntry,
            IForeignKey foreignKey)
            => FilterIncompatiblePrincipal(
                foreignKey,
                FindIdentityMap(foreignKey.PrincipalKey)
                    ?.TryGetEntry(foreignKey, dependentEntry));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry FindPrincipalUsingPreStoreGeneratedValues(
            InternalEntityEntry dependentEntry, IForeignKey foreignKey)
            => FilterIncompatiblePrincipal(
                foreignKey,
                FindIdentityMap(foreignKey.PrincipalKey)
                    ?.TryGetEntryUsingPreStoreGeneratedValues(foreignKey, dependentEntry));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual InternalEntityEntry FindPrincipalUsingRelationshipSnapshot(
            InternalEntityEntry dependentEntry, IForeignKey foreignKey)
            => FilterIncompatiblePrincipal(
                foreignKey,
                FindIdentityMap(foreignKey.PrincipalKey)
                    ?.TryGetEntryUsingRelationshipSnapshot(foreignKey, dependentEntry));

        private static InternalEntityEntry FilterIncompatiblePrincipal(IForeignKey foreignKey,
            InternalEntityEntry principalEntry)
            => principalEntry != null
               && foreignKey.PrincipalEntityType.IsAssignableFrom(principalEntry.EntityType)
                ? principalEntry
                : null;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void UpdateIdentityMap(InternalEntityEntry entry, IKey key)
        {
            if (entry.EntityState == EntityState.Detached)
            {
                return;
            }

            var identityMap = FindIdentityMap(key);
            if (identityMap == null)
            {
                return;
            }

            identityMap.RemoveUsingRelationshipSnapshot(entry);
            identityMap.Add(entry);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void UpdateDependentMap(InternalEntityEntry entry, IForeignKey foreignKey)
        {
            if (entry.EntityState == EntityState.Detached)
            {
                return;
            }

            FindIdentityMap(foreignKey.DeclaringEntityType.FindPrimaryKey())
                ?.FindDependentsMap(foreignKey)
                ?.Update(entry);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<InternalEntityEntry> GetDependents(
            InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            var dependentIdentityMap = FindIdentityMap(foreignKey.DeclaringEntityType.FindPrimaryKey());
            return dependentIdentityMap != null
                ? dependentIdentityMap.GetDependentsMap(foreignKey).GetDependents(principalEntry)
                : Enumerable.Empty<InternalEntityEntry>();
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<InternalEntityEntry> GetDependentsUsingRelationshipSnapshot(
            InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            var dependentIdentityMap = FindIdentityMap(foreignKey.DeclaringEntityType.FindPrimaryKey());
            return dependentIdentityMap != null
                ? dependentIdentityMap.GetDependentsMap(foreignKey).GetDependentsUsingRelationshipSnapshot(principalEntry)
                : Enumerable.Empty<InternalEntityEntry>();
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<InternalEntityEntry> GetDependentsFromNavigation(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            var navigation = foreignKey.PrincipalToDependent;
            if (navigation == null
                || navigation.IsShadowProperty())
            {
                return null;
            }

            var navigationValue = principalEntry[navigation];
            if (navigationValue == null)
            {
                return Enumerable.Empty<InternalEntityEntry>();
            }

            if (foreignKey.IsUnique)
            {
                var dependentEntry = TryGetEntry(navigationValue, foreignKey.DeclaringEntityType);

                return dependentEntry != null
                    ? new[] { dependentEntry }
                    : Enumerable.Empty<InternalEntityEntry>();
            }

            return ((IEnumerable<object>)navigationValue)
                .Select(v => TryGetEntry(v, foreignKey.DeclaringEntityType)).Where(e => e != null);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEntityFinder CreateEntityFinder(IEntityType entityType)
            => EntityFinderFactory.Create(entityType);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int ChangedCount { get; set; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (ChangedCount == 0)
            {
                return 0;
            }

            var entriesToSave = GetEntriesToSave(cascadeChanges: true);
            if (entriesToSave.Count == 0)
            {
                return 0;
            }

            try
            {
                var result = SaveChanges(entriesToSave);

                if (acceptAllChangesOnSuccess)
                {
                    AcceptAllChanges((IReadOnlyList<IUpdateEntry>)entriesToSave);
                }

                return result;
            }
            catch
            {
                foreach (var entry in entriesToSave)
                {
                    ((InternalEntityEntry)entry).DiscardStoreGeneratedValues();
                }

                throw;
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IList<IUpdateEntry> GetEntriesToSave(bool cascadeChanges)
        {
            if (cascadeChanges)
            {
                CascadeChanges(force: false);
            }

            var toSave = new List<IUpdateEntry>(GetCountForState(added: true, modified: true, deleted: true));

            // Perf sensitive

            foreach (var entry in GetEntriesForState(added: true, modified: true, deleted: true))
            {
                toSave.Add(entry.PrepareToSave());
            }

            return toSave;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void CascadeChanges(bool force)
        {
            // Perf sensitive

            var toHandle = new List<InternalEntityEntry>();

            foreach (var entry in GetEntriesForState(modified: true, added: true))
            {
                if (entry.HasConceptualNull)
                {
                    toHandle.Add(entry);
                }
            }

            foreach (var entry in toHandle)
            {
                entry.HandleConceptualNulls(SensitiveLoggingEnabled, force, isCascadeDelete: false);
            }

            foreach (var entry in this.ToListForState(deleted: true))
            {
                CascadeDelete(entry, force);
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void CascadeDelete(InternalEntityEntry entry, bool force, IEnumerable<IForeignKey> foreignKeys = null)
        {
            var doCascadeDelete = force || CascadeDeleteTiming != CascadeTiming.Never;

            foreignKeys ??= entry.EntityType.GetReferencingForeignKeys();
            foreach (var fk in foreignKeys)
            {
                if (fk.DeleteBehavior == DeleteBehavior.ClientNoAction)
                {
                    continue;
                }

                foreach (var dependent in (GetDependentsFromNavigation(entry, fk)
                                           ?? GetDependents(entry, fk)).ToList())
                {
                    if (dependent.EntityState != EntityState.Deleted
                        && dependent.EntityState != EntityState.Detached
                        && (dependent.EntityState == EntityState.Added
                            || KeysEqual(entry, fk, dependent)))
                    {
                        if ((fk.DeleteBehavior == DeleteBehavior.Cascade
                             || fk.DeleteBehavior == DeleteBehavior.ClientCascade)
                            && doCascadeDelete)
                        {
                            var cascadeState = dependent.EntityState == EntityState.Added
                                ? EntityState.Detached
                                : EntityState.Deleted;

                            if (SensitiveLoggingEnabled)
                            {
                                UpdateLogger.CascadeDeleteSensitive(dependent, entry, cascadeState);
                            }
                            else
                            {
                                UpdateLogger.CascadeDelete(dependent, entry, cascadeState);
                            }

                            dependent.SetEntityState(cascadeState);

                            CascadeDelete(dependent, force);
                        }
                        else
                        {
                            foreach (var dependentProperty in fk.Properties)
                            {
                                dependent.SetProperty(dependentProperty, null, isMaterialization: false, setModified: true, isCascadeDelete: true);
                            }

                            if (dependent.HasConceptualNull)
                            {
                                dependent.HandleConceptualNulls(SensitiveLoggingEnabled, force, isCascadeDelete: true);
                            }
                        }
                    }
                }
            }
        }

        private static bool KeysEqual(InternalEntityEntry entry, IForeignKey fk, InternalEntityEntry dependent)
        {
            for (var i = 0; i < fk.Properties.Count; i++)
            {
                var principalProperty = fk.PrincipalKey.Properties[i];
                var dependentProperty = fk.Properties[i];

                if (!KeyValuesEqual(
                    principalProperty,
                    entry[principalProperty],
                    dependent[dependentProperty]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool KeyValuesEqual(IProperty property, object value, object currentValue)
            => (property.GetKeyValueComparer()
                ?? property.FindTypeMapping()?.KeyComparer)
               ?.Equals(currentValue, value)
               ?? Equals(currentValue, value);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (ChangedCount == 0)
            {
                return 0;
            }

            var entriesToSave = GetEntriesToSave(cascadeChanges: true);
            if (entriesToSave.Count == 0)
            {
                return 0;
            }

            try
            {
                var result = await SaveChangesAsync(entriesToSave, cancellationToken);

                if (acceptAllChangesOnSuccess)
                {
                    AcceptAllChanges((IReadOnlyList<IUpdateEntry>)entriesToSave);
                }

                return result;
            }
            catch
            {
                foreach (var entry in entriesToSave)
                {
                    ((InternalEntityEntry)entry).DiscardStoreGeneratedValues();
                }

                throw;
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual int SaveChanges(
            [NotNull] IList<IUpdateEntry> entriesToSave)
        {
            using (_concurrencyDetector.EnterCriticalSection())
            {
                return _database.SaveChanges(entriesToSave);
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual async Task<int> SaveChangesAsync(
            [NotNull] IList<IUpdateEntry> entriesToSave,
            CancellationToken cancellationToken = default)
        {
            using (_concurrencyDetector.EnterCriticalSection())
            {
                return await _database.SaveChangesAsync(entriesToSave, cancellationToken);
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void AcceptAllChanges()
            => AcceptAllChanges(this.ToListForState(added: true, modified: true, deleted: true));

        private static void AcceptAllChanges(IReadOnlyList<IUpdateEntry> changedEntries)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var entryIndex = 0; entryIndex < changedEntries.Count; entryIndex++)
            {
                ((InternalEntityEntry)changedEntries[entryIndex]).AcceptChanges();
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public event EventHandler<EntityTrackedEventArgs> Tracked;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void OnTracked(InternalEntityEntry internalEntityEntry, bool fromQuery)
        {
            var @event = Tracked;

            if (SensitiveLoggingEnabled)
            {
                _changeTrackingLogger.StartedTrackingSensitive(internalEntityEntry);
            }
            else
            {
                _changeTrackingLogger.StartedTracking(internalEntityEntry);
            }

            @event?.Invoke(Context.ChangeTracker, new EntityTrackedEventArgs(internalEntityEntry, fromQuery));
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public event EventHandler<EntityStateChangedEventArgs> StateChanged;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void OnStateChanged(InternalEntityEntry internalEntityEntry, EntityState oldState)
        {
            var @event = StateChanged;
            var newState = internalEntityEntry.EntityState;

            if (SensitiveLoggingEnabled)
            {
                _changeTrackingLogger.StateChangedSensitive(internalEntityEntry, oldState, newState);
            }
            else
            {
                _changeTrackingLogger.StateChanged(internalEntityEntry, oldState, newState);
            }

            @event?.Invoke(Context.ChangeTracker, new EntityStateChangedEventArgs(internalEntityEntry, oldState, newState));
        }
    }
}
