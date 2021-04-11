// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Diagnostics;
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

        protected override bool IsHandledByConvention(IModel model, IAnnotation annotation)
        {
            return true;
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
                    return new MethodCallCodeFragment("HasCharSet", charSet);

                default:
                    return null;
            }
        }
    }
}
