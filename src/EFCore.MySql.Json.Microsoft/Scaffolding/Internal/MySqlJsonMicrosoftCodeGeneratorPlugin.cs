// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Scaffolding.Internal
{
    public class MySqlJsonMicrosoftCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        private static readonly MethodInfo _useMicrosoftJsonMethodInfo =
            typeof(MySqlJsonMicrosoftDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(MySqlJsonMicrosoftDbContextOptionsBuilderExtensions.UseMicrosoftJson),
                typeof(MySqlDbContextOptionsBuilder),
                typeof(MySqlCommonJsonChangeTrackingOptions));

        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(_useMicrosoftJsonMethodInfo);
    }
}
