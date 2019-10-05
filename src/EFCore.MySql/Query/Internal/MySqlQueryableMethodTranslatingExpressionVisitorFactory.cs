using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQueryableMethodTranslatingExpressionVisitorFactory : IQueryableMethodTranslatingExpressionVisitorFactory
    {
        private readonly QueryableMethodTranslatingExpressionVisitorDependencies _dependencies;
        private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies _relationalDependencies;
        private readonly IMySqlOptions _options;

        public MySqlQueryableMethodTranslatingExpressionVisitorFactory(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            IMySqlOptions options)
        {
            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
            _options = options;
        }

        public virtual QueryableMethodTranslatingExpressionVisitor Create(IModel model)
            => new MySqlQueryableMethodTranslatingExpressionVisitor(
                _dependencies,
                _relationalDependencies,
                _options,
                model);
    }
}
