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
using Pomelo.EntityFrameworkCore.MySql.Extensions;

namespace Pomelo.EntityFrameworkCore.MySql.Update.Internal
{
    // TODO: Revamp
    public class MySqlUpdateSqlGenerator : UpdateSqlGenerator, IMySqlUpdateSqlGenerator
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
            IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition)
        {
            if (modificationCommands.Count == 1
                && modificationCommands[0].ColumnModifications.All(o =>
                    !o.IsKey
                    || !o.IsRead
                    || o.Property?.GetValueGenerationStrategy() == MySqlValueGenerationStrategy.IdentityColumn))
            {
                return AppendInsertOperation(commandStringBuilder, modificationCommands[0], commandPosition);
            }

            var readOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsRead).ToList();
            var writeOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsWrite).ToList();
            var keyOperations = modificationCommands[0].ColumnModifications.Where(o => o.IsKey).ToList();

            var defaultValuesOnly = writeOperations.Count == 0;
            var nonIdentityOperations = modificationCommands[0].ColumnModifications
                .Where(o => o.Property?.GetValueGenerationStrategy() != MySqlValueGenerationStrategy.IdentityColumn)
                .ToList();

            if (defaultValuesOnly)
            {
                if (nonIdentityOperations.Count == 0
                    || readOperations.Count == 0)
                {
                    foreach (var modification in modificationCommands)
                    {
                        AppendInsertOperation(commandStringBuilder, modification, commandPosition);
                    }

                    return readOperations.Count == 0
                        ? ResultSetMapping.NoResultSet
                        : ResultSetMapping.LastInResultSet;
                }

                if (nonIdentityOperations.Count > 1)
                {
                    nonIdentityOperations = new List<ColumnModification> { nonIdentityOperations.First() };
                }
            }

            if (readOperations.Count == 0)
            {
                return AppendBulkInsertWithoutServerValues(commandStringBuilder, modificationCommands, writeOperations);
            }

            foreach (var modification in modificationCommands)
            {
                AppendInsertOperation(commandStringBuilder, modification, commandPosition);
            }

            return ResultSetMapping.LastInResultSet;
        }

        private ResultSetMapping AppendBulkInsertWithoutServerValues(
            StringBuilder commandStringBuilder,
            IReadOnlyList<ModificationCommand> modificationCommands,
            List<ColumnModification> writeOperations)
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

            return ResultSetMapping.NoResultSet;
        }

        protected override void AppendInsertCommandHeader(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> operations)
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
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder.AppendLine();
            commandStringBuilder.Append("VALUES ");
        }

        protected override void AppendValues(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> operations)
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

        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            // TODO: what is the effect of this statment?
            // there is no equivalent in mysql: https://stackoverflow.com/questions/3386217/is-there-an-equivalent-to-sql-servers-set-nocount-in-mysql
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
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
