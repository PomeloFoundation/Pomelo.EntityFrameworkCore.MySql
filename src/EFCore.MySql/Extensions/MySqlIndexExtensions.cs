// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Extension methods for <see cref="IIndex" /> for MySQL-specific metadata.
    /// </summary>
    public static class MySqlIndexExtensions
    {
        /// <summary>
        ///     Returns a value indicating whether the index is full text.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> <see langword="true"/> if the index is full text. </returns>
        public static bool? IsFullText([NotNull] this IIndex index)
            => (bool?)index[MySqlAnnotationNames.FullTextIndex];

        /// <summary>
        ///     Sets a value indicating whether the index is full text.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        public static void SetIsFullText([NotNull] this IMutableIndex index, bool? value)
            => index.SetOrRemoveAnnotation(
                MySqlAnnotationNames.FullTextIndex,
                value);

        /// <summary>
        ///     Sets a value indicating whether the index is full text.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static bool? SetIsFullText([NotNull] this IConventionIndex index, bool? value, bool fromDataAnnotation = false)
        {
            index.SetOrRemoveAnnotation(MySqlAnnotationNames.FullTextIndex, value, fromDataAnnotation);

            return value;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is full text.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is full text. </returns>
        public static ConfigurationSource? GetIsFullTextConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(MySqlAnnotationNames.FullTextIndex)?.GetConfigurationSource();

        /// <summary>
        ///     Returns a value indicating which full text parser to use.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The name of the full text parser. </returns>
        [CanBeNull] public static string FullTextParser([NotNull] this IIndex index)
            => (string)index[MySqlAnnotationNames.FullTextParser];

        /// <summary>
        ///     Sets a value indicating which full text parser to used.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        public static void SetFullTextParser([NotNull] this IMutableIndex index, [CanBeNull] string value)
            => index.SetOrRemoveAnnotation(
                MySqlAnnotationNames.FullTextParser,
                value);

        /// <summary>
        ///     Sets a value indicating which full text parser to used.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static string SetFullTextParser([NotNull] this IConventionIndex index, [CanBeNull] string value, bool fromDataAnnotation = false)
        {
            index.SetOrRemoveAnnotation(MySqlAnnotationNames.FullTextParser, value, fromDataAnnotation);

            return value;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is full text.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is full text. </returns>
        public static ConfigurationSource? GetFullTextParserConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(MySqlAnnotationNames.FullTextParser)?.GetConfigurationSource();

        /// <summary>
        ///     Returns prefix lengths for the index.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The prefix lengths.
        /// A value of `0` indicates, that the full length should be used for that column. </returns>
        public static int[] PrefixLength([NotNull] this IIndex index)
            => (int[])index[MySqlAnnotationNames.IndexPrefixLength];

        /// <summary>
        ///     Sets prefix lengths for the index.
        /// </summary>
        /// <param name="values"> The prefix lengths to set.
        /// A value of `0` indicates, that the full length should be used for that column. </param>
        /// <param name="index"> The index. </param>
        public static void SetPrefixLength([NotNull] this IMutableIndex index, int[] values)
            => index.SetOrRemoveAnnotation(
                MySqlAnnotationNames.IndexPrefixLength,
                values);

        /// <summary>
        ///     Sets prefix lengths for the index.
        /// </summary>
        /// <param name="values"> The prefix lengths to set.
        /// A value of `0` indicates, that the full length should be used for that column. </param>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static int[] SetPrefixLength([NotNull] this IConventionIndex index, int[] values, bool fromDataAnnotation = false)
        {
            index.SetOrRemoveAnnotation(MySqlAnnotationNames.IndexPrefixLength, values, fromDataAnnotation);

            return values;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for prefix lengths of the index.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for prefix lengths of the index. </returns>
        public static ConfigurationSource? GetPrefixLengthConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(MySqlAnnotationNames.IndexPrefixLength)?.GetConfigurationSource();

        /// <summary>
        ///     Returns a value indicating whether the index is spartial.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> <see langword="true"/> if the index is spartial. </returns>
        public static bool? IsSpatial([NotNull] this IIndex index)
            => (bool?)index[MySqlAnnotationNames.SpatialIndex];

        /// <summary>
        ///     Sets a value indicating whether the index is spartial.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        public static void SetIsSpatial([NotNull] this IMutableIndex index, bool? value)
            => index.SetOrRemoveAnnotation(
                MySqlAnnotationNames.SpatialIndex,
                value);

        /// <summary>
        ///     Sets a value indicating whether the index is spartial.
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static bool? SetIsSpatial([NotNull] this IConventionIndex index, bool? value, bool fromDataAnnotation = false)
        {
            index.SetOrRemoveAnnotation(MySqlAnnotationNames.SpatialIndex, value, fromDataAnnotation);

            return value;
        }

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is spartial.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is spartial. </returns>
        public static ConfigurationSource? GetIsSpatialConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(MySqlAnnotationNames.SpatialIndex)?.GetConfigurationSource();
    }
}
