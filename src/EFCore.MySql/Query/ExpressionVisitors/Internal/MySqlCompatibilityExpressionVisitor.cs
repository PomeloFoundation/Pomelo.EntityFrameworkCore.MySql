// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlCompatibilityExpressionVisitor : ExpressionVisitor
    {
        private const string Issue1790SkipFlagName = "Pomelo.EntityFrameworkCore.MySql.Issue1790.Skip";

        private static readonly bool _mySql8EngineCrashWhenUsingJsonTableSkip
            = AppContext.TryGetSwitch(Issue1790SkipFlagName, out var enabled) && enabled;

        private readonly IMySqlOptions _options;

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
                IntersectExpression intersectExpression => VisitIntercept(intersectExpression),
                JsonScalarExpression jsonScalarExpression => VisitJsonScalar(jsonScalarExpression),
                MySqlJsonTableExpression jsonTableExpression => VisitJsonTable(jsonTableExpression),

                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumberExpression)
            => CheckSupport(rowNumberExpression, _options.ServerVersion.Supports.WindowFunctions);

        protected virtual Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
            => CheckSupport(crossApplyExpression, _options.ServerVersion.Supports.CrossApply);

        protected virtual Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
            => CheckSupport(outerApplyExpression, _options.ServerVersion.Supports.OuterApply);

        protected virtual Expression VisitExcept(ExceptExpression exceptExpression)
            => CheckSupport(exceptExpression, _options.ServerVersion.Supports.ExceptIntercept);

        protected virtual Expression VisitIntercept(IntersectExpression intersectExpression)
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
                throw new InvalidOperationException($"JSON_TABLE() has been disabled by the '{Issue1790SkipFlagName}' AppContext switch, because it can crash MySQL 8.");
            }

            return jsonTableExpression;
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
