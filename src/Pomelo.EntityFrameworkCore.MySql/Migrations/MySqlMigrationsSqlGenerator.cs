// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
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
            EndStatement(builder);
        }

        protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

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
                        break;
                    default:
                        throw new NotImplementedException($"Not supporting creating IsIdentity for {type}");

                }
            }
            else
            {
                builder.Append(" DROP DEFAULT;");
            }

            EndStatement(builder);
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
                builder.Append("ALTER TABLE ")
                    .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table))
                    .Append(" RENAME INDEX ")
                    .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .Append(" TO ")
                    .Append(SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                    .AppendLine(";");

                EndStatement(builder);
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

            builder
                .Append("ALTER TABLE")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .Append(" RENAME ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.NewName));

            EndStatement(builder);
        }

        protected override void Generate([NotNull] CreateIndexOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder, bool terminate)
        {
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

            if (terminate)
            {
                builder.AppendLine(SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(
            [NotNull] CreateIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            Generate(operation, model, builder, true);
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
                .Append("ALTER TABLE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP INDEX ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .AppendLine(SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        protected override void Generate(
            [NotNull] RenameColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var property = FindProperty(model, model.MySql().DefaultSchema, operation.Table, operation.NewName);
            var type = TypeMapper.FindMapping(property).StoreType;

            builder.Append("ALTER TABLE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" CHANGE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                .Append(" ")
                .Append(type);

            if (!property.IsNullable)
                builder.Append(" NOT NULL");

            EndStatement(builder);
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

            var generatedOnAddAnnotation = annotatable[MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAdd];
            var generatedOnAdd = generatedOnAddAnnotation != null && (bool)generatedOnAddAnnotation;
            var generatedOnAddOrUpdateAnnotation = annotatable[MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAddOrUpdate];
            var generatedOnAddOrUpdate = generatedOnAddOrUpdateAnnotation != null && (bool)generatedOnAddOrUpdateAnnotation;
            if (generatedOnAdd && string.IsNullOrWhiteSpace(defaultValueSql) && defaultValue == null)
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
                    case "datetime":
                    case "timestamp":
                        defaultValueSql = "CURRENT_TIMESTAMP";
                        break;

                }
            }

            string onUpdateSql = null;

            if (generatedOnAddOrUpdate)
            {
                switch (type)
                {
                    case "datetime":
                    case "timestamp":
                        if (string.IsNullOrWhiteSpace(defaultValueSql) && defaultValue == null)
                            defaultValueSql = "CURRENT_TIMESTAMP";
                        onUpdateSql = "CURRENT_TIMESTAMP";
                        break;
                }
            }

            builder
                .Append(SqlGenerationHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(type ?? GetColumnType(schema, table, name, clrType, unicode, maxLength, rowVersion, model));

            if (!nullable)
            {
                builder.Append(" NOT NULL");
            }

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
            if (onUpdateSql != null)
            {
                builder
                    .Append(" ON UPDATE ")
                    .Append(onUpdateSql);
            }
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

        protected override void Generate([NotNull] DropForeignKeyOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP FOREIGN KEY ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .AppendLine(SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        protected override void Generate([NotNull] AddPrimaryKeyOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ADD ");
            PrimaryKeyConstraint(operation, model, builder);
            builder.AppendLine(SqlGenerationHelper.StatementTerminator);

            var annotations = model.GetAnnotations();
            if (operation.Columns.Count() == 1)
            {
                builder.Append(@"DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;
CREATE PROCEDURE POMELO_AFTER_ADD_PRIMARY_KEY(IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);

	SELECT COUNT(*) 
		INTO HAS_AUTO_INCREMENT_ID 
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT SCHEMA())
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` like '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT SCHEMA())
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` like '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT SCHEMA())
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` like '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT SCHEMA()), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END;");
                builder.AppendLine();

                builder.Append($"CALL POMELO_AFTER_ADD_PRIMARY_KEY('{ operation.Table }', '{ operation.Columns.First() }');");
                builder.AppendLine();
                builder.Append($"DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;");
                builder.AppendLine();
            }

            EndStatement(builder);
        }

        protected override void Generate([NotNull] DropPrimaryKeyOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            builder.Append(@"DROP PROCEDURE IF EXISTS POMELO_BEFORE_DROP_PRIMARY_KEY;
CREATE PROCEDURE POMELO_BEFORE_DROP_PRIMARY_KEY(IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);

	SELECT COUNT(*) 
		INTO HAS_AUTO_INCREMENT_ID 
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT SCHEMA())
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT SCHEMA())
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT SCHEMA())
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT SCHEMA()), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END;");
            builder.AppendLine();

            builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY('{ operation.Table }');");
            builder.AppendLine();
            builder.Append($"DROP PROCEDURE IF EXISTS POMELO_BEFORE_DROP_PRIMARY_KEY;");
            builder.AppendLine();

            builder
                .Append("ALTER TABLE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP PRIMARY KEY;")
                .AppendLine();

            EndStatement(builder);
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
