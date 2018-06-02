// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlByteArrayTypeMapping : ByteArrayTypeMapping
    {
        private const int MaxSize = 8000;
        private readonly int _maxSpecificSize;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlByteArrayTypeMapping(
            string storeType = null,
            int? size = null,
            bool fixedLength = false)
            : this(System.Data.DbType.Binary,
                storeType,
                size.HasValue && size < MaxSize ? size : null,
                fixedLength)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlByteArrayTypeMapping(
            DbType type,
            string storeType,
            int? size,
            bool fixedLength)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(byte[])),
                    storeType ?? GetBaseType(size, fixedLength),
                    GetStoreTypePostfix(size),
                    type,
                    size: size,
                    fixedLength: fixedLength))
        {
        }

        private static string GetBaseType(int? size, bool isFixedLength)
            => size == null
                ? "longblob"
                : isFixedLength ? "binary" : "varbinary";

        private static StoreTypePostfix GetStoreTypePostfix(int? size)
            => size != null && size <= MaxSize ? StoreTypePostfix.Size : StoreTypePostfix.None;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlByteArrayTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
            _maxSpecificSize = CalculateSize(parameters.Size);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new MySqlByteArrayTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override CoreTypeMapping Clone(ValueConverter converter)
            => new MySqlByteArrayTypeMapping(Parameters.WithComposedConverter(converter));

        private static int CalculateSize(int? size)
            => size.HasValue && size < MaxSize ? size.Value : MaxSize;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            // For strings and byte arrays, set the max length to the size facet if specified, or
            // 8000 bytes if no size facet specified, if the data will fit so as to avoid query cache
            // fragmentation by setting lots of different Size values otherwise always set to
            // -1 (unbounded) to avoid SQL client size inference.

            var value = parameter.Value;
            var length = (value as string)?.Length ?? (value as byte[])?.Length;

            parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
                ? _maxSpecificSize
                : -1;
        }

        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral(object value) => ByteArrayFormatter.ToHex((byte[])value);
    }
}
