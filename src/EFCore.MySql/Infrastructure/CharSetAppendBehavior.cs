// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure
{
    [Flags]
    public enum CharSetBehavior
    {
        NeverAppend = 0,
        AppendToAnsiIndexAndKeyColumns       = 1 << 0,
        AppendToUnicodeIndexAndKeyColumns    = 1 << 1,
        AppendToAnsiNonIndexAndKeyColumns    = 1 << 2,
        AppendToUnicodeNonIndexAndKeyColumns = 1 << 3,
        AppendToAllKeyAndIndexColumns = AppendToAnsiIndexAndKeyColumns | AppendToUnicodeIndexAndKeyColumns,
        AppendToAllAnsiColumns = AppendToAnsiIndexAndKeyColumns | AppendToAnsiNonIndexAndKeyColumns,
        AppendToAllUnicodeColumns = AppendToUnicodeIndexAndKeyColumns | AppendToUnicodeNonIndexAndKeyColumns,
        AppendToAllColumns = AppendToAllAnsiColumns | AppendToAllUnicodeColumns
    }
}
