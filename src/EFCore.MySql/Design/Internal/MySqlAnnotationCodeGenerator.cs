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

        public override MethodCallCodeFragment GenerateFluentApi(IProperty property, IAnnotation annotation)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(annotation, nameof(annotation));

            if (annotation.Name == MySqlAnnotationNames.CharSet &&
                annotation.Value is string charSetName &&
                charSetName.Length > 0)
            {
                return new MethodCallCodeFragment("HasCharSet", charSetName);
            }

            return null;
        }
    }
}
