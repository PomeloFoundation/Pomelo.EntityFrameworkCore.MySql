// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using EFCore.MySql.Infrastructure.Internal;
using EFCore.MySql.Metadata.Internal;
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
    public class MySqlMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private static readonly Regex _typeRe = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?", RegexOptions.IgnoreCase);
        private readonly IMySqlOptions _options;
        private readonly IMigrationsAnnotationProvider _migrationsAnnotationProvider;

        public MySqlMigrationsSqlGenerator(
            [NotNull] MigrationsSqlGeneratorDependencies dependencies,
            [NotNull] IMySqlOptions options,
            [NotNull] IMigrationsAnnotationProvider migrationsAnnotationProvider)
            : base(dependencies)
        {
            _options = options;
            _migrationsAnnotationProvider = migrationsAnnotationProvider;
        }

        protected override void Generate([NotNull] MigrationOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation is MySqlCreateDatabaseOperation createDatabaseOperation)
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
            var identifier = Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema);
            var alterBase = $"ALTER TABLE {identifier} DROP COLUMN {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name)}";
            builder.Append(alterBase).Append(Dependencies.SqlGenerationHelper.StatementTerminator);
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
                    ? Dependencies.TypeMappingSource.GetMapping(property).StoreType
                    : Dependencies.TypeMappingSource.GetMapping(operation.ClrType).StoreType;
            }

            var identifier = Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema);
            var alterBase = $"ALTER TABLE {identifier} MODIFY COLUMN {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name)}";

            // TYPE
            builder.Append(alterBase)
                .Append(" ")
                .Append(type)
                .Append(operation.IsNullable ? " NULL" : " NOT NULL")
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            switch (type)
            {
                case "tinyblob":
                case "blob":
                case "mediumblob":
                case "longblob":

                case "tinytext":
                case "text":
                case "mediumtext":
                case "longtext":

                case "geometry":
                case "point":
                case "linestring":
                case "polygon":
                case "multipoint":
                case "multilinestring":
                case "multipolygon":
                case "geometrycollection":

                case "json":
                    if (operation.DefaultValue != null || !string.IsNullOrWhiteSpace(operation.DefaultValueSql))
                    {
                        throw new NotSupportedException($"{type} column can't have a default value");
                    }
                    break;
                default:
                    alterBase = $"ALTER TABLE {identifier} ALTER COLUMN {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name)}";

                    builder.Append(alterBase);

                    if (operation.DefaultValue != null)
                    {
                        var typeMapping = Dependencies.TypeMappingSource.GetMappingForValue(operation.DefaultValue);
                        builder.Append(" SET DEFAULT ")
                            .Append(typeMapping.GenerateSqlLiteral(operation.DefaultValue))
                            .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
                    }
                    else if (!string.IsNullOrWhiteSpace(operation.DefaultValueSql))
                    {
                        builder.Append(" SET DEFAULT ")
                            .Append(operation.DefaultValueSql)
                            .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
                    }
                    else
                    {
                        builder.Append(" DROP DEFAULT;");
                    }
                    break;
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
                    var index = FindEntityTypes(model, operation.Schema, operation.Table)?.SelectMany(e => e.GetDeclaredIndexes())
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
                    createIndexOperation.AddAnnotations(_migrationsAnnotationProvider.For(index));
                    createIndexOperation.AddAnnotations(operation.GetAnnotations());

                    Generate(createIndexOperation, model, builder);
                }
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
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .Append(" RENAME ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName, operation.NewSchema));

            EndStatement(builder);
        }

        protected override void Generate(CreateIndexOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate)
        {
            var method = (string)operation[MySqlAnnotationNames.Prefix];
            var isFullText = !string.IsNullOrEmpty((string)operation[MySqlAnnotationNames.FullTextIndex]);
            var isSpatial = !string.IsNullOrEmpty((string)operation[MySqlAnnotationNames.SpatialIndex]);

            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }
            else if (isFullText)
            {
                builder.Append("FULLTEXT ");
            }
            else if (isSpatial)
            {
                builder.Append("SPATIAL ");
            }

            builder
                .Append("INDEX ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema));

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
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(CreateIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
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
                .Append("CREATE DATABASE IF NOT EXISTS ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(Dependencies.SqlGenerationHelper.StatementTerminator)
                .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
            EndStatement(builder);
        }

        public virtual void Generate(MySqlCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(Dependencies.SqlGenerationHelper.StatementTerminator)
                .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
            EndStatement(builder);
        }

        public virtual void Generate(MySqlDropDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
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

        protected override void Generate(DropIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP INDEX ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            EndStatement(builder);
        }

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
            if (type == null)
            {
                type = GetColumnType(schema, table, name, clrType, unicode, maxLength, fixedLength, rowVersion, model);
            }

            var property = FindProperty(model, schema, table, name);

            var matchType = type;
            var matchLen = "";
            var match = _typeRe.Match(type ?? "-");
            if (match.Success)
            {
                matchType = match.Groups[1].Value.ToLower();
                if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                {
                    matchLen = match.Groups[2].Value;
                }
            }

            var autoIncrement = false;
            var valueGenerationStrategy = annotatable[MySqlAnnotationNames.ValueGenerationStrategy] as MySqlValueGenerationStrategy?;
            if (!valueGenerationStrategy.HasValue)
            {
                if (property?.ValueGenerated == ValueGenerated.OnAdd)
                {
                    valueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;
                }
                else if (property?.ValueGenerated == ValueGenerated.OnAddOrUpdate)
                {
                    valueGenerationStrategy = MySqlValueGenerationStrategy.ComputedColumn;
                }
                else
                {
                    // check 1.x Value Generation Annotations
                    var generatedOnAddAnnotation = annotatable[MySqlAnnotationNames.LegacyValueGeneratedOnAdd];
                    if (generatedOnAddAnnotation != null && (bool)generatedOnAddAnnotation)
                        valueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;
                    var generatedOnAddOrUpdateAnnotation = annotatable[MySqlAnnotationNames.LegacyValueGeneratedOnAddOrUpdate];
                    if (generatedOnAddOrUpdateAnnotation != null && (bool)generatedOnAddOrUpdateAnnotation)
                        valueGenerationStrategy = MySqlValueGenerationStrategy.ComputedColumn;
                }
            }

            if ((valueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn) && string.IsNullOrWhiteSpace(defaultValueSql) && defaultValue == null)
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
            if (valueGenerationStrategy == MySqlValueGenerationStrategy.ComputedColumn)
            {
                switch (matchType)
                {
                    case "datetime":
                        if (!_options.ServerVersion.SupportsDateTime6)
                        {
                            throw new InvalidOperationException($"Error in {table}.{name}: DATETIME does not support values generated " +
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

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(type ?? GetColumnType(schema, table, name, clrType, unicode, maxLength, rowVersion, model));

            if (!nullable)
            {
                builder.Append(" NOT NULL");
            }

            if (autoIncrement)
            {
                builder.Append(" AUTO_INCREMENT");
            }
            else
            {
                if (defaultValueSql != null)
                {
                    builder
                        .Append(" DEFAULT ")
                        .Append(defaultValueSql);
                }
                else if (defaultValue != null)
                {
                    var defaultValueLiteral = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue);

                    builder
                        .Append(" DEFAULT ")
                        .Append(defaultValueLiteral.GenerateSqlLiteral(defaultValue));
                }
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
                var typeMapping = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue);

                builder
                    .Append(" DEFAULT ")
                    .Append(typeMapping.GenerateSqlLiteral(defaultValue));
            }
        }

        protected override void Generate([NotNull] DropForeignKeyOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP FOREIGN KEY ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        protected override void Generate([NotNull] AddPrimaryKeyOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ADD ");
            PrimaryKeyConstraint(operation, model, builder);
            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            var annotations = model.GetAnnotations();
            if (operation.Columns.Count() == 1)
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
                    builder.Append($"CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, '{ operation.Table }', '{ operation.Columns.First() }');");
                }
                else
                {
                    builder.Append($"CALL POMELO_AFTER_ADD_PRIMARY_KEY('{ operation.Schema }', '{ operation.Table }', '{ operation.Columns.First() }');");
                }
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
                builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, '{ operation.Table }');");
            }
            else
            {
                builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY('{ operation.Schema }', '{ operation.Table }');");
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
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name, schema))
                .Append(" RENAME TO ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(newName, schema));
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
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name, schema))
                .Append(" SET SCHEMA ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(newSchema));
        }

        protected override void ForeignKeyAction(ReferentialAction referentialAction, MigrationCommandListBuilder builder)
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
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name.Substring(0, Math.Min(operation.Name.Length, 64))))
                    .Append(" ");
            }

            builder
                .Append("FOREIGN KEY (")
                .Append(ColumnList(operation.Columns))
                .Append(") REFERENCES ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.PrincipalTable, operation.PrincipalSchema));

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

        protected override string ColumnList(string[] columns) => string.Join(", ", columns.Select(Dependencies.SqlGenerationHelper.DelimitIdentifier));
    }
}
