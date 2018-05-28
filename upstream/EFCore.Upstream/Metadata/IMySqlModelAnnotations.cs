// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     API for MySql-specific annotations accessed through
    ///     <see cref="MySqlMetadataExtensions.MySql(IModel)" />.
    /// </summary>
    public interface IMySqlModelAnnotations : IRelationalModelAnnotations
    {
        /// <summary>
        /// The <see cref="MySqlValueGenerationStrategy"/> to use for properties
        /// of keys in the model, unless the property has a different strategy explicitly set.
        /// </summary>
        MySqlValueGenerationStrategy? ValueGenerationStrategy { get; }

        /// <summary>
        ///     Gets the sequence name to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        string HiLoSequenceName { get; }

        /// <summary>
        ///     Gets the schema for the sequence to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        string HiLoSequenceSchema { get; }
    }
}
