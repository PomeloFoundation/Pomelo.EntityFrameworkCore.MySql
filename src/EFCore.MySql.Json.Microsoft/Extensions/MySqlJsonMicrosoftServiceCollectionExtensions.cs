// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microting.EntityFrameworkCore.MySql.Json.Microsoft.Query.Internal;
using Microting.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microting.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Microting.EntityFrameworkCore.MySql.Json.Microsoft extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class MySqlJsonMicrosoftServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the services required for Microsoft JSON support (System.Text.Json) in Microting's MySQL provider for Entity Framework Core.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddEntityFrameworkMySqlJsonMicrosoft(
            [NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<IRelationalTypeMappingSourcePlugin, MySqlJsonMicrosoftTypeMappingSourcePlugin>()
                .TryAdd<IMethodCallTranslatorPlugin, MySqlJsonMicrosoftMethodCallTranslatorPlugin>()
                .TryAdd<IMemberTranslatorPlugin, MySqlJsonMicrosoftMemberTranslatorPlugin>()
                .TryAddProviderSpecificServices(
                    x => x.TryAddScopedEnumerable<IMySqlJsonPocoTranslator, MySqlJsonMicrosoftPocoTranslator>());

            return serviceCollection;
        }
    }
}
