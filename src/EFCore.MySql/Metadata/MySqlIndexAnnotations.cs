// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            get { return (bool?)Annotations.GetAnnotation(MySqlAnnotationNames.Clustered); }
            [param: CanBeNull] set { SetIsClustered(value); }
        }

        protected virtual bool SetIsClustered(bool? value) => Annotations.SetAnnotation(
            MySqlAnnotationNames.Clustered,
            value);
    }
}
