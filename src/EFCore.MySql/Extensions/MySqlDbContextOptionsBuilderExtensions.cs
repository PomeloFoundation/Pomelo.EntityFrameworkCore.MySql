// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using MySqlConnector;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides extension methods on <see cref="DbContextOptionsBuilder"/> and <see cref="DbContextOptionsBuilder{T}"/>
    /// to configure a <see cref="DbContext"/> to use with MySQL/MariaDB and Pomelo.EntityFrameworkCore.MySql.
    /// </summary>
    public static class MySqlDbContextOptionsBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a MySQL compatible database, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseMySql(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] ServerVersion serverVersion,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder)
                .WithServerVersion(serverVersion);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseMySql(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [NotNull] ServerVersion serverVersion,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var csb = new MySqlConnectionStringBuilder(connectionString)
            {
                AllowUserVariables = true,
                UseAffectedRows = false
            };

            connectionString = csb.ConnectionString;

            var extension = (MySqlOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithServerVersion(serverVersion)
                .WithConnectionString(connectionString);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseMySql(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [NotNull] ServerVersion serverVersion,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var csb = new MySqlConnectionStringBuilder(connection.ConnectionString);

            if (!csb.AllowUserVariables ||
                csb.UseAffectedRows)
            {
                try
                {
                    csb.AllowUserVariables = true;
                    csb.UseAffectedRows = false;

                    connection.ConnectionString = csb.ConnectionString;
                }
                catch (MySqlException e)
                {
                    throw new InvalidOperationException(
                        @"The connection string used with Pomelo.EntityFrameworkCore.MySql must contain ""AllowUserVariables=true;UseAffectedRows=false"".",
                        e);
                }
            }

            var extension = (MySqlOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithServerVersion(serverVersion)
                .WithConnection(connection);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a MySQL compatible database, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseMySql<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] ServerVersion serverVersion,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseMySql(
                (DbContextOptionsBuilder)optionsBuilder, serverVersion, mySqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseMySql<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string connectionString,
            [NotNull] ServerVersion serverVersion,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseMySql(
                (DbContextOptionsBuilder)optionsBuilder, connectionString, serverVersion, mySqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseMySql<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] DbConnection connection,
            [NotNull] ServerVersion serverVersion,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseMySql(
                (DbContextOptionsBuilder)optionsBuilder, connection, serverVersion, mySqlOptionsAction);

        private static MySqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<MySqlOptionsExtension>()
               ?? new MySqlOptionsExtension();

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
