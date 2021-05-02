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
    public class MySqlMathMethodTranslator : IMethodCallTranslator
    {
        private static readonly IDictionary<MethodInfo, (string Name, bool OnlyNullByArgs)> _methodToFunctionName = new Dictionary<MethodInfo, (string Name, bool OnlyNullByArgs)>
        {
            { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(decimal) }), ("ABS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(double) }), ("ABS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(float) }), ("ABS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(int) }), ("ABS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(long) }), ("ABS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Abs), new[] { typeof(short) }), ("ABS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Acos), new[] { typeof(double) }), ("ACOS", false) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Asin), new[] { typeof(double) }), ("ASIN", false) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Atan), new[] { typeof(double) }), ("ATAN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Atan2), new[] { typeof(double), typeof(double) }), ("ATAN2", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Ceiling), new[] { typeof(decimal) }), ("CEILING", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Ceiling), new[] { typeof(double) }), ("CEILING", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Cos), new[] { typeof(double) }), ("COS", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Exp), new[] { typeof(double) }), ("EXP", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Floor), new[] { typeof(decimal) }), ("FLOOR", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Floor), new[] { typeof(double) }), ("FLOOR", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Log), new[] { typeof(double) }), ("LOG", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Log), new[] { typeof(double), typeof(double) }), ("LOG", false) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Log10), new[] { typeof(double) }), ("LOG10", false) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(decimal), typeof(decimal) }), ("GREATEST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(double), typeof(double) }), ("GREATEST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(float), typeof(float) }), ("GREATEST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) }), ("GREATEST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(long), typeof(long) }), ("GREATEST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Max), new[] { typeof(short), typeof(short) }), ("GREATEST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(decimal), typeof(decimal) }), ("LEAST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(double), typeof(double) }), ("LEAST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(float), typeof(float) }), ("LEAST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(int), typeof(int) }), ("LEAST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(long), typeof(long) }), ("LEAST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Min), new[] { typeof(short), typeof(short) }), ("LEAST", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Pow), new[] { typeof(double), typeof(double) }), ("POWER", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(double) }), ("ROUND", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(double), typeof(int) }), ("ROUND", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(decimal) }), ("ROUND", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Round), new[] { typeof(decimal), typeof(int) }), ("ROUND", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(decimal) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(double) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(float) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(int) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(long) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(sbyte) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sign), new[] { typeof(short) }), ("SIGN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sin), new[] { typeof(double) }), ("SIN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Sqrt), new[] { typeof(double) }), ("SQRT", false) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Tan), new[] { typeof(double) }), ("TAN", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Truncate), new[] { typeof(double) }), ("TRUNCATE", true) },
            { typeof(Math).GetRuntimeMethod(nameof(Math.Truncate), new[] { typeof(decimal) }), ("TRUNCATE", true) },
        };

        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        public MySqlMathMethodTranslator(MySqlSqlExpressionFactory sqlExpressionFactory)
            => _sqlExpressionFactory = sqlExpressionFactory;

        /// <inheritdoc />
        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_methodToFunctionName.TryGetValue(method, out var mapping))
            {
                var targetArgumentsCount = arguments.Count;

                if (mapping.Name == "TRUNCATE")
                {
                    targetArgumentsCount = 2;
                }

                var newArguments = new SqlExpression[targetArgumentsCount];
                newArguments[0] = arguments[0];

                if (targetArgumentsCount == 2)
                {
                    if (arguments.Count == 2)
                    {
                        newArguments[1] = arguments[1];
                    }
                    else
                    {
                        newArguments[1] = _sqlExpressionFactory.Constant(0);
                    }
                }

                return _sqlExpressionFactory.NullableFunction(
                    mapping.Name,
                    newArguments,
                    method.ReturnType,
                    mapping.OnlyNullByArgs);
            }

            return null;
        }
    }
}
