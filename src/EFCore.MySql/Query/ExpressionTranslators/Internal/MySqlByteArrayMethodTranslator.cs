// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    public class MySqlByteArrayMethodTranslator : IMethodCallTranslator
    {
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        private static readonly MethodInfo _containsMethod = typeof(Enumerable)
            .GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

        public MySqlByteArrayMethodTranslator([NotNull] MySqlSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            Check.NotNull(method, nameof(method));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(logger, nameof(logger));

            if (method.IsGenericMethod
                && method.GetGenericMethodDefinition().Equals(_containsMethod)
                && arguments[0].Type == typeof(byte[]))
            {
                var source = arguments[0];
                var sourceTypeMapping = source.TypeMapping;

                var value = arguments[1] is SqlConstantExpression constantValue
                    ? (SqlExpression)_sqlExpressionFactory.Constant(new[] { (byte)constantValue.Value }, sourceTypeMapping)
                    : _sqlExpressionFactory.Convert(arguments[1], typeof(byte[]), sourceTypeMapping);

                return _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.NullableFunction(
                        "LOCATE",
                        new[] { value, source },
                        typeof(int)),
                    _sqlExpressionFactory.Constant(0));
            }

            return null;
        }
    }
}
