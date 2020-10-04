// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Pomelo.EntityFrameworkCore.MySql.Utilities
{
    internal static class Statics
    {
        internal static readonly bool[][] TrueArrays =
        {
            Array.Empty<bool>(),
            new[] { true },
            new[] { true, true },
            new[] { true, true, true },
            new[] { true, true, true, true },
            new[] { true, true, true, true, true },
            new[] { true, true, true, true, true, true }
        };

        internal static readonly bool[][] FalseArrays =
        {
            Array.Empty<bool>(),
            new[] { false },
            new[] { false, false }
        };
    }
}
