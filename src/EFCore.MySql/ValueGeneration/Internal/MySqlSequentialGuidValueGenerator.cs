﻿using System;
using System.Security.Cryptography;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal
{
    public class MySqlSequentialGuidValueGenerator  : ValueGenerator<Guid>
    {

        private readonly IMySqlOptions _options;

        public MySqlSequentialGuidValueGenerator(IMySqlOptions options)
        {
            _options = options;
        }

        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        /// <summary>
        ///     Gets a value to be assigned to a property.
        ///     Creates a GUID where the first 8 bytes are the current UTC date/time (in ticks)
        ///     and the last 8 bytes are cryptographically random.  This allows for better performance
        ///     in clustered index scenarios.
        /// </summary>
        /// <para>The change tracking entry of the entity for which the value is being generated.</para>
        /// <returns> The value to be assigned to a property. </returns>
        public override Guid Next(EntityEntry entry)
        {
            var randomBytes = new byte[8];
            _rng.GetBytes(randomBytes);
            var ticks = (ulong) DateTime.UtcNow.Ticks;

            if (_options.ConnectionSettings.GuidFormat == MySqlGuidFormat.LittleEndianBinary16)
            {
                var guidBytes = new byte[16];
                var tickBytes = BitConverter.GetBytes(ticks);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(tickBytes);
                }

                Buffer.BlockCopy(tickBytes, 0, guidBytes, 0, 8);
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 8, 8);

                return new Guid(guidBytes);
            }

            var guid = new Guid((uint) (ticks >> 32), (ushort) (ticks << 32 >> 48), (ushort) (ticks << 48 >> 48),
                randomBytes[0],
                randomBytes[1],
                randomBytes[2],
                randomBytes[3],
                randomBytes[4],
                randomBytes[5],
                randomBytes[6],
                randomBytes[7]);

            return guid;
        }

        /// <summary>
        ///     Gets a value indicating whether the values generated are temporary or permanent. This implementation
        ///     always returns false, meaning the generated values will be saved to the database.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;

    }
}
