using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlJsonParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly RelationalTypeMapping _jsonTypeMapping;

        public MySqlJsonParameterExpressionVisitor(MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _jsonTypeMapping = _sqlExpressionFactory.FindMapping("json");
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SqlParameterExpression sqlParameterExpression => VisitParameter(sqlParameterExpression),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual SqlExpression VisitParameter(SqlParameterExpression sqlParameterExpression)
            => sqlParameterExpression.TypeMapping?.StoreType == "json"
                ? (SqlExpression)_sqlExpressionFactory.Convert(
                    sqlParameterExpression,
                    sqlParameterExpression.Type,
                    _jsonTypeMapping)
                : sqlParameterExpression;
    }
}
