// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

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
        ///     Configures the Default CharSet Behavior
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder CharSetBehavior(CharSetBehavior charSetBehavior)
            => WithOption(e => e.WithCharSetBehavior(charSetBehavior));

        /// <summary>
        ///     Configures the ANSI CharSet
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder AnsiCharSet(CharSet charSet)
            => WithOption(e => e.WithAnsiCharSetInfo(new CharSetInfo(charSet)));

        /// <summary>
        ///     Configures the Unicode CharSet
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder UnicodeCharSet(CharSet charSet)
            => WithOption(e => e.WithUnicodeCharSetInfo(new CharSetInfo(charSet)));

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
        ///     Configures string escaping in SQL query generation to ignore backslashes.
        ///     This applies to both constant and parameter values (i. e. user input, potentially).
        ///     Use this option if SQL mode NO_BACKSLASH_ESCAPES is guaranteed to be active.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder DisableBackslashEscaping() =>
            WithOption(e => e.DisableBackslashEscaping());

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
