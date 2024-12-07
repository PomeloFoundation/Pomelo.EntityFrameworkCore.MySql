// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

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
            MySqlQueryCompilationContext queryCompilationContext,
            IMySqlOptions options,
            MySqlSqlExpressionFactory sqlExpressionFactory)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _options = options;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public override Expression Process(Expression query)
        {
            var mySqlHavingExpressionVisitor = new MySqlHavingExpressionVisitor(_sqlExpressionFactory);

            query = mySqlHavingExpressionVisitor.Process(query, usePrePostprocessorMode: true);

            // Changes `SelectExpression.IsMutable` from `true` to `false`.
            query = base.Process(query);

            query = mySqlHavingExpressionVisitor.Process(query, usePrePostprocessorMode: false);

            query = new MySqlJsonParameterExpressionVisitor(_sqlExpressionFactory, _options).Visit(query);

            if (_options.ServerVersion.Supports.MySqlBug96947Workaround)
            {
                query = new MySqlBug96947WorkaroundExpressionVisitor(_sqlExpressionFactory).Visit(query);
            }

            query = new BitwiseOperationReturnTypeCorrectingExpressionVisitor(_sqlExpressionFactory).Visit(query);

            return query;
        }
    }
}
