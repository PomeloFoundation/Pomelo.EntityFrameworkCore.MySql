// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    /// <summary>
    ///     <para>
    ///         An implementation of <see cref="IModelSource" /> that produces a model based on
    ///         the <see cref="DbSet{TEntity}" /> properties exposed on the context. The model is cached to avoid
    ///         recreating it every time it is requested.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton"/>. This means a single instance
    ///         is used by many <see cref="DbContext"/> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped"/>.
    ///     </para>
    /// </summary>
    public class ModelSource : IModelSource
    {
        private readonly ConcurrentDictionary<object, Lazy<IModel>> _models
            = new ConcurrentDictionary<object, Lazy<IModel>>();

        /// <summary>
        ///     Creates a new <see cref="ModelSource"/> instance.
        /// </summary>
        /// <param name="dependencies"> The dependencies to use. </param>
        public ModelSource([NotNull] ModelSourceDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        ///     Dependencies used to create a <see cref="ModelSource" />
        /// </summary>
        protected virtual ModelSourceDependencies Dependencies { get; }

        /// <summary>
        ///     Returns the model from the cache, or creates a model if it is not present in the cache.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <returns> The model to be used. </returns>
        public virtual IModel GetModel(
            DbContext context,
            IConventionSetBuilder conventionSetBuilder)
            => _models.GetOrAdd(
                Dependencies.ModelCacheKeyFactory.Create(context),
                // Using a Lazy here so that OnModelCreating, etc. really only gets called once, since it may not be thread safe.
                k => new Lazy<IModel>(
                    () => CreateModel(context, conventionSetBuilder),
                    LazyThreadSafetyMode.ExecutionAndPublication)).Value;

        /// <summary>
        ///     Creates the model. This method is called when the model was not found in the cache.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <returns> The model to be used. </returns>
        protected virtual IModel CreateModel(
            [NotNull] DbContext context,
            [NotNull] IConventionSetBuilder conventionSetBuilder)
        {
            Check.NotNull(context, nameof(context));

            var modelBuilder = new ModelBuilder(conventionSetBuilder.CreateConventionSet());

            Dependencies.ModelCustomizer.Customize(modelBuilder, context);

            return modelBuilder.FinalizeModel();
        }
    }
}
