using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
    {
        private readonly IMySqlConnectionInfo _connectionInfo;

        public MySqlQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            IMySqlConnectionInfo connectionInfo,
            IModel model)
            : base(dependencies, relationalDependencies, model)
        {
            _connectionInfo = connectionInfo;
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
                .Any(t => t is CrossApplyExpression && !_connectionInfo.ServerVersion.SupportsCrossApply
                          || t is OuterApplyExpression && !_connectionInfo.ServerVersion.SupportsOuterApply))
            {
                return null;
            }

            return base.TranslateSelect(source, selector);
        }
    }
}
