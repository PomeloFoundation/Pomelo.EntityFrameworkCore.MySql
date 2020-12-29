// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Migrations
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="MigrationBuilder" />.
    /// </summary>
    public static class MySqlMigrationBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns true if the database provider currently in use is the MySQL provider.
        ///     </para>
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <returns> True if MySQL is being used; false otherwise. </returns>
        public static bool IsMySql([NotNull] this MigrationBuilder migrationBuilder)
            => string.Equals(migrationBuilder.ActiveProvider,
                typeof(MySqlOptionsExtension).GetTypeInfo().Assembly.GetName().Name,
                StringComparison.Ordinal);

        /// <summary>
        ///     Builds an <see cref="MySqlDropPrimaryKeyAndRecreateForeignKeysOperation" /> to drop an existing primary key and optionally
        ///     recreate all foreign keys of the table.
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <param name="name"> The name of the primary key constraint to drop. </param>
        /// <param name="table"> The table that contains the key. </param>
        /// <param name="schema"> The schema that contains the table, or <see langword="null" /> to use the default schema. </param>
        /// <param name="recreateForeignKeys"> The sole reasion to use this extension method. Set this parameter to `true`, to force all
        /// foreign keys of the table be be dropped before the primary key is dropped, and created again afterwards.</param>
        /// <returns> A builder to allow annotations to be added to the operation. </returns>
        public static OperationBuilder<MySqlDropPrimaryKeyAndRecreateForeignKeysOperation> DropPrimaryKey(
            [NotNull] this MigrationBuilder migrationBuilder,
            [NotNull] string name,
            [NotNull] string table,
            [CanBeNull] string schema = null,
            bool recreateForeignKeys = false)
        {
            Check.NotNull(migrationBuilder, nameof(migrationBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new MySqlDropPrimaryKeyAndRecreateForeignKeysOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                RecreateForeignKeys = recreateForeignKeys,
            };
            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<MySqlDropPrimaryKeyAndRecreateForeignKeysOperation>(operation);
        }

        /// <summary>
        ///     Builds an <see cref="MySqlDropPrimaryKeyAndRecreateForeignKeysOperation" /> to drop an existing unique constraint and optionally
        ///     recreate all foreign keys of the table.
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <param name="name"> The name of the constraint to drop. </param>
        /// <param name="table"> The table that contains the constraint. </param>
        /// <param name="schema"> The schema that contains the table, or <see langword="null" /> to use the default schema. </param>
        /// <param name="recreateForeignKeys"> The sole reasion to use this extension method. Set this parameter to `true`, to force all
        /// foreign keys of the table be be dropped before the primary key is dropped, and created again afterwards.</param>
        /// <returns> A builder to allow annotations to be added to the operation. </returns>
        public static OperationBuilder<MySqlDropUniqueConstraintAndRecreateForeignKeysOperation> DropUniqueConstraint(
            [NotNull] this MigrationBuilder migrationBuilder,
            [NotNull] string name,
            [NotNull] string table,
            [CanBeNull] string schema = null,
            bool recreateForeignKeys = false)
        {
            Check.NotNull(migrationBuilder, nameof(migrationBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new MySqlDropUniqueConstraintAndRecreateForeignKeysOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                RecreateForeignKeys = recreateForeignKeys,
            };
            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<MySqlDropUniqueConstraintAndRecreateForeignKeysOperation>(operation);
        }
    }
}
