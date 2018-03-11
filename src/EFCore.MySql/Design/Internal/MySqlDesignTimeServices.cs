// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using EFCore.MySql.Scaffolding.Internal;
using EFCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.MySql.Design.Internal
{
    public class MySqlDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IScaffoldingProviderCodeGenerator, MySqlScaffoldingCodeGenerator>()
                .AddSingleton<IDatabaseModelFactory, MySqlDatabaseModelFactory>()
                .AddSingleton<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>()
                .AddSingleton<IRelationalTypeMapper, MySqlTypeMapper>();
        }
    }
}
