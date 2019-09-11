using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    /// <summary>
    ///     An expression that explicitly specifies the collation of a string value.
    /// </summary>
    public class MySqlBinaryExpression : Expression
    {
        private readonly Expression _valueExpression;

        public MySqlBinaryExpression(
            [NotNull] Expression valueExpression)
        {
            _valueExpression = valueExpression;
        }

        /// <summary>
        ///     The expression for which a collation is being specified.
        /// </summary>
        public virtual Expression ValueExpression => _valueExpression;

        /// <summary>
        ///     Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> that represents this expression.</returns>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="Type" /> that represents the static type of the expression.</returns>
        public override Type Type => typeof(string);

        /// <summary>
        ///     Dispatches to the specific visit method for this node type.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is IMySqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitMySqlBinaryExpression(this)
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
            var newValueExpression = visitor.VisitAndConvert(_valueExpression, nameof(VisitChildren));

            return newValueExpression != _valueExpression && newValueExpression != null
                ? new MySqlBinaryExpression(newValueExpression)
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

            return obj.GetType() == GetType() && Equals((MySqlBinaryExpression)obj);
        }

        private bool Equals(MySqlBinaryExpression other)
            => _valueExpression.Equals(other._valueExpression);

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
                var hashCodes =
                    new[] {
                        _valueExpression.GetHashCode(),
                    };

                return hashCodes.Aggregate(0, (acc, hc) => (acc * 397) ^ hc);
            }
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the Expression.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the Expression.</returns>
        public override string ToString() =>
            $"BINARY {_valueExpression}";
    }
}
