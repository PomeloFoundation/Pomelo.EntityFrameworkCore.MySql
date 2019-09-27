using System;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    public sealed class SupportedServerVersionFactAttribute : FactAttribute
    {
        public SupportedServerVersionFactAttribute(string version)
            : this(new ServerVersion(version))
        {
        }

        public SupportedServerVersionFactAttribute(Version version)
            : this(new ServerVersion(version))
        {
        }

        private SupportedServerVersionFactAttribute(ServerVersion supportedVersion)
        {
            var currentVersion = new ServerVersion(AppConfig.Config.GetSection("Data")["ServerVersion"]);

            if (currentVersion.Version < supportedVersion.Version && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on {supportedVersion.Version} and higher.";
            }
        }
    }
}
