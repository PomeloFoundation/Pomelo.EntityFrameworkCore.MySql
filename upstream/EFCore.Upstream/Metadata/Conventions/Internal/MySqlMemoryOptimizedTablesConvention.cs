// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlMemoryOptimizedTablesConvention : IEntityTypeAnnotationChangedConvention, IKeyAddedConvention, IIndexAddedConvention
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Annotation Apply(
            InternalEntityTypeBuilder entityTypeBuilder, string name, Annotation annotation, Annotation oldAnnotation)
        {
            if (name == MySqlAnnotationNames.MemoryOptimized)
            {
                var memoryOptimized = annotation?.Value as bool? == true;
                foreach (var key in entityTypeBuilder.Metadata.GetDeclaredKeys())
                {
                    key.Builder.MySql(ConfigurationSource.Convention).IsClustered(memoryOptimized ? false : (bool?)null);
                }

                foreach (var index in entityTypeBuilder.Metadata.GetDerivedIndexesInclusive())
                {
                    index.Builder.MySql(ConfigurationSource.Convention).IsClustered(memoryOptimized ? false : (bool?)null);
                }
            }

            return annotation;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalKeyBuilder Apply(InternalKeyBuilder keyBuilder)
        {
            if (keyBuilder.Metadata.DeclaringEntityType.MySql().IsMemoryOptimized)
            {
                keyBuilder.MySql(ConfigurationSource.Convention).IsClustered(false);
            }

            return keyBuilder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalIndexBuilder Apply(InternalIndexBuilder indexBuilder)
        {
            if (indexBuilder.Metadata.DeclaringEntityType.GetAllBaseTypesInclusive().Any(et => et.MySql().IsMemoryOptimized))
            {
                indexBuilder.MySql(ConfigurationSource.Convention).IsClustered(false);
            }

            return indexBuilder;
        }
    }
}
