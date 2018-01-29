// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlIndexAnnotations : RelationalIndexAnnotations, IMySqlIndexAnnotations
    {
        public MySqlIndexAnnotations([NotNull] IIndex index)
            : base(index)
        {
        }

        protected MySqlIndexAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        public virtual bool? IsFullText
        {
            get => (bool?)Annotations.Metadata[MySqlAnnotationNames.FullTextIndex];
            set => SetIsFullText(value);
        }

        protected virtual bool SetIsFullText(bool? value) => Annotations.SetAnnotation(
            MySqlAnnotationNames.FullTextIndex,
            value);

        public virtual bool? IsSpatial
        {
            get => (bool?)Annotations.Metadata[MySqlAnnotationNames.SpatialIndex];
            set => SetIsSpatial(value);
        }

        protected virtual bool SetIsSpatial(bool? value) => Annotations.SetAnnotation(
            MySqlAnnotationNames.SpatialIndex,
            value);
    }
}
