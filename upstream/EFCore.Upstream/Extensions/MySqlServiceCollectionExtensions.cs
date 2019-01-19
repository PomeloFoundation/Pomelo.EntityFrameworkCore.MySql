// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     MySql specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class MySqlServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Adds the services required by the Microsoft MySql database provider for Entity Framework
        ///         to an <see cref="IServiceCollection" />. You use this method when using dependency injection
        ///         in your application, such as with ASP.NET. For more information on setting up dependency
        ///         injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        ///     <para>
        ///         You only need to use this functionality when you want Entity Framework to resolve the services it uses
        ///         from an external dependency injection container. If you are not using an external
        ///         dependency injection container, Entity Framework will take care of creating the services it requires.
        ///     </para>
        /// </summary>
        /// <example>
        ///     <code>
        ///            public void ConfigureServices(IServiceCollection services)
        ///            {
        ///                var connectionString = "connection string to database";
        /// 
        ///                services
        ///                    .AddEntityFrameworkMySql()
        ///                    .AddDbContext&lt;MyContext&gt;((serviceProvider, options) =>
        ///                        options.UseMySql(connectionString)
        ///                               .UseInternalServiceProvider(serviceProvider));
        ///            }
        ///        </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkMySql([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<IDatabaseProvider, DatabaseProvider<MySqlOptionsExtension>>()
                .TryAdd<IValueGeneratorCache>(p => p.GetService<IMySqlValueGeneratorCache>())
                .TryAdd<IRelationalTypeMappingSource, MySqlTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, MySqlSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, MySqlMigrationsAnnotationProvider>()
                .TryAdd<IModelValidator, MySqlModelValidator>()
                .TryAdd<IConventionSetBuilder, MySqlConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<IMySqlUpdateSqlGenerator>())
                .TryAdd<ISingletonUpdateSqlGenerator>(p => p.GetService<IMySqlUpdateSqlGenerator>())
                .TryAdd<IModificationCommandBatchFactory, MySqlModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, MySqlValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IMySqlConnection>())
                .TryAdd<IMigrationsSqlGenerator, MySqlMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, MySqlDatabaseCreator>()
                .TryAdd<IHistoryRepository, MySqlHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, MySqlCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, MySqlExecutionStrategyFactory>()
                .TryAdd<IQueryCompilationContextFactory, MySqlQueryCompilationContextFactory>()
                .TryAdd<IMemberTranslator, MySqlCompositeMemberTranslator>()
                .TryAdd<ICompositeMethodCallTranslator, MySqlCompositeMethodCallTranslator>()
                .TryAdd<IQuerySqlGeneratorFactory, MySqlQuerySqlGeneratorFactory>()
                .TryAdd<ISqlTranslatingExpressionVisitorFactory, MySqlSqlTranslatingExpressionVisitorFactory>()
                .TryAdd<ISingletonOptions, IMySqlOptions>(p => p.GetService<IMySqlOptions>())
                .TryAddProviderSpecificServices(
                    b => b
                        .TryAddSingleton<IMySqlValueGeneratorCache, MySqlValueGeneratorCache>()
                        .TryAddSingleton<IMySqlOptions, MySqlOptions>()
                        .TryAddSingleton<IMySqlUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
                        .TryAddSingleton<IMySqlSequenceValueGeneratorFactory, MySqlSequenceValueGeneratorFactory>()
                        .TryAddScoped<IMySqlConnection, MySqlConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
