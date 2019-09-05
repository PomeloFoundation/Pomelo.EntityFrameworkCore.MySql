﻿using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Extensions
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="MigrationBuilder" />.
    /// </summary>
    public static class MySqlMigrationBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns true if the database provider currently in use is the SQL Server provider.
        ///     </para>
        /// </summary>
        /// <param name="migrationBuilder"> The migrationBuilder from the parameters on <see cref="Migration.Up(MigrationBuilder)" /> or <see cref="Migration.Down(MigrationBuilder)" />. </param>
        /// <returns> True if SQL Server is being used; false otherwise. </returns>
        public static bool IsMySql([NotNull] this MigrationBuilder migrationBuilder)
            => string.Equals(migrationBuilder.ActiveProvider,
                typeof(MySqlOptionsExtension).GetTypeInfo().Assembly.GetName().Name,
                StringComparison.Ordinal);
    }
}
