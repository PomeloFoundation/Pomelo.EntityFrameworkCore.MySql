// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public class MySqlVectorByteTypeMapping : MySqlTypeMapping
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlVectorByteTypeMapping(
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
                valueConverter: GetValueConverter(clrType),
                valueComparer: GetValueComparer(clrType))
        {
        }

        protected MySqlVectorByteTypeMapping(RelationalTypeMappingParameters parameters, MySqlDbType mySqlDbType)
            : base(parameters, mySqlDbType)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
             => new MySqlVectorByteTypeMapping(parameters, MySqlDbType);
        private static ValueConverter GetValueConverter(Type clrType)
        {
            return clrType switch
            {
                Type t when t == typeof(float[]) =>
                    new ValueConverter<float[], byte[]>(
                        v => FloatArrayToBytes(v),
                        v => BytesToFloatArray(v)),

                Type t when t == typeof(ReadOnlyMemory<float>) =>
                    new ValueConverter<ReadOnlyMemory<float>, byte[]>(
                        v => FloatArrayToBytes(v.ToArray()),
                        v => new ReadOnlyMemory<float>(BytesToFloatArray(v))),

                Type t when t == typeof(Memory<float>) =>
                    new ValueConverter<Memory<float>, byte[]>(
                        v => FloatArrayToBytes(v.ToArray()),
                        v => new Memory<float>(BytesToFloatArray(v))),

                _ => throw new InvalidOperationException($"Unsupported CLR type for vector: {clrType}"),
            };
        }

        private static ValueComparer GetValueComparer(Type clrType)
        {
            return clrType switch
            {
                Type t when t == typeof(float[]) =>
                    new ValueComparer<float[]>(
                        (a, b) => a.SequenceEqual(b),
                        v => v.Aggregate(0, (hash, x) => HashCode.Combine(hash, x.GetHashCode())),
                        v => v.ToArray()),

                Type t when t == typeof(ReadOnlyMemory<float>) =>
                    new ValueComparer<ReadOnlyMemory<float>>(
                        (a, b) => a.ToArray().SequenceEqual(b.ToArray()),
                        v => v.ToArray().Aggregate(0, (hash, x) => HashCode.Combine(hash, x.GetHashCode())),
                        v => new ReadOnlyMemory<float>(v.ToArray())),

                Type t when t == typeof(Memory<float>) =>
                    new ValueComparer<Memory<float>>(
                        (a, b) => a.ToArray().SequenceEqual(b.ToArray()),
                        v => v.ToArray().Aggregate(0, (hash, x) => HashCode.Combine(hash, x.GetHashCode())),
                        v => new Memory<float>(v.ToArray())),

                _ => throw new InvalidOperationException($"Unsupported CLR type for vector: {clrType}"),
            };
        }

        private static byte[] FloatArrayToBytes(float[] values)
        {
            var result = new byte[values.Length * sizeof(float)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static float[] BytesToFloatArray(byte[] bytes)
        {
            var result = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }
    }
}
