using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlQueryTranslationPostprocessor : RelationalQueryTranslationPostprocessor
    {
        private readonly IMySqlOptions _options;

        public MySqlQueryTranslationPostprocessor(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext,
            IMySqlOptions options)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _options = options;
        }

        public override Expression Process(Expression query)
        {
            query = base.Process(query);
            return new MySqlCompatibilityExpressionVisitor(_options).Visit(query);
        }
    }
}
