// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    public class MySqlConventionSetBuilder : RelationalConventionSetBuilder
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public MySqlConventionSetBuilder(
            [NotNull] RelationalConventionSetBuilderDependencies dependencies,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper)
            : base(dependencies)
        {
            _sqlGenerationHelper = sqlGenerationHelper;
        }

        public override ConventionSet AddConventions(ConventionSet conventionSet)
        {
            Check.NotNull(conventionSet, nameof(conventionSet));

            base.AddConventions(conventionSet);

            var valueGenerationStrategyConvention = new MySqlValueGenerationStrategyConvention();
            conventionSet.ModelInitializedConventions.Add(valueGenerationStrategyConvention);
            
            ReplaceConvention(conventionSet.PropertyAddedConventions, (DatabaseGeneratedAttributeConvention)valueGenerationStrategyConvention);
            ReplaceConvention(conventionSet.PropertyFieldChangedConventions, (DatabaseGeneratedAttributeConvention)valueGenerationStrategyConvention);

            return conventionSet;
        }

        public static ConventionSet Build()
        {
            var typeMapper = new MySqlTypeMapper(new RelationalTypeMapperDependencies());

            return new MySqlConventionSetBuilder(
                    new RelationalConventionSetBuilderDependencies(typeMapper, null, null),
                    new MySqlSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()))
                .AddConventions(
                    new CoreConventionSetBuilder(
                            new CoreConventionSetBuilderDependencies(typeMapper))
                        .CreateConventionSet());
        }
    }
}
