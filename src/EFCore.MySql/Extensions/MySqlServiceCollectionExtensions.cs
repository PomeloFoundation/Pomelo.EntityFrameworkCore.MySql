// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pomelo.EntityFrameworkCore.MySql.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MySqlServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkMySql([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, MySqlLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<MySqlOptionsExtension>>()
                .TryAdd<IRelationalTypeMappingSource, MySqlTypeMappingSource>()
                .TryAdd<IRelationalTransactionFactory, MySqlRelationalTransactionFactory>()
                .TryAdd<ISqlGenerationHelper, MySqlSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, MySqlMigrationsAnnotationProvider>()
                .TryAdd<IProviderConventionSetBuilder, MySqlConventionSetBuilder>()
                .TryAdd<IModelValidator, MySqlModelValidator>()
                .TryAdd<IUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
                .TryAdd<IModificationCommandBatchFactory, MySqlModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, MySqlValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IMySqlRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, MySqlMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, MySqlDatabaseCreator>()
                .TryAdd<IHistoryRepository, MySqlHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, MySqlCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, MySqlExecutionStrategyFactory>()
                .TryAdd<ISingletonOptions, IMySqlOptions>(p => p.GetService<IMySqlOptions>())
                .TryAdd<IMigrator, MySqlMigrator>()
                .TryAdd<IMethodCallTranslatorProvider, MySqlMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, MySqlMemberTranslatorProvider>()
                .TryAdd<IQuerySqlGeneratorFactory, MySqlQuerySqlGeneratorFactory>()
                .TryAdd<ISqlExpressionFactory, MySqlSqlExpressionFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, MySqlSqlTranslatingExpressionVisitorFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, MySqlQueryTranslationPostprocessorFactory>()
                .TryAdd<IMigrationsModelDiffer, MySqlMigrationsModelDiffer>()
                .TryAddProviderSpecificServices(m => m
                    .TryAddSingleton<IMySqlOptions, MySqlOptions>()
                    .TryAddScoped<IMySqlRelationalConnection, MySqlRelationalConnection>()
                    .TryAddScoped<IMySqlUpdateSqlGenerator, MySqlUpdateSqlGenerator>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
