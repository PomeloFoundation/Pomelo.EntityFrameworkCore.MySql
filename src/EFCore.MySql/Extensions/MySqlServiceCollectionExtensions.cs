// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

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
                .TryAdd<IRelationalTypeMappingSource, MySqlTypeMappingSource>()
                .TryAdd<IRelationalTransactionFactory, MySqlRelationalTransactionFactory>()
                .TryAdd<ISqlGenerationHelper, MySqlSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, MySqlMigrationsAnnotationProvider>()
                .TryAdd<IConventionSetBuilder, MySqlConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<IMySqlUpdateSqlGenerator>())
                .TryAdd<IModificationCommandBatchFactory, MySqlModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, MySqlValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IMySqlRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, MySqlMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, MySqlDatabaseCreator>()
                .TryAdd<IHistoryRepository, MySqlHistoryRepository>()
                .TryAdd<IExecutionStrategyFactory, MySqlExecutionStrategyFactory>()
                .TryAdd<IMemberTranslator, MySqlCompositeMemberTranslator>()
                .TryAdd<ICompositeMethodCallTranslator, MySqlCompositeMethodCallTranslator>()
                .TryAdd<IQuerySqlGeneratorFactory, MySqlQuerySqlGeneratorFactory>()
                .TryAdd<ISingletonOptions, IMySqlOptions>(p => p.GetService<IMySqlOptions>())
                .TryAdd<IMigrator, MySqlMigrator>()
                .TryAddProviderSpecificServices(b => b
                    .TryAddSingleton<IMySqlOptions, MySqlOptions>()
                    .TryAddScoped<IMySqlUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
                    .TryAddScoped<IMySqlRelationalConnection, MySqlRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
