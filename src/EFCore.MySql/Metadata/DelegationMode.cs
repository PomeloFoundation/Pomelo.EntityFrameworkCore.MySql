// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// Provides precice control over recursive character set or collation delegation/inheritance aspects.
    /// </summary>
    [Flags]
    public enum DelegationMode
    {
        /// <summary>
        /// The current default is <see cref="ApplyToAll"/>.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Applys the character set or collation to the database.
        /// </summary>
        ApplyToDatabase = 1 << 0,

        /// <summary>
        /// Applys the character set or collation to tables.
        /// </summary>
        ApplyToTables = 2 << 0,

        /// <summary>
        /// Applys the character set or collation to columns.
        /// </summary>
        ApplyToColumns = 3 << 0,

        /// <summary>
        /// Applys the character set or collation to all objects.
        /// </summary>
        ApplyToAll = ApplyToDatabase | ApplyToTables | ApplyToColumns
    }
}
