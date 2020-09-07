using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlSqlTranslatingExpressionVisitorFactory : IRelationalSqlTranslatingExpressionVisitorFactory
    {
        private readonly RelationalSqlTranslatingExpressionVisitorDependencies _dependencies;
        private readonly IMySqlJsonPocoTranslator _jsonPocoTranslator;

        public MySqlSqlTranslatingExpressionVisitorFactory(
            [NotNull] RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] IServiceProvider serviceProvider)
        {
            _dependencies = dependencies;
            _jsonPocoTranslator = serviceProvider.GetService<IMySqlJsonPocoTranslator>();
        }

        public virtual RelationalSqlTranslatingExpressionVisitor Create(
            IModel model,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
            => new MySqlSqlTranslatingExpressionVisitor(
                _dependencies,
                model,
                queryableMethodTranslatingExpressionVisitor,
                _jsonPocoTranslator);
    }
}
