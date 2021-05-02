// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
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
    public class MySqlTimeSpanTypeMapping : TimeSpanTypeMapping, IDefaultValueCompatibilityAware
    {
        private readonly bool _isDefaultValueCompatible;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlTimeSpanTypeMapping(
            [NotNull] string storeType,
            int? precision = null,
            bool isDefaultValueCompatible = false)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(TimeSpan)),
                    storeType,
                    StoreTypePostfix.Precision,
                    System.Data.DbType.Time,
                    precision: precision),
                isDefaultValueCompatible)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlTimeSpanTypeMapping(RelationalTypeMappingParameters parameters, bool isDefaultValueCompatible)
            : base(parameters)
        {
            _isDefaultValueCompatible = isDefaultValueCompatible;
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlTimeSpanTypeMapping(parameters, _isDefaultValueCompatible);

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="isDefaultValueCompatible"> Use a default value compatible syntax, or not. </param>
        /// <returns> The newly created mapping. </returns>
        public virtual RelationalTypeMapping Clone(bool isDefaultValueCompatible = false)
            => new MySqlTimeSpanTypeMapping(Parameters, isDefaultValueCompatible);

        /// <summary>
        ///     Generates the SQL representation of a non-null literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral([NotNull] object value)
        {
            // Custom TimeSpan formats do not handle the fraction point character as gracefully as System.DateTime does.
            var literal = base.GenerateNonNullSqlLiteral(value);
            return literal.EndsWith(".")
                ? $"{(_isDefaultValueCompatible ? null : "TIME ")}'{literal[..^1]}'"
                : $"{(_isDefaultValueCompatible ? null : "TIME ")}'{literal}'";
        }

        /// <summary>
        ///     Gets the string format to be used to generate SQL literals of this type.
        /// </summary>
        protected override string SqlLiteralFormatString
            => $"{{0:{GetFormatString()}}}";

        public virtual string GetFormatString()
            => GetTimeSpanFormatString(Parameters.Precision);

        public static string GetTimeSpanFormatString(int? precision)
        {
            var validPrecision = Math.Min(Math.Max(precision.GetValueOrDefault(), 0), 6);
            var precisionFormat = validPrecision > 0
                ? @"\." + new string('F', validPrecision)
                : null;
            return @"hh\:mm\:ss" + precisionFormat;
        }
    }
}
