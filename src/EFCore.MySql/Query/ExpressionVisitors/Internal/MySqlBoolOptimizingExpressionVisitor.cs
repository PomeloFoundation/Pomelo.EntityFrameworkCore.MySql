// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
    public class MySqlBoolOptimizingExpressionVisitor : SqlExpressionVisitor
    {
        private bool _optimize;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlBoolOptimizingExpressionVisitor(
            ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        private Expression ApplyConversion(SqlExpression sqlExpression, bool condition)
        {
            if (_optimize &&
                sqlExpression is ColumnExpression &&
                sqlExpression.TypeMapping is MySqlBoolTypeMapping &&
                sqlExpression.Type == typeof(bool)/* &&
                condition*/)
            {
                return _sqlExpressionFactory.Equal(sqlExpression, _sqlExpressionFactory.Constant(true));
            }

            return sqlExpression;
        }

        protected override Expression VisitCase(CaseExpression caseExpression)
        {
            var parentOptimize = _optimize;

            var testIsCondition = caseExpression.Operand == null;
            _optimize = false;
            var operand = (SqlExpression)Visit(caseExpression.Operand);
            var whenClauses = new List<CaseWhenClause>();
            foreach (var whenClause in caseExpression.WhenClauses)
            {
                _optimize = testIsCondition;
                var test = (SqlExpression)Visit(whenClause.Test);
                _optimize = false;
                var result = (SqlExpression)Visit(whenClause.Result);
                whenClauses.Add(new CaseWhenClause(test, result));
            }

            _optimize = false;
            var elseResult = (SqlExpression)Visit(caseExpression.ElseResult);

            _optimize = parentOptimize;

            return ApplyConversion(caseExpression.Update(operand, whenClauses, elseResult), condition: false);
        }

        protected override Expression VisitColumn(ColumnExpression columnExpression)
        {
            return ApplyConversion(columnExpression, condition: false);
        }

        protected override Expression VisitExists(ExistsExpression existsExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var subquery = (SelectExpression)Visit(existsExpression.Subquery);
            _optimize = parentOptimize;

            return ApplyConversion(existsExpression.Update(subquery), condition: true);
        }

        protected override Expression VisitFromSql(FromSqlExpression fromSqlExpression)
            => fromSqlExpression;

        protected override Expression VisitIn(InExpression inExpression)
        {
            var parentOptimize = _optimize;

            _optimize = false;
            var item = (SqlExpression)Visit(inExpression.Item);
            var subquery = (SelectExpression)Visit(inExpression.Subquery);
            var values = (SqlExpression)Visit(inExpression.Values);
            _optimize = parentOptimize;

            return ApplyConversion(inExpression.Update(item, values, subquery), condition: true);
        }

        protected override Expression VisitLike(LikeExpression likeExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var match = (SqlExpression)Visit(likeExpression.Match);
            var pattern = (SqlExpression)Visit(likeExpression.Pattern);
            var escapeChar = (SqlExpression)Visit(likeExpression.EscapeChar);
            _optimize = parentOptimize;

            return ApplyConversion(likeExpression.Update(match, pattern, escapeChar), condition: true);
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            var changed = false;
            var parentOptimize = _optimize;

            var projections = new List<ProjectionExpression>();
            _optimize = false;
            foreach (var item in selectExpression.Projection)
            {
                var updatedProjection = (ProjectionExpression)Visit(item);
                projections.Add(updatedProjection);
                changed |= updatedProjection != item;
            }

            var tables = new List<TableExpressionBase>();
            foreach (var table in selectExpression.Tables)
            {
                var newTable = (TableExpressionBase)Visit(table);
                changed |= newTable != table;
                tables.Add(newTable);
            }

            _optimize = true;
            var predicate = (SqlExpression)Visit(selectExpression.Predicate);
            changed |= predicate != selectExpression.Predicate;

            var groupBy = new List<SqlExpression>();
            _optimize = false;
            foreach (var groupingKey in selectExpression.GroupBy)
            {
                var newGroupingKey = (SqlExpression)Visit(groupingKey);
                changed |= newGroupingKey != groupingKey;
                groupBy.Add(newGroupingKey);
            }

            _optimize = true;
            var havingExpression = (SqlExpression)Visit(selectExpression.Having);
            changed |= havingExpression != selectExpression.Having;

            var orderings = new List<OrderingExpression>();
            _optimize = false;
            foreach (var ordering in selectExpression.Orderings)
            {
                var orderingExpression = (SqlExpression)Visit(ordering.Expression);
                changed |= orderingExpression != ordering.Expression;
                orderings.Add(ordering.Update(orderingExpression));
            }

            var offset = (SqlExpression)Visit(selectExpression.Offset);
            changed |= offset != selectExpression.Offset;

            var limit = (SqlExpression)Visit(selectExpression.Limit);
            changed |= limit != selectExpression.Limit;

            _optimize = parentOptimize;

            return changed
                ? selectExpression.Update(
                    projections, tables, predicate, groupBy, havingExpression, orderings, limit, offset, selectExpression.IsDistinct,
                    selectExpression.Alias)
                : selectExpression;
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            var parentOptimize = _optimize;
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
            if (_optimize &&
                (sqlBinaryExpression.OperatorType == ExpressionType.Equal || sqlBinaryExpression.OperatorType == ExpressionType.NotEqual) &&
                columnExpression != null &&
                sqlConstantExpression != null &&
                columnExpression.TypeMapping is MySqlBoolTypeMapping &&
                columnExpression.Type == typeof(bool) &&
                sqlConstantExpression.TypeMapping is MySqlBoolTypeMapping &&
                sqlConstantExpression.Type == typeof(bool))
            {
                _optimize = false;
            }

            var newLeft = (SqlExpression)Visit(sqlBinaryExpression.Left);
            var newRight = (SqlExpression)Visit(sqlBinaryExpression.Right);

            _optimize = parentOptimize;

            sqlBinaryExpression = sqlBinaryExpression.Update(newLeft, newRight);

            var condition = sqlBinaryExpression.OperatorType == ExpressionType.AndAlso
                            || sqlBinaryExpression.OperatorType == ExpressionType.OrElse
                            || sqlBinaryExpression.OperatorType == ExpressionType.Equal
                            || sqlBinaryExpression.OperatorType == ExpressionType.NotEqual
                            || sqlBinaryExpression.OperatorType == ExpressionType.GreaterThan
                            || sqlBinaryExpression.OperatorType == ExpressionType.GreaterThanOrEqual
                            || sqlBinaryExpression.OperatorType == ExpressionType.LessThan
                            || sqlBinaryExpression.OperatorType == ExpressionType.LessThanOrEqual;

            return ApplyConversion(sqlBinaryExpression, condition);
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
        {
            var parentOptimize = _optimize;
            bool resultCondition;
            switch (sqlUnaryExpression.OperatorType)
            {
                case ExpressionType.Not:
                    _optimize = true;
                    resultCondition = true;
                    break;

                case ExpressionType.Convert:
                case ExpressionType.Negate:
                    _optimize = false;
                    resultCondition = false;
                    break;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    _optimize = false;
                    resultCondition = true;
                    break;

                default:
                    throw new InvalidOperationException("Unknown operator type encountered in SqlUnaryExpression.");
            }

            SqlExpression expression;

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
                _optimize = false;

                expression = _sqlExpressionFactory.MakeBinary(
                    ExpressionType.Equal,
                    (SqlExpression)Visit(sqlUnaryExpression.Operand),
                    _sqlExpressionFactory.Constant(false),
                    sqlUnaryExpression.TypeMapping);
            }
            else
            {
                expression = sqlUnaryExpression.Update((SqlExpression)Visit(sqlUnaryExpression.Operand));
            }

            _optimize = parentOptimize;

            return ApplyConversion(expression, condition: resultCondition);
        }

        protected override Expression VisitSqlConstant(SqlConstantExpression sqlConstantExpression)
        {
            return ApplyConversion(sqlConstantExpression, condition: false);
        }

        protected override Expression VisitSqlFragment(SqlFragmentExpression sqlFragmentExpression)
            => sqlFragmentExpression;

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var instance = (SqlExpression)Visit(sqlFunctionExpression.Instance);
            var arguments = new SqlExpression[sqlFunctionExpression.Arguments.Count];
            for (var i = 0; i < arguments.Length; i++)
            {
                arguments[i] = (SqlExpression)Visit(sqlFunctionExpression.Arguments[i]);
            }

            _optimize = parentOptimize;
            var newFunction = sqlFunctionExpression.Update(instance, arguments);

            var condition = string.Equals(sqlFunctionExpression.Name, "FREETEXT")
                || string.Equals(sqlFunctionExpression.Name, "CONTAINS");

            return ApplyConversion(newFunction, condition);
        }

        protected override Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
        {
            return ApplyConversion(sqlParameterExpression, condition: false);
        }

        protected override Expression VisitTable(TableExpression tableExpression)
            => tableExpression;

        protected override Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            var expression = (SqlExpression)Visit(projectionExpression.Expression);

            return projectionExpression.Update(expression);
        }

        protected override Expression VisitOrdering(OrderingExpression orderingExpression)
        {
            var expression = (SqlExpression)Visit(orderingExpression.Expression);

            return orderingExpression.Update(expression);
        }

        protected override Expression VisitCrossJoin(CrossJoinExpression crossJoinExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var table = (TableExpressionBase)Visit(crossJoinExpression.Table);
            _optimize = parentOptimize;

            return crossJoinExpression.Update(table);
        }

        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var table = (TableExpressionBase)Visit(crossApplyExpression.Table);
            _optimize = parentOptimize;

            return crossApplyExpression.Update(table);
        }

        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var table = (TableExpressionBase)Visit(outerApplyExpression.Table);
            _optimize = parentOptimize;

            return outerApplyExpression.Update(table);
        }

        protected override Expression VisitInnerJoin(InnerJoinExpression innerJoinExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var table = (TableExpressionBase)Visit(innerJoinExpression.Table);
            _optimize = true;
            var joinPredicate = (SqlExpression)Visit(innerJoinExpression.JoinPredicate);
            _optimize = parentOptimize;

            return innerJoinExpression.Update(table, joinPredicate);
        }

        protected override Expression VisitLeftJoin(LeftJoinExpression leftJoinExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var table = (TableExpressionBase)Visit(leftJoinExpression.Table);
            _optimize = true;
            var joinPredicate = (SqlExpression)Visit(leftJoinExpression.JoinPredicate);
            _optimize = parentOptimize;

            return leftJoinExpression.Update(table, joinPredicate);
        }

        protected override Expression VisitSubSelect(ScalarSubqueryExpression scalarSubqueryExpression)
        {
            var parentOptimize = _optimize;
            var subquery = (SelectExpression)Visit(scalarSubqueryExpression.Subquery);
            _optimize = parentOptimize;

            return ApplyConversion(scalarSubqueryExpression.Update(subquery), condition: false);
        }

        protected override Expression VisitRowNumber(RowNumberExpression rowNumberExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var changed = false;
            var partitions = new List<SqlExpression>();
            foreach (var partition in rowNumberExpression.Partitions)
            {
                var newPartition = (SqlExpression)Visit(partition);
                changed |= newPartition != partition;
                partitions.Add(newPartition);
            }

            var orderings = new List<OrderingExpression>();
            foreach (var ordering in rowNumberExpression.Orderings)
            {
                var newOrdering = (OrderingExpression)Visit(ordering);
                changed |= newOrdering != ordering;
                orderings.Add(newOrdering);
            }

            _optimize = parentOptimize;

            return ApplyConversion(rowNumberExpression.Update(partitions, orderings), condition: false);
        }

        protected override Expression VisitExcept(ExceptExpression exceptExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var source1 = (SelectExpression)Visit(exceptExpression.Source1);
            var source2 = (SelectExpression)Visit(exceptExpression.Source2);
            _optimize = parentOptimize;

            return exceptExpression.Update(source1, source2);
        }

        protected override Expression VisitIntersect(IntersectExpression intersectExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var source1 = (SelectExpression)Visit(intersectExpression.Source1);
            var source2 = (SelectExpression)Visit(intersectExpression.Source2);
            _optimize = parentOptimize;

            return intersectExpression.Update(source1, source2);
        }

        protected override Expression VisitUnion(UnionExpression unionExpression)
        {
            var parentOptimize = _optimize;
            _optimize = false;
            var source1 = (SelectExpression)Visit(unionExpression.Source1);
            var source2 = (SelectExpression)Visit(unionExpression.Source2);
            _optimize = parentOptimize;

            return unionExpression.Update(source1, source2);
        }
    }
}
