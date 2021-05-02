// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    /// <summary>
    /// Represents a MySQL JSON array index (i.e. x[y]).
    /// </summary>
    public class MySqlJsonArrayIndexExpression : SqlExpression, IEquatable<MySqlJsonArrayIndexExpression>
    {
        [NotNull]
        public virtual SqlExpression Expression { get; }

        public MySqlJsonArrayIndexExpression(
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Expression = expression;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update((SqlExpression)visitor.Visit(Expression));

        public virtual MySqlJsonArrayIndexExpression Update(
            [NotNull] SqlExpression expression)
            => expression == Expression
                ? this
                : new MySqlJsonArrayIndexExpression(expression, Type, TypeMapping);

        public override bool Equals(object obj)
            => Equals(obj as MySqlJsonArrayIndexExpression);

        public virtual bool Equals(MySqlJsonArrayIndexExpression other)
            => ReferenceEquals(this, other) ||
               other != null &&
               base.Equals(other) &&
               Equals(Expression, other.Expression);

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Expression);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("[");
            expressionPrinter.Visit(Expression);
            expressionPrinter.Append("]");
        }

        public override string ToString()
            => $"[{Expression}]";

        public virtual SqlExpression ApplyTypeMapping(RelationalTypeMapping typeMapping)
            => new MySqlJsonArrayIndexExpression(Expression, Type, typeMapping);
    }
}
