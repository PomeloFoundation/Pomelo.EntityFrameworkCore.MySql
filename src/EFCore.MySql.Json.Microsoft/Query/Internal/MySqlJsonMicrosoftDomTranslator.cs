using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Query.ExpressionTranslators.Internal
{
    public class MySqlJsonMicrosoftDomTranslator : IMemberTranslator, IMethodCallTranslator
    {
        private static readonly MemberInfo _rootElement = typeof(JsonDocument).GetProperty(nameof(JsonDocument.RootElement));
        private static readonly MethodInfo _getProperty = typeof(JsonElement).GetRuntimeMethod(nameof(JsonElement.GetProperty), new[] { typeof(string) });
        private static readonly MethodInfo _getArrayLength = typeof(JsonElement).GetRuntimeMethod(nameof(JsonElement.GetArrayLength), Type.EmptyTypes);

        private static readonly MethodInfo _arrayIndexer = typeof(JsonElement).GetProperties()
            .Single(p => p.GetIndexParameters().Length == 1 &&
                         p.GetIndexParameters()[0].ParameterType == typeof(int))
            .GetMethod;

        private static readonly string[] _getMethods =
        {
            nameof(JsonElement.GetBoolean),
            nameof(JsonElement.GetDateTime),
            nameof(JsonElement.GetDateTimeOffset),
            nameof(JsonElement.GetDecimal),
            nameof(JsonElement.GetDouble),
            nameof(JsonElement.GetGuid),
            nameof(JsonElement.GetInt16),
            nameof(JsonElement.GetInt32),
            nameof(JsonElement.GetInt64),
            nameof(JsonElement.GetSingle),
            nameof(JsonElement.GetString)
        };

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlJsonMicrosoftDomTranslator([NotNull] MySqlSqlExpressionFactory sqlExpressionFactory, [NotNull] IRelationalTypeMappingSource typeMappingSource)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            if (member.DeclaringType != typeof(JsonDocument))
            {
                return null;
            }

            if (member == _rootElement &&
                instance is ColumnExpression column &&
                column.TypeMapping is MySqlJsonTypeMapping)
            {
                // Simply get rid of the RootElement member access
                return column;
            }

            return null;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (method.DeclaringType != typeof(JsonElement) ||
                !(instance.TypeMapping is MySqlJsonTypeMapping mapping))
            {
                return null;
            }

            // The root of the JSON expression is a ColumnExpression. We wrap that with an empty traversal
            // expression (col->'$'); subsequent traversals will gradually append the path into that.
            // Note that it's possible to call methods such as GetString() directly on the root, and the
            // empty traversal is necessary to properly convert it to a text.
            instance = instance is ColumnExpression columnExpression
                ? _sqlExpressionFactory.JsonTraversal(
                    columnExpression, returnsText: false, typeof(string), mapping)
                : instance;

            if (method == _getProperty)
            {
                return instance is MySqlJsonTraversalExpression prevPathTraversal
                    ? prevPathTraversal.Append(
                        ApplyPathLocationTypeMapping(arguments[0]),
                        typeof(JsonElement),
                        _typeMappingSource.FindMapping(typeof(JsonElement)))
                    : null;
            }

            if (method == _arrayIndexer)
            {
                return instance is MySqlJsonTraversalExpression prevPathTraversal
                    ? prevPathTraversal.Append(
                        _sqlExpressionFactory.JsonArrayIndex(ApplyPathLocationTypeMapping(arguments[0])),
                        typeof(JsonElement),
                        _typeMappingSource.FindMapping(typeof(JsonElement)))
                    : null;
            }

            if (_getMethods.Contains(method.Name) &&
                arguments.Count == 0 &&
                instance is MySqlJsonTraversalExpression traversal)
            {
                return ConvertFromJsonExtract(
                    traversal.Clone(
                        method.Name == nameof(JsonElement.GetString),
                        method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType)
                    ),
                    method.ReturnType);
            }

            if (method == _getArrayLength)
            {
                return _sqlExpressionFactory.Function(
                    "JSON_LENGTH",
                    new[] { instance },
                    typeof(int));
            }

            if (method.Name.StartsWith("TryGet") && arguments.Count == 0)
            {
                throw new InvalidOperationException($"The TryGet* methods on {nameof(JsonElement)} aren't translated yet, use Get* instead.'");
            }

            return null;
        }

        private SqlExpression ConvertFromJsonExtract(SqlExpression expression, Type returnType)
            => returnType == typeof(bool)
                ? _sqlExpressionFactory.Equal(
                    expression,
                    _sqlExpressionFactory.Constant(true, _typeMappingSource.FindMapping(typeof(bool))))
                : expression;

        private SqlExpression ApplyPathLocationTypeMapping(SqlExpression expression)
        {
            var pathLocation = _sqlExpressionFactory.ApplyDefaultTypeMapping(expression);

            // Path locations are usually made of strings. And they should be rendered without surrounding quotes.
            if (pathLocation is SqlConstantExpression sqlConstantExpression &&
                sqlConstantExpression.TypeMapping is MySqlStringTypeMapping stringTypeMapping &&
                !stringTypeMapping.IsUnquoted)
            {
                pathLocation = sqlConstantExpression.ApplyTypeMapping(stringTypeMapping.Clone(true));
            }

            return pathLocation;
        }
    }
}
