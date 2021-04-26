// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

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
