// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class MySqlValueGeneratorConvention : RelationalValueGeneratorConvention
    {
        public override Annotation Apply(InternalPropertyBuilder propertyBuilder, string name, Annotation annotation, Annotation oldAnnotation)
        {
            if (name == MySqlAnnotationNames.ValueGenerationStrategy)
            {
                propertyBuilder.ValueGenerated(GetValueGenerated(propertyBuilder.Metadata), ConfigurationSource.Convention);
                return annotation;
            }

            return base.Apply(propertyBuilder, name, annotation, oldAnnotation);
        }

        public override ValueGenerated? GetValueGenerated(Property property)
        {
            var valueGenerated = base.GetValueGenerated(property);
            if (valueGenerated != null)
            {
                return valueGenerated;
            }

            return property.MySql().GetMySqlValueGenerationStrategy(fallbackToModel: false) != null
                ? ValueGenerated.OnAdd
                : (ValueGenerated?)null;
        }
    }
}
