// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

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
        private readonly IMySqlOptions _options;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlStringTypeMapping(
            [NotNull] string storeType,
            DbType? dbType,
            IMySqlOptions options,
            bool unicode = false,
            int? size = null,
            bool fixedLength = false)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(string)),
                    storeType,
                    StoreTypePostfix.None, // Has to be None until EF #11896 is fixed
                    dbType,
                    unicode,
                    size,
                    fixedLength),
                options)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlStringTypeMapping(RelationalTypeMappingParameters parameters, IMySqlOptions options)
            : base(parameters)
        {
            _maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
            _options = options;
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
            => new MySqlStringTypeMapping(parameters, _options);

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
            => EscapeSqlLiteralWithLineBreaks((string)value);

        private string EscapeSqlLiteralWithLineBreaks(string value)
        {
            var escapedLiteral = $"'{EscapeSqlLiteral(value)}'";

            // BUG: EF Core indents idempotent scripts, which can lead to unexpected values for strings
            //      that contain line breaks.
            //      Tracked by: https://github.com/aspnet/EntityFrameworkCore/issues/15256
            //
            //      Convert line break characters to their CHAR() representation as a workaround.

            if (_options.ReplaceLineBreaksWithCharFunction
                && (value.Contains("\r") || value.Contains("\n")))
            {
                escapedLiteral = "CONCAT(" + escapedLiteral
                    .Replace("\r\n", "', CHAR(13, 10), '")
                    .Replace("\r", "', CHAR(13), '")
                    .Replace("\n", "', CHAR(10), '") + ")";
            }

            return escapedLiteral;
        }

        protected override string EscapeSqlLiteral(string literal)
            => EscapeBackslashes(base.EscapeSqlLiteral(literal));

        private string EscapeBackslashes(string literal)
        {
            return _options.NoBackslashEscapes
                ? literal
                : literal.Replace(@"\", @"\\");
        }
    }
}
