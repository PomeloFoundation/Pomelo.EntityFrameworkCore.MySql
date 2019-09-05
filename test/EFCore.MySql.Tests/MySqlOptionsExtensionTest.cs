
using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public class MySqlOptionsExtensionTest
    {
        [Fact]
        public void GetServiceProviderHashCode_returns_same_value()
        {
            Assert.Equal(new MySqlOptionsExtension().Info.GetServiceProviderHashCode(), new MySqlOptionsExtension().Info.GetServiceProviderHashCode());

            Assert.Equal(new MySqlOptionsExtension()
                    .WithAnsiCharSetInfo(new CharSetInfo(CharSet.Latin1))
                    .WithCharSetBehavior(CharSetBehavior.AppendToAllColumns)
                    .WithUnicodeCharSetInfo(new CharSetInfo(CharSet.Utf8mb4))
                    .WithServerVersion(new ServerVersion(new Version(1, 2, 3, 4), ServerType.MySql))
                    .DisableBackslashEscaping()
                    .Info
                    .GetServiceProviderHashCode(),
                new MySqlOptionsExtension()
                    .WithAnsiCharSetInfo(new CharSetInfo(CharSet.Latin1))
                    .WithCharSetBehavior(CharSetBehavior.AppendToAllColumns)
                    .WithUnicodeCharSetInfo(new CharSetInfo(CharSet.Utf8mb4))
                    .WithServerVersion(new ServerVersion(new Version(1, 2, 3, 4), ServerType.MySql))
                    .DisableBackslashEscaping()
                    .Info
                    .GetServiceProviderHashCode());
        }
    }
}
