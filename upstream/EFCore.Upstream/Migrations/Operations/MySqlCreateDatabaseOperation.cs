// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    /// <summary>
    ///     A MySql-specific <see cref="MigrationOperation" /> to create a database.
    /// </summary>
    public class MySqlCreateDatabaseOperation : MigrationOperation
    {
        /// <summary>
        ///     The name of the database.
        /// </summary>
        public virtual string Name { get; [param: NotNull] set; }

        /// <summary>
        ///     The filename to use for the database, or <c>null</c> to let MySql choose.
        /// </summary>
        public virtual string FileName { get; [param: CanBeNull] set; }
    }
}
