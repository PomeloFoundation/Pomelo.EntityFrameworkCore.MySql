﻿using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Tests.Attributes
{
    public sealed class SupportedServerVersionFactAttribute : FactAttribute
    {
        public SupportedServerVersionFactAttribute(params string[] versionsOrKeys)
            : this(ServerVersion.GetSupport(versionsOrKeys))
        {
        }

        private SupportedServerVersionFactAttribute(ServerVersionSupport serverVersionSupport)
        {
            var currentVersion = AppConfig.ServerVersion;

            if (!serverVersionSupport.IsSupported(currentVersion) && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on {serverVersionSupport.SupportedServerVersions} and higher.";
            }
        }
    }
}
