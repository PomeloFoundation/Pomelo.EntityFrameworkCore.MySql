// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that finds a base entity type that's already part of the model based on the associated
    ///     CLR type hierarchy.
    /// </summary>
    public class BaseTypeDiscoveryConvention : InheritanceDiscoveryConventionBase, IEntityTypeAddedConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="BaseTypeDiscoveryConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public BaseTypeDiscoveryConvention([NotNull] ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     Called after an entity type is added to the model.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type. </param>
        /// <param name="context"> Additional information associated with convention execution. </param>
        public virtual void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
        {
            var entityType = entityTypeBuilder.Metadata;
            var clrType = entityType.ClrType;
            if (clrType == null
                || entityType.HasDefiningNavigation()
                || entityType.FindDeclaredOwnership() != null
                || entityType.Model.FindIsOwnedConfigurationSource(clrType) != null)
            {
                return;
            }

            var baseEntityType = FindClosestBaseType(entityType);
            if (baseEntityType?.HasDefiningNavigation() == false)
            {
                entityTypeBuilder = entityTypeBuilder.HasBaseType(baseEntityType);
                if (entityTypeBuilder != null)
                {
                    context.StopProcessingIfChanged(entityTypeBuilder);
                }
            }
        }
    }
}
