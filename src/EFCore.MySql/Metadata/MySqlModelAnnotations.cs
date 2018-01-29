// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

//ReSharper disable once CheckNamespace
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
            get => (MySqlValueGenerationStrategy?)Annotations.Metadata[MySqlAnnotationNames.ValueGenerationStrategy];

            set => SetValueGenerationStrategy(value);
        }

        protected virtual bool SetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
            => Annotations.SetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);
    }
}
