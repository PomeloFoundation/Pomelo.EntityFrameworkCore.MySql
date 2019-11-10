// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
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

        public override bool IsHandledByConvention(IModel model, IAnnotation annotation)
        {
            return true;
        }

        public override MethodCallCodeFragment GenerateFluentApi(IModel model, IAnnotation annotation)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(annotation, nameof(annotation));

            switch (annotation.Name)
            {
                case MySqlAnnotationNames.ServerVersion when annotation.Value is string serverVersion && serverVersion.Length > 0:
                    return new MethodCallCodeFragment("ForServerVersion", serverVersion);

                default:
                    return null;
            }
        }

        public override MethodCallCodeFragment GenerateFluentApi(IProperty property, IAnnotation annotation)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(annotation, nameof(annotation));

            switch (annotation.Name)
            {
                case MySqlAnnotationNames.CharSet when annotation.Value is string charSet && charSet.Length > 0:
                    return new MethodCallCodeFragment("HasCharSet", charSet);

                case MySqlAnnotationNames.Collation when annotation.Value is string collation && collation.Length > 0:
                    return new MethodCallCodeFragment("HasCollation", collation);

                default:
                    return null;
            }
        }
    }
}
