// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure
{
    public enum CharSet
    {
        Utf8mb4, // the default, as this is what is recommended in README.md
        Latin1,
        Ucs2,
        Utf8mb3
    }
}
