// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqlServerDesignTimeServices
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IServiceCollection ConfigureDesignTimeServices([NotNull]IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IScaffoldingModelFactory, SqlServerScaffoldingModelFactory>()
                .AddSingleton<IRelationalAnnotationProvider, SqlServerAnnotationProvider>()
                .AddSingleton<IRelationalTypeMapper, SqlServerTypeMapper>()
                .AddSingleton<IDatabaseModelFactory, SqlServerDatabaseModelFactory>();
    }
}
