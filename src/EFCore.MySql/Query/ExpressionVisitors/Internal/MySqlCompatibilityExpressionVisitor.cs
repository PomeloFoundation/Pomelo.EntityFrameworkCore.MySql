using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlCompatibilityExpressionVisitor : ExpressionVisitor
    {
        private readonly IMySqlConnectionInfo _connectionInfo;

        public MySqlCompatibilityExpressionVisitor(IMySqlConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            switch (extensionExpression)
            {
                case RowNumberExpression rowNumberExpression:
                    return VisitRowNumber(rowNumberExpression);

                case CrossApplyExpression crossApplyExpression:
                    return VisitCrossApply(crossApplyExpression);

                case OuterApplyExpression outerApplyExpression:
                    return VisitOuterApply(outerApplyExpression);

                case ExceptExpression exceptExpression:
                    return VisitExcept(exceptExpression);

                case IntersectExpression intersectExpression:
                    return VisitIntercept(intersectExpression);
            }

            return base.VisitExtension(extensionExpression);
        }

        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumberExpression)
            => CheckSupport(rowNumberExpression, _connectionInfo.ServerVersion.SupportsWindowFunctions);

        protected virtual Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
            => CheckSupport(crossApplyExpression, _connectionInfo.ServerVersion.SupportsCrossApply);
        
        protected virtual Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
            => CheckSupport(outerApplyExpression, _connectionInfo.ServerVersion.SupportsOuterApply);

        protected virtual Expression VisitExcept(ExceptExpression exceptExpression)
            => CheckSupport(exceptExpression, false);

        protected virtual Expression VisitIntercept(IntersectExpression intersectExpression)
            => CheckSupport(intersectExpression, false);

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
