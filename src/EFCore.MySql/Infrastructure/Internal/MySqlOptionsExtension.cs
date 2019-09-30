// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public class MySqlOptionsExtension : RelationalOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        public MySqlOptionsExtension()
        {
        }

        public MySqlOptionsExtension([NotNull] MySqlOptionsExtension copyFrom)
            : base(copyFrom)
        {
            ServerVersion = copyFrom.ServerVersion;
            NullableCharSetBehavior = copyFrom.NullableCharSetBehavior;
            AnsiCharSetInfo = copyFrom.AnsiCharSetInfo;
            UnicodeCharSetInfo = copyFrom.UnicodeCharSetInfo;
            NoBackslashEscapes = copyFrom.NoBackslashEscapes;
            UpdateSqlModeOnOpen = copyFrom.UpdateSqlModeOnOpen;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override DbContextOptionsExtensionInfo Info
            => _info = _info ?? new ExtensionInfo(this);

        protected override RelationalOptionsExtension Clone()
            => new MySqlOptionsExtension(this);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public ServerVersion ServerVersion { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CharSetBehavior? NullableCharSetBehavior { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CharSetInfo AnsiCharSetInfo { get; private set; }


        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CharSetInfo UnicodeCharSetInfo { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool NoBackslashEscapes { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool UpdateSqlModeOnOpen { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension WithServerVersion(ServerVersion serverVersion)
        {
            var clone = (MySqlOptionsExtension)Clone();

            clone.ServerVersion = serverVersion;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension WithCharSetBehavior(CharSetBehavior charSetBehavior)
        {
            var clone = (MySqlOptionsExtension)Clone();

            clone.NullableCharSetBehavior = charSetBehavior;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension WithAnsiCharSetInfo(CharSetInfo charSetInfo)
        {
            var clone = (MySqlOptionsExtension)Clone();

            clone.AnsiCharSetInfo = charSetInfo;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension WithUnicodeCharSetInfo(CharSetInfo charSetInfo)
        {
            var clone = (MySqlOptionsExtension)Clone();

            clone.UnicodeCharSetInfo = charSetInfo;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension DisableBackslashEscaping()
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.NoBackslashEscapes = true;
            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension SetSqlModeOnOpen()
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.UpdateSqlModeOnOpen = true;
            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkMySql();

        private sealed class ExtensionInfo : RelationalExtensionInfo
        {
            private long? _serviceProviderHash;
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new MySqlOptionsExtension Extension
                => (MySqlOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append(base.LogFragment);

                        if (Extension.ServerVersion != null)
                        {
                            builder.Append("ServerVersion ")
                                .Append(Extension.ServerVersion.Version)
                                .Append(" ")
                                .Append(Extension.ServerVersion.Type)
                                .Append(" ");
                        }

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override long GetServiceProviderHashCode()
            {
                if (_serviceProviderHash == null)
                {
                    _serviceProviderHash = (base.GetServiceProviderHashCode() * 397) ^ (Extension.ServerVersion?.GetHashCode() ?? 0L);
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ (Extension.NullableCharSetBehavior?.GetHashCode() ?? 0L);
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ (Extension.AnsiCharSetInfo?.GetHashCode() ?? 0L);
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ (Extension.UnicodeCharSetInfo?.GetHashCode() ?? 0L);
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ Extension.NoBackslashEscapes.GetHashCode();
                }

                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.ServerVersion)]
                    = (Extension.ServerVersion?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.CharSetBehavior)]
                    = (Extension.NullableCharSetBehavior?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.AnsiCharSet)]
                    = (Extension.AnsiCharSetInfo?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.UnicodeCharSet)]
                    = (Extension.UnicodeCharSetInfo?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.DisableBackslashEscaping)]
                    = Extension.NoBackslashEscapes.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
