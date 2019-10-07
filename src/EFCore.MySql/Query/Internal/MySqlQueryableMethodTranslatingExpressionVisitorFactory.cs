using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQueryableMethodTranslatingExpressionVisitorFactory : IQueryableMethodTranslatingExpressionVisitorFactory
    {
        private readonly QueryableMethodTranslatingExpressionVisitorDependencies _dependencies;
        private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies _relationalDependencies;
        private readonly IMySqlConnectionInfo _connectionInfo;

        public MySqlQueryableMethodTranslatingExpressionVisitorFactory(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            IMySqlConnectionInfo connectionInfo)
        {
            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
            _connectionInfo = connectionInfo;
        }

        public virtual QueryableMethodTranslatingExpressionVisitor Create(IModel model)
            => new MySqlQueryableMethodTranslatingExpressionVisitor(
                _dependencies,
                _relationalDependencies,
                _connectionInfo,
                model);
    }
}
