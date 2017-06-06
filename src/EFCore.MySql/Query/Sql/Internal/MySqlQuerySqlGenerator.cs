// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Sql.Internal
{
    public class MySqlQuerySqlGenerator : DefaultQuerySqlGenerator
    {
        protected override string ConcatOperator => "||";
        protected override string TypedTrueLiteral => "TRUE";
        protected override string TypedFalseLiteral => "FALSE";

        private static FieldInfo _relationalCommandBuilderFieldInfo = typeof(DefaultQuerySqlGenerator).GetTypeInfo().DeclaredFields.Single(x => x.Name == "_relationalCommandBuilder");

        public MySqlQuerySqlGenerator(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper,
            [NotNull] IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] IRelationalTypeMapper relationalTypeMapper,
            [NotNull] SelectExpression selectExpression)
            : base(commandBuilderFactory, sqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper, selectExpression)
        {
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
        }

        private static string[] SqlFuncAInB = new[] { "POSITION" };

        public override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (!SqlFuncAInB.Contains(sqlFunctionExpression.FunctionName))
                return base.VisitSqlFunction(sqlFunctionExpression);

            var  _relationalCommandBuilder = (IRelationalCommandBuilder)_relationalCommandBuilderFieldInfo.GetValue(this);
            _relationalCommandBuilder.Append(sqlFunctionExpression.FunctionName);
            _relationalCommandBuilder.Append("(");

            VisitAInB(sqlFunctionExpression.Arguments.ToList());

            _relationalCommandBuilder.Append(")");

            return sqlFunctionExpression;
        }

        private void VisitAInB(
            IReadOnlyList<Expression> expressions, Action<IRelationalCommandBuilder> joinAction = null)
            => VisitAInB(expressions, e => Visit(e), joinAction);

        private void VisitAInB<T>(
            IReadOnlyList<T> items, Action<T> itemAction, Action<IRelationalCommandBuilder> joinAction = null)
        {
            if (items.Count != 2)
                throw new ArgumentException("Argument count must be 2.");

            joinAction = joinAction ?? (isb => isb.Append(" IN "));

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    var _relationalCommandBuilder = (IRelationalCommandBuilder)_relationalCommandBuilderFieldInfo.GetValue(this);
                    joinAction(_relationalCommandBuilder);
                }

                itemAction(items[i]);
            }
        }

        public override Expression VisitTable(TableExpression tableExpression)
        {
            Check.NotNull(tableExpression, nameof(tableExpression));

            Sql.Append(SqlGenerator.DelimitIdentifier(tableExpression.Table, tableExpression.Schema))
                .Append(" AS ")
                .Append(SqlGenerator.DelimitIdentifier(tableExpression.Alias));

            return tableExpression;
        }


        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("LIMIT ");
                Visit(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                if (selectExpression.Limit == null)
                {
                    // if we want to use Skip() without Take() we have to define the upper limit of LIMIT 
                    Sql.AppendLine().Append("LIMIT ").Append(18446744073709551610);
                }
                Sql.Append(' ');
                Sql.Append("OFFSET ");
                Visit(selectExpression.Offset);
            }
        }

        public override Expression VisitCount(CountExpression countExpression)
        {
            Check.NotNull(countExpression, nameof(countExpression));

            if (countExpression.Type == typeof(long))
            {
                Sql.Append("COUNT(*)");
            }
            else if (countExpression.Type == typeof(int))
            {
                Sql.Append("CAST(COUNT(*) AS UNSIGNED)");
            }
            else throw new NotSupportedException($"Count expression with type {countExpression.Type} not supported");

            return countExpression;
        }

        public override Expression VisitSum(SumExpression sumExpression)
        {
            base.VisitSum(sumExpression);
            return sumExpression;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType == ExpressionType.Add &&
                binaryExpression.Left.Type == typeof (string) &&
                binaryExpression.Right.Type == typeof (string))
            {
                Sql.Append("CONCAT(");
                //var exp = base.VisitBinary(binaryExpression);
                Visit(binaryExpression.Left);
                Sql.Append(",");
                var exp = Visit(binaryExpression.Right);
                Sql.Append(")");
                return exp;
            }
            
            var expr = base.VisitBinary(binaryExpression);
            
            return expr;
        }
        
        public Expression VisitRegexMatch([NotNull] RegexMatchExpression regexMatchExpression)
        {
            Check.NotNull(regexMatchExpression, nameof(regexMatchExpression));

            Visit(regexMatchExpression.Match);
            Sql.Append(" RLIKE ");
            Visit(regexMatchExpression.Pattern);

            return regexMatchExpression;
        }

        public Expression VisitAtTimeZone([NotNull] AtTimeZoneExpression atTimeZoneExpression)
        {
            Check.NotNull(atTimeZoneExpression, nameof(atTimeZoneExpression));

            //Visit(atTimeZoneExpression.TimestampExpression);

            Sql.Append("UTC_TIMESTAMP()");
            return atTimeZoneExpression;
        }

        public Expression VisitDateAdd([NotNull] DateAddExpression dateAddExpression)
        {
            Check.NotNull(dateAddExpression, nameof(dateAddExpression));

            Sql.Append("DATE_ADD(");
            Visit(dateAddExpression.Arguments.First());

            Sql.Append(", INTERVAL ");

            Visit(dateAddExpression.Arguments.Last());
            Sql.Append($" {dateAddExpression.DatePart})");

            return dateAddExpression;
        }

        public Expression VisitDatePart([NotNull] DatePartExpression datePartExpression)
        {
            Check.NotNull(datePartExpression, nameof(datePartExpression));

            Sql.Append($"EXTRACT({datePartExpression.DatePart} FROM ");

            Visit(datePartExpression.Argument);
            Sql.Append(")");

            return datePartExpression;
        }
    }
}
