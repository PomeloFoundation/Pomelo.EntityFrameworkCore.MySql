// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlQuerySqlGenerator : DefaultQuerySqlGenerator, IMySqlExpressionVisitor
    {
        private const ulong LimitUpperBound = 18446744073709551610;

        protected override string TypedTrueLiteral => "TRUE";
        protected override string TypedFalseLiteral => "FALSE";

        private readonly IMySqlOptions _options;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlQuerySqlGenerator(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [NotNull] SelectExpression selectExpression,
                IMySqlOptions options)
            : base(dependencies, selectExpression)
        {
            _options = options;
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
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

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (sqlFunctionExpression.FunctionName.StartsWith("@@", StringComparison.Ordinal))
            {
                Sql.Append(sqlFunctionExpression.FunctionName);

                return sqlFunctionExpression;
            }

            return base.VisitSqlFunction(sqlFunctionExpression);
        }

        protected override void GenerateProjection(Expression projection)
        {
            var aliasedProjection = projection as AliasExpression;
            var expressionToProcess = aliasedProjection?.Expression ?? projection;
            var updatedExperssion = ExplicitCastToBool(expressionToProcess);

            expressionToProcess = aliasedProjection != null
                ? new AliasExpression(aliasedProjection.Alias, updatedExperssion)
                : updatedExperssion;

            base.GenerateProjection(expressionToProcess);
        }

        private Expression ExplicitCastToBool(Expression expression)
        {
            return (expression as BinaryExpression)?.NodeType == ExpressionType.Coalesce
                   && expression.Type.UnwrapNullableType() == typeof(bool)
                ? new ExplicitCastExpression(expression, expression.Type)
                : expression;
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
                Visit(binaryExpression.Right);
                Sql.Append(")");

                return binaryExpression;
            }

            return base.VisitBinary(binaryExpression);
        }

        private bool _isParameterReplaced = false;
        public override bool IsCacheable => !_isParameterReplaced && base.IsCacheable;

        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (_options?.NoBackslashEscapes ?? false)
            {
                //instead of having MySqlConnector replace parameter placeholders with escaped values
                //(causing "parameterized" queries to fail with NO_BACKSLASH_ESCAPES),
                //directly insert the value with only replacing ' with ''
                Check.NotNull(parameterExpression, nameof(parameterExpression));
                var isRegistered = ParameterValues.TryGetValue(parameterExpression.Name, out var value);
                if (isRegistered && value is string)
                {
                    _isParameterReplaced = true;
                    return VisitConstant(Expression.Constant(value));
                }
            }

            return base.VisitParameter(parameterExpression);
        }

        public virtual Expression VisitRegexp(RegexpExpression regexpExpression)
        {
            Check.NotNull(regexpExpression, nameof(regexpExpression));

            Visit(regexpExpression.Match);
            Sql.Append(" REGEXP ");
            Visit(regexpExpression.Pattern);

            return regexpExpression;
        }

        private static readonly Dictionary<string, string[]> _castMappings = new Dictionary<string, string[]>
        {
            { "signed", new []{ "tinyint", "smallint", "mediumint", "int", "bigint", "bit" }},
            { "decimal(65,30)", new []{ "decimal" } },
            { "double", new []{ "double" } },
            { "float", new []{ "float" } },
            { "binary", new []{ "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob" } },
            { "datetime", new []{ "datetime", "timestamp" } },
            { "date", new []{ "date" } },
            { "time", new []{ "time" } },
            { "json", new []{ "json" } },
            { "char", new []{ "char", "varchar", "text", "tinytext", "mediumtext", "longtext" } },
            { "nchar", new []{ "nchar", "nvarchar" } },
        };

        public override Expression VisitExplicitCast(ExplicitCastExpression explicitCastExpression)
        {
            var typeMapping = Dependencies.TypeMappingSource.FindMapping(explicitCastExpression.Type);
            if (typeMapping == null)
            {
                throw new InvalidOperationException($"Cannot cast to type '{explicitCastExpression.Type.Name}'");
            }

            //
            // TODO: Build better mappings with TypeMappingSource.
            //

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
            // For server versions before that, a workaround applied, that CAST() to a DECIMAL,
            // that is then added to 0e0, which results in a DOUBLE.
            // REF: https://stackoverflow.com/a/32991084/2618319

            var useDecimalToDoubleFloatWorkaround = false;

            if (castMapping.StartsWith("double") && (!_options?.ServerVersion.SupportsDoubleCast ?? false))
            {
                useDecimalToDoubleFloatWorkaround = true;
                castMapping = "decimal(65,30)";
            }

            if (castMapping.StartsWith("float") && (!_options?.ServerVersion.SupportsFloatCast ?? false))
            {
                useDecimalToDoubleFloatWorkaround = true;
                castMapping = "decimal(65,30)";
            }

            if (useDecimalToDoubleFloatWorkaround)
            {
                Sql.Append("(");
            }

            Sql.Append("CAST(");
            Visit(explicitCastExpression.Operand);
            Sql.Append(" AS ");
            Sql.Append(castMapping);
            Sql.Append(")");

            if (useDecimalToDoubleFloatWorkaround)
            {
                Sql.Append(" + 0e0)");
            }

            return explicitCastExpression;
        }
        
        public Expression VisitMySqlComplexFunctionArgumentExpression(
            [NotNull] MySqlComplexFunctionArgumentExpression mySqlComplexFunctionArgumentExpression)
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

        public Expression VisitMySqlCollateExpression(
            [NotNull] MySqlCollateExpression mySqlCollateExpression)
        {
            Check.NotNull(mySqlCollateExpression, nameof(mySqlCollateExpression));

            Sql.Append("CONVERT(");

            Visit(mySqlCollateExpression.ValueExpression);

            Sql.Append($" USING {mySqlCollateExpression.Charset}) COLLATE {mySqlCollateExpression.Collation}");

            return mySqlCollateExpression;
        }
    }
}
