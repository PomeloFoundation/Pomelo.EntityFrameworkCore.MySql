﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlKeyBuilderExtensions
    {
        /// <summary>
        /// Sets prefix lengths for the key.
        /// </summary>
        /// <param name="keyBuilder"> The key builder. </param>
        /// <param name="prefixLengths">The prefix lengths to set, in the order the key columns where specified.
        /// A value of `0` indicates, that the full length should be used for that column. </param>
        /// <returns> The key builder. </returns>
        public static KeyBuilder HasPrefixLength([NotNull] this KeyBuilder keyBuilder, params int[] prefixLengths)
        {
            Check.NotNull(keyBuilder, nameof(keyBuilder));

            keyBuilder.Metadata.SetPrefixLength(prefixLengths);

            return keyBuilder;
        }
    }
}
