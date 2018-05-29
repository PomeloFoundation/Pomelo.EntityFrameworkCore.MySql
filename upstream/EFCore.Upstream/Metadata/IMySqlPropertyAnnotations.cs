// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     API for MySql-specific annotations accessed through
    ///     <see cref="MySqlMetadataExtensions.MySql(IProperty)" />.
    /// </summary>
    public interface IMySqlPropertyAnnotations : IRelationalPropertyAnnotations
    {
        /// <summary>
        ///     <para>
        ///         Gets the <see cref="MySqlValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />
        ///     </para>
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

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use with
        ///     <see cref="MySqlPropertyBuilderExtensions.ForMySqlUseSequenceHiLo" />
        /// </summary>
        /// <returns> The sequence to use, or <c>null</c> if no sequence exists in the model. </returns>
        ISequence FindHiLoSequence();
    }
}
