// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
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
                .TryAddProviderSpecificServices(serviceMap => serviceMap
                    .TryAddSingleton<ICSharpEntityTypeGenerator, FixedCSharpEntityTypeGenerator>()) // TODO: Remove after CSharpEntityTypeGenerator has been fixed in EF Core upstream (6.0.0-preview.6).
                .TryAddCoreServices();
        }
    }
}
