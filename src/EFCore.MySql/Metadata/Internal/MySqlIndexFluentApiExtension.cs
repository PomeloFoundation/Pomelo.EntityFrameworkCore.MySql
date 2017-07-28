// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlIndexFluentApiExtension
    {
        public static IndexBuilder IsFullText([NotNull] this IndexBuilder indexBuilder, bool isFullText = true)
        {
            if (isFullText)
            {
                indexBuilder.Metadata.AddAnnotation(MySqlAnnotationNames.FullTextIndex, "FULLTEXT");
            }
            else
            {
                indexBuilder.Metadata.RemoveAnnotation(MySqlAnnotationNames.FullTextIndex);
            }
            return indexBuilder;
        }

        public static IndexBuilder IsSpatial([NotNull] this IndexBuilder indexBuilder, bool isSpatial = true)
        {
            if (isSpatial)
            {
                indexBuilder.Metadata.AddAnnotation(MySqlAnnotationNames.SpatialIndex, "SPATIAL");
            }
            else
            {
                indexBuilder.Metadata.RemoveAnnotation(MySqlAnnotationNames.SpatialIndex);
            }
            return indexBuilder;
        }
    }
}
