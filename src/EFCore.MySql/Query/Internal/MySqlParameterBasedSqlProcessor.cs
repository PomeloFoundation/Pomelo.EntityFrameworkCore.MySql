// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

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
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlParameterBasedSqlProcessor(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            IMySqlOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)Dependencies.SqlExpressionFactory;
            _options = options;
        }

        public override SelectExpression Optimize(SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = base.Optimize(selectExpression, parametersValues, out canCache);

            if (_options.ServerVersion.Supports.MySqlBugLimit0Offset0ExistsWorkaround)
            {
                selectExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Process(selectExpression, parametersValues, out var canCache2);

                canCache &= canCache2;
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                selectExpression = (SelectExpression)new MySqlBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(selectExpression);
            }

            selectExpression = (SelectExpression)new MySqlHavingExpressionVisitor(_sqlExpressionFactory).Visit(selectExpression);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            selectExpression = (SelectExpression)new MySqlCompatibilityExpressionVisitor(_options).Visit(selectExpression);

            return selectExpression;
        }

        /// <inheritdoc />
        protected override SelectExpression ProcessSqlNullability(
            SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = new MySqlSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(selectExpression, parametersValues, out canCache);

            return selectExpression;
        }
    }
}
