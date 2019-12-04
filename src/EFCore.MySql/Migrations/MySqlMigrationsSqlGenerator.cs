// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations
{
    /// <summary>
    ///     MySql-specific implementation of <see cref="MigrationsSqlGenerator" />.
    /// </summary>
    public class MySqlMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private static readonly Regex _typeRegex = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?",
            RegexOptions.IgnoreCase);

        private readonly IMigrationsAnnotationProvider _migrationsAnnotations;
        private readonly IMySqlOptions _options;
        private readonly RelationalTypeMapping _stringTypeMapping;

        protected virtual ServerVersion ServerVersion => _options.ServerVersion;

        public MySqlMigrationsSqlGenerator(
            [NotNull] MigrationsSqlGeneratorDependencies dependencies,
            [NotNull] IMigrationsAnnotationProvider migrationsAnnotations,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            _migrationsAnnotations = migrationsAnnotations;
            _options = options;
            _stringTypeMapping = dependencies.TypeMappingSource.GetMapping(typeof(string));
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
        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            CheckSchema(operation);

            switch (operation)
            {
                case MySqlCreateDatabaseOperation createDatabaseOperation:
                    Generate(createDatabaseOperation, model, builder);
                    break;
                case MySqlDropDatabaseOperation dropDatabaseOperation:
                    Generate(dropDatabaseOperation, model, builder);
                    break;
                default:
                    base.Generate(operation, model, builder);
                    break;
            }
        }

        protected virtual void CheckSchema(MigrationOperation operation)
        {
            var schema = operation.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(p => p.Name.IndexOf(nameof(AddForeignKeyOperation.Schema), StringComparison.Ordinal) >= 0)
                .Select(p => p.GetValue(operation) as string)
                .FirstOrDefault(schemaValue => schemaValue != null);

            if (schema != null)
            {
                var name = operation.GetType()
                    .GetProperty(nameof(AddForeignKeyOperation.Name), BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                    ?.GetValue(operation) as string;

                throw new InvalidOperationException($"A schema \"{schema}\" has been set for an object of type \"{operation.GetType().Name}\"{(string.IsNullOrEmpty(name) ? string.Empty : $" with the name of \"{name}\"")}. MySQL does not support the EF Core concept of schemas. Any schema property of any \"MigrationOperation\" must be null.");
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

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" MODIFY COLUMN ");

            ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                operation,
                model,
                builder);

            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
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
                        .FirstOrDefault(i => i.GetName() == operation.NewName);
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
                        Columns = index.Properties.Select(p => p.GetColumnName()).ToArray(),
                        IsUnique = index.IsUnique,
                        Filter = index.GetFilter()
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
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName, operation.NewSchema))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateIndexOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(
            CreateIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            operation.Filter = null;
            operation.Name = Truncate(operation.Name, 64);
            base.Generate(operation, model, builder, terminate);
        }

        /// /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(EnsureSchemaOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(DropSchemaOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
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
            [NotNull] DropIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
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
        ///     Builds commands for the given <see cref="DropUniqueConstraintOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            DropUniqueConstraintOperation operation, IModel model, MigrationCommandListBuilder builder)
            => Generate(operation, model, builder, terminate: true);

        /// <summary>
        ///     Builds commands for the given <see cref="DropUniqueConstraintOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected virtual void Generate(
            DropUniqueConstraintOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP KEY ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

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
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema));

            if (_options.ServerVersion.SupportsRenameColumn)
            {
                builder.Append(" RENAME COLUMN ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .Append(" TO ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
                return;
            }

            builder.Append(" CHANGE ")
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
                    .Append(type)
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
                return;
            }

            var typeMapping = Dependencies.TypeMappingSource.GetMapping(property);
            var converter = typeMapping.Converter;
            var clrType = (converter?.ProviderClrType ?? typeMapping.ClrType).UnwrapNullableType();
            var columnType = (string)(operation[RelationalAnnotationNames.ColumnType]
                                      ?? property[RelationalAnnotationNames.ColumnType]);
            var isNullable = property.IsColumnNullable();

            var defaultValue = property.GetDefaultValue();
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

            var isRowVersion = (property.ClrType == typeof(DateTime) || property.ClrType == typeof(byte[]))
                               && property.IsConcurrencyToken
                               && property.ValueGenerated == ValueGenerated.OnAddOrUpdate;

            var addColumnOperation = new AddColumnOperation
            {
                Schema = operation.Schema,
                Table = operation.Table,
                Name = operation.NewName,
                ClrType = clrType,
                ColumnType = columnType,
                IsUnicode = property.IsUnicode(),
                MaxLength = property.GetMaxLength(),
                IsFixedLength = property.IsFixedLength(),
                IsRowVersion = isRowVersion,
                IsNullable = isNullable,
                DefaultValue = defaultValue,
                DefaultValueSql = property.GetDefaultValueSql(),
                ComputedColumnSql = property.GetComputedColumnSql(),
            };

            ColumnDefinition(
                addColumnOperation,
                model,
                builder);
            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            EndStatement(builder);
        }

        /// <summary>
        ///     Generates a SQL fragment configuring a sequence with the given options.
        /// </summary>
        /// <param name="schema"> The schema that contains the sequence, or <c>null</c> to use the default schema. </param>
        /// <param name="name"> The sequence name. </param>
        /// <param name="operation"> The sequence options. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void SequenceOptions(
            string schema,
            string name,
            SequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append(" INCREMENT BY ")
                .Append(IntegerConstant(operation.IncrementBy));

            if (operation.MinValue.HasValue)
            {
                builder
                    .Append(" MINVALUE ")
                    .Append(IntegerConstant(operation.MinValue.Value));
            }
            else
            {
                builder.Append(" NO MINVALUE");
            }

            if (operation.MaxValue.HasValue)
            {
                builder
                    .Append(" MAXVALUE ")
                    .Append(IntegerConstant(operation.MaxValue.Value));
            }
            else
            {
                builder.Append(" NO MAXVALUE");
            }

            builder.Append(operation.IsCyclic ? " CYCLE" : " NO CYCLE");
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
                operation,
                model,
                builder);

        /// <summary>
        ///     Generates a SQL fragment for a column definition for the given column metadata.
        /// </summary>
        /// <param name="schema"> The schema that contains the table, or <c>null</c> to use the default schema. </param>
        /// <param name="table"> The table that contains the column. </param>
        /// <param name="name"> The column name. </param>
        /// <param name="operation"> The column metadata. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ColumnDefinition(
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string name,
            [NotNull] ColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var matchType = operation.ColumnType ?? GetColumnType(schema, table, name, operation, model);
            var matchLen = "";
            var match = _typeRegex.Match(matchType ?? "-");
            if (match.Success)
            {
                matchType = match.Groups[1].Value.ToLower();
                if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                {
                    matchLen = match.Groups[2].Value;
                }
            }

            var valueGenerationStrategy = MySqlValueGenerationStrategyCompatibility.GetValueGenerationStrategy(operation.GetAnnotations().OfType<IAnnotation>().ToArray());

            var autoIncrement = false;
            if (valueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn &&
                string.IsNullOrWhiteSpace(operation.DefaultValueSql) && operation.DefaultValue == null)
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
                        if (!_options.ServerVersion.SupportsDateTimeCurrentTimestamp)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                $"on Add or Update in versions lower than {ServerVersion.GetSupport(ServerVersion.DateTimeCurrentTimestampSupportKey)}. Try explicitly setting the column type to TIMESTAMP.");
                        }

                        goto case "timestamp";
                    case "timestamp":
                        operation.DefaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
                        break;
                }
            }

            string onUpdateSql = null;
            if (operation.IsRowVersion || valueGenerationStrategy == MySqlValueGenerationStrategy.ComputedColumn)
            {
                switch (matchType)
                {
                    case "datetime":
                        if (!_options.ServerVersion.SupportsDateTimeCurrentTimestamp)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                $"on Add or Update in versions lower than {ServerVersion.GetSupport(ServerVersion.DateTimeCurrentTimestampSupportKey)}. Try explicitly setting the column type to TIMESTAMP.");
                        }

                        goto case "timestamp";
                    case "timestamp":
                        if (string.IsNullOrWhiteSpace(operation.DefaultValueSql) && operation.DefaultValue == null)
                        {
                            operation.DefaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
                        }

                        onUpdateSql = $"CURRENT_TIMESTAMP({matchLen})";
                        break;
                }
            }

            if (operation.ComputedColumnSql == null)
            {
                ColumnDefinitionWithCharSet(schema, table, name, operation, model, builder);

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
                }
            }
            else
            {
                builder
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                    .Append(" ")
                    .Append(operation.ColumnType ?? GetColumnType(schema, table, name, operation, model));
                builder
                    .Append(" AS ")
                    .Append($"({operation.ComputedColumnSql})");

                if (operation.IsNullable && _options.ServerVersion.SupportsNullableGeneratedColumns)
                {
                    builder.Append(" NULL");
                }
            }
        }

        private void ColumnDefinitionWithCharSet(string schema, string table, string name, ColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation.ComputedColumnSql != null)
            {
                ComputedColumnDefinition(schema, table, name, operation, model, builder);
                return;
            }

            var columnType = operation.ColumnType != null
                ? GetColumnTypeWithCharSetAndCollation(operation, operation.ColumnType)
                : GetColumnType(schema, table, name, operation, model);

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(columnType);

            builder.Append(operation.IsNullable ? " NULL" : " NOT NULL");

            DefaultValue(operation.DefaultValue, operation.DefaultValueSql, columnType, builder);
        }

        protected override string GetColumnType(string schema, string table, string name, ColumnOperation operation, IModel model)
            => GetColumnTypeWithCharSetAndCollation(
                operation,
                base.GetColumnType(schema, table, name, operation, model));

        private static string GetColumnTypeWithCharSetAndCollation(ColumnOperation operation, string columnType)
        {
            var charSet = operation[MySqlAnnotationNames.CharSet];
            if (charSet != null)
            {
                const string characterSetClausePattern = @"CHARACTER SET \w+";
                var characterSetClause = $@"CHARACTER SET {charSet}";

                columnType = Regex.IsMatch(columnType, characterSetClausePattern, RegexOptions.IgnoreCase)
                    ? Regex.Replace(columnType, characterSetClausePattern, characterSetClause)
                    : columnType.TrimEnd() + " " + characterSetClause;
            }

            var collation = operation[MySqlAnnotationNames.Collation];
            if (collation != null)
            {
                const string collationClausePattern = @"COLLATE \w+";
                var collationClause = $@"COLLATE {collation}";

                columnType = Regex.IsMatch(columnType, collationClausePattern, RegexOptions.IgnoreCase)
                    ? Regex.Replace(columnType, collationClausePattern, collationClause)
                    : columnType.TrimEnd() + " " + collationClause;
            }

            return columnType;
        }

        /// <summary>
        ///     Generates a SQL fragment for the default constraint of a column.
        /// </summary>
        /// <param name="defaultValue"> The default value for the column. </param>
        /// <param name="defaultValueSql"> The SQL expression to use for the column's default constraint. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>

        protected override void DefaultValue(object defaultValue, string defaultValueSql, string columnType, MigrationCommandListBuilder builder)
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

        /// <summary>
        ///     Generates a SQL fragment for the primary key constraint of a <see cref="CreateTableOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void CreateTablePrimaryKeyConstraint(
            [NotNull] CreateTableOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var primaryKey = operation.PrimaryKey;
            if (primaryKey != null)
            {
                builder.AppendLine(",");

                // MySQL InnoDB has the requirement, that an AUTO_INCREMENT column has to be the first
                // column participating in an index.

                var sortedColumnNames = primaryKey.Columns.Length > 1
                    ? primaryKey.Columns
                        .Select(columnName => operation.Columns.First(co => co.Name == columnName))
                        .OrderBy(co => co[MySqlAnnotationNames.ValueGenerationStrategy] is MySqlValueGenerationStrategy generationStrategy
                                       && generationStrategy == MySqlValueGenerationStrategy.IdentityColumn
                            ? 0
                            : 1)
                        .Select(co => co.Name)
                        .ToArray()
                    : primaryKey.Columns;

                var sortedPrimaryKey = new AddPrimaryKeyOperation()
                {
                    Schema = primaryKey.Schema,
                    Table = primaryKey.Table,
                    Name = primaryKey.Name,
                    Columns = sortedColumnNames,
                    IsDestructiveChange = primaryKey.IsDestructiveChange,
                };

                foreach (var annotation in primaryKey.GetAnnotations())
                {
                    sortedPrimaryKey[annotation.Name] = annotation.Value;
                }

                PrimaryKeyConstraint(
                    sortedPrimaryKey,
                    model,
                    builder);
            }
        }

        protected override void Generate(AddPrimaryKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
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
                builder.Append(
                    $"CALL POMELO_AFTER_ADD_PRIMARY_KEY({_stringTypeMapping.GenerateSqlLiteral(operation.Schema)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Table)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Columns.First())});");

                builder.AppendLine();
            }

            EndStatement(builder);
        }

        protected override void Generate(DropPrimaryKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY({_stringTypeMapping.GenerateSqlLiteral(operation.Schema)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Table)});");

            builder.AppendLine();
            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP PRIMARY KEY;")
                .AppendLine();

            EndStatement(builder);
        }

        /// <summary>
        ///     Generates a SQL fragment for a foreign key constraint of an <see cref="AddForeignKeyOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ForeignKeyConstraint(
            AddForeignKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Name = Truncate(operation.Name, 64);
            base.ForeignKeyConstraint(operation, model, builder);
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
                builder.Append("RESTRICT");
            }
            else
            {
                base.ForeignKeyAction(referentialAction, builder);
            }
        }

        private string IntegerConstant(long value)
            => string.Format(CultureInfo.InvariantCulture, "{0}", value);

        private static string Truncate(string source, int maxLength)
        {
            if (source == null
                || source.Length <= maxLength)
            {
                return source;
            }

            return source.Substring(0, maxLength);
        }
    }
}
