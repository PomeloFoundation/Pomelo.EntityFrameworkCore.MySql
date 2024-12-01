// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

/// <summary>
///     <para>MySQL implicitly casts numbers used in all bitwise operations to BIGINT UNSIGNED.</para>
///     <para>Bitwise operations are:</para>
///     <list type="table">
///         <listheader>
///             <term>Operator</term>
///             <description>Description</description>
///         </listheader>
///         <item>
///             <term>&amp;</term>
///             <description>Bitwise AND</description>
///         </item>
///         <item>
///             <term>&gt;&gt;</term>
///             <description>Right shift</description>
///         </item>
///         <item>
///             <term>&lt;&lt;</term>
///             <description>Left shift</description>
///         </item>
///         <item>
///             <term>^</term>
///             <description>Bitwise OR</description>
///         </item>
///         <item>
///             <term>~</term>
///             <description>Bitwise inversion</description>
///         </item>
///     </list>
///     <para>We need to cast them back to their expected type.</para>
/// </summary>
public class BitwiseOperationReturnTypeCorrectingExpressionVisitor : ExpressionVisitor
{
    private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

    public BitwiseOperationReturnTypeCorrectingExpressionVisitor(MySqlSqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            SqlUnaryExpression unaryExpression => VisitUnary(unaryExpression),
            SqlBinaryExpression binaryExpression => VisitBinary(binaryExpression),
            ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.UpdateQueryExpression(Visit(shapedQueryExpression.QueryExpression)),
            _ => base.VisitExtension(extensionExpression)
        };

    protected virtual Expression VisitUnary(SqlUnaryExpression sqlUnaryExpression)
        => base.VisitExtension(sqlUnaryExpression) is var visitedExpression &&
           visitedExpression is SqlUnaryExpression { OperatorType: ExpressionType.Not } visitedSqlUnaryExpression &&
           visitedSqlUnaryExpression.Type != typeof(bool)
            ? _sqlExpressionFactory.Convert(
                visitedSqlUnaryExpression,
                visitedSqlUnaryExpression.Type,
                visitedSqlUnaryExpression.TypeMapping)
            : visitedExpression;

    protected virtual Expression VisitBinary(SqlBinaryExpression sqlBinaryExpression)
        => base.VisitExtension(sqlBinaryExpression) is var visitedExpression &&
           visitedExpression is SqlBinaryExpression
           {
               OperatorType: ExpressionType.And
               or ExpressionType.RightShift
               or ExpressionType.LeftShift
               or ExpressionType.ExclusiveOr
               or ExpressionType.Or
               or ExpressionType.Not
           } visitedSqlBinaryExpression &&
           visitedSqlBinaryExpression.Type != typeof(bool)
            ? _sqlExpressionFactory.Convert(
                visitedSqlBinaryExpression,
                visitedSqlBinaryExpression.Type,
                visitedSqlBinaryExpression.TypeMapping)
            : visitedExpression;
}
