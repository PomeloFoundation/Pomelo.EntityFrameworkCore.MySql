// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Design.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlAnnotationCodeGenerator([NotNull] AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override bool IsHandledByConvention(IModel model, IAnnotation annotation)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(annotation, nameof(annotation));

            if (annotation.Name == RelationalAnnotationNames.DefaultSchema)
            {
                return string.Equals("dbo", (string)annotation.Value);
            }

            return annotation.Name == MySqlAnnotationNames.ValueGenerationStrategy
                ? (MySqlValueGenerationStrategy)annotation.Value == MySqlValueGenerationStrategy.IdentityColumn
                : false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateFluentApi(IKey key, IAnnotation annotation)
        {
            return annotation.Name == MySqlAnnotationNames.Clustered
                ? (bool)annotation.Value == false
                    ? new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered), false)
                    : new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered))
                : null;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateFluentApi(IIndex index, IAnnotation annotation)
        {
            return annotation.Name == MySqlAnnotationNames.Clustered
                ? (bool)annotation.Value == false
                    ? new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered), false)
                    : new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered))
                : null;
        }
    }
}
