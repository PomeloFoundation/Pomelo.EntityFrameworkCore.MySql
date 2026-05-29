// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Microting.EntityFrameworkCore.MySql.Update.Internal
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

        /// <summary>
        /// Appends SQL for inserting a row to the commands being built, via an INSERT containing a RETURNING clause
        /// to retrieve any database-generated values.
        /// </summary>
        /// <param name="commandStringBuilder">The builder to which the SQL should be appended.</param>
        /// <param name="command">The command that represents the insert operation.</param>
        /// <param name="commandPosition">The ordinal of this command in the batch.</param>
        /// <param name="requiresTransaction">Returns whether the SQL appended must be executed in a transaction to work correctly.</param>
        /// <returns>The <see cref="ResultSetMapping" /> for the command.</returns>
        public override ResultSetMapping AppendInsertReturningOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            var name = command.TableName;
            var schema = command.Schema;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToList();
            var readOperations = operations.Where(o => o.IsRead).ToList();

            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, name, schema, writeOperations);

            // RETURNING is not supported by MySQL. MariaDB supports INSERT/DELETE RETURNING but not UPDATE RETURNING,
            // so we disable it for both databases to ensure consistent behavior across all DML operations.
            if (_options.ServerVersion.Supports.Returning && readOperations.Count > 0)
            {
                AppendReturningClause(commandStringBuilder, readOperations);
            }

            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            requiresTransaction = false;

            return readOperations.Count > 0 ? ResultSetMapping.LastInResultSet : ResultSetMapping.NoResults;
        }

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

        public override ResultSetMapping AppendUpdateOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            var result = _options.ServerVersion.Supports.Returning
                ? AppendUpdateReturningOperation(commandStringBuilder, command, commandPosition, out requiresTransaction)
                : base.AppendUpdateOperation(commandStringBuilder, command, commandPosition, out requiresTransaction);

            return result;
        }

        /// <summary>
        /// Appends SQL for updating a row to the commands being built, via an UPDATE containing a RETURNING clause
        /// to retrieve any database-generated values or for concurrency checking.
        /// </summary>
        /// <param name="commandStringBuilder">The builder to which the SQL should be appended.</param>
        /// <param name="command">The command that represents the update operation.</param>
        /// <param name="commandPosition">The ordinal of this command in the batch.</param>
        /// <param name="requiresTransaction">Returns whether the SQL appended must be executed in a transaction to work correctly.</param>
        /// <returns>The <see cref="ResultSetMapping" /> for the command.</returns>
        protected override ResultSetMapping AppendUpdateReturningOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            var name = command.TableName;
            var schema = command.Schema;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToList();
            var conditionOperations = operations.Where(o => o.IsCondition).ToList();
            var readOperations = operations.Where(o => o.IsRead).ToList();

            requiresTransaction = false;

            var anyReadOperations = readOperations.Count > 0;

            AppendUpdateCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendWhereClause(commandStringBuilder, conditionOperations);

            // RETURNING is not supported by MySQL. MariaDB supports INSERT/DELETE RETURNING but not UPDATE RETURNING,
            // so we disable it for both databases to ensure consistent behavior across all DML operations.
            if (_options.ServerVersion.Supports.Returning)
            {
                AppendReturningClause(commandStringBuilder, readOperations, anyReadOperations ? null : "1");
            }

            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            return anyReadOperations
                ? ResultSetMapping.LastInResultSet
                : ResultSetMapping.LastInResultSet | ResultSetMapping.ResultSetWithRowsAffectedOnly;
        }

        public override ResultSetMapping AppendDeleteOperation(StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
            => _options.ServerVersion.Supports.Returning
                ? AppendDeleteReturningOperation(commandStringBuilder, command, commandPosition, out requiresTransaction)
                : base.AppendDeleteOperation(commandStringBuilder, command, commandPosition, out requiresTransaction);

        /// <summary>
        /// Appends SQL for deleting a row to the commands being built, via a DELETE containing a RETURNING clause
        /// for concurrency checking.
        /// </summary>
        /// <param name="commandStringBuilder">The builder to which the SQL should be appended.</param>
        /// <param name="command">The command that represents the delete operation.</param>
        /// <param name="commandPosition">The ordinal of this command in the batch.</param>
        /// <param name="requiresTransaction">Returns whether the SQL appended must be executed in a transaction to work correctly.</param>
        /// <returns>The <see cref="ResultSetMapping" /> for the command.</returns>
        protected override ResultSetMapping AppendDeleteReturningOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            var name = command.TableName;
            var schema = command.Schema;
            var conditionOperations = command.ColumnModifications.Where(o => o.IsCondition).ToList();

            requiresTransaction = false;

            AppendDeleteCommandHeader(commandStringBuilder, name, schema);
            AppendWhereClause(commandStringBuilder, conditionOperations);

            // RETURNING is not supported by MySQL. MariaDB supports INSERT/DELETE RETURNING but not UPDATE RETURNING,
            // so we disable it for both databases to ensure consistent behavior across all DML operations.
            if (_options.ServerVersion.Supports.Returning)
            {
                AppendReturningClause(commandStringBuilder, [], "1");
            }

            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            return ResultSetMapping.LastInResultSet | ResultSetMapping.ResultSetWithRowsAffectedOnly;
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

        /// <summary>
        ///     Appends the SQL representation of a column value being updated, with support for partial JSON
        ///     updates using <c>JSON_SET()</c> when the database version supports it.
        /// </summary>
        protected override void AppendUpdateColumnValue(
            ISqlGenerationHelper updateSqlGeneratorHelper,
            IColumnModification columnModification,
            StringBuilder stringBuilder,
            string name,
            string schema)
        {
            if (columnModification.JsonPath is not (null or "$"))
            {
                if (!_options.ServerVersion.Supports.JsonSet)
                {
                    throw new InvalidOperationException(
                        "Cannot perform a partial JSON update because the current database server version does not support JSON_SET(). " +
                        "Upgrade to MySQL 5.7.8+ or MariaDB 10.2.3+, or use a workaround such as AsNoTracking() + Update() to replace the entire JSON value.");
                }

                // Use JSON_SET for partial JSON updates.
                // EF Core provides JsonPath in the format: $.PropertyName or $.PropertyName[0].SubProperty
                // MySQL's JSON_SET uses the same path format.
                stringBuilder
                    .Append("JSON_SET(")
                    .Append(updateSqlGeneratorHelper.DelimitIdentifier(columnModification.ColumnName))
                    .Append(", '");

                stringBuilder.Append(columnModification.JsonPath);

                stringBuilder.Append("', ");

                // When the value is null, EF Core doesn't produce a parameter placeholder for it.
                // We need to use reflection to set the value to the JSON literal "null" so the base method
                // generates the parameter correctly (same approach as Npgsql).
                if (columnModification.Value is null)
                {
                    _columnModificationValueField ??= typeof(ColumnModification).GetField(
                        "_value", BindingFlags.Instance | BindingFlags.NonPublic);

                    if (_columnModificationValueField is null)
                    {
                        throw new InvalidOperationException(
                            "Could not find the internal '_value' field on EF Core's ColumnModification type. " +
                            "This is likely due to an incompatible EF Core version. Please report this issue.");
                    }

                    _columnModificationValueField.SetValue(columnModification, "null");
                }

                base.AppendUpdateColumnValue(updateSqlGeneratorHelper, columnModification, stringBuilder, name, schema);

                stringBuilder.Append(')');
            }
            else
            {
                base.AppendUpdateColumnValue(updateSqlGeneratorHelper, columnModification, stringBuilder, name, schema);
            }
        }

        private FieldInfo _columnModificationValueField;

        public override ResultSetMapping AppendStoredProcedureCall(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            Check.DebugAssert(command.StoreStoredProcedure is not null, "command.StoreStoredProcedure is not null");

            var storedProcedure = command.StoreStoredProcedure;

            Check.DebugAssert(storedProcedure.Parameters.Any(), "Stored procedure call without parameters");

            var resultSetMapping = ResultSetMapping.NoResults;

            // IN parameters will get injected directly into the argument list of the CALL statement.
            // FOR INOUT or OUT parameters, we will declare variables.
            // For OUT parameters, we initialize those variables to NUll.
            // For IN parameters, we initialize those variables with their corresponding parameter of the command that executes the CALL
            // statement.
            for (var i = 0; i < command.ColumnModifications.Count; i++)
            {
                var columnModification = command.ColumnModifications[i];
                var parameter = (IStoreStoredProcedureParameter)columnModification.Column!;

                // MySQL stored procedures cannot return a regular result set, and output parameter values are simply sent back to us as the
                // result set, if we append a SELECT query for them. This is very different from SQL Server, where output parameter values
                // can be sent back in addition to result sets.
                if (!parameter.Direction.HasFlag(ParameterDirection.Output))
                {
                    continue;
                }

                // The distinction between having only a rows affected output parameter and having other non-rows affected parameters
                // is important later on (i.e. whether we need to propagate or not).
                resultSetMapping = parameter == command.RowsAffectedColumn &&
                                   resultSetMapping == ResultSetMapping.NoResults
                    ? ResultSetMapping.ResultSetWithRowsAffectedOnly | ResultSetMapping.LastInResultSet
                    : ResultSetMapping.LastInResultSet;

                commandStringBuilder.Append("SET ");

                var commandParameterName = columnModification.UseOriginalValueParameter
                    ? columnModification.OriginalParameterName!
                    : columnModification.ParameterName!;

                var procedureCallParameterName = GetProcedureCallOutParameterVariableName(commandParameterName);

                SqlGenerationHelper.GenerateParameterNamePlaceholder(commandStringBuilder, procedureCallParameterName);

                commandStringBuilder.Append(" = ");

                if (parameter.Direction.HasFlag(ParameterDirection.Input))
                {
                    SqlGenerationHelper.GenerateParameterNamePlaceholder(commandStringBuilder, commandParameterName);
                }
                else
                {
                    commandStringBuilder.Append("NULL");
                }

                commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
            }

            commandStringBuilder.Append("CALL ");

            // MySQL supports neither a return value nor a result set that gets returned from inside of a stored procedures. It only
            // supports output parameters to propagate values back to the caller.
            Check.DebugAssert(storedProcedure.ReturnValue is null, "storedProcedure.Return is null");
            Check.DebugAssert(!storedProcedure.ResultColumns.Any(), "!storedProcedure.ResultColumns.Any()");

            SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, storedProcedure.Name, storedProcedure.Schema);

            commandStringBuilder.Append('(');

            // Only positional parameter style supported for now, see https://github.com/dotnet/efcore/issues/28439

            // Note: the column modifications are already ordered according to the sproc parameter ordering
            // (see ModificationCommand.GenerateColumnModifications)
            for (var i = 0; i < command.ColumnModifications.Count; i++)
            {
                var columnModification = command.ColumnModifications[i];
                var parameter = (IStoreStoredProcedureParameter)columnModification.Column!;

                if (i > 0)
                {
                    commandStringBuilder.Append(", ");
                }

                Check.DebugAssert(columnModification.UseParameter, "Column modification matched a parameter, but UseParameter is false");

                var commandParameterName = columnModification.UseOriginalValueParameter
                    ? columnModification.OriginalParameterName!
                    : columnModification.ParameterName!;

                var procedureCallParameterName = GetProcedureCallOutParameterVariableName(commandParameterName);

                SqlGenerationHelper.GenerateParameterNamePlaceholder(
                    commandStringBuilder,
                    parameter.Direction.HasFlag(ParameterDirection.Output)
                        ? procedureCallParameterName
                        : commandParameterName);
            }

            commandStringBuilder.Append(')');
            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            // The CALL has propagated any INOUT and OUT values back into our previously declared variables.
            // To get those values back to the caller, we need to run a SELECT statement against those variables.
            // We start by checking, whether there exist any INOUT or OUT parameters.
            if (resultSetMapping != ResultSetMapping.NoResults)
            {
                commandStringBuilder.Append("SELECT ");

                var first = true;

                for (var i = 0; i < command.ColumnModifications.Count; i++)
                {
                    var columnModification = command.ColumnModifications[i];
                    var parameter = (IStoreStoredProcedureParameter)columnModification.Column!;

                    if (!parameter.Direction.HasFlag(ParameterDirection.Output))
                    {
                        continue;
                    }

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        commandStringBuilder.Append(", ");
                    }

                    var commandParameterName = columnModification.UseOriginalValueParameter
                        ? columnModification.OriginalParameterName!
                        : columnModification.ParameterName!;

                    var procedureCallParameterName = GetProcedureCallOutParameterVariableName(commandParameterName);

                    SqlGenerationHelper.GenerateParameterNamePlaceholder(
                        commandStringBuilder,
                        procedureCallParameterName);
                }

                commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
            }

            requiresTransaction = true;

            return resultSetMapping;
        }

        /// <summary>
        /// Returns the name (without the @ prefix) used for any temporary user variables, that need to be declared to get values out of a
        /// stored procedure.
        /// </summary>
        /// <param name="commandParameterName">The name of the parameter of the command that executes the CALL statement.</param>
        /// <returns>The variable name (without the @ prefix).</returns>
        protected virtual string GetProcedureCallOutParameterVariableName(string commandParameterName)
            => "_out_" + commandParameterName;

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

        public override void PrependEnsureAutocommit(StringBuilder commandStringBuilder)
            => commandStringBuilder.Insert(0, $"SET AUTOCOMMIT = 1{SqlGenerationHelper.StatementTerminator}{Environment.NewLine}");

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
                return (property.DeclaringType.GetTableName(), property.DeclaringType.GetSchema());
            }
        }
    }
}
