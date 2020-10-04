// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata
{
    // ReSharper disable once SA1602
    public enum MySqlValueGenerationStrategy
    {
        /// <summary>
        /// TODO
        /// </summary>
        None,

        /// <summary>
        /// TODO
        /// </summary>
        IdentityColumn,

        /// <summary>
        /// TODO
        /// </summary>
        ComputedColumn // TODO: Remove this and only use .HasComputedColumnSql() instead in EF Core 5
    }
}
