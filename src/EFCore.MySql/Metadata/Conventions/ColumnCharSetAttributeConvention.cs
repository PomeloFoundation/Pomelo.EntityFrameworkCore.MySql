﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.DataAnnotations;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the column character set for a property or field based on the applied <see cref="CharSetAttribute" />.
    /// </summary>
    public class ColumnCharSetAttributeConvention : PropertyAttributeConventionBase<CharSetAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UnicodeAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public ColumnCharSetAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            CharSetAttribute attribute,
            MemberInfo clrMember,
            IConventionContext context)
            => propertyBuilder.HasCharSet(attribute.CharSetName);
    }
}
