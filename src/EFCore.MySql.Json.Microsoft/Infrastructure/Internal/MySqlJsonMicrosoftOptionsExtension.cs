// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Infrastructure.Internal
{
    public class MySqlJsonMicrosoftOptionsExtension : MySqlJsonOptionsExtension
    {
        public MySqlJsonMicrosoftOptionsExtension()
        {
        }

        public MySqlJsonMicrosoftOptionsExtension([NotNull] MySqlJsonOptionsExtension copyFrom)
            : base(copyFrom)
        {
        }

        protected override MySqlJsonOptionsExtension Clone()
            => new MySqlJsonMicrosoftOptionsExtension(this);

        public override string UseJsonOptionName => nameof(MySqlJsonMicrosoftDbContextOptionsBuilderExtensions.UseMicrosoftJson);
        public override string AddEntityFrameworkName => nameof(MySqlJsonMicrosoftServiceCollectionExtensions.AddEntityFrameworkMySqlJsonMicrosoft);
        public override Type TypeMappingSourcePluginType => typeof(MySqlJsonMicrosoftTypeMappingSourcePlugin);

        /// <summary>
        ///     Adds the services required to make the selected options work. This is used when there
        ///     is no external <see cref="IServiceProvider" /> and EF is maintaining its own service
        ///     provider internally. This allows database providers (and other extensions) to register their
        ///     required services when EF is creating an service provider.
        /// </summary>
        /// <param name="services"> The collection to add services to. </param>
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkMySqlJsonMicrosoft();
    }
}
