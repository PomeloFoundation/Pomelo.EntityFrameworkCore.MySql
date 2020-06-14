using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlQueryTranslationPostprocessor : RelationalQueryTranslationPostprocessor
    {
        private readonly IMySqlOptions _options;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlQueryTranslationPostprocessor(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext,
            IMySqlOptions options,
            MySqlSqlExpressionFactory sqlExpressionFactory)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _options = options;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public override Expression Process(Expression query)
        {
            query = base.Process(query);

            if (_options.IndexOptimizedBooleanColumns)
            {
                query = new MySqlBoolOptimizingExpressionVisitor(SqlExpressionFactory).Visit(query);
            }

            query = new MySqlJsonParameterExpressionVisitor(_sqlExpressionFactory).Visit(query);
            query = new MySqlCompatibilityExpressionVisitor(_options).Visit(query);

            return query;
        }
    }
}
