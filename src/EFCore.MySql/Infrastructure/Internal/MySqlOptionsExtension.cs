// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.MySql.Infrastructure.Internal
{
    public sealed class MySqlOptionsExtension : RelationalOptionsExtension
    {
        public MySqlOptionsExtension()
        {
        }

        public MySqlOptionsExtension([NotNull] RelationalOptionsExtension copyFrom)
            : base(copyFrom)
        {
        }

        protected override RelationalOptionsExtension Clone()
            => new MySqlOptionsExtension(this);

        public override bool ApplyServices(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            services.AddEntityFrameworkMySql();
            return true;
        }
    }
}
