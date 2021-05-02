// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.Design.Internal
{
    public class MySqlDesignTimeServices : IDesignTimeServices
    {
        public virtual void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkMySql();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                .TryAdd<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>()
                .TryAdd<IDatabaseModelFactory, MySqlDatabaseModelFactory>()
                .TryAdd<IProviderConfigurationCodeGenerator, MySqlCodeGenerator>()
                .TryAddCoreServices();
        }
    }
}
