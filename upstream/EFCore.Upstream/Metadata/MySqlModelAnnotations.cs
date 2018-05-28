// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     Properties for relational-specific annotations accessed through
    ///     <see cref="MySqlMetadataExtensions.MySql(IMutableModel)" />.
    /// </summary>
    public class MySqlModelAnnotations : RelationalModelAnnotations, IMySqlModelAnnotations
    {
        /// <summary>
        ///     The default name for the sequence used
        ///     with <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        public const string DefaultHiLoSequenceName = "EntityFrameworkHiLoSequence";

        /// <summary>
        ///     Constructs an instance for annotations of the given <see cref="IModel" />.
        /// </summary>
        /// <param name="model"> The <see cref="IModel" /> to use. </param>
        public MySqlModelAnnotations([NotNull] IModel model)
            : base(model)
        {
        }

        /// <summary>
        ///     Constructs an instance for annotations of the <see cref="IModel" />
        ///     represented by the given annotation helper.
        /// </summary>
        /// <param name="annotations">
        ///     The <see cref="RelationalAnnotations" /> helper representing the <see cref="IModel" /> to annotate.
        /// </param>
        protected MySqlModelAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        /// <summary>
        ///     Gets or sets the sequence name to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        public virtual string HiLoSequenceName
        {
            get => (string)Annotations.Metadata[MySqlAnnotationNames.HiLoSequenceName];
            [param: CanBeNull] set => SetHiLoSequenceName(value);
        }

        /// <summary>
        ///     Attempts to set the sequence name to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <returns> <c>True</c> if the annotation was set; <c>false</c> otherwise. </returns>
        protected virtual bool SetHiLoSequenceName([CanBeNull] string value)
            => Annotations.SetAnnotation(
                MySqlAnnotationNames.HiLoSequenceName,
                Check.NullButNotEmpty(value, nameof(value)));

        /// <summary>
        ///     Gets or sets the schema for the sequence to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        public virtual string HiLoSequenceSchema
        {
            get => (string)Annotations.Metadata[MySqlAnnotationNames.HiLoSequenceSchema];
            [param: CanBeNull] set => SetHiLoSequenceSchema(value);
        }

        /// <summary>
        ///     Attempts to set the schema for the sequence to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <returns> <c>True</c> if the annotation was set; <c>false</c> otherwise. </returns>
        protected virtual bool SetHiLoSequenceSchema([CanBeNull] string value)
            => Annotations.SetAnnotation(
                MySqlAnnotationNames.HiLoSequenceSchema,
                Check.NullButNotEmpty(value, nameof(value)));

        /// <summary>
        ///     The <see cref="MySqlValueGenerationStrategy" /> to use for properties
        ///     of keys in the model, unless the property has a different strategy explicitly set.
        /// </summary>
        public virtual MySqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get => (MySqlValueGenerationStrategy?)Annotations.Metadata[MySqlAnnotationNames.ValueGenerationStrategy];

            set => SetValueGenerationStrategy(value);
        }

        /// <summary>
        ///     Attempts to set the <see cref="MySqlValueGenerationStrategy" /> to use for properties
        ///     of keys in the model.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <returns> <c>True</c> if the annotation was set; <c>false</c> otherwise. </returns>
        protected virtual bool SetValueGenerationStrategy(MySqlValueGenerationStrategy? value)
            => Annotations.SetAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);
    }
}
