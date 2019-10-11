using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    /// <summary>
    ///     An expression that explicitly specifies the collation of a string value.
    /// </summary>
    public class MySqlCollateExpression : SqlExpression
    {
        private readonly SqlExpression _valueExpression;
        private readonly string _charset;
        private readonly string _collation;

        public MySqlCollateExpression(
            [NotNull] SqlExpression valueExpression,
            [NotNull] string charset,
            [NotNull] string collation,
            RelationalTypeMapping typeMapping)
            : base(typeof(string), typeMapping)
        {
            _valueExpression = valueExpression;
            _charset = charset;
            _collation = collation;
        }

        /// <summary>
        ///     The expression for which a collation is being specified.
        /// </summary>
        public virtual SqlExpression ValueExpression => _valueExpression;

        /// <summary>
        ///     The character set that the string is being converted to.
        /// </summary>
        public virtual string Charset => _charset;

        /// <summary>
        ///     The collation that the string is being converted to.
        /// </summary>
        public virtual string Collation => _collation;

        /// <summary>
        ///     Dispatches to the specific visit method for this node type.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is MySqlQuerySqlGenerator mySqlQuerySqlGenerator
                ? mySqlQuerySqlGenerator.VisitMySqlCollateExpression(this)
                : base.Accept(visitor);

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
                ? new MySqlCollateExpression(newValueExpression, _charset, _collation, TypeMapping)
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

            return obj.GetType() == GetType() && Equals((MySqlCollateExpression)obj);
        }

        private bool Equals(MySqlCollateExpression other)
            => string.Equals(_charset, other._charset, StringComparison.OrdinalIgnoreCase)
               && string.Equals(_collation, other._collation, StringComparison.OrdinalIgnoreCase)
               && _valueExpression.Equals(other._valueExpression);

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
                        StringComparer.OrdinalIgnoreCase.GetHashCode(_charset),
                        StringComparer.OrdinalIgnoreCase.GetHashCode(_collation)
                    };

                return hashCodes.Aggregate(0, (acc, hc) => (acc * 397) ^ hc);
            }
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the Expression.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the Expression.</returns>
        public override string ToString() =>
            $"{_valueExpression} COLLATE {_collation}";

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append(ToString()); // TODO: ist this correct?
        }
    }
}
