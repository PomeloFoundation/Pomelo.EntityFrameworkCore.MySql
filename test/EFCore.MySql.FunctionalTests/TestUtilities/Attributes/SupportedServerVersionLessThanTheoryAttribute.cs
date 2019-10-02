﻿using System;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    public sealed class SupportedServerVersionLessThanTheoryAttribute : TheoryAttribute
    {
        public SupportedServerVersionLessThanTheoryAttribute(params string[] versions)
            : this(new ServerVersionSupport(versions))
        {
        }

        private SupportedServerVersionLessThanTheoryAttribute(ServerVersionSupport serverVersionSupport)
        {
            var currentVersion = new ServerVersion(AppConfig.Config.GetSection("Data")["ServerVersion"]);

            if (!serverVersionSupport.IsSupported(currentVersion) && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on versions lower than {serverVersionSupport}.";
            }
        }
    }
}
