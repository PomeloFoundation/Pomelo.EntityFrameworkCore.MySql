﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.ValueGeneration.Internal
{
  /// <summary>
    ///     Generates sequential <see cref="Guid" />
    ///     in MySQL Server. This is useful when entities are being saved to a database where sequential
    ///     GUIDs will provide a performance benefit. The generated values are non-temporary, meaning they will
    ///     be saved to the database.
    /// </summary>
    public class MySqlSequentialGuidValueGenerator : ValueGenerator<Guid>
    {
        
        /// <summary>
        ///     Gets a value to be assigned to a property.
        /// </summary>
        /// <para>The change tracking entry of the entity for which the value is being generated.</para>
        /// <returns> The value to be assigned to a property. </returns>
        public override Guid Next(EntityEntry entry)
        {
            return SequentialGuidGenerator.Instance.Create(SequentialGuidGenerator.SequentialGuidDatabaseType.MySql);
        }

        /// <summary>
        ///     Gets a value indicating whether the values generated are temporary or permanent. This implementation
        ///     always returns false, meaning the generated values will be saved to the database.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;
    }


    /// <summary>
    /// Implements <see cref="IGuidGenerator"/> by creating sequential Guids.
    /// This code is taken from https://github.com/jhtodd/SequentialGuid/blob/master/SequentialGuid/Classes/SequentialGuid.cs
    /// </summary>
    public class SequentialGuidGenerator 
    {
        /// <summary>
        /// Gets the singleton <see cref="SequentialGuidGenerator"/> instance.
        /// </summary>
        public static SequentialGuidGenerator Instance { get; } = new SequentialGuidGenerator();

        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public SequentialGuidDatabaseType DatabaseType { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="SequentialGuidGenerator"/> class from being created.
        /// Use <see cref="Instance"/>.
        /// </summary>
        private SequentialGuidGenerator()
        {
            DatabaseType = SequentialGuidDatabaseType.SqlServer;
        }

        public Guid Create()
        {
            return Create(DatabaseType);
        }

        public Guid Create(SequentialGuidDatabaseType databaseType)
        {
            switch (databaseType)
            {
                case SequentialGuidDatabaseType.SqlServer:
                    return Create(SequentialGuidType.SequentialAtEnd);
                case SequentialGuidDatabaseType.Oracle:
                    return Create(SequentialGuidType.SequentialAsBinary);
                case SequentialGuidDatabaseType.MySql:
                    return Create(SequentialGuidType.SequentialAsString);
                case SequentialGuidDatabaseType.PostgreSql:
                    return Create(SequentialGuidType.SequentialAsString);
                default:
                    throw new InvalidOperationException();
            }
        }

        public Guid Create(SequentialGuidType guidType)
        {
            // We start with 16 bytes of cryptographically strong random data.
            var randomBytes = new byte[10];
            Rng.GetBytes(randomBytes);

            // An alternate method: use a normally-created GUID to get our initial
            // random data:
            // byte[] randomBytes = Guid.NewGuid().ToByteArray();
            // This is faster than using RNGCryptoServiceProvider, but I don't
            // recommend it because the .NET Framework makes no guarantee of the
            // randomness of GUID data, and future versions (or different
            // implementations like Mono) might use a different method.

            // Now we have the random basis for our GUID.  Next, we need to
            // create the six-byte block which will be our timestamp.

            // We start with the number of milliseconds that have elapsed since
            // DateTime.MinValue.  This will form the timestamp.  There's no use
            // being more specific than milliseconds, since DateTime.Now has
            // limited resolution.

            // Using millisecond resolution for our 48-bit timestamp gives us
            // about 5900 years before the timestamp overflows and cycles.
            // Hopefully this should be sufficient for most purposes. :)
            long timestamp = DateTime.UtcNow.Ticks / 10000L;

            // Then get the bytes
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            // Since we're converting from an Int64, we have to reverse on
            // little-endian systems.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] guidBytes = new byte[16];

            switch (guidType)
            {
                case SequentialGuidType.SequentialAsString:
                case SequentialGuidType.SequentialAsBinary:

                    // For string and byte-array version, we copy the timestamp first, followed
                    // by the random data.
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    // If formatting as a string, we have to compensate for the fact
                    // that .NET regards the Data1 and Data2 block as an Int32 and an Int16,
                    // respectively.  That means that it switches the order on little-endian
                    // systems.  So again, we have to reverse.
                    if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }

                    break;

                case SequentialGuidType.SequentialAtEnd:

                    // For sequential-at-the-end versions, we copy the random data first,
                    // followed by the timestamp.
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            return new Guid(guidBytes);
        }

        /// <summary>
        /// Database type to generate GUIDs.
        /// </summary>
        public enum SequentialGuidDatabaseType
        {
            SqlServer,

            Oracle,

            MySql,

            PostgreSql,
        }

        /// <summary>
        /// Describes the type of a sequential GUID value.
        /// </summary>
        public enum SequentialGuidType
        {
            /// <summary>
            /// The GUID should be sequential when formatted using the
            /// <see cref="Guid.ToString()" /> method.
            /// </summary>
            SequentialAsString,

            /// <summary>
            /// The GUID should be sequential when formatted using the
            /// <see cref="Guid.ToByteArray" /> method.
            /// </summary>
            SequentialAsBinary,

            /// <summary>
            /// The sequential portion of the GUID should be located at the end
            /// of the Data4 block.
            /// </summary>
            SequentialAtEnd
        }
    }
}
