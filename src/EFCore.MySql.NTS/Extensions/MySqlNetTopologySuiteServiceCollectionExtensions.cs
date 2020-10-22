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
using NetTopologySuite;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     EntityFrameworkCore.MySql.NetTopologySuite extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class MySqlNetTopologySuiteServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the services required for NetTopologySuite support in the MySQL provider for Entity Framework.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddEntityFrameworkMySqlNetTopologySuite(
            [NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            serviceCollection.TryAddSingleton(NtsGeometryServices.Instance);

            new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAddProviderSpecificServices(
                    x => x.TryAddSingletonEnumerable<IRelationalTypeMappingSourcePlugin, MySqlNetTopologySuiteTypeMappingSourcePlugin>()
                        .TryAddSingletonEnumerable<IMethodCallTranslatorPlugin, MySqlNetTopologySuiteMethodCallTranslatorPlugin>()
                        .TryAddSingletonEnumerable<IMemberTranslatorPlugin, MySqlNetTopologySuiteMemberTranslatorPlugin>()
                        .TryAddSingletonEnumerable<IMySqlEvaluatableExpressionFilter, MySqlNetTopologySuiteEvaluatableExpressionFilter>());

            return serviceCollection;
        }
    }
}
