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
            RelationalParameterBasedSqlProcessorParameters parameters,
            IMySqlOptions options)
            : base(dependencies, parameters)
        {
            _options = options;
        }

        public override Expression Process(
            Expression queryExpression,
            ParametersCacheDecorator parametersDecorator)
        {
            queryExpression = base.Process(queryExpression, parametersDecorator);

            if (_options.ServerVersion.Supports.MySqlBugLimit0Offset0ExistsWorkaround)
            {
                queryExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Process(queryExpression, parametersDecorator);
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                queryExpression = new MySqlBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Visit(queryExpression);
            }

            queryExpression = new MySqlParameterInliningExpressionVisitor(
                Dependencies.TypeMappingSource,
                Dependencies.SqlExpressionFactory,
                _options).Process(queryExpression, parametersDecorator);

            // MySQL/MariaDB do not support expressions (e.g. LEAST/GREATEST, which EF Core 10 can generate when combining
            // nested Skip/Take) in the LIMIT/OFFSET clauses. Evaluate them to a constant now that parameter values are known.
            queryExpression = new MySqlLimitOffsetInliningExpressionVisitor(Dependencies.SqlExpressionFactory)
                .Process(queryExpression, parametersDecorator);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            queryExpression = new MySqlCompatibilityExpressionVisitor(_options).Visit(queryExpression);

            return queryExpression;
        }

        /// <inheritdoc />
        protected override Expression ProcessSqlNullability(
            Expression queryExpression,
            ParametersCacheDecorator parametersDecorator)
        {
            Check.NotNull(queryExpression, nameof(queryExpression));
            Check.NotNull(parametersDecorator, nameof(parametersDecorator));

            queryExpression = new MySqlSqlNullabilityProcessor(Dependencies, Parameters)
                .Process(queryExpression, parametersDecorator);

            return queryExpression;
        }
    }
}
