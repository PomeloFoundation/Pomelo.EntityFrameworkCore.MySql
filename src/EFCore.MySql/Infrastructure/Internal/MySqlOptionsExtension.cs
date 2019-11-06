// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public class MySqlOptionsExtension : RelationalOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        public MySqlOptionsExtension()
        {
            ReplaceLineBreaksWithCharFunction = true;
        }

        public MySqlOptionsExtension([NotNull] MySqlOptionsExtension copyFrom)
            : base(copyFrom)
        {
            ServerVersion = copyFrom.ServerVersion;
            NullableCharSetBehavior = copyFrom.NullableCharSetBehavior;
            CharSet = copyFrom.CharSet;
            NoBackslashEscapes = copyFrom.NoBackslashEscapes;
            UpdateSqlModeOnOpen = copyFrom.UpdateSqlModeOnOpen;
            ReplaceLineBreaksWithCharFunction = copyFrom.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = copyFrom.DefaultDataTypeMappings;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

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
        public CharSet CharSet { get; private set; }

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

        public bool ReplaceLineBreaksWithCharFunction { get; private set; }

        public MySqlDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }

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
        public MySqlOptionsExtension WithCharSet(CharSet charSet)
        {
            var clone = (MySqlOptionsExtension)Clone();

            clone.CharSet = charSet;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension WithDisabledBackslashEscaping()
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.NoBackslashEscapes = true;
            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlOptionsExtension WithSettingSqlModeOnOpen()
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.UpdateSqlModeOnOpen = true;
            return clone;
        }

        public MySqlOptionsExtension WithDisabledLineBreakToCharSubstition()
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.ReplaceLineBreaksWithCharFunction = false;
            return clone;
        }

        public MySqlOptionsExtension WithDefaultDataTypeMappings(MySqlDefaultDataTypeMappings defaultDataTypeMappings)
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.DefaultDataTypeMappings = defaultDataTypeMappings;
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
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ (Extension.CharSet?.GetHashCode() ?? 0L);
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ Extension.NoBackslashEscapes.GetHashCode();
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ Extension.UpdateSqlModeOnOpen.GetHashCode();
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ Extension.ReplaceLineBreaksWithCharFunction.GetHashCode();
                    _serviceProviderHash = (_serviceProviderHash * 397) ^ (Extension.DefaultDataTypeMappings?.GetHashCode() ?? 0L);
                }

                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.ServerVersion)]
                    = (Extension.ServerVersion?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.CharSetBehavior)]
                    = (Extension.NullableCharSetBehavior?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.CharSet)]
                    = (Extension.CharSet?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.DisableBackslashEscaping)]
                    = Extension.NoBackslashEscapes.GetHashCode().ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.SetSqlModeOnOpen)]
                    = Extension.UpdateSqlModeOnOpen.GetHashCode().ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.DisableLineBreakToCharSubstition)]
                    = Extension.ReplaceLineBreaksWithCharFunction.GetHashCode().ToString(CultureInfo.InvariantCulture);
                debugInfo["MySql:" + nameof(MySqlDbContextOptionsBuilder.DefaultDataTypeMappings)]
                    = (Extension.DefaultDataTypeMappings?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
