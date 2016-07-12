// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Sql.Internal
{
    public class MySqlQuerySqlGenerator : DefaultQuerySqlGenerator
    {
        protected override string ConcatOperator => "||";
        protected override string TypedTrueLiteral => "TRUE";
        protected override string TypedFalseLiteral => "FALSE";

        public MySqlQuerySqlGenerator(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper SqlGenerationHelper,
            [NotNull] IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] IRelationalTypeMapper relationalTypeMapper,
            [NotNull] SelectExpression selectExpression)
            : base(commandBuilderFactory, SqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper, selectExpression)
        {
        }

        protected override void GenerateTop([NotNull]SelectExpression selectExpression)
        {
           
        }

        public override Expression VisitTable(TableExpression tableExpression)
        {
            Check.NotNull(tableExpression, nameof(tableExpression));

            Sql.Append(SqlGenerator.DelimitIdentifier(tableExpression.Table))
                .Append(" AS ")
                .Append(SqlGenerator.DelimitIdentifier(tableExpression.Alias));

            return tableExpression;
        }


        protected override void GenerateLimitOffset([NotNull] SelectExpression selectExpression)
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
            var options = regexMatchExpression.Options;

            Visit(regexMatchExpression.Match);
            Sql.Append(" ~ ");

            // PG regexps are singleline by default
            if (options == RegexOptions.Singleline)
            {
                Visit(regexMatchExpression.Pattern);
                return regexMatchExpression;
            }

            Sql.Append("('(?");
            if (options.HasFlag(RegexOptions.IgnoreCase)) {
                Sql.Append('i');
            }

            if (options.HasFlag(RegexOptions.Multiline)) {
                Sql.Append('n');
            }
            else if (!options.HasFlag(RegexOptions.Singleline)) {
                Sql.Append('p');
            }

            if (options.HasFlag(RegexOptions.IgnorePatternWhitespace))
            {
                Sql.Append('x');
            }

            Sql.Append(")' || ");
            Visit(regexMatchExpression.Pattern);
            Sql.Append(')');
            return regexMatchExpression;
        }

        public Expression VisitAtTimeZone([NotNull] AtTimeZoneExpression atTimeZoneExpression)
        {
            Check.NotNull(atTimeZoneExpression, nameof(atTimeZoneExpression));

            //Visit(atTimeZoneExpression.TimestampExpression);

            Sql.Append("UTC_DATE()");
            return atTimeZoneExpression;
        }

        
    }
}
