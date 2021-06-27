// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations
{
    // CHECK: Can we increase the usage of the new model over the old one, or are we done here?
    /// <summary>
    ///     MySql-specific implementation of <see cref="MigrationsSqlGenerator" />.
    /// </summary>
    public class MySqlMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private static readonly Regex _typeRegex = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?",
            RegexOptions.IgnoreCase);

        private static readonly HashSet<string> _spatialStoreTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "geometry",
            "point",
            "curve",
            "linestring",
            "line",
            "linearring",
            "surface",
            "polygon",
            "geometrycollection",
            "multipoint",
            "multicurve",
            "multilinestring",
            "multisurface",
            "multipolygon",
        };

        private static readonly Dictionary<Type, (string beginText, string endText)> _customMigrationCommands = new Dictionary<Type, (string beginText, string endText)>
        {
            { typeof(DropPrimaryKeyOperation), (beginText: BeforeDropPrimaryKeyMigrationBegin, endText: BeforeDropPrimaryKeyMigrationEnd) },
            { typeof(AddPrimaryKeyOperation), (beginText: AfterAddPrimaryKeyMigrationBegin, endText: AfterAddPrimaryKeyMigrationEnd) },
        };

        #region Custom SQL

        private const string BeforeDropPrimaryKeyMigrationBegin = @"DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
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
END //
DELIMITER ;";
        private const string BeforeDropPrimaryKeyMigrationEnd = @"DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;";

        private const string AfterAddPrimaryKeyMigrationBegin = @"DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
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
END //
DELIMITER ;";
        private const string AfterAddPrimaryKeyMigrationEnd = @"DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;";

        #endregion

        private readonly IRelationalAnnotationProvider _annotationProvider;
        private readonly IMySqlOptions _options;
        private readonly RelationalTypeMapping _stringTypeMapping;

        protected virtual ServerVersion ServerVersion => _options.ServerVersion;

        public MySqlMigrationsSqlGenerator(
            [NotNull] MigrationsSqlGeneratorDependencies dependencies,
            [NotNull] IRelationalAnnotationProvider annotationProvider,
            [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            _annotationProvider = annotationProvider;
            _options = options;
            _stringTypeMapping = dependencies.TypeMappingSource.GetMapping(typeof(string));
        }

        public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
        {
            var commands = base.Generate(operations, model, options);

            // Some operations depend on custom stored procedures. If those operations are used, the stored procedures are added before any
            // actual migration operations and removed after all actual migration operations.
            return WrapWithCustomCommands(
                operations,
                commands.ToList(),
                options);
        }

        private IReadOnlyList<MigrationCommand> WrapWithCustomCommands(
            IReadOnlyList<MigrationOperation> migrationOperations,
            List<MigrationCommand> migrationCommands,
            MigrationsSqlGenerationOptions options)
        {
            var commandTexts = GetMigrationCommandTexts(migrationOperations, true, options);
            if (commandTexts.Length > 0)
            {
                var customCommands = new MigrationCommandListBuilder(Dependencies);

                foreach (var commandText in commandTexts)
                {
                    customCommands
                        .Append(commandText)
                        .EndCommand();
                }

                migrationCommands.InsertRange(0, customCommands.GetCommandList());
            }

            commandTexts = GetMigrationCommandTexts(migrationOperations, false, options);
            if (commandTexts.Length > 0)
            {
                var customCommands = new MigrationCommandListBuilder(Dependencies);

                foreach (var commandText in commandTexts)
                {
                    customCommands
                        .Append(commandText)
                        .EndCommand();
                }

                migrationCommands.AddRange(customCommands.GetCommandList());
            }

            return migrationCommands;
        }

        private string[] GetMigrationCommandTexts(
            IReadOnlyList<MigrationOperation> migrationOperations,
            bool beginTexts,
            MigrationsSqlGenerationOptions options)
            => GetCustomCommands(migrationOperations)
                .Select(t => PrepareString(beginTexts ? t.beginText : t.endText, options))
                .ToArray();

        private static IReadOnlyList<(string beginText, string endText)> GetCustomCommands(IReadOnlyList<MigrationOperation> migrationOperations)
            => _customMigrationCommands
                .Where(c => migrationOperations.Any(o => c.Key.IsInstanceOfType(o)))
                .Select(kvp => kvp.Value)
                .ToList();

        private string PrepareString(string str, MigrationsSqlGenerationOptions options)
        {
            str = options.HasFlag(MigrationsSqlGenerationOptions.Script)
                ? str
                : CleanUpScriptSpecificPseudoStatements(str);

            str = str
                .Replace("\r", string.Empty)
                .Replace("\n", Environment.NewLine);

            str += options.HasFlag(MigrationsSqlGenerationOptions.Script)
                ? Environment.NewLine + (
                    options.HasFlag(MigrationsSqlGenerationOptions.Idempotent)
                        ? Environment.NewLine
                        : string.Empty)
                : string.Empty;

            return str;
        }

        private static string CleanUpScriptSpecificPseudoStatements(string commandText)
        {
            const string delimiterPattern = @"^\s*DELIMITER\s*(?<Delimiter>;|//)\s*$";
            const RegexOptions delimiterPatternRegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;

            var delimiter = Regex.Match(commandText, delimiterPattern, delimiterPatternRegexOptions);

            if (delimiter.Success)
            {
                var result = Regex.Replace(commandText, delimiterPattern, string.Empty, delimiterPatternRegexOptions);
                return Regex.Replace(result, $@"\s*{Regex.Escape(delimiter.Groups["Delimiter"].Value)}\s*$", ";", delimiterPatternRegexOptions);
            }

            return commandText;
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
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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
                case MySqlDropPrimaryKeyAndRecreateForeignKeysOperation dropPrimaryKeyAndRecreateForeignKeysOperation:
                    Generate(dropPrimaryKeyAndRecreateForeignKeysOperation, model, builder);
                    break;
                case MySqlDropUniqueConstraintAndRecreateForeignKeysOperation dropUniqueConstraintAndRecreateForeignKeysOperation:
                    Generate(dropUniqueConstraintAndRecreateForeignKeysOperation, model, builder);
                    break;
                default:
                    base.Generate(operation, model, builder);
                    break;
            }
        }

        protected virtual void CheckSchema(MigrationOperation operation)
        {
            if (_options.SchemaNameTranslator != null)
            {
                return;
            }

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

                throw new InvalidOperationException($"A schema \"{schema}\" has been set for an object of type \"{operation.GetType().Name}\"{(string.IsNullOrEmpty(name) ? string.Empty : $" with the name of \"{name}\"")}. MySQL does not support the EF Core concept of schemas. Any schema property of any \"MigrationOperation\" must be null. This behavior can be changed by setting the `SchemaBehavior` option in the `UseMySql` call.");
            }
        }

        protected override void Generate(
            [NotNull] CreateTableOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            base.Generate(operation, model, builder, false);

            var tableOptions = new List<(string, string)>();

            if (operation[MySqlAnnotationNames.CharSet] is string charSet)
            {
                tableOptions.Add(("CHARACTER SET", charSet));
            }

            if (operation[RelationalAnnotationNames.Collation] is string collation)
            {
                tableOptions.Add(("COLLATE", collation));
            }

            if (operation.Comment != null)
            {
                tableOptions.Add(("COMMENT", _stringTypeMapping.GenerateSqlLiteral(operation.Comment)));
            }

            tableOptions.AddRange(
                MySqlEntityTypeExtensions.DeserializeTableOptions(operation[MySqlAnnotationNames.StoreOptions] as string)
                    .Select(kvp => (kvp.Key, kvp.Value)));

            foreach (var (key, value) in tableOptions)
            {
                builder
                    .Append(" ")
                    .Append(key)
                    .Append("=")
                    .Append(value);
            }

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            var oldCharSet = operation.OldTable[MySqlAnnotationNames.CharSet] as string;
            var newCharSet = operation[MySqlAnnotationNames.CharSet] as string;

            var oldCollation = operation.OldTable[RelationalAnnotationNames.Collation] as string;
            var newCollation = operation[RelationalAnnotationNames.Collation] as string;

            // Collations are more specific than charsets. So if a collation has been set, we use the collation instead of the charset.
            if (newCollation != oldCollation &&
                newCollation != null)
            {
                // A new collation has been set. It takes precedence over any defined charset.
                builder
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                    .Append(" COLLATE=")
                    .Append(newCollation)
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
            }
            else if (newCharSet != oldCharSet ||
                     newCollation != oldCollation && newCollation == null)
            {
                // The charset has been changed or the collation has been reset to the default.
                if (newCharSet != null)
                {
                    // A new charset has been set without an explicit collation.
                    builder
                        .Append("ALTER TABLE ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                        .Append(" CHARACTER SET=")
                        .Append(newCharSet)
                        .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                    EndStatement(builder);
                }
                else
                {
                    // The charset (and any collation) has been reset to the default.
                    var resetCharSetSql = $@"set @__pomelo_TableCharset = (
    SELECT `ccsa`.`CHARACTER_SET_NAME` as `TABLE_CHARACTER_SET`
    FROM `INFORMATION_SCHEMA`.`TABLES` as `t`
    LEFT JOIN `INFORMATION_SCHEMA`.`COLLATION_CHARACTER_SET_APPLICABILITY` as `ccsa` ON `ccsa`.`COLLATION_NAME` = `t`.`TABLE_COLLATION`
    WHERE `TABLE_SCHEMA` = SCHEMA() AND `TABLE_NAME` = {_stringTypeMapping.GenerateSqlLiteral(operation.Name)} AND `TABLE_TYPE` IN ('BASE TABLE', 'VIEW'));

SET @__pomelo_SqlExpr = CONCAT('ALTER TABLE {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name)} CHARACTER SET = ', @__pomelo_TableCharset, ';');
PREPARE __pomelo_SqlExprExecute FROM @__pomelo_SqlExpr;
EXECUTE __pomelo_SqlExprExecute;
DEALLOCATE PREPARE __pomelo_SqlExprExecute;";

                    builder.AppendLine(resetCharSetSql);
                    EndStatement(builder);
                }
            }

            if (operation.Comment != operation.OldTable.Comment)
            {
                builder.Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));

                // An existing comment will be removed, when set to an empty string.
                GenerateComment(operation.Comment ?? string.Empty, builder);

                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }

            var oldTableOptions = MySqlEntityTypeExtensions.DeserializeTableOptions(operation.OldTable[MySqlAnnotationNames.StoreOptions] as string);
            var newTableOptions = MySqlEntityTypeExtensions.DeserializeTableOptions(operation[MySqlAnnotationNames.StoreOptions] as string);
            var addedOrChangedTableOptions = newTableOptions.Except(oldTableOptions).ToArray();

            if (addedOrChangedTableOptions.Length > 0)
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));

                foreach (var (key, value) in addedOrChangedTableOptions)
                {
                    builder
                        .Append(" ")
                        .Append(key)
                        .Append("=")
                        .Append(value);
                }

                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="AlterColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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
                if (_options.ServerVersion.Supports.RenameIndex)
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
                    var index = model?
                        .GetRelationalModel()
                        .FindTable(operation.Table, operation.Schema)
                        ?.Indexes
                        .FirstOrDefault(i => i.Name == operation.NewName);

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
                        Columns = index.Columns.Select(c => c.Name).ToArray(),
                        IsUnique = index.IsUnique,
                        Filter = index.Filter,
                    };
                    createIndexOperation.AddAnnotations(_annotationProvider.For(index, true)); // CHECK: necessary?
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
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RestartSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            if (!_options.ServerVersion.Supports.Sequences)
            {
                throw new InvalidOperationException(
                    $"Cannot restart sequence '{operation.Name}' because sequences are not supported in server version {_options.ServerVersion}.");
            }
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
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(
            CreateIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (!_options.ServerVersion.Supports.SpatialIndexes &&
                operation[MySqlAnnotationNames.SpatialIndex] is true)
            {
                Dependencies.MigrationsLogger.Logger.LogWarning(
                    $"Spatial indexes are not supported on {_options.ServerVersion}. The CREATE INDEX operation will be ignored.");
                return;
            }

            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }

            IndexTraits(operation, model, builder);

            builder
                .Append("INDEX ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(Truncate(operation.Name, 64)))
                .Append(" ON ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" (")
                .Append(ColumnListWithIndexPrefixLength(operation, operation.Columns))
                .Append(")");

            IndexOptions(operation, model, builder);

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(EnsureSchemaOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(DropSchemaOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            [NotNull] CreateSequenceOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            if (!_options.ServerVersion.Supports.Sequences)
            {
                throw new InvalidOperationException(
                    $"Cannot create sequence '{operation.Name}' because sequences are not supported in server version {_options.ServerVersion}.");
            }

            // "CREATE SEQUENCE"  supported only in MariaDb from 10.3.
            // However, "CREATE SEQUENCE name AS type" expression is currently not supported.
            // The base MigrationsSqlGenerator.Generate method generates that expression.
            // Also, when creating a sequence current version of MariaDb doesn't tolerate "NO MINVALUE"
            // when specifying "STARTS WITH" so, StartValue mus be set accordingly.
            // https://github.com/aspnet/EntityFrameworkCore/blob/master/src/EFCore.Relational/Migrations/MigrationsSqlGenerator.cs#L535-L543
            var oldValue = operation.ClrType;
            operation.ClrType = typeof(long);
            if (operation.StartValue <= 0 )
            {
                operation.MinValue = operation.StartValue;
            }
            base.Generate(operation, model, builder);
            operation.ClrType = oldValue;
        }

        /// <summary>
        ///     Builds commands for the given <see cref="MySqlCreateDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

            if (operation.CharSet != null)
            {
                builder
                    .Append(" CHARACTER SET ")
                    .Append(operation.CharSet);
            }

            if (operation.Collation != null)
            {
                builder
                    .Append(" COLLATE ")
                    .Append(operation.Collation);
            }

            builder
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        /// <summary>
        ///     Builds commands for the given <see cref="MySqlDropDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

        protected override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            // Also at this point, all explicitly added `Relational:Collation` annotations (through delegation) should have been set to the
            // `Collation` property and removed.
            Debug.Assert(operation.FindAnnotation(RelationalAnnotationNames.Collation) == null);

            var oldCharSet = operation.OldDatabase[MySqlAnnotationNames.CharSet] as string;
            var newCharSet = operation[MySqlAnnotationNames.CharSet] as string;

            var oldCollation = operation.OldDatabase.Collation;
            var newCollation = operation.Collation;

            // Collations are more specific than charsets. So if a collation has been set, we use the collation instead of the charset.
            if (newCollation != oldCollation &&
                newCollation != null)
            {
                // A new collation has been set. It takes precedence over any defined charset.
                builder
                    .Append("ALTER DATABASE COLLATE ")
                    .Append(newCollation)
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
            }
            else if (newCharSet != oldCharSet ||
                     newCollation != oldCollation && newCollation == null)
            {
                // The charset has been changed or the collation has been reset to the default.
                if (newCharSet != null)
                {
                    // A new charset has been set without an explicit collation.
                    builder
                        .Append("ALTER DATABASE CHARACTER SET ")
                        .Append(newCharSet)
                        .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                    EndStatement(builder);
                }
                else
                {
                    Dependencies.MigrationsLogger.Logger.LogWarning(@"ALTER DATABASE operations can currently not implicitly reset the character set AND the collation to the server default values. Please explicitly specify a character set, a collation or both.");
                }
            }
        }

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

        protected override void Generate(
            DropUniqueConstraintOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
            => Generate(
                new MySqlDropUniqueConstraintAndRecreateForeignKeysOperation
                {
                    IsDestructiveChange = operation.IsDestructiveChange,
                    Name = operation.Name,
                    Schema = operation.Schema,
                    Table = operation.Table,
                    RecreateForeignKeys = false,
                },
                model,
                builder);

        protected virtual void Generate(
            MySqlDropUniqueConstraintAndRecreateForeignKeysOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            void DropUniqueKey()
            {
                builder.Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                    .Append(" DROP KEY ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
            }

            // A foreign key might reuse the alternate key for its own purposes and prohibit its deletion,
            // if the foreign key columns are listed as the first columns and in the same order as in the foreign key (#678).
            // We therefore drop and later recreate all foreign keys to ensure, that no other dependencies on the
            // alternate key exist, if explicitly requested by the user via `MySqlMigrationBuilderExtensions.DropUniqueConstraint()`.
            if (operation.RecreateForeignKeys)
            {
                TemporarilyDropForeignKeys(
                    model,
                    builder,
                    operation.Schema,
                    operation.Table,
                    // model.GetRelationalModel()
                    //     .FindTable(operation.Table, operation.Schema)
                    //     ?.Columns
                    //     .Select(c => c.Name)
                    //     .ToArray(),
                    null,
                    DropUniqueKey);
            }
            else
            {
                DropUniqueKey();
            }
        }

        protected virtual void TemporarilyDropForeignKeys(
            IModel model,
            MigrationCommandListBuilder builder,
            string schemaName,
            string tableName,
            string[] columnNames,
            Action action)
        {
            var foreignKeys = model.GetRelationalModel()
                .FindTable(tableName, schemaName)
                ?.ForeignKeyConstraints?.Where(cs => columnNames == null ||
                                                     columnNames.Length == 0 ||
                                                     cs.Columns.Select(c => c.Name).SequenceEqual(columnNames))
                .ToArray() ?? Array.Empty<IForeignKeyConstraint>();

            foreach (var foreignKey in foreignKeys)
            {
                Generate(new DropForeignKeyOperation
                {
                    Schema = schemaName,
                    Table = tableName,
                    Name = foreignKey.Name,
                }, model, builder);
            }

            action();

            foreach (var foreignKey in foreignKeys)
            {
                Generate(new AddForeignKeyOperation
                {
                    Schema = foreignKey.Table.Schema,
                    Table = foreignKey.Table.Name,
                    Name = foreignKey.Name,
                    Columns = foreignKey.Columns.Select(c => c.Name).ToArray(),
                    PrincipalSchema = foreignKey.PrincipalTable.Schema,
                    PrincipalTable = foreignKey.PrincipalTable.Name,
                    PrincipalColumns = foreignKey.PrincipalColumns.Select(c => c.Name).ToArray(),
                    OnDelete = foreignKey.OnDeleteAction,
                }, model, builder);
            }
        }

        protected static ReferentialAction ToReferentialAction(DeleteBehavior deleteBehavior)
        {
            switch (deleteBehavior)
            {
                case DeleteBehavior.SetNull:
                    return ReferentialAction.SetNull;
                case DeleteBehavior.Cascade:
                    return ReferentialAction.Cascade;
                case DeleteBehavior.NoAction:
                case DeleteBehavior.ClientNoAction:
                    return ReferentialAction.NoAction;
                default:
                    return ReferentialAction.Restrict;
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="DropForeignKeyOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

        // CHECK: Can we improve this implementation?
        /// <summary>
        ///     Builds commands for the given <see cref="RenameColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

            if (_options.ServerVersion.Supports.RenameColumn)
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

            var column = model?.GetRelationalModel().FindTable(operation.Table, operation.Schema).FindColumn(operation.NewName);
            if (column == null)
            {
                if (!(operation[RelationalAnnotationNames.ColumnType] is string type))
                {
                    throw new InvalidOperationException(
                        $"Could not find the column: {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema)}.{Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName)}. Specify the column type explicitly on 'RenameColumn' using the \"{RelationalAnnotationNames.ColumnType}\" annotation");
                }

                builder
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                    .Append(" ")
                    .Append(type)
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
                return;
            }

            var typeMapping = column.PropertyMappings.FirstOrDefault()?.TypeMapping;
            var converter = typeMapping?.Converter;
            var clrType = (converter?.ProviderClrType ?? typeMapping?.ClrType).UnwrapNullableType();
            var columnType = (string)(operation[RelationalAnnotationNames.ColumnType]
                                      ?? column[RelationalAnnotationNames.ColumnType]);
            var isNullable = column.IsNullable;

            var defaultValue = column.DefaultValue;
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

            var isRowVersion = (clrType == typeof(DateTime) || clrType == typeof(byte[])) &&
                               column.IsRowVersion;

            var addColumnOperation = new AddColumnOperation
            {
                Schema = operation.Schema,
                Table = operation.Table,
                Name = operation.NewName,
                ClrType = clrType,
                ColumnType = columnType,
                IsUnicode = column.IsUnicode,
                MaxLength = column.MaxLength,
                IsFixedLength = column.IsFixedLength,
                IsRowVersion = isRowVersion,
                IsNullable = isNullable,
                DefaultValue = defaultValue,
                DefaultValueSql = column.DefaultValueSql,
                ComputedColumnSql = column.ComputedColumnSql,
                IsStored = column.IsStored,
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
        /// <param name="schema"> The schema that contains the sequence, or <see langword="null"/> to use the default schema. </param>
        /// <param name="name"> The sequence name. </param>
        /// <param name="operation"> The sequence options. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

            builder.Append(operation.IsCyclic ? " CYCLE" : " NOCYCLE");
        }

        /// <summary>
        ///     Generates a SQL fragment for a column definition in an <see cref="AddColumnOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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
        /// <param name="schema"> The schema that contains the table, or <see langword="null"/> to use the default schema. </param>
        /// <param name="table"> The table that contains the column. </param>
        /// <param name="name"> The column name. </param>
        /// <param name="operation"> The column metadata. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

            var matchType = GetColumnType(schema, table, name, operation, model);
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
                        if (!_options.ServerVersion.Supports.DateTimeCurrentTimestamp)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                $"on Add or Update in server version {_options.ServerVersion}. Try explicitly setting the column type to TIMESTAMP.");
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
                        if (!_options.ServerVersion.Supports.DateTimeCurrentTimestamp)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                $"on Add or Update in server version {_options.ServerVersion}. Try explicitly setting the column type to TIMESTAMP.");
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

                GenerateComment(operation.Comment, builder);

                // AUTO_INCREMENT has priority over reference definitions.
                if (onUpdateSql != null && !autoIncrement)
                {
                    builder
                        .Append(" ON UPDATE ")
                        .Append(onUpdateSql);
                }
            }
            else
            {
                builder
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                    .Append(" ")
                    .Append(GetColumnType(schema, table, name, operation, model));
                builder
                    .Append(" AS ")
                    .Append($"({operation.ComputedColumnSql})");

                if (operation.IsStored.GetValueOrDefault())
                {
                    builder.Append(" STORED");
                }

                if (operation.IsNullable && _options.ServerVersion.Supports.NullableGeneratedColumns)
                {
                    builder.Append(" NULL");
                }

                GenerateComment(operation.Comment, builder);
            }
        }

        private void GenerateComment(string comment, MigrationCommandListBuilder builder)
        {
            if (comment == null)
                return;

            builder.Append(" COMMENT ")
                .Append(_stringTypeMapping.GenerateSqlLiteral(comment));
        }

        private void ColumnDefinitionWithCharSet(string schema, string table, string name, ColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation.ComputedColumnSql != null)
            {
                ComputedColumnDefinition(schema, table, name, operation, model, builder);
                return;
            }

            var columnType = GetColumnType(schema, table, name, operation, model);

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(columnType);

            builder.Append(operation.IsNullable ? " NULL" : " NOT NULL");

            var isSpatialStoreType = IsSpatialStoreType(columnType);

            if (columnType.IndexOf("blob", StringComparison.OrdinalIgnoreCase) < 0 &&
                columnType.IndexOf("text", StringComparison.OrdinalIgnoreCase) < 0 &&
                columnType.IndexOf("json", StringComparison.OrdinalIgnoreCase) < 0 &&
                !isSpatialStoreType)
            {
                DefaultValue(operation.DefaultValue, operation.DefaultValueSql, columnType, builder);
            }

            var srid = operation[MySqlAnnotationNames.SpatialReferenceSystemId];
            if (srid is int &&
                isSpatialStoreType)
            {
                builder.Append($" /*!80003 SRID {srid} */");
            }
        }

        protected override string GetColumnType(string schema, string table, string name, ColumnOperation operation, IModel model)
            => GetColumnTypeWithCharSetAndCollation(
                operation,
                operation.ColumnType ?? base.GetColumnType(schema, table, name, operation, model));

        private static string GetColumnTypeWithCharSetAndCollation(ColumnOperation operation, string columnType)
        {
            if (columnType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return columnType;
            }

            var charSet = operation[MySqlAnnotationNames.CharSet];
            if (charSet != null)
            {
                const string characterSetClausePattern = @"(CHARACTER SET|CHARSET)\s+\w+";
                var characterSetClause = $@"CHARACTER SET {charSet}";

                columnType = Regex.IsMatch(columnType, characterSetClausePattern, RegexOptions.IgnoreCase)
                    ? Regex.Replace(columnType, characterSetClausePattern, characterSetClause)
                    : columnType.TrimEnd() + " " + characterSetClause;
            }

            // At this point, all legacy `MySql:Collation` annotations should have been replaced by `Relational:Collation` ones.
#pragma warning disable 618
            Debug.Assert(operation.FindAnnotation(MySqlAnnotationNames.Collation) == null);
#pragma warning restore 618

            // Also at this point, all explicitly added `Relational:Collation` annotations (through delegation) should have been set to the
            // `Collation` property and removed.
            Debug.Assert(operation.FindAnnotation(RelationalAnnotationNames.Collation) == null);

            // If we set the collation through delegation, we use the `Relational:Collation` annotation, so the collation will not be in the
            // `Collation` property.
            var collation = operation.Collation;
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

                if (typeMapping is IDefaultValueCompatibilityAware defaultValueCompatibilityAware)
                {
                    typeMapping = defaultValueCompatibilityAware.Clone(true);
                }

                builder
                    .Append(" DEFAULT ")
                    .Append(typeMapping.GenerateSqlLiteral(defaultValue));
            }
        }

        /// <summary>
        ///     Generates a SQL fragment for the primary key constraint of a <see cref="CreateTableOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

        protected override void PrimaryKeyConstraint(
            [NotNull] AddPrimaryKeyOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("PRIMARY KEY ");

            IndexTraits(operation, model, builder);

            builder.Append("(")
                .Append(ColumnListWithIndexPrefixLength(operation, operation.Columns))
                .Append(")");
        }

        protected override void UniqueConstraint(
            [NotNull] AddUniqueConstraintOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("UNIQUE ");

            IndexTraits(operation, model, builder);

            builder.Append("(")
                .Append(ColumnListWithIndexPrefixLength(operation, operation.Columns))
                .Append(")");
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

        protected override void Generate(
            DropPrimaryKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
            => Generate(
                new MySqlDropPrimaryKeyAndRecreateForeignKeysOperation
                {
                    IsDestructiveChange = operation.IsDestructiveChange,
                    Name = operation.Name,
                    Schema = operation.Schema,
                    Table = operation.Table,
                    RecreateForeignKeys = false,
                },
                model,
                builder,
                terminate);

        protected virtual void Generate(
            MySqlDropPrimaryKeyAndRecreateForeignKeysOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            void DropPrimaryKey()
            {
                builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY({_stringTypeMapping.GenerateSqlLiteral(operation.Schema)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Table)});")
                    .AppendLine()
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                    .Append(" DROP PRIMARY KEY");

                if (terminate)
                {
                    builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                    EndStatement(builder);
                }
            }

            // A foreign key might reuse the primary key for its own purposes and prohibit its deletion,
            // if the foreign key columns are listed as the first columns and in the same order as in the foreign key (#678).
            // We therefore drop and later recreate all foreign keys to ensure, that no other dependencies on the
            // primary key exist, if explicitly requested by the user via `MySqlMigrationBuilderExtensions.DropPrimaryKey()`.
            if (operation.RecreateForeignKeys)
            {
                TemporarilyDropForeignKeys(
                    model,
                    builder,
                    operation.Schema,
                    operation.Table,
                    // model.GetRelationalModel()
                    //     .FindTable(operation.Table, operation.Schema)
                    //     ?.Columns
                    //     .Select(c => c.Name)
                    //     .ToArray(),
                    null,
                    DropPrimaryKey);
            }
            else
            {
                DropPrimaryKey();
            }
        }

        /// <summary>
        ///     Generates a SQL fragment for a foreign key constraint of an <see cref="AddForeignKeyOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
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

        protected override void IndexOptions(CreateIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            // The base implementation supports index filters in form of a WHERE clause.
            // This is not supported by MySQL, so we don't call it here.

            var fullText = operation[MySqlAnnotationNames.FullTextIndex] as bool?;
            if (fullText == true)
            {
                var fullTextParser = operation[MySqlAnnotationNames.FullTextParser] as string;
                if (!string.IsNullOrEmpty(fullTextParser))
                {
                    // Official MySQL support exists since 5.1, but since MariaDB does not support full-text parsers and does not recognize
                    // the "/*!xxxxx" syntax for versions below 50700, we use 50700 here, even though the statement would work in lower
                    // versions as well. Since we don't support MySQL 5.6 officially anymore, this is fine.
                    builder.Append(" /*!50700 WITH PARSER ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(fullTextParser))
                        .Append(" */");
                }
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

        private string ColumnListWithIndexPrefixLength(MigrationOperation operation, string[] columns)
            => operation[MySqlAnnotationNames.IndexPrefixLength] is int[] prefixValues
                ? ColumnList(
                    columns,
                    (c, i) => prefixValues.Length > i && prefixValues[i] > 0
                        ? $"({prefixValues[i]})"
                        : null)
                : ColumnList(columns);

        protected virtual string ColumnList([NotNull] string[] columns, Func<string, int, string> columnPostfix)
            => string.Join(", ", columns.Select((c, i) => Dependencies.SqlGenerationHelper.DelimitIdentifier(c) + columnPostfix?.Invoke(c, i)));

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

        private static bool IsSpatialStoreType(string storeType)
            => _spatialStoreTypes.Contains(storeType);
    }
}
