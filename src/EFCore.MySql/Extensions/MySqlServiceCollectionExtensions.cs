// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
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
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MySqlServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Registers the given Entity Framework context as a service in the <see cref="IServiceCollection" />
        ///         and configures it to connect to a MySQL compatible database.
        ///     </para>
        ///     <para>
        ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
        ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
        ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
        ///         overridden to configure the Pomelo.EntityFrameworkCore.MySql provider and connection string.
        ///     </para>
        ///     <para>
        ///         To configure the <see cref="DbContextOptions{TContext}" /> for the context, either override the
        ///         <see cref="DbContext.OnConfiguring" /> method in your derived context, or supply
        ///         an optional action to configure the <see cref="DbContextOptions" /> for the context.
        ///     </para>
        ///     <para>
        ///         For more information on how to use this method, see the Entity Framework Core documentation at https://aka.ms/efdocs.
        ///         For more information on using dependency injection, see https://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be registered. </typeparam>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="MySqlServerVersion"/> (for MySQL) or
        ///         <see cref="MariaDbServerVersion"/> (for MariaDB) classes.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <param name="optionsAction"> An optional action to configure the <see cref="DbContextOptions" /> for the context. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddMySql<TContext>(
            this IServiceCollection serviceCollection,
            string connectionString,
            ServerVersion serverVersion,
            Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null,
            Action<DbContextOptionsBuilder> optionsAction = null)
            where TContext : DbContext
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            return serviceCollection.AddDbContext<TContext>((_, options) =>
            {
                optionsAction?.Invoke(options);
                options.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
            });
        }

        public static IServiceCollection AddEntityFrameworkMySql([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, MySqlLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<MySqlOptionsExtension>>()
                //.TryAdd<IValueGeneratorCache>(p => p.GetService<IMySqlValueGeneratorCache>())
                .TryAdd<IRelationalTypeMappingSource, MySqlTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, MySqlSqlGenerationHelper>()
                .TryAdd<IRelationalAnnotationProvider, MySqlAnnotationProvider>()
                .TryAdd<IModelValidator, MySqlModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, MySqlConventionSetBuilder>()
                //.TryAdd<IRelationalValueBufferFactoryFactory, TypedRelationalValueBufferFactoryFactory>() // What is that?
                .TryAdd<IUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
                .TryAdd<IModificationCommandFactory, MySqlModificationCommandFactory>()
                .TryAdd<IModificationCommandBatchFactory, MySqlModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, MySqlValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IMySqlRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, MySqlMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, MySqlDatabaseCreator>()
                .TryAdd<IHistoryRepository, MySqlHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, MySqlCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, MySqlExecutionStrategyFactory>()
                .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, MySqlQueryableMethodTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalQueryStringFactory, MySqlQueryStringFactory>()
                .TryAdd<IMethodCallTranslatorProvider, MySqlMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, MySqlMemberTranslatorProvider>()
                .TryAdd<IEvaluatableExpressionFilter, MySqlEvaluatableExpressionFilter>()
                .TryAdd<IQuerySqlGeneratorFactory, MySqlQuerySqlGeneratorFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, MySqlSqlTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalParameterBasedSqlProcessorFactory, MySqlParameterBasedSqlProcessorFactory>()
                .TryAdd<ISqlExpressionFactory, MySqlSqlExpressionFactory>()
                .TryAdd<ISingletonOptions, IMySqlOptions>(p => p.GetService<IMySqlOptions>())
                //.TryAdd<IValueConverterSelector, MySqlValueConverterSelector>()
                .TryAdd<IQueryCompilationContextFactory, MySqlQueryCompilationContextFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, MySqlQueryTranslationPostprocessorFactory>()

                // TODO: Injecting this service will make our original JSON implementations work, but interferes with EF Core 8's new
                //       primitive collections support.
                //       We will need to limit the preprocessor logic to only the relevant cases.
                .TryAdd<IQueryTranslationPreprocessorFactory, MySqlQueryTranslationPreprocessorFactory>()

                .TryAdd<IMigrationsModelDiffer, MySqlMigrationsModelDiffer>()
                .TryAdd<IMigrator, MySqlMigrator>()
                .TryAddProviderSpecificServices(m => m
                    //.TryAddSingleton<IMySqlValueGeneratorCache, MySqlValueGeneratorCache>()
                    .TryAddSingleton<IMySqlOptions, MySqlOptions>()
                    //.TryAddScoped<IMySqlSequenceValueGeneratorFactory, MySqlSequenceValueGeneratorFactory>()
                    .TryAddScoped<IMySqlUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
                    .TryAddScoped<IMySqlRelationalConnection, MySqlRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
