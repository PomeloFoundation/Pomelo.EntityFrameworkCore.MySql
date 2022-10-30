﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
    {
        private readonly IMySqlOptions _options;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlParameterBasedSqlProcessor(
            RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            IMySqlOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)Dependencies.SqlExpressionFactory;
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
                queryExpression = (SelectExpression)new MySqlBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Visit(queryExpression);
            }

            queryExpression = (SelectExpression)new MySqlHavingExpressionVisitor(_sqlExpressionFactory).Visit(queryExpression);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            queryExpression = (SelectExpression)new MySqlCompatibilityExpressionVisitor(_options).Visit(queryExpression);

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
