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
    public class MySqlDatePartTranslator : IMemberTranslator
    {
        public virtual Expression Translate([NotNull] MemberExpression memberExpression)
        {
            string datePart;
            if (memberExpression.Expression.Type == typeof(DateTime)
                && (datePart = GetDatePart(memberExpression.Member.Name)) != null)
            {
                return new DatePartExpression(datePart, memberExpression.Type, memberExpression.Expression);
            }
            return null;
        }

        private static string GetDatePart(string memberName)
        {
            switch (memberName)
            {
                case nameof(DateTime.Year):
                    return "YEAR";
                case nameof(DateTime.Month):
                    return "MONTH";
                case nameof(DateTime.Day):
                    return "DAY";
                case nameof(DateTime.Hour):
                    return "HOUR";
                case nameof(DateTime.Minute):
                    return "MINUTE";
                case nameof(DateTime.Second):
                    return "SECOND";
                default:
                    return null;
            }
        }
    }
}
