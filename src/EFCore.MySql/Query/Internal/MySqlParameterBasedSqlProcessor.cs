// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System;
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
            ParametersCacheDecorator parametersDecorator)
        {
            var nullabilityProcessor = new MySqlSqlNullabilityProcessor(Dependencies, Parameters);
            return nullabilityProcessor.Process(queryExpression, parametersDecorator);
        }

        /// <summary>
        /// EF Core 10 replaced the obsolete <c>Optimize(...)</c> extension point with this
        /// <c>Process(...)</c> overload. The standard EF Core processing runs first
        /// (ProcessSqlNullability — overridden above to use MySqlSqlNullabilityProcessor —
        /// followed by ExpandFromSqlParameter), then the MySQL-specific optimizations are applied.
        /// </summary>
        /// <remarks>
        /// Caching semantics are fully retained compared to the previous EF Core 9 based
        /// implementation: the optimization visitors below invoke
        /// <see cref="ParametersCacheDecorator.GetAndDisableCaching"/> only in the exact same cases
        /// in which the old implementation set <c>canCache = false</c>, namely when a parameterized
        /// LIMIT/OFFSET is used (SkipTakeCollapsingExpressionVisitor) or when parameters are actually
        /// inlined (MySqlParameterInliningExpressionVisitor, JSON_TABLE scenarios).
        /// In all other cases the query remains cacheable.
        /// </remarks>
        public override Expression Process(
            Expression queryExpression,
            ParametersCacheDecorator parametersDecorator)
        {
            queryExpression = base.Process(queryExpression, parametersDecorator);

            return ApplyMySqlOptimizations(queryExpression, parametersDecorator);
        }

        internal Expression ApplyMySqlOptimizations(
            Expression queryExpression,
            ParametersCacheDecorator parametersDecorator)
        {
            if (_options.ServerVersion.Supports.MySqlBugLimit0Offset0ExistsWorkaround)
            {
                queryExpression = new SkipTakeCollapsingExpressionVisitor(
                    Dependencies.SqlExpressionFactory)
                    .Process(queryExpression, parametersDecorator);
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
                _options).Process(queryExpression, parametersDecorator);

            // Compatibility checks run late in the query pipeline.
            queryExpression = new MySqlCompatibilityExpressionVisitor(_options)
                .Visit(queryExpression);

            return queryExpression;
        }
    }
}
