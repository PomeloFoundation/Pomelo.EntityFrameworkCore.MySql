// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

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
            history.Property(h => h.MigrationId).HasColumnType("varchar(95)");
            history.Property(h => h.ProductVersion).HasColumnType("varchar(32)").IsRequired();
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
        ///     Overridden by database providers to generate a SQL Script that will <c>BEGIN</c> a block
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
        ///     Overridden by database providers to generate a SQL Script that will <c>BEGIN</c> a block
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
        ///     Overridden by database providers to generate a SQL script to <c>END</c> the SQL block.
        /// </summary>
        /// <returns> The generated SQL. </returns>
        public override string GetEndIfScript() => $@"
    END IF;
END //
DELIMITER ;
CALL {MigrationsScript}();
DROP PROCEDURE {MigrationsScript};
";
    }
}
