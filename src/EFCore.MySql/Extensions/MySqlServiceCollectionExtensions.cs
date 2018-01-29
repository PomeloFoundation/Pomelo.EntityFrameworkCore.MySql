// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MySqlServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkMySql([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<IDatabaseProvider, DatabaseProvider<MySqlOptionsExtension>>()
                .TryAdd<IRelationalTypeMapper, MySqlSmartTypeMapper>()
                .TryAdd<ISqlGenerationHelper, MySqlSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, MySqlMigrationsAnnotationProvider>()
                .TryAdd<IConventionSetBuilder, MySqlConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<IMySqlUpdateSqlGenerator>())
                .TryAdd<IModificationCommandBatchFactory, MySqlModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, MySqlValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IMySqlRelationalConnection>())
                .TryAdd<IRelationalCommandBuilderFactory, MySqlCommandBuilderFactory>()
                .TryAdd<IMigrationsSqlGenerator, MySqlMigrationsSqlGenerator>()
                .TryAdd<IBatchExecutor, MySqlBatchExecutor>()
                .TryAdd<IRelationalDatabaseCreator, MySqlDatabaseCreator>()
                .TryAdd<IHistoryRepository, MySqlHistoryRepository>()
                .TryAdd<IExecutionStrategyFactory, MySqlExecutionStrategyFactory>()
                .TryAdd<IMemberTranslator, MySqlCompositeMemberTranslator>()
                .TryAdd<ICompositeMethodCallTranslator, MySqlCompositeMethodCallTranslator>()
                .TryAdd<IQuerySqlGeneratorFactory, MySqlQuerySqlGeneratorFactory>()
                .TryAdd<ISingletonOptions, IMySqlOptions>(p => p.GetService<IMySqlOptions>())
                .TryAddProviderSpecificServices(b => b
                    .TryAddSingleton<IMySqlOptions, MySqlOptions>()
                    .TryAddScoped<IMySqlUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
                    .TryAddScoped<IMySqlRelationalConnection, MySqlRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
