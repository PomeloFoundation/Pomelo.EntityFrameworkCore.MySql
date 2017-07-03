// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        public virtual bool? IsClustered
        {
            get { return (bool?)Annotations.Metadata[MySqlAnnotationNames.Clustered]; }
            [param: CanBeNull] set { SetIsClustered(value); }
        }

        protected virtual bool SetIsClustered(bool? value) => Annotations.SetAnnotation(
            MySqlAnnotationNames.Clustered,
            value);
    }
}
