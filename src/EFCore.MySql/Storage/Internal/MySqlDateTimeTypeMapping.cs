// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         Represents the mapping between a .NET <see cref="DateTime" /> type and a database type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class MySqlDateTimeTypeMapping : RelationalTypeMapping
    {
        private const string DateTimeFormatConst6 = @"'{0:yyyy-MM-dd HH\:mm\:ss.ffffff}'";
        private const string DateTimeFormatConst = @"'{0:yyyy-MM-dd HH\:mm\:ss}'";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlDateTimeTypeMapping(
            [NotNull] string storeType,
            ValueConverter converter = null,
            ValueComparer comparer = null,
            int? precision = null)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(DateTime), converter, comparer),
                    storeType,
                    precision == null ? StoreTypePostfix.None : StoreTypePostfix.Precision,
                    System.Data.DbType.DateTime,
                    precision: precision))
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlDateTimeTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new MySqlDateTimeTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override CoreTypeMapping Clone(ValueConverter converter)
            => new MySqlDateTimeTypeMapping(Parameters.WithComposedConverter(converter));

        /// <summary>
        ///     Gets the string format to be used to generate SQL literals of this type.
        /// </summary>
        protected override string SqlLiteralFormatString
            => Parameters.Precision == null ? DateTimeFormatConst : DateTimeFormatConst6;
    }
}
