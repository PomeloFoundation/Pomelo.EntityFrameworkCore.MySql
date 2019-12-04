using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations.Internal
{
    public class MySqlMigrator : Migrator
    {
        private static readonly Dictionary<Type, Tuple<string, string>> _customMigrationCommands = new Dictionary<Type, Tuple<string, string>>
        {
            { typeof(DropPrimaryKeyOperation), new Tuple<string, string>(BeforeDropPrimaryKeyMigrationBegin, BeforeDropPrimaryKeyMigrationEnd) },
            { typeof(AddPrimaryKeyOperation), new Tuple<string, string>(AfterAddPrimaryKeyMigrationBegin, AfterAddPrimaryKeyMigrationEnd) },
        };

        private readonly IMigrationsAssembly _migrationsAssembly;
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;
        private readonly ICurrentDbContext _currentContext;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _commandLogger;

        private bool _generateScript;
        private bool _isIdempotent;

        public MySqlMigrator(
            [NotNull] IMigrationsAssembly migrationsAssembly,
            [NotNull] IHistoryRepository historyRepository,
            [NotNull] IDatabaseCreator databaseCreator,
            [NotNull] IMigrationsSqlGenerator migrationsSqlGenerator,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder,
            [NotNull] IMigrationCommandExecutor migrationCommandExecutor,
            [NotNull] IRelationalConnection connection,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper,
            [NotNull] ICurrentDbContext currentContext,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> commandLogger,
            [NotNull] IDatabaseProvider databaseProvider)
            : base(
                migrationsAssembly,
                historyRepository,
                databaseCreator,
                migrationsSqlGenerator,
                rawSqlCommandBuilder,
                migrationCommandExecutor,
                connection,
                sqlGenerationHelper,
                currentContext,
                logger,
                commandLogger,
                databaseProvider)
        {
            _migrationsAssembly = migrationsAssembly;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
            _currentContext = currentContext;
            _commandLogger = commandLogger;
        }

        protected override IReadOnlyList<MigrationCommand> GenerateUpSql(Migration migration)
        {
            var commands = base.GenerateUpSql(migration);

            return _generateScript && _isIdempotent
                ? commands
                : WrapWithCustomCommands(
                    migration.UpOperations,
                    commands.ToList());
        }

        protected override IReadOnlyList<MigrationCommand> GenerateDownSql(
            Migration migration,
            Migration previousMigration)
        {
            var commands = base.GenerateDownSql(migration, previousMigration);

            return _generateScript && _isIdempotent
                ? commands
                : WrapWithCustomCommands(
                    migration.DownOperations,
                    commands.ToList());
        }

        public override void Migrate(string targetMigration = null)
        {
            _generateScript = false;
            _isIdempotent = false;
            base.Migrate(targetMigration);
        }

        public override Task MigrateAsync(string targetMigration = null, CancellationToken cancellationToken = new CancellationToken())
        {
            _generateScript = false;
            _isIdempotent = false;
            return base.MigrateAsync(targetMigration, cancellationToken);
        }

        public override string GenerateScript(string fromMigration = null, string toMigration = null, bool idempotent = false)
        {
            _generateScript = true;
            _isIdempotent = idempotent;

            if (!idempotent)
            {
                return base.GenerateScript(fromMigration, toMigration, false);
            }

            IEnumerable<string> appliedMigrations;

            if (string.IsNullOrEmpty(fromMigration)
                || fromMigration == Migration.InitialDatabase)
            {
                appliedMigrations = Enumerable.Empty<string>();
            }
            else
            {
                var fromMigrationId = _migrationsAssembly.GetMigrationId(fromMigration);
                appliedMigrations = _migrationsAssembly.Migrations
                    .Where(t => string.Compare(t.Key, fromMigrationId, StringComparison.OrdinalIgnoreCase) <= 0)
                    .Select(t => t.Key);
            }

            PopulateMigrations(
                appliedMigrations,
                toMigration,
                out var migrationsToApply,
                out var migrationsToRevert,
                out var actualTargetMigration);

            var operations = migrationsToApply
                .SelectMany(x => x.UpOperations)
                .Concat(migrationsToRevert.SelectMany(x => x.DownOperations))
                .ToList();

            var builder = new StringBuilder();

            builder.AppendJoin(string.Empty, GetMigrationCommandTexts(operations, true));
            builder.Append(base.GenerateScript(fromMigration, toMigration, true));
            builder.AppendJoin(string.Empty, GetMigrationCommandTexts(operations, false));

            return builder.ToString();
        }

        private IReadOnlyList<MigrationCommand> WrapWithCustomCommands(
            IReadOnlyList<MigrationOperation> migrationOperations,
            List<MigrationCommand> migrationCommands)
        {
            var beginCommandTexts = GetMigrationCommandTexts(migrationOperations, true);
            var endCommandTexts = GetMigrationCommandTexts(migrationOperations, false);

            migrationCommands.InsertRange(0, beginCommandTexts.Select(t => new MigrationCommand(
                _rawSqlCommandBuilder.Build(t),
                _currentContext.Context,
                _commandLogger)));

            migrationCommands.AddRange(endCommandTexts.Select(t => new MigrationCommand(
                _rawSqlCommandBuilder.Build(t),
                _currentContext.Context,
                _commandLogger)));

            return migrationCommands;
        }

        private string[] GetMigrationCommandTexts(IReadOnlyList<MigrationOperation> migrationOperations, bool beginTexts)
            => GetCustomCommands(migrationOperations)
                .Select(t => PrepareString(beginTexts ? t.Item1 : t.Item2))
                .ToArray();

        private static IReadOnlyList<Tuple<string, string>> GetCustomCommands(IReadOnlyList<MigrationOperation> migrationOperations)
            => _customMigrationCommands
                .Where(c => migrationOperations.Any(o => c.Key.IsInstanceOfType(o)) && c.Value != null)
                .Select(kvp => kvp.Value)
                .ToList();

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

        private string PrepareString(string str)
        {
            str = _generateScript
                ? str
                : CleanUpScriptSpecificPseudoStatements(str);

            str = str
                .Replace("\r", string.Empty)
                .Replace("\n", Environment.NewLine);

            str += _generateScript
                ? Environment.NewLine + (
                      _isIdempotent
                          ? Environment.NewLine
                          : string.Empty)
                : string.Empty;

            return str;
        }

        #region BeforeDropPrimaryKey

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

        #endregion BeforeDropPrimaryKey

        #region AfterAddPrimaryKey

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

        #endregion AfterAddPrimaryKey
    }
}

