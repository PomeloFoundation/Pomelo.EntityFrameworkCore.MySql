// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public abstract class MySqlJsonOptionsExtension
        : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        protected MySqlJsonOptionsExtension()
        {
        }

        protected MySqlJsonOptionsExtension([NotNull] MySqlJsonOptionsExtension copyFrom)
        {
            Check.NotNull(copyFrom, nameof(copyFrom));

            JsonChangeTrackingOptions = copyFrom.JsonChangeTrackingOptions;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        /// <summary>
        ///     Override this method in a derived class to ensure that any clone created is also of that class.
        /// </summary>
        /// <returns> A clone of this instance, which can be modified before being returned as immutable. </returns>
        protected abstract MySqlJsonOptionsExtension Clone();

        public virtual MySqlJsonChangeTrackingOptions JsonChangeTrackingOptions { get; set; }

        public abstract string UseJsonOptionName { get; }
        public abstract string AddEntityFrameworkName { get; }
        public abstract Type TypeMappingSourcePluginType { get; }

        /// <summary>
        ///     The change tracking option that will be used as the default for all JSON column mapped properties.
        /// </summary>
        public virtual MySqlJsonOptionsExtension WithJsonChangeTrackingOptions(MySqlCommonJsonChangeTrackingOptions options)
            => WithJsonChangeTrackingOptions(options.ToJsonChangeTrackingOptions());

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual MySqlJsonOptionsExtension WithJsonChangeTrackingOptions(MySqlJsonChangeTrackingOptions options)
        {
            var clone = Clone();
            clone.JsonChangeTrackingOptions = options;
            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public abstract void ApplyServices(IServiceCollection services);

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
                    if (plugins?.Any(s => s.GetType() == TypeMappingSourcePluginType) != true)
                    {
                        throw new InvalidOperationException($"'{UseJsonOptionName}' requires {AddEntityFrameworkName} to be called on the internal service provider used.");
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

            private new MySqlJsonOptionsExtension Extension
                => (MySqlJsonOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;
            public override long GetServiceProviderHashCode()
            {
                var hashCode = new HashCode();
                hashCode.Add(Extension.JsonChangeTrackingOptions);
                return hashCode.ToHashCode();
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["MySql:" + Extension.UseJsonOptionName] = "1";
                debugInfo["MySql:" + nameof(JsonChangeTrackingOptions)]
                    = Extension.JsonChangeTrackingOptions.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }

            public override string LogFragment => $"using {Extension.UseJsonOptionName}";
        }
    }
}
