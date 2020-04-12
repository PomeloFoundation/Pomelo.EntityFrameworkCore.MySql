// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class MySqlDateTimeOffsetTypeMapping : DateTimeOffsetTypeMapping
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlDateTimeOffsetTypeMapping(
            [NotNull] string storeType,
            int? precision = null)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(DateTimeOffset)),
                    storeType,
                    StoreTypePostfix.Precision,
                    System.Data.DbType.DateTimeOffset,
                    precision: precision))
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlDateTimeOffsetTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlDateTimeOffsetTypeMapping(parameters);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            if (Size.HasValue
                && Size.Value != -1)
            {
                parameter.Size = Size.Value;
            }
        }

        public override string GenerateProviderValueSqlLiteral([CanBeNull] object value)
            => value == null
                ? "NULL"
                : GenerateNonNullSqlLiteral(
                    value is DateTimeOffset dateTimeOffset
                        ? dateTimeOffset.UtcDateTime
                        : value);

        /// <summary>
        ///     Gets the string format to be used to generate SQL literals of this type.
        /// </summary>
        protected override string SqlLiteralFormatString
            => $"'{{0:{GetFormatString()}}}'";

        public virtual string GetFormatString()
            => GetDateTimeOffsetFormatString(Parameters.Precision);

        public static string GetDateTimeOffsetFormatString(int? precision)
        {
            var validPrecision = Math.Min(Math.Max(precision.GetValueOrDefault(), 0), 6);
            var precisionFormat = validPrecision > 0
                ? @"." + new string('F', validPrecision)
                : null;
            return @"yyyy-MM-dd HH\:mm\:ss" + precisionFormat;
        }
    }
}
