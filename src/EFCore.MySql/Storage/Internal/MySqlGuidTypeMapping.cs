// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlGuidTypeMapping : GuidTypeMapping
    {
        private readonly MySqlGuidFormat _guidFormat;

        public MySqlGuidTypeMapping(MySqlGuidFormat guidFormat)
            : this(new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(Guid)),
                    GetStoreType(guidFormat),
                    StoreTypePostfix.Size,
                    System.Data.DbType.Guid,
                    false,
                    GetSize(guidFormat),
                    true),
                guidFormat)
        {
        }

        protected MySqlGuidTypeMapping(RelationalTypeMappingParameters parameters, MySqlGuidFormat guidFormat)
            : base(parameters)
        {
            _guidFormat = guidFormat;
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlGuidTypeMapping(parameters, _guidFormat);

        public virtual bool IsCharBasedStoreType
            => GetStoreType(_guidFormat) == "char";

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            switch (_guidFormat)
            {
                case MySqlGuidFormat.Char36:
                    return $"'{value:D}'";

                case MySqlGuidFormat.Char32:
                    return $"'{value:N}'";

                case MySqlGuidFormat.Binary16:
                case MySqlGuidFormat.TimeSwapBinary16:
                case MySqlGuidFormat.LittleEndianBinary16:
                    return ByteArrayFormatter.ToHex(GetBytesFromGuid(_guidFormat, (Guid)value));

                case MySqlGuidFormat.None:
                case MySqlGuidFormat.Default:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetStoreType(MySqlGuidFormat guidFormat)
        {
            switch (guidFormat)
            {
                case MySqlGuidFormat.Char36:
                case MySqlGuidFormat.Char32:
                    return "char";

                case MySqlGuidFormat.Binary16:
                case MySqlGuidFormat.TimeSwapBinary16:
                case MySqlGuidFormat.LittleEndianBinary16:
                    return "binary";

                case MySqlGuidFormat.None:
                case MySqlGuidFormat.Default:
                default:
                    throw new InvalidOperationException();
            }
        }

        private static int GetSize(MySqlGuidFormat guidFormat)
        {
            switch (guidFormat)
            {
                case MySqlGuidFormat.Char36:
                    return 36;

                case MySqlGuidFormat.Char32:
                    return 32;

                case MySqlGuidFormat.Binary16:
                case MySqlGuidFormat.TimeSwapBinary16:
                case MySqlGuidFormat.LittleEndianBinary16:
                    return 16;

                case MySqlGuidFormat.None:
                case MySqlGuidFormat.Default:
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool IsValidGuidFormat(MySqlGuidFormat guidFormat)
            => guidFormat != MySqlGuidFormat.None &&
               guidFormat != MySqlGuidFormat.Default;

        protected static byte[] GetBytesFromGuid(MySqlGuidFormat guidFormat, Guid guid)
        {
            var bytes = guid.ToByteArray();

            if (guidFormat == MySqlGuidFormat.Binary16)
            {
                return new[] { bytes[3], bytes[2], bytes[1], bytes[0], bytes[5], bytes[4], bytes[7], bytes[6], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15] };
            }

            if (guidFormat == MySqlGuidFormat.TimeSwapBinary16)
            {
                return new[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15] };
            }

            return bytes;
        }
    }
}
