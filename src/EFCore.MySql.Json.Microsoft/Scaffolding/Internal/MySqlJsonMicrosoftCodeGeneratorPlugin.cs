// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Scaffolding.Internal
{
    public class MySqlJsonMicrosoftCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(
                nameof(MySqlJsonMicrosoftDbContextOptionsBuilderExtensions.UseMicrosoftJson));
    }
}
