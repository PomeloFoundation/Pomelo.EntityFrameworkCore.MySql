// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     Properties for relational-specific annotations accessed through
    ///     <see cref="MySqlMetadataExtensions.MySql(IMutableEntityType)" />.
    /// </summary>
    public class MySqlEntityTypeAnnotations : RelationalEntityTypeAnnotations, IMySqlEntityTypeAnnotations
    {
        /// <summary>
        ///     Constructs an instance for annotations of the given <see cref="IEntityType" />.
        /// </summary>
        /// <param name="entityType"> The <see cref="IEntityType" /> to use. </param>
        public MySqlEntityTypeAnnotations([NotNull] IEntityType entityType)
            : base(entityType)
        {
        }

        /// <summary>
        ///     Constructs an instance for annotations of the <see cref="IEntityType" />
        ///     represented by the given annotation helper.
        /// </summary>
        /// <param name="annotations">
        ///     The <see cref="RelationalAnnotations" /> helper representing the <see cref="IEntityType" /> to annotate.
        /// </param>
        public MySqlEntityTypeAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        /// <summary>
        ///     Indicates whether or not the type is mapped to a memory-optimized table.
        /// </summary>
        public virtual bool IsMemoryOptimized
        {
            get => Annotations.Metadata[MySqlAnnotationNames.MemoryOptimized] as bool? ?? false;
            set => SetIsMemoryOptimized(value);
        }

        /// <summary>
        ///     Attempts to set memory optimization.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <returns> <c>True</c> if the annotation was set; <c>false</c> otherwise. </returns>
        protected virtual bool SetIsMemoryOptimized(bool value)
            => Annotations.SetAnnotation(MySqlAnnotationNames.MemoryOptimized, value);
    }
}
