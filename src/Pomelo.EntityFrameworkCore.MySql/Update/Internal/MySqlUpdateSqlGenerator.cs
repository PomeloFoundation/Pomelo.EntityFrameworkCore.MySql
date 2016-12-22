// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Update.Internal
{
    public class MySqlUpdateSqlGenerator : UpdateSqlGenerator, IMySqlUpdateSqlGenerator
    {
        public MySqlUpdateSqlGenerator([NotNull] ISqlGenerationHelper sqlGenerationHelper)
            : base(sqlGenerationHelper)
        {
        }

        public override ResultSetMapping AppendInsertOperation(
           StringBuilder commandStringBuilder,
           ModificationCommand command, 
           int commandPosition)
        {
            Check.NotNull(command, nameof(command));

            return AppendBulkInsertOperation(commandStringBuilder, new[] { command }, commandPosition);
        }


        public ResultSetMapping AppendBulkInsertOperation(StringBuilder commandStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(modificationCommands, nameof(modificationCommands));

            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            // TODO: Support TPH
            var defaultValuesOnly = !modificationCommands.First().ColumnModifications.Any(o => o.IsWrite);
            var statementCount = defaultValuesOnly
                ? modificationCommands.Count
                : 1;
            var valueSetCount = defaultValuesOnly
                ? 1
                : modificationCommands.Count;
            var resultSetCreated = false;
            for (var i = 0; i < statementCount; i++)
            {
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var readOperations = operations.Where(o => o.IsRead).ToArray();

                AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
                /*if (readOperations.Length > 0)
                {
                    AppendOutputClause(commandStringBuilder, readOperations);
                }*/
                AppendValuesHeader(commandStringBuilder, writeOperations);
                AppendValues(commandStringBuilder, writeOperations);
                for (var j = 1; j < valueSetCount; j++)
                {
                    commandStringBuilder.Append(",").AppendLine();
                    AppendValues(commandStringBuilder, modificationCommands[j].ColumnModifications.Where(o => o.IsWrite).ToArray());
                }
                commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

                if (readOperations.Length == 0)
                {
                    AppendSelectAffectedCountCommand(commandStringBuilder, name, schema, commandPosition);
                }
                else if (readOperations.Length > 0)
                {
                    AppendInsertOutputClause(commandStringBuilder, name, schema, readOperations, operations);
                    resultSetCreated = true;
                }
            }

            return resultSetCreated ?
                defaultValuesOnly
                    ? ResultSetMapping.LastInResultSet
                    : ResultSetMapping.NotLastInResultSet
                : ResultSetMapping.NoResultSet;
        }

        public override ResultSetMapping AppendUpdateOperation(
            StringBuilder commandStringBuilder,
            ModificationCommand command,
            int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(command, nameof(command));

            var name = command.TableName;
            var schema = command.Schema;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToArray();
            var conditionOperations = operations.Where(o => o.IsCondition).ToArray();
            var readOperations = operations.Where(o => o.IsRead).ToArray();

            AppendUpdateCommandHeader(commandStringBuilder, name, schema, writeOperations);

            AppendWhereClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

            commandStringBuilder
                .Append("SELECT ");

            AppendUpdateOutputClause(commandStringBuilder, schema, name, readOperations, operations);

            return ResultSetMapping.LastInResultSet;
        }

        private void AppendUpdateOutputClause(StringBuilder commandStringBuilder, string schema, string name, IReadOnlyList<ColumnModification> readOperations, IReadOnlyList<ColumnModification> allOperations)
        {
            if (readOperations.Any())
            {
                foreach (var x in readOperations)
                {
                    commandStringBuilder.Append($"{ SqlGenerationHelper.DelimitIdentifier(x.ColumnName) }, ");
                }

                commandStringBuilder.Append("ROW_COUNT()");

                commandStringBuilder
                    .Append($" FROM { SqlGenerationHelper.DelimitIdentifier(name) }")
                    .Append(" WHERE ");

                var predicates = new List<string>();
                foreach (var x in allOperations.Where(y => y.IsKey))
                {
                    predicates.Add($"{SqlGenerationHelper.DelimitIdentifier(x.ColumnName)} = @{ x.ParameterName }");
                }

                commandStringBuilder
                    .Append(string.Join(" AND ", predicates));
            }
            else
            {
                commandStringBuilder
                    .Append("ROW_COUNT()");
            }

            commandStringBuilder
                    .Append(SqlGenerationHelper.StatementTerminator);
        }


        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void AppendInsertOutputClause(
            StringBuilder commandStringBuilder,
            string name,
            string schema,
            IReadOnlyList<ColumnModification> operations,
            IReadOnlyList<ColumnModification> allOperations)
        {
            if (allOperations.Count > 0 && allOperations[0] == operations[0])
            {
                commandStringBuilder
                    .Append("SELECT LAST_INSERT_ID()");

                if (operations.Count > 1)
                    for (var i = 1; i < operations.Count; i++)
                        commandStringBuilder.Append($", (SELECT { SqlGenerationHelper.DelimitIdentifier(operations[i].ColumnName) } FROM { SqlGenerationHelper.DelimitIdentifier(name) } WHERE { SqlGenerationHelper.DelimitIdentifier(operations.First().ColumnName) } = LAST_INSERT_ID())");
                commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
            }
            else if (operations.Count > 0)
            {
                commandStringBuilder
                    .Append("SELECT ");

                bool isFirst = true;

                foreach(var x in operations)
                {
                    if (!isFirst)
                        commandStringBuilder.Append(", ");
                    if (isFirst)
                        isFirst = false;
                    commandStringBuilder.Append($"{ SqlGenerationHelper.DelimitIdentifier(x.ColumnName) }");
                }

                commandStringBuilder
                    .Append($" FROM { SqlGenerationHelper.DelimitIdentifier(name) }")
                    .Append(" WHERE ");

                var predicates = new List<string>();
                foreach(var x in allOperations.Where(y => y.IsKey))
                {
                    predicates.Add($"{SqlGenerationHelper.DelimitIdentifier(x.ColumnName)} = @{ x.ParameterName }");
                }

                commandStringBuilder
                    .Append(string.Join(" AND ", predicates))
                    .Append(SqlGenerationHelper.StatementTerminator).AppendLine();

            }
        }

        protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name,
            string schema, int commandPosition)
        {
        
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder))
                .Append("SELECT ROW_COUNT()")
                .Append(SqlGenerationHelper.StatementTerminator).AppendLine();

            return ResultSetMapping.LastInResultSet;
        }

        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));

            // TODO: MySql
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
        {
            throw new NotImplementedException();
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
            => Check.NotNull(commandStringBuilder, nameof(commandStringBuilder))
                .Append("ROW_COUNT() = " + expectedRowsAffected);
    }
}
