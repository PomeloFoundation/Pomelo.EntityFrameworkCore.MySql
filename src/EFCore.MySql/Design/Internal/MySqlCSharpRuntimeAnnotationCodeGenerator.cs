// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Design.Internal;

// Used to generate a compiled model. The compiled model is only used at app runtime and not for design-time purposes.
// Therefore, all annotations that are related to design-time concerns (i.e. databases, tables or columns) are superfluous and should be
// removed.
// TOOD: Check behavior for `ValueGenerationStrategy`, `LegacyValueGeneratedOnAdd` and `LegacyValueGeneratedOnAddOrUpdate`.
public class MySqlCSharpRuntimeAnnotationCodeGenerator : RelationalCSharpRuntimeAnnotationCodeGenerator
{
    public MySqlCSharpRuntimeAnnotationCodeGenerator(
        CSharpRuntimeAnnotationCodeGeneratorDependencies dependencies,
        RelationalCSharpRuntimeAnnotationCodeGeneratorDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    public override bool Create(
        CoreTypeMapping typeMapping,
        CSharpRuntimeAnnotationCodeGeneratorParameters parameters,
        ValueComparer valueComparer = null,
        ValueComparer keyValueComparer = null,
        ValueComparer providerValueComparer = null)
    {
        var result = base.Create(typeMapping, parameters, valueComparer, keyValueComparer, providerValueComparer);

        if (typeMapping is IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator extension)
        {
            extension.Create(parameters, Dependencies);
        }

        return result;
    }

    public override void Generate(IModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.CharSet);
            annotations.Remove(MySqlAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.CollationDelegation);
            annotations.Remove(MySqlAnnotationNames.GuidCollation);
        }

        base.Generate(model, parameters);
    }

    public override void Generate(IRelationalModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.CharSet);
            annotations.Remove(MySqlAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.CollationDelegation);
            annotations.Remove(MySqlAnnotationNames.GuidCollation);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(model, parameters);
    }

    public override void Generate(IEntityType entityType, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.CharSet);
            annotations.Remove(MySqlAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.CollationDelegation);
            annotations.Remove(MySqlAnnotationNames.StoreOptions);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(entityType, parameters);
    }

    public override void Generate(ITable table, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.CharSet);
            annotations.Remove(MySqlAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.CollationDelegation);
            annotations.Remove(MySqlAnnotationNames.StoreOptions);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(table, parameters);
    }

    public override void Generate(IProperty property, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.CharSet);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.SpatialReferenceSystemId);

            annotations.Remove(RelationalAnnotationNames.Collation);

            if (!annotations.ContainsKey(MySqlAnnotationNames.ValueGenerationStrategy))
            {
                annotations[MySqlAnnotationNames.ValueGenerationStrategy] = property.GetValueGenerationStrategy();
            }
        }

        base.Generate(property, parameters);
    }

    public override void Generate(IColumn column, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.CharSet);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.SpatialReferenceSystemId);
            annotations.Remove(MySqlAnnotationNames.ValueGenerationStrategy);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(column, parameters);
    }

    public override void Generate(IIndex index, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.FullTextIndex);
            annotations.Remove(MySqlAnnotationNames.FullTextParser);
            annotations.Remove(MySqlAnnotationNames.IndexPrefixLength);
            annotations.Remove(MySqlAnnotationNames.SpatialIndex);
        }

        base.Generate(index, parameters);
    }

    public override void Generate(ITableIndex index, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.FullTextIndex);
            annotations.Remove(MySqlAnnotationNames.FullTextParser);
            annotations.Remove(MySqlAnnotationNames.IndexPrefixLength);
            annotations.Remove(MySqlAnnotationNames.SpatialIndex);
        }

        base.Generate(index, parameters);
    }

    public override void Generate(IKey key, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(MySqlAnnotationNames.IndexPrefixLength);
        }

        base.Generate(key, parameters);
    }
}
