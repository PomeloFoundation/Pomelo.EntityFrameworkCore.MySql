// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlValueGenerationStrategyConvention : DatabaseGeneratedAttributeConvention, IModelInitializedConvention
    {
        public override InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder, DatabaseGeneratedAttribute attribute, MemberInfo clrMember)
        {
            MySqlValueGenerationStrategy? valueGenerationStrategy = null;
            ValueGenerated valueGenerated = ValueGenerated.Never;
            if (attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed)
            {
                valueGenerated = ValueGenerated.OnAddOrUpdate;
                valueGenerationStrategy = MySqlValueGenerationStrategy.ComputedColumn;
            }
            else if (attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
            {
                valueGenerated = ValueGenerated.OnAdd;
                valueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;
            }

            propertyBuilder.ValueGenerated(valueGenerated, ConfigurationSource.Convention);
            propertyBuilder.MySql(ConfigurationSource.DataAnnotation).ValueGenerationStrategy(valueGenerationStrategy);

            return base.Apply(propertyBuilder, attribute, clrMember);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalModelBuilder Apply(InternalModelBuilder modelBuilder)
        {
            modelBuilder.MySql(ConfigurationSource.Convention).ValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn);

            return modelBuilder;
        }
    }
}
