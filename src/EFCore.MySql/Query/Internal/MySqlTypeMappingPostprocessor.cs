// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microting.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Microting.EntityFrameworkCore.MySql.Query.Internal;

/// <summary>
///     Applies type mappings which could only be inferred after translation back onto their query roots.
///     For MySQL this is relevant for parameter collections, which are translated to <c>JSON_TABLE()</c> without a
///     <c>COLUMNS</c> clause, since the element type mapping is unknown at translation time.
/// </summary>
public class MySqlTypeMappingPostprocessor : RelationalTypeMappingPostprocessor
{
    private readonly IModel _model;
    private readonly IRelationalTypeMappingSource _typeMappingSource;

    public MySqlTypeMappingPostprocessor(
        QueryTranslationPostprocessorDependencies dependencies,
        RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
        RelationalQueryCompilationContext queryCompilationContext)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
        _model = queryCompilationContext.Model;
        _typeMappingSource = relationalDependencies.TypeMappingSource;
    }

    /// <inheritdoc />
    protected override Expression VisitExtension(Expression expression)
        => expression is MySqlJsonTableExpression jsonTableExpression
            ? ApplyTypeMappingsOnJsonTableExpression(jsonTableExpression)
            : base.VisitExtension(expression);

    /// <summary>
    ///     Applies the inferred element type mapping onto a <c>JSON_TABLE()</c> expression whose JSON source is an untyped
    ///     parameter, adding the <c>COLUMNS</c> clause which performs the JSON to relational type conversion.
    /// </summary>
    protected virtual MySqlJsonTableExpression ApplyTypeMappingsOnJsonTableExpression(
        MySqlJsonTableExpression jsonTableExpression)
    {
        // The element type mapping is already known for collection columns; only parameter collections need inference.
        if (jsonTableExpression.JsonExpression is not SqlParameterExpression parameterExpression ||
            parameterExpression.TypeMapping is not null)
        {
            return (MySqlJsonTableExpression)base.VisitExtension(jsonTableExpression);
        }

        RelationalTypeMapping parameterTypeMapping;

        if (TryGetInferredTypeMapping(jsonTableExpression.Alias, "value", out var elementTypeMapping))
        {
            parameterTypeMapping = _typeMappingSource.FindMapping(parameterExpression.Type, _model, elementTypeMapping);
        }
        else
        {
            parameterTypeMapping = _typeMappingSource.FindMapping(parameterExpression.Type, _model);
            elementTypeMapping = parameterTypeMapping?.ElementTypeMapping as RelationalTypeMapping;
        }

        if (parameterTypeMapping is null ||
            elementTypeMapping is null)
        {
            return (MySqlJsonTableExpression)base.VisitExtension(jsonTableExpression);
        }

        return jsonTableExpression.Update(
            parameterExpression.ApplyTypeMapping(parameterTypeMapping),
            jsonTableExpression.Path,
            [
                new MySqlJsonTableExpression.ColumnInfo(
                    "value",
                    elementTypeMapping,
                    [new PathSegment(new SqlConstantExpression(0, typeof(int), _typeMappingSource.FindMapping(typeof(int))))])
            ]);
    }
}
