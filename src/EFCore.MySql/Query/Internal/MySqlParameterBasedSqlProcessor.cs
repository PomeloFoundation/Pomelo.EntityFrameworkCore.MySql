// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
    {
        private readonly IMySqlOptions _options;

        public MySqlParameterBasedSqlProcessor(
            RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            IMySqlOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _options = options;
        }

        public override Expression Optimize(
            Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
            out bool canCache)
        {
            queryExpression = base.Optimize(queryExpression, parametersValues, out canCache);

            if (_options.ServerVersion.Supports.MySqlBugLimit0Offset0ExistsWorkaround)
            {
                queryExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Process(queryExpression, parametersValues, out var canCache2);

                canCache &= canCache2;
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                queryExpression = new MySqlBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Visit(queryExpression);
            }

            queryExpression = new MySqlHavingExpressionVisitor((MySqlSqlExpressionFactory)Dependencies.SqlExpressionFactory).Visit(queryExpression);

            queryExpression = new MySqlParameterInliningExpressionVisitor(
                Dependencies.TypeMappingSource,
                Dependencies.SqlExpressionFactory,
                _options).Process(queryExpression, parametersValues, out var canCache3);

            canCache &= canCache3;

            return queryExpression;
        }

        /// <inheritdoc />
        protected override Expression ProcessSqlNullability(
            Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
            out bool canCache)
        {
            Check.NotNull(queryExpression, nameof(queryExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            queryExpression = new MySqlSqlNullabilityProcessor(Dependencies, UseRelationalNulls)
                .Process(queryExpression, parametersValues, out canCache);

            return queryExpression;
        }
    }
}
