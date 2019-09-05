// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using JetBrains.Annotations;

namespace Pomelo.EntityFrameworkCore.MySql.Query
{
    public interface IMySqlExpressionVisitor
    {
        Expression VisitRegexp([NotNull] RegexpExpression regexpExpression);
        Expression VisitMySqlComplexFunctionArgumentExpression([NotNull] MySqlComplexFunctionArgumentExpression mySqlComplexFunctionArgumentExpression);
        Expression VisitMySqlCollateExpression([NotNull] MySqlCollateExpression mySqlCollateExpression);
        Expression VisitMySqlBinaryExpression([NotNull] MySqlBinaryExpression mySqlBinaryExpression);
    }
}
