// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class ByteArrayComparer : ValueComparer<byte[]>
    {
        public ByteArrayComparer()
            : base(
                (v1, v2) => StructuralComparisons.StructuralEqualityComparer.Equals(v1, v2),
                v => StructuralComparisons.StructuralEqualityComparer.GetHashCode(v),
                v => v == null ? null : v.ToArray())
        {
        }
    }
}
