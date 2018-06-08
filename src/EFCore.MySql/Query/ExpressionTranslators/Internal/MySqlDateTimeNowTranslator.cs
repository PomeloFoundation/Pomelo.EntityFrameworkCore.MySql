// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDateTimeNowTranslator : IMemberTranslator
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime))
            {
                switch (memberExpression.Member.Name)
                {
                    case nameof(DateTime.Now):
                        return new SqlFunctionExpression("CURRENT_TIMESTAMP", memberExpression.Type);
                    case nameof(DateTime.UtcNow):
                        return new SqlFunctionExpression("UTC_TIMESTAMP", memberExpression.Type);
                    case nameof(DateTime.Today):
                        return new SqlFunctionExpression("CURDATE", memberExpression.Type);
                }
            }

            return null;
        }
    }
}
