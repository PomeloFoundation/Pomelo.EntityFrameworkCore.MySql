// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microting.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Microting.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlCompatibilityExpressionVisitor : ExpressionVisitor
    {
        private const string Issue1792SkipFlagName = "Microting.EntityFrameworkCore.MySql.Issue1792.Skip";

        private static readonly bool _mySql8EngineCrashWhenUsingJsonTableSkip
            = AppContext.TryGetSwitch(Issue1792SkipFlagName, out var enabled) && enabled;

        private readonly IMySqlOptions _options;

        private SelectExpression _currentSelectExpression;
        private SelectExpression _parentSelectExpression;
        private bool _insideDeleteOrUpdate;

        private readonly MySqlContainsAggregateFunctionExpressionVisitor _mySqlContainsAggregateFunctionExpressionVisitor = new MySqlContainsAggregateFunctionExpressionVisitor();

        public MySqlCompatibilityExpressionVisitor(IMySqlOptions options)
        {
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                RowNumberExpression rowNumberExpression => VisitRowNumber(rowNumberExpression),
                CrossApplyExpression crossApplyExpression => VisitCrossApply(crossApplyExpression),
                OuterApplyExpression outerApplyExpression => VisitOuterApply(outerApplyExpression),
                ExceptExpression exceptExpression => VisitExcept(exceptExpression),
                IntersectExpression intersectExpression => VisitIntersect(intersectExpression),
                JsonScalarExpression jsonScalarExpression => VisitJsonScalar(jsonScalarExpression),
                MySqlJsonTableExpression jsonTableExpression => VisitJsonTable(jsonTableExpression),

                SelectExpression selectExpression => VisitSelect(selectExpression),
                DeleteExpression deleteExpression => VisitDelete(deleteExpression),
                UpdateExpression updateExpression => VisitUpdate(updateExpression),

                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumberExpression)
            => CheckSupport(rowNumberExpression, _options.ServerVersion.Supports.WindowFunctions);

        protected virtual Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            // When inside DELETE/UPDATE operations with databases that don't support self-referencing subqueries,
            // skip the compatibility check to allow the query to reach the database where it will throw MySqlException (Error 1093).
            // For databases that DO support self-referencing subqueries, check support normally so proper translation/handling occurs.
            if (_insideDeleteOrUpdate && !_options.ServerVersion.Supports.DeleteWithSelfReferencingSubquery)
            {
                return crossApplyExpression.Update((TableExpressionBase)Visit(crossApplyExpression.Table));
            }
            
            return CheckSupport(crossApplyExpression, _options.ServerVersion.Supports.CrossApply);
        }

        protected virtual Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            // When inside DELETE/UPDATE operations with databases that don't support self-referencing subqueries,
            // skip the compatibility check to allow the query to reach the database where it will throw MySqlException (Error 1093).
            // For databases that DO support self-referencing subqueries, check support normally so proper translation/handling occurs.
            if (_insideDeleteOrUpdate && !_options.ServerVersion.Supports.DeleteWithSelfReferencingSubquery)
            {
                return outerApplyExpression.Update((TableExpressionBase)Visit(outerApplyExpression.Table));
            }
            
            return CheckSupport(outerApplyExpression, _options.ServerVersion.Supports.OuterApply);
        }

        protected virtual Expression VisitExcept(ExceptExpression exceptExpression)
            => CheckSupport(exceptExpression, _options.ServerVersion.Supports.ExceptIntercept);

        protected virtual Expression VisitIntersect(IntersectExpression intersectExpression)
            => CheckSupport(intersectExpression, _options.ServerVersion.Supports.ExceptIntercept);

        protected virtual Expression VisitJsonScalar(JsonScalarExpression jsonScalarExpression)
            => CheckSupport(jsonScalarExpression, _options.ServerVersion.Supports.Json);

        protected virtual Expression VisitJsonTable(MySqlJsonTableExpression jsonTableExpression)
        {
            if (!_options.ServerVersion.Supports.JsonTable)
            {
                return CheckSupport(jsonTableExpression, false);
            }

            // Using primitive collections in parameters that are used as the JSON source argument for JSON_TABLE(source, ...) can crash
            // MySQL 8 somewhere later down the line. We mitigate this by inlining those parameters.
            // There are however other scenarios that can still crash MySQL 8 (e.g. `NorthwindSelectQueryMySqlTest.Correlated_collection_after_distinct_not_containing_original_identifier`).
            // For those cases, we implement a flag to skip skip the JSON_TABLE generation.
            if (!_options.ServerVersion.Supports.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash &&
                _mySql8EngineCrashWhenUsingJsonTableSkip)
            {
                throw new InvalidOperationException($"JSON_TABLE() has been disabled by the '{Issue1792SkipFlagName}' AppContext switch, because it can crash MySQL 8.");
            }

            if (!_options.ServerVersion.Supports.JsonTableImplementationWithAggregate &&
                _mySqlContainsAggregateFunctionExpressionVisitor.ProcessSelect(_currentSelectExpression))
            {
                throw new InvalidOperationException($"JSON_TABLE() does not support aggregates on {_options.ServerVersion} and would return unexpected results if used.");
            }

            if (!_options.ServerVersion.Supports.OuterApply &&
                jsonTableExpression.JsonExpression is ColumnExpression columnExpression &&
                _parentSelectExpression is not null &&
                _parentSelectExpression.Tables.All(t => t.Alias != columnExpression.TableAlias))
            {
                throw new InvalidOperationException($"JSON_TABLE() does not support references to an outer query that is not the immediate parent on {_options.ServerVersion}.");
            }

            return jsonTableExpression;
        }

        protected virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            var grandParentSelectExpression = _parentSelectExpression;
            _parentSelectExpression = _currentSelectExpression;
            _currentSelectExpression = selectExpression;

            foreach (var item in selectExpression.Projection)
            {
                Visit(item);
            }

            foreach (var table in selectExpression.Tables)
            {
                Visit(table);
            }

            Visit(selectExpression.Predicate);

            foreach (var groupingKey in selectExpression.GroupBy)
            {
                Visit(groupingKey);
            }

            Visit(selectExpression.Having);

            foreach (var ordering in selectExpression.Orderings)
            {
                Visit(ordering.Expression);
            }

            Visit(selectExpression.Offset);
            Visit(selectExpression.Limit);

            _currentSelectExpression = _parentSelectExpression;
            _parentSelectExpression = grandParentSelectExpression;

            return selectExpression;
        }

        protected virtual Expression VisitDelete(DeleteExpression deleteExpression)
            => VisitNonQueryExpression(deleteExpression);

        protected virtual Expression VisitUpdate(UpdateExpression updateExpression)
            => VisitNonQueryExpression(updateExpression);

        private Expression VisitNonQueryExpression(Expression expression)
        {
            var previousInsideDeleteOrUpdate = _insideDeleteOrUpdate;
            _insideDeleteOrUpdate = true;
            var result = base.VisitExtension(expression);
            _insideDeleteOrUpdate = previousInsideDeleteOrUpdate;
            return result;
        }

        protected virtual Expression CheckSupport(Expression expression, bool isSupported)
            => CheckTranslated(
                isSupported
                    ? base.VisitExtension(expression)
                    : null,
                expression);

        protected virtual Expression CheckTranslated(Expression translated, Expression original)
        {
            if (translated == null)
            {
                throw new InvalidOperationException(
                    CoreStrings.TranslationFailed(original.Print()));
            }

            return translated;
        }
    }
}
