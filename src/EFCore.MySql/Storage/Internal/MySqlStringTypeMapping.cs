// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlStringTypeMapping : StringTypeMapping
    {
        private const int UnicodeMax = 4000;
        private const int AnsiMax = 8000;

        private readonly int _maxSpecificSize;
        private readonly bool _noBackslashEscapes;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlStringTypeMapping(
            [NotNull] string storeType,
            DbType? dbType,
            bool unicode = false,
            int? size = null,
            bool fixedLength = false,
            bool noBackslashEscapes = false)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(string)),
                    storeType,
                    StoreTypePostfix.None, // Has to be None until EF #11896 is fixed
                    dbType,
                    unicode,
                    size,
                    fixedLength))
        {
            _noBackslashEscapes = noBackslashEscapes;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlStringTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
            _maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
        }

        private static int CalculateSize(bool unicode, int? size)
            => unicode
                ? size.HasValue && size <= UnicodeMax ? size.Value : UnicodeMax
                : size.HasValue && size <= AnsiMax ? size.Value : AnsiMax;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new MySqlStringTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override CoreTypeMapping Clone(ValueConverter converter)
            => new MySqlStringTypeMapping(Parameters.WithComposedConverter(converter));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            // For strings and byte arrays, set the max length to the size facet if specified, or
            // 8000 bytes if no size facet specified, if the data will fit so as to avoid query cache
            // fragmentation by setting lots of different Size values otherwise always set to
            // -1 (unbounded) to avoid size inference.

            var value = parameter.Value;
            var length = (value as string)?.Length ?? (value as byte[])?.Length;

            parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
                ? _maxSpecificSize
                : -1;
        }

        // eventually Unicode strings can be escaped with U'string'
        // see https://dev.mysql.com/worklog/task/?id=3529
        // until this change is made in MySQL, there is no way to escape unicode strings
        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral(object value)
            => IsUnicode
                ? $"'{EscapeSqlLiteral((string)value)}'" // Interpolation okay; strings
                : $"'{EscapeSqlLiteral((string)value)}'";

        protected override string EscapeSqlLiteral(string literal)
        {
            return _noBackslashEscapes
                ? base.EscapeSqlLiteral(literal)
                : base.EscapeSqlLiteral(literal).Replace("\\", "\\\\");
        }
    }
}
