// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Infrastructure.Internal
{
    public class MySqlJsonMicrosoftOptionsExtension : MySqlJsonOptionsExtension
    {
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkMySqlJsonMicrosoft();

        public override string UseJsonOptionName => nameof(MySqlJsonMicrosoftDbContextOptionsBuilderExtensions.UseMicrosoftJson);
        public override string AddEntityFrameworkName => nameof(MySqlJsonMicrosoftServiceCollectionExtensions.AddEntityFrameworkMySqlJsonMicrosoft);
        public override Type TypeMappingSourcePluginType => typeof(MySqlJsonMicrosoftTypeMappingSourcePlugin);
    }
}
