// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlLikeTranslator : LikeTranslator, IMethodCallTranslator
    {
        private static readonly Type[] _supportedTypes = {
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
            typeof(int?),
            typeof(long?),
            typeof(DateTime?),
            typeof(Guid?),
            typeof(bool?),
            typeof(byte?),
            typeof(double?),
            typeof(DateTimeOffset?),
            typeof(char?),
            typeof(short?),
            typeof(float?),
            typeof(decimal?),
            typeof(TimeSpan?),
            typeof(uint?),
            typeof(ushort?),
            typeof(ulong?),
            typeof(sbyte?),
        };

        private static readonly MethodInfo[] _methodInfos
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethods()
                .Where(method => method.Name == nameof(MySqlDbFunctionsExtensions.Like)
                                 && method.IsGenericMethod)
                .SelectMany(method => _supportedTypes.Select(type => method.MakeGenericMethod(type))).ToArray();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Expression Translate(MethodCallExpression methodCallExpression)
        {
            return _methodInfos.Any(method => Equals(methodCallExpression.Method, method))
                       ? methodCallExpression.Method.GetParameters().Length == 3
                           ? new LikeExpression(methodCallExpression.Arguments[1], methodCallExpression.Arguments[2])
                           : new LikeExpression(methodCallExpression.Arguments[1], methodCallExpression.Arguments[2], methodCallExpression.Arguments[3])
                       : base.Translate(methodCallExpression);
        }
    }
}