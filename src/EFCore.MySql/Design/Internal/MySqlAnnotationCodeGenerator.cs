// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Design.Internal
{
    public class MySqlAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        // NOTE: Using method name strings instead of MethodInfo for MySql extension methods.
        // This ensures EF Core's CSharpSnapshotGenerator outputs fluent chaining instead of
        // static method calls (e.g., .HasCharSet("latin1") instead of
        // MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Name"), "latin1")).
        // See: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1990

        private static readonly MethodInfo _modelHasAnnotationMethodInfo
            = typeof(ModelBuilder).GetRequiredRuntimeMethod(
                nameof(ModelBuilder.HasAnnotation),
                typeof(string),
                typeof(object));

        public MySqlAnnotationCodeGenerator(AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IEnumerable<IAnnotation> FilterIgnoredAnnotations(IEnumerable<IAnnotation> annotations)
        {
            annotations = base.FilterIgnoredAnnotations(annotations).ToArray();

            var hasCharSetAnnotation = annotations.Any(a => a.Name == MySqlAnnotationNames.CharSet);
            var hasCollationAnnotation = annotations.Any(a => a.Name == RelationalAnnotationNames.Collation);

            foreach (var annotation in annotations)
            {
                // Charsets and their delegation and collations and their delegation are handled in the same Fluent API call.
                // Since the GenerateFluentApi methods cannot skip annotations, we have to ignore one of them here early, if both have been
                // set, so we don't output a HasCharSet()/UseCollation() call and a CharSetDelegation/CollationDelegation annotation in
                // addition to that.
                if (annotation.Name == MySqlAnnotationNames.CharSetDelegation && hasCharSetAnnotation ||
                    annotation.Name == MySqlAnnotationNames.CollationDelegation && hasCollationAnnotation)
                {
                    continue;
                }

                yield return annotation;
            }
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IModel model, IAnnotation annotation)
        {
            if (annotation.Name == MySqlAnnotationNames.CharSet)
            {
                var delegationModes = model[MySqlAnnotationNames.CharSetDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.HasCharSet),
                    new[] { annotation.Value }
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == MySqlAnnotationNames.CharSetDelegation &&
                model[MySqlAnnotationNames.CharSet] is null)
            {
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.HasCharSet),
                    null,
                    annotation.Value);
            }

            // EF Core currently just falls back on using the `Relational:Collation` annotation instead of generating the `UseCollation()`
            // method call (though it could), so we can return our method call fragment here, without generating an ugly duplicate.
            if (annotation.Name == RelationalAnnotationNames.Collation)
            {
                var delegationModes = model[MySqlAnnotationNames.CollationDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.UseCollation),
                    new[] { annotation.Value }
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == MySqlAnnotationNames.CollationDelegation &&
                model[RelationalAnnotationNames.Collation] is null)
            {
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.UseCollation),
                    null,
                    annotation.Value);
            }

            if (annotation.Name == MySqlAnnotationNames.GuidCollation)
            {
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.UseGuidCollation),
                    annotation.Value);
            }

            return null;
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IEntityType entityType, IAnnotation annotation)
        {
            if (annotation.Name == MySqlAnnotationNames.CharSet)
            {
                var delegationModes = entityType[MySqlAnnotationNames.CharSetDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    nameof(MySqlEntityTypeBuilderExtensions.HasCharSet),
                    new[] { annotation.Value }
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == MySqlAnnotationNames.CharSetDelegation &&
                entityType[MySqlAnnotationNames.CharSet] is null)
            {
                return new MethodCallCodeFragment(
                    nameof(MySqlEntityTypeBuilderExtensions.HasCharSet),
                    null,
                    annotation.Value);
            }

            if (annotation.Name == RelationalAnnotationNames.Collation)
            {
                var delegationModes = entityType[MySqlAnnotationNames.CollationDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    nameof(MySqlEntityTypeBuilderExtensions.UseCollation),
                    new[] { annotation.Value }
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == MySqlAnnotationNames.CollationDelegation &&
                entityType[RelationalAnnotationNames.Collation] is null)
            {
                return new MethodCallCodeFragment(
                    nameof(MySqlEntityTypeBuilderExtensions.UseCollation),
                    null,
                    annotation.Value);
            }

            return null;
        }

        protected override AttributeCodeFragment GenerateDataAnnotation(IEntityType entityType, IAnnotation annotation)
        {
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(annotation, nameof(annotation));

            if (annotation.Name == MySqlAnnotationNames.CharSet)
            {
                var delegationModes = entityType[MySqlAnnotationNames.CharSetDelegation] as DelegationModes?;
                return new AttributeCodeFragment(
                    typeof(MySqlCharSetAttribute),
                    new[] { annotation.Value }
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == MySqlAnnotationNames.CharSetDelegation &&
                entityType[MySqlAnnotationNames.CharSet] is null)
            {
                return new AttributeCodeFragment(
                    typeof(MySqlCharSetAttribute),
                    null,
                    annotation.Value);
            }

            if (annotation.Name == RelationalAnnotationNames.Collation)
            {
                var delegationModes = entityType[MySqlAnnotationNames.CollationDelegation] as DelegationModes?;
                return new AttributeCodeFragment(
                    typeof(MySqlCollationAttribute),
                    new[] { annotation.Value }
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == MySqlAnnotationNames.CollationDelegation &&
                entityType[RelationalAnnotationNames.Collation] is null)
            {
                return new AttributeCodeFragment(
                    typeof(MySqlCollationAttribute),
                    null,
                    annotation.Value);
            }

            return base.GenerateDataAnnotation(entityType, annotation);
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IProperty property, IAnnotation annotation)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(annotation, nameof(annotation));

            // At this point, all legacy `MySql:Collation` annotations should have been replaced by `Relational:Collation` ones.
#pragma warning disable 618
            Debug.Assert(annotation.Name != MySqlAnnotationNames.Collation);
#pragma warning restore 618

            switch (annotation.Name)
            {
                case MySqlAnnotationNames.CharSet when annotation.Value is string { Length: > 0 } charSet:
                    return new MethodCallCodeFragment(
                        nameof(MySqlPropertyBuilderExtensions.HasCharSet),
                        charSet);

                default:
                    return null;
            }
        }

        protected override AttributeCodeFragment GenerateDataAnnotation(IProperty property, IAnnotation annotation)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(annotation, nameof(annotation));

            return annotation.Name switch
            {
                MySqlAnnotationNames.CharSet when annotation.Value is string { Length: > 0 } charSet => new AttributeCodeFragment(
                    typeof(MySqlCharSetAttribute), charSet),
                RelationalAnnotationNames.Collation when annotation.Value is string { Length: > 0 } collation => new AttributeCodeFragment(
                    typeof(MySqlCollationAttribute), collation),
                _ => base.GenerateDataAnnotation(property, annotation)
            };
        }

        public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
            IModel model,
            IDictionary<string, IAnnotation> annotations)
        {
            var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(model, annotations));

            if (GenerateValueGenerationStrategy(annotations, onModel: true) is { } valueGenerationStrategy)
            {
                fragments.Add(valueGenerationStrategy);
            }

            return fragments;
        }

        public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
            IProperty property,
            IDictionary<string, IAnnotation> annotations)
        {
            var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(property, annotations));

            if (GenerateValueGenerationStrategy(annotations, onModel: false) is { } valueGenerationStrategy)
            {
                fragments.Add(valueGenerationStrategy);
            }

            return fragments;
        }

        private MethodCallCodeFragment GenerateValueGenerationStrategy(IDictionary<string, IAnnotation> annotations, bool onModel)
            => TryGetAndRemove(annotations, MySqlAnnotationNames.ValueGenerationStrategy, out MySqlValueGenerationStrategy strategy)
                ? strategy switch
                {
                    MySqlValueGenerationStrategy.IdentityColumn => new MethodCallCodeFragment(
                        onModel
                            ? nameof(MySqlModelBuilderExtensions.AutoIncrementColumns)
                            : nameof(MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn)),
                    MySqlValueGenerationStrategy.ComputedColumn => new MethodCallCodeFragment(
                        nameof(MySqlPropertyBuilderExtensions.UseMySqlComputedColumn)),
                    MySqlValueGenerationStrategy.None => new MethodCallCodeFragment(
                        _modelHasAnnotationMethodInfo,
                        MySqlAnnotationNames.ValueGenerationStrategy,
                        MySqlValueGenerationStrategy.None),
                    _ => throw new ArgumentOutOfRangeException(strategy.ToString())
                }
                : null;

        private static bool TryGetAndRemove<T>(
            IDictionary<string, IAnnotation> annotations,
            string annotationName,
            [NotNullWhen(true)] out T annotationValue)
        {
            if (annotations.TryGetValue(annotationName, out var annotation)
                && annotation.Value is not null)
            {
                annotations.Remove(annotationName);
                annotationValue = (T)annotation.Value;
                return true;
            }

            annotationValue = default;
            return false;
        }
    }
}
