using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
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
