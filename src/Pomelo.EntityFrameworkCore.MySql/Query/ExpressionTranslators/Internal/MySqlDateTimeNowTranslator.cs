// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlDateTimeNowTranslator : IMemberTranslator
    {
        public virtual Expression Translate([NotNull] MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime))
            {
                if (memberExpression.Member.Name == nameof(DateTime.Now))
                {
                    return new SqlFunctionExpression("NOW", memberExpression.Type);
                }
                else if (memberExpression.Member.Name == nameof(DateTime.UtcNow))
                {
                    return new AtTimeZoneExpression(new SqlFunctionExpression("NOW", memberExpression.Type), "UTC");
                }
            }

            return null;
        }
    }
}
