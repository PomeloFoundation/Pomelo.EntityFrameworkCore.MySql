// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace EFCore.MySql.Scaffolding.Internal
{
    public class MySqlCodeGenerator : ProviderCodeGenerator
    {
        public MySqlCodeGenerator([NotNull] ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override string UseProviderMethod
            => nameof(MySqlDbContextOptionsExtensions.UseMySql);
    }
}
