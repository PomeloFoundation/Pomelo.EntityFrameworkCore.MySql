using System;
using System.Security.Cryptography;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MySqlConnector;

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
        ///     and the rest are cryptographically random. This allows for better performance
        ///     in clustered index scenarios.
        /// </summary>
        /// <para>The change tracking entry of the entity for which the value is being generated.</para>
        /// <returns> The value to be assigned to a property. </returns>
        public override Guid Next(EntityEntry entry)
        {
            return Next();
        }

        public Guid Next()
        {
            return Next(DateTimeOffset.UtcNow);
        }

        public Guid Next(DateTimeOffset timeNow)
        {
            // According to RFC 4122:
            // dddddddd-dddd-Mddd-Ndrr-rrrrrrrrrrrr
            // - M = RFC version, in this case '4' for random UUID
            // - N = RFC variant (plus other bits), in this case 0b1000 for variant 1
            // - d = nibbles based on UTC date/time in ticks
            // - r = nibbles based on random bytes

            var randomBytes = new byte[7];
            _rng.GetBytes(randomBytes);
            var ticks = (ulong) timeNow.Ticks;

            var uuidVersion = (ushort) 4;
            var uuidVariant = (ushort) 0b1000;

            var ticksAndVersion = (ushort)((ticks << 48 >> 52) | (ushort)(uuidVersion << 12));
            var ticksAndVariant = (byte)  ((ticks << 60 >> 60) | (byte)  (uuidVariant << 4));

            if (_options.ConnectionSettings.GuidFormat == MySqlGuidFormat.LittleEndianBinary16)
            {
                var guidBytes = new byte[16];
                var tickBytes = BitConverter.GetBytes(ticks);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(tickBytes);
                }

                Buffer.BlockCopy(tickBytes, 0, guidBytes, 0, 6);
                guidBytes[6] = (byte)(ticksAndVersion << 8 >> 8);
                guidBytes[7] = (byte)(ticksAndVersion >> 8);
                guidBytes[8] = ticksAndVariant;
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 9, 7);

                return new Guid(guidBytes);
            }

            var guid = new Guid((uint) (ticks >> 32), (ushort) (ticks << 32 >> 48), ticksAndVersion,
                ticksAndVariant,
                randomBytes[0],
                randomBytes[1],
                randomBytes[2],
                randomBytes[3],
                randomBytes[4],
                randomBytes[5],
                randomBytes[6]);

            return guid;
        }

        /// <summary>
        ///     Gets a value indicating whether the values generated are temporary or permanent. This implementation
        ///     always returns false, meaning the generated values will be saved to the database.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;

    }
}
