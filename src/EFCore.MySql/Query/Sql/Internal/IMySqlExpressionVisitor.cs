// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using EFCore.MySql.Query.Expressions.Internal;
using JetBrains.Annotations;

namespace EFCore.MySql.Query.Sql.Internal
{
    public interface IMySqlExpressionVisitor
    {
        Expression VisitRegexp([NotNull] RegexpExpression regexpExpression);
    }
}
