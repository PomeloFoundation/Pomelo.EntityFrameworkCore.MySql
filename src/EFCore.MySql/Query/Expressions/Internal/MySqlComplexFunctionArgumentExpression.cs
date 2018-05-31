// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Utilities;
using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    public class MySqlComplexFunctionArgumentExpression : Expression
    {
        private readonly ReadOnlyCollection<Expression> _argumentParts;

        public MySqlComplexFunctionArgumentExpression(
            [NotNull] IEnumerable<Expression> argumentParts,
            [NotNull] Type argumentType)
        {
            _argumentParts = argumentParts.ToList().AsReadOnly();
            Type = argumentType;
        }

        /// <summary>
        ///     The arguments parts.
        /// </summary>
        public virtual IReadOnlyList<Expression> ArgumentParts => _argumentParts;

        /// <summary>
        ///     Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> that represents this expression.</returns>
        public override ExpressionType NodeType => ExpressionType.Extension;

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

        /// <summary>
        ///     Reduces the node and then calls the <see cref="ExpressionVisitor.Visit(Expression)" /> method passing the
        ///     reduced expression.
        ///     Throws an exception if the node isn't reducible.
        /// </summary>
        /// <param name="visitor"> An instance of <see cref="ExpressionVisitor" />. </param>
        /// <returns> The expression being visited, or an expression which should replace it in the tree. </returns>
        /// <remarks>
        ///     Override this method to provide logic to walk the node's children.
        ///     A typical implementation will call visitor.Visit on each of its
        ///     children, and if any of them change, should return a new copy of
        ///     itself with the modified children.
        /// </remarks>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newArgumentParts = visitor.VisitAndConvert(_argumentParts, nameof(VisitChildren));

            return newArgumentParts != _argumentParts
                ? new MySqlComplexFunctionArgumentExpression(newArgumentParts, Type)
                : this;
        }

        /// <summary>
        ///     Tests if this object is considered equal to another.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object. </param>
        /// <returns>
        ///     true if the objects are considered equal, false if they are not.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((MySqlComplexFunctionArgumentExpression)obj);
        }

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
