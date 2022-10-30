// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Update.Internal
{
    public class MySqlUpdateSqlGenerator : UpdateAndSelectSqlGenerator, IMySqlUpdateSqlGenerator
    {
        [NotNull] private readonly IMySqlOptions _options;

        public MySqlUpdateSqlGenerator(
            [NotNull] UpdateSqlGeneratorDependencies dependencies,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        public override ResultSetMapping AppendInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
            => _options.ServerVersion.Supports.Returning ||
               command.ColumnModifications.All(o => !o.IsRead)
                ? AppendInsertReturningOperation(commandStringBuilder, command, commandPosition, out requiresTransaction)
                : base.AppendInsertOperation(commandStringBuilder, command, commandPosition, out requiresTransaction);

        public virtual ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition,
            out bool requiresTransaction)
        {
            if (modificationCommands.Count == 1)
            {
                return AppendInsertOperation(commandStringBuilder, modificationCommands[0], commandPosition, out requiresTransaction);
            }

            var readOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsRead).ToList();
            var writeOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsWrite).ToList();

            if (readOperations.Count == 0)
            {
                return AppendInsertMultipleRowsInSingleStatementOperation(commandStringBuilder, modificationCommands, writeOperations, out requiresTransaction);
            }

            requiresTransaction = modificationCommands.Count > 1;
            foreach (var modification in modificationCommands)
            {
                AppendInsertOperation(commandStringBuilder, modification, commandPosition, out var localRequiresTransaction);
                requiresTransaction = requiresTransaction || localRequiresTransaction;
            }

            return ResultSetMapping.LastInResultSet;
        }

        private ResultSetMapping AppendInsertMultipleRowsInSingleStatementOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            List<IColumnModification> writeOperations,
            out bool requiresTransaction)
        {
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

            return ResultSetMapping.LastInResultSet | ResultSetMapping.ResultSetWithRowsAffectedOnly;
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

        protected override bool IsIdentityOperation(IColumnModification modification)
        {
            var isIdentityOperation = base.IsIdentityOperation(modification);

            if (isIdentityOperation &&
                modification.Property is { } property)
            {
                var (tableName, schema) = GetTableNameAndSchema(modification, property);
                var storeObject = StoreObjectIdentifier.Table(tableName, schema);

                return property.GetValueGenerationStrategy(storeObject) is MySqlValueGenerationStrategy.IdentityColumn;
            }

            return isIdentityOperation;
        }

        private static (string tableName, string schema) GetTableNameAndSchema(IColumnModification modification, IProperty property)
        {
            if (modification.Column?.Table is { } table)
            {
                return (table.Name, table.Schema);
            }
            else
            {
                // CHECK: Is this branch ever hit and then returns something different than null, or can we just rely on
                // `modification.Column?.Table`?
                return (property.DeclaringEntityType.GetTableName(), property.DeclaringEntityType.GetSchema());
            }
        }
    }
}
