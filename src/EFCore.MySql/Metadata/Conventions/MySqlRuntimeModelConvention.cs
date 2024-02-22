// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions;

/// <summary>
///     A convention that creates an optimized copy of the mutable model.
///     The runtime model is only used at app runtime and not for design-time purposes.
///     Therefore, all annotations that are related to design-time concerns (i.e. databases, tables or columns) are superfluous and should
///     be removed.
/// </summary>
public class MySqlRuntimeModelConvention : RelationalRuntimeModelConvention
{
    /// <summary>
    ///     Creates a new instance of <see cref="MySqlRuntimeModelConvention" />.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
    /// <param name="relationalDependencies">Parameter object containing relational dependencies for this convention.</param>
    public MySqlRuntimeModelConvention(
        ProviderConventionSetBuilderDependencies dependencies,
        RelationalConventionSetBuilderDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <inheritdoc />
    protected override void ProcessModelAnnotations(
        Dictionary<string, object> annotations,
        IModel model,
        RuntimeModel runtimeModel,
        bool runtime)
    {
        base.ProcessModelAnnotations(annotations, model, runtimeModel, runtime);

        if (!runtime)
        {
            annotations.Remove(MySqlAnnotationNames.CharSet);
            annotations.Remove(MySqlAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.CollationDelegation);
            annotations.Remove(MySqlAnnotationNames.GuidCollation);
        }
    }

    /// <inheritdoc />
    protected override void ProcessEntityTypeAnnotations(
        Dictionary<string, object> annotations,
        IEntityType entityType,
        RuntimeEntityType runtimeEntityType,
        bool runtime)
    {
        base.ProcessEntityTypeAnnotations(annotations, entityType, runtimeEntityType, runtime);

        if (!runtime)
        {
            annotations.Remove(MySqlAnnotationNames.CharSet);
            annotations.Remove(MySqlAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.CollationDelegation);
            annotations.Remove(MySqlAnnotationNames.StoreOptions);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }
    }

    /// <inheritdoc />
    protected override void ProcessPropertyAnnotations(
        Dictionary<string, object> annotations,
        IProperty property,
        RuntimeProperty runtimeProperty,
        bool runtime)
    {
        base.ProcessPropertyAnnotations(annotations, property, runtimeProperty, runtime);

        if (!runtime)
        {
            annotations.Remove(MySqlAnnotationNames.CharSet);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(MySqlAnnotationNames.SpatialReferenceSystemId);

            if (!annotations.ContainsKey(MySqlAnnotationNames.ValueGenerationStrategy))
            {
                annotations[MySqlAnnotationNames.ValueGenerationStrategy] = property.GetValueGenerationStrategy();
            }
        }
    }

    /// <inheritdoc />
    protected override void ProcessIndexAnnotations(
        Dictionary<string, object> annotations,
        IIndex index,
        RuntimeIndex runtimeIndex,
        bool runtime)
    {
        base.ProcessIndexAnnotations(annotations, index, runtimeIndex, runtime);

        if (!runtime)
        {
            annotations.Remove(MySqlAnnotationNames.FullTextIndex);
            annotations.Remove(MySqlAnnotationNames.FullTextParser);
            annotations.Remove(MySqlAnnotationNames.IndexPrefixLength);
            annotations.Remove(MySqlAnnotationNames.SpatialIndex);
        }
    }

    /// <inheritdoc />
    protected override void ProcessKeyAnnotations(
        Dictionary<string, object> annotations,
        IKey key,
        RuntimeKey runtimeKey,
        bool runtime)
    {
        base.ProcessKeyAnnotations(annotations, key, runtimeKey, runtime);

        if (!runtime)
        {
            annotations.Remove(MySqlAnnotationNames.IndexPrefixLength);
        }
    }
}
