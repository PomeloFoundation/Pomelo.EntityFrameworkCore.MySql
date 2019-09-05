using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Extensions
{
    public static class MySqlModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the default value generation strategy for key properties marked as <see cref="ValueGenerated.OnAdd" />,
        ///     when targeting SQL Server.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="valueGenerationStrategy"> The value generation strategy. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <c>null</c> otherwise.
        /// </returns>
        public static IConventionModelBuilder HasValueGenerationStrategy(
            [NotNull] this IConventionModelBuilder modelBuilder,
            MySqlValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
                if (valueGenerationStrategy != MySqlValueGenerationStrategy.IdentityColumn)
                {
                    //modelBuilder.HasIdentityColumnSeed(null, fromDataAnnotation);
                    //modelBuilder.HasIdentityColumnIncrement(null, fromDataAnnotation);
                }

                return modelBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the default value generation strategy.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="valueGenerationStrategy"> The value generation strategy. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <c>true</c> if the given value can be set as the default value generation strategy. </returns>
        public static bool CanSetValueGenerationStrategy(
            [NotNull] this IConventionModelBuilder modelBuilder,
            MySqlValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            return modelBuilder.CanSetAnnotation(
                MySqlAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation);
        }
    }
}
