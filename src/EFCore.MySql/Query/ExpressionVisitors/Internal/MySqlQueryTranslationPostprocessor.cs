using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlQueryTranslationPostprocessor : RelationalQueryTranslationPostprocessor
    {
        private readonly IMySqlConnectionInfo _connectionInfo;

        public MySqlQueryTranslationPostprocessor(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext,
            IMySqlConnectionInfo connectionInfo)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _connectionInfo = connectionInfo;
        }

        public override Expression Process(Expression query)
        {
            query = new MySqlCompatibilityExpressionVisitor(_connectionInfo).Visit(query);

            return base.Process(query);
        }
    }
}
