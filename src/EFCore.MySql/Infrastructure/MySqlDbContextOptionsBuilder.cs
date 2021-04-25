// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
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

        [Obsolete("Call the Fluent API extension method 'HasCharSet()' on the builder object of your model/entities/properties instead. To get the exact behavior as with `CharSet()`, call 'modelBuilder.HasCharSet(charSet, DelegationMode.ApplyToColumns)'.", true)]
        public virtual MySqlDbContextOptionsBuilder CharSet(CharSet charSet) // TODO: Remove for EF Core 6.
            => throw new NotImplementedException("Call the Fluent API extension method 'HasCharSet()' on the builder object of your model/entities/properties instead. To get the exact behavior as with `CharSet()`, call 'modelBuilder.HasCharSet(charSet, DelegationMode.ApplyToColumns)'.");

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
        ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        /// <param name="maxRetryDelay"> The maximum delay between retries. </param>
        /// <param name="errorNumbersToAdd"> Additional error codes that should be considered transient. </param>
        public virtual MySqlDbContextOptionsBuilder EnableRetryOnFailure(
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            [CanBeNull] ICollection<int> errorNumbersToAdd)
            => ExecutionStrategy(c => new MySqlRetryingExecutionStrategy(c, maxRetryCount, maxRetryDelay, errorNumbersToAdd));

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
        ///     Configures the behavior for cases when a schema has been set for an entity. Because
        ///     MySQL does not support the EF Core concept of schemas, the default is to throw an
        ///     exception.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder SchemaBehavior(MySqlSchemaBehavior behavior, MySqlSchemaNameTranslator translator = null)
            => WithOption(e => e.WithSchemaBehavior(behavior, translator));

        /// <summary>
        ///     Configures the context to optimize `System.Boolean` mapped columns for index usage,
        ///     by translating `e.BoolColumn` to `BoolColumn = TRUE` and `!e.BoolColumn` to `BoolColumn = FALSE`.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder EnableIndexOptimizedBooleanColumns(bool enable = true)
            => WithOption(e => e.WithIndexOptimizedBooleanColumns(enable));

        /// <summary>
        ///     Configures the context to automatically limit the length of `System.String` mapped columns, that have not explicitly mapped
        ///     to a store type (e.g. `varchar(1024)`), to ensure that at least two indexed columns will be allowed on a given table (this
        ///     is the default if you don't configure this option).
        ///     If you intend to use `HasPrefixLength()` for those kind of columns, set this option to `false`.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder LimitKeyedOrIndexedStringColumnLength(bool enable = true)
            => WithOption(e => e.WithKeyedOrIndexedStringColumnLengthLimit(enable));

        /// <summary>
        ///     Configures the context to translate string related methods, containing a parameter of type <see cref="StringComparison"/>,
        ///     to their SQL equivalent, even though MySQL might not be able to use indexes when executing the query, resulting in decreased
        ///     performance. Whether MySQL is able to use indexes for the query, depends on the <see cref="StringComparison"/> option, the
        ///     underlying collation and the scenario.
        ///     It is also possible to just use `EF.Functions.Collate()`, possibly in addition to `string.ToUpper()` if needed, to achieve
        ///     the same result but with full control over the SQL generation.
        /// </summary>
        public virtual MySqlDbContextOptionsBuilder EnableStringComparisonTranslations(bool enable = true)
            => WithOption(e => e.WithStringComparisonTranslations(enable));
    }
}
