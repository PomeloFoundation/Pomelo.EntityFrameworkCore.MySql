// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" /> and multiple registrations
    ///         are allowed. This means a single instance of each service is used by many <see cref="DbContext" />
    ///         instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class MySqlJsonTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
    {
        [NotNull] private readonly IMySqlOptions _options;

        public MySqlJsonTypeMappingSourcePlugin([NotNull] IMySqlOptions options)
        {
            _options = options;
        }

        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;

            if (storeTypeName != null)
            {
                return storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase)
                    ? (RelationalTypeMapping)Activator.CreateInstance(
                        typeof(MySqlJsonTypeMapping<>).MakeGenericType(clrType ?? typeof(string)),
                        storeTypeName,
                        _options)
                    : null;
            }

            if (clrType == typeof(JsonDocument) ||
                clrType == typeof(JsonElement))
            {
                return (RelationalTypeMapping)Activator.CreateInstance(
                    typeof(MySqlJsonTypeMapping<>).MakeGenericType(clrType),
                    "json",
                    _options);
            }

            return null;
        }
    }
}
