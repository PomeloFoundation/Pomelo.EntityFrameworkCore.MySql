// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures store value generation as <see cref="ValueGenerated.OnAdd"/> on properties that are
    ///     part of the primary key and not part of any foreign keys, were configured to have a database default value
    ///     or were configured to use a <see cref="MySqlValueGenerationStrategy"/>.
    ///     It also configures properties as <see cref="ValueGenerated.OnAddOrUpdate"/> if they were configured as computed columns.
    /// </summary>
    public class MySqlValueGenerationConvention : RelationalValueGenerationConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="MySqlValueGenerationConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        /// <param name="relationalDependencies">  Parameter object containing relational dependencies for this convention. </param>
        public MySqlValueGenerationConvention(
            [NotNull] ProviderConventionSetBuilderDependencies dependencies,
            [NotNull] RelationalConventionSetBuilderDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     Called after an annotation is changed on a property.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property. </param>
        /// <param name="name"> The annotation name. </param>
        /// <param name="annotation"> The new annotation. </param>
        /// <param name="oldAnnotation"> The old annotation.  </param>
        /// <param name="context"> Additional information associated with convention execution. </param>
        public override void ProcessPropertyAnnotationChanged(
            IConventionPropertyBuilder propertyBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            if (name == MySqlAnnotationNames.ValueGenerationStrategy)
            {
                propertyBuilder.ValueGenerated(GetValueGenerated(propertyBuilder.Metadata));
                return;
            }

            base.ProcessPropertyAnnotationChanged(propertyBuilder, name, annotation, oldAnnotation, context);
        }

        /// <summary>
        ///     Returns the store value generation strategy to set for the given property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> The store value generation strategy to set for the given property. </returns>
        protected override ValueGenerated? GetValueGenerated(IConventionProperty property)
        {
            var tableName = property.DeclaringEntityType.GetTableName();
            if (tableName == null)
            {
                return null;
            }

            return GetValueGenerated(property, StoreObjectIdentifier.Table(tableName, property.DeclaringEntityType.GetSchema()));
        }

        /// <summary>
        ///     Returns the store value generation strategy to set for the given property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="storeObject"> The identifier of the store object. </param>
        /// <returns> The store value generation strategy to set for the given property. </returns>
        public static new ValueGenerated? GetValueGenerated([NotNull] IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var valueGenerated = RelationalValueGenerationConvention.GetValueGenerated(property, storeObject);
            if (valueGenerated != null)
            {
                return valueGenerated;
            }

            var valueGenerationStrategy = property.GetValueGenerationStrategy(storeObject);
            if (valueGenerationStrategy.HasValue)
            {
                switch (valueGenerationStrategy.Value)
                {
                    case MySqlValueGenerationStrategy.IdentityColumn:
                        return ValueGenerated.OnAdd;
                    case MySqlValueGenerationStrategy.ComputedColumn:
                        return ValueGenerated.OnAddOrUpdate;
                }
            }

            return null;
        }
    }
}
