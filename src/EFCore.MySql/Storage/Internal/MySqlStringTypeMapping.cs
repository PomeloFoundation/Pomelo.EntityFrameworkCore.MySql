// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

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
                    fixedLength),
                noBackslashEscapes)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlStringTypeMapping(RelationalTypeMappingParameters parameters, bool noBackslashEscapes)
            : base(parameters)
        {
            _maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
            _noBackslashEscapes = noBackslashEscapes;
        }

        private static int CalculateSize(bool unicode, int? size)
            => unicode
                ? size.HasValue && size <= UnicodeMax ? size.Value : UnicodeMax
                : size.HasValue && size <= AnsiMax ? size.Value : AnsiMax;

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlStringTypeMapping(parameters, _noBackslashEscapes);

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
            int? length;

            if (value is string stringValue)
            {
                length = stringValue.Length;
            }
            else if (value is byte[] byteArray)
            {
                length = byteArray.Length;
            }
            else
            {
                length = null;
            }
            
            parameter.Value = value;
            parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
                ? _maxSpecificSize
                : -1;
        }

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"'{EscapeSqlLiteral((string)value)}'";

        protected override string EscapeSqlLiteral(string literal)
            => EscapeBackslashes(base.EscapeSqlLiteral(literal));

        private string EscapeBackslashes(string literal)
        {
            return _noBackslashEscapes
                ? literal
                : literal.Replace(@"\", @"\\");
        }
    }
}
