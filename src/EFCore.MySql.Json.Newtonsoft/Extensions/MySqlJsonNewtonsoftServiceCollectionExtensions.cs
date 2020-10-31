// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class MySqlJsonNewtonsoftServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the services required for JSON.NET support (Newtonsoft.Json) in Pomelo's MySQL provider for Entity Framework Core.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddEntityFrameworkMySqlJsonNewtonsoft(
            [NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAddProviderSpecificServices(
                    x => x
                        .TryAddSingletonEnumerable<IRelationalTypeMappingSourcePlugin, MySqlJsonNewtonsoftTypeMappingSourcePlugin>()
                        .TryAddSingletonEnumerable<IMethodCallTranslatorPlugin, MySqlJsonNewtonsoftMethodCallTranslatorPlugin>()
                        .TryAddSingletonEnumerable<IMemberTranslatorPlugin, MySqlJsonNewtonsoftMemberTranslatorPlugin>()
                        .TryAddSingleton<IMySqlJsonPocoTranslator, MySqlJsonNewtonsoftPocoTranslator>());

            return serviceCollection;
        }
    }
}
