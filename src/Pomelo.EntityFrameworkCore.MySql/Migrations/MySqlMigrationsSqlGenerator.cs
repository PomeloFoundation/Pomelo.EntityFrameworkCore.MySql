// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    public class MySqlMigrationsSqlGenerationHelper : MigrationsSqlGenerator
    {
        public MySqlMigrationsSqlGenerationHelper(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper SqlGenerationHelper,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] IRelationalAnnotationProvider annotations)
            : base(commandBuilderFactory, SqlGenerationHelper, typeMapper, annotations)
        {
        }

        protected override void Generate([NotNull] MigrationOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var createDatabaseOperation = operation as MySqlCreateDatabaseOperation;
            if (createDatabaseOperation != null)
            {
                Generate(createDatabaseOperation, model, builder);
                builder.EndCommand();
                return;
            }

            var dropDatabaseOperation = operation as MySqlDropDatabaseOperation;
            if (dropDatabaseOperation is MySqlDropDatabaseOperation)
            {
                Generate(dropDatabaseOperation, model, builder);
                builder.EndCommand();
                return;
            }

            base.Generate(operation, model, builder);
        }

        protected override void Generate(DropColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            var identifier = SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema);
            var alterBase = $"ALTER TABLE {identifier} DROP COLUMN {SqlGenerationHelper.DelimitIdentifier(operation.Name)}";
            builder.Append(alterBase).Append(SqlGenerationHelper.StatementTerminator);
        }

        protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            // TODO: There is probably duplication here with other methods. See ColumnDefinition.

            //TODO: this should provide feature parity with the EF6 provider, check if there's anything missing for EF7

            var type = operation.ColumnType;
            if (operation.ColumnType == null)
            {
                var property = FindProperty(model, operation.Schema, operation.Table, operation.Name);
                type = property != null
                    ? TypeMapper.GetMapping(property).StoreType
                    : TypeMapper.GetMapping(operation.ClrType).StoreType;
            }

            var serial = operation.FindAnnotation(MySqlAnnotationNames.Prefix + MySqlAnnotationNames.Serial);
            var isSerial = serial != null && (bool)serial.Value;

            var identifier = SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema);
            var alterBase = $"ALTER TABLE {identifier} MODIFY COLUMN {SqlGenerationHelper.DelimitIdentifier(operation.Name)}";

            // TYPE
            builder.Append(alterBase)
                .Append(" ")
                .Append(type)
                .Append(operation.IsNullable ? " NULL" : " NOT NULL")
                .AppendLine(SqlGenerationHelper.StatementTerminator);

            alterBase = $"ALTER TABLE {identifier} ALTER COLUMN {SqlGenerationHelper.DelimitIdentifier(operation.Name)}";

            builder.Append(alterBase);

            if (operation.DefaultValue != null)
            {
                builder.Append(" SET DEFAULT ")
                    .Append(SqlGenerationHelper.GenerateLiteral((dynamic)operation.DefaultValue))
                    .AppendLine(SqlGenerationHelper.BatchTerminator);
            }
            else if (!string.IsNullOrWhiteSpace(operation.DefaultValueSql))
            {
                builder.Append(" SET DEFAULT ")
                    .Append(operation.DefaultValueSql)
                    .AppendLine(SqlGenerationHelper.BatchTerminator);
            }
            else if (isSerial)
            {
                builder.Append(" SET DEFAULT ");

                switch (type)
                {
                    case "smallint":
                    case "int":
                    case "bigint":
                    case "real":
                    case "double precision":
                    case "numeric":
                        //TODO: need function CREATE SEQUENCE IF NOT EXISTS and set to it...
                        //Until this is resolved changing IsIdentity from false to true
                        //on types int2, int4 and int8 won't switch to type serial2, serial4 and serial8
                        throw new NotImplementedException("Not supporting creating sequence for integer types");
                    case "char(38)":
                    case "uuid":
                    case "uniqueidentifier":
                        builder.Append("UUID()");
                        break;
                    default:
                        throw new NotImplementedException($"Not supporting creating IsIdentity for {type}");

                }
            }
            else
            {
                builder.Append(" DROP DEFAULT ");
            }
        }

        protected override void Generate(CreateSequenceOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            throw new NotSupportedException("MySql doesn't support sequence operation.");
        }

        protected override void Generate(RenameIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.NewName != null)
            {
                Rename(operation.Schema, operation.Name, operation.NewName, "INDEX", builder);
            }
        }

        protected override void Generate(RenameSequenceOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            throw new NotSupportedException("MySql doesn't support sequence operation.");
        }

        protected override void Generate(RenameTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var separate = false;
            var name = operation.Name;
            if (operation.NewName != null)
            {
                Rename(operation.Schema, operation.Name, operation.NewName, "TABLE", builder);

                separate = true;
                name = operation.NewName;
            }

            if (operation.NewSchema != null)
            {
                if (separate)
                {
                    builder.AppendLine(SqlGenerationHelper.BatchTerminator);
                }

                Transfer(operation.NewSchema, operation.Schema, name, "TABLE", builder);
            }
        }

        protected override void Generate(
            [NotNull] CreateIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var method = (string)operation[MySqlAnnotationNames.Prefix + MySqlAnnotationNames.IndexMethod];

            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }

            builder
                .Append("INDEX ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name.LimitLength(64)))
                .Append(" ON ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema));

            if (method != null)
            {
                builder
                    .Append(" USING ")
                    .Append(method);
            }

            builder
                .Append(" (")
                .Append(ColumnList(operation.Columns))
                .Append(")");
        }

        protected override void Generate(EnsureSchemaOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .AppendLine($"CREATE OR REPLACE FUNCTION mysql_ef_ensure_schema() RETURNS VOID")
                .AppendLine($"BEGIN")
                .AppendLine($"    IF NOT EXISTS(SELECT 1 FROM `information_schema`.`SCHEMATA` where  `SCHEMA_NAME` = '{ operation.Name }')")
                .AppendLine($"    THEN")
                .AppendLine($"        CREATE SCHEMA { operation.Name };")
                .AppendLine($"    END IF;")
                .AppendLine($"END")
                .AppendLine(SqlGenerationHelper.BatchTerminator);
        }

        public virtual void Generate(MySqlCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE SCHEMA ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .AppendLine(SqlGenerationHelper.BatchTerminator);
        }

        public virtual void Generate(MySqlDropDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var dbName = SqlGenerationHelper.DelimitIdentifier(operation.Name);

            builder
                .Append("DROP DATABASE ")
                .Append(dbName);
        }

        protected override void Generate(DropIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP INDEX ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));
        }

        protected override void Generate(
            [NotNull] RenameColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append("ALTER TABLE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" RENAME COLUMN ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" TO ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.NewName));
        }

        protected override void ColumnDefinition([CanBeNull] string schema, [NotNull] string table, [NotNull] string name, [NotNull] Type clrType, [CanBeNull] string type, [CanBeNull] bool? unicode, [CanBeNull] int? maxLength, bool rowVersion, bool nullable, [CanBeNull] object defaultValue, [CanBeNull] string defaultValueSql, [CanBeNull] string computedColumnSql, [NotNull] IAnnotatable annotatable, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(annotatable, nameof(annotatable));
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(builder, nameof(builder));

            if (type == null)
            {
                var property = FindProperty(model, schema, table, name);
                type = TypeMapper.FindMapping(property).StoreType;
            }

            var valueGeneration = annotatable[MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAdd];
            var generatedOnAdd = valueGeneration != null && (bool)valueGeneration;
            if (generatedOnAdd)
            {
                switch (type)
                {
                    case "int":
                    case "int4":
                        type = "int AUTO_INCREMENT";
                        break;
                    case "bigint":
                    case "int8":
                        type = "bigint AUTO_INCREMENT";
                        break;
                    case "smallint":
                    case "int2":
                        type = "short AUTO_INCREMENT";
                        break;
                    case "varchar": // Allow string identifier
                        break;
                    case "char(38)": // GUID identifier
                        break;
                    default:
                        if (type.IndexOf("varchar") >= 0)
                            break;
                        throw new InvalidOperationException($"Column {name} of type {type} can't be Identity");
                }
            }

            if (defaultValue != null)
            {
                if (type == "longtext")
                {
                    defaultValue = null;
                }
            }

            base.ColumnDefinition(schema, table, name, clrType, type, unicode, maxLength, rowVersion, nullable, defaultValue, defaultValueSql, computedColumnSql, annotatable, model, builder);
        }

        protected override void DefaultValue(object defaultValue, string defaultValueSql, MigrationCommandListBuilder builder)
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
                builder
                    .Append(" DEFAULT ")
                    .Append(SqlGenerationHelper.GenerateLiteral(defaultValue));
            }
        }

        public virtual void Rename(
            [CanBeNull] string schema,
            [NotNull] string name,
            [NotNull] string newName,
            [NotNull] string type,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(newName, nameof(newName));
            Check.NotEmpty(type, nameof(type));
            Check.NotNull(builder, nameof(builder));


            builder
                .Append("ALTER ")
                .Append(type)
                .Append(SqlGenerationHelper.DelimitIdentifier(name, schema))
                .Append(" RENAME TO ")
                .Append(SqlGenerationHelper.DelimitIdentifier(newName));
        }

        public virtual void Transfer(
            [NotNull] string newSchema,
            [CanBeNull] string schema,
            [NotNull] string name,
            [NotNull] string type,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(newSchema, nameof(newSchema));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(type, nameof(type));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER ")
                .Append(type)
                .Append(" ")
                .Append(SqlGenerationHelper.DelimitIdentifier(name, schema))
                .Append(" SET SCHEMA ")
                .Append(SqlGenerationHelper.DelimitIdentifier(newSchema));
        }

        protected override void ForeignKeyAction(ReferentialAction referentialAction, MigrationCommandListBuilder builder)
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

        protected override void ForeignKeyConstraint(
            [NotNull] AddForeignKeyOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name.Substring(0, Math.Min(operation.Name.Length, 64))))
                    .Append(" ");
            }

            builder
                .Append("FOREIGN KEY (")
                .Append(ColumnList(operation.Columns))
                .Append(") REFERENCES ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.PrincipalTable, operation.PrincipalSchema));

            if (operation.PrincipalColumns != null)
            {
                builder
                    .Append(" (")
                    .Append(ColumnList(operation.PrincipalColumns))
                    .Append(")");
            }

            if (operation.OnUpdate != ReferentialAction.NoAction)
            {
                builder.Append(" ON UPDATE ");
                ForeignKeyAction(operation.OnUpdate, builder);
            }

            if (operation.OnDelete != ReferentialAction.NoAction)
            {
                builder.Append(" ON DELETE ");
                ForeignKeyAction(operation.OnDelete, builder);
            }
        }

        string ColumnList(string[] columns) => string.Join(", ", columns.Select(SqlGenerationHelper.DelimitIdentifier));
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source text.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        public static string LimitLength(this string source, int maxLength)
        {
            if (source.Length <= maxLength)
            {
                return source;
            }

            return source.Substring(0, maxLength);
        }
    }
}
