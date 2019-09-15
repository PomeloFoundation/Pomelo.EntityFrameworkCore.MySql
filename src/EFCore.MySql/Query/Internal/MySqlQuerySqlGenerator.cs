// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlQuerySqlGenerator : QuerySqlGenerator
    {
        private static readonly Dictionary<string, string[]> _castMappings = new Dictionary<string, string[]>
        {
            { "signed", new []{ "tinyint", "smallint", "mediumint", "int", "bigint" }},
            { "decimal", new []{ "decimal", "double", "float" } },
            { "binary", new []{ "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob" } },
            { "datetime", new []{ "datetime", "timestamp" } },
            { "time", new []{ "time" } },
            { "json", new []{ "json" } },
        };

        private const ulong LimitUpperBound = 18446744073709551610;

        private readonly bool _noBackslashEscapes;

        public MySqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IMySqlOptions options)
            : base(dependencies)
        {
            _noBackslashEscapes = options?.NoBackslashEscapes ?? false;
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
            return; // Not supported in MySql https://www.tutorialrepublic.com/sql-tutorial/sql-top-clause.php
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
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
                    Sql.AppendLine().Append("LIMIT ").Append(LimitUpperBound);
                }

                Sql.Append(" OFFSET ");
                Visit(selectExpression.Offset);
            }
        }

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (sqlFunctionExpression.Name.StartsWith("@@", StringComparison.Ordinal))
            {
                Sql.Append(sqlFunctionExpression.Name);

                return sqlFunctionExpression;
            }

            return base.VisitSqlFunction(sqlFunctionExpression);
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType == ExpressionType.Add &&
                binaryExpression.Left.Type == typeof(string) &&
                binaryExpression.Right.Type == typeof(string))
            {
                Sql.Append("CONCAT(");
                Visit(binaryExpression.Left);
                Sql.Append(", ");
                var exp = Visit(binaryExpression.Right);
                Sql.Append(")");

                return binaryExpression;
            }

            return base.VisitBinary(binaryExpression);
        }

        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            // TODO: find a proper replacement

            //if (_noBackslashEscapes)
            //{
            //    //instead of having MySqlConnector replace parameter placeholders with escaped values
            //    //(causing "parameterized" queries to fail with NO_BACKSLASH_ESCAPES),
            //    //directly insert the value with only replacing ' with ''
            //    Check.NotNull(parameterExpression, nameof(parameterExpression));
            //    var isRegistered = ParameterValues.TryGetValue(parameterExpression.Name, out var value);
            //    if (isRegistered && value is string)
            //    {
            //        return VisitConstant(Expression.Constant(value));
            //    }
            //}

            return base.VisitParameter(parameterExpression);
        }

        public virtual Expression VisitMySqlRegexp(MySqlRegexpExpression mySqlRegexpExpression)
        {
            Check.NotNull(mySqlRegexpExpression, nameof(mySqlRegexpExpression));

            Visit(mySqlRegexpExpression.Match);
            Sql.Append(" REGEXP ");
            Visit(mySqlRegexpExpression.Pattern);

            return mySqlRegexpExpression;
        }

        
        public Expression VisitMySqlComplexFunctionArgumentExpression(MySqlComplexFunctionArgumentExpression mySqlComplexFunctionArgumentExpression)
        {
            Check.NotNull(mySqlComplexFunctionArgumentExpression, nameof(mySqlComplexFunctionArgumentExpression));

            var first = true;
            foreach (var argument in mySqlComplexFunctionArgumentExpression.ArgumentParts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Sql.Append(" ");
                }

                Visit(argument);
            }

            return mySqlComplexFunctionArgumentExpression;
        }

        public Expression VisitMySqlCollateExpression(MySqlCollateExpression mySqlCollateExpression)
        {
            Check.NotNull(mySqlCollateExpression, nameof(mySqlCollateExpression));

            Sql.Append("CONVERT(");

            Visit(mySqlCollateExpression.ValueExpression);

            Sql.Append($" USING {mySqlCollateExpression.Charset}) COLLATE {mySqlCollateExpression.Collation}");

            return mySqlCollateExpression;
        }
    }
}
