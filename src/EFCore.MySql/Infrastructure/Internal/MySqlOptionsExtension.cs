// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal
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
        public bool SelectForUpdate { get; private set; }
        public MySqlOptionsExtension WithSelectForUpdate(bool sfu=true)
        {
            this.SelectForUpdate = sfu;
            return this;
        }
    }
}
