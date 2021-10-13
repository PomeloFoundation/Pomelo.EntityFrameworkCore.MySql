// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Scaffolding.Internal
{
    public class MySqlJsonNewtonsoftCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        private static readonly MethodInfo _useNewtonsoftJsonMethodInfo =
            typeof(MySqlJsonNewtonsoftDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(MySqlJsonNewtonsoftDbContextOptionsBuilderExtensions.UseNewtonsoftJson),
                typeof(MySqlDbContextOptionsBuilder),
                typeof(MySqlCommonJsonChangeTrackingOptions));

        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(_useNewtonsoftJsonMethodInfo);
    }
}
