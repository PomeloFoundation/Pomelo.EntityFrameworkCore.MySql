// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    public class MySqlNetTopologySuiteCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        private static readonly MethodInfo _useNetTopologySuiteMethodInfo =
            typeof(MySqlNetTopologySuiteDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(MySqlNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite),
                typeof(MySqlDbContextOptionsBuilder));

        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(_useNetTopologySuiteMethodInfo);
    }
}
