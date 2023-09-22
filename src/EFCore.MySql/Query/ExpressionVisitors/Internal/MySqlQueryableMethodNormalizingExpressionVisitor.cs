// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

/// <summary>
/// Skips normalization of array[index].Property to array.Select(e => e.Property).ElementAt(index),
/// because it messes-up our JSON-Array handling in `MySqlSqlTranslatingExpressionVisitor`.
/// See https://github.com/dotnet/efcore/issues/30386.
/// </summary>
public class MySqlQueryableMethodNormalizingExpressionVisitor : QueryableMethodNormalizingExpressionVisitor
{
    public MySqlQueryableMethodNormalizingExpressionVisitor(QueryCompilationContext queryCompilationContext)
        : base(queryCompilationContext)
    {
    }

    protected override Expression VisitBinary(BinaryExpression binaryExpression)
    {
        // Convert array[x] to array.ElementAt(x)
        if (binaryExpression is
            {
                NodeType: ExpressionType.ArrayIndex,
                Left: var source,
                Right: var index
            })
        {
            return binaryExpression;

            // Original (base) implementation:
            //
            // return VisitMethodCall(
            //     Expression.Call(
            //         ElementAtMethodInfo.MakeGenericMethod(source.Type.GetSequenceType()), source, index));
        }

        return base.VisitBinary(binaryExpression);
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        // Normalize list[x] to list.ElementAt(x)
        if (methodCallExpression is
            {
                Method:
                {
                    Name: "get_Item",
                    IsStatic: false,
                    DeclaringType: Type declaringType
                },
                Object: Expression indexerSource,
                Arguments: [var index]
            }
            && declaringType.GetInterface("IReadOnlyList`1") is not null)
        {
            return methodCallExpression;

            // Original (base) implementation:
            //
            // return VisitMethodCall(
            //     Expression.Call(
            //         ElementAtMethodInfo.MakeGenericMethod(indexerSource.Type.GetSequenceType()),
            //         indexerSource,
            //         index));
        }

        return base.VisitMethodCall(methodCallExpression);
    }
}
