// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Design.Internal
{
    public class MySqlAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        public MySqlAnnotationCodeGenerator([NotNull] AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IModel model, IAnnotation annotation)
        {
            if (annotation.Name == MySqlAnnotationNames.CharSet)
            {
                var explicitlyDelegateToChildren = model[MySqlAnnotationNames.CharSetDelegation] as bool?;
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.HasCharSet),
                    new[] {annotation.Value}
                        .AppendIfTrue(explicitlyDelegateToChildren.HasValue, explicitlyDelegateToChildren));
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
                var explicitlyDelegateToChildren = model[MySqlAnnotationNames.CollationDelegation] as bool?;
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.UseCollation),
                    new[] {annotation.Value}
                        .AppendIfTrue(explicitlyDelegateToChildren.HasValue, explicitlyDelegateToChildren));
            }

            if (annotation.Name == MySqlAnnotationNames.CollationDelegation &&
                model[RelationalAnnotationNames.Collation] is null)
            {
                return new MethodCallCodeFragment(
                    nameof(MySqlModelBuilderExtensions.UseCollation),
                    null,
                    annotation.Value);
            }

            return null;
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IEntityType entityType, IAnnotation annotation)
        {
            if (annotation.Name == MySqlAnnotationNames.CharSet)
            {
                var explicitlyDelegateToChildren = entityType[MySqlAnnotationNames.CharSetDelegation] as bool?;
                return new MethodCallCodeFragment(
                    nameof(MySqlEntityTypeBuilderExtensions.HasCharSet),
                    new[] {annotation.Value}
                        .AppendIfTrue(explicitlyDelegateToChildren.HasValue, explicitlyDelegateToChildren));
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
                var explicitlyDelegateToChildren = entityType[MySqlAnnotationNames.CollationDelegation] as bool?;
                return new MethodCallCodeFragment(
                    nameof(MySqlEntityTypeBuilderExtensions.UseCollation),
                    new[] {annotation.Value}
                        .AppendIfTrue(explicitlyDelegateToChildren.HasValue, explicitlyDelegateToChildren));
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
                case MySqlAnnotationNames.CharSet when annotation.Value is string charSet && charSet.Length > 0:
                    return new MethodCallCodeFragment(nameof(MySqlPropertyBuilderExtensions.HasCharSet), charSet);

                default:
                    return null;
            }
        }
    }
}
