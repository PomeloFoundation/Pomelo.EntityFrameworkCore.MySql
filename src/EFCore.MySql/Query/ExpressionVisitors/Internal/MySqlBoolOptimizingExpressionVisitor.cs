// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    /// <summary>
    /// "WHERE `boolColumn`" doesn't use available indices, while "WHERE `boolColumn` = TRUE" does.
    /// See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1104
    /// </summary>
    public class MySqlBoolOptimizingExpressionVisitor : ExpressionVisitor
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private bool _skipExplicitTrueValue;

        public MySqlBoolOptimizingExpressionVisitor(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            return extensionExpression switch
            {
                ProjectionExpression projectionExpression => VisitProjectionExpression(projectionExpression),
                ColumnExpression columnExpression => VisitColumn(columnExpression),
                SqlUnaryExpression sqlUnaryExpression => VisitSqlUnaryExpression(sqlUnaryExpression),
                SqlBinaryExpression sqlBinaryExpression => VisitSqlBinaryExpression(sqlBinaryExpression),
                _ => base.VisitExtension(extensionExpression)
            };
        }

        protected virtual Expression VisitProjectionExpression(ProjectionExpression projectionExpression)
        {
            var oldSkipExplicitTrueValue = _skipExplicitTrueValue;

            try
            {
                _skipExplicitTrueValue = true;
                return projectionExpression.Update((SqlExpression)Visit(projectionExpression.Expression));
            }
            finally
            {
                _skipExplicitTrueValue = oldSkipExplicitTrueValue;
            }
        }

        private Expression VisitSqlUnaryExpression(SqlUnaryExpression sqlUnaryExpression)
        {
            var oldSkipExplicitTrueValue = _skipExplicitTrueValue;

            // Optimize translation of the following expressions:
            //     context.Table.Where(t => !t.BoolColumn)
            //         translate to: `boolColumn` = FALSE
            //         instead of:   NOT(`boolColumn` = TRUE)
            // Translating to "NOT(`boolColumn`)" would not use indices in MySQL 5.7.
            if (sqlUnaryExpression.OperatorType == ExpressionType.Not &&
                sqlUnaryExpression.Operand is ColumnExpression columnExpression &&
                columnExpression.TypeMapping is MySqlBoolTypeMapping &&
                columnExpression.Type == typeof(bool))
            {
                _skipExplicitTrueValue = true;

                try
                {
                    var newOperand = (SqlExpression)Visit(sqlUnaryExpression.Operand);

                    return _sqlExpressionFactory.MakeBinary(
                        ExpressionType.Equal,
                        newOperand,
                        _sqlExpressionFactory.Constant(false),
                        sqlUnaryExpression.TypeMapping);
                }
                finally
                {
                    _skipExplicitTrueValue = oldSkipExplicitTrueValue;
                }
            }

            return sqlUnaryExpression.Update((SqlExpression)Visit(sqlUnaryExpression.Operand));
        }

        private Expression VisitSqlBinaryExpression(SqlBinaryExpression sqlBinaryExpression)
        {
            var oldSkipExplicitTrueValue = _skipExplicitTrueValue;
            var columnExpression = sqlBinaryExpression.Left as ColumnExpression ?? sqlBinaryExpression.Right as ColumnExpression;
            var sqlConstantExpression = sqlBinaryExpression.Left as SqlConstantExpression ?? sqlBinaryExpression.Right as SqlConstantExpression;

            // Optimize translation of the following expressions:
            //     context.Table.Where(t => t.BoolColumn == true)
            //         translate to: `boolColumn` = TRUE
            //         instead of:   (`boolColumn` = TRUE) = TRUE
            //     context.Table.Where(t => t.BoolColumn == false)
            //         translate to: `boolColumn` = FALSE
            //         instead of:   (`boolColumn` = TRUE) = FALSE
            //     context.Table.Where(t => t.BoolColumn != true)
            //         translate to: `boolColumn` <> TRUE
            //         instead of:   (`boolColumn` = TRUE) <> TRUE
            //     context.Table.Where(t => t.BoolColumn != false)
            //         translate to: `boolColumn` <> FALSE
            //         instead of:   (`boolColumn` = TRUE) <> FALSE
            if (!_skipExplicitTrueValue &&
                (sqlBinaryExpression.OperatorType == ExpressionType.Equal || sqlBinaryExpression.OperatorType == ExpressionType.NotEqual) &&
                columnExpression != null &&
                sqlConstantExpression != null &&
                columnExpression.TypeMapping is MySqlBoolTypeMapping &&
                columnExpression.Type == typeof(bool) &&
                sqlConstantExpression.TypeMapping is MySqlBoolTypeMapping &&
                sqlConstantExpression.Type == typeof(bool))
            {
                _skipExplicitTrueValue = true;
            }

            try
            {
                var newLeft = (SqlExpression)Visit(sqlBinaryExpression.Left);
                var newRight = (SqlExpression)Visit(sqlBinaryExpression.Right);

                return _sqlExpressionFactory.MakeBinary(
                    sqlBinaryExpression.OperatorType,
                    newLeft,
                    newRight,
                    sqlBinaryExpression.TypeMapping);
            }
            finally
            {
                _skipExplicitTrueValue = oldSkipExplicitTrueValue;
            }
        }

        protected virtual Expression VisitColumn(ColumnExpression columnExpression)
            => columnExpression.TypeMapping is MySqlBoolTypeMapping &&
               columnExpression.Type == typeof(bool) &&
               !_skipExplicitTrueValue
                ? (Expression)_sqlExpressionFactory.Equal(columnExpression, _sqlExpressionFactory.Constant(true))
                : columnExpression;
    }
}
