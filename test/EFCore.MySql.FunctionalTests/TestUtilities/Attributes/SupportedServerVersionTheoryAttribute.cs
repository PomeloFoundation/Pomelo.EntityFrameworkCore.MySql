using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    public sealed class SupportedServerVersionTheoryAttribute : TheoryAttribute
    {
        public SupportedServerVersionTheoryAttribute(params string[] versions)
            : this(new ServerVersionSupport(versions))
        {
        }
        
        private SupportedServerVersionTheoryAttribute(ServerVersionSupport serverVersionSupport)
        {
            var currentVersion = new ServerVersion(AppConfig.Config.GetSection("Data")["ServerVersion"]);

            if (!serverVersionSupport.IsSupported(currentVersion) && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported on {serverVersionSupport} and higher.";
            }
        }
    }
}
