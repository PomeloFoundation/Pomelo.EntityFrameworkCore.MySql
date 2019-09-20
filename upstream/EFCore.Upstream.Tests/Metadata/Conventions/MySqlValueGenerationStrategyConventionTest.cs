// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    public class MySqlValueGenerationStrategyConventionTest
    {
        [ConditionalFact]
        public void Annotations_are_added_when_conventional_model_builder_is_used()
        {
            var model = MySqlTestHelpers.Instance.CreateConventionBuilder().Model;
            model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);

            var annotations = model.GetAnnotations().OrderBy(a => a.Name).ToList();
            Assert.Equal(2, annotations.Count);

            Assert.Equal(MySqlAnnotationNames.ValueGenerationStrategy, annotations.Last().Name);
            Assert.Equal(MySqlValueGenerationStrategy.IdentityColumn, annotations.Last().Value);
        }

        [ConditionalFact]
        public void Annotations_are_added_when_conventional_model_builder_is_used_with_sequences()
        {
            var model = MySqlTestHelpers.Instance.CreateConventionBuilder()
                .UseHiLo()
                .Model;

            model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);

            var annotations = model.GetAnnotations().OrderBy(a => a.Name).ToList();
            Assert.Equal(4, annotations.Count);

            Assert.Equal(RelationalAnnotationNames.MaxIdentifierLength, annotations[0].Name);

            Assert.Equal(
                RelationalAnnotationNames.SequencePrefix +
                "." +
                MySqlModelExtensions.DefaultHiLoSequenceName,
                annotations[1].Name);
            Assert.NotNull(annotations[1].Value);

            Assert.Equal(MySqlAnnotationNames.HiLoSequenceName, annotations[2].Name);
            Assert.Equal(MySqlModelExtensions.DefaultHiLoSequenceName, annotations[2].Value);

            Assert.Equal(MySqlAnnotationNames.ValueGenerationStrategy, annotations[3].Name);
            Assert.Equal(MySqlValueGenerationStrategy.SequenceHiLo, annotations[3].Value);
        }
    }
}
