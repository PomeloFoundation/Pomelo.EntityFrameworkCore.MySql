using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations
{
    public class ServerVersionTest
    {
        [Theory]
        [InlineData("5.7.18", ServerType.MySql, "5.7.18", true)]
        [InlineData("5.5.5-10.1.23-MariaDB-1~jessie", ServerType.MariaDb, "10.1.23", false)]
        [InlineData("5.5.5-10.3.0-MariaDB-10.3.0+maria~jessie", ServerType.MariaDb, "10.3.0", false)]
        [InlineData("5.5.5-1.2.3-myslq~jessie", ServerType.MySql, "5.5.5", false)]
        [InlineData("version5.0", ServerType.MySql, "5.0", false)]
        [InlineData("5.1", ServerType.MySql, "5.1", false)]
        public void TestValidVersion(string input, ServerType serverType, string actualVersion, bool supportsRenameIndex)
        {
            var serverVersion = ServerVersion.Parse(input);
            Assert.Equal(serverVersion.Type, serverType);
            Assert.Equal(serverVersion.Version, new Version(actualVersion));
            Assert.Equal(serverVersion.Supports.RenameIndex, supportsRenameIndex);
        }

        [Theory]
        [InlineData("unknown")]
        [InlineData("8")]
        [InlineData("8-mysql")]
        public void TestInvalidVersion(string input)
        {
            Assert.Throws<InvalidOperationException>(() => ServerVersion.Parse("unknown"));
        }
    }
}
