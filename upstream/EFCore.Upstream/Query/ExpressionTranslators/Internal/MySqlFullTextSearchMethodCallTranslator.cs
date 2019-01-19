// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlFullTextSearchMethodCallTranslator : IMethodCallTranslator
    {
        private const string FreeTextFunctionName = "FREETEXT";
        private const string ContainsFunctionName = "CONTAINS";

        private static readonly MethodInfo _freeTextMethodInfo
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                nameof(MySqlDbFunctionsExtensions.FreeText),
                new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static readonly MethodInfo _freeTextMethodInfoWithLanguage
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                nameof(MySqlDbFunctionsExtensions.FreeText),
                new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int) });

        private static readonly MethodInfo _containsMethodInfo
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                nameof(MySqlDbFunctionsExtensions.Contains),
                new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static readonly MethodInfo _containsMethodInfoWithLanguage
            = typeof(MySqlDbFunctionsExtensions).GetRuntimeMethod(
                nameof(MySqlDbFunctionsExtensions.Contains),
                new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int) });

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));

            if (Equals(methodCallExpression.Method, _freeTextMethodInfo))
            {
                ValidatePropertyReference(methodCallExpression.Arguments[1]);

                return new SqlFunctionExpression(
                    FreeTextFunctionName,
                    typeof(bool),
                    new[]
                    {
                        methodCallExpression.Arguments[1],
                        methodCallExpression.Arguments[2]
                    });
            }

            if (Equals(methodCallExpression.Method, _freeTextMethodInfoWithLanguage))
            {
                ValidatePropertyReference(methodCallExpression.Arguments[1]);

                return new SqlFunctionExpression(
                    FreeTextFunctionName,
                    typeof(bool),
                    new[]
                    {
                        methodCallExpression.Arguments[1],
                        methodCallExpression.Arguments[2],
                        new SqlFragmentExpression(
                            $"LANGUAGE {((ConstantExpression)methodCallExpression.Arguments[3]).Value}")
                    });
            }

            if (Equals(methodCallExpression.Method, _containsMethodInfo))
            {
                ValidatePropertyReference(methodCallExpression.Arguments[1]);

                return new SqlFunctionExpression(
                    ContainsFunctionName,
                    typeof(bool),
                    new[]
                    {
                        methodCallExpression.Arguments[1],
                        methodCallExpression.Arguments[2]
                    });
            }

            if (Equals(methodCallExpression.Method, _containsMethodInfoWithLanguage))
            {
                ValidatePropertyReference(methodCallExpression.Arguments[1]);

                return new SqlFunctionExpression(
                    ContainsFunctionName,
                    typeof(bool),
                    new[]
                    {
                        methodCallExpression.Arguments[1],
                        methodCallExpression.Arguments[2],
                        new SqlFragmentExpression(
                            $"LANGUAGE {((ConstantExpression)methodCallExpression.Arguments[3]).Value}")
                    });
            }

            return null;
        }

        private static void ValidatePropertyReference(Expression expression)
        {
            expression = expression.RemoveConvert();
            if (expression is NullableExpression nullableExpression)
            {
                expression = nullableExpression.Operand;
            }

            if (!(expression is ColumnExpression))
            {
                throw new InvalidOperationException(MySqlStrings.InvalidColumnNameForFreeText);
            }
        }
    }
}
