﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlConventionSetBuilder : RelationalConventionSetBuilder
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlConventionSetBuilder(
            [NotNull] ProviderConventionSetBuilderDependencies dependencies,
            [NotNull] RelationalConventionSetBuilderDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }


        /// <summary>
        ///     Builds and returns the convention set for the current database provider.
        /// </summary>
        /// <returns> The convention set for the current database provider. </returns>
        public override ConventionSet CreateConventionSet()
        {
            var conventionSet = base.CreateConventionSet();

            conventionSet.ModelInitializedConventions.Add(new RelationalMaxIdentifierLengthConvention(64, Dependencies, RelationalDependencies));

            var valueGeneratorConvention = new MySqlValueGenerationConvention(Dependencies, RelationalDependencies);
            ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, valueGeneratorConvention);
            ReplaceConvention(conventionSet.EntityTypePrimaryKeyChangedConventions, valueGeneratorConvention);
            ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGeneratorConvention);
            ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGeneratorConvention);
            conventionSet.PropertyAnnotationChangedConventions.Add(valueGeneratorConvention);

            return conventionSet;
        }

        /// <summary>
        ///     <para>
        ///         Call this method to build a <see cref="ConventionSet" /> for MySQL when using
        ///         the <see cref="ModelBuilder" /> outside of <see cref="DbContext.OnModelCreating" />.
        ///     </para>
        ///     <para>
        ///         Note that it is unusual to use this method.
        ///         Consider using <see cref="DbContext" /> in the normal way instead.
        ///     </para>
        /// </summary>
        /// <returns> The convention set. </returns>
        public static ConventionSet Build()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkMySql()
                .AddDbContext<DbContext>(o => o.UseMySql("Server=.", MySqlServerVersion.LatestSupportedServerVersion))
                .BuildServiceProvider();

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DbContext>())
                {
                    return ConventionSet.CreateConventionSet(context);
                }
            }
        }
    }
}
