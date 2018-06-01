// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    /// <summary>
    ///     MySql-specific implementation of <see cref="MigrationsSqlGenerator" />.
    /// </summary>
    public class MySqlMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private static readonly Regex _typeRe = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?",
            RegexOptions.IgnoreCase);

        private readonly IMigrationsAnnotationProvider _migrationsAnnotations;
        private readonly IMySqlOptions _options;

        private IReadOnlyList<MigrationOperation> _operations;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        /// <param name="migrationsAnnotations"> Provider-specific Migrations annotations to use. </param>
        /// <param name="options">IMySqlOptions</param>
        public MySqlMigrationsSqlGenerator(
            [NotNull] MigrationsSqlGeneratorDependencies dependencies,
            [NotNull] IMigrationsAnnotationProvider migrationsAnnotations,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            _migrationsAnnotations = migrationsAnnotations;
            _options = options;
        }

        /// <summary>
        ///     Generates commands from a list of operations.
        /// </summary>
        /// <param name="operations"> The operations. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <returns> The list of commands to be executed or scripted. </returns>
        public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations,
            IModel model = null)
        {
            _operations = operations;
            try
            {
                return base.Generate(operations, model);
            }
            finally
            {
                _operations = null;
            }
        }

        /// <summary>
        ///     <para>
        ///         Builds commands for the given <see cref="MigrationOperation" /> by making calls on the given
        ///         <see cref="MigrationCommandListBuilder" />.
        ///     </para>
        ///     <para>
        ///         This method uses a double-dispatch mechanism to call one of the 'Generate' methods that are
        ///         specific to a certain subtype of <see cref="MigrationOperation" />. Typically database providers
        ///         will override these specific methods rather than this method. However, providers can override
        ///         this methods to handle provider-specific operations.
        ///     </para>
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(MigrationOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var dropDatabaseOperation = operation as MySqlDropDatabaseOperation;
            if (operation is MySqlCreateDatabaseOperation createDatabaseOperation)
            {
                Generate(createDatabaseOperation, model, builder);
            }
            else if (dropDatabaseOperation != null)
            {
                Generate(dropDatabaseOperation, model, builder);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="AlterColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            AlterColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            IEnumerable<IIndex> indexesToRebuild = null;
            var property = FindProperty(model, operation.Schema, operation.Table, operation.Name);

            if (operation.ComputedColumnSql != null)
            {
                var dropColumnOperation = new DropColumnOperation
                {
                    Schema = operation.Schema,
                    Table = operation.Table,
                    Name = operation.Name
                };
                if (property != null)
                {
                    dropColumnOperation.AddAnnotations(_migrationsAnnotations.ForRemove(property));
                }

                var addColumnOperation = new AddColumnOperation
                {
                    Schema = operation.Schema,
                    Table = operation.Table,
                    Name = operation.Name,
                    ClrType = operation.ClrType,
                    ColumnType = operation.ColumnType,
                    IsUnicode = operation.IsUnicode,
                    MaxLength = operation.MaxLength,
                    IsRowVersion = operation.IsRowVersion,
                    IsNullable = operation.IsNullable,
                    DefaultValue = operation.DefaultValue,
                    DefaultValueSql = operation.DefaultValueSql,
                    ComputedColumnSql = operation.ComputedColumnSql,
                    IsFixedLength = operation.IsFixedLength
                };
                addColumnOperation.AddAnnotations(operation.GetAnnotations());

                // TODO: Use a column rebuild instead
                indexesToRebuild = GetIndexesToRebuild(property, operation).ToList();
                DropIndexes(indexesToRebuild, builder);
                Generate(dropColumnOperation, model, builder, terminate: false);
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                Generate(addColumnOperation, model, builder, terminate: false);
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                CreateIndexes(indexesToRebuild, builder);
                builder.EndCommand();

                return;
            }

            var narrowed = false;
            if (IsOldColumnSupported(model))
            {
                var valueGenerationStrategy = operation[
                    MySqlAnnotationNames.ValueGenerationStrategy] as MySqlValueGenerationStrategy?;
                var identity = valueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn;
                var oldValueGenerationStrategy = operation.OldColumn[
                    MySqlAnnotationNames.ValueGenerationStrategy] as MySqlValueGenerationStrategy?;
                var oldIdentity = oldValueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn;
                if (identity != oldIdentity)
                {
                    throw new InvalidOperationException(MySqlStrings.AlterIdentityColumn);
                }

                var type = operation.ColumnType
                           ?? GetColumnType(
                               operation.Schema,
                               operation.Table,
                               operation.Name,
                               operation.ClrType,
                               operation.IsUnicode,
                               operation.MaxLength,
                               operation.IsFixedLength,
                               operation.IsRowVersion,
                               model);
                var oldType = operation.OldColumn.ColumnType
                              ?? GetColumnType(
                                  operation.Schema,
                                  operation.Table,
                                  operation.Name,
                                  operation.OldColumn.ClrType,
                                  operation.OldColumn.IsUnicode,
                                  operation.OldColumn.MaxLength,
                                  operation.OldColumn.IsFixedLength,
                                  operation.OldColumn.IsRowVersion,
                                  model);
                narrowed = type != oldType || !operation.IsNullable && operation.OldColumn.IsNullable;
            }

            if (narrowed)
            {
                indexesToRebuild = GetIndexesToRebuild(property, operation).ToList();
                DropIndexes(indexesToRebuild, builder);
            }

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" MODIFY COLUMN ");

            ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                operation.ClrType,
                operation.ColumnType,
                operation.IsUnicode,
                operation.MaxLength,
                operation.IsFixedLength,
                operation.IsRowVersion,
                operation.IsNullable,
                /*defaultValue:*/ null,
                /*defaultValueSql:*/ null,
                operation.ComputedColumnSql,
                /*identity:*/ false,
                operation,
                model,
                builder);

            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            if (operation.DefaultValue != null
                || operation.DefaultValueSql != null)
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                    .Append(" ADD");
                DefaultValue(operation.DefaultValue, operation.DefaultValueSql, builder);
                builder
                    .Append(" FOR ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            }

            if (narrowed)
            {
                CreateIndexes(indexesToRebuild, builder);
            }

            builder.EndCommand();
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameIndexOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.NewName != null)
            {
                if (_options.ServerVersion.SupportsRenameIndex)
                {
                    builder.Append("ALTER TABLE ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                        .Append(" RENAME INDEX ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                        .Append(" TO ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                        .AppendLine(";");

                    EndStatement(builder);
                }
                else
                {
                    var index = FindEntityTypes(model, operation.Schema, operation.Table)
                        ?.SelectMany(e => e.GetDeclaredIndexes())
                        .FirstOrDefault(i => i.Relational().Name == operation.NewName);
                    if (index == null)
                    {
                        throw new InvalidOperationException(
                            $"Could not find the model index: {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema)}.{Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName)}. Upgrade to Mysql 5.7+ or split the 'RenameIndex' call into 'DropIndex' and 'CreateIndex'");
                    }

                    Generate(new DropIndexOperation
                    {
                        Schema = operation.Schema,
                        Table = operation.Table,
                        Name = operation.Name
                    }, model, builder);

                    var createIndexOperation = new CreateIndexOperation
                    {
                        Schema = operation.Schema,
                        Table = operation.Table,
                        Name = operation.NewName,
                        Columns = index.Properties.Select(p => p.Relational().ColumnName).ToArray(),
                        IsUnique = index.IsUnique,
                        Filter = index.Relational().Filter
                    };
                    createIndexOperation.AddAnnotations(_migrationsAnnotations.For(index));
                    createIndexOperation.AddAnnotations(operation.GetAnnotations());

                    Generate(createIndexOperation, model, builder);
                }
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RestartSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RestartSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER SEQUENCE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .Append(" RESTART WITH ")
                .Append(IntegerConstant(operation.StartValue))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameTableOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameTableOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .Append(" RENAME ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName, operation.NewSchema));

            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="EnsureSchemaOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(EnsureSchemaOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE IF NOT EXISTS ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            CreateSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE SEQUENCE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));

            if (operation.ClrType != typeof(long))
            {
                var typeMapping = Dependencies.TypeMappingSource.GetMapping(operation.ClrType);

                builder
                    .Append(" AS ")
                    .Append(typeMapping.StoreType);
            }

            builder
                .Append(" START WITH ")
                .Append(IntegerConstant(operation.StartValue));

            SequenceOptions(operation, model, builder);

            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="MySqlCreateDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected virtual void Generate(
            [NotNull] MySqlCreateDatabaseOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            builder
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        private static string ExpandFileName(string fileName)
        {
            Check.NotNull(fileName, nameof(fileName));

            if (fileName.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
                if (string.IsNullOrEmpty(dataDirectory))
                {
                    dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                fileName = Path.Combine(dataDirectory, fileName.Substring("|DataDirectory|".Length));
            }

            return Path.GetFullPath(fileName);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="MySqlDropDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected virtual void Generate(
            [NotNull] MySqlDropDatabaseOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP DATABASE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(Dependencies.SqlGenerationHelper.StatementTerminator)
                .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="DropIndexOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            DropIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
            => Generate(operation, model, builder, terminate: true);

        /// <summary>
        ///     Builds commands for the given <see cref="DropIndexOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected virtual void Generate(
            [NotNull] DropIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP INDEX ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="DropColumnOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            DropColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
            => Generate(operation, model, builder, terminate: true);

        /// <summary>
        ///     Builds commands for the given <see cref="DropForeignKeyOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            [NotNull] DropForeignKeyOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
            => Generate(operation, model, builder, terminate: true);

        /// <summary>
        ///     Builds commands for the given <see cref="DropForeignKeyOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(
            [NotNull] DropForeignKeyOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP FOREIGN KEY ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" CHANGE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" ");

            var property = FindProperty(model, operation.Schema, operation.Table, operation.NewName);
            if (property == null)
            {
                var type = operation[RelationalAnnotationNames.ColumnType];
                if (type == null)
                {
                    throw new InvalidOperationException(
                        $"Could not find the property corresponding to the column: {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema)}.{Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName)}. Specify the column type explicitly on 'RenameColumn' using the \"{RelationalAnnotationNames.ColumnType}\" annotation");
                }

                builder
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                    .Append(" ")
                    .Append(type);

                EndStatement(builder);
                return;
            }

            var typeMapping = Dependencies.TypeMappingSource.GetMapping(property);
            var converter = typeMapping.Converter;
            var clrType = (converter?.ProviderClrType ?? typeMapping.ClrType).UnwrapNullableType();
            var columnType = (string)(operation[RelationalAnnotationNames.ColumnType]
                                      ?? property[RelationalAnnotationNames.ColumnType]);
            var annotations = property.Relational();
            var isNullable = property.IsColumnNullable();

            var defaultValue = annotations.DefaultValue;
            defaultValue = converter != null
                ? converter.ConvertToProvider(defaultValue)
                : defaultValue;
            defaultValue = (defaultValue == DBNull.Value ? null : defaultValue)
                           ?? (isNullable
                               ? null
                               : clrType == typeof(string)
                                   ? string.Empty
                                   : clrType.IsArray
                                       ? Array.CreateInstance(clrType.GetElementType(), 0)
                                       : clrType.GetDefaultValue());

            var isRowVersion = property.ClrType == typeof(byte[])
                               && property.IsConcurrencyToken
                               && property.ValueGenerated == ValueGenerated.OnAddOrUpdate;
            ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.NewName,
                clrType,
                columnType,
                property.IsUnicode(),
                property.GetMaxLength(),
                annotations.IsFixedLength,
                isRowVersion,
                isNullable,
                defaultValue,
                annotations.DefaultValueSql,
                annotations.ComputedColumnSql,
                operation,
                model,
                builder);

            EndStatement(builder);
        }

        /// <summary>
        ///     Generates a SQL fragment configuring a sequence with the given options.
        /// </summary>
        /// <param name="schema"> The schema that contains the sequence, or <c>null</c> to use the default schema. </param>
        /// <param name="name"> The sequence name. </param>
        /// <param name="increment"> The amount to increment by to generate the next value in the sequence. </param>
        /// <param name="minimumValue"> The minimum value supported by the sequence, or <c>null</c> if none was specified. </param>
        /// <param name="maximumValue"> The maximum value supported by the sequence, or <c>null</c> if none was specified. </param>
        /// <param name="cycle"> Indicates whether or not the sequence will start again once the maximum value is reached. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void SequenceOptions(
            string schema,
            string name,
            int increment,
            long? minimumValue,
            long? maximumValue,
            bool cycle,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(increment, nameof(increment));
            Check.NotNull(cycle, nameof(cycle));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append(" INCREMENT BY ")
                .Append(IntegerConstant(increment));

            if (minimumValue.HasValue)
            {
                builder
                    .Append(" MINVALUE ")
                    .Append(IntegerConstant(minimumValue.Value));
            }
            else
            {
                builder.Append(" NO MINVALUE");
            }

            if (maximumValue.HasValue)
            {
                builder
                    .Append(" MAXVALUE ")
                    .Append(IntegerConstant(maximumValue.Value));
            }
            else
            {
                builder.Append(" NO MAXVALUE");
            }

            builder.Append(cycle ? " CYCLE" : " NO CYCLE");
        }

        /// <summary>
        ///     Generates a SQL fragment for a column definition in an <see cref="AddColumnOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ColumnDefinition(AddColumnOperation operation, IModel model,
            MigrationCommandListBuilder builder)
            => ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                operation.ClrType,
                operation.ColumnType,
                operation.IsUnicode,
                operation.MaxLength,
                operation.IsFixedLength,
                operation.IsRowVersion,
                operation.IsNullable,
                operation.DefaultValue,
                operation.DefaultValueSql,
                operation.ComputedColumnSql,
                operation,
                model,
                builder);

        /// <summary>
        ///     Generates a SQL fragment for a column definition for the given column metadata.
        /// </summary>
        /// <param name="schema"> The schema that contains the table, or <c>null</c> to use the default schema. </param>
        /// <param name="table"> The table that contains the column. </param>
        /// <param name="name"> The column name. </param>
        /// <param name="clrType"> The CLR <see cref="Type" /> that the column is mapped to. </param>
        /// <param name="type"> The database/store type for the column, or <c>null</c> if none has been specified. </param>
        /// <param name="unicode">
        ///     Indicates whether or not the column can contain Unicode data, or <c>null</c> if this is not applicable or not specified.
        /// </param>
        /// <param name="maxLength">
        ///     The maximum amount of data that the column can contain, or <c>null</c> if this is not applicable or not specified.
        /// </param>
        /// <param name="fixedLength"> Indicates whether or not the column is constrained to fixed-length data. </param>
        /// <param name="rowVersion">
        ///     Indicates whether or not this column is an automatic concurrency token, such as a MySql timestamp/rowversion.
        /// </param>
        /// <param name="nullable"> Indicates whether or not the column can store <c>NULL</c> values. </param>
        /// <param name="defaultValue"> The default value for the column. </param>
        /// <param name="defaultValueSql"> The SQL expression to use for the column's default constraint. </param>
        /// <param name="computedColumnSql"> The SQL expression to use to compute the column value. </param>
        /// <param name="annotatable"> The <see cref="MigrationOperation" /> to use to find any custom annotations. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ColumnDefinition(
            string schema,
            string table,
            string name,
            Type clrType,
            string type,
            bool? unicode,
            int? maxLength,
            bool? fixedLength,
            bool rowVersion,
            bool nullable,
            object defaultValue,
            string defaultValueSql,
            string computedColumnSql,
            IAnnotatable annotatable,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            var valueGenerationStrategy = annotatable[
                MySqlAnnotationNames.ValueGenerationStrategy] as MySqlValueGenerationStrategy?;

            ColumnDefinition(
                schema,
                table,
                name,
                clrType,
                type,
                unicode,
                maxLength,
                fixedLength,
                rowVersion,
                nullable,
                defaultValue,
                defaultValueSql,
                computedColumnSql,
                valueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn,
                annotatable,
                model,
                builder);
        }

        /// <summary>
        ///     Generates a SQL fragment for a column definition for the given column metadata.
        /// </summary>
        /// <param name="schema"> The schema that contains the table, or <c>null</c> to use the default schema. </param>
        /// <param name="table"> The table that contains the column. </param>
        /// <param name="name"> The column name. </param>
        /// <param name="clrType"> The CLR <see cref="Type" /> that the column is mapped to. </param>
        /// <param name="type"> The database/store type for the column, or <c>null</c> if none has been specified. </param>
        /// <param name="unicode">
        ///     Indicates whether or not the column can contain Unicode data, or <c>null</c> if this is not applicable or not specified.
        /// </param>
        /// <param name="maxLength">
        ///     The maximum amount of data that the column can contain, or <c>null</c> if this is not applicable or not specified.
        /// </param>
        /// <param name="rowVersion">
        ///     Indicates whether or not this column is an automatic concurrency token, such as a MySql timestamp/rowversion.
        /// </param>
        /// <param name="nullable"> Indicates whether or not the column can store <c>NULL</c> values. </param>
        /// <param name="defaultValue"> The default value for the column. </param>
        /// <param name="defaultValueSql"> The SQL expression to use for the column's default constraint. </param>
        /// <param name="computedColumnSql"> The SQL expression to use to compute the column value. </param>
        /// <param name="annotatable"> The <see cref="MigrationOperation" /> to use to find any custom annotations. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ColumnDefinition(
            string schema,
            string table,
            string name,
            Type clrType,
            string type,
            bool? unicode,
            int? maxLength,
            bool rowVersion,
            bool nullable,
            object defaultValue,
            string defaultValueSql,
            string computedColumnSql,
            IAnnotatable annotatable,
            IModel model,
            MigrationCommandListBuilder builder)
            => ColumnDefinition(schema, table, name, clrType, type, unicode, maxLength, null,
                rowVersion, nullable, defaultValue, defaultValueSql, computedColumnSql, annotatable, model, builder);

        /// <summary>
        ///     Generates a SQL fragment for a column definition for the given column metadata.
        /// </summary>
        /// <param name="schema"> The schema that contains the table, or <c>null</c> to use the default schema. </param>
        /// <param name="table"> The table that contains the column. </param>
        /// <param name="name"> The column name. </param>
        /// <param name="clrType"> The CLR <see cref="Type" /> that the column is mapped to. </param>
        /// <param name="type"> The database/store type for the column, or <c>null</c> if none has been specified. </param>
        /// <param name="unicode">
        ///     Indicates whether or not the column can contain Unicode data, or <c>null</c> if this is not applicable or not specified.
        /// </param>
        /// <param name="maxLength">
        ///     The maximum amount of data that the column can contain, or <c>null</c> if this is not applicable or not specified.
        /// </param>
        /// <param name="fixedLength"> Indicates whether or not the column is constrained to fixed-length data. </param>
        /// <param name="rowVersion">
        ///     Indicates whether or not this column is an automatic concurrency token, such as a MySql timestamp/rowversion.
        /// </param>
        /// <param name="nullable"> Indicates whether or not the column can store <c>NULL</c> values. </param>
        /// <param name="defaultValue"> The default value for the column. </param>
        /// <param name="defaultValueSql"> The SQL expression to use for the column's default constraint. </param>
        /// <param name="computedColumnSql"> The SQL expression to use to compute the column value. </param>
        /// <param name="identity"> Indicates whether or not the column is an Identity column. </param>
        /// <param name="annotatable"> The <see cref="MigrationOperation" /> to use to find any custom annotations. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected virtual void ColumnDefinition(
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string name,
            [NotNull] Type clrType,
            [CanBeNull] string type,
            bool? unicode,
            int? maxLength,
            bool? fixedLength,
            bool rowVersion,
            bool nullable,
            [CanBeNull] object defaultValue,
            [CanBeNull] string defaultValueSql,
            [CanBeNull] string computedColumnSql,
            bool identity,
            [NotNull] IAnnotatable annotatable,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(annotatable, nameof(annotatable));
            Check.NotNull(builder, nameof(builder));

            var matchType = type ?? GetColumnType(schema, table, name, clrType, unicode, maxLength, fixedLength, rowVersion, model);
            var matchLen = "";
            var match = _typeRe.Match(matchType ?? "-");
            if (match.Success)
            {
                matchType = match.Groups[1].Value.ToLower();
                if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                {
                    matchLen = match.Groups[2].Value;
                }
            }

            var valueGenerationStrategy = annotatable[
                MySqlAnnotationNames.ValueGenerationStrategy] as MySqlValueGenerationStrategy?;
            if (!valueGenerationStrategy.HasValue)
            {
                var generatedOnAddAnnotation = annotatable[MySqlAnnotationNames.LegacyValueGeneratedOnAdd];
                if (generatedOnAddAnnotation != null && (bool)generatedOnAddAnnotation)
                    valueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;
                var generatedOnAddOrUpdateAnnotation =
                    annotatable[MySqlAnnotationNames.LegacyValueGeneratedOnAddOrUpdate];
                if (generatedOnAddOrUpdateAnnotation != null && (bool)generatedOnAddOrUpdateAnnotation)
                    valueGenerationStrategy = MySqlValueGenerationStrategy.ComputedColumn;
            }

            var autoIncrement = false;
            if ((identity || valueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn) &&
                string.IsNullOrWhiteSpace(defaultValueSql) && defaultValue == null)
            {
                switch (matchType)
                {
                    case "tinyint":
                    case "smallint":
                    case "mediumint":
                    case "int":
                    case "bigint":
                        autoIncrement = true;
                        break;
                    case "datetime":
                        if (!_options.ServerVersion.SupportsDateTime6)
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                "on Add or Update in MySql <= 5.5, try explicitly setting the column type to TIMESTAMP");
                        goto case "timestamp";
                    case "timestamp":
                        defaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
                        break;
                }
            }

            string onUpdateSql = null;
            if (rowVersion || valueGenerationStrategy == MySqlValueGenerationStrategy.ComputedColumn)
            {
                switch (matchType)
                {
                    case "datetime":
                        if (!_options.ServerVersion.SupportsDateTime6)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                "on Add or Update in MySql <= 5.5, try explicitly setting the column type to TIMESTAMP");
                        }

                        goto case "timestamp";
                    case "timestamp":
                        if (string.IsNullOrWhiteSpace(defaultValueSql) && defaultValue == null)
                        {
                            defaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
                        }

                        onUpdateSql = $"CURRENT_TIMESTAMP({matchLen})";
                        break;
                }
            }

            base.ColumnDefinition(
                schema,
                table,
                name,
                clrType,
                type,
                unicode,
                maxLength,
                fixedLength,
                rowVersion,
                nullable,
                identity
                    ? null
                    : defaultValue,
                defaultValueSql,
                computedColumnSql,
                annotatable,
                model,
                builder);

            if (autoIncrement)
            {
                builder.Append(" AUTO_INCREMENT");
            }
            else
            {
                if (onUpdateSql != null)
                {
                    builder
                        .Append(" ON UPDATE ")
                        .Append(onUpdateSql);
                }

                if (computedColumnSql != null)
                {
                    builder
                        .Append(" AS ")
                        .Append(computedColumnSql);
                }
            }
        }


        /// <summary>
        ///     Generates a SQL fragment for the default constraint of a column.
        /// </summary>
        /// <param name="defaultValue"> The default value for the column. </param>
        /// <param name="defaultValueSql"> The SQL expression to use for the column's default constraint. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void DefaultValue(
            object defaultValue,
            string defaultValueSql,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (defaultValueSql != null)
            {
                builder
                    .Append(" DEFAULT ")
                    .Append(defaultValueSql);
            }
            else if (defaultValue != null)
            {
                var typeMapping = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue);

                builder
                    .Append(" DEFAULT ")
                    .Append(typeMapping.GenerateSqlLiteral(defaultValue));
            }
        }

        protected override void Generate(
            AddPrimaryKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ADD ");
            PrimaryKeyConstraint(operation, model, builder);
            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            if (operation.Columns.Length == 1)
            {
                builder.Append(@"DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;
CREATE PROCEDURE POMELO_AFTER_ADD_PRIMARY_KEY(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END;".Replace("\r", string.Empty).Replace("\n", Environment.NewLine));
                builder.AppendLine();

                if (operation.Schema == null)
                {
                    builder.Append(
                        $"CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, '{operation.Table}', '{operation.Columns.First()}');");
                }
                else
                {
                    builder.Append(
                        $"CALL POMELO_AFTER_ADD_PRIMARY_KEY('{operation.Schema}', '{operation.Table}', '{operation.Columns.First()}');");
                }

                builder.AppendLine();
                builder.Append($"DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;");
                builder.AppendLine();
            }

            EndStatement(builder);
        }

        protected override void Generate(
            DropPrimaryKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            builder.Append(@"DROP PROCEDURE IF EXISTS POMELO_BEFORE_DROP_PRIMARY_KEY;
CREATE PROCEDURE POMELO_BEFORE_DROP_PRIMARY_KEY(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END;".Replace("\r", string.Empty).Replace("\n", Environment.NewLine));
            builder.AppendLine();

            if (string.IsNullOrWhiteSpace(operation.Schema))
            {
                builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, '{operation.Table}');");
            }
            else
            {
                builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY('{operation.Schema}', '{operation.Table}');");
            }

            builder.AppendLine();
            builder.Append($"DROP PROCEDURE IF EXISTS POMELO_BEFORE_DROP_PRIMARY_KEY;");
            builder.AppendLine();

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP PRIMARY KEY;")
                .AppendLine();

            EndStatement(builder);
        }

        /// <summary>
        ///     Generates a SQL fragment for traits of an index from a <see cref="CreateIndexOperation" />,
        ///     <see cref="AddPrimaryKeyOperation" />, or <see cref="AddUniqueConstraintOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void IndexTraits(MigrationOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var fullText = operation[MySqlAnnotationNames.FullTextIndex] as bool?;
            if (fullText == true)
            {
                builder.Append("FULLTEXT ");
            }

            var spatial = operation[MySqlAnnotationNames.SpatialIndex] as bool?;
            if (spatial == true)
            {
                builder.Append("SPATIAL ");
            }
        }

        /// <summary>
        ///     Generates a SQL fragment for the given referential action.
        /// </summary>
        /// <param name="referentialAction"> The referential action. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ForeignKeyAction(ReferentialAction referentialAction,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (referentialAction == ReferentialAction.Restrict)
            {
                builder.Append("NO ACTION");
            }
            else
            {
                base.ForeignKeyAction(referentialAction, builder);
            }
        }

        /// <summary>
        ///     Gets the list of indexes that need to be rebuilt when the given property is changing.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="currentOperation"> The operation which may require a rebuild. </param>
        /// <returns> The list of indexes affected. </returns>
        protected virtual IEnumerable<IIndex> GetIndexesToRebuild(
            [CanBeNull] IProperty property,
            [NotNull] MigrationOperation currentOperation)
        {
            Check.NotNull(currentOperation, nameof(currentOperation));

            if (property == null)
            {
                yield break;
            }

            var createIndexOperations = _operations.SkipWhile(o => o != currentOperation).Skip(1)
                .OfType<CreateIndexOperation>().ToList();
            foreach (var index in property.GetContainingIndexes())
            {
                var indexName = index.Relational().Name;
                if (createIndexOperations.Any(o => o.Name == indexName))
                {
                    continue;
                }

                yield return index;
            }
        }

        /// <summary>
        ///     Generates SQL to drop the given indexes.
        /// </summary>
        /// <param name="indexes"> The indexes to drop. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected virtual void DropIndexes(
            [NotNull] IEnumerable<IIndex> indexes,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(indexes, nameof(indexes));
            Check.NotNull(builder, nameof(builder));

            foreach (var index in indexes)
            {
                var operation = new DropIndexOperation
                {
                    Schema = index.DeclaringEntityType.Relational().Schema,
                    Table = index.DeclaringEntityType.Relational().TableName,
                    Name = index.Relational().Name
                };
                operation.AddAnnotations(_migrationsAnnotations.ForRemove(index));

                Generate(operation, index.DeclaringEntityType.Model, builder, terminate: false);
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            }
        }

        /// <summary>
        ///     Generates SQL to create the given indexes.
        /// </summary>
        /// <param name="indexes"> The indexes to create. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected virtual void CreateIndexes(
            [NotNull] IEnumerable<IIndex> indexes,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(indexes, nameof(indexes));
            Check.NotNull(builder, nameof(builder));

            foreach (var index in indexes)
            {
                var operation = new CreateIndexOperation
                {
                    IsUnique = index.IsUnique,
                    Name = index.Relational().Name,
                    Schema = index.DeclaringEntityType.Relational().Schema,
                    Table = index.DeclaringEntityType.Relational().TableName,
                    Columns = index.Properties.Select(p => p.Relational().ColumnName).ToArray(),
                    Filter = index.Relational().Filter
                };
                operation.AddAnnotations(_migrationsAnnotations.For(index));

                Generate(operation, index.DeclaringEntityType.Model, builder, terminate: false);
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            }
        }

        private string IntegerConstant(long value)
            => string.Format(CultureInfo.InvariantCulture, "{0}", value);
    }
}
