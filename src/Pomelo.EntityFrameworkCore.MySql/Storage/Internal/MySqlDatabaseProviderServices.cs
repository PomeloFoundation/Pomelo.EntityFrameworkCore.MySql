// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlDatabaseProviderServices : RelationalDatabaseProviderServices
    {
        public MySqlDatabaseProviderServices([NotNull] IServiceProvider services)
            : base(services)
        {
        }

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;
        public override IBatchExecutor BatchExecutor => GetService<MySqlBatchExecutor>();
        public override IDatabaseCreator Creator => GetService<MySqlDatabaseCreator>();
        public override IRelationalConnection RelationalConnection => GetService<MySqlRelationalConnection>();
        public override ISqlGenerationHelper SqlGenerationHelper => GetService<MySqlSqlGenerationHelper>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<MySqlDatabaseCreator>();
        public override IMigrationsAnnotationProvider MigrationsAnnotationProvider => GetService<MySqlMigrationsAnnotationProvider>();
        public override IHistoryRepository HistoryRepository => GetService<MySqlHistoryRepository>();
        public override IMigrationsSqlGenerator MigrationsSqlGenerator => GetService<MySqlMigrationsSqlGenerationHelper>();
        public override IModelSource ModelSource => GetService<MySqlModelSource>();
        public override IUpdateSqlGenerator UpdateSqlGenerator => GetService<MySqlUpdateSqlGenerator>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<MySqlValueGeneratorCache>();
        public override IRelationalTypeMapper TypeMapper => GetService<MySqlTypeMapper>();
        public override IConventionSetBuilder ConventionSetBuilder => GetService<MySqlConventionSetBuilder>();
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => GetService<MySqlModificationCommandBatchFactory>();
        public override IRelationalValueBufferFactoryFactory ValueBufferFactoryFactory => GetService<TypedRelationalValueBufferFactoryFactory>();
        public override IRelationalAnnotationProvider AnnotationProvider => GetService<MySqlAnnotationProvider>();
        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<MySqlCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<MySqlCompositeMemberTranslator>();
        public override IQueryCompilationContextFactory QueryCompilationContextFactory => GetService<MySqlQueryCompilationContextFactory>();
        public override IQuerySqlGeneratorFactory QuerySqlGeneratorFactory => GetService<MySqlQuerySqlGenerationHelperFactory>();
    }
}
