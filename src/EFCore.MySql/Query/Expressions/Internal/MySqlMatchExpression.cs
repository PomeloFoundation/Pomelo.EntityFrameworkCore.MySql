// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal
{
    public class MySqlMatchExpression : SqlExpression
    {
        public MySqlMatchExpression(
            SqlExpression match,
            SqlExpression against,
            MySqlMatchSearchMode searchMode,
            RelationalTypeMapping typeMapping)
            : base(typeof(bool), typeMapping)
        {
            Check.NotNull(match, nameof(match));
            Check.NotNull(against, nameof(against));

            Match = match;
            Against = against;
            SearchMode = searchMode;
        }

        public virtual MySqlMatchSearchMode SearchMode { get; }

        public virtual SqlExpression Match { get; }
        public virtual SqlExpression Against { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is MySqlQuerySqlGenerator mySqlQuerySqlGenerator // TODO: Move to VisitExtensions
                ? mySqlQuerySqlGenerator.VisitMySqlMatch(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var match = (SqlExpression)visitor.Visit(Match);
            var against = (SqlExpression)visitor.Visit(Against);

            return Update(match, against);
        }

        public virtual MySqlMatchExpression Update(SqlExpression match, SqlExpression against)
            => match != Match || against != Against
                ? new MySqlMatchExpression(
                    match,
                    against,
                    SearchMode,
                    TypeMapping)
                : this;

        public override bool Equals(object obj)
            => obj != null && ReferenceEquals(this, obj)
            || obj is MySqlMatchExpression matchExpression && Equals(matchExpression);

        private bool Equals(MySqlMatchExpression matchExpression)
            => base.Equals(matchExpression)
            && SearchMode == matchExpression.SearchMode
            && Match.Equals(matchExpression.Match)
            && Against.Equals(matchExpression.Against);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), SearchMode, Match, Against);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("MATCH ");
            expressionPrinter.Append($"({expressionPrinter.Visit(Match)})");
            expressionPrinter.Append(" AGAINST ");
            expressionPrinter.Append($"({expressionPrinter.Visit(Against)}");

            switch (SearchMode)
            {
                case MySqlMatchSearchMode.NaturalLanguage:
                    break;
                case MySqlMatchSearchMode.NaturalLanguageWithQueryExpansion:
                    expressionPrinter.Append(" WITH QUERY EXPANSION");
                    break;
                case MySqlMatchSearchMode.Boolean:
                    expressionPrinter.Append(" IN BOOLEAN MODE");
                    break;
            }

            expressionPrinter.Append(")");
        }
    }
}
