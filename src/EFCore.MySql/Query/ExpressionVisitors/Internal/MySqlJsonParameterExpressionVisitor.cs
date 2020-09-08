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

            if (sqlParameterExpression.TypeMapping is MySqlJsonTypeMapping)
            {
                var typeMapping = _sqlExpressionFactory.FindMapping(expression.Type, "json");

                // MySQL has a real JSON datatype, and string parameters need to be converted to it.
                // MariaDB defines the JSON datatype just as a synonym for LONGTEXT.
                if (!_options.ServerVersion.SupportsJsonDataTypeEmulation)
                {
                    expression = _sqlExpressionFactory.Convert(
                        sqlParameterExpression,
                        sqlParameterExpression.Type,
                        typeMapping);
                }
            }

            return expression;
        }
    }
}
