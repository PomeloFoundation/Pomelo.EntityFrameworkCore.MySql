// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Update.Internal
{
    // TODO: Revamp
    public class MySqlUpdateSqlGenerator : UpdateAndSelectSqlGenerator, IMySqlUpdateSqlGenerator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlUpdateSqlGenerator(
            [NotNull] UpdateSqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition,
            out bool requiresTransaction)
        {
            var table = StoreObjectIdentifier.Table(modificationCommands[0].TableName, modificationCommands[0].Schema);

            if (modificationCommands.Count == 1
                && modificationCommands[0].ColumnModifications.All(o =>
                    !o.IsKey
                    || !o.IsRead
                    || o.Property?.GetValueGenerationStrategy(table) == MySqlValueGenerationStrategy.IdentityColumn))
            {
                return AppendInsertOperation(commandStringBuilder, modificationCommands[0], commandPosition, out requiresTransaction);
            }

            var readOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsRead).ToList();
            var writeOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsWrite).ToList();

            var writableOperations = modificationCommands[0].ColumnModifications
                .Where(o => o.Property?.GetValueGenerationStrategy(table) != MySqlValueGenerationStrategy.IdentityColumn
                            && o.Property?.GetComputedColumnSql() is null)
                .ToList();

            var defaultValuesOnly = writeOperations.Count == 0;
            if (defaultValuesOnly)
            {
                if (writableOperations.Count == 0
                    || readOperations.Count == 0)
                {
                    requiresTransaction = modificationCommands.Count > 1;
                    foreach (var modification in modificationCommands)
                    {
                        AppendInsertOperation(commandStringBuilder, modification, commandPosition, out var localRequiresTransaction);
                        requiresTransaction = requiresTransaction || localRequiresTransaction;
                    }

                    return readOperations.Count == 0
                        ? ResultSetMapping.NoResults
                        : ResultSetMapping.LastInResultSet;
                }
            }

            if (readOperations.Count == 0)
            {
                return AppendBulkInsertWithoutServerValues(commandStringBuilder, modificationCommands, writeOperations, out requiresTransaction);
            }

            requiresTransaction = modificationCommands.Count > 1;
            foreach (var modification in modificationCommands)
            {
                AppendInsertOperation(commandStringBuilder, modification, commandPosition, out var localRequiresTransaction);
                requiresTransaction = requiresTransaction || localRequiresTransaction;
            }

            return ResultSetMapping.LastInResultSet;
        }

        private ResultSetMapping AppendBulkInsertWithoutServerValues(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            List<IColumnModification> writeOperations,
            out bool requiresTransaction)
        {
            Debug.Assert(writeOperations.Count > 0);

            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, name, schema, writeOperations);
            for (var i = 1; i < modificationCommands.Count; i++)
            {
                commandStringBuilder.Append(",").AppendLine();
                AppendValues(commandStringBuilder, name, schema, modificationCommands[i].ColumnModifications.Where(o => o.IsWrite).ToList());
            }
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

            // A single INSERT command should run atomically, regardless of how many value lists it contains.
            requiresTransaction = false;

            return ResultSetMapping.NoResults;
        }

        protected override void AppendInsertCommandHeader(
            StringBuilder commandStringBuilder,
            string name,
            string schema,
            IReadOnlyList<IColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operations, nameof(operations));

            base.AppendInsertCommandHeader(commandStringBuilder, name, schema, operations);

            if (operations.Count <= 0)
            {
                // An empty column and value list signales MySQL that only default values should be used.
                // If not all columns have default values defined, an error occurs if STRICT_ALL_TABLES has been set.
                commandStringBuilder.Append(" ()");
            }
        }

        protected override void AppendValuesHeader(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder.AppendLine();
            commandStringBuilder.Append("VALUES ");
        }

        protected override void AppendValues(
            StringBuilder commandStringBuilder,
            string name,
            string schema,
            IReadOnlyList<IColumnModification> operations)
        {
            base.AppendValues(commandStringBuilder, name, schema, operations);

            if (operations.Count <= 0)
            {
                commandStringBuilder.Append("()");
            }
        }

        protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name, string schema, int commandPosition)
        {
            commandStringBuilder
                .Append("SELECT ROW_COUNT()")
                .Append(SqlGenerationHelper.StatementTerminator).AppendLine()
                .AppendLine();

            return ResultSetMapping.LastInResultSet;
        }

        protected override void AppendWhereAffectedClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            // If a compound key consists of an auto_increment column and a database generated column (e.g. a DEFAULT
            // value), then we only want to filter by `LAST_INSERT_ID()`, because we can't know what the other generated
            // values are.
            // Therefore, we filter out the key columns that are marked as `read`, but are not an auto_increment column,
            // so that `AppendIdentityWhereCondition()` can safely called for the remaining auto_increment column.
            // Because we currently use `MySqlValueGenerationStrategy.IdentityColumn` for auto_increment columns as well
            // as CURRENT_TIMESTAMP columns, we need to use `MySqlPropertyExtensions.IsCompatibleAutoIncrementColumn()`
            // to ensure, that the column is actually an auto_increment column.
            // See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1300
            var nonDefaultOperations = operations
                .Where(
                    o => !o.IsKey ||
                         !o.IsRead ||
                         o.Property == null ||
                         !o.Property.ValueGenerated.HasFlag(ValueGenerated.OnAdd) ||
                         MySqlPropertyExtensions.IsCompatibleAutoIncrementColumn(o.Property))
                .ToList()
                .AsReadOnly();

            base.AppendWhereAffectedClause(commandStringBuilder, nonDefaultOperations);
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification)
        {
            SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, columnModification.ColumnName);
            commandStringBuilder.Append(" = ")
                .Append("LAST_INSERT_ID()");
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
            => commandStringBuilder
                .Append("ROW_COUNT() = ")
                .Append(expectedRowsAffected.ToString(CultureInfo.InvariantCulture));
    }
}
