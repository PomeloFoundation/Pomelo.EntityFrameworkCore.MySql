// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), shapedQueryExpression.ShaperExpression),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual SqlExpression VisitParameter(SqlParameterExpression sqlParameterExpression)
        {
            if (sqlParameterExpression.TypeMapping is MySqlJsonTypeMapping)
            {
                var typeMapping = _sqlExpressionFactory.FindMapping(sqlParameterExpression.Type, "json");

                // MySQL has a real JSON datatype, and string parameters need to be converted to it.
                // MariaDB defines the JSON datatype just as a synonym for LONGTEXT.
                if (!_options.ServerVersion.Supports.JsonDataTypeEmulation)
                {
                    return _sqlExpressionFactory.Convert(
                        sqlParameterExpression,
                        typeMapping.ClrType, // will be typeof(string) when `sqlParameterExpression.Type`
                        typeMapping);        // is typeof(MySqlJsonString)
                }
            }

            return sqlParameterExpression;
        }
    }
}
