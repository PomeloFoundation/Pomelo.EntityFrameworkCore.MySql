// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class MySqlModelAnnotations : RelationalModelAnnotations, IMySqlModelAnnotations
    {
        public MySqlModelAnnotations([NotNull] IModel model)
            : base(model)
        {
        }

        protected MySqlModelAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        public virtual MySqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get => (MySqlValueGenerationStrategy?)Annotations.GetAnnotation(
                MySqlAnnotationNames.ValueGenerationStrategy);

            set => SetValueGenerationStrategy(value);
        }

        protected virtual bool SetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
            => Annotations.SetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);
    }
}
