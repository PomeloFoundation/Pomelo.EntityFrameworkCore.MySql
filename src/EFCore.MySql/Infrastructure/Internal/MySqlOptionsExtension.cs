// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
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

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;
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
            SchemaBehavior = copyFrom.SchemaBehavior;
            SchemaNameTranslator = copyFrom.SchemaNameTranslator;
            IndexOptimizedBooleanColumns = copyFrom.IndexOptimizedBooleanColumns;
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

        public MySqlSchemaBehavior SchemaBehavior { get; private set; }
        public MySqlSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public bool IndexOptimizedBooleanColumns { get; private set; }

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

        public MySqlOptionsExtension WithSchemaBehavior(MySqlSchemaBehavior behavior, MySqlSchemaNameTranslator translator = null)
        {
            if (behavior == MySqlSchemaBehavior.Translate && translator == null)
            {
                throw new ArgumentException($"The {nameof(translator)} parameter is mandatory when using `{nameof(MySqlSchemaBehavior)}.{nameof(MySqlSchemaBehavior.Translate)}` as the specified behavior.");
            }

            var clone = (MySqlOptionsExtension)Clone();

            clone.SchemaBehavior = behavior;
            clone.SchemaNameTranslator = behavior == MySqlSchemaBehavior.Translate
                ? translator
                : null;

            return clone;
        }

        public MySqlOptionsExtension WithIndexOptimizedBooleanColumns(bool enable)
        {
            var clone = (MySqlOptionsExtension)Clone();
            clone.IndexOptimizedBooleanColumns = enable;
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
                                .Append(Extension.ServerVersion)
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
                    var hashCode = new HashCode();
                    hashCode.Add(base.GetServiceProviderHashCode());
                    hashCode.Add(Extension.ServerVersion);
                    hashCode.Add(Extension.NullableCharSetBehavior);
                    hashCode.Add(Extension.CharSet);
                    hashCode.Add(Extension.NoBackslashEscapes);
                    hashCode.Add(Extension.UpdateSqlModeOnOpen);
                    hashCode.Add(Extension.ReplaceLineBreaksWithCharFunction);
                    hashCode.Add(Extension.DefaultDataTypeMappings);
                    hashCode.Add(Extension.SchemaBehavior);
                    hashCode.Add(Extension.SchemaNameTranslator);
                    hashCode.Add(Extension.IndexOptimizedBooleanColumns);

                    _serviceProviderHash = hashCode.ToHashCode();
                }

                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(Extension.ServerVersion)] = HashCode.Combine(Extension.ServerVersion).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.CharSetBehavior)] = HashCode.Combine(Extension.NullableCharSetBehavior).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.CharSet)] = HashCode.Combine(Extension.CharSet).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.DisableBackslashEscaping)] = HashCode.Combine(Extension.NoBackslashEscapes).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.SetSqlModeOnOpen)] = HashCode.Combine(Extension.UpdateSqlModeOnOpen).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.DisableLineBreakToCharSubstition)] = HashCode.Combine(Extension.ReplaceLineBreaksWithCharFunction).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.DefaultDataTypeMappings)] = HashCode.Combine(Extension.DefaultDataTypeMappings).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.SchemaBehavior)] = HashCode.Combine(Extension.SchemaBehavior).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(Extension.SchemaNameTranslator)] = HashCode.Combine(Extension.SchemaNameTranslator).ToString(CultureInfo.InvariantCulture);
                debugInfo["Pomelo.EntityFrameworkCore.MySql:" + nameof(MySqlDbContextOptionsBuilder.EnableIndexOptimizedBooleanColumns)] = HashCode.Combine(Extension.IndexOptimizedBooleanColumns).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
