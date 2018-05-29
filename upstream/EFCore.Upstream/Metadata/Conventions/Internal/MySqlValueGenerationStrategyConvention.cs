// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlValueGenerationStrategyConvention : IModelInitializedConvention, IModelBuiltConvention
    {
        InternalModelBuilder IModelInitializedConvention.Apply(InternalModelBuilder modelBuilder)
        {
            modelBuilder.MySql(ConfigurationSource.Convention).ValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn);

            return modelBuilder;
        }

        InternalModelBuilder IModelBuiltConvention.Apply(InternalModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                foreach (var property in entityType.GetDeclaredProperties())
                {
                    property.Builder.MySql(ConfigurationSource.Convention)
                        .ValueGenerationStrategy(property.MySql().ValueGenerationStrategy);
                }
            }

            return modelBuilder;
        }
    }
}
