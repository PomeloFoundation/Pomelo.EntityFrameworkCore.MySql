using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlJsonDbFunctionsTranslator : IMethodCallTranslator
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;
        private readonly RelationalTypeMapping _stringTypeMapping;

        public MySqlJsonDbFunctionsTranslator(
            [NotNull] MySqlSqlExpressionFactory sqlExpressionFactory,
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            IMySqlOptions options)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
            _stringTypeMapping = typeMappingSource.FindMapping(typeof(string));
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (method.DeclaringType != typeof(MySqlJsonDbFunctionsExtensions))
            {
                return null;
            }

            var args = arguments
                // Skip useless DbFunctions instance
                .Skip(1)
                // JSON extensions accept object parameters for JSON, since they must be able to handle POCOs, strings or DOM types.
                // This means they come wrapped in a convert node, which we need to remove.
                // Convert nodes may also come from wrapping JsonTraversalExpressions generated through POCO traversal.
                .Select(removeConvert)
                // CHECK: Either just not doing this it all is fine, or not applying it to JsonQuote and JsonUnquote
                // (as already implemented below) is needed. An alternative would be to move the check into the local
                // json() function.
                //
                // If a function is invoked over a JSON traversal expression, that expression may come with
                // returnText: true (i.e. operator ->> and not ->). Since the functions below require a json object and
                // not text, we transform it.
                // .Select(
                //     a => a is MySqlJsonTraversalExpression traversal &&
                //          method.Name != nameof(MySqlJsonDbFunctionsExtensions.JsonQuote) &&
                //          method.Name != nameof(MySqlJsonDbFunctionsExtensions.JsonUnquote)
                //         ? withReturnsText(traversal, false)
                //         : a)
                .ToArray();

            var result = method.Name switch
            {
                nameof(MySqlJsonDbFunctionsExtensions.AsJson)
                    => _sqlExpressionFactory.ApplyTypeMapping(
                        args[0],
                        _sqlExpressionFactory.FindMapping(method.ReturnType, "json")),
                nameof(MySqlJsonDbFunctionsExtensions.JsonType)
                    => _sqlExpressionFactory.Function(
                        "JSON_TYPE",
                        new[] {json(args[0])},
                        typeof(string)),
                nameof(MySqlJsonDbFunctionsExtensions.JsonQuote)
                    => _sqlExpressionFactory.Function(
                        "JSON_QUOTE",
                        new[] {args[0]},
                        method.ReturnType),
                nameof(MySqlJsonDbFunctionsExtensions.JsonUnquote)
                    => _sqlExpressionFactory.Function(
                        "JSON_UNQUOTE",
                        new[] {args[0]},
                        method.ReturnType),
                nameof(MySqlJsonDbFunctionsExtensions.JsonExtract)
                    => _sqlExpressionFactory.Function(
                        "JSON_EXTRACT",
                        Array.Empty<SqlExpression>()
                            .Append(json(args[0]))
                            .Concat(deconstructParamsArray(args[1])),
                        method.ReturnType,
                        _sqlExpressionFactory.FindMapping(method.ReturnType, "json")),
                nameof(MySqlJsonDbFunctionsExtensions.JsonContains)
                    => _sqlExpressionFactory.Function(
                        "JSON_CONTAINS",
                        args.Length >= 3
                            ? new[] {json(args[0]), args[1], args[2]}
                            : new[] {json(args[0]), args[1]},
                        typeof(bool)),
                nameof(MySqlJsonDbFunctionsExtensions.JsonContainsPath)
                    => _sqlExpressionFactory.Function(
                        "JSON_CONTAINS_PATH",
                        new[] {json(args[0]), _sqlExpressionFactory.Constant("one"), args[1]},
                        typeof(bool)),
                nameof(MySqlJsonDbFunctionsExtensions.JsonContainsPathAny)
                    => _sqlExpressionFactory.Function(
                        "JSON_CONTAINS_PATH",
                        Array.Empty<SqlExpression>()
                            .Append(json(args[0]))
                            .Append(_sqlExpressionFactory.Constant("one"))
                            .Concat(deconstructParamsArray(args[1])),
                        typeof(bool)),
                nameof(MySqlJsonDbFunctionsExtensions.JsonContainsPathAll)
                    => _sqlExpressionFactory.Function(
                        "JSON_CONTAINS_PATH",
                        Array.Empty<SqlExpression>()
                            .Append(json(args[0]))
                            .Append(_sqlExpressionFactory.Constant("all"))
                            .Concat(deconstructParamsArray(args[1])),
                        typeof(bool)),
                nameof(MySqlJsonDbFunctionsExtensions.JsonSearchAny)
                    => _sqlExpressionFactory.IsNotNull(
                        _sqlExpressionFactory.Function(
                            "JSON_SEARCH",
                            Array.Empty<SqlExpression>()
                                .Append(json(args[0]))
                                .Append(_sqlExpressionFactory.Constant("one"))
                                .Append(args[1])
                                .AppendIfTrue(args.Length >= 3, () => args.Length >= 4
                                    ? args[3]
                                    : _sqlExpressionFactory.Constant(null, RelationalTypeMapping.NullMapping))
                                .AppendIfTrue(args.Length >= 3, () => args[2]),
                            typeof(bool))),
                nameof(MySqlJsonDbFunctionsExtensions.JsonSearchAll)
                    => _sqlExpressionFactory.IsNotNull(
                        _sqlExpressionFactory.Function(
                            "JSON_SEARCH",
                            Array.Empty<SqlExpression>()
                                .Append(json(args[0]))
                                .Append(_sqlExpressionFactory.Constant("all"))
                                .Append(args[1])
                                .AppendIfTrue(args.Length >= 3, () => args.Length >= 4
                                    ? args[3]
                                    : _sqlExpressionFactory.Constant(null, RelationalTypeMapping.NullMapping))
                                .AppendIfTrue(args.Length >= 3, () => args[2]),
                            typeof(bool))) as SqlExpression,
                _ => null
            };

            return result;

            SqlExpression json(SqlExpression e) => _sqlExpressionFactory.ApplyTypeMapping(ensureJson(e), _sqlExpressionFactory.FindMapping(e.Type, "json"));

            static SqlExpression ensureJson(SqlExpression e)
                => e.TypeMapping is MySqlJsonTypeMapping ||
                   e is MySqlJsonTraversalExpression
                    ? e
                    : throw new InvalidOperationException("The JSON method requires a JSON parameter but none was found.");

            static SqlExpression removeConvert(SqlExpression e)
            {
                while (e is SqlUnaryExpression unary &&
                       (unary.OperatorType == ExpressionType.Convert || unary.OperatorType == ExpressionType.ConvertChecked))
                {
                    e = unary.Operand;
                }

                return e;
            }

            // MySqlJsonTraversalExpression withReturnsText(MySqlJsonTraversalExpression traversal, bool returnsText)
            //     => traversal.ReturnsText == returnsText
            //         ? traversal
            //         : returnsText
            //             ? traversal.Clone(returnsText, typeof(string), _stringTypeMapping)
            //             : traversal.Clone(returnsText, traversal.Type, traversal.Expression.TypeMapping);

            IEnumerable<SqlExpression> deconstructParamsArray(SqlExpression paramsArray)
            {
                if (paramsArray is SqlConstantExpression constant)
                {
                    if (constant.Value is object[] array)
                    {
                        foreach (var value in array)
                        {
                            yield return _sqlExpressionFactory.Constant(value);
                        }
                    }
                }
                else
                {
                    yield return paramsArray;
                }
            }
        }
    }
}
