// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using EFCore.MySql.Scaffolding.Internal;
using EFCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using FallbackRelationalCoreTypeMapper =
    EFCore.MySql.Storage.Internal.FallbackRelationalCoreTypeMapper;

namespace EFCore.MySql.Design.Internal
{
    public class MySqlDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IRelationalCoreTypeMapper, FallbackRelationalCoreTypeMapper>()
                .AddSingleton<IRelationalTypeMapper, MySqlTypeMapper>()
                .AddSingleton<IDatabaseModelFactory, MySqlDatabaseModelFactory>()
                .AddSingleton<IProviderCodeGenerator, MySqlCodeGenerator>()
                .AddSingleton<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>();
        }
    }
}
