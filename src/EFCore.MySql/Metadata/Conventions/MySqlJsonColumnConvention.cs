// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the container column type as <c>"json"</c> for complex properties
    ///     and complex collections that are mapped to JSON columns (via <c>.ToJson()</c>) in the database.
    ///     This is the MySQL equivalent of the relational <c>JsonColumnConvention</c>.
    /// </summary>
    public class MySqlJsonColumnConvention :
        IComplexPropertyAddedConvention,
        IComplexPropertyAnnotationChangedConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="MySqlJsonColumnConvention" />.
        /// </summary>
        public MySqlJsonColumnConvention(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
        {
            Dependencies = dependencies;
            RelationalDependencies = relationalDependencies;
        }

        /// <summary>
        ///     Dependencies for this service.
        /// </summary>
        protected virtual ProviderConventionSetBuilderDependencies Dependencies { get; }

        /// <summary>
        ///     Relational provider-specific dependencies for this service.
        /// </summary>
        protected virtual RelationalConventionSetBuilderDependencies RelationalDependencies { get; }

        /// <inheritdoc />
        public virtual void ProcessComplexPropertyAdded(
            IConventionComplexPropertyBuilder propertyBuilder,
            IConventionContext<IConventionComplexPropertyBuilder> context)
        {
            SetJsonColumnTypeIfNeeded(propertyBuilder);
        }

        /// <inheritdoc />
        public virtual void ProcessComplexPropertyAnnotationChanged(
            IConventionComplexPropertyBuilder propertyBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            // The JsonPropertyName annotation is set when .ToJson() is called.
            // The ContainerColumnName annotation is set by EF Core when it configures JSON mapping.
            if (name == RelationalAnnotationNames.JsonPropertyName ||
                name == RelationalAnnotationNames.ContainerColumnName)
            {
                SetJsonColumnTypeIfNeeded(propertyBuilder);
            }
        }

        private static void SetJsonColumnTypeIfNeeded(IConventionComplexPropertyBuilder propertyBuilder)
        {
            var complexProperty = propertyBuilder.Metadata;

            // Check if this complex property is mapped to a JSON column.
            // The JsonPropertyName annotation is set when .ToJson() is called.
            var jsonPropertyName = complexProperty.GetJsonPropertyName();
            if (jsonPropertyName != null)
            {
                // Set the container column type on the complex type so EF Core knows
                // to use "json" as the store type for the container column.
                var complexType = complexProperty.ComplexType;
                if (complexType is IConventionTypeBase conventionTypeBase)
                {
                    conventionTypeBase.Builder.HasAnnotation(
                        RelationalAnnotationNames.ContainerColumnType,
                        "json",
                        fromDataAnnotation: false);
                }
            }
        }
    }
}
