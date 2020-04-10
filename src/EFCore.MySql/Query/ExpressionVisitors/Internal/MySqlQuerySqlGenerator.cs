// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlQuerySqlGenerator : QuerySqlGenerator
    {
        // The order in which the types are specified matters, because types get matched by using StartsWith.
        private static readonly Dictionary<string, string[]> _castMappings = new Dictionary<string, string[]>
        {
            { "signed", new []{ "tinyint", "smallint", "mediumint", "int", "bigint", "bit" }},
            { "decimal(65,30)", new []{ "decimal" } },
            { "double", new []{ "double" } },
            { "float", new []{ "float" } },
            { "binary", new []{ "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob" } },
            { "datetime(6)", new []{ "datetime(6)" } },
            { "datetime", new []{ "datetime" } },
            { "date", new []{ "date" } },
            { "timestamp(6)", new []{ "timestamp(6)" } },
            { "timestamp", new []{ "timestamp" } },
            { "time(6)", new []{ "time(6)" } },
            { "time", new []{ "time" } },
            { "json", new []{ "json" } },
            { "char", new []{ "char", "varchar", "text", "tinytext", "mediumtext", "longtext" } },
            { "nchar", new []{ "nchar", "nvarchar" } },
        };

        private const ulong LimitUpperBound = 18446744073709551610;

        private readonly IMySqlOptions _options;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlQuerySqlGenerator(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [CanBeNull] IMySqlOptions options)
            : base(dependencies)
        {
            _options = options;
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

        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            Sql.Append("JOIN LATERAL ");

            Visit(crossApplyExpression.Table);

            Sql.Append(" ON TRUE");

            return crossApplyExpression;
        }

        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            Sql.Append("LEFT JOIN LATERAL ");

            Visit(outerApplyExpression.Table);

            Sql.Append(" ON TRUE");

            return outerApplyExpression;
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            if (sqlBinaryExpression.OperatorType == ExpressionType.Add &&
                sqlBinaryExpression.Type == typeof(string) &&
                sqlBinaryExpression.Left.TypeMapping?.ClrType == typeof(string) &&
                sqlBinaryExpression.Right.TypeMapping?.ClrType == typeof(string))
            {
                Sql.Append("CONCAT(");
                Visit(sqlBinaryExpression.Left);
                Sql.Append(", ");
                Visit(sqlBinaryExpression.Right);
                Sql.Append(")");

                return sqlBinaryExpression;
            }

            return base.VisitSqlBinary(sqlBinaryExpression);
        }

        public virtual Expression VisitMySqlRegexp(MySqlRegexpExpression mySqlRegexpExpression)
        {
            Check.NotNull(mySqlRegexpExpression, nameof(mySqlRegexpExpression));

            Visit(mySqlRegexpExpression.Match);
            Sql.Append(" REGEXP ");
            Visit(mySqlRegexpExpression.Pattern);

            return mySqlRegexpExpression;
        }

        public Expression VisitMySqlMatch(MySqlMatchExpression mySqlMatchExpression)
        {
            Check.NotNull(mySqlMatchExpression, nameof(mySqlMatchExpression));

            Sql.Append("MATCH ");
            Sql.Append("(");
            Visit(mySqlMatchExpression.Match);
            Sql.Append(")");
            Sql.Append(" AGAINST ");
            Sql.Append($"(");
            Visit(mySqlMatchExpression.Against);

            switch (mySqlMatchExpression.SearchMode)
            {
                case MySqlMatchSearchMode.NaturalLanguage:
                    break;
                case MySqlMatchSearchMode.NaturalLanguageWithQueryExpansion:
                    Sql.Append(" WITH QUERY EXPANSION");
                    break;
                case MySqlMatchSearchMode.Boolean:
                    Sql.Append(" IN BOOLEAN MODE");
                    break;
            }

            Sql.Append(")");

            return mySqlMatchExpression;
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
        {
            if (sqlUnaryExpression.OperatorType != ExpressionType.Convert)
            {
                return base.VisitSqlUnary(sqlUnaryExpression);
            }

            var typeMapping = sqlUnaryExpression.TypeMapping;

            var storeTypeLower = typeMapping.StoreType.ToLower();
            string castMapping = null;
            foreach (var kvp in _castMappings)
            {
                foreach (var storeType in kvp.Value)
                {
                    if (storeTypeLower.StartsWith(storeType))
                    {
                        castMapping = kvp.Key;
                        break;
                    }
                }
                if (castMapping != null)
                {
                    break;
                }
            }

            if (castMapping == null)
            {
                throw new InvalidOperationException($"Cannot cast from type '{typeMapping.StoreType}'");
            }

            if (castMapping == "signed" && storeTypeLower.Contains("unsigned"))
            {
                castMapping = "unsigned";
            }

            // FLOAT and DOUBLE are supported by CAST() as of MySQL 8.0.17.
            // For server versions before that, a workaround is applied, that casts to a DECIMAL,
            // that is then added to 0e0, which results in a DOUBLE.
            // As of MySQL 8.0.18, a FLOAT cast might unnecessarily drop decimal places and round,
            // so we just keep casting to double instead. MySqlConnector ensures, that a System.Single
            // will be returned if expected, even if we return a DOUBLE.
            // REF: https://stackoverflow.com/a/32991084/2618319

            var useDecimalToDoubleWorkaround = false;

            if (castMapping.StartsWith("float") &&
                !_options.ServerVersion.SupportsFloatCast)
            {
                castMapping = "double";
            }

            if (castMapping.StartsWith("double") &&
                !_options.ServerVersion.SupportsDoubleCast)
            {
                useDecimalToDoubleWorkaround = true;
                castMapping = "decimal(65,30)";
            }

            if (useDecimalToDoubleWorkaround)
            {
                Sql.Append("(");
            }

            Sql.Append("CAST(");
            Visit(sqlUnaryExpression.Operand);
            Sql.Append(" AS ");
            Sql.Append(castMapping);
            Sql.Append(")");

            if (useDecimalToDoubleWorkaround)
            {
                Sql.Append(" + 0e0)");
            }
            else if (castMapping.EndsWith("char"))
            {
                // Expressions like `"mystring" + 1` can lead to collation mismatches.
                // We force `utf8mb4_bin` here, that should always work. It might however change the case sensitivity of
                // operations it is part of.
                Sql.Append(" COLLATE utf8mb4_bin");
            }

            return sqlUnaryExpression;
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

        public Expression VisitMySqlBinaryExpression(MySqlBinaryExpression mySqlBinaryExpression)
        {
            Sql.Append("(");
            Visit(mySqlBinaryExpression.Left);
            Sql.Append(")");

            switch (mySqlBinaryExpression.OperatorType)
            {
                case MySqlBinaryExpressionOperatorType.IntegerDivision:
                    Sql.Append(" DIV ");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Sql.Append("(");
            Visit(mySqlBinaryExpression.Right);
            Sql.Append(")");

            return mySqlBinaryExpression;
        }
    }
}
