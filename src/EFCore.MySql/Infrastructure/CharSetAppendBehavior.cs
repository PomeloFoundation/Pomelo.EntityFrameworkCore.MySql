// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure
{
    // CHECK: Can we remove this for 5.0?
    [Flags]
    public enum CharSetBehavior
    {
        /// <summary>
        /// TODO
        /// </summary>
        NeverAppend = 0,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToAnsiIndexAndKeyColumns       = 1 << 0,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToUnicodeIndexAndKeyColumns    = 1 << 1,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToAnsiNonIndexAndKeyColumns    = 1 << 2,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToUnicodeNonIndexAndKeyColumns = 1 << 3,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToAllKeyAndIndexColumns = AppendToAnsiIndexAndKeyColumns | AppendToUnicodeIndexAndKeyColumns,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToAllAnsiColumns = AppendToAnsiIndexAndKeyColumns | AppendToAnsiNonIndexAndKeyColumns,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToAllUnicodeColumns = AppendToUnicodeIndexAndKeyColumns | AppendToUnicodeNonIndexAndKeyColumns,

        /// <summary>
        /// TODO
        /// </summary>
        AppendToAllColumns = AppendToAllAnsiColumns | AppendToAllUnicodeColumns
    }
}
