// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class BytesToDateTimeConverter : ValueConverter<byte[], DateTime>
    {
        private static readonly NumberToBytesConverter<long> _longToBytes
            = new NumberToBytesConverter<long>();

        public BytesToDateTimeConverter()
            : base(
                v => FromBytes(v),
                v => ToBytes(v))
        {
        }

        public static byte[] ToBytes(DateTime v)
            => (byte[])_longToBytes.ConvertToProvider(v.ToBinary());

        public static DateTime FromBytes(byte[] v)
            => DateTime.FromBinary((long)_longToBytes.ConvertFromProvider(v));
    }
}
