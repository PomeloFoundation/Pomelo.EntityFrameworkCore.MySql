// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    public class MySqlComplexFunctionArgumentExpression : SqlExpression
    {
        public MySqlComplexFunctionArgumentExpression(
            IEnumerable<SqlExpression> argumentParts,
            Type type,
            RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            ArgumentParts = argumentParts.ToList().AsReadOnly();
        }

        /// <summary>
        ///     The arguments parts.
        /// </summary>
        public virtual IReadOnlyList<SqlExpression> ArgumentParts { get; }

        /// <summary>
        ///     Dispatches to the specific visit method for this node type.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor) =>
            visitor is MySqlQuerySqlGenerator mySqlQuerySqlGenerator
                ? mySqlQuerySqlGenerator.VisitMySqlComplexFunctionArgumentExpression(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var changed = false;
            var newArgumentParts = new SqlExpression[ArgumentParts.Count];

            for (var i = 0; i < newArgumentParts.Length; i++)
            {
                newArgumentParts[i] = (SqlExpression) visitor.Visit(ArgumentParts[i]);
                changed |= newArgumentParts[i] != ArgumentParts[i];
            }
            
            return changed
                ? new MySqlComplexFunctionArgumentExpression(
                    newArgumentParts,
                    Type,
                    TypeMapping)
                : this;
        }

        public override void Print(ExpressionPrinter expressionPrinter)
            => expressionPrinter.Append(ToString());

        public override bool Equals(object obj)
            => obj != null
            && (ReferenceEquals(this, obj)
                || obj is MySqlComplexFunctionArgumentExpression sqlFragmentExpression
                    && Equals(sqlFragmentExpression));

        private bool Equals(MySqlComplexFunctionArgumentExpression other)
            => base.Equals(other)
               && Type == other.Type
               && ArgumentParts.SequenceEqual(other.ArgumentParts);

        /// <summary>
        ///     Returns a hash code for this object.
        /// </summary>
        /// <returns>
        ///     A hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ArgumentParts.Aggregate(0, (current, argument) => current + ((current * 397) ^ argument.GetHashCode()));
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the Expression.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the Expression.</returns>
        public override string ToString()
            => string.Join(" ", ArgumentParts);
    }
}
