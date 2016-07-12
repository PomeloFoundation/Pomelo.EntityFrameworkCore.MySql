// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Migrations.Internal
{
    public class MySqlMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            // The migrations SQL generator gets the property's DefaultValue and DefaultValueSql.
            // However, there's no way there to detect properties that have ValueGenerated.OnAdd
            // *without* defining a default value; these should translate to SERIAL columns.
            // So we add a custom annotation here to pass the information.
            if (property.ValueGenerated == ValueGenerated.OnAdd)
                yield return new Annotation(MySqlAnnotationNames.Prefix + MySqlAnnotationNames.ValueGeneratedOnAdd, true);
        }

        public override IEnumerable<IAnnotation> For(IIndex index)
        {
            if (index.MySql().Method != null)
            {
                yield return new Annotation(
                     MySqlAnnotationNames.Prefix + MySqlAnnotationNames.IndexMethod,
                     index.MySql().Method);
            }
        }
    }
}