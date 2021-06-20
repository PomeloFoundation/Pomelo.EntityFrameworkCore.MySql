// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the collation for an entity based on the applied <see cref="MySqlCollationAttribute" />.
    /// </summary>
    public class TableCollationAttributeConvention : EntityTypeAttributeConventionBase<MySqlCollationAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TableCollationAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public TableCollationAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            MySqlCollationAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
            => entityTypeBuilder.UseCollation(attribute.CollationName, attribute.DelegationModes);
    }
}
