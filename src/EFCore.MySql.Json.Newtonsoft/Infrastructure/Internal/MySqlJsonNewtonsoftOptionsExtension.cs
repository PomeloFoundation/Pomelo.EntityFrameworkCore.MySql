// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft.Infrastructure.Internal
{
    public class MySqlJsonNewtonsoftOptionsExtension : MySqlJsonOptionsExtension
    {
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkMySqlJsonNewtonsoft();

        public override string UseJsonOptionName => nameof(MySqlJsonNewtonsoftDbContextOptionsBuilderExtensions.UseNewtonsoftJson);
        public override string AddEntityFrameworkName => nameof(MySqlJsonNewtonsoftServiceCollectionExtensions.AddEntityFrameworkMySqlJsonNewtonsoft);
        public override Type TypeMappingSourcePluginType => typeof(MySqlJsonNewtonsoftTypeMappingSourcePlugin);
    }
}
