// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlStringTypeMapping : MySqlTypeMapping
    {
        private const int UnicodeMax = 4000;
        private const int AnsiMax = 8000;

        private readonly int _maxSpecificSize;
        private readonly IMySqlOptions _options;

        public bool IsUnquoted { get; }

        public MySqlStringTypeMapping(
            [NotNull] string storeType,
            IMySqlOptions options,
            bool unicode = true,
            int? size = null,
            bool fixedLength = false,
            bool unquoted = false)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(string)),
                    storeType,
                    StoreTypePostfix.None, // Has to be None until EF #11896 is fixed
                    unicode
                        ? fixedLength
                            ? System.Data.DbType.StringFixedLength
                            : System.Data.DbType.String
                        : fixedLength
                            ? System.Data.DbType.AnsiStringFixedLength
                            : System.Data.DbType.AnsiString,
                    unicode,
                    size,
                    fixedLength),
                fixedLength
                    ? MySqlDbType.String
                    : MySqlDbType.VarString,
                options,
                unquoted)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlStringTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            IMySqlOptions options,
            bool isUnquoted)
            : base(parameters, mySqlDbType)
        {
            _maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
            _options = options;
            IsUnquoted = isUnquoted;
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
            => new MySqlStringTypeMapping(parameters, MySqlDbType, _options, IsUnquoted);

        public virtual RelationalTypeMapping Clone(bool unquoted)
            => new MySqlStringTypeMapping(Parameters, MySqlDbType, _options, unquoted);

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
            => IsUnquoted
                ? EscapeSqlLiteral((string)value)
                : EscapeSqlLiteralWithLineBreaks((string)value, _options);

        public static string EscapeSqlLiteralWithLineBreaks(string value, IMySqlOptions options)
        {
            var escapedLiteral = $"'{EscapeSqlLiteral(value, options)}'";

            // BUG: EF Core indents idempotent scripts, which can lead to unexpected values for strings
            //      that contain line breaks.
            //      Tracked by: https://github.com/aspnet/EntityFrameworkCore/issues/15256
            //
            //      Convert line break characters to their CHAR() representation as a workaround.

            if (options.ReplaceLineBreaksWithCharFunction
                && (value.Contains("\r") || value.Contains("\n")))
            {
                escapedLiteral = "CONCAT(" + escapedLiteral
                    .Replace("\r\n", "', CHAR(13, 10), '")
                    .Replace("\r", "', CHAR(13), '")
                    .Replace("\n", "', CHAR(10), '") + ")";
            }

            return escapedLiteral;
        }

        protected virtual string EscapeSqlLiteral(string literal)
            => EscapeSqlLiteral(literal, _options);

        public static string EscapeSqlLiteral(string literal, IMySqlOptions options)
            => EscapeBackslashes(Check.NotNull(literal, nameof(literal)).Replace("'", "''"), options);

        public static string EscapeBackslashes(string literal, IMySqlOptions options)
        {
            return options.NoBackslashEscapes
                ? literal
                : literal.Replace(@"\", @"\\");
        }
    }
}
