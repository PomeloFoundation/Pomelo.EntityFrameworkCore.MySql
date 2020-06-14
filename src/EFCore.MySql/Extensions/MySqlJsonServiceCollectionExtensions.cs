// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using Pomelo.EntityFrameworkCore.MySql;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     EntityFrameworkCore.MySql.Json extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class MySqlJsonServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the services required for Json support in the MySQL provider for Entity Framework.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddEntityFrameworkMySqlJson(
            [NotNull] this IServiceCollection serviceCollection,
            [NotNull] IMySqlJsonSerializer jsonSerializer)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            serviceCollection.TryAddSingleton(jsonSerializer);

            new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAddProviderSpecificServices(
                    x => x.TryAddSingletonEnumerable<IRelationalTypeMappingSourcePlugin, MySqlJsonTypeMappingSourcePlugin>()
                        .TryAddSingletonEnumerable<IMethodCallTranslatorPlugin, MySqlJsonMethodCallTranslatorPlugin>()
                        .TryAddSingletonEnumerable<IMemberTranslatorPlugin, MySqlJsonMemberTranslatorPlugin>());

            return serviceCollection;
        }
    }
}
