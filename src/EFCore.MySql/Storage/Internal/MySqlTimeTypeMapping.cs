// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Linq.Expressions;
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
    public class MySqlTimeTypeMapping : RelationalTypeMapping, IDefaultValueCompatibilityAware
    {
        private readonly bool _isDefaultValueCompatible;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlTimeTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            int? precision = null,
            bool isDefaultValueCompatible = false)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(clrType),
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
        protected MySqlTimeTypeMapping(RelationalTypeMappingParameters parameters, bool isDefaultValueCompatible)
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
            => new MySqlTimeTypeMapping(parameters, _isDefaultValueCompatible);

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="isDefaultValueCompatible"> Use a default value compatible syntax, or not. </param>
        /// <returns> The newly created mapping. </returns>
        public virtual RelationalTypeMapping Clone(bool isDefaultValueCompatible = false)
            => new MySqlTimeTypeMapping(Parameters, isDefaultValueCompatible);

        /// <summary>
        ///     Generates the SQL representation of a non-null literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral([NotNull] object value)
        {
            // Custom TimeSpan formats do not handle the fraction point character as gracefully as System.DateTime does, and emit a trailing
            // decimal point even when there are no decimal places to output (probably a bug that is not going to be corrected in .NET).
            // TimeOnly only outputs a decimal point, if actual decimal places will be output as well.
            var literal = string.Format(CultureInfo.InvariantCulture, $"{{0:{GetTimeFormatString(value, Precision)}}}", value);
            return literal.EndsWith(".")
                ? $"{(_isDefaultValueCompatible ? null : "TIME ")}'{literal[..^1]}'"
                : $"{(_isDefaultValueCompatible ? null : "TIME ")}'{literal}'";
        }

        // TODO: Just implicitly rely on TIME_TRUNCATE_FRACTIONAL SQL mode and use a default-like implementation instead (check for
        //       fractions in the actual value and then use a format with fractions or without). Then use this format for any precision.
        //       Check support in different MySQL/MariaDB versions first.
        protected static string GetTimeFormatString(object value, int? precision)
        {
            var validPrecision = Math.Min(Math.Max(precision.GetValueOrDefault(), 0), 6);

            var format = value switch
            {
                TimeOnly => @"HH\:mm\:ss",
                TimeSpan => @"hh\:mm\:ss",
                _ => throw new InvalidCastException($"Can't generate time SQL literal for CLR type '{value.GetType()}'.")
            };

            return validPrecision > 0
                ? $@"{format}\.{new string('F', validPrecision)}"
                : format;
        }

        /// <summary>
        /// Workaround https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1513.
        /// CHECK: Remove once fixed upstream in EF Core.
        /// </summary>
        public override Expression GenerateCodeLiteral(object value)
        {
            if (value is not TimeOnly timeOnlyValue)
            {
                return base.GenerateCodeLiteral(value);
            }

            return timeOnlyValue.Ticks % 10000 > 0
                ? Expression.New(
                    typeof(TimeOnly).GetConstructor(new[] { typeof(long) })!,
                    Expression.Constant(timeOnlyValue.Ticks))
                : Expression.New(
                    typeof(TimeOnly).GetConstructor(
                        new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        })!,
                    Expression.Constant(timeOnlyValue.Hour),
                    Expression.Constant(timeOnlyValue.Minute),
                    Expression.Constant(timeOnlyValue.Second),
                    Expression.Constant(timeOnlyValue.Millisecond));
        }
    }
}
