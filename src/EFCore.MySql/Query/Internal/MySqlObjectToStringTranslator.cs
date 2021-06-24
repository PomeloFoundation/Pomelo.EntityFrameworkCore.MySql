// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        private static readonly List<Type> _supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(long),
            typeof(DateTime),
            typeof(Guid),
            typeof(bool),
            typeof(byte),
            typeof(byte[]),
            typeof(double),
            typeof(DateTimeOffset),
            typeof(char),
            typeof(short),
            typeof(float),
            typeof(decimal),
            typeof(TimeSpan),
            typeof(uint),
            typeof(ushort),
            typeof(ulong),
            typeof(sbyte),
        };
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlObjectToStringTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (instance == null ||
                method.Name != nameof(ToString) ||
                arguments.Count != 0)
            {
                return null;
            }

            if (instance.Type == typeof(bool))
            {
                return instance is ColumnExpression columnExpression &&
                       columnExpression.IsNullable
                    ? _sqlExpressionFactory.Case(
                        new[]
                        {
                            new CaseWhenClause(
                                _sqlExpressionFactory.Equal(instance, _sqlExpressionFactory.Constant(false)),
                                _sqlExpressionFactory.Constant(false.ToString())),
                            new CaseWhenClause(
                                _sqlExpressionFactory.Equal(instance, _sqlExpressionFactory.Constant(true)),
                                _sqlExpressionFactory.Constant(true.ToString()))
                        },
                        _sqlExpressionFactory.Constant(null))
                    : _sqlExpressionFactory.Case(
                        new[]
                        {
                            new CaseWhenClause(
                                _sqlExpressionFactory.Equal(instance, _sqlExpressionFactory.Constant(false)),
                                _sqlExpressionFactory.Constant(false.ToString()))
                        },
                        _sqlExpressionFactory.Constant(true.ToString()));
            }

            // Translates parameterless Object.ToString() calls.
            return _supportedTypes.Contains(instance.Type.UnwrapNullableType())
                ? _sqlExpressionFactory.Convert(instance, typeof(string))
                : null;
        }
    }
}
