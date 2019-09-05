using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="IIndex" /> for SQL Server-specific metadata.
    /// </summary>
    public static class MySqlIndexExtensions
    {
        /// <summary>
        ///     Returns a value indicating whether the index is full text.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> <c>true</c> if the index is clustered. </returns>
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
        public static void SetIsFullText(
            [NotNull] this IConventionIndex index, bool? value, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(
                MySqlAnnotationNames.FullTextIndex,
                value,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is full text.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is full text. </returns>
        public static ConfigurationSource? GetIsFullTextConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(MySqlAnnotationNames.FullTextIndex)?.GetConfigurationSource();


        /// <summary>
        ///     Returns a value indicating whether the index is spartial.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> <c>true</c> if the index is clustered. </returns>
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
        public static void SetIsSpatial(
            [NotNull] this IConventionIndex index, bool? value, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(
                MySqlAnnotationNames.SpatialIndex,
                value,
                fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for whether the index is spartial.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for whether the index is spartial. </returns>
        public static ConfigurationSource? GetIsSpatialConfigurationSource([NotNull] this IConventionIndex property)
            => property.FindAnnotation(MySqlAnnotationNames.SpatialIndex)?.GetConfigurationSource();
    }
}
