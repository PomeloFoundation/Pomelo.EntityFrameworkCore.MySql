// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDateTimeNowTranslator : IMemberTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlDateTimeNowTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            if (instance == null
                && member.DeclaringType == typeof(DateTime))
            {
                switch (member.Name)
                {
                    case nameof(DateTime.Now):
                        return _sqlExpressionFactory.Function("CURRENT_TIMESTAMP", returnType);
                    case nameof(DateTime.UtcNow):
                        return _sqlExpressionFactory.Function("UTC_TIMESTAMP", returnType);
                    case nameof(DateTime.Today):
                        return _sqlExpressionFactory.Function("CURDATE", returnType);
                }
            }

            return null;
        }
    }
}
