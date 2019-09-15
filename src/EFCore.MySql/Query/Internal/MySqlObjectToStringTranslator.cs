// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlObjectToStringTranslator : IMethodCallTranslator
    {
        private const int DefaultLength = 127;

        private static readonly Dictionary<Type, string> _typeMapping
            = new Dictionary<Type, string>
            {
                { typeof(int), "CHAR(11)" },
                { typeof(long), "CHAR(20)" },
                { typeof(DateTime), $"CHAR({DefaultLength})" },
                { typeof(Guid), "CHAR(36)" },
                { typeof(bool), "CHAR(5)" },
                { typeof(byte), "CHAR(3)" },
                { typeof(byte[]), $"CHAR({DefaultLength})" },
                { typeof(double), $"CHAR({DefaultLength})" },
                { typeof(DateTimeOffset), $"CHAR({DefaultLength})" },
                { typeof(char), "CHAR(1)" },
                { typeof(short), "CHAR(6)" },
                { typeof(float), $"CHAR({DefaultLength})" },
                { typeof(decimal), $"CHAR({DefaultLength})" },
                { typeof(TimeSpan), $"CHAR({DefaultLength})" },
                { typeof(uint), "CHAR(10)" },
                { typeof(ushort), "CHAR(5)" },
                { typeof(ulong), "CHAR(19)" },
                { typeof(sbyte), "CHAR(4)" }
            };
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlObjectToStringTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            // Translates parameterless Object.ToString() calls.
            return method.Name == nameof(ToString)
                   && arguments.Count == 0
                   && instance != null
                   && _typeMapping.TryGetValue(
                       instance.Type
                           .UnwrapNullableType(),
                       out var storeType)
                ? _sqlExpressionFactory.Function(
                    "CONVERT",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Fragment(storeType)
                    },
                    typeof(string))
                : null;
        }
    }
}
