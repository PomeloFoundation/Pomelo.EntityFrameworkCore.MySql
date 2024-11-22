//ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestMigrator : MySqlMigrator
    {
        public Func<MigrationsSqlGenerationOptions, MigrationsSqlGenerationOptions> MigrationsSqlGenerationOptionsOverrider { get; set; }

        public MySqlTestMigrator(
            IMigrationsAssembly migrationsAssembly,
            IHistoryRepository historyRepository,
            IDatabaseCreator databaseCreator,
            IMigrationsSqlGenerator migrationsSqlGenerator,
            IRawSqlCommandBuilder rawSqlCommandBuilder,
            IMigrationCommandExecutor migrationCommandExecutor,
            IRelationalConnection connection,
            ISqlGenerationHelper sqlGenerationHelper,
            ICurrentDbContext currentContext,
            IModelRuntimeInitializer modelRuntimeInitializer,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            IRelationalCommandDiagnosticsLogger commandLogger,
            IDatabaseProvider databaseProvider,
            IMigrationsModelDiffer migrationsModelDiffer,
            IDesignTimeModel designTimeModel,
            IDbContextOptions contextOptions,
            IExecutionStrategy executionStrategy)
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
                modelRuntimeInitializer,
                logger,
                commandLogger,
                databaseProvider,
                migrationsModelDiffer,
                designTimeModel,
                contextOptions,
                executionStrategy)
        {
        }

        protected override IReadOnlyList<MigrationCommand> GenerateUpSql(
            Migration migration,
            MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
            => base.GenerateUpSql(migration, MigrationsSqlGenerationOptionsOverrider?.Invoke(options) ?? options);

        protected override IReadOnlyList<MigrationCommand> GenerateDownSql(
            Migration migration,
            Migration previousMigration,
            MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
            => base.GenerateDownSql(migration, previousMigration, MigrationsSqlGenerationOptionsOverrider?.Invoke(options) ?? options);
    }
}
