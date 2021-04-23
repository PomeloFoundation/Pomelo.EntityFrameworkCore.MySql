// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Extension methods for <see cref="IKey" /> for MySQL-specific metadata.
    /// </summary>
    public static class MySqlKeyExtensions
    {
        /// <summary>
        ///     Returns prefix lengths for the key.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns> The prefix lengths.
        /// A value of `0` indicates, that the full length should be used for that column. </returns>
        public static int[] PrefixLength([NotNull] this IKey key)
            => (int[])key[MySqlAnnotationNames.IndexPrefixLength];

        /// <summary>
        ///     Sets prefix lengths for the key.
        /// </summary>
        /// <param name="values"> The prefix lengths to set.
        /// A value of `0` indicates, that the full length should be used for that column. </param>
        /// <param name="key"> The key. </param>
        public static void SetPrefixLength([NotNull] this IMutableKey key, int[] values)
            => key.SetOrRemoveAnnotation(
                MySqlAnnotationNames.IndexPrefixLength,
                values);

        /// <summary>
        ///     Sets prefix lengths for the key.
        /// </summary>
        /// <param name="values"> The prefix lengths to set.
        /// A value of `0` indicates, that the full length should be used for that column. </param>
        /// <param name="key"> The key. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetPrefixLength(
            [NotNull] this IConventionKey key, int[] values, bool fromDataAnnotation = false)
            => key.SetOrRemoveAnnotation(
                MySqlAnnotationNames.IndexPrefixLength,
                values,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for prefix lengths of the key.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for prefix lengths of the key. </returns>
        public static ConfigurationSource? GetPrefixLengthConfigurationSource([NotNull] this IConventionKey property)
            => property.FindAnnotation(MySqlAnnotationNames.IndexPrefixLength)?.GetConfigurationSource();
    }
}
