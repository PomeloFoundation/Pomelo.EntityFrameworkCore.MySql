// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using EFCore.MySql.Metadata.Conventions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    public class MySqlConventionSetBuilder : RelationalConventionSetBuilder
    {
        public MySqlConventionSetBuilder(
            [NotNull] RelationalConventionSetBuilderDependencies dependencies
        )
            : base(dependencies)
        {
        }

        public override ConventionSet AddConventions(ConventionSet conventionSet)
        {
            Check.NotNull(conventionSet, nameof(conventionSet));

            base.AddConventions(conventionSet);

            var valueGenerationStrategyConvention = new MySqlValueGenerationStrategyConvention();
            conventionSet.ModelInitializedConventions.Add(valueGenerationStrategyConvention);
            conventionSet.ModelInitializedConventions.Add(new RelationalMaxIdentifierLengthConvention(64));

            ValueGeneratorConvention valueGeneratorConvention = new MySqlValueGeneratorConvention();
            ReplaceConvention(conventionSet.BaseEntityTypeChangedConventions, valueGeneratorConvention);

            ReplaceConvention(conventionSet.PrimaryKeyChangedConventions, valueGeneratorConvention);

            ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGeneratorConvention);

            ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGeneratorConvention);

            conventionSet.PropertyAnnotationChangedConventions.Add(
                (MySqlValueGeneratorConvention)valueGeneratorConvention);

            return conventionSet;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static ConventionSet Build()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkMySql()
                .AddDbContext<DbContext>(o => o.UseMySql("server=."))
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
