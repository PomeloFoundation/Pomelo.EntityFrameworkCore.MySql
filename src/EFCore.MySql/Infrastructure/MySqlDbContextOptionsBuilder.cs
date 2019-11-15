// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class MySqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>
    {
        public MySqlDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }

        /// <summary>
        ///     Configures the target server version and type.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder ServerVersion(Version version, ServerType type)
            => WithOption(e => e.WithServerVersion(new ServerVersion(version, type)));

        /// <summary>
        ///     Configures the target server version and type.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder ServerVersion(string serverVersion)
            => WithOption(e => e.WithServerVersion(new ServerVersion(serverVersion)));

        /// <summary>
        ///     Configures the target <see cref="ServerVersion"/>.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder ServerVersion(ServerVersion serverVersion)
            => WithOption(e => e.WithServerVersion(serverVersion));

        /// <summary>
        ///     Configures the Default CharSet Behavior
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder CharSetBehavior(CharSetBehavior charSetBehavior)
            => WithOption(e => e.WithCharSetBehavior(charSetBehavior));

        /// <summary>
        ///     Configures the ANSI CharSet
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder CharSet(CharSet charSet)
            => WithOption(e => e.WithCharSet(charSet));

        /// <summary>
        ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder EnableRetryOnFailure()
            => ExecutionStrategy(c => new MySqlRetryingExecutionStrategy(c));

        /// <summary>
        ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder EnableRetryOnFailure(int maxRetryCount)
            => ExecutionStrategy(c => new MySqlRetryingExecutionStrategy(c, maxRetryCount));

        /// <summary>
        ///     Configures string escaping in SQL query generation to ignore backslashes, and assumes
        ///     that `sql_mode` has been set to `NO_BACKSLASH_ESCAPES`.
        ///     This applies to both constant and parameter values (i. e. user input, potentially).
        /// </summary>
        /// <param name="setSqlModeOnOpen">When `true`, enables the <see cref="SetSqlModeOnOpen" /> option,
        /// which sets `sql_mode` to `NO_BACKSLASH_ESCAPES` automatically, when a connection has been
        /// opened. This is the default.
        /// When `false`, does not change the <see cref="SetSqlModeOnOpen" /> option, when calling this method.</param>
        public virtual MySqlDbContextOptionsBuilder DisableBackslashEscaping(bool setSqlModeOnOpen = true)
        {
            var builder = WithOption(e => e.WithDisabledBackslashEscaping());

            if (setSqlModeOnOpen)
            {
                builder = builder.WithOption(e => e.WithSettingSqlModeOnOpen());
            }

            return builder;
        }

        /// <summary>
        ///     When `true`, implicitly executes a `SET SESSION sql_mode` statement after opening
        ///     a connection to the database server, adding the modes enabled by other options.
        ///     When `false`, the `sql_mode` is not being set by the provider and has to be manually
        ///     handled by the caller, to synchronize it with other options that have been set.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder SetSqlModeOnOpen()
            => WithOption(e => e.WithSettingSqlModeOnOpen());

        /// <summary>
        ///     Skip replacing `\r` and `\n` with `CHAR()` calls in strings inside queries.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder DisableLineBreakToCharSubstition()
            => WithOption(e => e.WithDisabledLineBreakToCharSubstition());

        /// <summary>
        ///     Configures default mappings between specific CLR and MySQL types.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder DefaultDataTypeMappings(Func<MySqlDefaultDataTypeMappings, MySqlDefaultDataTypeMappings> defaultDataTypeMappings)
            => WithOption(e => e.WithDefaultDataTypeMappings(defaultDataTypeMappings(new MySqlDefaultDataTypeMappings())));

        /// <summary>
        ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        /// <param name="maxRetryDelay"> The maximum delay between retries. </param>
        /// <param name="errorNumbersToAdd"> Additional error codes that should be considered transient. </param>
        public virtual MySqlDbContextOptionsBuilder EnableRetryOnFailure(
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            [NotNull] ICollection<int> errorNumbersToAdd)
            => ExecutionStrategy(c => new MySqlRetryingExecutionStrategy(c, maxRetryCount, maxRetryDelay, errorNumbersToAdd));
    }
}
