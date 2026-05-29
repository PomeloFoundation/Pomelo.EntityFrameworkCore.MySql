// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microting.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Microting.EntityFrameworkCore.MySql.Query.Internal;

public class MySqlQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
{
    private const string Issue1792SkipWithParameterFlagName = "Microting.EntityFrameworkCore.MySql.Issue1792.SkipWithParameter";

    private static readonly bool _mySql8EngineCrashWhenUsingJsonTableWithPrimitiveCollectionInParametersSkip
        = AppContext.TryGetSwitch(Issue1792SkipWithParameterFlagName, out var enabled) && enabled;

    private readonly IMySqlOptions _options;
    private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly SqlAliasManager _sqlAliasManager;

    public MySqlQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        RelationalQueryCompilationContext relationalQueryCompilationContext,
        IMySqlOptions options)
        : base(dependencies, relationalDependencies, relationalQueryCompilationContext)
    {
        _sqlExpressionFactory = (MySqlSqlExpressionFactory)relationalDependencies.SqlExpressionFactory;
        _typeMappingSource = relationalDependencies.TypeMappingSource;
        _sqlAliasManager = relationalQueryCompilationContext.SqlAliasManager;
        _options = options;
    }

    protected MySqlQueryableMethodTranslatingExpressionVisitor(
        MySqlQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
        _sqlExpressionFactory = parentVisitor._sqlExpressionFactory;
        _typeMappingSource = parentVisitor._typeMappingSource;
        _sqlAliasManager = parentVisitor._sqlAliasManager;
        _options = parentVisitor._options;
    }

    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new MySqlQueryableMethodTranslatingExpressionVisitor(this);

    protected override bool IsNaturallyOrdered(SelectExpression selectExpression)
    {
        return selectExpression is
               {
                   Tables: [var mainTable, ..],
                   Orderings:
                   [
                       {
                           Expression: ColumnExpression { Name: "key", TableAlias: var orderingTable } orderingColumn,
                           IsAscending: true
                       }
                   ]
               }
               && orderingTable == mainTable.Alias
               && IsJsonEachKeyColumn(selectExpression, orderingColumn);

        bool IsJsonEachKeyColumn(SelectExpression selectExpression, ColumnExpression orderingColumn)
            => selectExpression.Tables.FirstOrDefault(t => t.Alias == orderingColumn.TableAlias)?.UnwrapJoin() is TableExpressionBase table
               && (table is MySqlJsonTableExpression
                   || (table is SelectExpression subquery
                       && subquery.Projection.FirstOrDefault(p => p.Alias == "key")?.Expression is ColumnExpression projectedColumn
                       && IsJsonEachKeyColumn(subquery, projectedColumn)));
    }

    protected override bool IsValidSelectExpressionForExecuteDelete(SelectExpression selectExpression)
        => selectExpression is
           {
               Offset: null,
               GroupBy: [],
               Having: null
           } &&
           (selectExpression.Tables.Count == 1 || (selectExpression.Orderings.Count == 0 && selectExpression.Limit is null)) &&
           selectExpression.Tables[0] is TableExpression &&
           selectExpression.Tables.Skip(1).All(t => t is InnerJoinExpression);

    protected override bool IsValidSelectExpressionForExecuteUpdate(
        SelectExpression selectExpression,
        TableExpressionBase targetTable,
        [NotNullWhen(true)] out TableExpression tableExpression)
    {
        if (selectExpression is
            {
                Offset: null,
                IsDistinct: false,
                GroupBy: [],
                Having: null,
                Orderings: []
            })
        {
            TableExpressionBase table;
            if (selectExpression.Tables.Count == 1)
            {
                table = selectExpression.Tables[0];
            }
            else
            {
                table = targetTable;

                if (selectExpression.Tables.Count > 1 &&
                    table is JoinExpressionBase joinExpressionBase)
                {
                    table = joinExpressionBase.Table;
                }
            }

            if (table is TableExpression te)
            {
                tableExpression = te;
                return true;
            }
        }

        tableExpression = null;
        return false;
    }

    protected override ShapedQueryExpression TranslateAny(ShapedQueryExpression source, LambdaExpression predicate)
    {
        // Simplify x.Array.Any() => JSON_LENGTH(x.Array) > 0 instead of WHERE EXISTS (SELECT 1 FROM JSON_TABLE(x.Array))
        if (predicate is null
            && source.QueryExpression is SelectExpression
            {
                Tables: [TableValuedFunctionExpression { Name: "JSON_TABLE", Schema: null, IsBuiltIn: true, Arguments: [var array] }],
                GroupBy: [],
                Having: null,
                IsDistinct: false,
                Limit: null,
                Offset: null
            })
        {
            var translation =
                _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.NullableFunction(
                        "JSON_LENGTH",
                        new[] { array },
                        typeof(int)),
                    _sqlExpressionFactory.Constant(0));

            return source.UpdateQueryExpression(new SelectExpression(translation, _sqlAliasManager));
        }

        return base.TranslateAny(source, predicate);
    }

    protected override ShapedQueryExpression TranslateElementAtOrDefault(
        ShapedQueryExpression source,
        Expression index,
        bool returnDefault)
    {
        if (!returnDefault
            && source.QueryExpression is SelectExpression
            {
                Tables:
                [
                    MySqlJsonTableExpression
                    {
                        Name: "JSON_TABLE", Schema: null, IsBuiltIn: true, JsonExpression: var jsonArrayColumn
                    } jsonEachExpression
                ],
                GroupBy: [],
                Having: null,
                IsDistinct: false,
                Orderings: [{ Expression: ColumnExpression { Name: "key" } orderingColumn, IsAscending: true }],
                Limit: null,
                Offset: null
            } selectExpression
            && orderingColumn.TableAlias == jsonEachExpression.Alias
            && TranslateExpression(index) is { } translatedIndex)
        {
            // Index on JSON array

            // Extract the column projected out of the source, and simplify the subquery to a simple JsonScalarExpression
            var shaperExpression = source.ShaperExpression;
            if (shaperExpression is UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression
                && unaryExpression.Operand.Type.IsNullableType()
                && unaryExpression.Operand.Type.UnwrapNullableType() == unaryExpression.Type)
            {
                shaperExpression = unaryExpression.Operand;
            }

            if (shaperExpression is ProjectionBindingExpression projectionBindingExpression
                && selectExpression.GetProjection(projectionBindingExpression) is ColumnExpression projectionColumn)
            {
                SqlExpression translation = new JsonScalarExpression(
                    jsonArrayColumn,
                    new[] { new PathSegment(translatedIndex) },
                    projectionColumn.Type,
                    projectionColumn.TypeMapping,
                    projectionColumn.IsNullable);

                // If we have a type mapping (i.e. translating over a column rather than a parameter), apply any necessary server-side
                // conversions.
                if (projectionColumn.TypeMapping is not null)
                {
                    translation = ApplyJsonSqlConversion(
                        translation, _sqlExpressionFactory, projectionColumn.TypeMapping, projectionColumn.IsNullable);
                }

                return source.UpdateQueryExpression(new SelectExpression(translation, _sqlAliasManager));
            }
        }

        return base.TranslateElementAtOrDefault(source, index, returnDefault);
    }

    protected override ShapedQueryExpression TransformJsonQueryToTable(JsonQueryExpression jsonQueryExpression)
    {
        // Calculate the table alias for the JSON_TABLE function based on the last named path segment
        // (or the JSON column name if there are none)
        var lastNamedPathSegmentPropertyName = jsonQueryExpression.Path.LastOrDefault(ps => ps.PropertyName is not null).PropertyName;
        var tableAlias = _sqlAliasManager.GenerateTableAlias(
            lastNamedPathSegmentPropertyName ?? jsonQueryExpression.JsonColumn.Name);

        var jsonTypeMapping = jsonQueryExpression.JsonColumn.TypeMapping!;

        // We now add all of the projected entity's properties and navigations into the JSON_TABLE's COLUMNS clause
        var columnInfos = new List<MySqlJsonTableExpression.ColumnInfo>();

        // We're only interested in properties which actually exist in the JSON, filter out uninteresting shadow keys
        foreach (var property in jsonQueryExpression.StructuralType.GetPropertiesInHierarchy())
        {
            if (property.GetJsonPropertyName() is string jsonPropertyName)
            {
                columnInfos.Add(
                    new MySqlJsonTableExpression.ColumnInfo(
                        Name: jsonPropertyName,
                        TypeMapping: property.GetRelationalTypeMapping(),
                        // Path for JSON_TABLE COLUMNS: each row is already an array element, so access properties directly
                        Path: [new PathSegment(jsonPropertyName)],
                        AsJson: false));
            }
        }

        // Add navigations to owned entities mapped to JSON
        switch (jsonQueryExpression.StructuralType)
        {
            case IEntityType entityType:
                foreach (var navigation in entityType.GetNavigationsInHierarchy()
                    .Where(n => n.ForeignKey.IsOwnership
                        && n.TargetEntityType.IsMappedToJson()
                        && n.ForeignKey.PrincipalToDependent == n))
                {
                    var jsonNavigationName = navigation.TargetEntityType.GetJsonPropertyName();
                    Check.DebugAssert(jsonNavigationName is not null, $"No JSON property name for navigation {navigation.Name}");

                    columnInfos.Add(
                        new MySqlJsonTableExpression.ColumnInfo(
                            Name: jsonNavigationName,
                            TypeMapping: jsonTypeMapping,
                            Path: [new PathSegment(jsonNavigationName)],
                            AsJson: true));
                }
                break;

            case IComplexType complexType:
                foreach (var complexProperty in complexType.GetComplexProperties())
                {
                    var jsonPropertyName = complexProperty.ComplexType.GetJsonPropertyName();
                    Check.DebugAssert(jsonPropertyName is not null, $"No JSON property name for complex property {complexProperty.Name}");

                    columnInfos.Add(
                        new MySqlJsonTableExpression.ColumnInfo(
                            Name: jsonPropertyName,
                            TypeMapping: jsonTypeMapping,
                            Path: [new PathSegment(jsonPropertyName)],
                            AsJson: true));
                }
                break;

            default:
                throw new UnreachableException();
        }

        // json_to_recordset in Npgsql uses JsonScalarExpression which PostgreSQL handles properly.
        // However, MySQL's JsonScalarExpression translates to JSON_VALUE which extracts scalar values
        // and strips JSON structure. For JSON_TABLE, we need the raw JSON fragment.
        // Use JSON_EXTRACT explicitly to get the nested JSON array/object.
        
        SqlExpression jsonSource;
        if (jsonQueryExpression.Path.Count > 0)
        {
            // Build the JSON path for extraction
            var pathBuilder = new System.Text.StringBuilder("$");
            foreach (var segment in jsonQueryExpression.Path)
            {
                if (segment.PropertyName is not null)
                {
                    pathBuilder.Append('.').Append(segment.PropertyName);
                }
                else if (segment.ArrayIndex is SqlConstantExpression { Value: int index })
                {
                    pathBuilder.Append('[').Append(index).Append(']');
                }
            }

            // Use JSON_EXTRACT to get the nested JSON document with structure intact
            jsonSource = _sqlExpressionFactory.Function(
                "JSON_EXTRACT",
                [jsonQueryExpression.JsonColumn, _sqlExpressionFactory.Constant(pathBuilder.ToString())],
                nullable: true,
                argumentsPropagateNullability: [true, true],
                typeof(string),
                jsonTypeMapping);
        }
        else
        {
            // No path - use the JSON column directly
            jsonSource = jsonQueryExpression.JsonColumn;
        }

        // Construct the JSON_TABLE expression with column definitions
        var jsonTableExpression = new MySqlJsonTableExpression(
            tableAlias,
            jsonSource,
            // Path to iterate over array elements: $[*]
            [new PathSegment(_sqlExpressionFactory.Constant("*", RelationalTypeMapping.NullMapping))],
            [.. columnInfos]);

        // MySQL JSON_TABLE returns a 'key' column for array ordering (similar to PostgreSQL's ordinality)
        var keyColumnTypeMapping = _typeMappingSource.FindMapping(typeof(int))!;

#pragma warning disable EF1001 // Internal EF Core API usage.
        // Use CreateSelect helper method (from base class) to create the SelectExpression
        var selectExpression = CreateSelect(
            jsonQueryExpression,
            jsonTableExpression,
            "key",
            typeof(int),
            keyColumnTypeMapping);
#pragma warning restore EF1001 // Internal EF Core API usage.

        return new ShapedQueryExpression(
            selectExpression,
            new RelationalStructuralTypeShaperExpression(
                jsonQueryExpression.StructuralType,
                new ProjectionBindingExpression(
                    selectExpression,
                    new ProjectionMember(),
                    typeof(ValueBuffer)),
                false));
    }

    protected override ShapedQueryExpression TranslatePrimitiveCollection(SqlExpression sqlExpression, IProperty property, string tableAlias)
    {
        if (!_options.PrimitiveCollectionsSupport)
        {
            throw new InvalidOperationException(
                CoreStrings.TranslationFailedWithDetails(
                    sqlExpression.Print(),
                    "Primitive collections support has not been enabled."));
        }

        // if (!_options.ServerVersion.Supports.JsonTableImplementationUsesImplicitLateralJoin &&
        //     sqlExpression is ColumnExpression)
        // {
        //     // MariaDB will just return wrong results if the column of an outer table is being referenced.
        //     return null;
        // }

        // Generate the JSON_TABLE() function expression, and wrap it in a SelectExpression.

        // Note that where the elementTypeMapping is known (i.e. collection columns), we immediately generate JSON_TABLE() with a COLUMNS clause
        // (i.e. with a columnInfo), which determines the type conversion to apply to the JSON elements coming out.
        // For parameter collections, the element type mapping will only be inferred and applied later (see
        // MySqlInferredTypeMappingApplier below), at which point the we'll apply it to add the COLUMNS clause.
        var elementTypeMapping = (RelationalTypeMapping)sqlExpression.TypeMapping?.ElementTypeMapping;

        var jsonTableExpression = new MySqlJsonTableExpression(
            tableAlias,
            sqlExpression,
            new[] { new PathSegment(_sqlExpressionFactory.Constant("*", RelationalTypeMapping.NullMapping)) },
            elementTypeMapping is not null
                ? new[]
                {
                    new MySqlJsonTableExpression.ColumnInfo
                    {
                        Name = "value",
                        TypeMapping = elementTypeMapping,
                        Path = new[] { new PathSegment(_sqlExpressionFactory.Constant(0, _typeMappingSource.FindMapping(typeof(int)))) },
                    }
                }
                : null);


        // Using primitive collections in parameters that are used as the JSON source argument for JSON_TABLE(source, ...) can crash
        // MySQL 8 somewhere later down the line. We mitigate this by inlining those parameters.
        // There are however other scenarios that can still crash MySQL 8 (e.g. `NorthwindSelectQueryMySqlTest.Correlated_collection_after_distinct_not_containing_original_identifier`).
        // For those cases, we implement a flag to skip skip the JSON_TABLE generation.
        if (elementTypeMapping is null &&
            !_options.ServerVersion.Supports.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash &&
            _mySql8EngineCrashWhenUsingJsonTableWithPrimitiveCollectionInParametersSkip)
        {
            AddTranslationErrorDetails($"JSON_TABLE() has been disabled by the '{Issue1792SkipWithParameterFlagName}' AppContext switch, because it can crash MySQL 8.");
            return null;
        }

        var elementClrType = sqlExpression.Type.GetSequenceType();

        // If this is a collection property, get the element's nullability out of metadata. Otherwise, this is a parameter property, in
        // which case we only have the CLR type (note that we cannot produce different SQLs based on the nullability of an *element* in
        // a parameter collection - our caching mechanism only supports varying by the nullability of the parameter itself (i.e. the
        // collection).
        var isElementNullable = property?.GetElementType()!.IsNullable;

        var keyColumnTypeMapping = _typeMappingSource.FindMapping(typeof(int))!;

#pragma warning disable EF1001 // Internal EF Core API usage.
        var selectExpression = new SelectExpression(
            [jsonTableExpression],
            new ColumnExpression(
                "value",
                tableAlias,
                elementClrType.UnwrapNullableType(),
                elementTypeMapping,
                isElementNullable ?? elementClrType.IsNullableType()),
            identifier: [(new ColumnExpression("key", tableAlias, typeof(int), keyColumnTypeMapping, nullable: false), keyColumnTypeMapping.Comparer)],
            _sqlAliasManager);
#pragma warning restore EF1001 // Internal EF Core API usage.

        // JSON_TABLE() doesn't guarantee the ordering of the elements coming out; when using JSON_TABLE() without COLUMNS, a [key] column is returned
        // with the JSON array's ordering, which we can ORDER BY; this option doesn't exist with JSON_TABLE() with COLUMNS, unfortunately.
        // However, JSON_TABLE() with COLUMNS has better performance, and also applies JSON-specific conversions we cannot be done otherwise
        // (e.g. JSON_TABLE() with COLUMNS does base64 decoding for VARBINARY).
        // Here we generate JSON_TABLE() with COLUMNS, but also add an ordering by [key] - this is a temporary invalid representation.
        // In MySqlQueryTranslationPostprocessor, we'll post-process the expression; if the ORDER BY was stripped (e.g. because of
        // IN, EXISTS or a set operation), we'll just leave the JSON_TABLE() with COLUMNS. If not, we'll convert the JSON_TABLE() with COLUMNS to an
        // JSON_TABLE() without COLUMNS.
        // Note that the JSON_TABLE() 'key' column is an nvarchar - we convert it to an int before sorting.
        selectExpression.AppendOrdering(
            new OrderingExpression(
                selectExpression.CreateColumnExpression(
                    jsonTableExpression,
                    "key",
                    typeof(uint),
                    typeMapping: _typeMappingSource.FindMapping(typeof(uint)),
                    columnNullable: false),
                ascending: true));

        var shaperExpression = (Expression)new ProjectionBindingExpression(selectExpression, new ProjectionMember(), elementClrType.MakeNullable());
        if (shaperExpression.Type != elementClrType)
        {
            Check.DebugAssert(
                elementClrType.MakeNullable() == shaperExpression.Type,
                "expression.Type must be nullable of targetType");

            shaperExpression = Expression.Convert(shaperExpression, elementClrType);
        }

        return new ShapedQueryExpression(selectExpression, shaperExpression);
    }

    /// <summary>
    /// Wraps the given expression with any SQL logic necessary to convert a value coming out of a JSON document into the relational value
    /// represented by the given type mapping.
    /// </summary>
    private static SqlExpression ApplyJsonSqlConversion(
        SqlExpression expression,
        ISqlExpressionFactory sqlExpressionFactory,
        RelationalTypeMapping typeMapping,
        bool isNullable)
        => typeMapping switch
        {
            // TODO: EF Core 8 - Decide between UNHEX() and FROM_BASE64().
            ByteArrayTypeMapping => sqlExpressionFactory.Function("FROM_BASE64", new[] { expression }, isNullable, new[] { true }, typeof(byte[]), typeMapping),
            _ => expression
        };

    private sealed class FakeMemberInfo : MemberInfo
    {
        public FakeMemberInfo(string name)
            => Name = name;

        public override string Name { get; }

        public override object[] GetCustomAttributes(bool inherit)
            => throw new NotSupportedException();
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            => throw new NotSupportedException();
        public override bool IsDefined(Type attributeType, bool inherit)
            => throw new NotSupportedException();
        public override Type DeclaringType => throw new NotSupportedException();
        public override MemberTypes MemberType => throw new NotSupportedException();
        public override Type ReflectedType => throw new NotSupportedException();
    }
}
