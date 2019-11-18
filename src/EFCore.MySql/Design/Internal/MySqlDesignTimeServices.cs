// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Pomelo.EntityFrameworkCore.MySql.Design.Internal
{
    public class MySqlDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<LoggingDefinitions, MySqlLoggingDefinitions>()
                .AddSingleton<IRelationalTypeMappingSource, MySqlTypeMappingSource>()
                .AddSingleton<IDatabaseModelFactory, MySqlDatabaseModelFactory>()
                .AddSingleton<IProviderConfigurationCodeGenerator, MySqlCodeGenerator>()
                .AddSingleton<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>();

            serviceCollection.TryAddSingleton<IMySqlOptions, MySqlOptions>();
        }
    }
}
