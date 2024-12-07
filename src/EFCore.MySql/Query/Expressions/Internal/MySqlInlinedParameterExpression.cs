// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

public class MySqlInlinedParameterExpression : SqlExpression
{
    private static ConstructorInfo _quotingConstructor;

    public MySqlInlinedParameterExpression(
        SqlParameterExpression parameterExpression,
        SqlConstantExpression valueExpression)
        : base(parameterExpression.Type, parameterExpression.TypeMapping)
    {
        Check.NotNull(parameterExpression, nameof(parameterExpression));

        ParameterExpression = parameterExpression;
        ValueExpression = valueExpression;
    }

    public virtual SqlParameterExpression ParameterExpression { get; }
    public virtual SqlConstantExpression ValueExpression { get; }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var parameterExpression = (SqlParameterExpression)visitor.Visit(ParameterExpression);
        var valueExpression = (SqlConstantExpression)visitor.Visit(ValueExpression);

        return Update(parameterExpression, valueExpression);
    }

    /// <inheritdoc />
    public override Expression Quote()
        => New(
            _quotingConstructor ??= typeof(MySqlInlinedParameterExpression).GetConstructor(
                [typeof(SqlParameterExpression), typeof(SqlConstantExpression)])!,
            ParameterExpression.Quote(),
            ValueExpression.Quote());

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Visit(ValueExpression);
        expressionPrinter.Append(" (<<== ");
        expressionPrinter.Visit(ParameterExpression);
        expressionPrinter.Append(")");
    }

    public virtual MySqlInlinedParameterExpression Update(SqlParameterExpression parameterExpression, SqlConstantExpression valueExpression)
        => parameterExpression != ParameterExpression || valueExpression != ValueExpression
            ? new MySqlInlinedParameterExpression(parameterExpression, valueExpression)
            : this;

    public override bool Equals(object obj)
        => obj != null
           && (ReferenceEquals(this, obj)
               || obj is MySqlInlinedParameterExpression inlinedParameterExpression
               && Equals(inlinedParameterExpression));

    private bool Equals(MySqlInlinedParameterExpression inlinedParameterExpression)
        => base.Equals(inlinedParameterExpression)
           && ParameterExpression.Equals(inlinedParameterExpression.ParameterExpression)
           && ValueExpression.Equals(inlinedParameterExpression.ValueExpression);

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), ParameterExpression, ValueExpression);
}
