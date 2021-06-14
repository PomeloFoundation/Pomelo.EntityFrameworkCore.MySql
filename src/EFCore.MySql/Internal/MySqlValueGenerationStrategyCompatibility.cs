using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Internal
{
    public static class MySqlValueGenerationStrategyCompatibility
    {
        public static MySqlValueGenerationStrategy? GetValueGenerationStrategy(IAnnotation[] annotations)
        {
            // Allow users to use the underlying type value instead of the enum itself.
            // Workaround for: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1205
            var valueGenerationStrategy = ObjectToEnumConverter.GetEnumValue<MySqlValueGenerationStrategy>(
                annotations.FirstOrDefault(a => a.Name == MySqlAnnotationNames.ValueGenerationStrategy)?.Value);

            if (!valueGenerationStrategy.HasValue ||
                valueGenerationStrategy == MySqlValueGenerationStrategy.None)
            {
                var generatedOnAddAnnotation = annotations.FirstOrDefault(a => a.Name == MySqlAnnotationNames.LegacyValueGeneratedOnAdd)?.Value;
                if (generatedOnAddAnnotation != null && (bool)generatedOnAddAnnotation)
                {
                    valueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;
                }

                var generatedOnAddOrUpdateAnnotation = annotations.FirstOrDefault(a => a.Name == MySqlAnnotationNames.LegacyValueGeneratedOnAddOrUpdate)?.Value;
                if (generatedOnAddOrUpdateAnnotation != null && (bool)generatedOnAddOrUpdateAnnotation)
                {
                    valueGenerationStrategy = MySqlValueGenerationStrategy.ComputedColumn;
                }
            }

            return valueGenerationStrategy;
        }
    }
}
