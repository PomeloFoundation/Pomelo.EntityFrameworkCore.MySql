// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    /// <summary>
    ///     A MySql Server-specific <see cref="MigrationOperation" /> to create a database.
    /// </summary>
    public class MySqlCreateDatabaseOperation : DatabaseOperation
    {
        /// <summary>
        ///     The name of the database.
        /// </summary>
        public virtual string Name { get; [param: NotNull] set; }

        /// <summary>
        ///     The default character set of the database.
        /// </summary>
        public virtual string CharSet { get; [param: CanBeNull] set; }
    }
}
