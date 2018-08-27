// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    public sealed class MySqlOptionsExtension : RelationalOptionsExtension
    {
        private long? _serviceProviderHash;
        private string _logFragment;

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
        }

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
        public override long GetServiceProviderHashCode()
        {
            if (_serviceProviderHash == null)
            {
                _serviceProviderHash = (base.GetServiceProviderHashCode() * 397) ^ (ServerVersion?.GetHashCode() ?? 0L);
                _serviceProviderHash = (_serviceProviderHash * 397) ^ (NullableCharSetBehavior?.GetHashCode() ?? 0L);
                _serviceProviderHash = (_serviceProviderHash * 397) ^ (AnsiCharSetInfo?.GetHashCode() ?? 0L);
                _serviceProviderHash = (_serviceProviderHash * 397) ^ (UnicodeCharSetInfo?.GetHashCode() ?? 0L);
                _serviceProviderHash = (_serviceProviderHash * 397) ^ NoBackslashEscapes.GetHashCode();
            }

            return _serviceProviderHash.Value;
        }

        public override bool ApplyServices(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            services.AddEntityFrameworkMySql();
            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    var builder = new StringBuilder();

                    builder.Append(base.LogFragment);

                    if (ServerVersion != null)
                    {
                        builder.Append("ServerVersion ")
                            .Append(ServerVersion.Version)
                            .Append(" ")
                            .Append(ServerVersion.Type)
                            .Append(" ");
                    }

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }
    }
}
