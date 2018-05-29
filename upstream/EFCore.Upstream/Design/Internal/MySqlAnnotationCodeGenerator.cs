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
            if (annotation.Name == MySqlAnnotationNames.ValueGenerationStrategy)
            {
                return (MySqlValueGenerationStrategy)annotation.Value == MySqlValueGenerationStrategy.IdentityColumn;
            }

            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateFluentApi(IKey key, IAnnotation annotation)
        {
            if (annotation.Name == MySqlAnnotationNames.Clustered)
            {
                return (bool)annotation.Value == false
                    ? new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered), false)
                    : new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered));
            }

            return null;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateFluentApi(IIndex index, IAnnotation annotation)
        {
            if (annotation.Name == MySqlAnnotationNames.Clustered)
            {
                return (bool)annotation.Value == false
                    ? new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered), false)
                    : new MethodCallCodeFragment(nameof(MySqlIndexBuilderExtensions.ForMySqlIsClustered));
            }

            return null;
        }
    }
}
