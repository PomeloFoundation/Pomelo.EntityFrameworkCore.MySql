using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SupportedServerVersionConditionAttribute : Attribute, ITestCondition
    {
        protected ServerVersionSupport ServerVersionSupport { get; }

        public SupportedServerVersionConditionAttribute(params string[] versionsOrKeys)
        {
            ServerVersionSupport = ServerVersion.GetSupport(versionsOrKeys);
        }

        public virtual ValueTask<bool> IsMetAsync()
        {
            var currentVersion = AppConfig.ServerVersion;
            var isMet = ServerVersionSupport.IsSupported(currentVersion);

            if (!isMet && string.IsNullOrEmpty(Skip))
            {
                Skip = $"Test is supported only on {ServerVersionSupport.SupportedServerVersions} and higher.";
            }

            return new ValueTask<bool>(isMet);
        }

        public virtual string SkipReason => Skip;
        public virtual string Skip { get; set; }
    }
}
