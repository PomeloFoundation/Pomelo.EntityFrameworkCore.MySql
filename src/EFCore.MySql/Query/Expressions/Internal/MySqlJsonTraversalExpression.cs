// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    /// <summary>
    /// Represents a MySQL JSON operator traversing a JSON document with a path (i.e. x->y or x->>y)
    /// </summary>
    public class MySqlJsonTraversalExpression : SqlExpression, IEquatable<MySqlJsonTraversalExpression>
    {
        /// <summary>
        /// The JSON column.
        /// </summary>
        [NotNull]
        public virtual SqlExpression Expression { get; }

        /// <summary>
        /// The path inside the JSON column.
        /// </summary>
        [NotNull]
        public virtual IReadOnlyList<SqlExpression> Path { get; }

        /// <summary>
        /// Whether the text-returning operator (x->>y) or the object-returning operator (x->y) is used.
        /// </summary>
        public virtual bool ReturnsText { get; }

        public MySqlJsonTraversalExpression(
            [NotNull] SqlExpression expression,
            bool returnsText,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping)
            : this(expression, new SqlExpression[0], returnsText, type, typeMapping)
        {
        }

        protected MySqlJsonTraversalExpression(
            [NotNull] SqlExpression expression,
            [NotNull] IReadOnlyList<SqlExpression> path,
            bool returnsText,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            if (returnsText && type != typeof(string))
                throw new ArgumentException($"{nameof(type)} must be string", nameof(type));

            Expression = expression;
            Path = path;
            ReturnsText = returnsText;
        }

        public virtual MySqlJsonTraversalExpression Clone(bool returnsText, Type type, RelationalTypeMapping typeMapping)
            => new MySqlJsonTraversalExpression(Expression, Path, returnsText, type, typeMapping);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(
                (SqlExpression)visitor.Visit(Expression),
                Path.Select(p => (SqlExpression)visitor.Visit(p)).ToArray());

        public virtual MySqlJsonTraversalExpression Update(
            [NotNull] SqlExpression expression,
            [NotNull] IReadOnlyList<SqlExpression> path)
            => expression == Expression &&
               path.Count == Path.Count &&
               path.Zip(Path, (x, y) => (x, y)).All(tup => tup.x == tup.y)
                ? this
                : new MySqlJsonTraversalExpression(expression, path, ReturnsText, Type, TypeMapping);

        public virtual MySqlJsonTraversalExpression Append([NotNull] SqlExpression pathComponent, Type returnType, RelationalTypeMapping typeMapping)
        {
            var newPath = new SqlExpression[Path.Count + 1];
            for (var i = 0; i < Path.Count; i++)
            {
                newPath[i] = Path[i];
            }
            newPath[newPath.Length - 1] = pathComponent;
            return new MySqlJsonTraversalExpression(Expression, newPath, ReturnsText, returnType.UnwrapNullableType(), typeMapping);
        }

        public override bool Equals(object obj) => Equals(obj as MySqlJsonTraversalExpression);

        public virtual bool Equals(MySqlJsonTraversalExpression other)
            => ReferenceEquals(this, other) ||
               other is object &&
               base.Equals(other) &&
               Equals(Expression, other.Expression) &&
               Path.Count == other.Path.Count &&
               Path.Zip(other.Path, (x, y) => (x, y)).All(tup => tup.x == tup.y);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Expression, Path);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Visit(Expression);
            expressionPrinter.Append(ReturnsText ? "->>" : "->");
            expressionPrinter.Append("'$");
            foreach (var location in Path)
            {
                expressionPrinter.Append(".");
                expressionPrinter.Visit(location);
            }
            expressionPrinter.Append("'");
        }

        public override string ToString() => $"{Expression}{(ReturnsText ? "->>" : "->")}{Path}";
    }
}
