// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    public class MySqlComplexFunctionArgumentExpression : SqlExpression
    {
        private readonly ReadOnlyCollection<Expression> _argumentParts;

        public MySqlComplexFunctionArgumentExpression(
            [NotNull] IEnumerable<Expression> argumentParts,
            [NotNull] Type argumentType)
            : base(argumentType, null)
        {
            _argumentParts = argumentParts.ToList().AsReadOnly();
            Type = argumentType;
        }

        /// <summary>
        ///     The arguments parts.
        /// </summary>
        public virtual IReadOnlyList<Expression> ArgumentParts => _argumentParts;

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="Type" /> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        ///     Dispatches to the specific visit method for this node type.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is IMySqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitMySqlComplexFunctionArgumentExpression(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newArgumentParts = visitor.VisitAndConvert(_argumentParts, nameof(VisitChildren));

            return newArgumentParts != _argumentParts
                ? new MySqlComplexFunctionArgumentExpression(newArgumentParts, Type)
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
            => Type == other.Type
               && _argumentParts.SequenceEqual(other._argumentParts);

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
                var hashCode = _argumentParts.Aggregate(0, (current, argument) => current + ((current * 397) ^ argument.GetHashCode()));
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
