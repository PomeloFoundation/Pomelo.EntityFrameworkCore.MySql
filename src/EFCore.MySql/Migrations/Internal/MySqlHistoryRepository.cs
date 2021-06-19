// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations.Internal
{
    public class MySqlHistoryRepository : HistoryRepository
    {
        private const string MigrationsScript = nameof(MigrationsScript);

        public MySqlHistoryRepository([NotNull] HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override void ConfigureTable([NotNull] EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.HasCharSet(CharSet.Utf8Mb4);
        }

        protected override string ExistsSql
        {
            get
            {
                var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

                var builder = new StringBuilder();

                builder.Append("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE ");

                builder
                    .Append("TABLE_SCHEMA=")
                    .Append(stringTypeMapping.GenerateSqlLiteral(TableSchema ?? Dependencies.Connection.DbConnection.Database))
                    .Append(" AND TABLE_NAME=")
                    .Append(stringTypeMapping.GenerateSqlLiteral(TableName))
                    .Append(";");

                return builder.ToString();
            }
        }

        protected override bool InterpretExistsResult(object value) => value != null;

        public override string GetCreateIfNotExistsScript()
        {
            var script = GetCreateScript();
            return script.Insert(script.IndexOf("CREATE TABLE", StringComparison.Ordinal) + 12, " IF NOT EXISTS");
        }

        /// <summary>
        ///     Overridden by database providers to generate a SQL Script that will `BEGIN` a block
        ///     of SQL if and only if the migration with the given identifier does not already exist in the history table.
        /// </summary>
        /// <param name="migrationId"> The migration identifier. </param>
        /// <returns> The generated SQL. </returns>
        public override string GetBeginIfNotExistsScript(string migrationId) => $@"
DROP PROCEDURE IF EXISTS {MigrationsScript};
DELIMITER //
CREATE PROCEDURE {MigrationsScript}()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM {SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema)} WHERE {SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName)} = '{migrationId}') THEN
";

        /// <summary>
        ///     Overridden by database providers to generate a SQL Script that will `BEGIN` a block
        ///     of SQL if and only if the migration with the given identifier already exists in the history table.
        /// </summary>
        /// <param name="migrationId"> The migration identifier. </param>
        /// <returns> The generated SQL. </returns>
        public override string GetBeginIfExistsScript(string migrationId) => $@"
DROP PROCEDURE IF EXISTS {MigrationsScript};
DELIMITER //
CREATE PROCEDURE {MigrationsScript}()
BEGIN
    IF EXISTS(SELECT 1 FROM {SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema)} WHERE {SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName)} = '{migrationId}') THEN
";

        /// <summary>
        ///     Overridden by database providers to generate a SQL script to `END` the SQL block.
        /// </summary>
        /// <returns> The generated SQL. </returns>
        public override string GetEndIfScript() => $@"
    END IF;
END //
DELIMITER ;
CALL {MigrationsScript}();
DROP PROCEDURE {MigrationsScript};
";

        public virtual void ConfigureModel(ModelBuilder modelBuilder)
            => modelBuilder.HasCharSet(null, DelegationModes.ApplyToDatabases);

        #region Necessary implementation because we cannot directly override EnsureModel

        private IModel _model;
        private string _migrationIdColumnName;
        private string _productVersionColumnName;

        // Customized implementation.
        protected virtual IModel EnsureModel()
        {
            if (_model == null)
            {
                var conventionSet = Dependencies.ConventionSetBuilder.CreateConventionSet();

                // Use public API to remove the convention, issue #214
                ConventionSet.Remove(conventionSet.ModelInitializedConventions, typeof(DbSetFindingConvention));
                ConventionSet.Remove(conventionSet.ModelInitializedConventions, typeof(RelationalDbFunctionAttributeConvention));

                var modelBuilder = new ModelBuilder(conventionSet);

                #region Custom implementation

                ConfigureModel(modelBuilder);

                #endregion

                modelBuilder.Entity<HistoryRow>(
                    x =>
                    {
                        ConfigureTable(x);
                        x.ToTable(TableName, TableSchema);
                    });

                _model = Dependencies.ModelRuntimeInitializer.Initialize(modelBuilder.FinalizeModel(), designTime: true, validationLogger: null);
            }

            return _model;
        }

        // Original implementation.
        public override string GetCreateScript()
        {
            var model = EnsureModel();

            var operations = Dependencies.ModelDiffer.GetDifferences(null, model.GetRelationalModel());
            var commandList = Dependencies.MigrationsSqlGenerator.Generate(operations, model);

            return string.Concat(commandList.Select(c => c.CommandText));
        }

        // Original implementation.
        protected override string MigrationIdColumnName
            => _migrationIdColumnName ??= EnsureModel()
                .FindEntityType(typeof(HistoryRow))!
                .FindProperty(nameof(HistoryRow.MigrationId))!
                .GetColumnBaseName();

        // Original implementation.
        protected override string ProductVersionColumnName
            => _productVersionColumnName ??= EnsureModel()
                .FindEntityType(typeof(HistoryRow))!
                .FindProperty(nameof(HistoryRow.ProductVersion))!
                .GetColumnBaseName();

        #endregion Necessary implementation because we cannot directly override EnsureModel
    }
}
