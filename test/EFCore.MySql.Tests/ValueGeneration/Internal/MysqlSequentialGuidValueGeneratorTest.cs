using System;
using System.Linq;
using Moq;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.ValueGeneration.Internal
{
    public class MysqlSequentialGuidValueGeneratorTest
    {
        private MySqlSequentialGuidValueGenerator getGenerator(bool mysqlGuidFormatIsLittleEndianBinary16) {
            var connectionSettings = new Mock<MySqlConnectionSettings>();
            connectionSettings.SetupGet(x => x.GuidFormat).Returns(mysqlGuidFormatIsLittleEndianBinary16 ? MySqlGuidFormat.LittleEndianBinary16 : MySqlGuidFormat.None);

            var options = new Mock<MySqlOptions>();
            options.SetupGet(x => x.ConnectionSettings).Returns(connectionSettings.Object);

            return new MySqlSequentialGuidValueGenerator(options.Object);
        }

        [Fact]
        public void NextReturnsNewGuid()
        {
            var guid = getGenerator(false).Next();
            Assert.NotEqual(Guid.Empty, guid);
        }

        [Fact]
        public void NextReturnsNewGuidLEB16()
        {
            var guid = getGenerator(true).Next();
            Assert.NotEqual(Guid.Empty, guid);
        }

        [Fact]
        public void NextReturnsLooselySortedGuid()
        {
            // GUIDs are loosely sorted on time. When one GUID is generated at
            // least one tick (100 ns) after the last, it is sorted higher than
            // the previous one. This property gives us a performance benefit
            // on sorting in the database.
            var generator = getGenerator(false);

            var timeNow = DateTimeOffset.UtcNow;
            var lastGuid = generator.Next(timeNow);
            for(var i = 0; i < 100; ++i) {
                timeNow = timeNow.AddTicks(1);
                var guid = generator.Next(timeNow);
                Assert.True(string.Compare(guid.ToString(), lastGuid.ToString()) > 0);
                lastGuid = guid;
            }
        }

        [Fact]
        public void NextReturnsLooselySortedGuidLEB16()
        {
            // GUIDs are loosely sorted on time. When one GUID is generated at
            // least one tick (100 ns) after the last, it is sorted higher than
            // the previous one. This property gives us a performance benefit
            // on sorting in the database.
            var generator = getGenerator(true);

            var timeNow = DateTimeOffset.UtcNow;
            var lastGuid = generator.Next(timeNow);
            for(var i = 0; i < 100; ++i) {
                timeNow = timeNow.AddTicks(1);
                var guid = generator.Next(timeNow);
                Assert.True(string.Compare(guid.ToString(), lastGuid.ToString()) > 0);
                lastGuid = guid;
            }
        }

        [Fact]
        public void NextReturnsValidRFC4122RandomGuid()
        {
            var generator = getGenerator(false);

            // Since the values are based on randomness and time, perform this test
            // multiple times to increase the chance of finding incorrect values.
            for(var i = 0; i < 100; ++i) {
                var guid = generator.Next();
                var bytes = guid.ToByteArray();
                // Version, indicated by the 13th nibble, must be 4
                var version = bytes[7] >> 4;
                Assert.Equal(4, version);
                // Variant, indicated by the first bits of the 17th nibble, must be 0b10xx
                var variant = (bytes[8] >> 4) & 0b1100;
                Assert.Equal(0b1000, variant);
            }
        }

        [Fact]
        public void NextReturnsValidRFC4122RandomGuidLEB16()
        {
            var generator = getGenerator(true);

            // Since the values are based on randomness and time, perform this test
            // multiple times to increase the chance of finding incorrect values.
            for(var i = 0; i < 100; ++i) {
                var guid = generator.Next();
                var bytes = guid.ToByteArray();
                // Version, indicated by the 13th nibble, must be 4
                var version = bytes[7] >> 4;
                Assert.Equal(4, version);
                // Variant, indicated by the first bits of the 17th nibble, must be 0b10xx
                var variant = (bytes[8] >> 4) & 0b1100;
                Assert.Equal(0b1000, variant);
            }
        }

        [Fact]
        public void NextReturnsUniqueGuid()
        {
            var generator = getGenerator(false);

            var guids = Enumerable.Range(0, 100).Select(_ => generator.Next()).ToArray();
            Assert.Equal(guids.Distinct().Count(), guids.Length);
        }

        [Fact]
        public void NextReturnsUniqueGuidLEB16()
        {
            var generator = getGenerator(true);

            var guids = Enumerable.Range(0, 100).Select(_ => generator.Next()).ToArray();
            Assert.Equal(guids.Distinct().Count(), guids.Length);
        }
    }
}
