﻿using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    public sealed class SupportedServerVersionTheoryAttribute : TheoryAttribute
    {
        public SupportedServerVersionTheoryAttribute(params string[] versionsOrKeys)
            : this(ServerVersion.GetSupport(versionsOrKeys))
        {
        }

        private SupportedServerVersionTheoryAttribute(ServerVersionSupport serverVersionSupport)
        {
            var currentVersion = new ServerVersion(AppConfig.Config.GetSection("Data")["ServerVersion"]);

            if (!serverVersionSupport.IsSupported(currentVersion) && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on {serverVersionSupport} and higher.";
            }
        }
    }
}
