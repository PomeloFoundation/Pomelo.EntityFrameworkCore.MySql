using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
    {
        private readonly IMySqlOptions _options;

        public MySqlQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            IMySqlOptions options,
            IModel model)
            : base(dependencies, relationalDependencies, model)
        {
            _options = options;
        }

        // There is no native support for EXCEPT in MySQL.
        protected override ShapedQueryExpression TranslateExcept(ShapedQueryExpression source1, ShapedQueryExpression source2) => null;

        // There is no native support for INTERSECT in MySQL.
        protected override ShapedQueryExpression TranslateIntersect(ShapedQueryExpression source1, ShapedQueryExpression source2) => null;

        protected override ShapedQueryExpression TranslateSelect(ShapedQueryExpression source, LambdaExpression selector)
        {
            // Trigger client eval, if there is no LATERAL support.
            // TODO: Check why this does not catch all OuterApply statements.
            if (((SelectExpression)source.QueryExpression).Tables
                .Any(t => t is CrossApplyExpression && !_options.ServerVersion.SupportsCrossApply
                          || t is OuterApplyExpression && !_options.ServerVersion.SupportsOuterApply))
            {
                return null;
            }

            return base.TranslateSelect(source, selector);
        }
    }
}
