using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlJsonPocoTranslator : IMemberTranslator
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly RelationalTypeMapping _jsonTypeMapping;
        private readonly RelationalTypeMapping _unquotedStringTypeMapping;
        private readonly RelationalTypeMapping _intTypeMapping;
        private readonly RelationalTypeMapping _boolTypeMapping;

        public MySqlJsonPocoTranslator(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
            _unquotedStringTypeMapping = ((MySqlStringTypeMapping)_typeMappingSource.FindMapping(typeof(string))).Clone(true);
            _jsonTypeMapping = _typeMappingSource.FindMapping("json");
            _intTypeMapping = _typeMappingSource.FindMapping(typeof(int));
            _boolTypeMapping = _typeMappingSource.FindMapping(typeof(bool));
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            if (instance?.TypeMapping is MySqlJsonTypeMapping ||
                instance is MySqlJsonTraversalExpression)
            {
                // Path locations need to be rendered without surrounding quotes, because the path itself already
                // has quotes.
                var sqlConstantExpression = _sqlExpressionFactory.ApplyDefaultTypeMapping(
                    _sqlExpressionFactory.Constant(
                        member.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? member.Name,
                        _unquotedStringTypeMapping));

                return TranslateMemberAccess(
                    instance,
                    sqlConstantExpression,
                    returnType);
            }

            return null;
        }

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
                            returnsText: returnType == typeof(string),
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
                ? _sqlExpressionFactory.Function(
                    "JSON_LENGTH",
                    new[] {expression},
                    typeof(int),
                    _intTypeMapping)
                : null;

        private SqlExpression ConvertFromJsonExtract(SqlExpression expression, Type returnType)
            => returnType == typeof(bool)
                ? _sqlExpressionFactory.Equal(
                    expression,
                    _sqlExpressionFactory.Constant(true, _boolTypeMapping))
                : expression;

        private RelationalTypeMapping FindPocoTypeMapping(Type type)
        {
            var mapping = _typeMappingSource.FindMapping(type);

            if (mapping == null)
            {
                mapping = _typeMappingSource.FindMapping(type, "json");
            }

            return mapping;
        }
    }
}
