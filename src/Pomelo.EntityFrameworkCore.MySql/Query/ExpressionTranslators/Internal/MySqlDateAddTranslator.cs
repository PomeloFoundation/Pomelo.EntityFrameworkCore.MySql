// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlDateAddTranslator : IMethodCallTranslator
    {
        public virtual Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            string dateTimePart;
            if (methodCallExpression.Method.DeclaringType == typeof(DateTime)
                && (dateTimePart = GetDatePart(methodCallExpression.Method.Name)) != null)
            {
                return new DateAddExpression(dateTimePart, methodCallExpression.Type, new[] {
                        methodCallExpression.Object,
                        methodCallExpression.Arguments.First()
                });
            }
            return null;
        }

        private static string GetDatePart(string memberName)
        {
            switch (memberName)
            {
                case nameof(DateTime.AddYears):
                    return "YEAR";
                case nameof(DateTime.AddMonths):
                    return "MONTH";
                case nameof(DateTime.AddDays):
                    return "DAY";
                case nameof(DateTime.AddHours):
                    return "HOUR";
                case nameof(DateTime.AddMinutes):
                    return "MINUTE";
                case nameof(DateTime.AddSeconds):
                    return "SECOND";
                default:
                    return null;
            }
        }
    }
}
