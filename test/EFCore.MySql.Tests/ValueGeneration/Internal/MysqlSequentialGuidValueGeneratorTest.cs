using System;
using System.Linq;
using Moq;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.ValueGeneration.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.ValueGeneration.Internal
{
    public class MysqlSequentialGuidValueGeneratorTest
    {
        private MySqlSequentialGuidValueGenerator GetGenerator(bool mysqlGuidFormatIsLittleEndianBinary16) {
            var connectionSettings = new Mock<MySqlConnectionSettings>();
            connectionSettings.SetupGet(x => x.GuidFormat).Returns(mysqlGuidFormatIsLittleEndianBinary16 ? MySqlGuidFormat.LittleEndianBinary16 : MySqlGuidFormat.None);

            var options = new Mock<MySqlOptions>();
            options.SetupGet(x => x.ConnectionSettings).Returns(connectionSettings.Object);

            return new MySqlSequentialGuidValueGenerator(options.Object);
        }

        [Fact]
        public void Next_returns_new_Guid()
        {
            var guid = GetGenerator(false).Next();
            Assert.NotEqual(Guid.Empty, guid);
        }

        [Fact]
        public void Next_returns_new_Guid_LittleEndianBinary16()
        {
            var guid = GetGenerator(true).Next();
            Assert.NotEqual(Guid.Empty, guid);
        }

        [Fact]
        public void Next_returns_loosely_sorted_Guid()
        {
            // GUIDs are loosely sorted on time. When one GUID is generated at
            // least one tick (100 ns) after the last, it is sorted higher than
            // the previous one. This property gives us a performance benefit
            // on sorting in the database.
            var generator = GetGenerator(false);

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
        public void Next_returns_loosely_sorted_Guid_LittleEndianBinary16()
        {
            // GUIDs are loosely sorted on time. When one GUID is generated at
            // least one tick (100 ns) after the last, it is sorted higher than
            // the previous one. This property gives us a performance benefit
            // on sorting in the database.
            var generator = GetGenerator(true);

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
        public void Next_returns_valid_Rfc4122_random_Guid()
        {
            var generator = GetGenerator(false);

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
        public void Next_returns_valid_Rfc4122_random_Guid_LittleEndianBinary16()
        {
            var generator = GetGenerator(true);

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
        public void Next_returns_unique_Guid()
        {
            var generator = GetGenerator(false);

            var guids = Enumerable.Range(0, 100).Select(_ => generator.Next()).ToArray();
            Assert.Equal(guids.Distinct().Count(), guids.Length);
        }

        [Fact]
        public void Next_returns_unique_Guid_LittleEndianBinary16()
        {
            var generator = GetGenerator(true);

            var guids = Enumerable.Range(0, 100).Select(_ => generator.Next()).ToArray();
            Assert.Equal(guids.Distinct().Count(), guids.Length);
        }
    }
}
