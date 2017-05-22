using System;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Migrations
{
    public class ServerVersionTest
    {
        [Theory]
        [InlineData("5.7.18", ServerType.MySql, "5.7.18")]
        [InlineData("5.5.5-10.1.23-MariaDB-1~jessie", ServerType.MariaDb, "10.1.23")]
        [InlineData("5.5.5-10.3.0-MariaDB-10.3.0+maria~jessie", ServerType.MariaDb, "10.3.0")]
        [InlineData("5.5.5-1.2.3-myslq~jessie", ServerType.MySql, "5.5.5")]
        [InlineData("version5.0", ServerType.MySql, "5.0")]
        [InlineData("5.1", ServerType.MySql, "5.1")]
        public void TestValidVersion(string input, ServerType serverType, string actualVersion)
        {
            var serverVersion = new ServerVersion(input);
            Assert.Equal(serverVersion.Type, serverType);
            Assert.Equal(serverVersion.Version, new Version(actualVersion));
        }

        [Fact]
        public void TestInvalidVersion()
        {
            Assert.Throws<InvalidOperationException>(() => new ServerVersion("unknown"));
        }
    }
}
