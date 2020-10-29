// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
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
        private readonly SqlNullabilityProcessor _sqlNullabilityProcessor;

        public MySqlParameterBasedSqlProcessor(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            IMySqlOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _options = options;
            _sqlNullabilityProcessor = new MySqlSqlNullabilityProcessor(dependencies, useRelationalNulls);
        }

        /// <inheritdoc />
        protected override SelectExpression ProcessSqlNullability(
            SelectExpression selectExpression, IReadOnlyDictionary<string, object> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = _sqlNullabilityProcessor.Process(selectExpression, parametersValues, out canCache);

            if (_options.IndexOptimizedBooleanColumns)
            {
                selectExpression = (SelectExpression)new MySqlBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(selectExpression);
            }

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            selectExpression = (SelectExpression)new MySqlCompatibilityExpressionVisitor(_options).Visit(selectExpression);

            return selectExpression;
        }
    }
}
