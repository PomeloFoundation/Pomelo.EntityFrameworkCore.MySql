// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    /// <summary>
    ///     A <see cref="MigrationOperation" /> for dropping a primary key.
    /// </summary>
    [DebuggerDisplay("ALTER TABLE {Table} DROP CONSTRAINT {Name}")]
    public class MySqlDropUniqueConstraintAndRecreateForeignKeysOperation : DropUniqueConstraintOperation
    {
        /// <summary>
        ///     Recreate all foreign keys or not.
        /// </summary>
        public virtual bool RecreateForeignKeys { get; set; }
    }
}
