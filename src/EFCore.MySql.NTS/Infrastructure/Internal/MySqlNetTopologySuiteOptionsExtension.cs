// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class MySqlNetTopologySuiteOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkMySqlNetTopologySuite();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
            var internalServiceProvider = options.FindExtension<CoreOptionsExtension>()?.InternalServiceProvider;
            if (internalServiceProvider != null)
            {
                using (var scope = internalServiceProvider.CreateScope())
                {
                    var plugins = scope.ServiceProvider.GetService<IEnumerable<IRelationalTypeMappingSourcePlugin>>();
                    if (plugins?.Any(s => s is MySqlNetTopologySuiteTypeMappingSourcePlugin) != true)
                    {
                        throw new InvalidOperationException($"{nameof(MySqlNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite)} requires {nameof(MySqlNetTopologySuiteServiceCollectionExtensions.AddEntityFrameworkMySqlNetTopologySuite)} to be called on the internal service provider used.");
                    }
                }
            }
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new MySqlNetTopologySuiteOptionsExtension Extension
                => (MySqlNetTopologySuiteOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override long GetServiceProviderHashCode() => 0;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
                => debugInfo["MySql:" + nameof(MySqlNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite)] = "1";

            public override string LogFragment => "using NetTopologySuite ";
        }
    }
}
