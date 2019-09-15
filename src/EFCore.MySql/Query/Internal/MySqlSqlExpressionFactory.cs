﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
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

        #endregion Expression factory methods

        public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
        {
            if (sqlExpression == null
                || sqlExpression.TypeMapping != null)
            {
                return sqlExpression;
            }

            switch (sqlExpression)
            {
                case MySqlComplexFunctionArgumentExpression e:
                    return ApplyTypeMappingOnComplexFunctionArgument(e);

                case MySqlCollateExpression e:
                    return ApplyTypeMappingOnCollate(e);

                case MySqlRegexpExpression e:
                    return ApplyTypeMappingOnRegexp(e);

                default:
                    return base.ApplyTypeMapping(sqlExpression, typeMapping);
            }
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

        private SqlExpression ApplyTypeMappingOnRegexp(MySqlRegexpExpression regexpExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(regexpExpression.Match)
                                      ?? _typeMappingSource.FindMapping(regexpExpression.Match.Type);

            return new MySqlRegexpExpression(
                ApplyTypeMapping(regexpExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(regexpExpression.Pattern, inferredTypeMapping),
                _boolTypeMapping);
        }
    }
}
