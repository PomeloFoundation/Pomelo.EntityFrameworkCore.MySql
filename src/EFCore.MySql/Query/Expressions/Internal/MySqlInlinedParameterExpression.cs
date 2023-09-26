// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

public class MySqlInlinedParameterExpression : SqlExpression
{
    public MySqlInlinedParameterExpression(
        SqlParameterExpression parameterExpression,
        SqlConstantExpression valueExpression)
        : base(parameterExpression.Type, parameterExpression.TypeMapping)
    {
        Check.NotNull(parameterExpression, nameof(parameterExpression));

        ParameterExpression = parameterExpression;
        ValueExpression = valueExpression;
    }

    public virtual Expression ParameterExpression { get; }
    public virtual SqlConstantExpression ValueExpression { get; }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var parameterExpression = (SqlParameterExpression)visitor.Visit(ParameterExpression);
        var valueExpression = (SqlConstantExpression)visitor.Visit(ValueExpression);

        return Update(parameterExpression, valueExpression);
    }

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
