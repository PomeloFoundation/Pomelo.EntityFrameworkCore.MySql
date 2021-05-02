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
        private readonly bool _forceToString;
        private const int UnicodeMax = 4000;
        private const int AnsiMax = 8000;

        private readonly int _maxSpecificSize;
        private readonly IMySqlOptions _options;

        public virtual bool IsUnquoted { get; }
        public virtual bool IsNationalChar
            => StoreTypeNameBase.StartsWith("n") && StoreTypeNameBase.Contains("char");

        public MySqlStringTypeMapping(
            [NotNull] string storeType,
            IMySqlOptions options,
            StoreTypePostfix storeTypePostfix,
            bool unicode = true,
            int? size = null,
            bool fixedLength = false,
            bool unquoted = false,
            bool forceToString = false)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(string)),
                    storeType,
                    storeTypePostfix,
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
                unquoted,
                forceToString)
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
            bool isUnquoted,
            bool forceToString)
            : base(parameters, mySqlDbType)
        {
            _maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
            _options = options;
            _forceToString = forceToString;
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
            => new MySqlStringTypeMapping(parameters, MySqlDbType, _options, IsUnquoted, _forceToString);

        public virtual RelationalTypeMapping Clone(bool? unquoted = null, bool? forceToString = null)
            => new MySqlStringTypeMapping(Parameters, MySqlDbType, _options, unquoted ?? IsUnquoted, forceToString ?? _forceToString);

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
            if (_forceToString && value != null && value != DBNull.Value)
            {
                value = value.ToString();
            }

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

            parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
                ? _maxSpecificSize
                : -1;

            if (parameter.Value != value)
            {
                parameter.Value = value;
            }
        }

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            var stringValue = _forceToString
                ? value.ToString()
                : (string)value;

            return IsUnquoted
                ? EscapeSqlLiteral(stringValue)
                : EscapeSqlLiteralWithLineBreaks(stringValue, _options);
        }

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
