// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    public enum MySqlBinaryExpressionOperatorType
    {
        /// <summary>
        /// TODO
        /// </summary>
        IntegerDivision,

        /// <summary>
        /// Use to force an equals expression, that will not be optimized by EF Core.
        /// Can be used, to force a `value = TRUE` expression.
        /// </summary>
        NonOptimizedEqual,
    }

    public class MySqlBinaryExpression : SqlExpression
    {
        public MySqlBinaryExpression(
            MySqlBinaryExpressionOperatorType operatorType,
            SqlExpression left,
            SqlExpression right,
            Type type,
            RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            OperatorType = operatorType;

            Left = left;
            Right = right;
        }

        public virtual MySqlBinaryExpressionOperatorType OperatorType { get; }
        public virtual SqlExpression Left { get; }
        public virtual SqlExpression Right { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is MySqlQuerySqlGenerator mySqlQuerySqlGenerator // TODO: Move to VisitExtensions
                ? mySqlQuerySqlGenerator.VisitMySqlBinaryExpression(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var left = (SqlExpression)visitor.Visit(Left);
            var right = (SqlExpression)visitor.Visit(Right);

            return Update(left, right);
        }

        public virtual MySqlBinaryExpression Update(SqlExpression left, SqlExpression right)
            => left != Left || right != Right
                ? new MySqlBinaryExpression(OperatorType, left, right, Type, TypeMapping)
                : this;

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            var requiresBrackets = RequiresBrackets(Left);

            if (requiresBrackets)
            {
                expressionPrinter.Append("(");
            }

            expressionPrinter.Visit(Left);

            if (requiresBrackets)
            {
                expressionPrinter.Append(")");
            }

            switch (OperatorType)
            {
                case MySqlBinaryExpressionOperatorType.IntegerDivision:
                    expressionPrinter.Append(" DIV ");
                    break;
                case MySqlBinaryExpressionOperatorType.NonOptimizedEqual:
                    expressionPrinter.Append(" = ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            requiresBrackets = RequiresBrackets(Right);

            if (requiresBrackets)
            {
                expressionPrinter.Append("(");
            }

            expressionPrinter.Visit(Right);

            if (requiresBrackets)
            {
                expressionPrinter.Append(")");
            }
        }

        private bool RequiresBrackets(SqlExpression expression)
        {
            return expression is SqlBinaryExpression sqlBinary
                && sqlBinary.OperatorType != ExpressionType.Coalesce
                || expression is LikeExpression;
        }

        public override bool Equals(object obj)
            => obj != null
            && (ReferenceEquals(this, obj)
                || obj is MySqlBinaryExpression sqlBinaryExpression
                    && Equals(sqlBinaryExpression));

        private bool Equals(MySqlBinaryExpression sqlBinaryExpression)
            => base.Equals(sqlBinaryExpression)
            && OperatorType == sqlBinaryExpression.OperatorType
            && Left.Equals(sqlBinaryExpression.Left)
            && Right.Equals(sqlBinaryExpression.Right);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), OperatorType, Left, Right);
    }
}
