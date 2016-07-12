// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Update.Internal
{
    public class MySqlUpdateSqlGenerator : UpdateSqlGenerator, IMySqlUpdateSqlGenerator
    {
        public MySqlUpdateSqlGenerator([NotNull] ISqlGenerationHelper SqlGenerationHelper)
            : base(SqlGenerationHelper)
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
                    AppendOutputClause(commandStringBuilder, readOperations);
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
            /*if (readOperations.Length > 0)
            {
                AppendOutputClause(commandStringBuilder, readOperations);
            }*/
            AppendWhereClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

            if (readOperations.Length == 0)
            {
                return AppendSelectAffectedCountCommand(commandStringBuilder, name, schema, commandPosition);
            }
            return ResultSetMapping.LastInResultSet;
        }

        
        
        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void AppendOutputClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<ColumnModification> operations)
            => commandStringBuilder
                .AppendLine()
                .Append("SELECT LAST_INSERT_ID();");

        protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name,
            string schema, int commandPosition)
        {
        
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder))
                .Append("SELECT ROW_COUNT()")
                .Append(SqlGenerationHelper.BatchTerminator).AppendLine();

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
