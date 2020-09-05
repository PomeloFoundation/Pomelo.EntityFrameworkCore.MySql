using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlSqlExpressionFactory : SqlExpressionFactory
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly RelationalTypeMapping _boolTypeMapping;

        public MySqlSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies)
            : base(dependencies)
        {
            _typeMappingSource = dependencies.TypeMappingSource;
            _boolTypeMapping = _typeMappingSource.FindMapping(typeof(bool));
        }

        public virtual RelationalTypeMapping FindMapping([NotNull] string storeTypeName)
            => _typeMappingSource.FindMapping(storeTypeName);

        public virtual RelationalTypeMapping FindMapping(
            [NotNull] Type type,
            [CanBeNull] string storeTypeName,
            bool keyOrIndex = false,
            bool? unicode = null,
            int? size = null,
            bool? rowVersion = null,
            bool? fixedLength = null,
            int? precision = null,
            int? scale = null)
            => _typeMappingSource.FindMapping(
                type,
                storeTypeName,
                keyOrIndex,
                unicode,
                size,
                rowVersion,
                fixedLength,
                precision,
                scale);

        #region Expression factory methods

        public MySqlComplexFunctionArgumentExpression ComplexFunctionArgument(
            IEnumerable<SqlExpression> argumentParts,
            Type argumentType,
            RelationalTypeMapping typeMapping = null)
        {
            var typeMappedArgumentParts = new List<SqlExpression>();

            foreach (var argument in argumentParts)
            {
                typeMappedArgumentParts.Add(ApplyDefaultTypeMapping(argument));
            }

            return new MySqlComplexFunctionArgumentExpression(
                typeMappedArgumentParts,
                argumentType,
                typeMapping);
        }

        public MySqlCollateExpression Collate(
            SqlExpression valueExpression,
            string charset,
            string collation)
            => (MySqlCollateExpression)ApplyDefaultTypeMapping(
                new MySqlCollateExpression(
                    valueExpression,
                    charset,
                    collation,
                    null));

        public MySqlRegexpExpression Regexp(
            SqlExpression match,
            SqlExpression pattern)
            => (MySqlRegexpExpression)ApplyDefaultTypeMapping(
                new MySqlRegexpExpression(
                    match,
                    pattern,
                    null));

        public virtual MySqlBinaryExpression MySqlIntegerDivide(
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping = null)
            => MakeBinary(
                MySqlBinaryExpressionOperatorType.IntegerDivision,
                left,
                right,
                typeMapping);

        #endregion Expression factory methods

        public virtual MySqlBinaryExpression MakeBinary(
            MySqlBinaryExpressionOperatorType operatorType,
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping)
        {
            var returnType = left.Type;

            return (MySqlBinaryExpression)ApplyTypeMapping(
                new MySqlBinaryExpression(
                    operatorType,
                    left,
                    right,
                    returnType,
                    null),
                typeMapping);
        }

        public virtual MySqlMatchExpression MakeMatch(
            SqlExpression match,
            SqlExpression against,
            MySqlMatchSearchMode searchMode)
        {
            return (MySqlMatchExpression)ApplyDefaultTypeMapping(
                new MySqlMatchExpression(
                    match,
                    against,
                    searchMode,
                    null));
        }

        public virtual MySqlJsonTraversalExpression JsonTraversal(
            [NotNull] SqlExpression expression,
            bool returnsText,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping = null)
            => new MySqlJsonTraversalExpression(
                ApplyDefaultTypeMapping(expression),
                returnsText,
                type,
                typeMapping);

        public virtual MySqlJsonArrayIndexExpression JsonArrayIndex(
            [NotNull] SqlExpression expression)
            => JsonArrayIndex(expression, typeof(int));

        public virtual MySqlJsonArrayIndexExpression JsonArrayIndex(
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping = null)
            => (MySqlJsonArrayIndexExpression)ApplyDefaultTypeMapping(
                new MySqlJsonArrayIndexExpression(
                    ApplyDefaultTypeMapping(expression),
                    type,
                    typeMapping));

        public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
        {
            if (sqlExpression == null
                || sqlExpression.TypeMapping != null)
            {
                return sqlExpression;
            }

            return ApplyNewTypeMapping(sqlExpression, typeMapping);
        }

        private SqlExpression ApplyNewTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
        {
            return sqlExpression switch
            {
                MySqlComplexFunctionArgumentExpression e => ApplyTypeMappingOnComplexFunctionArgument(e),
                MySqlCollateExpression e => ApplyTypeMappingOnCollate(e),
                MySqlRegexpExpression e => ApplyTypeMappingOnRegexp(e),
                MySqlBinaryExpression e => ApplyTypeMappingOnMySqlBinary(e, typeMapping),
                MySqlMatchExpression e => ApplyTypeMappingOnMatch(e),
                MySqlJsonArrayIndexExpression e => e.ApplyTypeMapping(typeMapping),
                _ => base.ApplyTypeMapping(sqlExpression, typeMapping)
            };
        }

        private MySqlComplexFunctionArgumentExpression ApplyTypeMappingOnComplexFunctionArgument(MySqlComplexFunctionArgumentExpression complexFunctionArgumentExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(complexFunctionArgumentExpression.ArgumentParts.ToArray())
                                      ?? _typeMappingSource.FindMapping(complexFunctionArgumentExpression.Type);

            return new MySqlComplexFunctionArgumentExpression(
                complexFunctionArgumentExpression.ArgumentParts,
                complexFunctionArgumentExpression.Type,
                inferredTypeMapping ?? complexFunctionArgumentExpression.TypeMapping);
        }

        private MySqlCollateExpression ApplyTypeMappingOnCollate(MySqlCollateExpression collateExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(collateExpression.ValueExpression)
                                      ?? _typeMappingSource.FindMapping(collateExpression.ValueExpression.Type);

            return new MySqlCollateExpression(
                ApplyTypeMapping(collateExpression.ValueExpression, inferredTypeMapping),
                collateExpression.Charset,
                collateExpression.Collation,
                inferredTypeMapping ?? collateExpression.TypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnMatch(MySqlMatchExpression matchExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(matchExpression.Match)
                                      ?? _typeMappingSource.FindMapping(matchExpression.Match.Type);

            return new MySqlMatchExpression(
                ApplyTypeMapping(matchExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(matchExpression.Against, inferredTypeMapping),
                matchExpression.SearchMode,
                _boolTypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnRegexp(MySqlRegexpExpression regexpExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(regexpExpression.Match)
                                      ?? _typeMappingSource.FindMapping(regexpExpression.Match.Type);

            return new MySqlRegexpExpression(
                ApplyTypeMapping(regexpExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(regexpExpression.Pattern, inferredTypeMapping),
                _boolTypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnMySqlBinary(
            MySqlBinaryExpression sqlBinaryExpression,
            RelationalTypeMapping typeMapping)
        {
            var left = sqlBinaryExpression.Left;
            var right = sqlBinaryExpression.Right;

            Type resultType;
            RelationalTypeMapping resultTypeMapping;
            RelationalTypeMapping inferredTypeMapping;

            switch (sqlBinaryExpression.OperatorType)
            {
                case MySqlBinaryExpressionOperatorType.IntegerDivision:
                {
                    inferredTypeMapping = typeMapping ?? ExpressionExtensions.InferTypeMapping(left, right);
                    resultType = left.Type;
                    resultTypeMapping = inferredTypeMapping;
                }
                break;

                default:
                    throw new InvalidOperationException("Incorrect OperatorType for MySqlBinaryExpression");
            }

            return new MySqlBinaryExpression(
                sqlBinaryExpression.OperatorType,
                ApplyTypeMapping(left, inferredTypeMapping),
                ApplyTypeMapping(right, inferredTypeMapping),
                resultType,
                resultTypeMapping);
        }
    }
}
