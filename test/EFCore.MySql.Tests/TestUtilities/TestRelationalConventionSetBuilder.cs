// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestRelationalConventionSetBuilder : RelationalConventionSetBuilder
    {
        public TestRelationalConventionSetBuilder(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        public static ConventionSet Build()
        {
            // TODO: is this correct?
            var optionsBuilder = new DbContextOptionsBuilder();
            var context = new DbContext(optionsBuilder.Options);
            return ConventionSet.CreateConventionSet(context);
        }
            //=> new TestRelationalConventionSetBuilder(
            //    new RelationalConventionSetBuilderDependencies(
            //        TestServiceFactory.Instance.Create<TestRelationalTypeMappingSource>(),
            //        new FakeDiagnosticsLogger<DbLoggerCategory.Model>(),
            //        null,
            //        null,
            //        null))
            //    .AddConventions(
            //        TestServiceFactory.Instance.Create<CoreConventionSetBuilder>()
            //            .CreateConventionSet());
    }
}
