// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

/// <summary>
/// Inject parameter inlining expressions where parameters are not supported for some reason.
/// </summary>
public class MySqlParameterInliningExpressionVisitor : ExpressionVisitor
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly ISqlExpressionFactory _sqlExpressionFactory;
    private readonly IMySqlOptions _options;

    private ParametersCacheDecorator _parametersDecorator;

    private bool _shouldInlineParameters;

    public MySqlParameterInliningExpressionVisitor(
        IRelationalTypeMappingSource typeMappingSource,
        ISqlExpressionFactory sqlExpressionFactory,
        IMySqlOptions options)
    {
        _typeMappingSource = typeMappingSource;
        _sqlExpressionFactory = sqlExpressionFactory;
        _options = options;
    }

    public virtual Expression Process(Expression expression, ParametersCacheDecorator parametersDecorator)
    {
        Check.NotNull(expression, nameof(expression));

        _parametersDecorator = parametersDecorator;
        _shouldInlineParameters = false;

        return Visit(expression);
    }

    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            MySqlJsonTableExpression jsonTableExpression => VisitJsonTable(jsonTableExpression),
            SelectExpression selectExpression => VisitSelect(selectExpression),
            SqlParameterExpression sqlParameterExpression => VisitSqlParameter(sqlParameterExpression),
            ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(
                Visit(shapedQueryExpression.QueryExpression),
                Visit(shapedQueryExpression.ShaperExpression)),
            _ => base.VisitExtension(extensionExpression)
        };

    protected virtual Expression VisitSelect(SelectExpression selectExpression)
    {
        // MySQL/MariaDB do not support arbitrary expressions (e.g. LEAST/GREATEST) in LIMIT/OFFSET.
        // Evaluate any non-constant, non-parameter expression in Limit/Offset client-side and replace with a constant.
        var newLimit = TrySimplifyLimitOffset(selectExpression.Limit);
        var newOffset = TrySimplifyLimitOffset(selectExpression.Offset);

        if (newLimit != selectExpression.Limit || newOffset != selectExpression.Offset)
        {
            selectExpression = selectExpression.Update(
                selectExpression.Tables,
                selectExpression.Predicate,
                selectExpression.GroupBy,
                selectExpression.Having,
                selectExpression.Projection,
                selectExpression.Orderings,
                newOffset,
                newLimit);
        }

        return NewInlineParametersScope(
            inlineParameters: false,
            () => base.VisitExtension(selectExpression));
    }

    private SqlExpression TrySimplifyLimitOffset(SqlExpression sqlExpression)
    {
        if (sqlExpression is null
            || sqlExpression is SqlConstantExpression
            || sqlExpression is SqlParameterExpression)
        {
            return sqlExpression;
        }

        // Complex expression (e.g. LEAST(@p, 1)) — evaluate client-side.
        try
        {
            var value = EvaluateExpressionClientSide(sqlExpression);
            if (value is not null)
            {
                return _sqlExpressionFactory.Constant(value, sqlExpression.TypeMapping);
            }
        }
        catch
        {
            // If we can't evaluate, leave as-is (will fail at runtime, but at least we tried).
        }

        return sqlExpression;
    }

    private object EvaluateExpressionClientSide(SqlExpression sqlExpression)
    {
        return sqlExpression switch
        {
            SqlConstantExpression c => c.Value,
            SqlParameterExpression p => _parametersDecorator.GetAndDisableCaching()[p.Name],
            SqlFunctionExpression f => EvaluateFunctionClientSide(f),
            _ => null,
        };
    }

    private object EvaluateFunctionClientSide(SqlFunctionExpression function)
    {
        if (function.Arguments is null)
        {
            return null;
        }

        var args = function.Arguments.Select(EvaluateExpressionClientSide).Where(a => a is not null).ToArray();

        return function.Name switch
        {
            "LEAST" => args.Min(),
            "GREATEST" => args.Max(),
            _ => null,
        };
    }

    // For test simplicity, we currently inline parameters even for non MySQL database engines (even though it should not be necessary
    // for e.g. MariaDB).
    // TODO: Use inlined parameters only if JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash is true.
    protected virtual Expression VisitJsonTable(MySqlJsonTableExpression jsonTableExpression)
        => jsonTableExpression.Update(
            NewInlineParametersScope(
                inlineParameters: true,
                () => (SqlExpression)Visit(jsonTableExpression.JsonExpression)),
            jsonTableExpression.Path,
            jsonTableExpression.ColumnInfos);

    protected virtual Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
    {
        if (!_shouldInlineParameters)
        {
            return sqlParameterExpression;
        }

        var parametersValues = _parametersDecorator.GetAndDisableCaching();

        return new MySqlInlinedParameterExpression(
            sqlParameterExpression,
            (SqlConstantExpression)_sqlExpressionFactory.Constant(
                parametersValues[sqlParameterExpression.Name],
                sqlParameterExpression.TypeMapping));
    }

    protected virtual T NewInlineParametersScope<T>(bool inlineParameters, Func<T> func)
    {
        var parentShouldInlineParameters = _shouldInlineParameters;
        _shouldInlineParameters = inlineParameters;

        try
        {
            return func();
        }
        finally
        {
            _shouldInlineParameters = parentShouldInlineParameters;
        }
    }
}
