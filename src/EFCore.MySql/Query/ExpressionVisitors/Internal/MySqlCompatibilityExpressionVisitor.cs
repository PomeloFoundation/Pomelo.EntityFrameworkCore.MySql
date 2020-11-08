// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlCompatibilityExpressionVisitor : ExpressionVisitor
    {
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
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), shapedQueryExpression.ShaperExpression),
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
