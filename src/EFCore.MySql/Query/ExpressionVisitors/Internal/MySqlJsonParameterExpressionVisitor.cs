using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlJsonParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;

        public MySqlJsonParameterExpressionVisitor(MySqlSqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SqlParameterExpression sqlParameterExpression => VisitParameter(sqlParameterExpression),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual SqlExpression VisitParameter(SqlParameterExpression sqlParameterExpression)
        {
            SqlExpression expression = sqlParameterExpression;

            if (expression.TypeMapping is MySqlJsonTypeMapping)
            {
                var typeMapping = _sqlExpressionFactory.FindMapping(expression.Type, "json");

                if (_options.ServerVersion.SupportsJsonDataTypeEmulation)
                {
                    // expression = _sqlExpressionFactory.Function(
                    //     "JSON_COMPACT",
                    //     new[] {expression},
                    //     expression.Type,
                    //     typeMapping);
                }
                else
                {
                    expression = _sqlExpressionFactory.Convert(
                        expression,
                        expression.Type,
                        typeMapping);
                }
            }

            return expression;
        }
    }
}
