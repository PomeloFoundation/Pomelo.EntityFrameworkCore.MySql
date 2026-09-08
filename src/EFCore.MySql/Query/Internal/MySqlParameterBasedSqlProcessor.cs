// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
    {
        private readonly IMySqlOptions _options;

        public MySqlParameterBasedSqlProcessor(
            RelationalParameterBasedSqlProcessorDependencies dependencies,
            RelationalParameterBasedSqlProcessorParameters parameters,
            IMySqlOptions options)
            : base(dependencies, parameters)
        {
            _options = options;
        }

        /// <inheritdoc />
        protected override Expression ProcessSqlNullability(
            Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
            out bool canCache)
        {
            var nullabilityProcessor = new MySqlSqlNullabilityProcessor(Dependencies, Parameters);
            return nullabilityProcessor.Process(queryExpression, parametersValues, out canCache);
        }

        /// <inheritdoc />
        public override Expression Optimize(
            Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
            out bool canCache)
        {
            // Let the base class handle standard EF Core processing.
            // ProcessSqlNullability (overridden above) is called by base.Optimize internally,
            // so our MySqlSqlNullabilityProcessor handles all Pomelo custom expression types.
            queryExpression = base.Optimize(queryExpression, parametersValues, out canCache);

            // Then apply MySQL-specific optimizations.
            queryExpression = ApplyMySqlOptimizations(queryExpression, parametersValues, out var mysqlCanCache);
            canCache &= mysqlCanCache;

            return queryExpression;
        }

        internal Expression ApplyMySqlOptimizations(
            Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
            out bool canCache)
        {
            canCache = true;

            if (_options.ServerVersion.Supports.MySqlBugLimit0Offset0ExistsWorkaround)
            {
                queryExpression = new SkipTakeCollapsingExpressionVisitor(
                    Dependencies.SqlExpressionFactory)
                    .Process(queryExpression, parametersValues, out var canCache2);
                canCache &= canCache2;
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                queryExpression = new MySqlBoolOptimizingExpressionVisitor(
                    Dependencies.SqlExpressionFactory)
                    .Visit(queryExpression);
            }

            queryExpression = new MySqlParameterInliningExpressionVisitor(
                Dependencies.TypeMappingSource,
                Dependencies.SqlExpressionFactory,
                _options).Process(queryExpression, parametersValues, out var canCache3);
            canCache &= canCache3;

            // Compatibility checks run late in the query pipeline.
            queryExpression = new MySqlCompatibilityExpressionVisitor(_options)
                .Visit(queryExpression);

            return queryExpression;
        }
    }
}
