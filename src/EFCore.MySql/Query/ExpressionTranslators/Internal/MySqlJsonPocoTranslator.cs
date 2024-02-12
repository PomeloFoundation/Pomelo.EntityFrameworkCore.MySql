// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public abstract class MySqlJsonPocoTranslator : IMySqlJsonPocoTranslator
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly RelationalTypeMapping _unquotedStringTypeMapping;
        private readonly RelationalTypeMapping _intTypeMapping;

        public MySqlJsonPocoTranslator(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
            _unquotedStringTypeMapping = ((MySqlStringTypeMapping)_typeMappingSource.FindMapping(typeof(string))).Clone(unquoted: true);
            _intTypeMapping = _typeMappingSource.FindMapping(typeof(int));
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (instance?.TypeMapping is MySqlJsonTypeMapping ||
                instance is MySqlJsonTraversalExpression)
            {
                // Path locations need to be rendered without surrounding quotes, because the path itself already
                // has quotes.
                var sqlConstantExpression = _sqlExpressionFactory.ApplyDefaultTypeMapping(
                    _sqlExpressionFactory.Constant(
                        GetJsonPropertyName(member) ?? member.Name,
                        _unquotedStringTypeMapping));

                return TranslateMemberAccess(
                    instance,
                    sqlConstantExpression,
                    returnType);
            }

            return null;
        }

        public abstract string GetJsonPropertyName(MemberInfo member);

        public virtual SqlExpression TranslateMemberAccess(
            [NotNull] SqlExpression instance, [NotNull] SqlExpression member, [NotNull] Type returnType)
        {
            // The first time we see a JSON traversal it's on a column - create a JsonTraversalExpression.
            // Traversals on top of that get appended into the same expression.

            if (instance is ColumnExpression columnExpression &&
                columnExpression.TypeMapping is MySqlJsonTypeMapping)
            {
                return ConvertFromJsonExtract(
                    _sqlExpressionFactory.JsonTraversal(
                            columnExpression,
                            returnsText: MySqlJsonTraversalExpression.TypeReturnsText(returnType),
                            returnType)
                        .Append(
                            member,
                            returnType,
                            FindPocoTypeMapping(returnType)),
                    returnType);
            }

            if (instance is MySqlJsonTraversalExpression prevPathTraversal)
            {
                return prevPathTraversal.Append(
                    member,
                    returnType,
                    FindPocoTypeMapping(returnType));
            }

            return null;
        }

        public virtual SqlExpression TranslateArrayLength([NotNull] SqlExpression expression)
            => expression is MySqlJsonTraversalExpression ||
               expression is ColumnExpression columnExpression && columnExpression.TypeMapping is MySqlJsonTypeMapping
                ? _sqlExpressionFactory.NullableFunction(
                    "JSON_LENGTH",
                    new[] {expression},
                    typeof(int),
                    _intTypeMapping,
                    false)
                : null;

        protected virtual SqlExpression ConvertFromJsonExtract(SqlExpression expression, Type returnType)
        {
            var unwrappedReturnType = returnType.UnwrapNullableType();
            var typeMapping = FindPocoTypeMapping(returnType);

            switch (Type.GetTypeCode(unwrappedReturnType))
            {
                case TypeCode.Boolean:
                    return _sqlExpressionFactory.NonOptimizedEqual(
                        expression,
                        _sqlExpressionFactory.Constant(true, typeMapping));

                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return _sqlExpressionFactory.Convert(
                        expression,
                        returnType,
                        typeMapping);
            }

            if (unwrappedReturnType == typeof(Guid)
                || unwrappedReturnType == typeof(DateTimeOffset)
                || unwrappedReturnType == typeof(DateOnly)
                || unwrappedReturnType == typeof(TimeOnly))
            {
                return _sqlExpressionFactory.Convert(
                    expression,
                    returnType,
                    typeMapping);
            }

            return expression;
        }

        protected virtual RelationalTypeMapping FindPocoTypeMapping(Type type)
            => GetJsonSpecificTypeMapping(_typeMappingSource.FindMapping(type) ??
                                   _typeMappingSource.FindMapping(type, "json"));

        protected virtual RelationalTypeMapping GetJsonSpecificTypeMapping(RelationalTypeMapping typeMapping)
            => typeMapping is IJsonSpecificTypeMapping jsonSpecificTypeMapping
                ? jsonSpecificTypeMapping.CloneAsJsonCompatible()
                : typeMapping;
    }
}
