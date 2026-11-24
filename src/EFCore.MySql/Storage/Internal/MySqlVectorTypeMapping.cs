// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class MySqlVectorTypeMapping : MySqlTypeMapping
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlVectorTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            int? size = null)
            : base(
                storeType,
                clrType,
                MySqlDbType.Vector,
                size: size,
                unicode: false,
                storeTypePostfix: StoreTypePostfix.Size,
                valueConverter: GetValueConverter(clrType))
        {
        }

        protected MySqlVectorTypeMapping(RelationalTypeMappingParameters parameters, MySqlDbType mySqlDbType)
            : base(parameters, mySqlDbType)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
             => new MySqlVectorTypeMapping(parameters, MySqlDbType);

        private static ValueConverter GetValueConverter(Type clrType)
        {
            return clrType switch
            {
                Type t when t == typeof(float[]) => new ValueConverter<float[], ReadOnlyMemory<float>>(v => v.AsMemory(), v => v.ToArray()),
                Type t when t == typeof(ReadOnlyMemory<float>) => null,
                Type t when t == typeof(Memory<float>) => new ValueConverter<Memory<float>, ReadOnlyMemory<float>>(v => v, v => v.ToArray()),
                _ => throw new InvalidOperationException($"Unsupported CLR type for vector: {clrType}"),
            };
        }
    }
}
