// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDateTimeDateComponentTranslator : IMemberTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlDateTimeDateComponentTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            var declaringType = member.DeclaringType;
            if (instance != null)
            {
                if ((declaringType == typeof(DateTime) || declaringType == typeof(DateTimeOffset)) && member.Name == nameof(DateTime.Date))
                {
                    return _sqlExpressionFactory.Function(
                                  "CONVERT",
                                  new[]{
                                      instance,
                                      _sqlExpressionFactory.Fragment("date")
                                  },
                                  returnType,
                                  instance.TypeMapping);
                }
            }
            return null;
        }
    }
}
