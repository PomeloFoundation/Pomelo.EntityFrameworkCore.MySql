using System;
using Microsoft.EntityFrameworkCore;
using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql
{
    public class MySqlOptionsExtensionTest
    {
        [Fact]
        public void GetServiceProviderHashCode_returns_same_value()
        {
            Assert.Equal(
                new MySqlOptionsExtension().Info.GetServiceProviderHashCode(),
                new MySqlOptionsExtension().Info.GetServiceProviderHashCode());

            Assert.Equal(
                new MySqlOptionsExtension()
                    .WithServerVersion(new MySqlServerVersion(new Version(1, 2, 3, 4)))
                    .WithDisabledBackslashEscaping()
                    .Info
                    .GetServiceProviderHashCode(),
                new MySqlOptionsExtension()
                    .WithServerVersion(new MySqlServerVersion(new Version(1, 2, 3, 4)))
                    .WithDisabledBackslashEscaping()
                    .Info
                    .GetServiceProviderHashCode());
        }
    }
}
