// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    /// <summary>
    ///     <para>
    ///         Service dependencies parameter class for <see cref="StateManager" />
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    ///     <para>
    ///         Do not construct instances of this class directly from either provider or application code as the
    ///         constructor signature may change as new dependencies are added. Instead, use this type in
    ///         your constructor so that an instance will be created and injected automatically by the
    ///         dependency injection container. To create an instance with some dependent services replaced,
    ///         first resolve the object from the dependency injection container, then replace selected
    ///         services using the 'With...' methods. Do not call the constructor at any point in this process.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped"/>. This means that each
    ///         <see cref="DbContext"/> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    public sealed class StateManagerDependencies
    {
        /// <summary>
        ///     <para>
        ///         Creates the service dependencies parameter object for a <see cref="StateManager" />.
        ///     </para>
        ///     <para>
        ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///         any release. You should only use it directly in your code with extreme caution and knowing that
        ///         doing so can result in application failures when updating to a new Entity Framework Core release.
        ///     </para>
        ///     <para>
        ///         Do not call this constructor directly from either provider or application code as it may change
        ///         as new dependencies are added. Instead, use this type in your constructor so that an instance
        ///         will be created and injected automatically by the dependency injection container. To create
        ///         an instance with some dependent services replaced, first resolve the object from the dependency
        ///         injection container, then replace selected services using the 'With...' methods. Do not call
        ///         the constructor at any point in this process.
        ///     </para>
        ///     <para>
        ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///         any release. You should only use it directly in your code with extreme caution and knowing that
        ///         doing so can result in application failures when updating to a new Entity Framework Core release.
        ///     </para>
        /// </summary>
        [EntityFrameworkInternal]
        public StateManagerDependencies(
            [NotNull] IInternalEntityEntryFactory internalEntityEntryFactory,
            [NotNull] IInternalEntityEntrySubscriber internalEntityEntrySubscriber,
            [NotNull] IInternalEntityEntryNotifier internalEntityEntryNotifier,
            [NotNull] IValueGenerationManager valueGenerationManager,
            [NotNull] IModel model,
            [NotNull] IDatabase database,
            [NotNull] IConcurrencyDetector concurrencyDetector,
            [NotNull] ICurrentDbContext currentContext,
            [NotNull] IEntityFinderSource entityFinderSource,
            [NotNull] IDbSetSource setSource,
            [NotNull] IEntityMaterializerSource entityMaterializerSource,
            [NotNull] ILoggingOptions loggingOptions,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.ChangeTracking> changeTrackingLogger)
        {
            InternalEntityEntryFactory = internalEntityEntryFactory;
            InternalEntityEntrySubscriber = internalEntityEntrySubscriber;
            InternalEntityEntryNotifier = internalEntityEntryNotifier;
            ValueGenerationManager = valueGenerationManager;
            Model = model;
            Database = database;
            ConcurrencyDetector = concurrencyDetector;
            CurrentContext = currentContext;
            EntityFinderSource = entityFinderSource;
            SetSource = setSource;
            EntityMaterializerSource = entityMaterializerSource;
            LoggingOptions = loggingOptions;
            UpdateLogger = updateLogger;
            ChangeTrackingLogger = changeTrackingLogger;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IInternalEntityEntryFactory InternalEntityEntryFactory { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IInternalEntityEntrySubscriber InternalEntityEntrySubscriber { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IInternalEntityEntryNotifier InternalEntityEntryNotifier { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IValueGenerationManager ValueGenerationManager { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IModel Model { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IDatabase Database { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IConcurrencyDetector ConcurrencyDetector { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public ICurrentDbContext CurrentContext { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IDbSetSource SetSource { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IEntityFinderSource EntityFinderSource { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IEntityMaterializerSource EntityMaterializerSource { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public ILoggingOptions LoggingOptions { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public IDiagnosticsLogger<DbLoggerCategory.ChangeTracking> ChangeTrackingLogger { get; }

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="internalEntityEntryFactory"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IInternalEntityEntryFactory internalEntityEntryFactory)
            => new StateManagerDependencies(
                internalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="internalEntityEntrySubscriber"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IInternalEntityEntrySubscriber internalEntityEntrySubscriber)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                internalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="internalEntityEntryNotifier"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IInternalEntityEntryNotifier internalEntityEntryNotifier)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                internalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="valueGenerationManager"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] ValueGenerationManager valueGenerationManager)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                valueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="model"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IModel model)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="database"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IDatabase database)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="concurrencyDetector"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IConcurrencyDetector concurrencyDetector)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                concurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="currentContext"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] ICurrentDbContext currentContext)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                currentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="entityFinderSource"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IEntityFinderSource entityFinderSource)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                entityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="setSource"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IDbSetSource setSource)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                setSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="entityMaterializerSource"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IEntityMaterializerSource entityMaterializerSource)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                entityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="loggingOptions"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] ILoggingOptions loggingOptions)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                loggingOptions,
                UpdateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="updateLogger"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                updateLogger,
                ChangeTrackingLogger);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="changeTrackingLogger"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public StateManagerDependencies With([NotNull] IDiagnosticsLogger<DbLoggerCategory.ChangeTracking> changeTrackingLogger)
            => new StateManagerDependencies(
                InternalEntityEntryFactory,
                InternalEntityEntrySubscriber,
                InternalEntityEntryNotifier,
                ValueGenerationManager,
                Model,
                Database,
                ConcurrencyDetector,
                CurrentContext,
                EntityFinderSource,
                SetSource,
                EntityMaterializerSource,
                LoggingOptions,
                UpdateLogger,
                changeTrackingLogger);
    }
}
