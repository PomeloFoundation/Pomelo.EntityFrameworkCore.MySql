using System;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    public sealed class SupportedServerVersionLessThanTheoryAttribute : TheoryAttribute
    {
        public SupportedServerVersionLessThanTheoryAttribute(string version)
            : this(new ServerVersion(version))
        {
        }

        public SupportedServerVersionLessThanTheoryAttribute(Version version)
            : this(new ServerVersion(version))
        {
        }

        private SupportedServerVersionLessThanTheoryAttribute(ServerVersion supportedVersion)
        {
            var currentVersion = new ServerVersion(AppConfig.Config.GetSection("Data")["ServerVersion"]);

            if (currentVersion.Version >= supportedVersion.Version && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on server versions lower than {supportedVersion.Version}.";
            }
        }
    }
}
