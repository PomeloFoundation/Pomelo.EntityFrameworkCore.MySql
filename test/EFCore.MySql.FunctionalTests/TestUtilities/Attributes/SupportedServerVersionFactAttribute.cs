using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    public sealed class SupportedServerVersionFactAttribute : FactAttribute
    {
        public SupportedServerVersionFactAttribute(params string[] versions)
            : this(new ServerVersionSupport(versions))
        {
        }

        private SupportedServerVersionFactAttribute(ServerVersionSupport serverVersionSupport)
        {
            var currentVersion = new ServerVersion(AppConfig.Config.GetSection("Data")["ServerVersion"]);

            if (!serverVersionSupport.IsSupported(currentVersion) && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported on {serverVersionSupport.SupportedServerVersions} and higher.";
            }
        }
    }
}
